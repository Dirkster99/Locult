namespace Settings.ProgramSettings
{
    using Settings.Interfaces;
    using SettingsModel.Interfaces;
    using SettingsModel.Models;

    internal class OptionsPanel : IOptionsPanel
    {
        private IEngine mQuery = null;

        /// <summary>
        /// Class constructor.
        /// </summary>
        public OptionsPanel()
        {
            mQuery = Factory.CreateEngine();
        }

        /// <summary>
        /// Get/sets the options engine of this class.
        /// </summary>
        public IEngine Options
        {
            get
            {
                return mQuery;
            }

            set
            {
                mQuery = value;
            }
        }
    }
}
