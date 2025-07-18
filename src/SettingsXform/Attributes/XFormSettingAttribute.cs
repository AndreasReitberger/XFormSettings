﻿namespace AndreasReitberger.XForm.Attributes
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class XFormSettingAttribute : XFormSettingBaseAttribute
    {
        #region Properties
        public bool Secure { get; set; } = false;
        public bool Encrypt { get; set; } = false;
        public bool SkipForExport { get; set; } = false;
        #endregion
    }
}
