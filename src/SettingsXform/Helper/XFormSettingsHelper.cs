﻿using Newtonsoft.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AndreasReitberger.XForm.Helper
{
    internal class XFormSettingsHelper
    {
        #region Methods
        // Docs: https://docs.microsoft.com/en-us/dotnet/maui/platform-integration/storage/preferences
        /*
         * Avalaible DataTypes:
         * - Boolean
         * - Double
         * - Int32
         * - Single
         * - Int64
         * - String
         * - DateTime
        */
        public static T? GetSettingsValue<T>(string key, T defaultValue)
        {
            object? returnValue = null;
            switch (defaultValue)
            {
                case bool b:
                    returnValue = Preferences.Get(key, b);
                    break;
                case double d:
                    returnValue = Preferences.Get(key, d);
                    break;
                case int i:
                    returnValue = Preferences.Get(key, i);
                    break;
                case float f:
                    returnValue = Preferences.Get(key, f);
                    break;
                case long l:
                    returnValue = Preferences.Get(key, l);
                    break;
                case string s:
                    returnValue = Preferences.Get(key, s);
                    break;
                case DateTime dt:
                    returnValue = Preferences.Get(key, dt);
                    break;
                default:
                    // For all other types try to serialize it as JSON
                    string jsonString = Preferences.Get(key, string.Empty) ?? string.Empty;
                    if (defaultValue == null)
                    {
                        // In this case it's unkown to what data type the string should be deserialized.
                        // So just return the string as it is to avoid exceptions while converting.
                        returnValue = jsonString;
                    }
                    else
                    {
                        returnValue = JsonConvert.DeserializeObject<T>(jsonString);
                    }
                    break;
            }
            return ChangeSettingsType<T>(returnValue, defaultValue);
        }

        public static T? ChangeSettingsType<T>(object settingsValue, T defaultValue) => (T)Convert.ChangeType(settingsValue, typeof(T)) ?? defaultValue;

        // Docs: https://docs.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage?tabs=ios
        // Only string is allowed for secure storage
        public static async Task<string> GetSecureSettingsValueAsync(string key, string? defaultValue)
        {
            defaultValue ??= string.Empty;
            string settingsObject = await SecureStorage.GetAsync(key);
            return settingsObject == null ? defaultValue : settingsObject;
        }

        public static void SetSettingsValue(string key, object? value)
        {
            switch (value)
            {
                case bool b:
                    Preferences.Set(key, b);
                    break;
                case double d:
                    Preferences.Set(key, d);
                    break;
                case int i:
                    Preferences.Set(key, i);
                    break;
                case float f:
                    Preferences.Set(key, f);
                    break;
                case long l:
                    Preferences.Set(key, l);
                    break;
                case string s:
                    Preferences.Set(key, s);
                    break;
                case DateTime dt:
                    Preferences.Set(key, dt);
                    break;
                default:
                    // For all other types try to serialize it as JSON
                    string? jsonString = JsonConvert.SerializeObject(value, Formatting.Indented);
                    Preferences.Set(key, jsonString);
                    break;
            }
        }
        public static async Task SetSecureSettingsValueAsync(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // It's not allowed to set a secure value to an empty string, so remove it.
                SecureStorage.Remove(key);
            }
            else await SecureStorage.SetAsync(key, value);
        }

        public static void ClearSettings() => Preferences.Clear();

        public static void ClearSecureSettings() => SecureStorage.RemoveAll();

        #endregion
    }
}
