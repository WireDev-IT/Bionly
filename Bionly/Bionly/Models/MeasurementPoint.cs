using Newtonsoft.Json;
using System;
using System.IO;

namespace Bionly.Models
{
    public class MeasurementPoint
    {
        public DateTime Time { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float Pressure { get; set; }

        [JsonIgnore]
        public string TimeStr
        {
            get => Time.ToShortDateString() + ", " + Time.ToShortTimeString();
        }

        [JsonIgnore]
        public string TemperatureStr
        {
            get => Temperature.ToString("N1") + " °C";
        }

        [JsonIgnore]
        public string HumidityStr
        {
            get => Humidity.ToString("00") + " %";
        }

        [JsonIgnore]
        public string PressureStr
        {
            get => Pressure.ToString() + " hPa";
        }

        [JsonIgnore]
        public bool Exists
        {
            get => File.Exists(Path);
        }

        [JsonIgnore]
        private string _path = null;
        [JsonIgnore]
        public string Path
        {
            get => _path;
            set => _path = value + $"\\{Time.Ticks}.json";
        }

        public void Save(string path = null)
        {
            if (!string.IsNullOrEmpty(path)) Path = path;

            File.WriteAllText(Path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static MeasurementPoint Load(string path)
        {
            return JsonConvert.DeserializeObject<MeasurementPoint>(File.ReadAllText(path));
        }

    }
}
