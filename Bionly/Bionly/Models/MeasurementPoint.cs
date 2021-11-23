using System;

namespace Bionly.Models
{
    public class MeasurementPoint
    {
        public DateTime Time { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float Pressure { get; set; }

        public string GetTime
        {
            get => Time.ToShortDateString() + ", " + Time.ToShortTimeString();
        }

        public string GetTemperature
        {
            get => Temperature.ToString("N1") + " °C";
        }
        public string GetHumidity
        {
            get => Humidity.ToString("00") + " %";
        }
        public string GetPressure
        {
            get => Pressure.ToString() + " hPa";
        }
    }
}
