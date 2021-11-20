using FluentFTP;
using FluentFTP.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class DeviceExplorerViewModel : BaseViewModel
    {
        public static Models.Device Device { get; internal set; } = new();

        private FtpClient ftp;
        CancellationToken token = new();
        public double progress;

        public ICommand ConnectFTPS => new Command(async () =>
        {

        });

        public ICommand RefreshFiles => new Command(async () =>
        {

        });

        public DeviceExplorerViewModel()
        {
            Title = Device.Name;
            //GetListing();
        }

        public async void GetListing()
        {
            using (var conn = new FtpClient())
            {
                conn.Host = "192.168.178.62";
                await conn.ConnectAsync(token);
            }

            var rules = new List<FtpRule>
                {
                    new FtpFileExtensionRule(true, new List<string>{ "json" })
                };
            Directory.CreateDirectory("/Files");

            Progress<FtpProgress> prog = new(p => progress = p.Progress);
            await ftp.DownloadDirectoryAsync("/Files", @"/Files", FtpFolderSyncMode.Update, FtpLocalExists.Skip, FtpVerify.OnlyChecksum, rules, prog, token);
        }
    }
}
