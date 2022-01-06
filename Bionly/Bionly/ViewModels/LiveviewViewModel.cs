using Bionly.Resx;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class LiveviewViewModel : BaseViewModel
    {
        private static LibVLC LibVLC { get; set; }

        private bool _isBuffering = false;
        public bool IsBuffering
        {
            get => _isBuffering;
            set
            {
                if (_isBuffering != value)
                {
                    _isBuffering = value;
                    OnPropertyChanged(nameof(IsBuffering));
                }
            }
        }

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }

        private MediaPlayer _player;
        public MediaPlayer Player
        {
            get => _player;
            set
            {
                if (_player != value)
                {
                    _player = value;
                    OnPropertyChanged(nameof(Player));

                    Player.Playing += Player_Playing;
                    Player.Stopped += Player_Stopped;

                    IsBuffering = true;
                    Player.Play();
                }
            }
        }

        private void Player_Stopped(object sender, EventArgs e)
        {
            IsPlaying = false;
        }

        private void Player_Playing(object sender, EventArgs e)
        {
            IsBuffering = false;
            IsPlaying = true;
        }

        /// <summary>
        /// Contains all devices with name and stream uri that can be used in liveview.
        /// </summary>
        public Dictionary<string, Uri> Connections { get; set; } = new();

        /// <summary>
        /// Sets title of page and initializes the VLC library if needed.
        /// </summary>
        public LiveviewViewModel()
        {
            Title = Strings.Liveview;

            if (IsPlatfromOk)
            {
                Core.Initialize();
                LibVLC = new LibVLC();
            }
        }

        /// <param name="uri">The uri for the stream</param>
        internal void SetPlayer(Uri uri)
        {
            Player = new MediaPlayer(new Media(LibVLC, uri));
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
                    if (!string.IsNullOrEmpty(device.IpAddress)) Connections.Add(device.Name, new Uri($"rtsp://{device.IpAddress}"));
                }
            }
        }

        /// <summary>
        /// true, if the runtime platform is compatible with VLC
        /// </summary>
        public bool IsPlatfromOk
        {
            get => Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS;
        }
    }
}