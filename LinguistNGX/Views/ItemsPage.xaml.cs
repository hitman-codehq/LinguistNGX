using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LinguistNGX.Models;
using LinguistNGX.Views;
using LinguistNGX.ViewModels;

namespace LinguistNGX.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ItemsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();

            if (_viewModel.Items.Count == 0)
                _viewModel.IsBusy = true;
        }
    }
}
