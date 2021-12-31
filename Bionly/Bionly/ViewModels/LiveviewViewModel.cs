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

            if (IsPlatfromOk)
            {
                Core.Initialize();
                LibVLC = new LibVLC();
            }
        }

        private LibVLC LibVLC { get; set; }

        /// <param name="uri">The uri for the stream</param>
        internal MediaPlayer GetPlayer(Uri uri)
        {
            return new MediaPlayer(new Media(LibVLC, uri));
        }

        /// <summary>
        /// Sets up the list of devices from which streaming is possible
        /// </summary>
        internal void OnAppearing()
        {
            if (IsPlatfromOk)
            {
                Connections.Clear();
                foreach (Models.Device device in RuntimeData.Devices)
                {
                    if (!string.IsNullOrEmpty(device.IpAddress)) Connections.Add(device.Name, new Uri($"{device.IpAddress}"));
                }
            }
        }

        /// <summary>
        /// Checks if the current runtime platform is compatible with VLC
        /// </summary>
        private bool IsPlatfromOk
        {
            get => Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS;
        }
    }
}