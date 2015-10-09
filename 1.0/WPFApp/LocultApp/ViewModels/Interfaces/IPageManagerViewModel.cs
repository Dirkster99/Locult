namespace LocultApp.ViewModels.Interfaces
{
    using AppResourcesLib;
    using LocultApp.Controls.Exception;
    using LocultApp.ViewModels.Pages;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public interface IPageManagerViewModel : ICanDisplayException,
                                             IDocumentCollection,
                                             System.IDisposable
    {
        #region properties
        /// <summary>
        /// Gets a property that determines the state dependent page
        /// that should currently be shown in the main view.
        /// </summary>
        PageBaseViewModel CurrentPage { get; }

        /// <summary>
        /// Gets a command to save the curently active solution (if any).
        /// </summary>
        ICommand SaveSolutionCommand { get; }

        /// <summary>
        /// Gets a command to save the curently active solution (if any).
        /// </summary>
        ICommand ShowStartPageCommand { get; }

        /// <summary>
        /// Gets a command that toggles the Settings Page View.
        /// </summary>
        ICommand ShowSettingsCommand { get; }

        ICommand CancelSettingsCommand { get; }

        /// <summary>
        /// Gets a Boolean property that determines whether the settings page is currently visible or not.
        /// </summary>
        bool IsSettingsPageVisible { get; }

        /// <summary>
        /// Gets whether the viewmodel is currently processing data or not.
        /// </summary>
        bool IsProcessing { get; }

        IExceptionViewModel ViewException { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Construct a startpage with New and Load Solution options
        /// to get us started ...
        /// </summary>
        bool GetStarted();

        /// <summary>
        /// Attempts to load a solution and initialize user interface with defaults.
        /// </summary>
        /// <param name="fileLocation"></param>
        Task LoadSolutionAndShowInCurrentPage(string fileLocation);

        /// <summary>
        /// Checks the state of each solution item and changes
        /// properties to signal state changes.
        /// </summary>
        Task CheckSolutionState();

        /// <summary>
        /// Add a seetings page viewmodel for display of seetingspages inside the main settings page.
        /// </summary>
        /// <param name="tModel"></param>
        void AddSettingsPageViewModel(IPageTemplateModel tModel);
        #endregion methods
    }
}
