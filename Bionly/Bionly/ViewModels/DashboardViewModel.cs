using Bionly.Resx;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using static Bionly.Enums.Connection;

namespace Bionly.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public RadialGaugeChart radChart;
        public int SelectedIndex { get; set; } = -1;

        public ICommand DrawGraphs => new Command(() =>
        {
            if (RuntimeData.SelectedDevice != null)
            {
                List<ChartEntry> radEntries = new();
                float temperature = 100 - (100 - (RuntimeData.SelectedDevice.CurrentTemp + 30));
                float pressure = 100 - ((1120 - RuntimeData.SelectedDevice.CurrentPres) / 2);

                if (temperature > 100)
                {
                    temperature = 100;
                }
                else if (temperature < 0)
                {
                    temperature = 0;
                }

                if (pressure > 100)
                {
                    pressure = 100;
                }
                else if (pressure < 0)
                {
                    pressure = 0;
                }

                radEntries.Add(new ChartEntry(pressure)
                {
                    Color = new SKColor(0, 0, 0),
                    ValueLabelColor = new SKColor(0, 0, 0),
                    ValueLabel = Strings.AirPressure,
                    Label = RuntimeData.SelectedDevice.CurrentPres.ToString() + " hPa"
                });
                radEntries.Add(new ChartEntry(RuntimeData.SelectedDevice.CurrentHumi)
                {
                    Color = new SKColor(0, 0, 255),
                    ValueLabelColor = new SKColor(0, 0, 255),
                    ValueLabel = Strings.Humidity,
                    Label = RuntimeData.SelectedDevice.CurrentHumi.ToString("00") + " %"
                });
                radEntries.Add(new ChartEntry(temperature)
                {
                    Color = new SKColor(255, 0, 0),
                    ValueLabelColor = new SKColor(255, 0, 0),
                    ValueLabel = Strings.Temperatur,
                    Label = RuntimeData.SelectedDevice.CurrentTemp.ToString("N1") + " °C"
                });

                radChart.Entries = radEntries;
            }
        });

        public ICommand Refresh => new Command(async () =>
        {
            await RuntimeData.ConnectAllDevices();
            await RuntimeData.LoadAllCurrentValues(true);
        });

        public DashboardViewModel()
        {
            Title = Strings.Dashboard;
            radChart = new()
            {
                MaxValue = 100,
                MinValue = 0,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
            Refresh.Execute(null);
        }

        public string GetConnectedText()
        {
            if (RuntimeData.Devices != null)
            {
                int count = RuntimeData.Devices.Count;

                if (count > 0)
                {
                    return string.Format(Strings.ConnectedDevicesCountTxt, RuntimeData.Devices.ToList().FindAll(x => x.Connected == ConnectionStatus.Connected).Count, count);
                }
            }

            return Strings.NoDevicesConfigured;
        }

        public string GetWelcomeText()
        {
            if (DateTime.Now.Hour < 9)
            {
                return Strings.GoodMorning;
            }
            else if (DateTime.Now.Hour < 18)
            {
                return Strings.GoodDay;
            }

            return Strings.GoodEvening;
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