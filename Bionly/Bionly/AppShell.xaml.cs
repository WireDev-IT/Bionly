using Bionly.Views;
using System;
using Xamarin.Forms;

namespace Bionly
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void AccBtn_Clicked(object sender, EventArgs e)
        {
            await Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
