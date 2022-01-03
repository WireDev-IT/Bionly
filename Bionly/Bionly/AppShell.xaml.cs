using Bionly.Resx;
using Bionly.Views;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Bionly
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            LocalizationHelper.Initialize();
            InitializeComponent();
            RuntimeData.LoadAllDevices.Execute(null);
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

        private async void LangBtn_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet(Strings.ChangeLanguage, Strings.Cancel, null, LocalizationHelper.SupportedLanguagesStr);
            if (!string.IsNullOrWhiteSpace(result) && result != Strings.Cancel)
            {
                try
                {
                    LocalizationHelper.Settings.TwoLetterISOLanguageName = LocalizationHelper.SupportedLanguages.First(x => x.DisplayName == result).TwoLetterISOLanguageName;
                    if (!await LocalizationHelper.Settings.Save()) throw new Exception();

                    await DisplayAlert(Strings.RestartRequired, Strings.CloseAppForLanguage, Strings.OK);

                    Application.Current.MainPage = new AppShell();
                }
                catch (Exception)
                {
                    await DisplayAlert(Strings.Error, Strings.NewLangError, Strings.OK);
                }
            }
        }
    }
}
