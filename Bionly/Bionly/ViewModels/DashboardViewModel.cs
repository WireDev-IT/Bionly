using Bionly.Models;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using static Bionly.Enums.Connection;

namespace Bionly.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public RadialGaugeChart radChart;
        public Models.Device Device { get; private set; }

        public static List<MeasurementPoint> Values = new();

        public ICommand DrawGraphs => new Command(() =>
        {
            List<ChartEntry> radEntries = new();

            //
            //TODO: Berechnung
            //

            byte temperature = 0;
            byte pressure = 0;

            radEntries.Add(new ChartEntry(50)
            {
                Color = new SKColor(0, 0, 0),
                ValueLabelColor = new SKColor(0, 0, 0),
                ValueLabel = "Luftdruck",
                Label = pressure.ToString() + " hPa"
            });
            radEntries.Add(new ChartEntry(30)
            {
                Color = new SKColor(0, 0, 255),
                ValueLabelColor = new SKColor(0, 0, 255),
                ValueLabel = "Feuchtigkeit",
                Label = Device.CurrentHumi.ToString("00") + " %"
            });
            radEntries.Add(new ChartEntry(temperature)
            {
                Color = new SKColor(255, 0, 0),
                ValueLabelColor = new SKColor(255, 0, 0),
                ValueLabel = "Temperatur",
                Label = temperature.ToString("N1") + " °C"
            });

            radChart.Entries = radEntries;
        });

        public ICommand Refresh => new Command(() =>
        {
            SettingsViewModel.LoadAllDevices.Execute(null);

            foreach (Models.Device d in SettingsViewModel.Devices)
            {
                _ = d.GetCurrentValues();
            }
        });

        public DashboardViewModel()
        {
            Title = "Dashboard";
            radChart = new()
            {
                MaxValue = 100,
                MinValue = 0,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
        }

        public string GetConnectedText()
        {
            if (SettingsViewModel.Devices != null)
            {
                int count = SettingsViewModel.Devices.Count;

                if (count > 0)
                {
                    return SettingsViewModel.Devices.FindAll(x => x.Connected == ConnectionStatus.Connected).Count + $" von {count} Geräten verbunden";
                }
            }

            return "Keine Geräte konfiguriert";
        }

        public string GetWelcomeText()
        {
            if (DateTime.Now.Hour < 9)
            {
                return "Guten Morgen!";
            }
            else if (DateTime.Now.Hour < 18)
            {
                return "Guten Tag!";
            }

            return "Guten Abend!";
        }

        public ImageSource GetWelcomeImage()
        {
            if (DateTime.Now.Hour >= 22 || DateTime.Now.Hour < 5)
            {
                return "icon_night.png";
            }
            else if (DateTime.Now.Hour >= 5 && DateTime.Now.Hour < 10)
            {
                return "icon_morning.png";
            }
            else if (DateTime.Now.Hour >= 18 && DateTime.Now.Hour < 22)
            {
                return "icon_evening.png";
            }

            return "icon_afternoon.png";
        }
    }
}