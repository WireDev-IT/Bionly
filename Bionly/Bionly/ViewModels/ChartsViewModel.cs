using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bionly.ViewModels
{
    public class ChartsViewModel : BaseViewModel
    {
        public LineChart tempChart;
        public LineChart humiChart;
        public LineChart presChart;

        public ChartsViewModel()
        {
            Title = "Diagramme";
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
        }

        public ICommand DrawGraphs => new Command(() =>
        {
            List<ChartEntry> tempEntries = new();
            List<ChartEntry> humiEntries = new();
            List<ChartEntry> presEntries = new();

            RuntimeData.SelectedDevice.MPoints = RuntimeData.SelectedDevice.MPoints.OrderBy(x => x.Time).ToList();

            for (int i = 0; i < RuntimeData.SelectedDevice.MPoints.Count; i++)
            {
                tempEntries.Add(new ChartEntry(RuntimeData.SelectedDevice.MPoints[i].Temperature)
                {
                    Label = RuntimeData.SelectedDevice.MPoints[i].Time.ToShortDateString(),
                    ValueLabel = RuntimeData.SelectedDevice.MPoints[i].TemperatureStr,
                    Color = new SKColor(255, 0, 0)
                });
                humiEntries.Add(new ChartEntry(RuntimeData.SelectedDevice.MPoints[i].Humidity)
                {
                    Label = RuntimeData.SelectedDevice.MPoints[i].Time.ToShortDateString(),
                    ValueLabel = RuntimeData.SelectedDevice.MPoints[i].HumidityStr,
                    Color = new SKColor(0, 0, 255)
                });
                presEntries.Add(new ChartEntry(RuntimeData.SelectedDevice.MPoints[i].Pressure)
                {
                    Label = RuntimeData.SelectedDevice.MPoints[i].Time.ToShortDateString(),
                    ValueLabel = RuntimeData.SelectedDevice.MPoints[i].PressureStr
                });
            }

            tempChart.Entries = tempEntries;
            humiChart.Entries = humiEntries;
            presChart.Entries = presEntries;
        });
    }
}
