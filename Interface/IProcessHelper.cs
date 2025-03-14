using System.Threading.Tasks;

namespace UCParser.Interface
{
    public interface IProcessHelper
    {
        Task<string> FindManagedFolderAsync(string processName);
    }
}
