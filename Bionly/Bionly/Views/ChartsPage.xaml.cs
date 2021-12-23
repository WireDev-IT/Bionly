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

            TempChart.Chart = ((DashboardViewModel)BindingContext).tempChart;
            HumiChart.Chart = ((DashboardViewModel)BindingContext).humiChart;
            PresChart.Chart = ((DashboardViewModel)BindingContext).presChart;
        }
    }
}