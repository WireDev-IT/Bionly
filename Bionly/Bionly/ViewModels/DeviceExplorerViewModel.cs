using Bionly.Models;
using FluentFTP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Bionly.Enums.Sensor;

namespace Bionly.ViewModels
{
    public class DeviceExplorerViewModel : BaseViewModel
    {
        private static Models.Device _device = new();
        public static Models.Device Device
        {
            get => _device;
            internal set
            {
                if (_device != value)
                {
                    _device = value;
                    Directory.CreateDirectory(GetDevicePath());
                    Directory.CreateDirectory(GetTempPath());
                }
            }
        }
        public List<MeasurementPoint> Stats { get; internal set; } = new();
        public Dictionary<DateTime, ImageSource> Images { get; internal set; } = new();

        private FtpClient ftp;
        private CancellationToken token = new();
        public double progress;

        private static string GetDevicePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "\\Files\\" + Device.Id;
        }

        private bool CheckExistingFile(DateTime time)
        {
            return File.Exists(GetDevicePath() + $"\\{time.Ticks}.json");
        }

        private static string GetTempPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "\\temp\\" + Device.Id;
        }

        public ICommand ConnectFTPS => new Command(async () =>
        {
            ftp = new();
            ftp.Host = "192.168.178.62";
            await ftp.ConnectAsync(token);
        });

        public ICommand RefreshFiles => new Command(async () =>
        {
            //List<FtpRule> rules = new()
            //{
            //    new FtpFileExtensionRule(true, new List<string>{ "json" }),
            //};
            //Directory.CreateDirectory(GetDevicePath());
            //Directory.CreateDirectory(GetTempPath());

            //Progress<FtpProgress> prog = new(p => progress = p.Progress);
            //List<FtpResult> results = await ftp.DownloadDirectoryAsync(GetTempPath(), @"/Files", verifyOptions: FtpVerify.OnlyChecksum, rules: rules, progress: prog, token: token);

            List<FtpResult> results = new();
            foreach (string file in Directory.GetFiles(GetTempPath(), "*.json"))
            {
                results.Add(new() { IsSuccess = true, LocalPath = file });
            }

            foreach (FtpResult result in results)
            {
                if (result.IsSuccess)
                {
                    JsonMeasurementPoint point = JsonConvert.DeserializeObject<JsonMeasurementPoint>(File.ReadAllText(result.LocalPath));

                    if (!Stats.Exists(x => x.Time == point.Time))
                    {
                        Stats.Add(new MeasurementPoint() { Time = point.Time });
                    }

                    int index = Stats.FindIndex(x => x.Time == point.Time);
                    if (point.SensorType == SensorType.Temperature)
                    {
                        Stats[index].Temperature = point.Value;
                    }
                    else if (point.SensorType == SensorType.Humidity)
                    {
                        Stats[index].Humidity = point.Value;
                    }
                    else if (point.SensorType == SensorType.Pressure)
                    {
                        Stats[index].Pressure = point.Value;
                    }

                    Stats[index].Save(GetDevicePath());
                    File.Delete(result.LocalPath);
                }
            }
        });

        public DeviceExplorerViewModel()
        {
            Title = Device.Name;
            //_ = GenerateFiles();

            foreach (string file in Directory.GetFiles(GetDevicePath(), "*.json"))
            {
                try
                {
                    Stats.Add(MeasurementPoint.Load(file));
                }
                catch (Exception) { }
            }

            Stats = Stats.OrderBy(x => x.Time).ToList();
            DashboardViewModel.Values = Stats.ToList();

            for (int i = 0; i < 10; i++)
            {
                Images.Add(DateTime.Now, ImageSource.FromFile("Resources/bionly_logo.png"));
            }
        }

        private Task GenerateFiles()
        {
            Directory.CreateDirectory(GetDevicePath());
            string[] sensors = new string[] { "dht22-temperature", "dht22-humidity", "bmp180-temperature", "bmp180-pressure" };
            DateTime time = new();

            for (int i = 0; i < 20; i++)
            {
                time = time.AddDays(1);

                JsonMeasurementPoint j = new();
                j.Sensor = sensors[0];
                j.Value = new Random().Next(10, 30);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(GetTempPath() + $"\\{sensors[0]}-{i}.json", JsonConvert.SerializeObject(j));

                j.Sensor = sensors[1];
                j.Value = new Random().Next(30, 80);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(GetTempPath() + $"\\{sensors[1]}-{i}.json", JsonConvert.SerializeObject(j));

                j.Sensor = sensors[3];
                j.Value = new Random().Next(900, 1500);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(GetTempPath() + $"\\{sensors[3]}-{i}.json", JsonConvert.SerializeObject(j));
            }

            RefreshFiles.Execute(this);
            return Task.CompletedTask;
        }
    }
}
