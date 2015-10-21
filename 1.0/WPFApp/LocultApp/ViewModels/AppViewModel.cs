namespace LocultApp.ViewModels
{
    using LocultApp.Controls.Solution;
    using LocultApp.ViewModels.Interfaces;
    using MsgBox;
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TranslationSolutionViewModelLib.ViewModels;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// Class holds main entry points (methods and properties) relevant to this application.
    /// This class is likely to be bound to the datacontext of the MainWindow or Shell object.
    /// </summary>
    public class AppViewModel : Base.ViewModelBase, IDisposable, IDocumentCollection, IProcessItems
    {
        #region fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AppLifeCycleViewModel mAppLifeCycle = null;
        private IPageManagerViewModel mPageManager = null;
        private bool mDisposed = false;

        // Task processing setup enables more than 1 global process but each task has to reset its own flag
        // Otherwise, Global UI functions are not enabled ...
        private bool mIsProcessingSolution = false;       // Interaction logic for disabling/enabling UI
        private string mCurrentProcessId;     // When processing global items

        private object lockObject = new object();
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constrcutor from parameters.
        /// </summary>
        /// <param name="lifeCylceModel"></param>
        public AppViewModel(AppLifeCycleViewModel lifeCylceModel)
            : this()
        {
            mAppLifeCycle = lifeCylceModel;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            mCurrentProcessId = string.Empty;
            mAppLifeCycle = new AppLifeCycleViewModel();
            mPageManager = new Pages.PageManagerViewModel(this as IProcessItems);
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a bool value that indicates whether the solution and attached documents
        /// are currently being processed (cannot be edit) or not.
        /// </summary>
        public bool IsProcessingSolution
        {
            get
            {
                lock (lockObject)
                {
                    return mIsProcessingSolution;
                }
            }

            private set
            {
                if (mIsProcessingSolution != value)
                {
                    mIsProcessingSolution = value;
                    RaisePropertyChanged(() => IsProcessingSolution);
                }
            }
        }

        public string CurrentProcessId
        {
            get
            {
                return this.mCurrentProcessId;
            }
        }

        /// <summary>
        /// Gets an object that contains all properties and methods that are relevant to
        /// the standard life cycle of an application.
        /// </summary>
        public AppLifeCycleViewModel AppLifeCycle
        {
            get
            {
                return mAppLifeCycle;
            }
        }

        /// <summary>
        /// Gets the current PageManager object that holds all properties
        /// and methods to switch and display the current page in main view.
        /// </summary>
        public IPageManagerViewModel PageManager
        {
            get
            {
                return mPageManager;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Initialize the application by reloading data or showing Welcome! view.
        /// </summary>
        /// <param name="reloadLastSolutionOnStartup"></param>
        /// <param name="solutionFilename"></param>
        public bool InitAppViewModel(bool reloadLastSolutionOnStartup = false,
                                     string solutionFilename = null)
        {
            if (reloadLastSolutionOnStartup == true && string.IsNullOrEmpty(solutionFilename) == false)
            {
                Task.Factory.StartNew(async () =>
                {
                    await mPageManager.LoadSolutionAndShowInCurrentPage(solutionFilename);
                });

                return true;
            }
            else
                return mPageManager.GetStarted();
        }

        /// <summary>
        /// Gets the currently displayed document collection from the Process Manager to determine
        /// whether unsaved data should be saved or not (when exiting application etc).
        /// </summary>
        /// <returns></returns>
        public IList<IDirtyDocument> GetDirtyDocuments()
        {
            if (mPageManager == null)
                return null;

            return mPageManager.GetDirtyDocuments();
        }

        /// <summary>
        /// Register a new process as running on a global scale.
        /// </summary>
        /// <param name="newIsProcessingValue"></param>
        /// <param name="processID"></param>
        /// <returns></returns>
        bool IProcessItems.SetIsProcessingSolution(bool newIsProcessingValue, string processID)
        {
            lock (lockObject)
            {
                if (newIsProcessingValue == true)
                {
                    if (IsProcessingSolution == false && this.mCurrentProcessId == string.Empty)
                    {
                        IsProcessingSolution = true;
                        this.mCurrentProcessId = processID;

                        return true;
                    }
                    else // we are already processing something on a global level...
                        return false;
                }
                else // value is false -> Reset this if processId can be matched with previous request
                {
                    if (IsProcessingSolution == true && this.mCurrentProcessId == processID)
                    {
                        if (this.mCurrentProcessId.Contains(processID) == true)
                        {
                            Task.Factory.StartNew(async () =>
                            {
                                await mPageManager.CheckSolutionState();
                            });

                            mCurrentProcessId = string.Empty;
                            IsProcessingSolution = false;

                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
        }

        /// <summary>
        /// Ends a currenlty running global process state and displays the suggested error items.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="Message"></param>
        /// <param name="Caption"></param>
        void IProcessItems.EndProcessingSolutionWithError(string processID, Exception exp, string Message)
        {
            if (exp != null)
                mPageManager.ViewException.SetExceptionForDisplay(exp);
            else
                GetService<IMessageBoxService>().Show(Message, Local.Strings.STR_UNEXPECTED_ERROR, MsgBoxButtons.OK);
        }

        /// <summary>
        /// Converts the given parameters into a solution model and returns it.
        /// </summary>
        /// <param name="solutionPathName"></param>
        /// <param name="newSolutionName"></param>
        /// <param name="TargetFilesCollection"></param>
        /// <returns></returns>
        public static TranslatorSolution ConvertToSolutionModel(string solutionPathName,
                                                                string newSolutionName,
                                                                string newSourceFilePathName,
                                                                string newCommment,
                                                                IList<FileReferenceViewModel> TargetFilesCollection)
        {
            // Build solution model
            var solution = new TranslatorSolution(newSolutionName);

            var project = new Project();
            project.SetSourceFile(new FileReference(newSourceFilePathName,
                                                    UpdateNewSolutionTargetFilesViewModel.mDefaultType,
                                                    newCommment));

            foreach (var item in TargetFilesCollection)
                project.AddTargetFile(item.GetModel());

            solution.AddProject(project);

            return solution;
        }

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
                    // Dispose of the curently displayed content
                    mPageManager.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion methods
    }
}
