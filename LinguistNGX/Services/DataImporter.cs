
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using Xamarin.Essentials;
using Xamarin.Forms;

//using Android.Util.Log;

// TODO: CAW - Not sure where the best place to put this is
namespace LinguistNGX.Services
{
    public partial class DataImporter
    {
        public static void ImportDictionary(DatabaseContext db, string name)
        {
            bool entryValid;
            string message;

            try
            {
                string qualifiedName = Path.Combine(FileSystem.AppDataDirectory, name);

                /*IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
                var stream = new IsolatedStorageFileStream(name, FileMode.OpenOrCreate, store);*/

                var stream = new FileStream(qualifiedName, FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(stream);

                var signature = reader.ReadLine();

                if (signature == "*LINGUIST*")
                {
                    char[] delimiters = { '=' };
                    int index;
                    string line;
                    string groupName = "Default";
                    Group group = new Group { Name = groupName };

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            if (line[0] == '[')
                            {
                                if ((index = line.IndexOf(']')) != -1)
                                {
                                    groupName = line.Substring(1, (index - 1));
                                    group = new Group { Name = groupName };
                                }
                            }
                            else
                            {
                                string[] words = line.Split(delimiters);

                                // Ensure that the line is valid;  It must contain at least two words, delimited
                                // by an "=" sign
                                entryValid = false;

                                if (words.Length > 1)
                                {
                                    words[0] = words[0].Trim();
                                    words[1] = words[1].Trim();

                                    if ((words[0].Length > 0) && (words[1].Length > 0))
                                    {
                                        entryValid = true;
                                    }
                                }

                                // Add the entry, if it was found to be valid
                                if (entryValid)
                                {
                                    Entry newEntry = new Entry
                                    {
                                        Foreign = words[0].Trim(),
                                        English = words[1].Trim(),
                                        Group = group
                                    };

                                    db.Add(newEntry);
                                }
                                // Otherwise display an error advising that the entry is invalid
                                else
                                {
                                    message = System.String.Format("Invalid entry found:\n{0}", line);
                                    //DisplayAlert("Error", message, "OK");
                                }
                            }
                        }
                    }
                }

                reader.Close();
            }
            catch (Exception exception)
            {
                message = System.String.Format("Unable to import dictionary:\n{0}", exception.Message);
                //MessageBox.Show(message, "Error", MessageBoxButton.OK);
            }

            db.SaveChanges();
        }
    }
}
