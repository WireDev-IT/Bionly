using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace Bionly.Resx
{
    internal class LocalizationHelper
    {
        public class LanguageSettings : INotifyPropertyChanged
        {
            [JsonIgnore]
            public static string Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "/settings.json";

            private string _twoLetterISOLanguageName = null;
            [JsonProperty("language")]
            public string TwoLetterISOLanguageName
            {
                get => _twoLetterISOLanguageName;
                set
                {
                    if (_twoLetterISOLanguageName != value)
                    {
                        _twoLetterISOLanguageName = value;
                        OnPropertyChanged(nameof(TwoLetterISOLanguageName));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }

            public Task<bool> Save()
            {
                try
                {
                    File.WriteAllText(Path, JsonConvert.SerializeObject(Settings, Formatting.Indented));
                }
                catch (Exception)
                {
                    return Task.FromResult(false);
                }
                return Task.FromResult(true);
            }
        }

        private static LanguageSettings _settings = new();
        public static LanguageSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnStaticPropertyChanged(nameof(Settings));
                }
            }
        }

        private static CultureInfo _currentLanguage;
        public static CultureInfo CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    Thread.CurrentThread.CurrentUICulture = value;
                    OnStaticPropertyChanged(nameof(CurrentLanguage));
                }
            }
        }

        public static string[] SupportedLanguagesStr
        {
            get
            {
                string[] result = new string[] { };
                foreach (CultureInfo culture in SupportedLanguages)
                {
                    Array.Resize(ref result, result.Length + 1);
                    result[result.Length - 1] = culture.DisplayName;
                }
                return result;
            }
        }

        public static void Initialize()
        {
            SupportedLanguages = GetAllSupportedLanguages();
            try
            {
                LanguageSettings settings = JsonConvert.DeserializeObject<LanguageSettings>(File.ReadAllText(LanguageSettings.Path));
                CurrentLanguage = SupportedLanguages.First(x => x.ToString() == settings.TwoLetterISOLanguageName);
            }
            catch (Exception)
            {
                CurrentLanguage = CultureInfo.GetCultureInfo("en");
            }

            Thread.CurrentThread.CurrentUICulture = CurrentLanguage;
        }

        public static ObservableCollection<CultureInfo> SupportedLanguages { get; private set; }

        private static ObservableCollection<CultureInfo> GetAllSupportedLanguages()
        {
            ObservableCollection<CultureInfo> supportedLanguages = new();

            ResourceManager resourceManager = Strings.ResourceManager;
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture))
                    {
                        continue;
                    }

                    ResourceSet resourceSet = resourceManager.GetResourceSet(culture, true, false);
                    if (resourceSet != null)
                    {
                        supportedLanguages.Add(culture);
                    }
                }
                catch (CultureNotFoundException) { }
            }

            return supportedLanguages;
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void OnStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));
        }
    }
}
