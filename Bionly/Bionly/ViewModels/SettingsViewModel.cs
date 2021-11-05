using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public ObservableCollection<Models.Device> Devices { get; set; } = new();
        public static readonly string[] strings = new string[7];

        public ICommand LoadAllDevices => new Command(async () =>
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
                    byte[] codebytes = new byte[8];
                    string code;
                    do
                    {
                        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(codebytes);
                        }
                        code = BitConverter.ToString(codebytes).ToLower().Replace("-", "");
                    } while (File.Exists(Models.Device.path + $"/Devices/{code}.json"));
                    d.Id = code;
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
