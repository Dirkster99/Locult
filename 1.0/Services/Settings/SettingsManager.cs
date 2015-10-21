namespace Settings
{
    using Settings.Interfaces;
    using Settings.ProgramSettings;
    using Settings.Themes;
    using Settings.UserProfile;
    using Settings.UserProfile.Persistable;
    using SettingsModel.Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// This class keeps track of program options and user profile (session) data.
    /// Both data items can be added and are loaded on application start to restore
    /// the program state of the last user session or to implement the default
    /// application state when starting the application for the very first time.
    /// </summary>
    internal class SettingsManager : ISettingsManager
    {
        #region fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IOptionsPanel mSettingsDataPanel = null;

        private IProfile mSessionData = null;
        #endregion fields

        #region constructor
        public SettingsManager()
        {
            mSettingsDataPanel = new OptionsPanel();
            SessionData = new Profile();

            Themes = new ThemeInfos();
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Implement <seealso cref="IOptionsPanel"/> method to query options from model container.
        /// </summary>
        public IEngine Options
        {
            get
            {
                return mSettingsDataPanel.Options;
            }
        }

        /// <summary>
        /// Gets the user profile session data for this application.
        /// </summary>
        public IProfile SessionData
        {
            get
            {
                if (mSessionData == null)
                    mSessionData = new Profile();

                return mSessionData;
            }

            private set
            {
                if (mSessionData != value)
                    mSessionData = value;
            }
        }

        #region min max definitions for useful option values
        public int IconSizeMin
        {
            get
            {
                return 16;
            }
        }

        public int IconSizeMax
        {
            get
            {
                return 114;
            }
        }

        public int FontSizeMin
        {
            get
            {
                return 8;
            }
        }

        public int FontSizeMax
        {
            get
            {
                return 220;
            }
        }
        #endregion min max definitions for useful option values

        /// <summary>
        /// Gets the default icon size for the application.
        /// </summary>
        public int DefaultIconSize
        {
            get
            {
                return 32;
            }
        }

        /// <summary>
        /// Gets the default font size for the application.
        /// </summary>
        public int DefaultFontSize
        {
            get
            {
                return 14;
            }
        }

        /// <summary>
        /// Gets the default fixed font size for the application.
        /// </summary>
        public int DefaultFixedFontSize
        {
            get
            {
                return 12;
            }
        }

        /// <summary>
        /// Gets the internal name and Uri source for all available themes.
        /// </summary>
        public IThemeInfos Themes { get; private set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Get a list of all supported languages in Edi.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LanguageCollection> GetSupportedLanguages()
        {
            List<LanguageCollection> ret = new List<LanguageCollection>();

            ret.Add(new LanguageCollection() { Language = "en", Locale = "US", Name = "English (English)" });
            ret.Add(new LanguageCollection() { Language = "de", Locale = "DE", Name = "Deutsch (German)" });

            return ret;
        }

        /// <summary>
        /// Determine whether program options are valid and corrext
        /// settings if they appear to be invalid on current system
        /// </summary>
        public void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft,
                                        double SystemParameters_VirtualScreenTop)
        {
            //// Dirkster: Not sure whether this is working correctly yet...
            //// this.SessionData.CheckSettingsOnLoad(SystemParameters_VirtualScreenLeft,
            ////                                      SystemParameters_VirtualScreenTop);
        }

        #region Load Save UserSessionData
        /// <summary>
        /// Save program options into persistence.
        /// See <seealso cref="SaveOptions"/> to save program options on program end.
        /// </summary>
        /// <param name="sessionDataFileName"></param>
        /// <returns></returns>
        public void LoadSessionData(string sessionDataFileName)
        {
            // Just get the defaults if serilization wasn't working here...
            SessionData = new Profile();
            SessionData.SetObjectFromPersistence(ProfilePersistable.GetObjectFromPersistence(sessionDataFileName));
        }

        /// <summary>
        /// Save program options into persistence.
        /// See <seealso cref="LoadOptions"/> to load program options on program start.
        /// </summary>
        /// <param name="sessionDataFileName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveSessionData(string sessionDataFileName, IProfile model)
        {
            var persistable = model.GetObjectForPersistence();

            return ProfilePersistable.SaveObjectToPersistence(sessionDataFileName, persistable);
        }
        #endregion Load Save UserSessionData
        #endregion methods
    }
}
