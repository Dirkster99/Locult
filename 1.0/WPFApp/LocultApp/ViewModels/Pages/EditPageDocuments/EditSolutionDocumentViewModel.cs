namespace LocultApp.ViewModels.Pages.EditPageDocuments
{
    using ExplorerLib;
    using FolderBrowser;
    using FolderBrowser.Dialogs.Interfaces;
    using FolderBrowser.Interfaces;
    using FolderBrowser.ViewModels;
    using LocultApp.Controls.Solution;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces;
    using LocultApp;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Windows.Input;
    using TranslationSolutionViewModelLib.ViewModels;
    using TranslatorSolutionLib.Models;
    using Settings.Interfaces;
    using SettingsModel.Interfaces;
    using LocultApp.ViewModels.Pages.SettingsPages;

    /// <summary>
    /// This viewmodel manages control states of the <seealso cref="EditSolutionDocument"/> view.
    /// This document view represent solution properties and is shown when the user is supposed to
    /// edit/view these translation solution properties.
    /// </summary>
    public class EditSolutionDocumentViewModel : DocumentViewModelBase, IEditSolutionDocument
    {
        #region fields
        private IBookmarkedLocationsViewModel mBookmarkedLocation = null;
        private IDropDownViewModel mDropDownBrowser = null;

        private SolutionRootViewModel mSolutionRoot;

        private string mNewEditTarget;
        private string mNewEditTargetComment;

        private ICommand mSelectedItemChangedCommand;
        private ICommand mAddTargetFileCommand;
        private ICommand mRemoveTargetFileCommand;
        private ICommand mBrowseForTargetFileCommand;
        private ISolutionRoot mRoot;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public EditSolutionDocumentViewModel(IProcessItems processItems)
            : base()
        {
            BookmarkedLocations = this.ConstructBookmarks();
            DropDownBrowser = InitializeDropDownBrowser(SourceFilePath);
        }
        #endregion constructors

        #region properties
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

        public ObservableCollection<ISolutionItem> SolutionChildren
        {
            get
            {
                if (mSolutionRoot != null)
                    return mSolutionRoot.Children;

                return null;
            }
        }

        /// <summary>
        /// Gets/sets a path string for the corrently selected or a new target file
        /// </summary>
        public string NewEditTarget
        {
            get
            {
                return mNewEditTarget;
            }

            set
            {
                if (mNewEditTarget != value)
                {
                    mNewEditTarget = value;
                    SetSolutionIsDirty();
                    RaisePropertyChanged(() => NewEditTarget);
                }
            }
        }

        /// <summary>
        /// Gets/sets a comment string for the corrently selected or a new target file
        /// </summary>
        public string NewEditTargetComment
        {
            get
            {
                return mNewEditTargetComment;
            }

            set
            {
                if (mNewEditTargetComment != value)
                {
                    mNewEditTargetComment = value;
                    SetSolutionIsDirty();
                    RaisePropertyChanged(() => NewEditTargetComment);

                    // Not necessary here since editing strings does not invalidate the collection
                    // Add/Remove does invalidate the collection...
                    ////this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets a command to update related properties when the currently
        /// selected project has changed.
        /// </summary>
        public ICommand SelectedItemChangedCommand
        {
            get
            {
                if (mSelectedItemChangedCommand == null)
                {
                    mSelectedItemChangedCommand = new LocultApp.ViewModels.Base.RelayCommand<object>((p) =>
                    {
                        var param = p as ProjectViewModel;

                        if (param == null)
                            return;

                        NewEditTarget = param.Path;
                        NewEditTargetComment = param.ItemTip;
                        ////this.NewEditTaregetType = param.Type;
                    });
                }

                return mSelectedItemChangedCommand;
            }
        }

        /// <summary>
        /// Gets a command to add another project target to this solution.
        /// </summary>
        public ICommand AddTargetFileCommand
        {
            get
            {
                if (mAddTargetFileCommand == null)
                {
                    mAddTargetFileCommand = new LocultApp.ViewModels.Base.RelayCommand<object>((p) =>
                        AddTargetFileCommandExecuted(mNewEditTarget,
                                                          mNewEditTargetComment,
                                                          UpdateNewSolutionTargetFilesViewModel.mDefaultType));
                }

                return mAddTargetFileCommand;
            }
        }

        /// <summary>
        /// Gets a command to remove the currently selected target file.
        /// </summary>
        public ICommand RemoveTargetFileCommand
        {
            get
            {
                if (mRemoveTargetFileCommand == null)
                {
                    mRemoveTargetFileCommand = new LocultApp.ViewModels.Base.RelayCommand<object>((p) =>
                    {
                        var param = p as ProjectViewModel;

                        if (param == null)
                            return;

                        RemoveTargetFileCommandExecuted(param);
                    });
                }

                return mRemoveTargetFileCommand;
            }
        }

        /// <summary>
        /// Gets a command to select a taget file for this project.
        /// </summary>
        public ICommand BrowseForTargetFileCommand
        {
            get
            {
                if (mBrowseForTargetFileCommand == null)
                {
                    mBrowseForTargetFileCommand = new LocultApp.ViewModels.Base.RelayCommand<object>((p) =>
                    {
                        var explorer = ServiceLocator.ServiceContainer.Instance.GetService<IExplorer>();

                        var targetDir = (string.IsNullOrEmpty(NewEditTarget) == true ? SourceFilePath :
                                                                                            NewEditTarget);

                        string nextFilePath = explorer.FileOpen(EditTranslationsDocumentViewModel.StringResourceFileFilter,
                                                                targetDir,
                                                                AppCore.MyDocumentsUserDir);

                        if (string.IsNullOrEmpty(nextFilePath) == false)
                        {
                            if (nextFilePath != NewEditTarget)
                                NewEditTarget = nextFilePath;
                        }
                    });
                }

                return mBrowseForTargetFileCommand;
            }
        }

        /// <summary>
        /// Gets the path and name of a new solution.
        /// </summary>
        public string SourceFilePathName
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(SourceFilePath) == false &&
                        string.IsNullOrEmpty(SourceFileName) == false)
                        return System.IO.Path.Combine(SourceFilePath, SourceFileName);
                    else
                    {
                        if (string.IsNullOrEmpty(SourceFilePath) == false)
                        {
                            return SourceFilePath;
                        }
                        else
                            if (string.IsNullOrEmpty(SourceFileName) == false)
                                return SourceFileName;
                    }
                }
                catch
                {
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the name of a source file for this object.
        /// </summary>
        public string SourceFileName
        {
            get
            {
                if (mSolutionRoot == null)
                    return null;

                return mSolutionRoot.ItemName;
            }

            set
            {
                if (mSolutionRoot == null)
                    return;

                if (mSolutionRoot.ItemName != value)
                {
                    mSolutionRoot.ItemName = value;
                    RaisePropertyChanged(() => SourceFileName);
                    RaisePropertyChanged(() => SourceFilePathName);
                    SetSolutionIsDirty();
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets/sets the path of a source file for this object.
        /// </summary>
        public string SourceFilePath
        {
            get
            {
                if (mSolutionRoot == null)
                    return null;

                return mSolutionRoot.Path;
            }

            set
            {
                if (mSolutionRoot == null)
                    return;

                if (mSolutionRoot.Path != value)
                {
                    mSolutionRoot.Path = value;
                    RaisePropertyChanged(() => SourceFilePath);
                    RaisePropertyChanged(() => SourceFilePathName);
                    SetSolutionIsDirty();
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets/sets an informal text based comment for this object.
        /// </summary>
        public string Comment
        {
            get
            {
                if (mSolutionRoot != null)
                    return mSolutionRoot.Comment;

                return null;
            }

            set
            {
                if (mSolutionRoot == null)
                    return;

                if (mSolutionRoot.Comment != value)
                {
                    mSolutionRoot.Comment = value;
                    RaisePropertyChanged(() => Comment);
                    IsDirty = true;
                }
            }
        }

        private SolutionRootViewModel SolutionRoot
        {
            get
            {
                return mSolutionRoot;
            }

            set
            {
                if (mSolutionRoot != value)
                {
                    mSolutionRoot = value;
                    mRoot = value;
                    RaisePropertyChanged(() => SolutionRoot);
                    RaisePropertyChanged(() => SourceFileName);
                    RaisePropertyChanged(() => SourceFilePathName);
                    RaisePropertyChanged(() => Comment);

                    IsDirty = true;
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Initalizes the editsolution document viewmodel with data from the supplied solution.
        /// </summary>
        /// <param name="solution"></param>
        public void InitDocument(ISolutionRoot root, ISolutionItem solution)
        {
            SolutionRoot = solution as SolutionRootViewModel;
            if (SolutionRoot == null)
                throw new System.NotSupportedException("Only SolutionRootViewModel parameter is supported.");

            RaisePropertyChanged(() => SolutionChildren);

            mRoot = root;

            IsDirty = false;
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (mDisposed == false)
            {
                if (disposing == true)
                {
                    if (mSolutionRoot != null)
                    {
                        // Dispose of the curently displayed content
                        Interlocked.Exchange(ref mSolutionRoot, null);
                    }
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            base.Dispose(disposing);
        }

        /// <summary>
        /// Adds new target file references into the collection of target files.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="comment"></param>
        /// <param name="type"></param>
        internal void AddTargetFileCommandExecuted(string path, string comment, string type)
        {
            if (string.IsNullOrEmpty(path) == true)
                return;

            if (string.IsNullOrEmpty(type) == true)
                return;

            var sourceFile = new FileReference(path, type, comment);

            var projectModel = new Project(sourceFile);

            SolutionRoot.AddProject(projectModel);
            SetSolutionIsDirty();
        }

        private bool RemoveTargetFileCommandExecuted(ProjectViewModel viewModel)
        {
            if (viewModel == null)
                return false;

            if (SolutionRoot.RemoveProject(viewModel) == true)
            {
                IsDirty = true;
                SetSolutionIsDirty();
                return true;
            }

            return false;
        }

        private void SetSolutionIsDirty()
        {
            if (mRoot != null)
                mRoot.SolutionIsDirty();
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

        private string UpdateCurrentPath()
        {
            return this.SourceFilePath;
        }

        private IBookmarkedLocationsViewModel UpdateBookmarks()
        {
            return this.BookmarkedLocations;
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
                treeBrowser.InitialPath = this.SourceFilePath;

            treeBrowser.SetSpecialFoldersVisibility(true);

            var dlgVM = FolderBrowserFactory.CreateDropDownViewModel(treeBrowser, BookmarkedLocations, this.DropDownClosedResult);

            dlgVM.UpdateInitialPath = this.UpdateCurrentPath;
            dlgVM.UpdateInitialBookmarks = this.UpdateBookmarks;

            dlgVM.ButtonLabel = LocultApp.Local.Strings.STR_SELECT_A_FOLDER;

            return dlgVM;
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
                this.SourceFilePath = selectedPath;

                var optGroup = GetService<ISettingsManager>().Options.GetOptionGroup("Options");

                EditBookmarksViewModel.SaveBookmarksToModel(BookmarkedLocations.DropDownItems, optGroup);
            }
        }
        #endregion methods
    }
}
