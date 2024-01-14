
using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using LinguistNGX.Services;
using LinguistNGX.Views; // TODO: CAW - Illegal updward dependency

// TODO: CAW - Rename this?
namespace LinguistNGX.ViewModels
{
    public class ViewModel
    {
        public bool groupSet = false;
        private string groupName = String.Empty;
        private string subWord = String.Empty;
        private DatabaseContext dataContext;
        private DatabaseContext scratchDataContext;
        private Entry lastTest;
        private ObservableCollection<Group> groups;
        private Random random;
        public int searchEntryIndex = 0;
        public TestTypes testType = TestTypes.All;

        // These are the collections that are used for testing.  testItems is populated at the same time as allItems and is either
        // a copy of that collection (if all groups are being tested) or contains only the copy of a single group (if only a
        // single group is being tested).  When a test is performed, a copy of this collection is made into currentTestItems, and
        // and as the test progresses items are removed from the collection until there are zero left, to ensure that each item is
        // tested only once.  Because the test collections often contain less than the entire set of words, we need these copy
        // collections so that functionalty such as search still works on the entire collection, as held in allItems
        private ObservableCollection<Entry> allItems;
        private ObservableCollection<Entry> testItems;
        private ObservableCollection<Entry> currentTestItems;
        private ObservableCollection<Entry> scratchItems;

        public ViewModel(string connectionString, string scratchConnectionString)
        {
            dataContext = new DatabaseContext(connectionString);
            scratchDataContext = new DatabaseContext(scratchConnectionString);
            lastTest = new Entry();
            random = new Random();
            scratchItems = new ObservableCollection<Entry>();

            // Determine the name of the group that was last being tested (if any), so that we can continue
            // testing that group

            /*IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            // If the persistent settings object contains a "group" entry then load it
            if (settings.Contains("group"))
            {
                groupName = (string) settings["group"];

                // The flag that indicates that we are currently testing a group is persistent only for the
                // lifetime of the application, so it is stored in the state dictionary.  Load it if it exists
                if (PhoneApplicationService.Current.State.ContainsKey("testType"))
                {
                    testType = (TestTypes) PhoneApplicationService.Current.State["testType"];
                }
            }*/
        }

        public void AddItem(Entry newItem)
        {
            // Udate the collection used for counting and searching
            allItems.Add(newItem);

            // And add the new item to the Items database and submit it now
            /*dataContext.Items.InsertOnSubmit(newItem);
            dataContext.SubmitChanges();*/
        }

        // Written: Friday 21-Nov-2014 7:13 am, Code HQ Ehinger Tor

        public bool AddItemToScratchCollection(Entry newItem)
        {
            bool retVal = false;
            Group group = new Group { Name = newItem.Group.Name };

            Entry newEntry = new Entry
            {
                Foreign = newItem.Foreign,
                English = newItem.English,
                Group = group
            };

            // Add the item to the collection used for testing the scratch group, if it is not already in
            // the collection
            if (scratchItems.IndexOf(newEntry) == -1)
            {
                retVal = true;
                scratchItems.Add(newEntry);

                // And add the new item to the scratch database and submit it now
                /*scratchDataContext.Items.InsertOnSubmit(newEntry);
                scratchDataContext.SubmitChanges();*/
            }

            return retVal;
        }

        // Written: Tuesday 25-Nov-2014 6:35 am, Code HQ Ehinger Tor

        public void ClearScratchCollection()
        {
            // Clear any items that have been added to the scratch collection
            scratchItems.Clear();

            // If we are currently testing the scratch group then the items will also be in the current testing
            // collection.  Also clear these so that an empty Entry is returned when GetEntry() is called
            if (testType == TestTypes.Scratch)
            {
                currentTestItems.Clear();
                testItems.Clear();
            }

            // Perform a query that comprises all items in the scratch database
            /*var itemsInDatabase = from Entry item in scratchDataContext.Items
                                  select item;

            // And delete the items from the scratch database and submit it now
            scratchDataContext.Items.DeleteAllOnSubmit(itemsInDatabase);
            scratchDataContext.SubmitChanges();*/
        }

        public void DeleteItem(Entry deleteItem)
        {
            // Update the collection used for counting and searching
            allItems.Remove(deleteItem);

            // And delete the item from the Items database and submit it now
            /*dataContext.Items.DeleteOnSubmit(deleteItem);
            dataContext.SubmitChanges();*/
        }

        public int GetCount()
        {
            return allItems.Count;
        }

        // Written: Tuesday 01-Mar-2016 6:52 am, Code HQ Ehinger Tor

        public int GetTestCount()
        {
            return testItems.Count;
        }

        // Written: Wednesday 10-Sep-2014 7:04 am, Code HQ Ehinger Tor

        public Entry GetEntry()
        {
            bool foundUnique;
            int index;
            Entry retVal;

            // If all words have been tested, generate a new test
            if ((currentTestItems == null) || (currentTestItems.Count == 0))
            {
                currentTestItems = new ObservableCollection<Entry>(testItems);
            }

            // There is nothing to prevent a user from trying to do a test when there is no data
            // available so check for this
            if (currentTestItems.Count > 0)
            {
                // Loop around randomly choosing words until one is found that is different to the last one returned
                foundUnique = false;

                do
                {
                    // Choose an entry at random
                    index = random.Next(currentTestItems.Count);
                    retVal = currentTestItems[index];

                    // If an entry was previously returned, make sure that it was different to the one just chosen,
                    // in case the test was just regenerated and the chosen entry happens to be the same as the
                    // last one returned for the previous test.  Only perform this check if there is more than one
                    // entry in the group, or we will loop infinitely, checking the same entry forever!
                    if ((retVal != lastTest) || (currentTestItems.Count == 1))
                    {
                        foundUnique = true;
                        currentTestItems.RemoveAt(index);
                        lastTest = retVal;
                    }
                }
                while (!foundUnique);
            }

            // No data is available so return an empty entry
            else
            {
                retVal = new Entry();
            }

            return retVal;
        }

        public void LoadCollections()
        {
            // Perform a query that comprises all items in the database, and put them into a collection that
            // will never change during the lifetime of the application
            var itemsInDatabase = from Entry item in dataContext.Entries
                                  select item;

            allItems = new ObservableCollection<Entry>(itemsInDatabase);

            // If only a subset of entries is to be tested then perform a second query to find those entries,
            // and put them into a second collection for testing
            if ((testType == TestTypes.Group) && (groupName != String.Empty))
            {
                itemsInDatabase = from Entry item in dataContext.Entries
                                  where item.Group.Name == groupName
                                  select item;

                testItems = new ObservableCollection<Entry>(itemsInDatabase);
            }

            // If we are testing a subword then query the Items database for any entries that contain that word
            // in either the foreign or English fields
            else if (testType == TestTypes.SubWords)
            {
                itemsInDatabase = from Entry item in dataContext.Entries
                                  where item.Foreign.Contains(subWord) || item.English.Contains(subWord)
                                  select item;

                testItems = new ObservableCollection<Entry>(itemsInDatabase);
            }

            // If the scratch group is to be tested then make a copy of its collection
            else if (testType == TestTypes.Scratch)
            {
                testItems = new ObservableCollection<Entry>(scratchItems);
            }

            // All entries in the database are being tested so simply make a copy of the allItems collection
            else
            {
                testItems = new ObservableCollection<Entry>(allItems);
            }

            currentTestItems = null;

            // Now query the database to find a list of available groups.  These will be displayed by pages
            // such as Entry and SelectGroup
            var groupsInDatabase = from Group target in dataContext.Groups
                                   orderby target.Name
                                   select target;

            groups = new ObservableCollection<Group>(groupsInDatabase);

            // Perform a query that comprises all items in the scratch database, and put them into a collection
            // can be used for testing and modifying said scratch database
            itemsInDatabase = from Entry item in scratchDataContext.Entries
                              select item;

            scratchItems = new ObservableCollection<Entry>(itemsInDatabase);
        }

        // Written: Tuesday 04-Oct-2014 6:44 am, Code HQ Ehinger Tor

        public void SaveSettings(int direction)
        {
            // Save the name of the group and the results displaying direction, so that testing of the group can be
            // continued when the program is reloaded
            /*IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            settings.Clear();
            settings.Add("group", groupName);
            settings.Add("direction", direction);*/
        }

        // Written: Tuesday 09-Sep-2014 7:06 am, Code HQ Ehinger Tor

        public Entry Search(string searchTerm, bool firstSearch)
        {
            int index, offset;
            Entry retVal;

            // Assume failure
            offset = -1;
            retVal = null;

            // If the search is to be from the first entry, setup the index to search from the beginning.
            // Otherwise the previous value will be used, thus continuing the search from the entry after
            // the previously matching entry
            if (firstSearch)
            {
                searchEntryIndex = -1;
            }

            // Iterate through all the entries, starting with the one requested
            for (index = (searchEntryIndex + 1); index < allItems.Count; ++index)
            {
                // Get the entry for the current index and see if either the foreign or English words match
                // the search term
                offset = allItems[index].English.IndexOf(searchTerm, StringComparison.CurrentCultureIgnoreCase);

                if (offset == -1)
                {
                    offset = allItems[index].Foreign.IndexOf(searchTerm, StringComparison.CurrentCultureIgnoreCase);
                }

                // If we have found a match, save the index so that the search can be continued later.  Also prepare
                // the return value and break out of the search loop
                if (offset != -1)
                {
                    searchEntryIndex = index;
                    retVal = allItems[index];

                    break;
                }
            }

            // Otherwise reset the starting point so that if another search is performed, it will start from the
            // first entry again
            if (offset == -1)
            {
                searchEntryIndex = -1;
            }

            // If nothing was found then return an empty search result
            if (retVal == null)
            {
                retVal = new Entry();
            }

            return retVal;
        }

        public void SetGroup(String groupName)
        {
            this.groupName = groupName;
            groupSet = true;

            LoadCollections();
        }

        // Written: Monday 24-Nov-2014 6:43 am, Code HQ Ehinger Tor

        public void SetTestGroup(TestTypes testType, string subWord)
        {
            // Reload the collections from the database in order to perform a test.  Do this even if the test
            // type has not changed, to reset the list of words to be tested if a test is being restarted.  We
            // also save the test type for later use, in case the application is restarted
            this.testType = testType;
            this.subWord = subWord;
            //PhoneApplicationService.Current.State["testType"] = testType;
            LoadCollections();
        }

        // Written: Tuesday 11-Nov-2014 7:33 am, Code HQ Ehinger Tor

        public ObservableCollection<Entry> CurrentTestItems
        {
            get
            {
                return currentTestItems;
            }
        }

        // Written: Wednesday 29-Oct-2014 6:30 am, Code HQ Ehinger Tor

        public ObservableCollection<Group> Groups
        {
            get
            {
                return groups;
            }
        }

        // Written: Saturday 18-Oct-2014 8:38 am, Code HQ Ehinger Tor

        public string GroupName
        {
            get
            {
                return groupName;
            }
        }

        public DatabaseContext DataContext
        {
            get
            {
                return dataContext;
            }
        }

        // Written: Tuesday 11-Nov-2014 7:32 am, Code HQ Ehinger Tor

        public ObservableCollection<Entry> TestItems
        {
            get
            {
                return testItems;
            }
        }
    }
}
