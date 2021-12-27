using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class DeviceSettingsViewModel : BaseViewModel
    {
        public static Models.Device Device{ get; internal set; } = new();

        public ICommand SaveDevice => new Command(async () =>
        {
            await Device.Save();
            SettingsViewModel.LoadAllDevices.Execute(null);
        });

        public ICommand DeleteDevice => new Command(async () =>
        {
            string result = await Application.Current.MainPage.DisplayActionSheet($"\"{Device.Name}\" löschen", "Abbrechen", "Nur Gerät", new string[] { "Gerät mit Inhalten" });
            if (result == "Gerät mit Inhalten")
            {
                if (!(await Device.DeleteWithContents()).contents)
                {
                    await Application.Current.MainPage.DisplayAlert("Löschen fehlgeschlagen", "Nicht alle Inhalte zu diesem Gerät konnten gelöscht werden.", "OK");
                }
            }
            else if (result == "Nur Gerät")
            {
                await Device.Delete();
            }

            SettingsViewModel.Devices.Remove(Device);
            SettingsViewModel.LoadAllDevices.Execute(null);
        });

        public DeviceSettingsViewModel()
        {
            Title = "Gerät bearbeiten";
        }
    }
}
