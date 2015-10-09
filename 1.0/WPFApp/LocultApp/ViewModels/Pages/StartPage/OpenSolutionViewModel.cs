namespace LocultApp.ViewModels.Pages.StartPage
{
    using ExplorerLib;
    using LocultApp;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Events;
    using LocultApp.ViewModels.Pages.EditPageDocuments;
    using System;
    using System.Threading;
    using System.Windows.Input;

    /// <summary>
    /// Manages a viewmodel for a view that can open a translation solution.
    /// The result is an event that, when subscriped, can be used to do the
    /// actual work of opening and reading data from persistence.
    /// </summary>
    public class OpenSolutionViewModel : StartPageSolutionViewModel, IDisposable
    {
        #region fields
        private string mDefaultDocumentLocation = @"C:\TEMP\";
        private ApplicationRequestEventHandler RequestedAction;
        
        private string mSolutionLocation;
        private RelayCommand<object> mOpenSolutionCommand;
        private RelayCommand<object> mBrowseForSolutionCommand;
        private bool mDisposed = false;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="defaultNamePath"></param>
        /// <param name="requestedAction"></param>
        public OpenSolutionViewModel(string defaultNamePath = null,
                                    ApplicationRequestEventHandler requestedAction = null)
        {
            if (defaultNamePath != null)
                mDefaultDocumentLocation = System.IO.Path.GetDirectoryName(defaultNamePath);

            mSolutionLocation = defaultNamePath;

            RequestedAction = requestedAction;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Get/set file path and name of new solution (complete path)
        /// </summary>
        public string SolutionLocation
        {
            get
            {
                return mSolutionLocation;
            }

            set
            {
                if (mSolutionLocation != value)
                {
                    mSolutionLocation = value;
                    RaisePropertyChanged(() => SolutionLocation);
                }
            }
        }

        /// <summary>
        /// Gets a command that will fire an event to indicate that a
        /// translation solution should opened.
        /// </summary>
        public ICommand OpenSolutionCommand
        {
            get
            {
                if (mOpenSolutionCommand == null)
                {
                    mOpenSolutionCommand = new RelayCommand<object>((p) =>
                    {
                        OpenSolutionCommandExecuted(SolutionLocation);
                    },
                    (p) =>
                    {
                        // At least a solution name, location, and source file are required
                        if (string.IsNullOrEmpty(SolutionLocation) == true)
                            return false;

                        // Check if characters are printable
                        // if string appear to have content (see previous check)
                        if (SolutionLocation.Trim().Length == 0)
                            return false;

                        // Enable command since basic input seems to be available
                        return true;
                    });
                }

                return mOpenSolutionCommand;
            }
        }
  
        /// <summary>
        /// Gets a command to show a file open dialog that lets
        /// a user select a solution file from persistence.
        /// </summary>
        public ICommand BrowseForSolutionCommand
        {
            get
            {
                if (mBrowseForSolutionCommand == null)
                {
                    mBrowseForSolutionCommand = new RelayCommand<object>((p) =>
                    {
                        var explorer = ServiceLocator.ServiceContainer.Instance.GetService<IExplorer>();

                        string nextFilePath = explorer.FileOpen(EditTranslationsDocumentViewModel.StringSolutionFileFilter,
                                                                SolutionLocation, AppCore.MyDocumentsUserDir);

                        if (string.IsNullOrEmpty(nextFilePath) == false)
                            SolutionLocation = nextFilePath;
                    });
                }

                return mBrowseForSolutionCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed == false)
            {
                if (disposing == true)
                {
                    // NUll 'em out boys
                    Interlocked.Exchange(ref RequestedAction, null);
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            //// base.Dispose(disposing);
        }

        // Disposable types implement a finalizer.
        ~OpenSolutionViewModel()
        {
            Dispose(false);
        }

        /// <summary>
        /// Checks if a given file path and name reference to a solution file exists
        /// and fire the corresponding event if it does indeed exist.
        /// </summary>
        /// <param name="solutionLocation"></param>
        private void OpenSolutionCommandExecuted(string solutionLocation)
        {
            try
            {
                RequestApplicationEvent(new ApplicationRequestEvent(ApplicationRequest.InitProcessing));

                // switch view and reload saved solution
                RequestApplicationEvent(new ApplicationRequestEvent(ApplicationRequest.OpenSolution, solutionLocation));
            }
            catch (System.Exception exp)
            {
                RequestApplicationEvent(new ApplicationRequestEvent(ApplicationRequest.DisplayException, exp));
            }
        }

        private bool RequestApplicationEvent(ApplicationRequestEvent e)
        {
            if (RequestedAction != null)
            {
                RequestedAction(this, e);
                return true;
            }

            return false;
        }
        #endregion methods
    }
}
