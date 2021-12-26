using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using static Bionly.Enums.Path;

namespace Bionly.ViewModels
{
    public class ImagesViewModel : BaseViewModel
    {
        private Models.Device Device = null;

        public Dictionary<DateTime, ImageSource> Images { get; internal set; } = new();

        public ImagesViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                Images.Add(DateTime.Now, ImageSource.FromFile("Resources/bionly_logo.png"));
            }
        }

        public void Setup(string deviceId)
        {
            Device = SettingsViewModel.Devices.First(x => x.Id == deviceId);
            Title = $"Aufnahmen von \"{Device.Name}\"";

            foreach (string file in Directory.GetFiles(Device.GetPath(PathType.Images), "*.jpg"))
            {
                try
                {
                    Images.Add(File.GetCreationTime(file), ImageSource.FromFile(file));
                }
                catch (Exception) { }
            }

            Images = Images.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

    }
}
