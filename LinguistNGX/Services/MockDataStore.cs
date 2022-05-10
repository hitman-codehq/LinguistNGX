
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;
using LinguistNGX.Models;

namespace LinguistNGX.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        // Written: Sunday 27-Mar-2022 2:00 pm, Qatar Airlines flight QA 60 from Munich to Doha

        // TODO: CAW - Rename this
        public MockDataStore()
        {
            /*if (Debugger.IsAttached) return Android.OS.Environment.ExternalStorageDirectory.Path;
            else return Application.Context.FilesDir.Path;*/

            /*var dirs = Directory.EnumerateDirectories(Path.Combine(FileSystem.AppDataDirectory, ".config/.isolated-storage"));

            System.Console.WriteLine("*** AppDataDirectory/.config/.isolated-storage:");

            foreach (var dir in dirs)
            {
                System.Console.WriteLine(dir);
                //Log.Error(dir);
            }*/

            var files = Directory.EnumerateFiles(FileSystem.AppDataDirectory);

            items = new List<Item>();

            // TODO: CAW - Think of a better place to do this and only do it if the database does not already exist
            foreach (var file in files)
            {
                var FileName = Path.GetFileName(file);
                System.Console.WriteLine(FileName);

                if (Path.GetExtension(FileName) == ".lng")
                {
                    var databaseName = FileName + ".db3"; // TODO: CAW - Naming

                    if (!DatabaseContext.DatabaseExists(databaseName))
                    {
                        // Create an instance of the data context that can be used for initialising the words database
                        using (DatabaseContext db = new DatabaseContext(databaseName))
                        {
                            DataImporter.ImportDictionary(db, FileName);
                        }
                    }
                }
            }

            foreach (var file in files)
            {
                var FileName = Path.GetFileName(file);

                if ((Path.GetExtension(FileName) == ".db3") && (FileName != "Scratch.db3"))
                {
                    items.Add(new Item { Id = Guid.NewGuid().ToString(), Text = Path.GetFileName(file), Description = "Linguist Dictionary" });
                }
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}
