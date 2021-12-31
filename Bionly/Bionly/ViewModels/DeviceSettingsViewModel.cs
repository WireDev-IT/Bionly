using Bionly.Views;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class DeviceSettingsViewModel : BaseViewModel
    {
        public static Models.Device Device { get; set; } = new();

        public ICommand SaveDevice => new Command(async () =>
        {
            await Device.Save();
            RuntimeData.LoadAllDevices.Execute(null);
        });

        public ICommand DeleteDevice => new Command(async () =>
        {
            if (!string.IsNullOrEmpty(Device.Id))
            {
                string result = await Application.Current.MainPage.DisplayActionSheet($"\"{Device.Name}\" löschen", "Abbrechen", "Nur Gerät", new string[] { "Gerät mit Inhalten" });
                if (result == "Gerät mit Inhalten")
                {
                    if (!(await Device.DeleteWithContents()).contents)
                    {
                        await Application.Current.MainPage.DisplayAlert("Löschen fehlgeschlagen", "Nicht alle Inhalte zu diesem Gerät konnten entfernt werden.", "OK");
                    }
                }
                else if (result == "Nur Gerät")
                {
                    await Device.Delete();
                }

                RuntimeData.Devices.Remove(Device);
                await Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
            }
        });

        public DeviceSettingsViewModel()
        {
            Title = "Gerät bearbeiten";
        }
    }
}
