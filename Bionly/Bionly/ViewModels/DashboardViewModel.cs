using Bionly.Models;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public LineChart tempChart;
        public LineChart humiChart;
        public LineChart presChart;
        public RadialGaugeChart radChart;

        public static List<MeasurementPoint> Values = new();

        public ICommand DrawGraphs => new Command(() =>
        {
            List<ChartEntry> tempEntries = new();
            List<ChartEntry> humiEntries = new();
            List<ChartEntry> presEntries = new();
            List<ChartEntry> radEntries = new();

            for (int i = 0; i < Values.Count; i++)
            {
                tempEntries.Add(new ChartEntry(Values[i].Temperature)
                {
                    Label = Values[i].Time.ToShortDateString(),
                    ValueLabel = Values[i].TemperatureStr,
                    Color = new SKColor(255, 0, 0)
                });
                humiEntries.Add(new ChartEntry(Values[i].Humidity)
                {
                    Label = Values[i].Time.ToShortDateString(),
                    ValueLabel = Values[i].HumidityStr,
                    Color = new SKColor(0, 0, 255)
                });
                presEntries.Add(new ChartEntry(Values[i].Pressure)
                {
                    Label = Values[i].Time.ToShortDateString(),
                    ValueLabel = Values[i].PressureStr
                });
            }

            radEntries.Add(new ChartEntry(50)
            {
                Color = new SKColor(0, 0, 0),
                ValueLabelColor = new SKColor(0, 0, 0),
                ValueLabel = "Luftdruck",
                Label = "1000 hPa"
            });
            radEntries.Add(new ChartEntry(30)
            {
                Color = new SKColor(0, 0, 255),
                ValueLabelColor = new SKColor(0, 0, 255),
                ValueLabel = "Feuchtigkeit",
                Label = "30 %"
            });
            radEntries.Add(new ChartEntry(60)
            {
                Color = new SKColor(255, 0, 0),
                ValueLabelColor = new SKColor(255, 0, 0),
                ValueLabel = "Temperatur",
                Label = "26 °C"
            });

            tempChart.Entries = tempEntries;
            humiChart.Entries = humiEntries;
            presChart.Entries = presEntries;
            radChart.Entries = radEntries;
        });

        public ICommand Refresh => new Command(() =>
        {
            SettingsViewModel.LoadAllDevices.Execute(null);
        });

        public DashboardViewModel()
        {
            Title = "Dashboard";
            tempChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                //BackgroundColor = new SKColor().WithAlpha(0),
            };
            humiChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                //BackgroundColor = new SKColor().WithAlpha(0)
            };
            presChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
            radChart = new()
            {
                MaxValue = 100,
                MinValue = 0,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
        }
    }
}