using System.Collections.Generic;
using System.Linq;
using StoryReaderApp.Data;
using StoryReaderApp.Models;

namespace StoryReaderApp.Services
{
    public class StoryService : IStoryService
    {
        private readonly IStoryRepository _repo;
        public StoryService(IStoryRepository repo) { _repo = repo; }

        public IEnumerable<string> GetCategoriesWithAllOption(string allLabel)
        {
            var cats = _repo.GetCategories();
            return new[] { allLabel }.Concat(cats);
        }

        public IEnumerable<Story> GetStories(string categoryOrAll, string allLabel)
        {
            return categoryOrAll == allLabel ? _repo.GetAll() : _repo.GetByCategory(categoryOrAll);
        }

        public int Add(Story s) { return _repo.Create(s); }
        public void Edit(Story s) { _repo.Update(s); }
        public void Remove(int id) { _repo.Delete(id); }
    }
}
