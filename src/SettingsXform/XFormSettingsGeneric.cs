﻿using AndreasReitberger.XForm.Attributes;
#if IOS
using AndreasReitberger.XForm.Cloud;
#endif
using AndreasReitberger.XForm.Enums;
using AndreasReitberger.XForm.Helper;
using AndreasReitberger.XForm.Utilities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        static object _settingsObject;
        public static SO SettingsObject
        {
            get
            {
                if (_settingsObject == null)
                {
                    _settingsObject = new SO();
                }
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
        public static void LoadSettings()
        {
            LoadSettings(settings: SettingsObject);
        }
        public static void LoadSetting<T>(Expression<Func<SO, T>> value)
        {
            LoadObjectSetting(SettingsObject, value);
        }
        public static async Task LoadSettingAsync<T>(Expression<Func<SO, T>> value)
        {
            await Task.Run(async delegate
            {
                await LoadObjectSettingAsync(SettingsObject, value);
            });
        }
        public void LoadObjectSettings()
        {
            LoadSettings(this);
        }
        public static void LoadObjectSetting<T>(object settingsObject, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings: settingsObject, value, XFormSettingsActions.Load);
        }
        public static async Task LoadObjectSettingAsync<T>(object settingsObject, Expression<Func<SO, T>> value)
        {
            await Task.Run(async delegate
            {
                await GetExpressionMetaAsync(settings: settingsObject, value, XFormSettingsActions.Load);
            });
        }
        public static void LoadSettings(object settings)
        {
            GetClassMeta(settings: settings, mode: XFormSettingsActions.Load);
        }
        public static async Task LoadSettingsAsync()
        {
            await Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject);
            });
        }
        public static async Task LoadSettingsAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Load);
            });
        }
        public static async Task LoadSecureSettingsAsync()
        {
            await Task.Run(async delegate
            {
                await LoadSecureSettingsAsync(settings: SettingsObject);
            });
        }
        public static async Task LoadSecureSettingsAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Load, secureOnly: true);
            });
        }
        public static async Task LoadSettingsAsync(Dictionary<string, Tuple<object, Type>> dictionary, bool save = true)
        {
            await Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, dictionary: dictionary, save: save);
            });
        }
        public static async Task LoadSettingsAsync(string key, Tuple<object, Type> data, bool save = true)
        {
            await Task.Run(async delegate
            {
                await LoadSettingsAsync(settings: SettingsObject, dictionary: new() { { key, data} }, save: save);
            });
        }
        public static async Task LoadSettingsAsync(object settings, Dictionary<string, Tuple<object, Type>> dictionary, bool save = true)
        {
            await Task.Run(async delegate
            {
                await GetMetaFromDictionaryAsync(settings: settings, dictionary: dictionary, mode: XFormSettingsActions.Load, secureOnly: false);
                // Save the restored settings right away
                if (save) await SaveSettingsAsync(settings: settings);
            });
        }
        #endregion

        #region SaveSettings
        public static void SaveSettings()
        {
            SaveSettings(SettingsObject);
        }
        public static void SaveSetting<T>(Expression<Func<SO, T>> value)
        {
            SaveObjectSetting(SettingsObject, value);
        }
        public static void SaveObjectSetting<T>(object settings, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings, value, XFormSettingsActions.Save);
        }
        public void SaveObjectSetting<T>(Expression<Func<SO, T>> value)
        {
            SaveObjectSetting(this, value);
        }
        public void SaveObjectSettings()
        {
            SaveSettings(this);
        }
        public static void SaveSettings(object settings)
        {
            GetClassMeta(settings, XFormSettingsActions.Save);
        }
        public static async Task SaveSettingsAsync()
        {
            await Task.Run(async delegate
            {
                await SaveSettingsAsync(settings: SettingsObject);
            });
        }
        public static async Task SaveSettingsAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Save);
            });
        }
        public static async Task SaveSecureSettingsAsync()
        {
            await Task.Run(async delegate
            {
                await SaveSecureSettingsAsync(settings: SettingsObject);
            });
        }
        public static async Task SaveSecureSettingsAsync(object settings)
        {
            await Task.Run(async delegate
            {
                await GetClassMetaAsync(settings: settings, mode: XFormSettingsActions.Save, secureOnly: true);
            });
        }
        #endregion

        #region DeleteSettings
        public static void DeleteSettings()
        {
            DeleteSettings(SettingsObject);
        }
        public static void DeleteSetting<T>(Expression<Func<SO, T>> value)
        {
            DeleteObjectSetting(SettingsObject, value);
        }
        public void DeleteObjectSetting<T>(Expression<Func<SO, T>> value)
        {
            DeleteObjectSetting(this, value);
        }
        public static void DeleteObjectSetting<T>(object settings, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings, value, XFormSettingsActions.Delete);
        }
        public void DeleteObjectSettings()
        {
            DeleteSettings(this);
        }
        public static void DeleteSettings(object settings)
        {
            GetClassMeta(settings, XFormSettingsActions.Delete);
        }
        #endregion

        #region LoadDefaults
        public static void LoadDefaultSettings()
        {
            LoadDefaultSettings(SettingsObject);
        }
        public static void LoadDefaultSetting<T>(Expression<Func<SO, T>> value)
        {
            LoadObjectDefaultSetting(SettingsObject, value);
        }

        public void LoadObjectDefaultSetting<T>(Expression<Func<SO, T>> value)
        {
            LoadObjectDefaultSetting(this, value);
        }

        public static void LoadObjectDefaultSetting<T>(object settings, Expression<Func<SO, T>> value)
        {
            GetExpressionMeta(settings, value, XFormSettingsActions.LoadDefaults);
        }
        public void LoadObjectDefaultSettings()
        {
            LoadDefaultSettings(this);
        }
        public static void LoadDefaultSettings(object settings)
        {
            GetClassMeta(settings, XFormSettingsActions.LoadDefaults);
        }
        #endregion

        #region Conversion

        public static async Task<Dictionary<string, Tuple<object, Type>>> ToDictionaryAsync()
        {
            return await ToDictionaryAsync(settings: SettingsObject);
        }
        public static async Task<Dictionary<string, Tuple<object, Type>>> ToDictionaryAsync(object settings)
        {
            if (true)
            {
                Dictionary<string, Tuple<object, Type>> setting = new();
                //List<MemberInfo> members = GetClassMetaAsList(settings);

                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

                XFormSettingsMemberInfo settingsObjectInfo = new();
                XFormSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = mInfo;
                    // Handles saving the settings to the Maui.Storage.Preferences
                    XFormSettingsInfo settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(settingsObjectInfo, settingsInfo);
                    if (settingsPair != null)
                    {
                        setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                    }
                }
                /*
                members?.ForEach(member =>
                {
                    settingsObjectInfo.OrignalSettingsObject = settings;
                    settingsObjectInfo.Info = member;
                    var settingsPair = ProcessSettingsInfoAsKeyValuePair(settingsObjectInfo, settingsInfo);
                    if (settingsPair != null)
                    {
                        setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                    }
                });
                */
            return setting;
            }
        }

        public static async Task<ConcurrentDictionary<string, Tuple<object, Type>>> ToConcurrentDictionaryAsync()
        {
            return await ToConcurrentDictionaryAsync(settings: SettingsObject);
        }
        public static async Task<ConcurrentDictionary<string, Tuple<object, Type>>> ToConcurrentDictionaryAsync(object settings)
        {
            ConcurrentDictionary<string, Tuple<object, Type>> setting = new();
            List<MemberInfo> members = GetClassMetaAsList(settings);

            XFormSettingsMemberInfo settingsObjectInfo = new();
            XFormSettingsInfo settingsInfo = new();

            foreach (MemberInfo mInfo in members)
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = mInfo;
                // Handles saving the settings to the Maui.Storage.Preferences
                XFormSettingsInfo settingsPair = await ProcessSettingsInfoAsKeyValuePairAsync(settingsObjectInfo, settingsInfo);
                if (settingsPair != null)
                {
                    setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                }
            }
            /*
            members?.ForEach(member =>
            {
                settingsObjectInfo.OrignalSettingsObject = settings;
                settingsObjectInfo.Info = member;
                var settingsPair = ProcessSettingsInfoAsKeyValuePair(settingsObjectInfo, settingsInfo);
                if(settingsPair != null)
                {
                    setting.TryAdd(settingsPair.Name, new Tuple<object, Type>(settingsPair.Value ?? settingsPair.Default, settingsPair.SettingsType));
                }
            });
            */
            return setting;
        }

        public static async Task<Tuple<string, Tuple<object, Type>>> ToSettingsTupleAsync<T>(Expression<Func<SO, T>> value)
        {
            return await ToSettingsTupleAsync(settings: SettingsObject, value: value);
        }

        public static async Task<Tuple<string, Tuple<object, Type>>> ToSettingsTupleAsync<T>(object settings, Expression<Func<SO, T>> value)
        {
            XFormSettingsInfo info = await GetExpressionMetaAsKeyValuePairAsync(settings: settings, value: value);
            return new(info.Name, new(info.Value, info.SettingsType));
        }
        #endregion

        #region Private
        static List<MemberInfo> GetClassMetaAsList(object settings)
        {
            lock (lockObject)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

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
        static async Task GetClassMetaAsync(object settings, XFormSettingsActions mode, XFormSettingsTarget target = XFormSettingsTarget.Local, bool secureOnly = false)
        {
            //lock (lockObject)
            if (true)
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
                    _ = await ProcessSettingsInfoAsync(settingsObjectInfo, settingsInfo, mode, target, secureOnly: secureOnly);
                }
            }
        }
        static async Task GetMetaFromDictionaryAsync(object settings, Dictionary<string, Tuple<object, Type>> dictionary, XFormSettingsActions mode, XFormSettingsTarget target = XFormSettingsTarget.Local, bool secureOnly = false)
        {
            //lock (lockObject)
            if (true)
            {
                // Get all member infos from the passed settingsObject
                IEnumerable<MemberInfo> declaredMembers = settings.GetType().GetTypeInfo().DeclaredMembers;

                XFormSettingsMemberInfo settingsObjectInfo = new();
                XFormSettingsInfo settingsInfo = new();

                foreach (MemberInfo mInfo in declaredMembers)
                {
                    bool useValueFromSettingsInfo = false;
                    // Try to find the matching key
                    KeyValuePair<string, Tuple<object, Type>>? keyPair = dictionary?.FirstOrDefault(keypair => 
                        keypair.Key.EndsWith(mInfo.Name
                        //?.Replace("get_", string.Empty)
                        ));
                    if (keyPair?.Key != null)
                    {
                        useValueFromSettingsInfo = true;
                        // If a matching key was found, prepare the settingsInfo with the loaded data
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
                    _ = await ProcessSettingsInfoAsync(settingsObjectInfo, settingsInfo, mode, target, secureOnly: secureOnly, useValueFromSettingsInfo: useValueFromSettingsInfo);
                }
            }
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

        static async Task GetExpressionMetaAsync<T>(object settings, Expression<Func<SO, T>> value, XFormSettingsActions mode, XFormSettingsTarget target = XFormSettingsTarget.Local)
        {

            if (value.Body is MemberExpression memberExpression)
            {
                _ = await ProcessSettingsInfoAsync(new XFormSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new XFormSettingsInfo(), mode, target);
            }        
        }

        static async Task<XFormSettingsInfo> GetExpressionMetaAsKeyValuePairAsync<T>(object settings, Expression<Func<SO, T>> value)
        {
            if (value.Body is MemberExpression memberExpression)
            {
                return await ProcessSettingsInfoAsKeyValuePairAsync(new XFormSettingsMemberInfo()
                {
                    OrignalSettingsObject = settings,
                    Info = memberExpression.Member,

                }, new XFormSettingsInfo());
            }
            return new();
        }

        static bool ProcessSettingsInfo(XFormSettingsMemberInfo settingsObjectInfo, XFormSettingsInfo settingsInfo, XFormSettingsActions mode, XFormSettingsTarget target, bool throwOnError = false)
        {
            settingsInfo ??= new();
            XFormSettingBaseAttribute settingBaseAttribute = null;
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

        static async Task<bool> ProcessSettingsInfoAsync(XFormSettingsMemberInfo settingsObjectInfo, XFormSettingsInfo settingsInfo, XFormSettingsActions mode, XFormSettingsTarget target, bool secureOnly = false, bool useValueFromSettingsInfo = false)
        {
            settingsInfo ??= new();
            XFormSettingBaseAttribute settingBaseAttribute = null;
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
            }
            bool secure = false;
            if (settingBaseAttribute is XFormSettingAttribute settingAttribute)
            {
                secure = settingAttribute.Secure;
                if (!secure)
                {
                    // If only secure storage should be loaded, stop here.
                    if (secureOnly) 
                        return true;
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
                    // Sets the loaded value back to the settingsObject
                    XFormSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
                    /* => Calling SaveSettingsAsync() after loading
                    if(useValueFromSettingsInfo)
                    {
                        // If the value is used from the dictionary, save it.
                        switch (target)
                        {
#if IOS
                        case MauiSettingsTarget.ICloud:
                            ICloudStoreManager.SetValue(settingsInfo.Name, settingsInfo.Value?.ToString());
                            break;
#endif
                            case MauiSettingsTarget.Local:
                            default:
                                if (secure)
                                {
                                    if (settingsInfo.Value is string secureString)
                                    {
                                        await MauiSettingsHelper.SetSecureSettingsValueAsync(settingsInfo.Name, secureString);
                                    }
                                    else
                                    {
                                        throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                                    }
                                }
                                else
                                {
                                    MauiSettingsHelper.SetSettingsValue(settingsInfo.Name, settingsInfo.Value);
                                }
                                break;
                        }
                    }*/
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
            return true;
        }

        static async Task<XFormSettingsInfo> ProcessSettingsInfoAsKeyValuePairAsync(XFormSettingsMemberInfo settingsObjectInfo, XFormSettingsInfo settingsInfo, bool secureOnly = false)
        {
            settingsInfo ??= new();
            XFormSettingBaseAttribute settingBaseAttribute = null;
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
            }
            if (settingsObjectInfo.Info is not null)
            {
                settingsInfo.Name = XFormSettingNameFormater.GetFullSettingName(settingsObjectInfo.OrignalSettingsObject.GetType(), settingsObjectInfo.Info, settingBaseAttribute);
                settingsInfo.SettingsType = (settingsInfo.SettingsType = XFormSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));

                settingsInfo.Default = XFormSettingsObjectHelper.GetDefaultValue(settingBaseAttribute, settingsInfo.SettingsType);

                //Type type = (settingsInfo.SettingsType = MauiSettingsObjectHelper.GetSettingType(settingsObjectInfo.Info));
                //settingsInfo.Value = MauiSettingsObjectHelper.GetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject);
                //settingsInfo.Value = MauiSettingsHelper.GetSettingsValue(settingsInfo.Name, settingsInfo.Default);
            }
            if (settingBaseAttribute is XFormSettingAttribute settingAttribute)
            {
                bool secure = settingAttribute.Secure;
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
                }
                else
                {
                    throw new InvalidDataException($"Only data type of '{typeof(string)}' is allowed for secure storage!");
                }
            }
            // Sets the loaded value back to the settingsObject
            //MauiSettingsObjectHelper.SetSettingValue(settingsObjectInfo.Info, settingsObjectInfo.OrignalSettingsObject, settingsInfo.Value, settingsInfo.SettingsType);
            return settingsInfo;
        }

        #endregion

        #endregion
    }
}
