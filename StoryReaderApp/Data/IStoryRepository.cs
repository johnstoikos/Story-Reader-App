
using System.Collections.Generic;
using System.Data;
using StoryReaderApp.Models;

namespace StoryReaderApp.Data
{
    public interface IStoryRepository
    {
        IEnumerable<string> GetCategories();
        IEnumerable<Story> GetAll();
        IEnumerable<Story> GetByCategory(string category);
        int Create(Story s);
        void Update(Story s);
        void Delete(int id);
        IDbConnection OpenConnection();
    }
}