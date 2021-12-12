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
            ((LiveviewViewModel)BindingContext).OnDisappearing();
        }

        private void VideoView_MediaPlayerChanged(object sender, MediaPlayerChangedEventArgs e)
        {
            ((LiveviewViewModel)BindingContext).OnVideoViewInitialized();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((LiveviewViewModel)BindingContext).PlayStream.Execute(((KeyValuePair<string, Uri>)e.Item).Value);
        }
    }
}