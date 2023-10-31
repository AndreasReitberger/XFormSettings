namespace AndreasReitberger.XForm.Enums
{
    public enum MauiSettingsTarget
    {
        Local,
#if IOS
        ICloud,
#endif
    }
}
