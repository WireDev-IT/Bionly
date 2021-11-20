using Bionly.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void DevicesView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DeviceDetailViewModel.Device = (Models.Device)e.Item;
            await Navigation.PushAsync(new DeviceDetailPage());
        }

        private async void CreateDevice(object sender, EventArgs e)
        {
            DeviceDetailViewModel.Device = new();
            await Navigation.PushAsync(new DeviceDetailPage());
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            SettingsViewModel.LoadAllDevices.Execute(null);
        }
    }
}