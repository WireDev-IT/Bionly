using Bionly.Views;
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

        public static ICommand LoadAllDevices => new Command(async () =>
        {
            Devices.Clear();
            _ = Directory.CreateDirectory(Models.Device.path);
            foreach (string file in Directory.GetFiles(Models.Device.path, "*.json", SearchOption.TopDirectoryOnly))
            {
                Models.Device d = await Models.Device.Load(file);
                Devices.Add(d);
            }
        });

        public SettingsViewModel()
        {
            Title = "Einstellungen";
        }
    }
}
