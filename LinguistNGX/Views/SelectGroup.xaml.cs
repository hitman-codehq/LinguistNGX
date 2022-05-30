
using System;
using Xamarin.Forms;

using LinguistNGX.Services;

namespace LinguistNGX.Views
{
    public partial class SelectGroupPage : ContentPage
    {
        private Group group;

        public SelectGroupPage()
        {
            InitializeComponent();

            BindingContext = App.ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            int index;

            GroupList.ItemsSource = App.ViewModel.Groups;

            // Search for the currently selected group in the ObservableCollection that was just created.
            // Start by creating a temporary Group with which to search
            Group targetGroup = new Group { Name = App.ViewModel.GroupName };

            // See if this is in the ObservableCollection and if so, make it the current entry in the Groups list
            if ((index = App.ViewModel.Groups.IndexOf(targetGroup)) != -1)
            {
                GroupList.SelectedItem = App.ViewModel.Groups[index];

                // This is a hack.  The above statement will select the current item but will not scroll it into
                // view, and we cannot do this immediately (by calling ScrollIntoView()) because the collection is
                // loaded into the control asynchronously.  Every "nice" way of doing this that I have found does
                // not work on Windows Phone, so we use the dispatcher to call ScrollIntoView() at a later time
                // when the collection is loaded
                //Dispatcher.BeginInvoke(new Action(() => GroupList.ScrollIntoView(GroupList.SelectedItem)));
            }
            // Otherwise set the currently selected group to the first one in the collection, but only if the
            // collection contains any entries
            else
            {
                if (App.ViewModel.Groups.Count > 0)
                {
                    //GroupList.SelectedIndex = 0;
                }
            }
        }

        private void appBarOkButton_Click(Object sender, EventArgs e)
        {
            if (group != null)
            {
                App.ViewModel.SetGroup(group.Name);
            }

            Shell.Current.GoToAsync("..");
        }

        private void listBox_SelectionChanged(Object sender, EventArgs e)
        {
            group = ((ListView)sender).SelectedItem as Group;
        }
    }
}
