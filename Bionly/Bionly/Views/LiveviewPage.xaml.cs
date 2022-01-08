using Bionly.ViewModels;
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

        /// <summary>
        /// Actions that are executed when a device is selected.
        /// </summary>
        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            VidView.MediaPlayer?.Stop();
            await ((LiveviewViewModel)BindingContext).SetPlayer(((KeyValuePair<string, Uri>)e.Item).Value);
            VidView.MediaPlayer?.Play();
        }

        /// <summary>
        /// Actions to stop the player.
        /// </summary>
        private void StopButton_Clicked(object sender, EventArgs e)
        {
            VidView.MediaPlayer?.Stop();
            DeviceList.SelectedItem = null;
            StopBtn.IsEnabled = false;
        }
    }
}