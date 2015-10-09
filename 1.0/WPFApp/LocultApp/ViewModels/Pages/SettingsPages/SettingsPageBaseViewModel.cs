namespace LocultApp.ViewModels.Pages.SettingsPages
{
    using AppResourcesLib;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events;
    using SettingsModel.Interfaces;
    using System.Windows;

    public abstract class SettingsPageBaseViewModel : DocumentDirtyChangedViewModelBase
    {
        public SettingsPageBaseViewModel(string displayTitle)
            : this()
        {
            DisplayTitle = displayTitle;
        }

        protected SettingsPageBaseViewModel()
            : base()
        {

        }

        public string DisplayTitle { get; private set; }

        /// <summary>
        /// Reset the view model to those options that are going to be presented for editing.
        /// </summary>
        /// <param name="settingData"></param>
        public abstract void ApplyOptionsFromModel(IEngine optionsOptionGroup);

        /// <summary>
        /// Save changed settings back to model for further
        /// application and persistence in file system.
        /// </summary>
        /// <param name="settingData"></param>
        public abstract void SaveOptionsToModel(IEngine optionsOptionGroup);
    }
}
