using Newtonsoft.Json;
using System;
using static Bionly.Enums.Sensor;

namespace Bionly.Models
{
    public class JsonMeasurementPoint
    {
        [JsonProperty("timestamp")]
        public string TimeString { get; set; }

        [JsonProperty("sensor")]
        public string Sensor { get; set; }

        [JsonProperty("value")]
        public float Value { get; set; }

        public SensorType GetSensorType()
        {
            return Sensor switch
            {
                "dht22-temperature" => SensorType.Temperature,
                "dht22-humidity" => SensorType.Humidity,
                "bmp180-temperature" => SensorType.Temperature,
                "bmp180-pressure" => SensorType.Pressure,
                _ => SensorType.Unknown,
            };
        }

        [JsonIgnore]
        public DateTime Time
        {
            get
            {
                char[] delimiterChars = { '-', '_' };

                string[] strings = TimeString.Split(delimiterChars);
                int[] numbers = new int[strings.Length];

                for (int i = 0; i < strings.Length; i++)
                {
                    numbers[i] = int.Parse(strings[i]);
                }

                return new DateTime(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5]);
            }
        }
    }
}
