
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LinguistNGX.Services;
using LinguistNGX.ViewModels;

// TODO: CAW - https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
// TODO: CAW - https://docs.microsoft.com/en-us/dotnet/standard/io/isolated-storage
// TODO: CAW - https://medium.com/@_jamesmundy/isolatedstoragesettings-for-xamarin-android-ebeea59f145f

namespace LinguistNGX
{
    public partial class App : Application
    {
        private static ViewModel viewModel;
        public static ViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
        }

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        // Written: Thursday 10-Oct-2014 6:39 am, Code HQ Ehinger Tor

        public void LoadData(string databaseName)
        {
            string DBLinguistString = databaseName; // "LinguistX.db3";
            string DBScratchString = "Scratch.db3";

            // Create an instance of the data context that can be used for initialising the words database
            using (DatabaseContext db = new DatabaseContext(DBLinguistString))
            {
                // If the database does not exist then create and populate it now
                /*if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();

                    DataImporter.ImportDictionary(db, "Español.lng");

                    db.SubmitChanges();
                }*/
            }

            // Create an instance of the data context that can be used for initialising the scratch database
            using (DatabaseContext db = new DatabaseContext(DBScratchString))
            {
                // If the database does not exist then create it now
                /*if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                    db.SubmitChanges();
                }*/
            }

            // Create a view for displaying the contents of the database, and load the contents of the
            // database into a collection object that can be accessed by the XAML GUI
            viewModel = new ViewModel(DBLinguistString, DBScratchString);
            viewModel.LoadCollections();
        }
    }
}
