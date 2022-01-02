using Bionly.Resx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Bionly.Enums.User;

namespace Bionly.Models
{
    public class Account : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public Account(string username, string password)
        {
            Name = username;
            Password = password;
        }

        private string _name;
        public string Name
        {
            get => _name;
            private set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            private set
            {
                if (_password != value)
                {
                    _password = GetHashString(value);
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new();
            foreach (byte b in SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString)))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }

    public class Accounts
    {
        public static string GeneralPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);

        public Dictionary<string, string> Users { get; private set; } = new();

        public bool Add(Account account)
        {
            if (account.Name != "guest" && !Users.ContainsKey(account.Name))
            {
                Users.Add(account.Name, account.Password);
                return true;
            }
            return false;
        }

        public bool Remove(Account account)
        {
            return Users.Remove(account.Name);
        }

        public UserType GetUserType(Account account)
        {
            if (account.Name == "guest" && account.Password == Account.GetHashString("guest"))
            {
                return UserType.Guest;
            }
            else
            {
                foreach (KeyValuePair<string, string> pair in Users)
                {
                    if (account.Name == pair.Key && account.Password == pair.Value)
                    {
                        return UserType.Admin;
                    }
                }

                return UserType.None;
            }
        }

        public async Task<bool> Save()
        {
            try
            {
                _ = Directory.CreateDirectory(GeneralPath);
                File.WriteAllText(GeneralPath + $"/accounts.json", JsonConvert.SerializeObject(this, Formatting.Indented));
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.ErrorOnUserSave, ex.Message, Strings.OK);
            }
            return false;
        }
        public static async Task<Accounts> Load()
        {
            try
            {
                _ = Directory.CreateDirectory(GeneralPath);
                return JsonConvert.DeserializeObject<Accounts>(File.ReadAllText(GeneralPath + "/accounts.json"));
            }
            catch (FileNotFoundException)
            {
                Accounts a = new();
                a.Add(new Account("admin", "admin"));
                await a.Save();
                return a;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(Strings.ErrorOnUserLoad, ex.Message, Strings.OK);
            }
            return new();
        }
    }
}
