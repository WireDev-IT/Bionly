using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bionly.Models
{
    public class MeasurementPoint
    {
        public DateTime Time { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float Pressure { get; set; }

        /// <summary>
        /// Gets a string that contains date and time separated by a comma.
        /// </summary>
        [JsonIgnore]
        public string TimeStr
        {
            get => Time.ToShortDateString() + ", " + Time.ToShortTimeString();
        }

        /// <summary>
        /// Gets a string that contains the temperature with one decimal place and the unit.
        /// </summary>
        [JsonIgnore]
        public string TemperatureStr
        {
            get => Temperature.ToString("N1") + " °C";
        }

        /// <summary>
        /// Gets a string that contains the humidity in a length of two characters and the unit.
        /// </summary>
        [JsonIgnore]
        public string HumidityStr
        {
            get => Humidity.ToString("00") + " %";
        }

        /// <summary>
        /// Gets a string that contains the air pressure and the unit.
        /// </summary>
        [JsonIgnore]
        public string PressureStr
        {
            get => Pressure.ToString() + " hPa";
        }

        /// <summary>
        /// true, if the file already exists.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
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

    public class AverageMeasurementPoint
    {
        /// <summary>
        /// The measurement point that contains the average values.
        /// </summary>
        public MeasurementPoint Point { get; set; } = new();

        /// <summary>
        /// The number of devices that have been calculated in this measuring point.
        /// </summary>
        public uint ValuesNumber { get; set; } = 1;

        /// <summary>
        /// IDs of devices that are excluded from the calculation. 
        /// </summary>
        public List<string> ExcudeIds { get; set; } = new();

        public override string ToString()
        {
            return $"{Point.TemperatureStr}; {Point.HumidityStr}; {Point.PressureStr}; ({ValuesNumber})";
        }
    }
}