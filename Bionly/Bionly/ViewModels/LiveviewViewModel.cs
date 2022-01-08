using Bionly.Resx;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class LiveviewViewModel : BaseViewModel
    {
        private static LibVLC LibVLC { get; set; }

        private bool _isBuffering = false;
        /// <summary>
        /// Can be bound to the UI even if player is null.
        /// </summary>
        /// <returns>
        /// true, when a video is starting to play, otherwise false
        /// </returns>
        public bool IsBuffering
        {
            get => _isBuffering;
            set
            {
                if (_isBuffering != value)
                {
                    _isBuffering = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isPlaying = false;
        /// <summary>
        /// Can be bound to the UI even if player is null.
        /// </summary>
        /// <returns>
        /// true, when a video is playing, otherwise false
        /// </returns>
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged();
                }
            }
        }

        private MediaPlayer _player;
        /// <summary>
        /// The MediaPlayer object that can be bound to the user interface.
        /// </summary>
        public MediaPlayer Player
        {
            get => _player;
            set
            {
                if (_player != value)
                {
                    _player = value;
                    OnPropertyChanged();

                    Player.Playing += Player_Playing;
                    Player.Stopped += Player_Stopped;
                    Player.Opening += Player_Opening;
                }
            }
        }

        private void Player_Opening(object sender, EventArgs e)
        {
            IsBuffering = true;
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

            if (IsPlatformOk)
            {
                Core.Initialize();
                LibVLC = new LibVLC();
            }

        }

        /// <param name="uri">The uri for the stream</param>
        internal Task SetPlayer(Uri uri)
        {
            Player = new MediaPlayer(new Media(LibVLC, uri));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets up the list of devices from which streaming is possible
        /// </summary>
        internal void OnAppearing()
        {
            if (IsPlatformOk)
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
        public bool IsPlatformOk
        {
            get => Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS;
        }
    }
}