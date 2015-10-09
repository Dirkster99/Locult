namespace LocultApp.ViewModels.Pages.StartPage
{
    using ExplorerLib;
    using FolderBrowser;
    using FolderBrowser.Dialogs.Interfaces;
    using LocultApp;
    using LocultApp.Controls.Exception;
    using LocultApp.Controls.Solution;
    using LocultApp.Controls.Solution.Base;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Events;
    using LocultApp.ViewModels.Pages.EditPageDocuments;
    using LocultApp.ViewModels.Pages.SettingsPages;
    using MsgBox;
    using MSTranslate.Interfaces;
    using ServiceLocator;
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using TranslationSolutionViewModelLib.ViewModels;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// Class manages all aspects necessary to either create or
    /// open an existing translator solution.
    /// 
    /// The result should always be an OpenSolution event since
    /// even a new solution is saved to disk in the first step.
    /// </summary>
    public class NewSolutionViewModel : StartPageSolutionViewModel, IDisposable, ICanForwardExceptionsToDisplay
    {
        #region fields
        private string mNewSolutionName = TranslatorSolution._DefaultName;
        private string mNewSolutionLocation;
        private string mNewSourceFilePathName = @"C:\Temp\Strings.resx";
        private string mNewTargetFilehName = @"Strings";

        private ICommand mCreateSolutionCommand;
        private ICommand mBrowseForSourceFileCommand;

        private UpdateNewSolutionTargetFilesViewModel mUpdateTargetFiles = null;

        private string mDefaultDocumentLocation = @"C:\TEMP\";
        private ApplicationRequestEventHandler RequestedAction;
        private string mComment;
        private bool mDisposed = false;

        private IDropDownViewModel mDropDownBrowser = null;
        private IBookmarkedLocationsViewModel mBookmarkedLocation = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="defaultDocumentLocation"></param>
        public NewSolutionViewModel(string defaultDocumentLocation = null,
                                    ApplicationRequestEventHandler requestedAction = null)
        {
            mComment = Project.DefaultComment;

            if (defaultDocumentLocation != null)
                mDefaultDocumentLocation = defaultDocumentLocation;

            mNewSolutionLocation = mDefaultDocumentLocation;

            var settings = GetService<ISettingsManager>();
            var translator = GetService<ITranslator>();

            string comment = string.Empty;
            var defaultTargetLanguage = settings.Options.GetOptionValue<string>("Options", "DefaultTargetLanguage");
            var langCode = translator.LanguageList.SingleOrDefault(item => item.Bcp47_LangCode == defaultTargetLanguage);

            if (langCode != null)
                comment = string.Format("Translate to {0}", langCode.FormatedString);                

            // Initialize solution target viewmodel with meaningful sample data
            mUpdateTargetFiles = new UpdateNewSolutionTargetFilesViewModel(this);
            mUpdateTargetFiles.ClearData();

            mUpdateTargetFiles.InitTargetSampleData(UpdateTargetFilesBaseViewModel.mDefaultTargetCollectionItemFilePathName + "\\" +
                                                    mNewTargetFilehName + "." + defaultTargetLanguage + "." + UpdateTargetFilesBaseViewModel.mDefaultTypeFileExtension,
                                                    comment,
                                                    UpdateTargetFilesBaseViewModel.mDefaultType);

            RequestedAction = requestedAction;

            BookmarkedLocations = this.ConstructBookmarks();
            DropDownBrowser = InitializeDropDownBrowser(NewSolutionLocation);
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets/Sets the name of a new translation solution.
        /// </summary>
        public string NewSolutionName
        {
            get
            {
                return mNewSolutionName;
            }

            set
            {
                if (mNewSolutionName != value)
                {
                    mNewSolutionName = value;
                    RaisePropertyChanged(() => NewSolutionName);
                }
            }
        }

        /// <summary>
        /// Get/set file path of new solution (minus name)
        /// </summary>
        public string NewSolutionLocation
        {
            get
            {
                return mNewSolutionLocation;
            }

            set
            {
                if (mNewSolutionLocation != value)
                {
                    mNewSolutionLocation = value;
                    RaisePropertyChanged(() => NewSolutionLocation);
                }
            }
        }

        /// <summary>
        /// Gets the path and name of a new solution.
        /// </summary>
        public string NewSourceFilePathName
        {
            get
            {
                return mNewSourceFilePathName;
            }

            set
            {
                if (mNewSourceFilePathName != value)
                {
                    mNewSourceFilePathName = value;
                    RaisePropertyChanged(() => NewSourceFilePathName);
                }
            }
        }

        /// <summary>
        /// Gets/sets a comment string for the corrently selected or a new target file
        /// </summary>
        public string Comment
        {
            get
            {
                return mComment;
            }

            set
            {
                if (mComment != value)
                {
                    mComment = value;
                    RaisePropertyChanged(() => Comment);

                    // Not necessary here since editing strings does not invalidate the collection
                    // Add/Remove does invalidate the collection...
                    ////this.IsDirty = true;
                }
            }
        }      

        /// <summary>
        /// Gets a viewmodel object that can manage a target file collection.
        /// </summary>
        public UpdateNewSolutionTargetFilesViewModel UpdateTargetFiles
        {
            get
            {
                return mUpdateTargetFiles;
            }
        }

        /// <summary>
        /// Gets a command that creates a new solution from given parameters
        /// </summary>
        public ICommand CreateSolutionCommand
        {
            get
            {
                if (mCreateSolutionCommand == null)
                {
                    mCreateSolutionCommand = new RelayCommand<object>((p) =>
                    {
                        CreateSolutionCommandExecuted(NewSolutionName,
                                                      NewSolutionLocation,
                                                      NewSourceFilePathName,
                                                      Comment,
                                                      mUpdateTargetFiles.TargetFilesCollection);
                    },
                    (p) =>
                    {
                        // At least 1 target file is required for translation
                        if (mUpdateTargetFiles == null)
                            return false;

                        if (mUpdateTargetFiles.TargetFilesCollection == null)
                            return false;

                        if (mUpdateTargetFiles.TargetFilesCollection.Count <= 0)
                            return false;

                        // At least a solution name, location, and source file are required
                        if (string.IsNullOrEmpty(NewSolutionName) == true ||
                            string.IsNullOrEmpty(NewSolutionLocation) == true ||
                            string.IsNullOrEmpty(NewSourceFilePathName) == true
                            )
                            return false;

                        // Check if characters are printable
                        // if string appear to have content (see previous check)
                        if (NewSolutionName.Trim().Length == 0 ||
                            NewSolutionLocation.Trim().Length == 0 ||
                            NewSourceFilePathName.Trim().Length == 0
                            )
                            return false;

                        // Enable command since basic input seems to be available
                        return true;
                    });
                }

                return mCreateSolutionCommand;
            }
        }

        /// <summary>
        /// Gets a command to show a file open dialog that lets
        /// a user select a file from persistence.
        /// </summary>
        public ICommand BrowseForSourceFileCommand
        {
            get
            {
                if (mBrowseForSourceFileCommand == null)
                {
                    mBrowseForSourceFileCommand = new RelayCommand<object>((p) =>
                    {
                        var explorer = ServiceLocator.ServiceContainer.Instance.GetService<IExplorer>();

                        string nextFilePath = explorer.FileOpen(EditTranslationsDocumentViewModel.StringResourceFileFilter,
                                                                mNewSourceFilePathName, AppCore.MyDocumentsUserDir);

                        if (string.IsNullOrEmpty(nextFilePath) == false)
                        {
                            NewSourceFilePathName = nextFilePath;
                        }
                    });
                }

                return mBrowseForSourceFileCommand;
            }
        }

        /// <summary>
        /// Gets a viewmodel object that can be used to drive a folder browser
        /// displayed inside a drop down button element.
        /// </summary>
        public IDropDownViewModel DropDownBrowser
        {
            get
            {
                return mDropDownBrowser;
            }

            private set
            {
                if (mDropDownBrowser != value)
                {
                    mDropDownBrowser = value;
                    RaisePropertyChanged(() => DropDownBrowser);
                }
            }
        }

        /// <summary>
        /// Gets/sets a bookmark folder property to manage bookmarked folders.
        /// </summary>
        public IBookmarkedLocationsViewModel BookmarkedLocations
        {
            get
            {
                return mBookmarkedLocation;
            }

            private set
            {
                if (mBookmarkedLocation != value)
                {
                    mBookmarkedLocation = value;
                    RaisePropertyChanged(() => BookmarkedLocations);
                }
            }
        }

        private string UpdateCurrentPath()
        {
            return this.NewSolutionLocation;
        }

        private IBookmarkedLocationsViewModel UpdateBookmarks()
        {
            return this.BookmarkedLocations;
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

        public void ForwardExceptionToDisplay(Exception exp)
        {
            if (exp == null)
                RequestApplicationEvent(new ApplicationRequestEvent(ApplicationRequest.InitProcessing));
            else
                RequestApplicationEvent(new ApplicationRequestEvent(ApplicationRequest.DisplayException, exp));
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
                    if (mUpdateTargetFiles != null)
                    {
                        // Dispose of the curently used resources
                        //// this.mUpdateTargetFiles.DirtyFlagChangedEvent -= this.UpdateTargetFiles_DirtyFlagChangedEvent;

                        mUpdateTargetFiles.Dispose();

                        // NUll 'em out boys
                        Interlocked.Exchange(ref mUpdateTargetFiles, null);
                        Interlocked.Exchange(ref RequestedAction, null);
                    }
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
        ~NewSolutionViewModel()
        {
            Dispose(false);
        }

        /// <summary>
        /// Creates a new solution file in the file system.
        /// </summary>
        /// <param name="newSolutionName"></param>
        /// <param name="newSolutionLocation"></param>
        /// <param name="newSourceFilePathName"></param>
        /// <param name="TargetFilesCollection"></param>
        private void CreateSolutionCommandExecuted(string newSolutionName,
                                                   string newSolutionLocation,
                                                   string newSourceFilePathName,
                                                   string newProjectComment,
                                                   IList<FileReferenceViewModel> TargetFilesCollection)
        {
            var msg = ServiceContainer.Instance.GetService<IMessageBoxService>();

            try
            {
                RequestApplicationEvent(new ApplicationRequestEvent(ApplicationRequest.InitProcessing));
                    
                string msgString = null;
                string solutionPathName = null;

                if (System.IO.Directory.Exists(newSolutionLocation) == false)
                    msgString = string.Format( LocultApp.Local.Strings.STR_ERROR_ACCESSING_DIRECTORY, newSolutionLocation);
                else
                {
                    if (System.IO.File.Exists(newSourceFilePathName) == false)
                        msgString = string.Format(LocultApp.Local.Strings.STR_ERROR_ACCESSING_FILE, newSourceFilePathName);
                    else
                    {
                        solutionPathName = System.IO.Path.Combine(newSolutionLocation, newSolutionName + TranslatorSolution._DefaultSolutionExtension);

                        if (System.IO.File.Exists(solutionPathName) == true)
                            msgString = string.Format(LocultApp.Local.Strings.STR_ERROR_ACCESSING_FILE_CREATE_WOULD_Overwrite, solutionPathName);
                        else
                        {
                            var item = TargetFilesCollection.SingleOrDefault(p => string.Compare(p.Path, newSourceFilePathName, true) == 0);

                            if (item != null)
                                msgString = string.Format(LocultApp.Local.Strings.STR_ERROR_DUPLICATE_REFERENCE, newSourceFilePathName);
                        }
                    }
                }

                // Show error message if a prerequisite failed.
                if (msgString != null)
                    throw new Exception(msgString);

                // Build solution model and save and reload it via requested action
                var solution = AppViewModel.ConvertToSolutionModel(solutionPathName, newSolutionName,
                                                                   newSourceFilePathName, newProjectComment,
                                                                   TargetFilesCollection);

                solution.SetComment(string.Format("{0}\n{1}: {2}",
                                                  TranslatorSolution._DefaultComment,
                                                  LocultApp.Local.Strings.STR_CREATED_WITH_DATE_TIME_APPENDED,
                                                  DateTime.Now));

                // switch view and reload saved solution
                if (RequestedAction != null)
                    RequestedAction(this, new ApplicationRequestEvent(ApplicationRequest.NewSolution, solutionPathName, solution));
            }
            catch (System.Exception exp)
            {
                if (RequestApplicationEvent(new ApplicationRequestEvent(ApplicationRequest.DisplayException, exp)) == false)
                    msg.Show(exp);
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

        /// <summary>
        /// Method configures a drop down element to show a
        /// folder picker dialog up on opening up.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IDropDownViewModel InitializeDropDownBrowser(string path)
        {
            // See Loaded event in FolderBrowserTreeView_Loaded method to understand initial load
            var treeBrowser = FolderBrowserFactory.CreateBrowserViewModel();

            if (string.IsNullOrEmpty(path) == false)
                treeBrowser.InitialPath = path;
            else
                treeBrowser.InitialPath = this.NewSolutionLocation;

            treeBrowser.SetSpecialFoldersVisibility(true);

            var dlgVM = FolderBrowserFactory.CreateDropDownViewModel(treeBrowser, BookmarkedLocations, this.DropDownClosedResult);

            dlgVM.UpdateInitialPath = this.UpdateCurrentPath;
            dlgVM.UpdateInitialBookmarks = this.UpdateBookmarks;

            dlgVM.ButtonLabel = LocultApp.Local.Strings.STR_SELECT_A_FOLDER;

            return dlgVM;
        }

        /// <summary>
        /// Constructs a few initial entries for
        /// the recent folder collection that implements folder bookmarks.
        /// </summary>
        /// <returns></returns>
        private IBookmarkedLocationsViewModel ConstructBookmarks()
        {
            IBookmarkedLocationsViewModel ret = FolderBrowserFactory.CreateReceentLocationsViewModel();

            var optGroup = GetService<ISettingsManager>().Options.GetOptionGroup("Options");
            EditBookmarksViewModel.LoadBookMarkOptionsFromModel(GetService<ISettingsManager>().Options, ret);

            if (ret.DropDownItems.Count > 0)
                ret.SelectedItem = ret.DropDownItems[0];

            return ret;
        }

        /// <summary>
        /// Method is invoked when drop element is closed.
        /// </summary>
        /// <param name="bookmarks"></param>
        /// <param name="selectedPath"></param>
        /// <param name="result"></param>
        private void DropDownClosedResult(IBookmarkedLocationsViewModel bookmarks,
                                          string selectedPath,
                                          FolderBrowser.Dialogs.Interfaces.Result result)
        {
            if (result == FolderBrowser.Dialogs.Interfaces.Result.OK)
            {
                this.BookmarkedLocations = bookmarks.Copy();
                this.NewSolutionLocation = selectedPath;

                var optGroup = GetService<ISettingsManager>().Options.GetOptionGroup("Options");

                EditBookmarksViewModel.SaveBookmarksToModel(BookmarkedLocations.DropDownItems, optGroup);
            }
        }
        #endregion methods
    }
}
