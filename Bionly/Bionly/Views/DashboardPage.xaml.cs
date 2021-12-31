using Bionly.ViewModels;
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

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            RuntimeData.SelectedDeviceIndex = e.ItemIndex;
            ((DashboardViewModel)BindingContext).DrawGraphs.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            WelcomeTxt.Text = ((DashboardViewModel)BindingContext).GetWelcomeText();
            WelcomeImg.Source = ((DashboardViewModel)BindingContext).GetWelcomeImage();
            ConnectedTxt.Text = ((DashboardViewModel)BindingContext).GetConnectedText();
            RadChart.Chart = ((DashboardViewModel)BindingContext).radChart;
            RuntimeData.SelectedDeviceIndex = ((DashboardViewModel)BindingContext).SelectedIndex;
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
            await DisplayAlert("Fehler", "Das Gerät unterstützt keine Fernkonfiguration.", "OK");
        }

        private void DeviceList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            RuntimeData.SelectedDeviceIndex = ((DashboardViewModel)BindingContext).SelectedIndex = e.SelectedItemIndex;
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
    }
}