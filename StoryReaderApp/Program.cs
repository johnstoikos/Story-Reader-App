using System;
using System.Windows.Forms;
using StoryReaderApp.Data;
using StoryReaderApp.Services;
using StoryReaderApp.UI;

namespace StoryReaderApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            DbFactory.EnsureCreated();


            IStoryRepository repo = new StoryRepositorySqlite();
            IStoryService storyService = new StoryService(repo);
            IStoryInfoService infoService = new StoryInfoService(); 

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(storyService, infoService));
        }
    }
}
