using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Bionly.Models;
using System.Collections.ObjectModel;
using Microcharts.Forms;

namespace Bionly.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public LineChart tempChart;
        public LineChart humiChart;
        public LineChart presChart;

        public static List<MeasurementPoint> Values = new();

        public ICommand DrawGraphs => new Command(() =>
        {
            List<ChartEntry> tempEntries = new();
            List<ChartEntry> humiEntries = new();
            List<ChartEntry> presEntries = new();

            for (int i = 0; i < Values.Count; i++)
            {
                tempEntries.Add(new ChartEntry(Values[i].Temperature)
                {
                    Label = Values[i].Time.ToShortDateString(),
                    ValueLabel = Values[i].GetTemperature,
                    Color = new SKColor(255, 0, 0)
                });
                humiEntries.Add(new ChartEntry(Values[i].Humidity)
                {
                    Label = Values[i].Time.ToShortDateString(),
                    ValueLabel = Values[i].GetHumidity,
                    Color = new SKColor(0, 0, 255)
                });
                presEntries.Add(new ChartEntry(Values[i].Pressure)
                {
                    Label = Values[i].Time.ToShortDateString(),
                    ValueLabel = Values[i].GetPressure
                });
            }

            tempChart.Entries = tempEntries;
            humiChart.Entries = humiEntries;
            presChart.Entries = presEntries;
        });

        public DashboardViewModel()
        {
            Title = "Dashboard";
            tempChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
            humiChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
            presChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
        }
    }
}