using Bionly.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
            SettingsViewModel.LoadAllDevices.Execute(null);
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DeviceExplorerViewModel.Device = (Models.Device)e.Item;
            await Navigation.PushAsync(new DeviceExplorer());
        }
    }
}