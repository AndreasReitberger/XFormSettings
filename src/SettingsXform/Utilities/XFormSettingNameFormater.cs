﻿using AndreasReitberger.XForm.Attributes;
using System.Reflection;

namespace AndreasReitberger.XForm.Utilities
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */
    internal class XFormSettingNameFormater
    {
        #region Variables
        public const string Version = "v1";
        public const string NamePrefix = "AR";
        #endregion

        #region Methods
        public static string GetFullSettingName(Type? settingsType, MemberInfo mi, XFormSettingBaseAttribute? baseSettingAttr)
        {
            string name;
            if (string.IsNullOrEmpty(baseSettingAttr?.Name))
            {
                name = mi.Name;
            }
            else
            {
                name = baseSettingAttr.Name;
                settingsType = null;
            }
            return $"{NamePrefix}_{Version}_{name}{(settingsType == null ? "" : $"_{settingsType}")}";
        }
        public static string GetSettingsNamePrefix() => $"{NamePrefix}_{Version}";
        #endregion
    }
}
