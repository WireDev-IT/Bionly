using Bionly.Models;
using Bionly.Resx;
using Bionly.Views;
using System.Windows.Input;
using Xamarin.Forms;
using static Bionly.Enums.User;

namespace Bionly.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public static bool IsLoggedIn { get; private set; } = false;
        public static UserType LoggedInUser { get; private set; } = UserType.Guest;
        internal static Account Account { get; set; } = new("guest", "guest");
        internal static Accounts users = new();

        public LoginViewModel()
        {
            users = Accounts.Load().Result;
        }

        public ICommand LoginCommand => new Command(async () =>
        {
            LoggedInUser = users.GetUserType(Account);
            if (LoggedInUser == UserType.Guest)
            {
                IsLoggedIn = true;
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            }
            else if (LoggedInUser == UserType.Admin)
            {
                IsLoggedIn = true;
                await Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
            }
            else
            {
                IsLoggedIn = false;
                await Application.Current.MainPage.DisplayAlert(Strings.Error, Strings.IncorrectUserCredits, Strings.OK);
            }
        });
    }
}