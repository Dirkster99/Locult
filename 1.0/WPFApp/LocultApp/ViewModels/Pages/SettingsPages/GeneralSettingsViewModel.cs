namespace LocultApp.ViewModels.Pages.SettingsPages
{
    using MSTranslate.Interfaces;
    using Settings.Interfaces;
    using Settings.ProgramSettings;
    using Settings.Translate;
    using SettingsModel.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Windows.Media;

    /// <summary>
    /// Implements a viewmodel for viewing and editing general
    /// (build-in) application settings.
    /// 
    /// Other, non-buildin, dynamic, or on-(addin)-config-demand settingmodels
    /// can also be part of this application but have to implement
    /// their own viewmodel.
    /// </summary>
    public class GeneralSettingsViewModel : SettingsPageBaseViewModel, System.IDisposable
    {
        #region fields
        private TranslationService mMSTranslateService = null;
        private bool mReloadFilesOnAppStart = true;

        private ILanguageCode mDefaultSourceLanguageSelected;
        private ILanguageCode mDefaultTargetLanguageSelected;

        private LanguageCollection mLanguageSelected;
        private SettingsPageBaseViewModel mBookmarkedFolders = null;

        private int _IconSize = 32;
        private bool mColorGridOption;
        private Color mAlternatingColor;
        private bool mIsIconSizeEditVisible = true;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="settingsManager"></param>
        public GeneralSettingsViewModel(string displayCaption)
            : base(displayCaption)
        {
            ConstructDefaults();
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected GeneralSettingsViewModel()
            : base()
        {
            ConstructDefaults();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets an object with commands and properties for managing a list of folder bookmarks.
        /// </summary>
        public SettingsPageBaseViewModel BookmarkedFolders
        {
            get
            {
                return mBookmarkedFolders;
            }

            private set
            {
                if (mBookmarkedFolders != value)
                {
                    mBookmarkedFolders = value;
                    RaisePropertyChanged(() => BookmarkedFolders);
                }
            }
        }

        #region Language Localization Support
        /// <summary>
        /// Get list of GUI languages supported in this application.
        /// </summary>
        public List<LanguageCollection> Languages { get; private set; }

        /// <summary>
        /// Get/set language application language.
        /// </summary>
        public LanguageCollection LanguageSelected
        {
            get
            {
                return mLanguageSelected;
            }

            set
            {
                if (mLanguageSelected != value)
                {
                    mLanguageSelected = value;
                    RaisePropertyChanged(() => LanguageSelected);
                    IsDirty = true;
                }
            }
        }
        #endregion Language Localization Support

        #region Default Solution Translation Settings
        /// <summary>
        /// Get/set default source language.
        /// </summary>
        public ILanguageCode DefaultSourceLanguageSelected
        {
            get
            {
                return mDefaultSourceLanguageSelected;
            }

            set
            {
                if (mDefaultSourceLanguageSelected != value)
                {
                    mDefaultSourceLanguageSelected = value;
                    RaisePropertyChanged(() => DefaultSourceLanguageSelected);
                }
            }
        }

        /// <summary>
        /// Get/set default target language.
        /// </summary>
        public ILanguageCode DefaultTargetLanguageSelected
        {
            get
            {
                return mDefaultTargetLanguageSelected;
            }

            set
            {
                if (mDefaultTargetLanguageSelected != value)
                {
                    mDefaultTargetLanguageSelected = value;
                    RaisePropertyChanged(() => DefaultTargetLanguageSelected);
                }
            }
        }

        /// <summary>
        /// Gets a list of languages that can be used as a default for
        /// source or target language selection.
        /// </summary>
        public List<ILanguageCode> DefaultLanguages
        {
            get
            {
                var coll = ServiceLocator.ServiceContainer.Instance.GetService<ITranslator>();
                return coll.LanguageList;
            }
        }
        #endregion Default Solution Translation Settings

        /// <summary>
        /// Gets/sets whether the application restarts with reload of files from last session or not.
        /// </summary>
        public bool ReloadFilesOnAppStart
        {
            get
            {
                return mReloadFilesOnAppStart;
            }

            set
            {
                if (mReloadFilesOnAppStart != value)
                {
                    mReloadFilesOnAppStart = value;
                    RaisePropertyChanged(() => ReloadFilesOnAppStart);
                    IsDirty = true;
                }
            }
        }

        #region GridColor Options
        /// <summary>
        /// Gets/sets whether the main grid control should contain colored bars or not. 
        /// </summary>
        public bool ColorGridOption
        {
            get
            {
                return mColorGridOption;
            }

            set
            {
                if (mColorGridOption != value)
                {
                    mColorGridOption = value;
                    RaisePropertyChanged(() => ColorGridOption);
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Get/sets <seealso cref="SolidColorBrush"/> of the alternating color of the grid display.
        /// </summary>
        public Color AlternatingColor
        {
            get
            {
                return mAlternatingColor;
            }

            set
            {
                if (value == null)
                    return;

                if (mAlternatingColor != value)
                {
                    mAlternatingColor = value;
                    RaisePropertyChanged(() => AlternatingColor);
                    IsDirty = true;
                }
            }
        }
        #endregion GridColor Options

        /// <summary>
        /// Gets/sets the URI of the Microsoft Translator Service.
        /// </summary>
        public string TranslationServiceUri
        {
            get
            {
                if (mMSTranslateService == null)
                    return null;

                return mMSTranslateService.TranslationServiceUri;
            }

            set
            {
                if (mMSTranslateService.TranslationServiceUri != value)
                {
                    mMSTranslateService.TranslationServiceUri = value;
                    RaisePropertyChanged(() => TranslationServiceUri);
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets/sets the password of the Microsoft Translator Service.
        /// </summary>
        public SecureString TranslationServicePassword
        {
            get
            {
                if (mMSTranslateService == null)
                    return null;

                return mMSTranslateService.TranslationServicePassword;
            }

            set
            {
                if (mMSTranslateService.TranslationServicePassword != value)
                {
                    mMSTranslateService.TranslationServicePassword = value;
                    RaisePropertyChanged(() => TranslationServicePassword);
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets/sets the user name (login) of the Microsoft Translator Service.
        /// </summary>
        public SecureString TranslationServiceUser
        {
            get
            {
                if (mMSTranslateService == null)
                    return null;

                return mMSTranslateService.TranslationServiceUser;
            }

            set
            {
                if (mMSTranslateService.TranslationServiceUser != value)
                {
                    mMSTranslateService.TranslationServiceUser = value;
                    RaisePropertyChanged(() => TranslationServiceUser);
                    IsDirty = true;
                }
            }
        }

        #region IconSize
        /// <summary>
        /// Gets/sets whether the Icon Size editing control is visible or not.
        /// </summary>
        public bool IsIconSizeEditVisible
        {
            get
            {
                return mIsIconSizeEditVisible;
            }

            set
            {
                if (mIsIconSizeEditVisible != value)
                {
                    mIsIconSizeEditVisible = value;
                    RaisePropertyChanged(() => IsIconSizeEditVisible);
                }
            }
        }

        /// <summary>
        /// Gets/sets the size of an icon for display.
        /// </summary>
        public int IconSize
        {
            get
            {
                return _IconSize;
            }

            set
            {
                if (_IconSize != value)
                {
                    _IconSize = value;
                    IsDirty = true;
                    RaisePropertyChanged(() => IconSize);
                }
            }
        }

        /// <summary>
        /// Gets the minimal size of an icon for display.
        /// </summary>
        public int IconSizeMin
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the maximum size of an icon for display.
        /// </summary>
        public int IconSizeMax
        {
            get;
            private set;
        }
        #endregion IconSize
        #endregion properties

        #region methods
        /// <summary>
        /// Reset the view model to those options that are going to be presented for editing.
        /// </summary>
        /// <param name="settingData"></param>
        public override void ApplyOptionsFromModel(IEngine optionsEngine)
        {
            var group = optionsEngine.GetOptionGroup("Options");
            LoadOptionsFromModel(group);

            if (mBookmarkedFolders != null)
            {
                mBookmarkedFolders.DirtyFlagChangedEvent -= BookmarkedFolders_DirtyFlagChangedEvent;
                mBookmarkedFolders = null;
            }

            // Construct bookmark collection and folder browser viewmodels
            BookmarkedFolders = new EditBookmarksViewModel(Local.Strings.STR_BOOKMARKS_OPTION_CAPTION);
            BookmarkedFolders.DirtyFlagChangedEvent += BookmarkedFolders_DirtyFlagChangedEvent;
            BookmarkedFolders.ApplyOptionsFromModel(optionsEngine);
        }

        /// <summary>
        /// Save changed settings back to model for further
        /// application and persistence in file system.
        /// </summary>
        /// <param name="settingData"></param>
        public override void SaveOptionsToModel(IEngine optionsEngine)
        {
            var group = optionsEngine.GetOptionGroup("Options");

            SaveOptionsToModel(group);

            BookmarkedFolders.SaveOptionsToModel(optionsEngine);
        }

        /// <summary>
		/// Reset the view model to those options that are going to be presented for editing.
		/// </summary>
		/// <param name="settingData"></param>
		private void LoadOptionsFromModel(IOptionGroup optGroup)
        {
            SecureString securePassword = optGroup.GetValue<SecureString>("MSTranslate.TranslationServicePassword");
            SecureString secureUser = optGroup.GetValue<SecureString>("MSTranslate.TranslationServiceUser");

            mReloadFilesOnAppStart = optGroup.GetValue<bool>("ReloadOpenFilesFromLastSession");
            mMSTranslateService = new TranslationService
            (
                optGroup.GetValue<string>("MSTranslate.TranslationServiceUri"),
                secureUser, securePassword
            );

            var settings = GetService<ISettingsManager>();

            // Initialize localization settings
            Languages = new List<LanguageCollection>(settings.GetSupportedLanguages());

            // Set default language to make sure app neutral is selected and available for sure
            // (this is a fallback if all else fails)
            try
            {
                var langOpt = optGroup.GetValue<string>("LanguageSelected");
                LanguageSelected = Languages.FirstOrDefault(lang => lang.BCP47 == langOpt);

                if (LanguageSelected == null)
                    LanguageSelected = Languages[0];
            }
            catch
            {
            }

            var service = GetService<ITranslator>();

            DefaultSourceLanguageSelected =
                GetLanguageOptionFromModel(service, optGroup, "DefaultSourceLanguage", "DefaultSourceLanguage", 0);

            DefaultTargetLanguageSelected =
                GetLanguageOptionFromModel(service, optGroup, "DefaultTargetLanguage", "DefaultTargetLanguage", 1);

            // get copy of actual value and min, max value definitions from model into viewmodel
            IconSize = optGroup.GetValue<int>("DefaultIconSize");
            IconSizeMax = settings.IconSizeMax;
            IconSizeMin = settings.IconSizeMin;

            ColorGridOption = optGroup.GetValue<bool>("ColorGridOption");
            AlternatingColor = optGroup.GetValue<Color>("AlternatingGridColor");
        }

        /// <summary>
        /// Save changed settings back to model for further
        /// application and persistence in file system.
        /// </summary>
        /// <param name="settingData"></param>
        private void SaveOptionsToModel(IOptionGroup optionsOptionGroup)
        {
            var loginData = MSTranslateGetData();

            optionsOptionGroup.SetValue("MSTranslate.TranslationServiceUri", loginData.TranslationServiceUri);
            optionsOptionGroup.SetValue("MSTranslate.TranslationServiceUser", loginData.TranslationServiceUser);
            optionsOptionGroup.SetValue("MSTranslate.TranslationServicePassword", loginData.TranslationServicePassword);

            optionsOptionGroup.SetValue("ReloadOpenFilesFromLastSession", ReloadFilesOnAppStart);
            optionsOptionGroup.SetValue("LanguageSelected", LanguageSelected.BCP47);

            optionsOptionGroup.SetValue("DefaultSourceLanguage", DefaultSourceLanguageSelected.Bcp47_LangCode);
            optionsOptionGroup.SetValue("DefaultTargetLanguage", DefaultTargetLanguageSelected.Bcp47_LangCode);

            optionsOptionGroup.SetValue("DefaultIconSize", (int)IconSize);

            optionsOptionGroup.SetValue("ColorGridOption", (bool)ColorGridOption);
            optionsOptionGroup.SetValue("AlternatingGridColor", AlternatingColor);
        }

        /// <summary>
        /// Attempts to retrieve a language option from the <seealso cref="Settingsmodel"/> and
        /// falls back to alternive default options if retrieval of option value fails.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="OptGroup"></param>
        /// <param name="languageOptionName">The name of the option in the SettingsModel eg.: "SourceLanguage"</param>
        /// <param name="defaultLanguageOptionName">The name of the default option in the SettingsModel eg.: "DefaultSourceLanguage"</param>
        /// <param name="defaultLanguageListIndex">Index of default language in list of languages if everything else fails.</param>
        /// <returns></returns>
        private ILanguageCode GetLanguageOptionFromModel(ITranslator service,
                                                       IOptionGroup OptGroup,
                                                       string languageOptionName,
                                                       string defaultLanguageOptionName,
                                                       int defaultLanguageListIndex)
        {
            ILanguageCode ret = null;
            ret = service.LanguageList.SingleOrDefault(
                item => item.Bcp47_LangCode == OptGroup.GetValue<string>(languageOptionName));

            // Select defaults if there was no data to be had
            if (ret == null)
                ret = service.LanguageList.SingleOrDefault(
                    item => item.Bcp47_LangCode == OptGroup.GetValue<string>(defaultLanguageOptionName));

            // Attach to first entry in language list if default source language is not set
            if (ret == null)
                ret = service.LanguageList[defaultLanguageListIndex];

            return ret;
        }

        /// <summary>
        /// Gets a copy of the internal uri/login data about Microsoft's translation service
        /// </summary>
        /// <returns></returns>
        private TranslationService MSTranslateGetData()
        {
            return new TranslationService(mMSTranslateService);
        }

        /// <summary>
        /// Method executes when the dirty state of BookmarkedFolders object has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BookmarkedFolders_DirtyFlagChangedEvent(object sender, EditPageDocuments.Base.Events.DocumentDirtyChangedEventArgs e)
        {
            if (e.IsDirtyNewValue == true)
                IsDirty = true;
        }

        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        void System.IDisposable.Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (mDisposed == false)
            {
                if (disposing == true)
                {
                    if (BookmarkedFolders != null)
                    {
                        BookmarkedFolders.DirtyFlagChangedEvent -= BookmarkedFolders_DirtyFlagChangedEvent;
                        System.Threading.Interlocked.Exchange(ref mBookmarkedFolders, null);
                    }
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.Dispose(disposing);
        }

        private void ConstructDefaults()
        {
            mAlternatingColor = Color.FromScRgb(0x10, 0x00, 0x00, 0x00);
        }
        #endregion methods
    }
}
