namespace LocultApp.Controls.Solution.Base
{
    using ExplorerLib;
    using LocultApp;
    using LocultApp.Models;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Pages.EditPageDocuments;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events;
    using MSTranslate.Interfaces;
    using ServiceLocator;
    using System;
    using System.Globalization;
    using System.Windows.Input;
    using TranslationSolutionViewModelLib.ViewModels;

    /// <summary>
    /// Class delivers a base implementation for editing target file settings of a project.
    /// These settings can be edit when creating a new solution or when reviewing/updating
    /// projects of exisitng solutions. each of these use cases can be supported by a separate
    /// derived class.
    /// </summary>
    public abstract class UpdateTargetFilesBaseViewModel : DocumentDirtyChangedViewModelBase
    {
        #region fields
        public const string mDefaultType = @"RESX";
        public const string mDefaultTypeFileExtension = @"resx";
        public const string mDefaultTargetCollectionItemFilePathName = @"C:\TEMP\";

        protected string mNewEditTarget;
        protected string mNewEditTargetComment;

        private ICommand mAddTargetFileCommand;
        private ICommand mRemoveTargetFileCommand;
        private ICommand mSelectedItemChangedCommand;
        private ICommand mBrowseForTargetFileCommand;
        #endregion fields

        #region properties
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
                    RaisePropertyChanged(() => NewEditTarget);

                    // Not necessary here since editing strings does not invalidate the collection
                    // Add/Remove does invalidate the collection...
                    ////this.IsDirty = true;
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
                    RaisePropertyChanged(() => NewEditTargetComment);

                    // Not necessary here since editing strings does not invalidate the collection
                    // Add/Remove does invalidate the collection...
                    ////this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets a command to exit (end) the application.
        /// </summary>
        public ICommand AddTargetFileCommand
        {
            get
            {
                if (mAddTargetFileCommand == null)
                {
                    mAddTargetFileCommand = new RelayCommand<object>((p) =>
                        AddTargetFileCommandExecuted(NewEditTarget,
                                                          NewEditTargetComment,
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
                    mRemoveTargetFileCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as FileReferenceViewModel;

                        if (param == null)
                            return;

                        RemoveTargetFileCommandExecuted(param);
                    });
                }

                return mRemoveTargetFileCommand;
            }
        }

        /// <summary>
        /// Gets a command to update related properties when the currently
        /// selected target file changed.
        /// </summary>
        public ICommand SelectedItemChangedCommand
        {
            get
            {
                if (mSelectedItemChangedCommand == null)
                {
                    mSelectedItemChangedCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as FileReferenceViewModel;

                        if (param == null)
                            return;

                        NewEditTarget = param.Path;
                        NewEditTargetComment = param.Comment;
                        ////this.NewEditTaregetType = param.Type;
                    });
                }

                return mSelectedItemChangedCommand;
            }
        }

        /// <summary>
        /// Gets a command to show a file open dialog that lets
        /// a user select a file from persistence.
        /// </summary>
        public ICommand BrowseForTargetFileCommand
        {
            get
            {
                if (mBrowseForTargetFileCommand == null)
                {
                    mBrowseForTargetFileCommand = new RelayCommand<object>((p) =>
                    {
                        var explorer = ServiceLocator.ServiceContainer.Instance.GetService<IExplorer>();

                        var filePath = NewEditTarget;
                        if (string.IsNullOrEmpty(filePath) == true)
                        {
                            // Try to reset default path from collection of files if new path is empty
                            filePath = GetDefaultTargetFilePath();
                        }

                        string nextFilePath = explorer.FileOpen(EditTranslationsDocumentViewModel.StringResourceFileFilter,
                                                                filePath, AppCore.MyDocumentsUserDir);

                        if (string.IsNullOrEmpty(nextFilePath) == false)
                        {
                            NewEditTarget = nextFilePath;
                        }
                    });
                }

                return mBrowseForTargetFileCommand;
            }
        }

        private ICommand mAddTargetFilesCommand;
        public ICommand AddTargetFilesCommand
        {
            get
            {
                if (mAddTargetFilesCommand == null)
                {
                    mAddTargetFilesCommand = new RelayCommand<object>((p) =>
                    {
                        var explorer = ServiceLocator.ServiceContainer.Instance.GetService<IExplorer>();

                        var filePath = NewEditTarget;
                        if (string.IsNullOrEmpty(filePath) == true)
                        {
                            // Try to reset default path from collection of files if new path is empty
                            filePath = GetDefaultTargetFilePath();
                        }

                        var nextFilePath = explorer.FileOpenMultipleFiles(EditTranslationsDocumentViewModel.StringResourceFileFilter,
                                                                filePath, AppCore.MyDocumentsUserDir);

                        if (nextFilePath == null)
                            return;

                        var translator = ServiceContainer.Instance.GetService<ITranslator>();

                        foreach (var item in nextFilePath)
                        {
                            string info = "unknown language (not BCP 47 conform?).";
                            try
                            {
                                var regionCulture = BCP_47.ResolveRegionCultureFromFileName(item);
                                info = regionCulture;

                                //// There is no zh language so we reset this to CHS
                                //// https://msdn.microsoft.com/en-us/library/windows/apps/system.globalization.regioninfo%28v=vs.105%29.aspx
                                ////if (regionCulture == "zh-CHS" || regionCulture == "zh-Hans")
                                ////    regionCulture = "CN";

                                //var regionInfo = new RegionInfo(regionCulture);
                                CultureInfo ci = new CultureInfo(regionCulture);

                                info = string.Format("BCP 47: {0}, language/country: {1}, {2}", ci.Name, ci.NativeName, ci.EnglishName);
                            }
                            catch
                            {
                            }

                            info = "String translation to: " + info;

                            AddTargetFileCommandExecuted(item, info,
                                                         UpdateNewSolutionTargetFilesViewModel.mDefaultType, false);
                        }
                    }
                    );
                }

                return mAddTargetFilesCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Check whether a given string can represent a path and throw an exception if it cannot.
        /// </summary>
        /// <param name="testName"></param>
        protected void IsValidFIlename(string testName)
        {
            var filename = System.IO.Path.IsPathRooted(testName);
            System.IO.Path.GetFileName(testName);

            new Uri(testName, UriKind.Absolute);
        }

        protected abstract void AddTargetFileCommandExecuted(string newEditTarget,
                                                             string newEditTargetComment,
                                                             string mDefaultType,
                                                             bool updateFields = true);

        /// <summary>
        /// Get the path and file name of the first item in the colleciton of target files.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetDefaultTargetFilePath();

        /// <summary>
        /// Remove the given item from the colleciton of target files.
        /// </summary>
        /// <param name="fileReferenceViewModel"></param>
        /// <returns></returns>
        protected abstract bool RemoveTargetFileCommandExecuted(FileReferenceViewModel param);
        #endregion methods
    }
}
