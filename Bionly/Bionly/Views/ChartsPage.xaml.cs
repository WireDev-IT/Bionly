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
        public ChartsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (RuntimeData.SelectedDeviceIndex >= 0)
            {
                HeaderLbl.Text = $"Graphen für \"{RuntimeData.SelectedDevice.Name}\"";
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
        }

    }
}