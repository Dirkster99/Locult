namespace Settings.Interfaces
{
    using Settings.ProgramSettings;
    using Settings.Themes;
    using System.Collections.Generic;

    public interface ISettingsManager : IOptionsPanel
    {
        #region properties
        Settings.Interfaces.IProfile SessionData { get; }
        ////Settings.Interfaces.IOptions SettingData { get; } > replaced qith Query() method in IOptionsPanel

        int IconSizeMin { get; }
        int IconSizeMax { get; }

        int FontSizeMin { get; }
        int FontSizeMax { get; }

        /// <summary>
        /// Gets the default icon size for the application.
        /// </summary>
        int DefaultIconSize { get; }

        /// <summary>
        /// Gets the default font size for the application.
        /// </summary>
        int DefaultFontSize { get; }

        /// <summary>
        /// Gets the default fixed font size for the application.
        /// </summary>
        int DefaultFixedFontSize { get; }

        /// <summary>
        /// Gets the internal name and Uri source for all available themes.
        /// </summary>
        IThemeInfos Themes { get; }
        #endregion properties

        #region methods
        void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft, double SystemParameters_VirtualScreenTop);

        ////void LoadOptions(string settingsFileName);
        void LoadSessionData(string sessionDataFileName);

        ////bool SaveOptions(string settingsFileName, Settings.Interfaces.IOptions optionsModel);
        bool SaveSessionData(string sessionDataFileName, Settings.Interfaces.IProfile model);

        /// <summary>
        /// Get a list of all supported languages in Edi.
        /// </summary>
        /// <returns></returns>
        IEnumerable<LanguageCollection> GetSupportedLanguages();
        #endregion methods
    }
}
