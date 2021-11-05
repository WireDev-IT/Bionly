using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Bionly.ViewModels.SettingsViewModel;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void DevicesView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Models.Device d = (Models.Device)e.Item;

            string action = await DisplayActionSheet(d.Name + $" bearbeiten", strings[0], strings[3], strings[4], strings[5], strings[6]);
            if (action == strings[3])
            {
                bool answer = await DisplayAlert(d.Name, "Möchten Sie das Gerät wirklich löschen?", "Ja", "Nein");
                if (answer)
                {
                    if (await d.Delete())
                    {

                    }
                }
            }
            else if (action == strings[4])
            {
                string result = await DisplayPromptAsync(strings[4] + " bearbeiten", $"Geben Sie einen neuen Namen für {d.Name} ein", strings[2], strings[0], initialValue: d.Name);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    d.Name = result;
                }
            }
            else if (action == strings[5])
            {
                string result = await DisplayPromptAsync(strings[5] + " bearbeiten", $"Geben Sie eine neue {strings[5]} für {d.Name} ein", strings[2], strings[0], "Beschreibung");
                d.Description = result;
            }
            else if (action == strings[6])
            {
                bool valid;
                do
                {
                    string result = await DisplayPromptAsync(strings[6] + " bearbeiten", $"Geben Sie die neue {strings[6]} für {d.Name} ein", strings[2], strings[0], "127.0.0.1");
                    if (result == null)
                    {
                        break;
                    }
                    else
                    {
                        valid = await SetIpAdress(d, result);
                    }
                } while (!valid);
            }
            _ = await d.Save();
        }
    }
}