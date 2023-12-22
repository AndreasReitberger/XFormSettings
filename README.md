# XFormSettings
A nuget to improve settings storage (locally and eventually in the cloud) on .NETStandard Xamarin.Forms projects.
There is also an `Maui` version, which can be found here.
https://github.com/AndreasReitberger/MauiSettings

The plugin idea is based on the Advexp.Settings.Local nuget by Alexey Ivakin</br>
Repo: https://bitbucket.org/advexp/component-advexp.settings/src/master/</br>
License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)</br>

This project was created from scratch, however uses the basic idea to keep all Settings in the
static object. All taken and changed files have been marked so.

# Nuget
Get the latest version from nuget.org<br>
[![NuGet](https://img.shields.io/nuget/v/SettingsXForm.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/SettingsXForm/)
[![NuGet](https://img.shields.io/nuget/dt/SettingsXForm.svg)](https://www.nuget.org/packages/SettingsXForm)

# Usage
## Settings Object
In the .NET MAUI project, create a new `Class` (for instance `SettingsApp.cs`) holding your setting properties.

```cs
public partial class SettingsApp : XFormSettings<SettingsApp>
{
    
    #region Settings

    #region Version
    [XFormSetting(Name = nameof(App_SettingsVersion))]
    public static Version App_SettingsVersion { get; set; } = new("1.0.0");

    #endregion

    #region CloudSync
    [XFormSetting(Name = nameof(Cloud_ShowInitialPrompt), DefaultValue = true)]
    public static bool Cloud_ShowInitialPrompt { get; set; }

    [XFormSetting(Name = nameof(Cloud_ShowInitialPrompt), DefaultValue = SettingsStaticDefault.Cloud_EnableSync)]
    public static bool Cloud_EnableSync { get; set; }
    #endregion

    #region Theme 
    [XFormSetting(Name = nameof(Theme_UseDeviceDefaultSettings), DefaultValue = SettingsStaticDefault.General_UseDeviceSettings)]
    public static bool Theme_UseDeviceDefaultSettings { get; set; }

    [XFormSetting(Name = nameof(Theme_UseDarkTheme), DefaultValue = SettingsStaticDefault.General_UseDarkTheme)]
    public static bool Theme_UseDarkTheme { get; set; }

    [XFormSetting(Name = nameof(Theme_PrimaryThemeColor), DefaultValue = SettingsStaticDefault.Theme_PrimaryThemeColor)]
    public static string Theme_PrimaryThemeColor { get; set; }

    #endregion

    #region Localization

    [XFormSetting(Name = nameof(Localization_CultureCode), DefaultValue = SettingsStaticDefault.Localization_Default)]
    public static string Localization_CultureCode { get; set; }

    #endregion

    #endregion
}
```

## Load
To load the settings from the storage, call `SettingsApp.LoadSettings()` (mostly in the `App` constructor of your App.xmls file. The project uses the `Xamarin.Essentials.Preferences` in order to store the settings on the corresponding device.

## Save
Whenever you do make changes to a settings property of your class, call `SettingsApp.SaveSettings()`. This will write the settings to the storage.
