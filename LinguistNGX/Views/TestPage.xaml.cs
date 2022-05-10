
using System;
using Xamarin.Forms;
using LinguistNGX.Services;

namespace LinguistNGX.Views
{
    // TODO: CAW - This is not a good place to put this
    public enum TestTypes
    {
        All, Group, Scratch, SubWords
    };

    [QueryProperty(nameof(Dictionary), "dictionary")]
    public partial class TestPage : ContentPage
    {
        private bool newPage = true;
        private bool searching = false;
        private bool showAnswer = false;
        private bool testing = false;
        private int direction = 0;
        private int rightCount = 0;
        private int wrongCount = 0;
        private string databaseName;
        private string foreign = String.Empty;
        private string english = String.Empty;
        private string searchTerm = String.Empty;
        private LinguistNGX.Services.Entry testWords;

        public string Dictionary
        {
            set
            {
                databaseName = value;
            }
        }

        // Constructor
        public TestPage()
        {
            InitializeComponent();

            //this.DataContext = App.ViewModel;
            this.testWords = new LinguistNGX.Services.Entry();
        }

        private void countWords_Click(object sender, EventArgs e)
        {
            string message;

            message = System.String.Format("{0} (you genius)", App.ViewModel.GetCount());
            DisplayAlert("# of test words", message, "OK");
        }

        // Written: Wednesday 10-Sep-2014 6:49 am, Code HQ Ehinger Tor

        private void draw()
        {
            // Depending on the translation direction chosen, setup the words to be displayed
            if (direction == 0)
            {
                foreign = testWords.Foreign;
                english = testWords.English;
            }
            else
            {
                english = testWords.Foreign;
                foreign = testWords.English;
            }

            // If we are testing then display the titles of the statistics text blocks
            if (testing)
            {
                rightTitle.Text = "Right";
                remainingTitle.Text = "Remaining";
                wrongTitle.Text = "Wrong";
            }
            // Otherwise blank them out, along with the statistics themselves
            else
            {
                rightTitle.Text = remainingTitle.Text = wrongTitle.Text = "";
                rightNum.Text = remainingNum.Text = wrongNum.Text = "";
            }

            if (searching)
            {
                // If a search result has been found, display the source and destination words
                if (!String.IsNullOrEmpty(foreign))
                {
                    foreignWord.Text = foreign;
                    englishWord.Text = english;

                    // And enable the Scratch button, as an entry has been found that can be added
                    Scratch.IsEnabled = true;
                }
                // No search result has been found so display an error near the centre of the screen
                else
                {
                    foreignWord.Text = "Not found";
                    englishWord.Text = String.Empty;

                    // And disable the Scratch button, as no entry has been found to be added
                    Scratch.IsEnabled = false;
                }
            }
            else if (testing)
            {
                // Draw the source word being tested
                foreignWord.Text = foreign;

                // If the user has tapped on the screen to see the answer, display that as well
                if (showAnswer)
                {
                    englishWord.Text = english;
                }
                else
                {
                    englishWord.Text = String.Empty;
                }

                // And finally, update the statistics text blocks
                rightNum.Text = rightCount.ToString();
                wrongNum.Text = wrongCount.ToString();

                // The remaining count is special as we have to take into account that there is a word
                // currently under test, which is no longer in the test list.  Otherwise the total count
                // of right + wrong + remaining is one less than the number in the group.  However, we
                // also need to account for the special case where the group under test is empty
                if (App.ViewModel.GetTestCount() > 0)
                {
                    remainingNum.Text = (App.ViewModel.CurrentTestItems.Count + 1).ToString();
                }
                else
                {
                    remainingNum.Text = "0";
                }
            }
        }

        // Written: Tuesday 25-Nov-2014 6:34 am, Code HQ Ehinger Tor

        private void GetWordAndRedraw()
        {
            // Disable display of the answer word and obtain the next set of test words from the current dictionary
            showAnswer = false;
            Right.IsEnabled = Wrong.IsEnabled = false;
            testWords = App.ViewModel.GetEntry();

            // And request a redraw
            draw();
        }

        private void newTaskAppBarButton_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/NewEntry.xaml", UriKind.Relative));
        }

        // Written: Tuesday 09-Sep-2014 7:00 am, Code HQ Ehinger Tor

        private void search_Click(object sender, EventArgs e)
        {
            // Enable searching mode and disable testing mode
            searching = true;
            testing = false;
            Right.IsEnabled = Wrong.IsEnabled = false;

            // Save the requested search term for later and search for the first match of the
            // search term
            searchTerm = searchString.Text;
            testWords = App.ViewModel.Search(searchTerm, true);

            // Request a redraw to display the result
            draw();

            // Update the title to indicate that we are in search mode
            ApplicationTitle.Text = "Linguist: Search";
        }

        private void selectGroup_Click(object sender, EventArgs e)
        {
            // Clear the group set flag before bringing up the SelectGroup page.  When the page returns
            // to this one, we can check to see if a group was selected by checking the value of this variable
            App.ViewModel.groupSet = false;
            //NavigationService.Navigate(new Uri("/SelectGroup.xaml", UriKind.Relative));
        }

        private void swap_Click(object sender, EventArgs e)
        {
            // Change the order in which the foreign and English words are displayed
            direction = (direction == 0) ? 1 : 0;

            // Request a redraw to display the new layout immediately
            draw();
        }

        // Written: Tuesday 07-Oct-2014 6:33 am, Code HQ Ehinger Tor

        /*protected void onKeyUp(object sender, KeyEventArgs e)
        {
            // Check to see if it was the enter key that was pressed and if so, call the search function to
            // simulate the "Go" button being pressed
            if (e.Key == Key.Enter)
            {
                search_Click(sender, e);
            }
        }*/

        // Written: Wednesday 15-Oct-2014 6:20 am, Code HQ Ehinger Tor

        protected override void OnDisappearing()
        {
            // The page is being navigated away from.  If the navigation mode is Back then the user has
            // tapped the back button and we want to throw the state away.  If the mode is not Back then
            // the user has probably hit the Windows button to bring up the home screen and make the application
            // dormant (or something else caused this) so we want to save the state in the state dictionary so
            // that it can be restored when the page is redisplayed
            /*if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
            {
                State["searching"] = searching;
                State["searchTerm"] = searchString.Text;
                State["searchEntryIndex"] = App.ViewModel.searchEntryIndex;
                State["showAnswer"] = showAnswer;
                State["testing"] = testing;
                State["foreign"] = testWords.Foreign;
                State["english"] = testWords.English;
                State["applicationTitle"] = ApplicationTitle.Text;
            }

            // Unconditionally and permanently save the current group and results displaying direction
            App.ViewModel.SaveSettings(direction);*/
        }

        // Written: Wednesday 15-Oct-2014 6:24 am, Code HQ Ehinger Tor

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // If newPage is true then the page has just been created (meaning that the application has
            // also just been created).  If there is any data in the state dictionary then it means that
            // we are returning from being tombstoned, so restore the contents of the state dictionary
            if (newPage)
            {
                // First, get a pointer to the App class and import the dictionary.  We do this here rather
                // than when the application is starting so that we can display messages on failure, which
                // cannot be done during startup
                App app = (App)App.Current;
                app.LoadData(databaseName);

                // Unconditionally restore the state of the results displaying direction, which is
                // a permanent setting
                /*IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                if (settings.Contains("direction"))
                {
                    direction = (int) settings["direction"];
                }

                // Now restore the contents of the state dictionary
                if (State.Count > 0)
                {
                    searching = (bool) State["searching"];
                    searchTerm = searchString.Text = (string) State["searchTerm"];
                    showAnswer = (bool) State["showAnswer"];
                    testing = (bool) State["testing"];
                    testWords.Foreign = (string) State["foreign"];
                    testWords.English = (string) State["english"];
                    ApplicationTitle.Text = (string) State["applicationTitle"];
                    draw();

                    // Let the view model know what the current search entry index is so that it can
                    // continue searching as though we have never been tombstoned
                    App.ViewModel.searchEntryIndex = (int) State["searchEntryIndex"];

                    // If we are in the middle of a test then restore the state of the answer buttons
                    if ((testing) && (showAnswer))
                    {
                        Right.IsEnabled = Wrong.IsEnabled = true;
                    }
                }*/

                // Prevent this from happening if the application is made dormant and then restored
                newPage = false;

                // TODO: CAW - Temporary
                Test(TestTypes.Group);
            }
            else
            {
                // The application was not tombstoned.  See if we are returning from the SelectGroup page by checking
                // to see if the currently selected group has changed and if so, start a group test
                if (App.ViewModel.groupSet)
                {
                    Test(TestTypes.Group);

                    // Indicate that we have already started a test, or the test will restart if we switch away from
                    // the application and back again
                    App.ViewModel.groupSet = false;
                }
            }
        }

        // Written: Wednesday 10-Sep-2014 7:21 am, Code HQ Ehinger Tor

        private void screenTap(object sender, EventArgs e)
        {
            if (testing)
            {
                // If the user has tapped the screen and the "question" word is being displayed, enable the
                // display of the "answer" word and the right and wrong buttons
                if (!showAnswer)
                {
                    showAnswer = true;
                    Right.IsEnabled = Wrong.IsEnabled = true;
                }
            }
            else if (searching)
            {
                // Search for the next instance of the current search term
                testWords = App.ViewModel.Search(searchTerm, false);
            }

            // And request a redraw
            draw();
        }

        // Written: Sunday 13-Mar-2022 9:41 am, John & Sally's place, Devon

        private void MessageBox(string message)
        {
            DisplayAlert("Information", message, "OK");
        }

        // Written: Thursday 11-Sep-2014 6:49 am, Code HQ Ehinger Tor

        public void Test(TestTypes testType, string subWord = "")
        {
            // Disable searching mode and enable testing mode
            searching = showAnswer = false;
            testing = true;

            // Reset the test results and disable the right and wrong buttons
            rightCount = wrongCount = 0;
            Right.IsEnabled = Wrong.IsEnabled = false;

            // Generate a list of words to test, using the current test type as appropriate
            App.ViewModel.SetTestGroup(testType, subWord);

            // If we are not testing the scratch group already, enable the Scratch button.  Otherwise disable it
            Scratch.IsEnabled = (testType != TestTypes.Scratch);

            // Get the first entry to be tested and request a redraw to draw the "question" word
            testWords = App.ViewModel.GetEntry();
            draw();

            // Update the title to indicate that we are in one of the test modes
            if (testType == TestTypes.All)
            {
                ApplicationTitle.Text = "Linguist: Test all";
            }
            else if (testType == TestTypes.Group)
            {
                ApplicationTitle.Text = "Linguist: Test group";
            }
            else
            {
                ApplicationTitle.Text = "Linguist: Test scratch group";
            }

            // If the selected test group is empty then display a message regarding this
            if (App.ViewModel.GetTestCount() == 0)
            {
                MessageBox("No words to test");
            }
        }

        private void testAllGroups_Click(object sender, EventArgs e)
        {
            Test(TestTypes.All);
        }

        private void testCurrentGroup_Click(object sender, EventArgs e)
        {
            Test(TestTypes.Group);
        }

        // Written: Monday 24-Nov-2014 6:47 am, Code HQ Ehinger Tor

        private void testScratchGroup_Click(object sender, EventArgs e)
        {
            Test(TestTypes.Scratch);
        }

        // Written: Thursday 10-Mar-2016 6:50 am, Code HQ Ehinger Tor

        private void testSubWords_Click(object sender, EventArgs e)
        {
            Test(TestTypes.SubWords, searchString.Text);
        }

        // Written: Tuesday 25-Nov-2014 6:34 am, Code HQ Ehinger Tor

        private void clearScratchGroup_Click(object sender, EventArgs e)
        {
            // Delete all items from the scratch collection
            App.ViewModel.ClearScratchCollection();

            // And try to fetch a new word, but only if we are currently testing the scratch collection.  This
            // will cause the button states to be reset and the screen to be cleared as there are no more words
            // to be fetched
            if (App.ViewModel.testType == TestTypes.Scratch)
            {
                GetWordAndRedraw();
            }
        }

        // Written: Tuesday 11-Nov-2014 7:26 am, Code HQ Ehinger Tor

        private void Right_Click(object sender, EventArgs e)
        {
            // Increment count of right answers
            ++rightCount;

            // If the current test group is empty then check the results of the overall test
            if (App.ViewModel.CurrentTestItems.Count == 0)
            {
                // If no answers were wrong then see if three test rounds in a row were correct
                if (wrongCount == 0)
                {
                    if (rightCount == (App.ViewModel.TestItems.Count * 3))
                    {
                        string message;

                        message = System.String.Format("{0} out of {1} correct!  Reversing testing direction...", rightCount, rightCount);
                        DisplayAlert("Test result", message, "OK");

                        // Reset the test
                        rightCount = wrongCount = 0;

                        // And change the order in which the foreign and English words are displayed
                        direction = (direction == 0) ? 1 : 0;
                    }
                }
                // One or more answers were wrong, so simply reset the test
                else
                {
                    rightCount = wrongCount = 0;
                }
            }

            // Disable display of the answer word and obtain the next set of test words from the current dictionary
            GetWordAndRedraw();
        }

        // Written: Friday 21-Nov-2014 6:27 am, Code HQ Ehinger Tor

        private void Scratch_Click(object sender, EventArgs e)
        {
            if (!App.ViewModel.AddItemToScratchCollection(testWords))
            {
                MessageBox("Entry is already in Scratch Group");
            }
        }

        // Written: Tuesday 11-Nov-2014 7:35 am, Code HQ Ehinger Tor

        private void Wrong_Click(object sender, EventArgs e)
        {
            // Increment count of wrong answers
            ++wrongCount;

            // If the current test group is empty then reset the test
            if (App.ViewModel.CurrentTestItems.Count == 0)
            {
                rightCount = wrongCount = 0;
            }

            // Disable display of the answer word and obtain the next set of test words from the current dictionary
            GetWordAndRedraw();
        }
    }
}
