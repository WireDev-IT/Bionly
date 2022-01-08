using Bionly.Resx;
using Bionly.ViewModels;
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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DefaultView.SelectedItem = RuntimeData.SelectedDevice;
        }

        private async void ChartsBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ChartsPage());
        }

        private async void MeasurementsBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MeasurementsPage());
        }

        private async void ImagesBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ImagesPage());
        }

        private async void ConfigBtn_Clicked(object sender, System.EventArgs e)
        {
            await DisplayAlert(Strings.Error, Strings.NoRemoteConfig, Strings.OK);
        }

        /// <summary>
        /// Actions that are executed when a device is selected.
        /// </summary>
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            RuntimeData.SelectedDeviceIndex = e.ItemIndex;
            ((DashboardViewModel)BindingContext).DrawGraphs.Execute(null);
            ConnectBtn.IsEnabled = true;
        }

        /// <summary>
        /// Sets the device view to be displayed.
        /// </summary>
        private void DeviceList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItemIndex < 0)
            {
                StartpageContainer.IsVisible = true;
                DeviceContainer.IsVisible = false;
            }
            else
            {
                StartpageContainer.IsVisible = false;
                DeviceContainer.IsVisible = true;
            }
        }

        /// <summary>
        /// Connects to the selected device.
        /// </summary>
        private async void ConnectBtn_Clicked(object sender, System.EventArgs e)
        {
            ConnectBtn.IsEnabled = false;
            await RuntimeData.SelectedDevice.CheckConnection();

            //cooldown for connection
            await Task.Delay(10000);
            ConnectBtn.IsEnabled = true;
        }
    }
}