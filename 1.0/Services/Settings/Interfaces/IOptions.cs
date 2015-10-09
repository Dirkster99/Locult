namespace Settings.Interfaces
{
    using Settings.ProgramSettings;
    using Settings.Translate;
    using System.Collections.Generic;

    public interface IOptions
    {
        #region properties
        bool IsDirty { get; set; }
        string LanguageSelected { get; set; }
        bool ReloadOpenFilesOnAppStart { get; set; }
        string SourceFilePath { get; set; }

        TranslationService MSTranslate { get; set; }

        string DefaultSourceLanguage { get; set; }
        string DefaultTargetLanguage { get; set; }

        string DefaultDefaultSourceLanguage { get; }
        string DefaultDefaultTargetLanguage { get; }

        int DefaultIconSize { get; }
        int IconSizeMin { get; }
        int IconSizeMax { get; }

        int DefaultFontSize { get; }
        int FontSizeMin { get; }
        int FontSizeMax { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Reset the dirty flag (e.g. after saving program options when they where edit).
        /// </summary>
        /// <param name="flag"></param>
        void SetDirtyFlag(bool flag);

        void SetIconSize(int size);
        void SetFontSize(int size);

        /// <summary>
        /// Sets a copy of the internal login data on Microsoft's translation
        /// in the options object.
        /// </summary>
        /// <param name="service"></param>
        TranslationService MSTranslateGetData();

        /// <summary>
        /// Gets a copy of the internal login data on Microsoft's translation
        /// from the options object.
        /// </summary>
        /// <returns></returns>
        void MSTranslateSetData(TranslationService service);
        #endregion methods
    }
}
