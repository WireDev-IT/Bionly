using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bionly.Models
{
    public class Device : INotifyPropertyChanged
    {
        public static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);

        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private string _ipAdress;
        public string IpAddress
        {
            get => _ipAdress;
            set
            {
                if (_ipAdress != value && CheckAdress(value))
                {
                    _ipAdress = value;
                    OnPropertyChanged(nameof(IpAddress));
                }
            }
        }

        public Device()
        {

        }

        public async Task<bool> Delete()
        {
            try
            {
                File.Delete(path + $"/Devices/{Id}.json");
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Die App besitzt nicht die benötigten Berechtigungen um dieses Gerät zu löschen.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", ex.Message, "OK");
            }
            return false;
        }
        public async Task<bool> Save()
        {
            try
            {
                _ = Directory.CreateDirectory(path + $"/Devices");
                File.WriteAllText(path + $"/Devices/{Id}.json", JsonConvert.SerializeObject(this, Formatting.Indented));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Die App besitzt nicht die benötigten Berechtigungen um dieses Gerät zu speichern.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", ex.Message, "OK");
            }
            return false;
        }
        public static async Task<Device> Load(string file)
        {
            try
            {
                _ = Directory.CreateDirectory(path + $"/Devices");
                return JsonConvert.DeserializeObject<Device>(File.ReadAllText(file));
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Die App besitzt nicht die benötigten Berechtigungen um dieses Gerät zu laden.", "OK");
            }
            catch (FileNotFoundException)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Dieses Gerät wurde nicht gefunden.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", ex.Message, "OK");
            }
            return new();
        }

        public bool CheckAdress(string input)
        {
            if (IPAddress.TryParse(input, out IPAddress address))
            {
                _ipAdress = address.ToString();
                return true;
            }
            return false;
        }
        public IPAddress GetAdress()
        {
            return IPAddress.Parse(IpAddress);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
