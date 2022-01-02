using Bionly.Properties;
using Bionly.Resx;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = Strings.About;
            AboutText = Resources.AboutText;
        }

        public string AboutText { get; }

        public Uri RepoLink { get; } = new Uri("https://github.com/tomo2403/bionly");
        public Uri DeveloperLink { get; } = new Uri("https://github.com/tomo2403");
        public Uri ProjectLeaderLink { get; } = new Uri("http://www.schulbiologiezentrum-leipzig.de/");

        public ICommand OpenWebCommand => new Command<string>((url) =>
        {
            _ = Launcher.OpenAsync(new Uri(url));
        });
    }
}