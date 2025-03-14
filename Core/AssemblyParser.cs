using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UCParser.Interface;
using UCParser.Models;
using UCParser.Utils;

namespace UCParser.Core
{
    public class AssemblyParser : IAssemblyParser
    {
        private readonly ILogger _logger;
        private readonly IProcessHelper _processHelper;
        private readonly string _parsedFolderPath;
        private string _appFolderPath;
        private const string DEFAULT_PROCESS_NAME = "REPO";

        public AssemblyParser(ILogger logger, IProcessHelper processHelper)
        {
            _logger = logger;
            _processHelper = processHelper;

            _parsedFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Parsed");
            FileSystemHelper.EnsureDirectoryExists(_parsedFolderPath);
        }

        public async Task RunAsync()
        {
            Console.WriteLine("UCParser - Unity Class Parser by kimuru");
            Console.WriteLine($"Logs saved at: {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ucparser.txt")}");

            try
            {
                string processName = PromptForInput("Process Name: ", DEFAULT_PROCESS_NAME);

                _appFolderPath = Path.Combine(_parsedFolderPath, processName);
                FileSystemHelper.EnsureDirectoryExists(_appFolderPath);

                Console.WriteLine($"Result path: {_appFolderPath}");

                string managedFolder = await _processHelper.FindManagedFolderAsync(processName);

                if (string.IsNullOrEmpty(managedFolder))
                {
                    string managedPath = PromptForInput("Managed path: ");

                    if (string.IsNullOrWhiteSpace(managedPath) || !Directory.Exists(managedPath))
                    {
                        _logger.LogError("Wrong Managed path");
                        WaitForKeyPress();
                        return;
                    }

                    managedFolder = managedPath;
                }

                string targetClassName = PromptForInput("Class name: ");
                string targetMethodName = PromptForInput("Method name: ");

                string assemblyPath = Path.Combine(managedFolder, "Assembly-CSharp.dll");
                await ParseAssemblyAsync(assemblyPath, targetClassName, targetMethodName);

                Console.WriteLine($"Analysis completed. The results are saved in {Path.Combine(Path.GetTempPath(), "ucparser.txt")}");
                Console.WriteLine($"The results by class are saved in {_appFolderPath}.");
                Console.WriteLine("Press any key to exit...");
                WaitForKeyPress();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                _logger.LogError(ex.StackTrace);
                WaitForKeyPress();
            }
        }

        public async Task ParseAssemblyAsync(string assemblyPath, string targetClassName = "", string targetMethodName = "")
        {
            try
            {
                if (!File.Exists(assemblyPath))
                {
                    _logger.LogError($"{assemblyPath} not found");
                    return;
                }

                _logger.LogInfo($"Found file: {assemblyPath}");

                string managedFolder = Path.GetDirectoryName(assemblyPath);
                ConfigureAssemblyResolver(managedFolder);

                Assembly assembly = await LoadAssemblyAsync(assemblyPath);
                _logger.LogInfo($"Assembly {Path.GetFileName(assemblyPath)} has been loaded");

                Type[] types = GetTypesFromAssembly(assembly);
                _logger.LogInfo($"Found {types.Length} types in the assembly");

                await ProcessTypesAsync(types, targetClassName, targetMethodName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during assembly analysis: {ex.Message}");
                _logger.LogError(ex.StackTrace);
            }
        }

        private async Task ProcessTypesAsync(Type[] types, string targetClassName, string targetMethodName)
        {
            int methodCount = 0;
            int classCount = 0;
            var tasks = new List<Task>();

            foreach (Type type in types)
            {
                if (ShouldSkipType(type, targetClassName))
                    continue;

                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                  BindingFlags.Instance | BindingFlags.Static |
                                                  BindingFlags.DeclaredOnly);

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                           BindingFlags.Instance | BindingFlags.Static |
                                           BindingFlags.DeclaredOnly);

                if (methods.Length == 0 && fields.Length == 0)
                    continue;

                _logger.LogInfo($"Тип: {type.FullName}");

                string safeClassName = FileSystemHelper.GetSafeFileName(type.Name);
                string classFilePath = Path.Combine(_appFolderPath, $"{safeClassName}.txt");

                var typeInfo = new StringBuilder();
                typeInfo.AppendLine($"Class: {type.FullName}");
                typeInfo.AppendLine($"Token: 0x{type.MetadataToken:X}");
                typeInfo.AppendLine();
                typeInfo.AppendLine("Methods:");
                typeInfo.AppendLine("=======");

                int methodsFound = ProcessMethods(methods, targetMethodName, typeInfo);
                methodCount += methodsFound;

                typeInfo.AppendLine();
                typeInfo.AppendLine("Fields:");
                typeInfo.AppendLine("=====");

                ProcessFields(fields, typeInfo);

                if (typeInfo.ToString().Contains("Method:") || typeInfo.ToString().Contains("Field:"))
                {
                    tasks.Add(FileSystemHelper.WriteAllTextAsync(classFilePath, typeInfo.ToString()));
                    classCount++;
                }
            }

            await Task.WhenAll(tasks);
            _logger.LogInfo($"Total { methodCount} methods found in { classCount} classes");
            _logger.LogInfo($"Results saved in { _appFolderPath}");
        }

        private int ProcessMethods(MethodInfo[] methods, string targetMethodName, StringBuilder typeInfo)
        {
            int methodCount = 0;

            foreach (MethodInfo method in methods)
            {
                if (!string.IsNullOrWhiteSpace(targetMethodName) && !method.Name.Contains(targetMethodName))
                    continue;

                if (ReflectionHelper.IsAutoGeneratedMethod(method))
                    continue;

                string signature = ReflectionHelper.GetMethodSignature(method);
                string methodInfo = $"Method: {signature}, Token: 0x{method.MetadataToken:X}";

                _logger.LogInfo(methodInfo);
                typeInfo.AppendLine(methodInfo);
                methodCount++;
            }

            return methodCount;
        }

        private void ProcessFields(FieldInfo[] fields, StringBuilder typeInfo)
        {
            foreach (FieldInfo field in fields)
            {
                if (ReflectionHelper.IsAutoGeneratedField(field))
                    continue;

                string signature = ReflectionHelper.GetFieldSignature(field);
                string fieldInfo = $"{signature}, Token: 0x{field.MetadataToken:X}";

                _logger.LogInfo(fieldInfo);
                typeInfo.AppendLine(fieldInfo);
            }
        }

        private Task<Assembly> LoadAssemblyAsync(string assemblyPath)
        {
            return Task.Run(() => Assembly.LoadFrom(assemblyPath));
        }

        private Type[] GetTypesFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    _logger.LogError($"Loader exception: {loaderException.Message}");
                }
                return ex.Types.Where(t => t != null).ToArray();
            }
        }

        private void ConfigureAssemblyResolver(string managedFolder)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                string assemblyName = new AssemblyName(args.Name).Name;
                string assemblyDll = Path.Combine(managedFolder, assemblyName + ".dll");

                if (File.Exists(assemblyDll))
                {
                    return Assembly.LoadFrom(assemblyDll);
                }

                return null;
            };
        }

        private bool ShouldSkipType(Type type, string targetClassName)
        {
            if (!string.IsNullOrWhiteSpace(targetClassName) &&
                !type.Name.Contains(targetClassName) &&
                !type.FullName.Contains(targetClassName))
            {
                return true;
            }

            return ReflectionHelper.IsSystemType(type);
        }

        private string PromptForInput(string prompt, string defaultValue = "")
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }

        private void WaitForKeyPress()
        {
            Console.ReadKey();
        }
    }
}