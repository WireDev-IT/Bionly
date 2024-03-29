﻿using Bionly.Resx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Bionly.Enums.Connection;
using static Bionly.Enums.Path;

namespace Bionly.Models
{
    public class Device : INotifyPropertyChanged
    {
        public Device() { }

        public override string ToString()
        {
            base.ToString(); 
            return Name;
        }

        public static string GeneralPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "/Devices/";
        private bool ErrorOnCurrentValues = false;

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
                if (_ipAdress != value)
                {
                    _ipAdress = value;
                    OnPropertyChanged(nameof(IpAddress));

                    switch (CheckAdress(value))
                    {
                        case true:
                            Connected = ConnectionStatus.Disconnected;
                            break;
                        case false:
                            Connected = ConnectionStatus.Error;
                            break;
                    }
                }
            }
        }

        private ushort _camPort;
        public ushort CameraPort
        {
            get => _camPort;
            set
            {
                if (_camPort != value)
                {
                    _camPort = value;
                    OnPropertyChanged(nameof(CameraPort));
                }
            }
        }

        private ushort _ftpPort;
        public ushort FtpPort
        {
            get => _ftpPort;
            set
            {
                if (_ftpPort != value)
                {
                    _ftpPort = value;
                    OnPropertyChanged(nameof(FtpPort));
                }
            }
        }

        private bool[] _settings = new bool[2];
        public bool[] Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged(nameof(Settings));
                }
            }
        }

        private List<MeasurementPoint> _mPoints = new();
        [JsonIgnore]
        public List<MeasurementPoint> MPoints
        {
            get => _mPoints;
            set
            {
                if (_mPoints != value)
                {
                    _mPoints = value;
                    OnPropertyChanged(nameof(MPoints));
                }
            }
        }

        private Dictionary<DateTime, string> _images = new();
        [JsonIgnore]
        public Dictionary<DateTime, string> Images
        {
            get => _images;
            set
            {
                if (_images != value)
                {
                    _images = value;
                    OnPropertyChanged(nameof(Images));
                }
            }
        }

        private float _currentTemp = 0;
        [JsonIgnore]
        public float CurrentTemp
        {
            get => _currentTemp;
            set
            {
                if (_currentTemp != value)
                {
                    _currentTemp = value;
                    OnPropertyChanged(nameof(CurrentTemp));
                }
            }
        }

        private float _currentHumi = 0;
        [JsonIgnore]
        public float CurrentHumi
        {
            get => _currentHumi;
            set
            {
                if (_currentHumi != value)
                {
                    _currentHumi = value;
                    OnPropertyChanged(nameof(CurrentHumi));
                }
            }
        }

        private float _currentPres = 0;
        [JsonIgnore]
        public float CurrentPres
        {
            get => _currentPres;
            set
            {
                if (_currentPres != value)
                {
                    _currentPres = value;
                    OnPropertyChanged(nameof(CurrentPres));
                }
            }
        }

        [JsonIgnore]
        public string CurrentValuesStr
        {
            get
            {
                if (Connected == ConnectionStatus.Disconnected)
                {
                    return "Nicht Verbunden";
                }
                else if (Connected == ConnectionStatus.Connecting)
                {
                    return "Verbinden...";
                }
                else if (Connected == ConnectionStatus.Connected)
                {
                    return ErrorOnCurrentValues ? Strings.ErrorOnCall : CurrentTemp.ToString("N1") + " °C, " + CurrentHumi.ToString("00") + " %, " + CurrentPres + " hPa";
                }
                else
                {
                    return Strings.ErrorOnCall;
                }
            }
        }    

        private ConnectionStatus _connected = ConnectionStatus.Error;
        [JsonIgnore]
        public ConnectionStatus Connected
        {
            get => _connected;
            set
            {
                if (_connected != value)
                {
                    _connected = value;
                    OnPropertyChanged(nameof(Connected));
                    OnPropertyChanged(nameof(CurrentValuesStr));
                    OnPropertyChanged(nameof(DeviceSymbol));
                }
            }
        }

        /// <returns>
        /// Returns the matching image of the connection state.
        /// </returns>
        [JsonIgnore]
        public ImageSource DeviceSymbol => Connected switch
        {
            ConnectionStatus.Connected => ImageSource.FromFile("icon_cloud_done.png"),
            ConnectionStatus.Connecting => ImageSource.FromFile("icon_cloud_sync.png"),
            ConnectionStatus.Disconnected => ImageSource.FromFile("icon_cloud_unavailable.png"),
            ConnectionStatus.Error => ImageSource.FromFile("icon_cloud_cross.png"),
            _ => null,
        };

        public async Task<bool> Delete()
        {
            try
            {
                File.Delete(GetPath(PathType.Device, false));
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.Error, Strings.NoPermissionToDeleteDevice, Strings.OK);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.Error, ex.Message, Strings.OK);
            }
            return false;
        }
        public async Task<(bool device, bool contents)> DeleteWithContents()
        {
            bool success = true;

            foreach (PathType type in Enum.GetValues(typeof(PathType)))
            {
                try
                {
                    if (type == PathType.Device) { File.Delete(GetPath(type, false)); }
                    else { Directory.CreateDirectory(GetPath(type, false)); }
                }
                catch (DirectoryNotFoundException)
                {

                }
                catch (UnauthorizedAccessException)
                {
                    await Application.Current.MainPage.DisplayAlert(Strings.Error, Strings.NoPermissionToDeleteDeviceContents, Strings.OK);
                    success = false;
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(Strings.Error, ex.Message, Strings.OK);
                    success = false;
                }
            }
            return (await Delete(), success);
        }
        public async Task<bool> Save()
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
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
                    } while (File.Exists(GeneralPath + $"{code}.json"));
                    Id = code;
                }

                _ = Directory.CreateDirectory(GeneralPath);
                File.WriteAllText(GeneralPath + $"{Id}.json", JsonConvert.SerializeObject(this, Formatting.Indented));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.Error, Strings.NoPermissionToSaveDevice, Strings.OK);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.Error, ex.Message, Strings.OK);
            }
            return false;
        }
        public static async Task<Device> Load(string file)
        {
            try
            {
                _ = Directory.CreateDirectory(GeneralPath);
                return JsonConvert.DeserializeObject<Device>(File.ReadAllText(file));
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.Error, Strings.NoPermissionToLoadDevice, Strings.OK);
            }
            catch (FileNotFoundException)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.Error, Strings.DeviceNotFound, Strings.OK);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.Error, ex.Message, Strings.OK);
            }
            return new();
        }

        public bool CheckAdress(string input)
        {
            if (IPAddress.TryParse(input, out IPAddress address))
            {
                IpAddress = address.ToString();
                return true;
            }
            return false;
        }
        public IPAddress GetAdress()
        {
            if (IPAddress.TryParse(IpAddress, out IPAddress adress))
            {
                return adress;
            }
            else
            {
                return null;
            }
        }

        /// <param name="type">The type of path that is needed.</param>
        /// <param name="createDirectory">Creates the path so that it can be used directly.</param>
        public string GetPath(PathType type, bool createDirectory = true)
        {
            string path = type switch
            {
                PathType.Device => GeneralPath + $"/{_id}.json",
                PathType.Files => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "/Files/" + _id,
                PathType.Images => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "/Images/" + _id,
                PathType.Temporary => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "/temp/" + _id,
                _ => GeneralPath,
            };

            if (createDirectory)
            {
                try { Directory.CreateDirectory(path); } catch { }
            }

            return path;
        }
        public async Task<bool> LoadCurrentValues(string json = null)
        {
            try
            {
                CurrentValues values;
                if (string.IsNullOrEmpty(json))
                {
                    HttpResponseMessage response = await new HttpClient().GetAsync("http://" + IpAddress + "/json/current");
                    response.EnsureSuccessStatusCode();
                    values = JsonConvert.DeserializeObject<CurrentValues>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    values = JsonConvert.DeserializeObject<CurrentValues>(json);
                }

                CurrentTemp = values.CurrentTemp;
                CurrentHumi = values.CurrentHumi;
                CurrentPres = values.CurrentPres;

                ErrorOnCurrentValues = false;
                OnPropertyChanged(nameof(CurrentValuesStr));
                return true;
            }
            catch (Exception)
            {
                ErrorOnCurrentValues = true;
                return false;
            }
        }
        public Task LoadMeasurementPoints()
        {
            foreach (string file in Directory.GetFiles(GetPath(PathType.Files), "*.json"))
            {
                try
                {
                    MeasurementPoint p = MeasurementPoint.Load(file);
                    if (p != null && !MPoints.Exists(x => x.Time == p.Time))
                    {
                        MPoints.Add(p);
                    }
                }
                catch (Exception) { }
            }

            if (MPoints != null)
            {
                MPoints = MPoints.OrderBy(x => x.Time).ToList();
            }

            return Task.CompletedTask;
        }
        public async Task<bool> LoadImages(string json = null)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    HttpResponseMessage response = await new HttpClient().GetAsync("http://" + IpAddress + "/json/images");
                    response.EnsureSuccessStatusCode();
                    Images = JsonConvert.DeserializeObject<Dictionary<DateTime, string>>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Images = JsonConvert.DeserializeObject<Dictionary<DateTime, string>>(json);
                }

                if (Images != null)
                {
                    Images = Images.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<ConnectionStatus> CheckConnection(bool loadCurrentValues = false, bool demo = false)
        {
            if (CheckAdress(IpAddress))
            {
                Connected = ConnectionStatus.Connecting;

                try
                {
                    HttpResponseMessage response = await new HttpClient().GetAsync("http://" + IpAddress);
                    if (loadCurrentValues) _ = LoadCurrentValues(demo ? "{\"temperature\":" + new Random().Next(-10, 30) + "." + new Random().Next(0, 9) + ",\"humidity\":" + new Random().Next(20, 90) + ",\"pressure\":" + new Random().Next(900, 1200) + "}" : null);
                    Connected = ConnectionStatus.Connected;
                }
                catch (Exception)
                {
                    Connected = ConnectionStatus.Disconnected;
                }
            }
            else
            {
                Connected = ConnectionStatus.Error;
            }

            await RuntimeData.OutputData.RefreshConnectedDevices();
            return Connected;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public struct CurrentValues
        {
            [JsonProperty("temperature")]
            public float CurrentTemp { get; set; }
            [JsonProperty("humidity")]
            public float CurrentHumi { get; set; }
            [JsonProperty("pressure")]
            public float CurrentPres { get; set; }
        }
    }
}