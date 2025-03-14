using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UCParser.Interface;

namespace UCParser.Core
{
    public class ProcessHelper : IProcessHelper
    {
        private readonly ILogger _logger;

        public ProcessHelper(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> FindManagedFolderAsync(string processName)
        {
            return await Task.Run(() => {
                try
                {
                    Process[] processes = Process.GetProcessesByName(processName);
                    if (processes.Length == 0)
                    {
                        _logger.LogInfo($"Process {processName}.exe not found");
                        return null;
                    }
                    
                    Process targetProcess = processes[0];
                    _logger.LogInfo($"Process {processName}.exe (ID: {targetProcess.Id}) was found");
                    
                    string exePath = targetProcess.MainModule.FileName;
                    string gameFolder = Path.GetDirectoryName(exePath);
                    string managedFolder = Path.Combine(gameFolder, $"{processName}_Data", "Managed");
                    
                    if (!Directory.Exists(managedFolder))
                    {
                        _logger.LogInfo($"Folder {managedFolder} not found");
                        
                        managedFolder = Path.Combine(gameFolder, "Managed");
                        if (!Directory.Exists(managedFolder))
                        {
                            _logger.LogInfo($"Folder {managedFolder} not found");
                            return null;
                        }
                    }
                    
                    _logger.LogInfo($"Found folder: {managedFolder}");
                    return managedFolder;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while searching for a process: {ex.Message}");
                    return null;
                }
            });
        }
    }
}
