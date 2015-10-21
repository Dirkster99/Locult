namespace LocultApp.ViewModels.Interfaces
{
    using LocultApp.ViewModels.Pages;
    using Settings.Interfaces;

    /// <summary>
    /// Objects that implement this interface can be asked to save their settings
    /// into the <seealso cref="ISettingsManager"/> component for later storage
    /// and/or retrieval.
    /// </summary>
    interface ISaveSettings
    {
        /// <summary>
        /// Have a look at the object and determine whether there are any data items that require persistence.
        /// </summary>
        /// <param name="settings"></param>
        void SavePageSettings(ISettingsManager settings);
    }
}
