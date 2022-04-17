using System.ComponentModel;
using Xamarin.Forms;
using LinguistNGX.ViewModels;

namespace LinguistNGX.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
