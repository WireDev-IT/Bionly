using Bionly.Resx;
using Bionly.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Bionly.Enums.Connection;
using Device = Bionly.Models.Device;

namespace Bionly
{
    public class RuntimeData
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;
        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        private static OutputData _outputData = new();
        public static OutputData OutputData
        {
            get => _outputData;
            set
            {
                _outputData = value;
                OnStaticPropertyChanged();
            }
        }

        private static ObservableCollection<Device> _devices = new();
        public static ObservableCollection<Device> Devices
        {
            get => _devices;
            set
            {
                if (_devices != value)
                {
                    _devices = value;
                    OnStaticPropertyChanged();
                }
            }
        }

        private static int _selectedDeviceIndex = -1;
        public static int SelectedDeviceIndex
        {
            get => _selectedDeviceIndex;
            set
            {
                if (_selectedDeviceIndex != value)
                {
                    _selectedDeviceIndex = value;
                    OnStaticPropertyChanged();
                }
            }
        }

        public static Device SelectedDevice
        {
            get
            {
                if (SelectedDeviceIndex < 0 || SelectedDeviceIndex > Devices.Count - 1)
                {
                    return null;
                }
                return Devices[SelectedDeviceIndex];
            }
            internal set
            {
                if (Devices[SelectedDeviceIndex] != value)
                {
                    Devices[SelectedDeviceIndex] = value;
                    OnStaticPropertyChanged();
                }
            }
        }

        public static async Task ConnectAllDevicesAsync(bool loadCurrentValues = false)
        {
            await Task.WhenAll(Devices.Select(i => i.CheckConnection(loadCurrentValues, loadCurrentValues)));
        }

        public static async Task LoadAllCurrentValuesAsync(bool demo = false)
        {
            await Task.WhenAll(Devices.Select(i => i.LoadCurrentValues(demo ? "{\"temperature\":" + new Random().Next(-10, 30) + ".3,\"humidity\":" + new Random().Next(20, 90) + ",\"pressure\":" + new Random().Next(900, 1200) + "}" : null)));
        }

        public static async Task LoadAllMeasurementPointsAsync()
        {
            foreach (Device d in Devices)
            {
                await d.LoadMeasurementPoints();
            }
        }

        public static async Task LoadAllDevices()
        {
            Devices.Clear();
            _ = Directory.CreateDirectory(Device.GeneralPath);
            foreach (string file in Directory.GetFiles(Device.GeneralPath, "*.json", SearchOption.TopDirectoryOnly))
            {
                Device d = await Device.Load(file);
                Devices.Add(d);
            }

            List<Device> dl = Devices.ToList();
            dl.Sort((x, y) => (x.Name ?? "").CompareTo(y.Name ?? ""));
            Devices = new ObservableCollection<Device>(dl);
        }
    }

    public class OutputData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Task RefreshConnectedDevices()
        {
            OnPropertyChanged(nameof(ConnectedDevices));
            OnPropertyChanged(nameof(ConnectedDevicesText));
            return Task.CompletedTask;
        }

        public string ConnectedDevicesText
        {
            get
            {
                if (RuntimeData.Devices != null)
                {
                    int i = RuntimeData.Devices.Count;
                    if (i > 0)
                    {
                        return string.Format(Strings.ConnectedDevicesCountTxt, RuntimeData.OutputData.ConnectedDevices, i);
                    }
                }

                return Strings.NoDevicesConfigured;
            }
        }

        public int ConnectedDevices => (RuntimeData.Devices == null || RuntimeData.Devices.Count == 0) ? 0 : RuntimeData.Devices.ToList().FindAll(x => x.Connected == ConnectionStatus.Connected).Count;
    }
}