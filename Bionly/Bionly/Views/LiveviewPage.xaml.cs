using Bionly.ViewModels;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LiveviewPage : ContentPage
    {
        public LiveviewPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((LiveviewViewModel)BindingContext).OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopButton_Clicked(new object(), new EventArgs());
        }

        private void VideoView_MediaPlayerChanged(object sender, MediaPlayerChangedEventArgs e)
        {
            ActIndicator.IsRunning = true;
            VidView.MediaPlayer.Play();
            StopBtn.IsEnabled = true;
            ((LiveviewViewModel)BindingContext).IsPlaying = true;
            //DeviceList.IsEnabled = false;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (((LiveviewViewModel)BindingContext).IsPlaying)
            {
                VidView.MediaPlayer.Stop();
                VidView.MediaPlayer.Dispose();
            }
            VidView.MediaPlayer = ((LiveviewViewModel)BindingContext).GetPlayer(((KeyValuePair<string, Uri>)e.Item).Value);
            VidView.MediaPlayer.Playing += MediaPlayer_Playing;
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            ActIndicator.IsRunning = false;
        }

        private void StopButton_Clicked(object sender, EventArgs e)
        {
            if (VidView.MediaPlayer != null)
            {
                VidView.MediaPlayer.Stop();
                VidView.MediaPlayer.Dispose();
            }
            DeviceList.SelectedItem = null;
            StopBtn.IsEnabled = false;
            ((LiveviewViewModel)BindingContext).IsPlaying = false;
            //DeviceList.IsEnabled = true;
        }
    }
}