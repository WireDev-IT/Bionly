using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class ImagesViewModel : BaseViewModel
    {
        public ImagesViewModel()
        {
            Refresh.Execute(null);
        }

        public ICommand Refresh => new Command(async () =>
        {
            //await RuntimeData.SelectedDevice.LoadImages();


            //
            //Demo
            //
            Dictionary<DateTime, string> images = new();
            images.Add(DateTime.Now, "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Ftse3.mm.bing.net%2Fth%3Fid%3DOIP.MrTrcxnBVySnvXWFSOxg6wHaEK%26pid%3DApi&f=1");
            images.Add(DateTime.Now.AddHours(1), "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Ftse2.mm.bing.net%2Fth%3Fid%3DOIP.7DOCvrN6aQ_IV5Yyo3xnLwHaEd%26pid%3DApi&f=1");
            images.Add(DateTime.Now.AddHours(2), "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Ftse2.mm.bing.net%2Fth%3Fid%3DOIP.piWve27IefZeDejDCAu79QHaE7%26pid%3DApi&f=1");
            await RuntimeData.SelectedDevice.LoadImages(JsonConvert.SerializeObject(images, Formatting.Indented));
        });

    }
}
