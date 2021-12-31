using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Device = Bionly.Models.Device;

namespace Bionly
{
    public static class RuntimeData
    {
        public static List<Device> Devices { get; internal set; } = new();
        public static int SelectedDeviceIndex { get; internal set; } = -1;
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
            internal set => Devices[SelectedDeviceIndex] = value;
        }

        public static async Task LoadAllCurrentValues()
        {
            foreach (Device d in Devices)
            {
                await d.LoadCurrentValues("{\"temperature\":26.2,\"humidity\":40,\"pressure\":1020}");
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

            Devices.Sort((x, y) => (x.Name ?? "").CompareTo(y.Name ?? ""));
        });
    }
}
