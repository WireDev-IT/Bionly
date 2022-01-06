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

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((LiveviewViewModel)BindingContext).SetPlayer(((KeyValuePair<string, Uri>)e.Item).Value);
        }

        private void StopButton_Clicked(object sender, EventArgs e)
        {
            if (((LiveviewViewModel)BindingContext).Player != null)
            {
                ((LiveviewViewModel)BindingContext).Player.Dispose();
            }
            DeviceList.SelectedItem = null;
            StopBtn.IsEnabled = false;
        }
    }
}