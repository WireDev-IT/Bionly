using Bionly.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeasurementsPage : ContentPage
    {
        public MeasurementsPage()
        {
            InitializeComponent();
        }

        public MeasurementsPage(string deviceId)
        {
            InitializeComponent();
            ((MeasurementsViewModel)BindingContext).Setup(deviceId);
        }
    }
}