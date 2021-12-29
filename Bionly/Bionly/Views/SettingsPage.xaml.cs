using Bionly.Models;
using Bionly.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Bionly.Enums.User;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ((SettingsViewModel)BindingContext).ControlsEnabled = LoginViewModel.IsLoggedIn;
        }

        private async void DevicesView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            DeviceSettingsViewModel.Device = (Models.Device)e.Item;
            await Navigation.PushAsync(new DeviceSettingsPage());
        }

        private async void CreateDevice(object sender, EventArgs e)
        {
            DeviceSettingsViewModel.Device = new();
            await Navigation.PushAsync(new DeviceSettingsPage());
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            UserTxt.Text = LoginViewModel.Account.Name;
            if (LoginViewModel.LoggedInUser != UserType.Admin)
            {
                MainContainer.IsVisible = false;
                foreach (ToolbarItem t in ToolbarItems)
                {
                    t.IsEnabled = false;
                }
                GuestTxt.IsVisible = true;
            }
            else
            {
                MainContainer.IsVisible = true;
                foreach (ToolbarItem t in ToolbarItems)
                {
                    t.IsEnabled = true;
                }
                GuestTxt.IsVisible = false;
            }
        }

        private async void SaveUserBtn_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UserTxt.Text))
            {
                if (!string.IsNullOrWhiteSpace(OldPassTxt.Text) && Account.GetHashString(OldPassTxt.Text) == LoginViewModel.Account.Password)
                {
                    if (string.IsNullOrWhiteSpace(NewPassTxt.Text))
                    {
                        LoginViewModel.users.Remove(LoginViewModel.Account);
                        Account a = new(UserTxt.Text, OldPassTxt.Text);
                        LoginViewModel.users.Add(a);
                        LoginViewModel.Account = a;
                        await LoginViewModel.users.Save();
                        await DisplayAlert("Gespeichert", $"Der neue Benutzername ist jetzt \"{UserTxt.Text}\"!", "OK");
                        ResetUserBtn_Clicked(sender, e);
                    }
                    else if (NewPassTxt.Text == NewPassTxt2.Text)
                    {
                        LoginViewModel.users.Remove(LoginViewModel.Account);
                        Account a = new(UserTxt.Text, NewPassTxt.Text);
                        LoginViewModel.users.Add(a);
                        LoginViewModel.Account = a;
                        await LoginViewModel.users.Save();
                        await DisplayAlert("Gespeichert", $"Das neues Passwort für den Benutzer \"{UserTxt.Text}\" wurde gespeichert!", "OK");
                        ResetUserBtn_Clicked(sender, e);
                    }
                    else
                    {
                        await DisplayAlert("Fehler", "Neues Passwort stimmt nicht überein", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Fehler", "Altes Passwort nicht korrekt", "OK");
                }
            }
            else
            {
                await DisplayAlert("Fehler", "Benutzername darf nicht leer sein", "OK");
            }
        }

        private void ResetUserBtn_Clicked(object sender, EventArgs e)
        {
            UserTxt.Text = LoginViewModel.Account.Name;
            OldPassTxt.Text = NewPassTxt.Text = NewPassTxt2.Text = "";
        }
    }
}