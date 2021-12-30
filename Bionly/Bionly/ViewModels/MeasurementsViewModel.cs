using Bionly.Models;
using FluentFTP;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Bionly.Enums.Path;
using static Bionly.Enums.Sensor;

namespace Bionly.ViewModels
{
    public class MeasurementsViewModel : BaseViewModel
    {
        public Models.Device Device { get; private set; }

        private readonly FtpClient ftp;
        private CancellationToken token = new();
        public double progress;

        public MeasurementsViewModel()
        {
            Title = "Kein Gerät für Messwerte ausgewählt";
        }

        public MeasurementsViewModel(string deviceId)
        {
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                Device = SettingsViewModel.Devices.FirstOrDefault(x => x.Id == deviceId);
                Title = $"Messwerte von \"{Device.Name}\"";

                foreach (string file in Directory.GetFiles(Device.GetPath(PathType.Files), "*.json"))
                {
                    try
                    {
                        MeasurementPoint p = MeasurementPoint.Load(file);
                        if (!Device.MPoints.Exists(x => x.Time == p.Time))
                        {
                            Device.MPoints.Add(p);
                        }
                    }
                    catch (Exception) { }
                }

                Device.MPoints = Device.MPoints.OrderBy(x => x.Time).ToList();
            }
            else
            {
                Title = "Kein Gerät für Messwerte ausgewählt";
            }
        }

        private Task GenerateDemoFiles()
        {
            string[] sensors = new string[] { "dht22-temperature", "dht22-humidity", "bmp180-temperature", "bmp180-pressure" };
            DateTime time = new();

            for (int i = 0; i < 20; i++)
            {
                time = time.AddDays(1);

                JsonMeasurementPoint j = new();
                j.Sensor = sensors[0];
                j.Value = new Random().Next(10, 30);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(Device.GetPath(PathType.Temporary) + $"/{sensors[0]}-{i}.json", JsonConvert.SerializeObject(j));

                j.Sensor = sensors[1];
                j.Value = new Random().Next(30, 80);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(Device.GetPath(PathType.Temporary) + $"/{sensors[1]}-{i}.json", JsonConvert.SerializeObject(j));

                j.Sensor = sensors[3];
                j.Value = new Random().Next(900, 1500);
                j.TimeString = time.ToString("yyyy-MM-dd_HH-mm-ss");
                File.WriteAllText(Device.GetPath(PathType.Temporary) + $"/{sensors[3]}-{i}.json", JsonConvert.SerializeObject(j));
            }
            return Task.CompletedTask;
        }

        public ICommand RefreshFiles => new Command(async () =>
        {
            //ftp = new() { Host = "192.168.178.62" };
            //await ftp.ConnectAsync(token);

            //List<FtpRule> rules = new()
            //{
            //    new FtpFileExtensionRule(true, new List<string>{ "json" }),
            //};
            //Directory.CreateDirectory(GetDevicePath());
            //Directory.CreateDirectory(GetTempPath());

            //Progress<FtpProgress> prog = new(p => progress = p.Progress);
            //List<FtpResult> results = await ftp.DownloadDirectoryAsync(GetTempPath(), @"/Files", verifyOptions: FtpVerify.OnlyChecksum, rules: rules, progress: prog, token: token);

            await GenerateDemoFiles();
            List<FtpResult> results = new();
            foreach (string file in Directory.GetFiles(Device.GetPath(PathType.Temporary), "*.json"))
            {
                results.Add(new() { IsSuccess = true, LocalPath = file });
            }

            foreach (FtpResult result in results)
            {
                if (result.IsSuccess)
                {
                    JsonMeasurementPoint point = JsonConvert.DeserializeObject<JsonMeasurementPoint>(File.ReadAllText(result.LocalPath));

                    if (!Device.MPoints.Exists(x => x.Time == point.Time))
                    {
                        Device.MPoints.Add(new MeasurementPoint() { Time = point.Time });
                    }

                    int index = Device.MPoints.FindIndex(x => x.Time == point.Time);
                    if (point.SensorType == SensorType.Temperature)
                    {
                        Device.MPoints[index].Temperature = point.Value;
                    }
                    else if (point.SensorType == SensorType.Humidity)
                    {
                        Device.MPoints[index].Humidity = point.Value;
                    }
                    else if (point.SensorType == SensorType.Pressure)
                    {
                        Device.MPoints[index].Pressure = point.Value;
                    }

                    Device.MPoints[index].Save(Device.GetPath(PathType.Files));
                    File.Delete(result.LocalPath);
                }
            }

            Device.MPoints = Device.MPoints.OrderBy(x => x.Time).ToList();
        });
    }
}
