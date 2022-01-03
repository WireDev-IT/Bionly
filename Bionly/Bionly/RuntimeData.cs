using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
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

        private static ObservableCollection<Device> _devices = new();
        public static ObservableCollection<Device> Devices
        {
            get => _devices;
            set
            {
                if (_devices != value)
                {
                    _devices = value;
                    OnStaticPropertyChanged(nameof(Devices));
                }
            }
        }

        private static int _selectedDeviceIndex= -1;
        public static int SelectedDeviceIndex
        {
            get => _selectedDeviceIndex;
            set
            {
                if (_selectedDeviceIndex != value)
                {
                    _selectedDeviceIndex = value;
                    OnStaticPropertyChanged(nameof(SelectedDeviceIndex));
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
                    OnStaticPropertyChanged(nameof(SelectedDevice));
                }
            }
        }

        public static async Task ConnectAllDevices()
        {
            foreach (Device d in Devices)
            {
                _ = await d.CheckConnection();
            }
        }

        public static async Task LoadAllCurrentValues(bool demo = false)
        {
            foreach (Device d in Devices)
            {
                await d.LoadCurrentValues(demo ? "{\"temperature\":26.2,\"humidity\":40,\"pressure\":1020}" : null);
            }
        }
        public static async Task LoadAllMeasurementPoints()
        {
            foreach (Device d in Devices)
            {
                await d.LoadMeasurementPoints();
            }
        }

        public static ICommand LoadAllDevices => new Command(async () =>
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
        });
    }
}