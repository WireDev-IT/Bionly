using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class LiveviewViewModel : BaseViewModel
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<string, Uri> Connections { get; set; } = new();

        public LiveviewViewModel()
        {
            Title = "Liveansicht";
            Initialize();
        }

        private LibVLC LibVLC { get; set; }

        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        private bool IsLoaded { get; set; }
        private bool IsVideoViewInitialized { get; set; }

        private void Set<T>(string propertyName, ref T field, T value)
        {
            if (field == null && value != null || field != null && !field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand PlayStream => new Command<string>((string name) =>
        {
            if (Connections.TryGetValue(name, out Uri uri))
            {
                OnDisappearing();
                MediaPlayer = new MediaPlayer(LibVLC) { Media = new Media(LibVLC, uri) };
            }
        });

        private Task Initialize()
        {
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
            {
                Core.Initialize();

                //LibVLC = new LibVLC(enableDebugLogs: true);
                //var media = new Media(LibVLC, new Uri("rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mp4"));

                //MediaPlayer = new MediaPlayer(LibVLC)
                //{
                //    Media = media
                //};

                //media.Dispose();

                Connections.Clear();
                foreach (Models.Device device in SettingsViewModel.Devices)
                {
                    Connections.Add(device.Name, new Uri($"{device.IpAddress}"));
                }
            }
            return Task.CompletedTask;
        }

        public void OnAppearing()
        {
            IsLoaded = true;
            Play();
        }

        internal void OnDisappearing()
        {
            if (MediaPlayer != null)
                MediaPlayer.Dispose();
        }

        public void OnVideoViewInitialized()
        {
            IsVideoViewInitialized = true;
            Play();
        }

        private void Play()
        {
            if (IsLoaded && IsVideoViewInitialized)
            {
                MediaPlayer.Play();
            }
        }
    }
}
