using Bionly.ViewModels;
using Bionly.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Bionly
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync("//LoginPage");
        }
    }
}
