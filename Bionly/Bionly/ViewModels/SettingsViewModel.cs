using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public static ObservableCollection<Models.Device> Devices { get; set; } = new();
        public static readonly string[] strings = new string[10];

        public static ICommand LoadAllDevices => new Command(async () =>
        {
            Devices.Clear();
            foreach (string file in Directory.GetFiles(Models.Device.path + "\\Devices\\", "*.json", SearchOption.TopDirectoryOnly))
            {
                Models.Device d = await Models.Device.Load(file);
                Devices.Add(d);
            }
        });

        public ICommand AddNewDevice => new Command(async () =>
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync("Neues Gerät", "Wie soll das Gerät heißen?", strings[1], strings[0], "Neues Gerät", keyboard: Keyboard.Text);
            if (!string.IsNullOrWhiteSpace(result))
            {
                Models.Device d = new();
                d.Name = result;

                bool answer = await Application.Current.MainPage.DisplayAlert("Beschreibung hinzufügen", "Möchten Sie eine Beschreibung zum Gerät hinzufügen?", "Ja", "Nein");
                if (answer)
                {
                    result = await Application.Current.MainPage.DisplayPromptAsync(d.Name, "Geben Sie eine Beschreibung ein", strings[1], strings[0], keyboard: Keyboard.Text);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        d.Description = result;
                    }
                }

                bool valid;
                bool cancel = true;
                do
                {
                    result = await Application.Current.MainPage.DisplayPromptAsync(d.Name, "Geben Sie die IP-Adresse des Gerätes ein", strings[2], strings[0], "127.0.0.1", keyboard: Keyboard.Text);
                    if (result == null)
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = await SetIpAdress(d, result);
                        if (valid)
                        {
                            cancel = false;
                        }
                    }
                } while (!valid);

                if (!cancel)
                {
                    _ = d.NewId();
                    _ = await d.Save();
                    Devices.Add(d);
                }
            }
        });

        public SettingsViewModel()
        {
            Title = "Einstellungen";
            strings[0] = "Abbrechen";
            strings[1] = "Weiter";
            strings[2] = "Speichern";
            strings[3] = "Entfernen";
            strings[4] = "Name";
            strings[5] = "Beschreibung";
            strings[6] = "IP-Adresse";
            strings[7] = "Kamera Port";
            strings[8] = "FTP Port";
        }

        public static async Task<bool> SetIpAdress(Models.Device d, string input)
        {
            if (!d.CheckAdress(input))
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", input + " ist keine gültige IP-Adresse!", "OK");
                return false;
            }
            return true;
        }
    }
}
