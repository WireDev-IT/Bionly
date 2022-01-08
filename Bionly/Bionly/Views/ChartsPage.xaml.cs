using Bionly.Resx;
using Bionly.ViewModels;
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
                HeaderLbl.Text = string.Format(Strings.ChartsOf_Name, RuntimeData.SelectedDevice.Name);
                ((ChartsViewModel)BindingContext).DrawGraphs.Execute(RuntimeData.SelectedDevice.MPoints);
            }
            else
            {
                HeaderLbl.Text = Strings.AverageValues;
                ((ChartsViewModel)BindingContext).CalcAverage.Execute(null);
                ((ChartsViewModel)BindingContext).DrawGraphs.Execute(null);
            }
        }
    }
}