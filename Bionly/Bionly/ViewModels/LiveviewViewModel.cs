using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Bionly.ViewModels
{
    public class LiveviewViewModel : BaseViewModel
    {
        new public event PropertyChangedEventHandler PropertyChanged;

        public LiveviewViewModel()
        {
            Title = "Live Ansicht";
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

        private void Initialize()
        {
            Core.Initialize();

            LibVLC = new LibVLC();
            var media = new Media(LibVLC, new Uri("rtsp://demo:demo@ipvmdemo.dyndns.org:5541/onvif-media/media.amp?profile=profile_1_h264&sessiontimeout=60&streamtype=unicast"));

            MediaPlayer = new MediaPlayer(LibVLC)
            {
                Media = media,
            };

            media.Dispose();
        }

        public void OnAppearing()
        {
            IsLoaded = true;
            Play();
        }

        internal void OnDisappearing()
        {
            //MediaPlayer.Dispose();
            //LibVLC.Dispose();
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
