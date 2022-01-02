using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace Bionly.Resx
{
    internal class LocalizationHelper
    {
        private static LanguageInfo _currentLanguage;

        public static LanguageInfo CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    Thread.CurrentThread.CurrentUICulture = value.CultureInfo;
                    OnStaticPropertyChanged();

                    if (Shell.Current != null)
                    {
                        if (Shell.Current.DisplayAlert(Strings.RestartRequired, Strings.CloseAppForLanguage, Strings.CloseNow, Strings.OK).Result)
                        {
                            Application.Current.Quit();
                        }
                    }
                }
            }
        }

        public static void Initialize()
        {
            SupportedLanguages = GetAllSupportedLanguages();
            string language = null ?? "en";
            try
            {
                CurrentLanguage = SupportedLanguages.FirstOrDefault(x => x.LanguageName == language);
            }
            catch (Exception) { }

            Thread.CurrentThread.CurrentUICulture = CurrentLanguage.CultureInfo;
        }

        public static ObservableCollection<LanguageInfo> SupportedLanguages { get; private set; }

        private static ObservableCollection<LanguageInfo> GetAllSupportedLanguages()
        {
            ObservableCollection<LanguageInfo> supportedLanguages = new();

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
                        supportedLanguages.Add(new LanguageInfo(culture));
                    }
                }
                catch (CultureNotFoundException) { }
            }

            return supportedLanguages;
        }

        public class LanguageInfo
        {
            public LanguageInfo(CultureInfo cultureInfo)
            {
                CultureInfo = cultureInfo;
            }

            public CultureInfo CultureInfo { get; private set; }

            public string DisplayName => CultureInfo.NativeName;

            public string LanguageName => CultureInfo.Name;

            public override string ToString()
            {
                return DisplayName;
            }
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
