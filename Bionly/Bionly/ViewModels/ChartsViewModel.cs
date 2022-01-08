using Bionly.Models;
using Bionly.Resx;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Device = Bionly.Models.Device;

namespace Bionly.ViewModels
{
    public class ChartsViewModel : BaseViewModel
    {
        public static string AveragePath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "/Files/";

        private readonly Dictionary<DateTime, AverageMeasurementPoint> averagePoints = new();

        private LineChart _tempChart;
        public LineChart TempChart
        {
            get => _tempChart;
            set
            {
                if (_tempChart != value)
                {
                    _tempChart = value;
                    OnPropertyChanged();
                }
            }
        }

        private LineChart _humiChart;
        public LineChart HumiChart
        {
            get => _humiChart;
            set
            {
                if (_humiChart != value)
                {
                    _humiChart = value;
                    OnPropertyChanged();
                }
            }
        }

        public LineChart _presChart;
        public LineChart PresChart
        {
            get => _presChart;
            set
            {
                if (_presChart != value)
                {
                    _presChart = value;
                    OnPropertyChanged();
                }
            }
        }

        public ChartsViewModel()
        {
            Title = Strings.Charts;
            TempChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                //BackgroundColor = new SKColor().WithAlpha(0),
            };
            HumiChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                //BackgroundColor = new SKColor().WithAlpha(0)
            };
            PresChart = new()
            {
                LineMode = LineMode.Straight,
                LabelOrientation = Orientation.Default,
                ValueLabelOrientation = Orientation.Default,
                AnimationDuration = TimeSpan.FromMilliseconds(500)
            };
        }

        public ICommand DrawGraphs => new Command<List<MeasurementPoint>>((List<MeasurementPoint> points) =>
        {
            List<ChartEntry> tempEntries = new();
            List<ChartEntry> humiEntries = new();
            List<ChartEntry> presEntries = new();

            if (points == null)
            {
                points = new();
                foreach (KeyValuePair<DateTime, AverageMeasurementPoint> entry in averagePoints)
                {
                    points.Add(entry.Value.Point);
                }
            }

            points = points.OrderBy(x => x.Time).ToList();

            for (int i = 0; i < points.Count; i++)
            {
                tempEntries.Add(new ChartEntry(points[i].Temperature)
                {
                    Label = points[i].Time.ToShortDateString(),
                    ValueLabel = points[i].TemperatureStr,
                    Color = new SKColor(255, 0, 0)
                });
                humiEntries.Add(new ChartEntry(points[i].Humidity)
                {
                    Label = points[i].Time.ToShortDateString(),
                    ValueLabel = points[i].HumidityStr,
                    Color = new SKColor(0, 0, 255)
                });
                presEntries.Add(new ChartEntry(points[i].Pressure)
                {
                    Label = points[i].Time.ToShortDateString(),
                    ValueLabel = points[i].PressureStr
                });
            }

            TempChart.Entries = tempEntries;
            HumiChart.Entries = humiEntries;
            PresChart.Entries = presEntries;
        });

        public ICommand CalcAverage => new Command(() =>
        {
            foreach (Device d in RuntimeData.Devices)
            {
                foreach (MeasurementPoint m in d.MPoints)
                {
                    m.Time = m.Time.AddMinutes(-m.Time.Minute).AddSeconds(-m.Time.Second);

                    if (averagePoints.TryGetValue(m.Time, out AverageMeasurementPoint a))
                    {
                        float t = a.Point.Temperature * a.ValuesNumber;
                        float h = a.Point.Humidity * a.ValuesNumber;
                        float p = a.Point.Pressure * a.ValuesNumber;

                        a.ValuesNumber++;

                        t += m.Temperature;
                        t /= a.ValuesNumber;

                        h += m.Humidity;
                        h /= a.ValuesNumber;

                        p += m.Pressure;
                        p /= a.ValuesNumber;

                        averagePoints[m.Time].Point.Temperature = t;
                        averagePoints[m.Time].Point.Humidity = h;
                        averagePoints[m.Time].Point.Pressure = p;
                        averagePoints[m.Time].ValuesNumber = a.ValuesNumber;
                    }
                    else
                    {
                        averagePoints.Add(m.Time, new AverageMeasurementPoint() { Point = m });
                    }
                }
            }
        });
    }
}
