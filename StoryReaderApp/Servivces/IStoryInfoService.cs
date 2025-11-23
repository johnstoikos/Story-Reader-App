using System.Threading.Tasks;
using StoryReaderApp.Models;

namespace StoryReaderApp.Services
{
    public interface IStoryInfoService
    {
        Task<StoryInfo> GetInfoAsync(string title);
    }
}
