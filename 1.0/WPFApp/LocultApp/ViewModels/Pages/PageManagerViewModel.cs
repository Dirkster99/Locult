namespace LocultApp.ViewModels.Pages
{
    using AppResourcesLib;
    using LocultApp.Controls.Exception;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Pages.Interfaces;
    using MsgBox;
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using TranslationSolutionViewModelLib.SolutionModelVisitors;
    using TranslationSolutionViewModelLib.ViewModels;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// Class manages the main view and switches it whenever requested by other
    /// viewmodels or when a corresponding state requires that.
    /// </summary>
    public class PageManagerViewModel : Base.ViewModelBase,
                                        LocultApp.ViewModels.Interfaces.IPageManagerViewModel
    {
        #region fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PageBaseViewModel mCurrentPage = null;
        private bool mDisposed = false;

        private ICommand mSaveSolutionCommand;
        private ICommand mShowStartPageCommand;

        private ICommand mShowSettingsCommand;
        private ICommand mCancelSettingsCommand;
        private bool mIsSettingsPageVisible = false;
        private PageBaseViewModel mCurrentBackPage = null;

        private IProcessItems mProcessItems;
        private readonly string mLoadSolutionProcessID = "LoadSolution {29718EF1-D6D8-4A68-9339-06C963440D12}";
        private readonly string mSaveSolutionProcessID = "SaveSolution {29718EF1-D6D8-4A68-9339-06C963440D12}";
        private bool mIsProcessing;
        private IExceptionViewModel mViewException;

        private List<IPageTemplateModel> mSettingsPageModels = new List<IPageTemplateModel>();
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        protected PageManagerViewModel()
        {
            mIsProcessing = false;
        }

        /// <summary>
        /// Class constructor from paramters
        /// </summary>
        /// <param name="processItems"></param>
        public PageManagerViewModel(IProcessItems processItems)
            : this()
        {
            mProcessItems = processItems;
            ViewException = new ExceptionViewModel();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a property that determines the state dependent page
        /// that should currently be shown in the main view.
        /// </summary>
        public PageBaseViewModel CurrentPage
        {
            get
            {
                return mCurrentPage;
            }

            private set
            {
                if (mCurrentPage != value)
                {
                    mCurrentPage = value;
                    RaisePropertyChanged(() => CurrentPage);
                }
            }
        }

        /// <summary>
        /// Gets a command to save the curently active solution (if any).
        /// </summary>
        public ICommand SaveSolutionCommand
        {
            get
            {
                if (mSaveSolutionCommand == null)
                {
                    mSaveSolutionCommand = new RelayCommand<object>((p) =>
                    {
                        var editPage = CurrentPage as EditPageViewModel;
                        if (editPage == null)
                            return;

                        var model = editPage.Solution.Root.GetModel();

                        if (model != null)
                        {
                            this.SaveSolution(editPage.Solution.Root.ItemPathName, model,
                                              editPage.Solution);
                        }
                    },
                    (p) =>
                    {
                        // Saving a solution requires an EditPageViewModel
                        if (CurrentPage is EditPageViewModel &&
                            mProcessItems.IsProcessingSolution == false)
                            return true;

                        return false;
                    });
                }

                return mSaveSolutionCommand;
            }
        }

        /// <summary>
        /// Gets a command to save the curently active solution (if any).
        /// </summary>
        public ICommand ShowStartPageCommand
        {
            get
            {
                if (mShowStartPageCommand == null)
                {
                    mShowStartPageCommand = new RelayCommand<object>((p) =>
                    {
                        var docColl = GetDirtyDocuments();

                        // Cancel this processing if there are dirty documents and user wants to save them first...
                        if (GetOKToLeavePageWithoutSave(docColl) == false)
                            return;

                        GetStarted();
                    }, (p) =>
                    {
                        // Enabling view for Start Page when StartPage is visible make not much sense
                        if (CurrentPage is StartPageViewModel)
                            return false;

                        // Disable this command if something is processed on a global level
                        if (mProcessItems.IsProcessingSolution == false)
                            return true;

                        return false;
                    });
                }

                return mShowStartPageCommand;
            }
        }

        /// <summary>
        /// Gets a command that toggles the Settings Page View.
        /// </summary>
        public ICommand ShowSettingsCommand
        {
            get
            {
                if (mShowSettingsCommand == null)
                {
                    mShowSettingsCommand = new RelayCommand<object>((p) =>
                    {
                        var settingsService = GetService<ISettingsManager>();
                        SwitchSettingsPageView(!IsSettingsPageVisible, settingsService);
                    },
                    (p) =>
                    {
                        return true;
                    });
                }

                return mShowSettingsCommand;
            }
        }

        /// <summary>
        /// Gets a command that Cancels editing settings when settings are currently visible.
        /// </summary>
        public ICommand CancelSettingsCommand
        {
            get
            {
                if (mCancelSettingsCommand == null)
                {
                    mCancelSettingsCommand = new RelayCommand<object>((p) =>
                    {
                        var settingsVM = CurrentPage as SettingsPageViewModel;

                        if (IsSettingsPageVisible == true && settingsVM != null)
                        {
                            if (settingsVM.IsDirty == true)
                            {
                                var msg = GetService<IMessageBoxService>();
                                if (msg.Show(Local.Strings.STR_LEAVE_SETTINGS_WITHOUT_SAVING_CHANGES,
                                             Local.Strings.STR_LEAVE_SETTINGS_WITHOUT_SAVING_CHANGES_TT,
                                             MsgBoxButtons.YesNoCancel, MsgBoxResult.No) == MsgBoxResult.Yes)
                                {
                                    SwitchSettingsPageView(false);
                                    return;
                                }
                                else
                                    return;
                            }
                        }

                        SwitchSettingsPageView(false);
                    },
                    (p) =>
                    {
                        return true;
                    });
                }

                return mCancelSettingsCommand;
            }
        }

        /// <summary>
        /// Gets a Boolean property that determines whether the settings page is currently visible or not.
        /// </summary>
        public bool IsSettingsPageVisible
        {
            get
            {
                return mIsSettingsPageVisible;
            }

            private set
            {
                if (mIsSettingsPageVisible != value)
                {
                    mIsSettingsPageVisible = value;
                    this.RaisePropertyChanged(() => IsSettingsPageVisible);
                }
            }
        }

        /// <summary>
        /// Gets whether the viewmodel is currently processing data or not.
        /// </summary>
        public bool IsProcessing
        {
            get
            {
                return mIsProcessing;
            }

            private set
            {
                if (mIsProcessing != value)
                {
                    mIsProcessing = value;
                    RaisePropertyChanged(() => IsProcessing);
                }
            }
        }

        public IExceptionViewModel ViewException
        {
            get
            {
                return mViewException;
            }

            private set
            {
                if (mViewException != value)
                {
                    mViewException = value;
                    RaisePropertyChanged(() => ViewException);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method is executed to check whether there are any dirty
        /// documents that would need saving to persistence...
        /// </summary>
        /// <param name="docColl"></param>
        /// <returns></returns>
        public static bool GetOKToLeavePageWithoutSave(IList<IDirtyDocument> docColl)
        {
            if (docColl == null)
                return true;

            if (docColl.Count > 0) // Are there any dirty documents?
            {
                // Yes, collect them in a string and ask user whether its OK to proceed without saving
                string files = string.Empty;
                for (int i = 0; i < docColl.Count; i++)
                {
                    files += docColl[i].FilePathName + '\n';
                }

                var msg = ServiceLocator.ServiceContainer.Instance.GetService<IMessageBoxService>();

                // Ask user whether it is OK to exit application without saving changed data.
                var result = msg.Show(LocultApp.Local.Strings.STR_ASK_TO_SAVE_CHANGES + "\n\n" + files, "",
                                      MsgBoxButtons.YesNoCancel);

                if (result == MsgBoxResult.Cancel || result == MsgBoxResult.Yes)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Construct a startpage with New and Load Solution options
        /// to get us started ...
        /// </summary>
        public bool GetStarted()
        {
            try
            {
                // Construct a viewmodel for loading/opening an existing solution
                var settings = GetService<ISettingsManager>();

                var lastActiveFile = settings.SessionData.LastActiveSolution;
                if (string.IsNullOrEmpty(lastActiveFile))
                {
                    lastActiveFile = System.IO.Path.Combine(AppLifeCycleViewModel.MyDocumentsUserDir,
                                                            TranslatorSolution._DefaultName + TranslatorSolution._DefaultSolutionExtension);
                }

                var startPage = new StartPageViewModel(RequestedApplicationAction, lastActiveFile);
                startPage.GetStarted(lastActiveFile);

                // Move settings page out of the way if it is currently visible
                if (CurrentPage is SettingsPageViewModel)
                    SwitchSettingsPageView(false, settings);

                // Construct a start page and listen for requested appliaction actions
                SwitchCurrentPageToNewPage(startPage, true);

                return true;
            }
            catch (Exception exp)
            {
                ViewException.SetExceptionForDisplay(exp);
                return false;
            }
        }

        /// <summary>
        /// Attempts to load a solution and initialize user interface with defaults.
        /// </summary>
        /// <param name="fileLocation"></param>
        public Task LoadSolutionAndShowInCurrentPage(string fileLocation)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    var page = await LoadSolution4EditPage(fileLocation);

                    // Switch view to data loaded into the edit viewmodel
                    if (page != null)
                        SwitchCurrentPageToNewPage(page, true);
                }
                catch (Exception exp)
                {
                    if (CurrentPage == null)
                        GetStarted();

                    ViewException.SetExceptionForDisplay(exp);
                }
            });
        }

        /// <summary>
        /// Gets the currently displayed document collection from the Process Manager to determine
        /// whether unsaved data should be saved or not (when exiting application etc).
        /// </summary>
        /// <returns></returns>
        public IList<IDirtyDocument> GetDirtyDocuments()
        {
            if (CurrentPage == null)
                return null;

            // Documents can currently be edit in this viewmodel
            if (CurrentPage is EditPageViewModel)
                return (CurrentPage as EditPageViewModel).GetDirtyDocuments();

            // return this if edit page collection is hidden by settings page or such...
            if (mCurrentBackPage is EditPageViewModel)
                return (mCurrentBackPage as EditPageViewModel).GetDirtyDocuments();

            return null;
        }

        /// <summary>
        /// Add a seetings page viewmodel for display of seetingspages inside the main settings page.
        /// </summary>
        /// <param name="tModel"></param>
        public void AddSettingsPageViewModel(IPageTemplateModel tModel)
        {
            mSettingsPageModels.Add(tModel);
        }

        /// <summary>
        /// Checks the state of each solution item and changes
        /// properties to signal state changes.
        /// </summary>
        public Task CheckSolutionState()
        {
            return Task.Factory.StartNew(async () =>
            {
                if (CurrentPage is EditPageViewModel)
                {
                    var editVM = CurrentPage as EditPageViewModel;

                    bool result = await editVM.CheckSolutionState();
                }
            });
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
                    var currentPage = mCurrentPage as IDisposable;

                    // Dispose of the curently displayed content
                    if (currentPage != null)
                    {
                        currentPage.Dispose();
                        Interlocked.Exchange(ref mCurrentPage, null);
                    }

                    Interlocked.Exchange(ref mProcessItems, null);
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }

        /// <summary>
        /// Method executes when a <seealso cref="ApplicationRequestEvent"/> is fired.
        /// This can change the current page and update the currentpage main view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RequestedApplicationAction(object sender, Events.ApplicationRequestEvent e)
        {
            try
            {
                this.IsProcessing = true;

                switch (e.Request)
                {
                    case Events.ApplicationRequest.OpenSolution:
                        await Task.Factory.StartNew(async () =>
                        {
                            await LoadSolutionAndShowInCurrentPage(e.FileLocation);
                        });
                        break;

                    case Events.ApplicationRequest.NewSolution:
                        // Save solution to disk
                        var result = await SaveSolutionAsync(e.FileLocation, e.Solution);

                        // Reload new solution from disk
                        var page = await LoadSolution4EditPage(e.FileLocation);

                        // Switch view to data loaded into the edit viewmodel
                        if (page != null)
                            SwitchCurrentPageToNewPage(page, true);
                        break;

                    // Display an exception if we were asked to do that
                    case Events.ApplicationRequest.DisplayException:
                        ViewException.SetExceptionForDisplay(e.Result as Exception);
                        break;

                    case Events.ApplicationRequest.InitProcessing:
                        ViewException.ShowExceptionView(false);
                        break;

                    default:
                        throw new System.NotImplementedException(e.Request.ToString());
                }
            }
            catch (System.Exception exp)
            {
                ViewException.SetExceptionForDisplay(exp);
            }
            finally
            {
                this.IsProcessing = false;
            }
        }

        /// <summary>
        /// Loads a given solution and returns an <seealso cref="EditPageViewModel"/> if all went well.
        /// Otherwise, null is returned.
        /// </summary>
        /// <param name="FileLocation"></param>
        /// <returns></returns>
        private async Task<EditPageViewModel> LoadSolution4EditPage(string FileLocation)
        {
            // Construct the solution model and EditPageViewModel (and view) and load requested data
            var solution = await LoadSolution(FileLocation);

            string path = System.IO.Path.GetDirectoryName(FileLocation);
            string name = System.IO.Path.GetFileName(FileLocation);

            // Switch view to data loaded into the edit viewmodel
            var editVM = new EditPageViewModel(solution, path, name, mProcessItems, RequestedApplicationAction);

            var result = await editVM.CheckSolutionState();
            editVM.PerformDefaultSeleciton();

            return editVM;
        }

        private async Task<bool> SaveSolutionAsync(string fileLocation, TranslatorSolution solution)
        {
            if (mProcessItems.SetIsProcessingSolution(true, mSaveSolutionProcessID) == false)
                throw new Exception(string.Format(LocultApp.Local.Strings.STR_BLOCKED_SOLUTION_TASK,
                                                  mProcessItems.CurrentProcessId));

            try
            {
                var modelExt = new PersistSolutionModelInXML();
                var result = await modelExt.SaveSolutionAsync(solution, fileLocation);

                var settings = GetService<ISettingsManager>();
                settings.SessionData.LastActiveSolution = fileLocation;

                return result;
            }
            finally
            {
                if (mProcessItems.SetIsProcessingSolution(false, mSaveSolutionProcessID) == false)
                    ViewException.SetExceptionForDisplay(new Exception(string.Format(Local.Strings.STR_FAILED_RESET_OF_SOLUTION_TASK, mProcessItems.CurrentProcessId)));
            }
        }

        private async Task<TranslatorSolution> LoadSolution(string fileLocation)
        {
            TranslatorSolution solution = null;

            if (mProcessItems.SetIsProcessingSolution(true, mLoadSolutionProcessID) == false)
                throw new Exception(string.Format(Local.Strings.STR_BLOCKED_SOLUTION_TASK, mProcessItems.CurrentProcessId));

            try
            {
                var solFacade = new PersistSolutionModelInXML();
                solution = await solFacade.LoadSolutionAsync(fileLocation);

                var settings = GetService<ISettingsManager>();
                settings.SessionData.LastActiveSolution = fileLocation;
            }
            finally
            {
                if (solution == null)
                    solution = new TranslatorSolution();

                if (mProcessItems.SetIsProcessingSolution(false, mLoadSolutionProcessID) == false)
                    ViewException.SetExceptionForDisplay(new Exception(string.Format(Local.Strings.STR_FAILED_RESET_OF_SOLUTION_TASK,
                                                                                     mProcessItems.CurrentProcessId)));
            }

            return solution;
        }

        /// <summary>
        /// editPage.Solution.Root.ItemPathName
        /// </summary>
        /// <param name="filePathName"></param>
        /// <returns></returns>
        private async void SaveSolution(string filePathName,
                                        TranslatorSolution solutionModel,
                                        SolutionViewModel solutionViewModel = null)
        {
            try
            {
                // Copy solution and compute relative paths
                var copy = solutionModel.CopySolution(filePathName);

                // save data to persistence
                this.IsProcessing = true;

                if (mProcessItems.SetIsProcessingSolution(true, mSaveSolutionProcessID) == false)
                    throw new Exception(string.Format(Local.Strings.STR_BLOCKED_SOLUTION_TASK, mProcessItems.CurrentProcessId));

                try
                {
                    var modelExt = new PersistSolutionModelInXML();
                    var result = await modelExt.SaveSolutionAsync(copy, filePathName);

                    if (solutionViewModel != null)
                        solutionViewModel.Root.SolutionIsDirty_Reset();
                }
                finally
                {
                    if (mProcessItems.SetIsProcessingSolution(false, mSaveSolutionProcessID) == false)
                        ViewException.SetExceptionForDisplay(new Exception(string.Format(Local.Strings.STR_FAILED_RESET_OF_SOLUTION_TASK, mProcessItems.CurrentProcessId)));
                }
            }
            catch (System.Exception exp)
            {
                ViewException.SetExceptionForDisplay(exp);
            }
            finally
            {
                this.IsProcessing = false;
            }
        }

        /// <summary>
        /// Switches the settings page view on or off depending on the
        /// given request in <paramref name="isSettingsPageVisibleRequest"/>.
        /// 
        /// Translation Service data is saved to <seealso cref="ISettingsManager"/>
        /// if settings page is switched from visible to invisible and the
        /// <paramref name="settings"/> param is non-null.
        /// </summary>
        /// <param name="isSettingsPageVisibleRequest"></param>
        /// <param name="settings"></param>
        private void SwitchSettingsPageView(bool isSettingsPageVisibleRequest,
                                            ISettingsManager settings = null)
        {
            var settingsViewModel = CurrentPage as SettingsPageViewModel;

            if (settingsViewModel != null)
            {
                // Switch to display other than Settings page
                if (settings != null)
                {
                    // Copy data from settings viewmodel to model
                    settingsViewModel.SaveOptionsToModel(settings.Options);
                }

                // Switch pages back to normal view and dispose of settings page
                SwitchCurrentPageToNewPage(mCurrentBackPage, true);
                mCurrentBackPage = null;
                IsSettingsPageVisible = false;
            }
            else
            {
                // Switch to display Settings page
                if (settings == null)
                    settings = GetService<ISettingsManager>();

                // Copy data from settings Options model to viewmodel
                var settingsVM = new SettingsPageViewModel(mSettingsPageModels);
                settingsVM.LoadOptionsFromModel(settings.Options);

                mCurrentBackPage = SwitchCurrentPageToNewPage(settingsVM, false);
                IsSettingsPageVisible = true;
            }
        }

        /// <summary>
        /// Switch the currently shown page to the page in <paramref name="newPage"/>.
        /// The page that is current at the time of the call is disposed if <paramref name="disposeOldPage"/>
        /// is true. Otherwise, the currentpage is returned and can later on be switched in back again.
        /// </summary>
        /// <param name="newPage"></param>
        /// <param name="disposeOldPage"></param>
        /// <returns></returns>
        private PageBaseViewModel SwitchCurrentPageToNewPage(PageBaseViewModel newPage, bool disposeOldPage)
        {
            PageBaseViewModel oldPage = CurrentPage;
            CurrentPage = null;

            var freeze = newPage as IDocumentCanUnload;  // Freeze states of current page
            if (freeze != null)
                freeze.OnViewUnloaded();

            CurrentPage = newPage;

            freeze = oldPage as IDocumentCanUnload;    // Restore states of new current page
            if (freeze != null)
                freeze.OnViewLoaded();

            if (disposeOldPage == true && oldPage != null)
            {
                if (oldPage is IDisposable)
                    (oldPage as IDisposable).Dispose();

                oldPage = null;
            }

            return oldPage;
        }
        #endregion methods
    }
}
