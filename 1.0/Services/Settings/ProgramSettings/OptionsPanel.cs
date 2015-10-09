namespace Settings.ProgramSettings
{
    using Settings.Interfaces;
    using SettingsModel.Interfaces;
    using SettingsModel.Models;

    internal class OptionsPanel : IOptionsPanel
    {
        private IEngine mQuery = null;

        public OptionsPanel()
        {
            mQuery = Factory.CreateEngine();
        }

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
