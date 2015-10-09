namespace LocultMetro
{
    using FirstFloor.ModernUI.Presentation;
    using Settings.Interfaces;
    using SettingsModel.Interfaces;
    using System.Windows.Media;

    /// <summary>
    /// Class contains methods necessary to initialize the applications settings model.
    /// </summary>
    internal static class MetroSettingsDefaults
    {
        /// <summary>
        /// Create the minimal settings model that should be used for every locult application.
        /// </summary>
        /// <param name="settings"></param>
        internal static void CreateAppearanceSettings(IEngine options, ISettingsManager settings)
        {
            const string groupName = "Appearance";

            options.AddOption(groupName, "ThemeDisplayName", typeof(string), false, "Light");
            options.AddOption(groupName, "AccentColor", typeof(Color), false, Color.FromRgb(0x33, 0x99, 0xff));

            options.AddOption(groupName, "DefaultFontSize", typeof(int), false, 18);
            options.AddOption(groupName, "FixedFontSize", typeof(int), false, 14);
        }
    }
}
