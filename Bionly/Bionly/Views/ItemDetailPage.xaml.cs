using Bionly.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Bionly.Views
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