using Bionly.Resx;
using Bionly.Views;
using System;
using Xamarin.Forms;

namespace Bionly
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RuntimeData.LoadAllDevices.Execute(null);
            LocalizationHelper.Initialize();
        }

        private async void AccBtn_Clicked(object sender, EventArgs e)
        {
            await Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        private void Shell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.Target.Location.ToString() == $"//{nameof(ChartsPage)}")
            {
                RuntimeData.SelectedDeviceIndex = -1;
            }
        }

        private void LangBtn_Clicked(object sender, EventArgs e)
        {
            DisplayActionSheet(Strings.ChangeLanguage, Strings.Cancel, null, "Deutsch", "English");
        }
    }
}
