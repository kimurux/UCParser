using System;
using System.Threading.Tasks;
using UCParser.Core;
using UCParser.Interface;

namespace UCParser
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ucparser.txt");
                ILogger logger = new FileLogger(logPath);
                IProcessHelper processHelper = new ProcessHelper(logger);
                var parser = new AssemblyParser(logger, processHelper);
                await parser.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}