using Bionly.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            UserTxt.Text = PassTxt.Text = "";
        }

        private void LoginBtn_Clicked(object sender, EventArgs e)
        {
            LoginViewModel.Account = new(UserTxt.Text, PassTxt.Text);
            ((LoginViewModel)BindingContext).LoginCommand.Execute(null);
        }

        private void GuestBtn_Clicked(object sender, EventArgs e)
        {
            LoginViewModel.Account = new("guest", "guest");
            ((LoginViewModel)BindingContext).LoginCommand.Execute(null);
        }
    }
}