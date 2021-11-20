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
            _ = await Device.Save();
            SettingsViewModel.LoadAllDevices.Execute(null);
        });

        public DeviceSettingsViewModel()
        {
            Title = "Gerät bearbeiten";
        }
    }
}
