using Bionly.Resx;
using Bionly.Views;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class DeviceSettingsViewModel : BaseViewModel
    {
        public static Models.Device Device { get; set; } = new();

        public ICommand SaveDevice => new Command(async () =>
        {
            if (await Device.Save())
            {
                if (RuntimeData.Devices.ToList().Exists(x => x.Id == Device.Id))
                {
                    RuntimeData.Devices[RuntimeData.Devices.ToList().FindIndex(x => x.Id == Device.Id)] = Device;
                }
                else
                {
                    RuntimeData.Devices.Add(Device);
                }
            }
        });

        public ICommand DeleteDevice => new Command(async () =>
        {
            if (!string.IsNullOrEmpty(Device.Id))
            {
                string result = await Application.Current.MainPage.DisplayActionSheet(string.Format(Strings.Delete_Name, Device.Name), Strings.Cancel, Strings.OnlyDevice, Strings.DeviceWithContents);
                if (result == Strings.DeviceWithContents)
                {
                    if (!(await Device.DeleteWithContents()).contents)
                    {
                        await Application.Current.MainPage.DisplayAlert(Strings.DeleteFailed, Strings.DeleteDeviceContentsFailed, Strings.OK);
                    }
                }
                else if (result == Strings.OnlyDevice)
                {
                    await Device.Delete();
                }

                RuntimeData.Devices.Remove(Device);
                await Shell.Current.Navigation.PopAsync();
                //await Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
            }
        });

        public DeviceSettingsViewModel()
        {
            Title = Strings.EditDevice;
        }
    }
}
