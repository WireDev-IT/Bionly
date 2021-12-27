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
    public partial class DeviceSettingsPage : ContentPage
    {
        public DeviceSettingsPage()
        {
            InitializeComponent();
        }

        private async void DeleteBtn_Clicked(object sender, EventArgs e)
        {
            ((DeviceSettingsViewModel)BindingContext).DeleteDevice.Execute(null);
            await Navigation.PopAsync();
        }
    }
}