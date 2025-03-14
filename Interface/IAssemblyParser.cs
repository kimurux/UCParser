using System.Threading.Tasks;

namespace UCParser.Interface
{
    public interface IAssemblyParser
    {
        Task RunAsync();
        Task ParseAssemblyAsync(string assemblyPath, string targetClassName = "", string targetMethodName = "");
    }
}
