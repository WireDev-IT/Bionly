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
                Models.Device d = SettingsViewModel.Devices.FirstOrDefault(x => x.Id == DeviceId);
                ((ChartsViewModel)BindingContext).Device = d;
                HeaderLbl.Text = $"Graphen für \"{d.Name}\"";
                TempChart.Chart = ((ChartsViewModel)BindingContext).tempChart;
                HumiChart.Chart = ((ChartsViewModel)BindingContext).humiChart;
                PresChart.Chart = ((ChartsViewModel)BindingContext).presChart;
                ((ChartsViewModel)BindingContext).DrawGraphs.Execute(null);
            }
            else
            {
                HeaderLbl.Text = "Durchschnittswerte der Vergangenheit";
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DeviceId = null;
        }

    }
}