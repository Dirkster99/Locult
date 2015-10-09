namespace Settings.Interfaces
{
    using SettingsModel.Interfaces;

    /// <summary>
    /// Provides an interface to an options (settings) model that can be used
    /// by the application to store and retireve a user's application settings.
    /// </summary>
    public interface IOptionsPanel
    {
        /// <summary>
        /// Gets a <seealso cref="SettingsModel"/> that models the application's settings.
        /// </summary>
        IEngine Options { get; }
    }
}