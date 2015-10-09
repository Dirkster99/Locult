namespace LocultApp.ViewModels.Pages
{
    using LocultApp.ViewModels.Events;
    using LocultApp.ViewModels.Pages.StartPage;
    using System;
    using System.Threading;

    public class StartPageViewModel : PageBaseViewModel, LocultApp.ViewModels.Pages.StartPage.IRequestAction, IDisposable
    {
        #region fields
        private const int NewSolutionViewModelIdx = 0;
        private const int OpenSolutionViewModelIdx = 1;

        private StartPage.StartPageSolutionViewModel[] mSolutionOperations = null;

        private int mSelectedSolutionIndex = 0;
        bool mDisposed = false;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="AppRequestHandler">
        /// Is used by this object to indicate that the current page (this object being the current page)
        /// can change and the currentpage main view show a different page better matching the requested action.
        /// </param>
        public StartPageViewModel(ApplicationRequestEventHandler AppRequestHandler, string lastActiveSolution)
        {
            mSolutionOperations = new StartPage.StartPageSolutionViewModel[2];

            // This handler is executed when either of the below viewmodel requests an action
            // Thérequest goes back to the object that called this class constructor
            RequestedAction += AppRequestHandler;

            // Construct a viewmodel for creating a new solution
            mSolutionOperations[NewSolutionViewModelIdx] = new StartPage.NewSolutionViewModel(AppLifeCycleViewModel.MyDocumentsUserDir,
                                                                             RequestedAction);

            SolutionOperations[OpenSolutionViewModelIdx] = new StartPage.OpenSolutionViewModel(lastActiveSolution, RequestedAction);
        }
        #endregion constructors

        #region events
        public event ApplicationRequestEventHandler RequestedAction;
        #endregion events

        #region properties
        /// <summary>
        /// Gets the index of the currently selected page viewmodel.
        /// </summary>
        public int SelectedSolutionIndex
        {
            get
            {
                return mSelectedSolutionIndex;
            }
            
            set
            {
                if (mSelectedSolutionIndex != value)
                {
                    mSelectedSolutionIndex = value;
                    RaisePropertyChanged(() => SelectedSolutionIndex);
                }
            }
        }

        /// <summary>
        /// Gets the collection of page viewmodels.
        /// </summary>
        public StartPage.StartPageSolutionViewModel[] SolutionOperations
        {
            get
            {
                return mSolutionOperations;
            }
            
            protected set
            {
                if (mSolutionOperations != value)
                {
                    mSolutionOperations = value;
                    RaisePropertyChanged(() => SolutionOperations);
                }
            }
        }

        public string PageDisplayName
        {
            get
            {
                return Local.Strings.STR_START_PAGE_TITLE;
            }
        }

        public string PageDisplayName_Tip
        {
            get
            {
                return Local.Strings.STR_START_PAGE_TITLE_TT;
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
                    if (mSolutionOperations != null)
                    {
                        for (int i=0;i<mSolutionOperations.Length; i++)
                        {
                            var idisposable = mSolutionOperations[i] as IDisposable;

                            if (idisposable != null)
                                idisposable.Dispose();

                            Interlocked.Exchange(ref mSolutionOperations[i], null);
                        }

                        Interlocked.Exchange(ref mSolutionOperations, null);
                    }

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
        ~StartPageViewModel()
        {
            Dispose(false);
        }

        public void GetStarted(string lastActiveSolution)
        {
            try
            {
                // Prefer Open Solution Tab over New Solution Tab since users are more likely to re-load rather than create new again
                if (string.IsNullOrEmpty(lastActiveSolution) == false)
                {
                    if (System.IO.File.Exists(lastActiveSolution) == true)
                    {
                        SelectedSolutionIndex = OpenSolutionViewModelIdx;
                    }
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        #endregion methods
    }
}
