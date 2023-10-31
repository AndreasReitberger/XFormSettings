using System.Reflection;

namespace AndreasReitberger.XForm.Helper
{
    /*
     * Based on the idea of Advexp.Settings.Local by Alexey Ivakin
     * Repo: https://bitbucket.org/advexp/component-advexp.settings
     * License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)
     * 
     * Modifed by Andreas Reitberger to work on .NET MAUI
     */
    internal class XFormSettingsMemberInfo
    {
        #region Properties
        public object OrignalSettingsObject { get; set; }
        public MemberInfo Info { get; set; }
        public Type SettingsType { get; set; }
        #endregion
    }
}
