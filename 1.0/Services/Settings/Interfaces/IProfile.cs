namespace Settings.Interfaces
{
    using Settings.UserProfile;
    using System;

    public interface IProfile
    {
        void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft, double SystemParameters_VirtualScreenTop);
        string GetLastActivePath();
        string LastActiveSolution { get; set; }
        string LastActiveTargetFile { get; set; }
        ViewPosSizeModel MainWindowPosSz { get; set; }
    }
}
