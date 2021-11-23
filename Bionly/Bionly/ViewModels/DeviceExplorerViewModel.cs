using Bionly.Models;
using FluentFTP;
using FluentFTP.Rules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;
using static Bionly.Enums.Sensor;

namespace Bionly.ViewModels
{
    public class DeviceExplorerViewModel : BaseViewModel
    {
        public static Models.Device Device { get; internal set; } = new();
        public ObservableCollection<MeasurementPoint> Stats { get; internal set; } = new();

        private FtpClient ftp;
        private CancellationToken token = new();
        public double progress;

        public ICommand ConnectFTPS => new Command(async () =>
        {
            ftp = new();
            ftp.Host = "192.168.178.62";
            await ftp.ConnectAsync(token);
        });

        public ICommand RefreshFiles => new Command(async () =>
        {
            List<FtpRule> rules = new()
            {
                    new FtpFileExtensionRule(true, new List<string>{ "json" })
                };
            Directory.CreateDirectory("Files");

            Progress<FtpProgress> prog = new(p => progress = p.Progress);
            List<FtpResult> results = await ftp.DownloadDirectoryAsync("/Files", @"/Files", FtpFolderSyncMode.Update, FtpLocalExists.Skip, FtpVerify.OnlyChecksum, rules, prog, token);

            foreach (FtpResult result in results)
            {
                if (result.IsSuccess)
                {
                    JsonMeasurementPoint point = JsonConvert.DeserializeObject<JsonMeasurementPoint>(File.ReadAllText(result.LocalPath));
                    List<MeasurementPoint> l = Stats.ToList();
                    if (!l.Exists(x => x.Time == point.Time))
                    {
                        Stats.Add(new MeasurementPoint() { Time = point.Time });
                        l.Add(new MeasurementPoint() { Time = point.Time });
                    }

                    int index = l.FindIndex(x => x.Time == point.Time);
                    if (point.GetSensorType() == SensorType.Temperature)
                    {
                        Stats[index].Temperature = point.Value;
                    }
                    else if (point.GetSensorType() == SensorType.Humidity)
                    {
                        Stats[index].Humidity = point.Value;
                    }
                    else if (point.GetSensorType() == SensorType.Pressure)
                    {
                        Stats[index].Pressure = point.Value;
                    }
                }
            }
        });

        public DeviceExplorerViewModel()
        {
            Title = Device.Name;
            //GenerateFiles();

            foreach (string file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "\\Files\\", "*.json"))
            {
                JsonMeasurementPoint point = JsonConvert.DeserializeObject<JsonMeasurementPoint>(File.ReadAllText(file));
                List<MeasurementPoint> l = Stats.ToList();
                if (!l.Exists(x => x.Time == point.Time))
                {
                    Stats.Add(new MeasurementPoint() { Time = point.Time });
                    l.Add(new MeasurementPoint() { Time = point.Time });
                }

                int index = l.FindIndex(x => x.Time == point.Time);
                if (point.GetSensorType() == SensorType.Temperature)
                {
                    Stats[index].Temperature = point.Value;
                }
                else if (point.GetSensorType() == SensorType.Humidity)
                {
                    Stats[index].Humidity = point.Value;
                }
                else if (point.GetSensorType() == SensorType.Pressure)
                {
                    Stats[index].Pressure = point.Value;
                }
            }
        }

        private void GenerateFiles()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "\\Files\\";
            Directory.CreateDirectory(path);
            string[] sensors = new string[] { "dht22-temperature", "dht22-humidity", "bmp180-temperature", "bmp180-pressure" };
            DateTime time = new();

            for (int i = 0; i < 20; i++)
            {
                time = time.AddDays(1);

                JsonMeasurementPoint j = new();
                j.Sensor = sensors[0];
                j.Value = new Random().Next(10, 30);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(path + sensors[0] + $"-{i}.json", JsonConvert.SerializeObject(j));

                j.Sensor = sensors[1];
                j.Value = new Random().Next(30, 80);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(path + sensors[1] + $"-{i}.json", JsonConvert.SerializeObject(j));

                j.Sensor = sensors[3];
                j.Value = new Random().Next(900, 1500);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(path + sensors[3] + $"-{i}.json", JsonConvert.SerializeObject(j));
            }
        }
    }
}
