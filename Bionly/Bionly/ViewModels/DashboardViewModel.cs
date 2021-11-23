using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Bionly.Models;

namespace Bionly.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public LineChart lineChart;

        public static List<MeasurementPoint> Values = new();

        public ICommand DrawGraphs => new Command(async () =>
        {
            List<ChartEntry> entries = new();
            for (int i = 0; i < Values.Count; i++)
            {
                entries.Add(new ChartEntry(Values[i].Temperature)
                {
                    Label = Values[i].Time.ToShortDateString(),
                    ValueLabel = Values[i].GetTemperature
                });
            }

            lineChart.Entries = entries;
        });

        public DashboardViewModel()
        {
            Title = "Dashboard";
            lineChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
        }
    }
}