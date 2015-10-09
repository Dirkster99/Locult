namespace LocultApp.ViewModels.Pages.SettingsPages
{
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    /// <summary>
    /// Class contains all methods necessary to initialize the applications settings model.
    /// </summary>
    public static class SettingDefaults
    {
        /// <summary>
        /// Create the minimal settings model that should be used for every locult application.
        /// This model does not include advanced features like theming etc...
        /// </summary>
        /// <param name="settings"></param>
        static public void CreateGeneralSettings(ISettingsManager settings)
        {
            var optsEngine = settings.Options;

            var OptionsGroup = "Options";

            optsEngine.AddOption(OptionsGroup, "ReloadOpenFilesFromLastSession", typeof(bool), false, true);
            optsEngine.AddOption(OptionsGroup, "SourceFilePath", typeof(string), false, @"C:\temp\source\");
            optsEngine.AddOption(OptionsGroup, "LanguageSelected", typeof(string), false, "de-DE");
            optsEngine.AddOption(OptionsGroup, "IsDirty", typeof(bool), false, true);
            optsEngine.AddOption(OptionsGroup, "DefaultSourceLanguage", typeof(string), false, "en");
            optsEngine.AddOption(OptionsGroup, "DefaultTargetLanguage", typeof(string), false, "de");
            optsEngine.AddOption(OptionsGroup, "DefaultIconSize", typeof(int), false, 16);
            ////optsEngine.AddOption(OptionsGroup, "DefaultFontSize", typeof(int), false, 16);

            optsEngine.AddOption(OptionsGroup, "MSTranslate.TranslationServiceUri", typeof(string), false, "https://api.datamarket.azure.com/Bing/MicrosoftTranslator/");
            optsEngine.AddOption(OptionsGroup, "MSTranslate.TranslationServiceUser", typeof(SecureString), false, new SecureString());
            optsEngine.AddOption(OptionsGroup, "MSTranslate.TranslationServicePassword", typeof(SecureString), false, new SecureString());

            // Create a list of folder bookmarks for easy re-discovery
            var schema = optsEngine.AddListOption<string>(OptionsGroup, "BookmarkedFolders", typeof(string), false, new List<string>());
            schema.List_AddValue(@"C:\TEMP", @"C:\TEMP");
            schema.List_AddValue(@"C:\Windows", @"C:\Windows");

            optsEngine.AddOption(OptionsGroup, "ColorGridOption", typeof(bool), false, true);

            // Light Blue: 370061D1
            optsEngine.AddOption(OptionsGroup, "AlternatingGridColor", typeof(Color), false, Color.FromScRgb(0x37, 0x00, 0x61, 0xD1));
        }
    }
}
