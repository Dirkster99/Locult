namespace Settings
{
    using Settings.Interfaces;

    /// <summary>
    /// Factory creates new objects from default constructors.
    /// </summary>
    static public class Factory
    {
        /// <summary>
        /// Get a new settingsmanager object.
        /// </summary>
        /// <returns></returns>
        static public ISettingsManager CreateSettingsManager()
        {
            return new SettingsManager();
        }
    }
}
