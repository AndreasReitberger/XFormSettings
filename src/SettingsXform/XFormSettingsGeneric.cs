using AndreasReitberger.Shared.Core.Utilities;
using AndreasReitberger.XForm.Attributes;
#if IOS
using AndreasReitberger.XForm.Cloud;
#endif
using AndreasReitberger.XForm.Enums;
using AndreasReitberger.XForm.Events;
using AndreasReitberger.XForm.Helper;
using AndreasReitberger.XForm.Utilities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AndreasReitberger.XForm
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */

    public partial class XFormSettingsGeneric<SO> where SO : new()
    {
        #region Settings Object

        static object? _settingsObject;
        public static SO SettingsObject
        {
            get
            {
                _settingsObject ??= new SO();
                return (SO)_settingsObject;
            }
        }
        #endregion

        #region Variables
        static readonly object lockObject = new();
        #endregion

        #region Constructor
        public XFormSettingsGeneric() { }
        public XFormSettingsGeneric(SO settingsObject)
        {
            _settingsObject = settingsObject;
        }
        #endregion

        #region Methods

        #region LoadSettings
        public static void LoadSettings() => LoadSettings(settings: SettingsObject);

        public static void LoadSetting<T>(Expression<Func<SO, T>> value) => LoadObjectSetting(SettingsObject, value);

        public static Task LoadSettingAsync<T>(Expression<Func<SO, T>> value, string? key = null) => Task.Run(async delegate
        {
            await LoadObjectSettingAsync(SettingsObject, value, key: key);
        });
        public static Task LoadSecureSettingAsync<T>(Expression<Func<SO, T>> value, string? key = null) => Task.Run(async delegate
        {
            await LoadSecureObjectSettingAsync(SettingsObject, value, key: key);
        });

        public void LoadObjectSettings() => LoadSettings(this);

        public static void LoadObjectSetting<T>(object settingsObject, Expression<Func<SO, T>> value)
            => GetExpressionMeta(settings: settingsObject, value, XFormSettingsActions.Load);

        public static Task LoadObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, string? key = null) => Task.Run(async delegate
        {
            await GetExpressionMetaAsync(settings: settingsObject, value, XFormSettingsActions.Load, key: key);
        });

        public static Task LoadSecureObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value, string? key = null) => Task.Run(async delegate
        {
            await GetExpressionMetaAsync(settings: settingsObject, value, XFormSettingsActions.Load, secureOnly: true, key: key);
        });

        public static void LoadSettings(object settings) => GetClassMeta(settings: settings, mode: XFormSettingsActions.Load);

        public static Task LoadSettingsAsync(string? key = null) => Task.Run(async delegate
        {
            await LoadSettingsAsync(settings: SettingsObject, key: key);
        });

        public static Task LoadSettingsAsync(object settings, string? key = null) => Task.Run(async delegate
        {
            await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Load, key: key);
        });

        public static Task<bool> TryLoadSettingsAsync(string? key = null)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, key: key);
            });

        public static Task<bool> TryLoadSettingsAsync(object settings, string? key = null)
            => Task.Run(async delegate
            {
                return await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Load, key: key, justTryLoading: true);
            });

        public static Task LoadSecureSettingsAsync(string? key = null) => Task.Run(async delegate
        {
            await LoadSecureSettingsAsync(settings: SettingsObject, key: key);
        });

        public static Task LoadSecureSettingsAsync(object settings, string? key = null) => Task.Run(async delegate
        {
            await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Load, secureOnly: true, key: key);
        });

        public static Task LoadSettingsAsync(Dictionary<string, Tuple<object, Type>> dictionary, bool save = true, string? key = null) => Task.Run(async delegate
        {
            await LoadSettingsAsync(settings: SettingsObject, dictionary: dictionary, save: save, key: key);
        });

        public static Task<bool> TryLoadSettingsAsync(Dictionary<string, Tuple<object, Type>> dictionary, string? key = null)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, dictionary: dictionary, key: key);
            });

        public static Task LoadSettingsAsync(string settingsKey, Tuple<object, Type> data, bool save = true, string? key = null) => Task.Run(async delegate
        {
            await LoadSettingsAsync(settings: SettingsObject, dictionary: new() { { settingsKey, data } }, save: save, key: key);
        });

        public static Task<bool> TryLoadSettingsAsync(string settingsKey, Tuple<object, Type> data, string? key = null)
            => Task.Run(async delegate
            {
                return await TryLoadSettingsAsync(settings: SettingsObject, dictionary: new() { { settingsKey, data } }, key: key);
            });

        public static Task LoadSettingsAsync(object settings, Dictionary<string, Tuple<object, Type>> dictionary, bool save = true, string? key = null) => Task.Run(async delegate
        {
            await GetMetaFromDictionaryAsync(settings: settings, dictionary: dictionary, mode: XFormSettingsActions.Load, secureOnly: false, key: key);
            // Save the restored settings right away
            if (save) await SaveSettingsAsync(settings: settings, key: key);
        });

        public static Task<bool> TryLoadSettingsAsync(object settings, Dictionary<string, Tuple<object, Type>> dictionary, string? key = null)
            => Task.Run(async delegate
            {
                return await GetMetaFromDictionaryAsync(settings: settings, dictionary: dictionary, mode: XFormSettingsActions.Load, secureOnly: false, key: key, justTryLoading: true);
            });

        #endregion

        #region SaveSettings
        public static void SaveSettings() => SaveSettings(SettingsObject);

        public static void SaveSetting<T>(Expression<Func<SO, T>> value) => SaveObjectSetting(SettingsObject, value);

        public static void SaveObjectSetting<T>(object settings, Expression<Func<SO, T>> value) => GetExpressionMeta(settings, value, XFormSettingsActions.Save);

        public void SaveObjectSetting<T>(Expression<Func<SO, T>> value) => SaveObjectSetting(this, value);

        public void SaveObjectSettings() => SaveSettings(this);

        public static void SaveSettings(object settings) => GetClassMeta(settings, XFormSettingsActions.Save);

        public static Task SaveSettingsAsync(string? key = null) => Task.Run(async delegate
        {
            await SaveSettingsAsync(settings: SettingsObject, key: key);
        });

        public static Task SaveSettingsAsync(object settings, string? key = null) => Task.Run(async delegate
        {
            await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Save, key: key);
        });

        public static Task SaveSecureSettingsAsync(string? key = null) => Task.Run(async delegate
        {
            await SaveSecureSettingsAsync(settings: SettingsObject, key: key);
        });

        public static Task SaveSecureSettingsAsync(object settings, string? key = null) => Task.Run(async delegate
        {
            await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Save, secureOnly: true, key: key);
        });

        #endregion

        #region DeleteSettings
        public static void DeleteSettings() => DeleteSettings(SettingsObject);

        public static void DeleteSetting<T>(Expression<Func<SO, T>> value) => DeleteObjectSetting(SettingsObject, value);

        public void DeleteObjectSetting<T>(Expression<Func<SO, T>> value) => DeleteObjectSetting(this, value);

        public static void DeleteObjectSetting<T>(object settings, Expression<Func<SO, T>> value) => GetExpressionMeta(settings, value, XFormSettingsActions.Delete);

        public void DeleteObjectSettings() => DeleteSettings(this);

        public static void DeleteSettings(object settings) => GetClassMeta(settings, XFormSettingsActions.Delete);

        #endregion

        #region LoadDefaults
        public static void LoadDefaultSettings() => LoadDefaultSettings(SettingsObject);

        public static void LoadDefaultSetting<T>(Expression<Func<SO, T>> value) => LoadObjectDefaultSetting(SettingsObject, value);

        public void LoadObjectDefaultSetting<T>(Expression<Func<SO, T>> value) => LoadObjectDefaultSetting(this, value);

        public static void LoadObjectDefaultSetting<T>(object settings, Expression<Func<SO, T>> value)
            => GetExpressionMeta(settings, value, XFormSettingsActions.LoadDefaults);

        public void LoadObjectDefaultSettings() => LoadDefaultSettings(this);

        public static void LoadDefaultSettings(object settings) => GetClassMeta(settings, XFormSettingsActions.LoadDefaults);

        #endregion

        #region Conversion

        public static Task<Dictionary<string, Tuple<object, Type>>> ToDictionaryAsync()
           => ToDictionaryAsync(settings: SettingsObject);

        public static Task<Dictionary<string, Tuple<object, Type>>> ToDictionaryAsync(bool secureOnly = false, string? key = null)
            => ToDictionaryAsync(settings: SettingsObject, secureOnly: secureOnly, key: key);

        public static async Task<Dictionary<string, Tuple<object, Type>>> ToDictionaryAsync(object? settings, bool secureOnly = false, string? key = null)
        {
            if (true)
            {
                Dictionary<string, Tuple<object, Type>> setting = [];
                IEnumerable<MemberInfo>? declaredMembers = settings?.GetType().GetTypeInfo().DeclaredMembers;

                XFormSettingsMemberInfo settingsObjectInfo = new();
                XFormSettingsInfo settingsInfo = new();
                if (declaredMembers is null) return setting;

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    XFormSettingsInfo settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(settingsObjectInfo, settingsInfo, secureOnly: secureOnly, key: key, keeyEncrypted: true);
                    if (settingsPair != null && !settingsPair.SkipForExport)
                    {
                        setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                    }
                }
                return setting;
            }
        }

        public static Task<ConcurrentDictionary<string, Tuple<object, Type>>> ToConcurrentDictionaryAsync()
            => ToConcurrentDictionaryAsync(settings: SettingsObject);
        public static Task<ConcurrentDictionary<string, Tuple<object, Type>>> ToConcurrentDictionaryAsync(bool secureOnly = false, string? key = null)
            => ToConcurrentDictionaryAsync(settings: SettingsObject, secureOnly: secureOnly, key: key);
        public static async Task<ConcurrentDictionary<string, Tuple<object, Type>>> ToConcurrentDictionaryAsync(object? settings, bool secureOnly = false, string? key = null)
        {
            ConcurrentDictionary<string, Tuple<object, Type>> setting = new();
            List<MemberInfo>? members = GetClassMetaAsList(settings);

            XFormSettingsMemberInfo settingsObjectInfo = new();
            XFormSettingsInfo settingsInfo = new();
            if (members is null) return setting;

            foreach (MemberInfo mInfo in members)
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                XFormSettingsInfo settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(settingsObjectInfo, settingsInfo, secureOnly: secureOnly, key: key, keeyEncrypted: true);
                if (settingsPair != null && !settingsPair.SkipForExport)
                {
                    setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                }
            }
            return setting;
        }

        public static Task<Tuple<string, Tuple<object, Type>>> ToSettingsTupleAsync<T>(Expression<Func<SO, T>> value, string? key = null)
            => ToSettingsTupleAsync(settings: SettingsObject, value: value, key: key);


        public static async Task<Tuple<string, Tuple<object, Type>>> ToSettingsTupleAsync<T>(object? settings, Expression<Func<SO, T>> value, string? key = null)
        {
            XFormSettingsInfo? info = await GetExpressionMetaAsKeyValuePairAsync(settings: settings, value: value, key: key);
            return new(info.Name, new(info.Value, info.SettingsType));
        }
        #endregion

        #region Encryption

        public static Task ExhangeKeyAsync(string newKey, string? oldKey = null, bool reloadSettings = false)
            => Task.Run(async delegate
            {
                if (reloadSettings) await LoadSecureSettingsAsync(key: oldKey);
                await SaveSettingsAsync(key: newKey);
            });

        #endregion

        #region Private
        static List<MemberInfo>? GetClassMetaAsList(object? settings)
        {
            lock (lockObject)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo>? declaredMembers = settings?.GetType().GetTypeInfo().DeclaredMembers;

                XFormSettingsMemberInfo settingsObjectInfo = new();
                XFormSettingsInfo settingsInfo = new();
                return declaredMembers?.ToList();
            }
        }
        static void GetClassMeta(object settings, XFormSettingsActions mode, XFormSettingsTarget target = XFormSettingsTarget.Local)
        {
            lock (lockObject)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

                XFormSettingsMemberInfo settingsObjectInfo = new();
                XFormSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    _ = ProcessSettingsInfo(settingsObjectInfo, settingsInfo, mode, target);
                }
            }
        }
        static async Task<bool> GetClassMetaAsync(object settings, XFormSettingsActions mode, XFormSettingsTarget target = XFormSettingsTarget.Local, bool secureOnly = false, string? key = null, bool justTryLoading = false)
        {
            // Get all member infos from the passed settingsObject
            IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

            XFormSettingsMemberInfo settingsObjectInfo = new();
            XFormSettingsInfo settingsInfo = new();

            foreach (MemberInfo mInfo in declaredMembers)
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                XFormSettingsResults result = await ProcessSettingsInfoAsync(settingsObjectInfo, settingsInfo, mode, target, secureOnly: secureOnly, key: key, justTryLoading: justTryLoading);
                if (result == XFormSettingsResults.EncryptionError || result == XFormSettingsResults.Failed) { return false; }
            }
            return true;
        }
        static async Task<bool> GetMetaFromDictionaryAsync(object settings, Dictionary<string, Tuple<object, Type>> dictionary, XFormSettingsActions mode, XFormSettingsTarget target = XFormSettingsTarget.Local, bool secureOnly = false, string? key = null, bool justTryLoading = false)
        {
            // Get all member infos from the passed settingsObject
            IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

            XFormSettingsMemberInfo settingsObjectInfo = new();
            XFormSettingsInfo settingsInfo = new();

            foreach (MemberInfo mInfo in declaredMembers)
            {
                bool useValueFromSettingsInfo = false;
                // Try to find the matching settingsKey
                KeyValuePair<string, Tuple<object, Type>>? keyPair = dictionary?.FirstOrDefault(keypair =>
                    keypair.Key.EndsWith(mInfo.Name
                    //?.Replace("get_", string.Empty)
                    ));
                if (keyPair?.Key != null)
                {
                    useValueFromSettingsInfo = true;
                    // If a matching settingsKey was found, prepare the settingsInfo with the loaded data
                    settingsInfo = new()
                    {
                        Name = mInfo.Name?.Replace("get_", string.Empty),
                        Value = keyPair.Value.Value.Item1,
                        SettingsType = keyPair.Value.Value.Item2,
                    };
                }
                else
                    useValueFromSettingsInfo = false;
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                XFormSettingsResults result = await ProcessSettingsInfoAsync(
                    settingsObjectInfo, settingsInfo, mode, target, secureOnly: secureOnly, useValueFromSettingsInfo: useValueFromSettingsInfo, key: key, justTryLoading: justTryLoading);
                if (result == XFormSettingsResults.EncryptionError || result == XFormSettingsResults.Failed) { return false; }
            }
            return true;
        }

        static void GetExpressionMeta<T>(object settings, Expression<Func<SO, T>> value, XFormSettingsActions mode, XFormSettingsTarget target = XFormSettingsTarget.Local)
        {
            lock (lockObject)
            {
                if (value.Body is MemberExpression memberExpression)
                {
                    _ = ProcessSettingsInfo(new XFormSettingsMemberInfo()
                    {
                        OrignalSettingsObject = settings,
                        Info = memberExpression.Member,

                    }, new XFormSettingsInfo(), mode, target);
                }
            }
        }

        static async Task GetExpressionMetaAsync<T>(object settings, Expression<Func<SO, T>> value, XFormSettingsActions mode, XFormSettingsTarget target = XFormSettingsTarget.Local, bool secureOnly = false, string? key = null)
        {

            if (value.Body is MemberExpression memberExpression)
            {
                _ = await ProcessSettingsInfoAsync(new XFormSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new XFormSettingsInfo(), mode, target, secureOnly: secureOnly, key: key);
            }
        }

        static async Task<XFormSettingsInfo?> GetExpressionMetaAsKeyValuePairAsync<T>(object settings, Expression<Func<SO, T>> value, string? key = null)
        {
            if (value.Body is MemberExpression memberExpression)
            {
                return await ProcessSettingsInfoAsKeyValuePairAsync(new XFormSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new XFormSettingsInfo(), key: key, keeyEncrypted: true);
            }
            return new();
        }

        static bool ProcessSettingsInfo(
            XFormSettingsMemberInfo settingsObjectInfo, XFormSettingsInfo settingsInfo, XFormSettingsActions mode, XFormSettingsTarget target,
            bool throwOnError = false, bool justTryLoading = false
            )
        {
            settingsInfo ??= new();
            XFormSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<XFormSettingAttribute> settingBaseAttributes
                    = settingsObjectInfo.Info.GetCustomAttributes<XFormSettingAttribute>(inherit: false)
                    .ToList();
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return false;
                }
                settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
            }
            if (settingsObjectInfo.Info is not null)
            {
                settingsInfo.Name = XFormSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = XFormSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));

                settingsInfo.Default = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);

                //Type type = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                //settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                switch (target)
                {
#if IOS
                    case MauiSettingsTarget.ICloud:
                        settingsInfo.Value = ICloudStoreManager.GetValue(settingsInfo.Name) ?? settingsInfo.Default;
                        break;
#endif
                    case XFormSettingsTarget.Local:
                    default:
                        settingsInfo.Value = XFormSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                        break;

                }
                //settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
            }
            bool? secure = false;
            if (settingBaseAttribute is XFormSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                if (secure ?? false)
                {
#if IOS
                    switch (target)
                    {
                        case MauiSettingsTarget.ICloud:
                            if (throwOnError) throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                            else break;
                        case MauiSettingsTarget.Local:
                        default:
                            if (throwOnError) throw new NotSupportedException("SecureStorage is only available in the Async methods!");
                            else break;

                    }
#else
                    if (throwOnError) throw new NotSupportedException("SecureStorage is only available in the Async methods!");
#endif
                }
            }

            switch (mode)
            {
                case XFormSettingsActions.Load:
                    if (settingBaseAttribute?.DefaultValueInUse ?? false)
                    {
                        object defaultValue = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);

                    }
                    // Sets the loaded value back to the settingsObject
                    if (!justTryLoading)
                        XFormSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                    break;
                case XFormSettingsActions.Save:
                    // Get the value from the settingsObject
                    settingsInfo.Value = XFormSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case XFormSettingsTarget.Local:
                        default:
                            XFormSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            break;
                    }
                    break;
                case XFormSettingsActions.Delete:
                    object fallbackValue = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    settingsInfo.Value = fallbackValue;
                    settingsInfo.Default = fallbackValue;
                    if (settingsObjectInfo.Info is not null)
                    {
                        XFormSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
                    }
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            if (settingsInfo.Value != null)
                            {
                                // If there is a default value, do not delete the key. Instead write the default value
                                ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            }
                            else
                            {
                                // Otherwise delete the key from the cloud storage
                                ICloudStoreManager.DeleteValue(settingsInfo.Name);
                            }
                            break;
#endif
                        case XFormSettingsTarget.Local:
                        default:
                            XFormSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            break;
                    }
                    break;
                case XFormSettingsActions.LoadDefaults:
                    object defaulSettingtValue = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    if (settingsObjectInfo.Info is not null)
                    {
                        XFormSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, defaulSettingtValue, settingsInfo.SettingsType);
                    }
                    settingsInfo.Value = defaulSettingtValue;
                    settingsInfo.Default = defaulSettingtValue;
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case XFormSettingsTarget.Local:
                        default:
                            XFormSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            break;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        static async Task<XFormSettingsResults> ProcessSettingsInfoAsync(
            XFormSettingsMemberInfo settingsObjectInfo, XFormSettingsInfo settingsInfo, XFormSettingsActions mode, XFormSettingsTarget target,
            bool secureOnly = false, bool useValueFromSettingsInfo = false, string? key = null, bool keepEncrypted = false, bool justTryLoading = false
            )
        {
            settingsInfo ??= new();
            XFormSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<XFormSettingAttribute> settingBaseAttributes
                    = settingsObjectInfo.Info.GetCustomAttributes<XFormSettingAttribute>(inherit: false)
                    .ToList();
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return XFormSettingsResults.Skipped;
                }
                settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
            }
            if (settingsObjectInfo.Info is not null)
            {
                settingsInfo.Name = XFormSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = XFormSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                settingsInfo.Default = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
            }
            bool secure = false;
            if (settingBaseAttribute is XFormSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                // Save the states
                settingsInfo.IsSecure = secure;
                settingsInfo.Encrypt = settingAttribute.Encrypt;
                settingsInfo.SkipForExport = settingAttribute.SkipForExport;
                if (!secure)
                {
                    // If only secure storage should be loaded, stop here.
                    if (secureOnly)
                        return XFormSettingsResults.Skipped;
                    // If the value is not used from the passed settingsInfo, load it

                    switch (target)
                    {
#if IOS
                    case MauiSettingsTarget.ICloud:
                        settingsInfo.Value = ICloudStoreManager.GetValue(settingsInfo.Name) ?? settingsInfo.Default;
                        break;
#endif
                        case XFormSettingsTarget.Local:
                        default:
                            if (!useValueFromSettingsInfo)
                                settingsInfo.Value = XFormSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                            else
                                settingsInfo.Value = XFormSettingsHelper.ChangeSettingsType(settingsInfo.Value, settingsInfo.Default);
                            break;
                    }

                }
                else if (settingsInfo.SettingsType == typeof(string))
                {
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                            //break;
#endif
                        case XFormSettingsTarget.Local:
                        default:
                            if (!useValueFromSettingsInfo)
                                settingsInfo.Value = await XFormSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                            else
                                settingsInfo.Value = XFormSettingsHelper.ChangeSettingsType(settingsInfo.Value, settingsInfo.Default);
                            break;
                    }
                }
                else
                {
#if IOS
                    switch (target)
                    {
                        case MauiSettingsTarget.ICloud:
                            throw new NotSupportedException("SecureStorage is not available for iCloud sync!");
                        case MauiSettingsTarget.Local:
                        default:
                            throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");

                    }
#else
                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
#endif
                }
            }

            switch (mode)
            {
                case XFormSettingsActions.Load:
                    if (settingBaseAttribute?.DefaultValueInUse ?? false)
                    {
                        object defaultValue = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    }
                    if (secure && settingsInfo.Encrypt && !keepEncrypted)
                    {
                        if (string.IsNullOrEmpty(key))
                            throw new ArgumentNullException(nameof(key));
                        if (settingsInfo.Value is string secureString)
                        {
                            // Decrypt string
                            try
                            {
                                string decryptedString = EncryptionManager.DecryptStringFromBase64String(secureString, key);
                                // Throw on key missmatch
                                if (string.IsNullOrEmpty(decryptedString) && !string.IsNullOrEmpty(secureString))
                                    throw new Exception($"The secure string is not empty, but the decrypted string is. This indicates a key missmatch!");
                                if (!justTryLoading)
                                    XFormSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, decryptedString, settingsInfo.SettingsType);
                            }
                            catch (Exception ex)
                            {
                                OnEncryptionErrorEvent(new()
                                {
                                    Exception = ex,
                                    Key = key,
                                });
                                return XFormSettingsResults.EncryptionError;
                            }
                            break;
                        }
                    }
                    // Sets the loaded value back to the settingsObject
                    if (!justTryLoading)
                        XFormSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                    break;
                case XFormSettingsActions.Save:
                    // Get the value from the settingsObject
                    settingsInfo.Value = XFormSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case XFormSettingsTarget.Local:
                        default:
                            if (secure)
                            {
                                if (settingsInfo.Value is string secureString)
                                {
                                    if (settingsInfo.Encrypt && !string.IsNullOrEmpty(secureString))
                                    {
                                        if (string.IsNullOrEmpty(key))
                                            throw new ArgumentNullException(nameof(key));
                                        // Encrypt string
                                        try
                                        {
                                            string encryptedString = EncryptionManager.EncryptStringToBase64String(secureString, key);
                                            await XFormSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return XFormSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
                                        await XFormSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                XFormSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            }
                            break;
                    }
                    break;
                case XFormSettingsActions.Delete:
                    object fallbackValue = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    settingsInfo.Value = fallbackValue;
                    settingsInfo.Default = fallbackValue;
                    if (settingsObjectInfo.Info is not null)
                    {
                        XFormSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, fallbackValue, settingsInfo.SettingsType);
                    }
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            if (settingsInfo.Value != null)
                            {
                                // If there is a default value, do not delete the key. Instead write the default value
                                ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            }
                            else
                            {
                                // Otherwise delete the key from the cloud storage
                                ICloudStoreManager.DeleteValue(settingsInfo.Name);
                            }
                            break;
#endif
                        case XFormSettingsTarget.Local:
                        default:
                            if (secure)
                            {
                                if (settingsInfo.Value is string secureString)
                                {
                                    if (settingsInfo.Encrypt && !string.IsNullOrEmpty(secureString))
                                    {
                                        if (string.IsNullOrEmpty(key))
                                            throw new ArgumentNullException(nameof(key));
                                        // Encrypt string
                                        try
                                        {
                                            string encryptedString = EncryptionManager.EncryptStringToBase64String(secureString, key);
                                            await XFormSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return XFormSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
                                        await XFormSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                XFormSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            }
                            break;
                    }
                    break;
                case XFormSettingsActions.LoadDefaults:
                    object defaulSettingtValue = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
                    if (settingsObjectInfo.Info is not null)
                    {
                        XFormSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, defaulSettingtValue, settingsInfo.SettingsType);
                    }
                    settingsInfo.Value = defaulSettingtValue;
                    settingsInfo.Default = defaulSettingtValue;
                    switch (target)
                    {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                        case XFormSettingsTarget.Local:
                        default:
                            if (secure)
                            {
                                if (settingsInfo.Value is string secureString)
                                {
                                    if (settingsInfo.Encrypt && !string.IsNullOrEmpty(secureString))
                                    {
                                        if (string.IsNullOrEmpty(key))
                                            throw new ArgumentNullException(nameof(key));
                                        // Encrypt string
                                        try
                                        {
                                            string encryptedString = EncryptionManager.EncryptStringToBase64String(secureString, key);
                                            await XFormSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, encryptedString);
                                        }
                                        catch (Exception ex)
                                        {
                                            OnEncryptionErrorEvent(new()
                                            {
                                                Exception = ex,
                                                Key = key,
                                            });
                                            return XFormSettingsResults.EncryptionError;
                                        }
                                    }
                                    else
                                        await XFormSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                }
                                else
                                {
                                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                }
                            }
                            else
                            {
                                XFormSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                            }
                            break;
                    }
                    break;
                default:
                    break;
            }
            return XFormSettingsResults.Success;
        }

        static async Task<XFormSettingsInfo?> ProcessSettingsInfoAsKeyValuePairAsync(XFormSettingsMemberInfo settingsObjectInfo, XFormSettingsInfo settingsInfo, bool secureOnly = false, string? key = null, bool keeyEncrypted = false)
        {
            settingsInfo ??= new();
            XFormSettingBaseAttribute? settingBaseAttribute = null;
            if (settingsObjectInfo.Info is not null)
            {
                List<XFormSettingAttribute> settingBaseAttributes
                    = settingsObjectInfo.Info.GetCustomAttributes<XFormSettingAttribute>(inherit: false)
                    .ToList();
                if (settingBaseAttributes?.Count == 0)
                {
                    // If the member has not the needed MauiSettingsAttribute, continue with the search
                    return null;
                }
                settingBaseAttribute = settingBaseAttributes.FirstOrDefault();
                settingsInfo.Name = XFormSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = XFormSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                settingsInfo.Default = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);
            }
            if (settingBaseAttribute is XFormSettingAttribute settingAttribute)
            {
                bool secure = settingAttribute.Secure;
                // Save the states
                settingsInfo.IsSecure = secure;
                settingsInfo.Encrypt = settingAttribute.Encrypt;
                settingsInfo.SkipForExport = settingAttribute.SkipForExport;
                if (!secure)
                {
                    // If only secure storage should be loaded, stop here.
                    if (secureOnly)
                        return null;
                    settingsInfo.Value = XFormSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
                }
                else if (settingsInfo.SettingsType == typeof(string))
                {
                    settingsInfo.Value = await XFormSettingsHelper.GetSecureSettingsValueAsync(settingsInfo.Name, settingsInfo.Default as string);
                    if (settingsInfo.Encrypt && !keeyEncrypted)
                    {
                        if (string.IsNullOrEmpty(key))
                            throw new ArgumentNullException(nameof(key));
                        // Decrypt string
                        if (settingsInfo.Value is string secureString)
                        {
                            try
                            {
                                string decryptedString = EncryptionManager.DecryptStringFromBase64String(secureString, key);
                                // Throw on key missmatch
                                if (string.IsNullOrEmpty(decryptedString) && !string.IsNullOrEmpty(secureString))
                                    throw new Exception($"The secure string is not empty, but the decrypted string is. This indicates a key missmatch!");
                                settingsInfo.Value = decryptedString;
                            }
                            catch (Exception ex)
                            {
                                OnEncryptionErrorEvent(new()
                                {
                                    Exception = ex,
                                    Key = key,
                                });
                                return null;
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                }
            }
            return settingsInfo;
        }

        #endregion

        #endregion

        #region Events

        public static event EventHandler? ErrorEvent;
        protected static void OnErrorEvent(ErrorEventArgs e)
        {
            ErrorEvent?.Invoke(typeof(XFormSettingsGeneric<SO>), e);
        }

        public static event EventHandler? EncryptionErrorEvent;
        protected static void OnEncryptionErrorEvent(EncryptionErrorEventArgs e)
        {
            EncryptionErrorEvent?.Invoke(typeof(XFormSettingsGeneric<SO>), e);
        }
        #endregion
    }
}
