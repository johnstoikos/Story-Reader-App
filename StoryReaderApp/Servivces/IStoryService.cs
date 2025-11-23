using System.Collections.Generic;
using StoryReaderApp.Models;

namespace StoryReaderApp.Services
{
    public interface IStoryService
    {
        IEnumerable<string> GetCategoriesWithAllOption(string allLabel);
        IEnumerable<Story> GetStories(string categoryOrAll, string allLabel);
        int Add(Story s);
        void Edit(Story s);
        void Remove(int id);
    }
}
