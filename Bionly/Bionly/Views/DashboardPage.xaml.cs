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
            SettingsViewModel.LoadAllDevices.Execute(null);
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DeviceExplorerViewModel.Device = (Models.Device)e.Item;
            await Navigation.PushAsync(new DeviceExplorer());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TempChart.Chart = ((DashboardViewModel)BindingContext).tempChart;
            HumiChart.Chart = ((DashboardViewModel)BindingContext).humiChart;
            PresChart.Chart = ((DashboardViewModel)BindingContext).presChart;
        }
    }
}