﻿using System;
using System.Collections.Generic;
using LinguistNGX.ViewModels;
using LinguistNGX.Views;
using Xamarin.Forms;

namespace LinguistNGX
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(SelectGroupPage), typeof(SelectGroupPage));
            Routing.RegisterRoute(nameof(TestPage), typeof(TestPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
