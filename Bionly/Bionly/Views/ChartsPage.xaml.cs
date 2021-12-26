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
    public partial class ChartsPage : ContentPage
    {
        private string DeviceId = null;

        public ChartsPage()
        {
            InitializeComponent();
        }

        public ChartsPage(string deviceId)
        {
            InitializeComponent();
            DeviceId = deviceId;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (DeviceId != null)
            {
                HeaderLbl.Text = $"Graphen für \"{SettingsViewModel.Devices.First(x => x.Id == DeviceId).Name}\"";
            }
            else
            {
                HeaderLbl.Text = "Durchschnittswerte der Vergangenheit";
            }

            TempChart.Chart = ((DashboardViewModel)BindingContext).tempChart;
            HumiChart.Chart = ((DashboardViewModel)BindingContext).humiChart;
            PresChart.Chart = ((DashboardViewModel)BindingContext).presChart;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DeviceId = null;
        }

    }
}