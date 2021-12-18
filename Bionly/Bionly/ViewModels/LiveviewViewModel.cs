using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class LiveviewViewModel : BaseViewModel
    {
        public bool IsPlaying { get; set; } = false;
        public Dictionary<string, Uri> Connections { get; set; } = new();

        public LiveviewViewModel()
        {
            Title = "Liveansicht";
        }

        private LibVLC LibVLC { get; set; }

        internal MediaPlayer GetPlayer(Uri uri)
        {
            return new MediaPlayer(new Media(LibVLC, uri));
        }

        internal void OnAppearing()
        {
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                Core.Initialize();
                LibVLC = new LibVLC();

                Connections.Clear();
                foreach (Models.Device device in SettingsViewModel.Devices)
                {
                    Connections.Add(device.Name, new Uri($"{device.IpAddress}"));
                }
            }
        }
    }
}