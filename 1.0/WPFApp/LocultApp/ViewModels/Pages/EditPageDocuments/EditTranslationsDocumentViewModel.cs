namespace LocultApp.ViewModels.Pages.EditPageDocuments
{
    using ExplorerLib;
    using LocultApp;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events;
    using LocultApp.ViewModels.StringSource;
    using MsgBox;
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Implements a viewmodel for the translation of strings (eg. side by side grid view of strings)
    /// with editing capabilities.
    /// 
    /// This viewmodel supports only the document related content.
    /// Support for additional controls, such as, solution navigation (treeview)
    /// etc. are implemented in seperate viewmodel classes.
    /// </summary>
    public class EditTranslationsDocumentViewModel : DocumentDirtyChangedViewModelBase,
                                                     IStringCollectionDiffViewModelParent
    {
        #region fields
        public static readonly string StringSolutionFileFilter =
                                         "(*.locult)|*.locult" +
                                         "|" + LocultApp.Local.Strings.STR_FileType_FileFilter_AllFiles;

        public static readonly string StringResourceFileFilter =
                                         LocultApp.Local.Strings.STR_FileType_FileFilter_resx +
                                         "|" + LocultApp.Local.Strings.STR_FileType_FileFilter_AllFiles;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private RelayCommand<object> mBrowseForSourceFile = null;
        private RelayCommand<object> mBrowseForTargetFile = null;

        private RelayCommand<object> mLoadSourceAndTargetFilesCommand = null;
        private RelayCommand<object> mSaveSourceCommand = null;
        private RelayCommand<object> mSaveTargetCommand = null;

        private RelayCommand<object> mApplyCurrentEdit = null;
        private RelayCommand<object> mDeleteSelectedEntries = null;
        private RelayCommand<object> mNewTargetCommand = null;

        private StringCollectionDiffViewModel mStringDiff = null;

        private IProcessItems mProcessItems;
        private Color mAlternateGridColor;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public EditTranslationsDocumentViewModel(IProcessItems processItems)
            : base(null)
        {
            mProcessItems = processItems;
            mStringDiff = new StringCollectionDiffViewModel(this, mProcessItems);

            ColorGridOption = true;
            mAlternateGridColor = System.Windows.Media.Color.FromScRgb(0x10, 0x33, 0x99, 0xff);
        }
        #endregion constructors

        #region properties
        #region grid color options
        /// <summary>
        /// Get whether the displayed grid contains alternating colored rows or not.
        /// </summary>
        public bool ColorGridOption{ get; private set; }

        /// <summary>
        /// Gets the alternate grid color of the grid (this is active if ColorGridOption is set).
        /// </summary>
        public Color AlternateGridColor
        {
            get
            {
                return mAlternateGridColor;
            }

            private set
            {
                if (mAlternateGridColor != value)
                {
                    mAlternateGridColor = value;
                    RaisePropertyChanged(() => AlternateGridColor);
                }
            }
        }
        #endregion grid color options

        /// <summary>
        /// Get viewmodel that manages all fucntions concering
        /// the diff of 2 data-sources (source and target)
        /// that provide string for translation (usually from
        /// source into target).
        /// </summary>
        public StringCollectionDiffViewModel StringDiff
        {
            get
            {
                return mStringDiff;
            }
        }

        /// <summary>
        /// Determine whether data is available in the string colleciton or not.
        /// </summary>
        public bool StringCollectionHasData
        {
            get
            {
                if (mStringDiff == null)
                    return false;

                if (mStringDiff.DiffSource == null)
                    return false;

                if (mStringDiff.DiffSource.LogView == null)
                    return false;

                return mStringDiff.DiffSource.HasData;
            }
        }

        /// <summary>
        /// Determine if exactly 1 item is selected or not.
        /// </summary>
        public bool OneEntrySelected
        {
            get
            {
                if (mStringDiff == null)
                    return false;

                if (mStringDiff.DiffSource == null)
                    return false;

                if (mStringDiff.DiffSource.LogView == null)
                    return false;

                if (mStringDiff.DiffSource.HasData == true)
                    return (mStringDiff.DiffSource.SelectedItemsCount == 1);

                return false;
            }
        }

        /// <summary>
        /// Determine whether user has selected multiple items or not.
        /// This property is useful for bindings and commands that can
        /// process more than one selected item.
        /// </summary>
        public bool MultipleItemsSelected
        {
            get
            {
                if (mStringDiff == null)
                    return false;

                if (mStringDiff.DiffSource == null)
                    return false;

                if (mStringDiff.DiffSource.LogView == null)
                    return false;

                if (mStringDiff.DiffSource.HasData == true)
                {
                    if (mStringDiff.DiffSource.SelectedItemsCount > 0)
                        return true;
                }

                return false;
            }
        }

        #region commands
        #region Load Save
        /// <summary>
        /// Get command to show a file open command that lets the user select a source file.
        /// </summary>
        public RelayCommand<object> BrowseForSourceFile
        {
            get
            {
                if (mBrowseForSourceFile == null)
                    mBrowseForSourceFile = new RelayCommand<object>((p) =>
                    {
                        try
                        {
                            mStringDiff.SourceFilePath = FileOpen(EditTranslationsDocumentViewModel.StringResourceFileFilter, mStringDiff.SourceFilePath);
                        }
                        catch (Exception exp)
                        {
                            MsgBox.Show(exp, LocultApp.Local.Strings.STR_UNEXPECTED_ERROR,
                                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton);
                        }
                    });

                return mBrowseForSourceFile;
            }
        }

        /// <summary>
        /// Get command to show a file open command that lets the user select a target file.
        /// </summary>
        public RelayCommand<object> BrowseForTargetFile
        {
            get
            {
                if (mBrowseForTargetFile == null)
                    mBrowseForTargetFile = new RelayCommand<object>((p) =>
                    {
                        try
                        {
                            mStringDiff.TargetFilePath = FileOpen(EditTranslationsDocumentViewModel.StringResourceFileFilter,
                                                                            mStringDiff.SourceFilePath);
                        }
                        catch (Exception exp)
                        {
                            MsgBox.Show(exp, LocultApp.Local.Strings.STR_UNEXPECTED_ERROR,
                                             MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton);
                        }
                    });

                return mBrowseForTargetFile;
            }
        }

        /// <summary>
        /// Get command that loads source and target strings into the application
        /// to let the user start editing and viewing of translations
        /// </summary>
        public RelayCommand<object> LoadSourceAndTargetFilesCommand
        {
            get
            {
                if (mLoadSourceAndTargetFilesCommand == null)
                    mLoadSourceAndTargetFilesCommand = new RelayCommand<object>((p) =>
                    {
                        // Load source and target file to start editing and viewing of translations
                        StringDiff.LoadFiles(mStringDiff.SourceFilePath, mStringDiff.TargetFilePath);
                    },
                    (p) =>
                    {
                        if (mStringDiff == null)
                            return false;

                        // Enable Load Diff command if both paths have a string to process
                        if (string.IsNullOrEmpty(mStringDiff.TargetFilePath) || string.IsNullOrEmpty(mStringDiff.SourceFilePath))
                            return false;

                        return true;
                    });

                return mLoadSourceAndTargetFilesCommand;
            }
        }

        public RelayCommand<object> NewTargetCommand
        {
            get
            {
                if (mNewTargetCommand == null)
                    mNewTargetCommand = new RelayCommand<object>((p) =>
                    {
                        // Load source and target file to start editing and viewing of translations
                        StringDiff.LoadFilesWithNewTarget(mStringDiff.SourceFilePath, mStringDiff.TargetFilePath);
                    },
                    (p) =>
                    {
                        if (mStringDiff == null)
                            return false;

                        // Enable Load Diff command if both paths have a string to process
                        if (string.IsNullOrEmpty(mStringDiff.TargetFilePath) || string.IsNullOrEmpty(mStringDiff.SourceFilePath))
                            return false;

                        return true;
                    });

                return mNewTargetCommand;
            }
        }

        /// <summary>
        /// Get command that saves target strings in to the file system.
        /// </summary>
        public RelayCommand<object> SaveSourceCommand
        {
            get
            {
                if (mSaveSourceCommand == null)
                    mSaveSourceCommand = new RelayCommand<object>((p) =>
                    {
                        string path = p as string;

                        SaveDocumentFile(mStringDiff.SourceFilePath, StringDiff.SaveSourceFile,
                                         false, EditTranslationsDocumentViewModel.StringResourceFileFilter);
                    },
                    (p) =>
                    {
                        return StringCollectionHasData;
                    });

                return mSaveSourceCommand;
            }
        }

        /// <summary>
        /// Get command that saves target strings in to the file system.
        /// </summary>
        public RelayCommand<object> SaveTargetCommand
        {
            get
            {
                if (mSaveTargetCommand == null)
                    mSaveTargetCommand = new RelayCommand<object>((p) =>
                    {
                        string path = p as string;

                        SaveDocumentFile(mStringDiff.TargetFilePath,
                                         StringDiff.SaveTargetFile,  // This delegate will do the actual saving
                                         false,
                                         EditTranslationsDocumentViewModel.StringResourceFileFilter);
                    },
                    (p) =>
                    {
                        return StringCollectionHasData;
                    });

                return mSaveTargetCommand;
            }
        }
        #endregion Load Save

        #region DataGrid Commands
        /// <summary>
        /// Gets a command that commit edits from seperate textbox controls to
        /// the currently selected row.
        /// </summary>
        public RelayCommand<object> ApplyCurrentEdit
        {
            get
            {
                if (mApplyCurrentEdit == null)
                    mApplyCurrentEdit = new RelayCommand<object>((p) => ApplyCurrentEdit_Executed(p), (p) =>
                    {
                        try
                        {
                            if (StringDiff != null)
                            {
                                if (StringDiff.DiffSource != null)
                                {
                                    if (StringDiff.DiffSource != null)
                                        return StringDiff.DiffSource.SelectedItems.Count == 1;
                                }
                            }

                            return false;
                        }
                        catch (Exception)
                        {
                        }

                        return false;
                    });

                return mApplyCurrentEdit;
            }
        }

        /// <summary>
        /// Gets a command that deletes the currently selected rows from the grid.
        /// </summary>
        public RelayCommand<object> DeleteSelectedEntries
        {
            get
            {
                if (mDeleteSelectedEntries == null)
                    mDeleteSelectedEntries =
                         new RelayCommand<object>((p) => DeleteSelectedEntries_Executed(),
                                                  (p) => DeleteSelectedEntries_CanExecute());

                return mDeleteSelectedEntries;
            }
        }
        #endregion DataGrid Commands
        #endregion commands

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
                    if (mStringDiff != null)
                    {
                        var dispose = mStringDiff as IDisposable;

                        if (dispose != null)
                            dispose.Dispose();

                        // Dispose of the curently displayed content
                        Interlocked.Exchange(ref mStringDiff, null);
                        Interlocked.Exchange(ref mProcessItems, null);
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
        /// Gets the message box service from the service locator.
        /// </summary>
        private IMessageBoxService MsgBox
        {
            get
            {
                return ServiceLocator.ServiceContainer.Instance.GetService<IMessageBoxService>();
            }
        }

        /// <summary>
        /// Gets the application settings service from the service locator.
        /// </summary>
        private ISettingsManager Settings
        {
            get
            {
                return ServiceLocator.ServiceContainer.Instance.GetService<ISettingsManager>();
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Signal end of processing with error to parent object.
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="exp"></param>
        /// <param name="message"></param>
        public void EndProcessingSolutionWithError(string processID, Exception exp, string message)
        {
            mProcessItems.EndProcessingSolutionWithError(processID, exp, message);
        }

        /// <summary>
        /// Loads the editing content from disk for source and target (if available).
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public bool LoadEditPage(string sourceFilePath, string targetPath)
        {
            mStringDiff.SourceFilePath = sourceFilePath;

            // The direct child of a project is a collection of target files
            mStringDiff.TargetFilePath = targetPath;

            if (System.IO.File.Exists(mStringDiff.TargetFilePath))
                StringDiff.LoadFiles(mStringDiff.SourceFilePath, mStringDiff.TargetFilePath);
            else
                StringDiff.LoadFilesWithNewTarget(mStringDiff.SourceFilePath, mStringDiff.TargetFilePath);

            return true;
        }

        /// <summary>
        /// Get a file path name reference and return the containing path (if any)
        /// </summary>
        /// <param name="lastFilePath"></param>
        /// <returns></returns>
        private string GetDirectoryFromFilePath(string lastFilePath)
        {
            string dir = null;

            try
            {
                if (string.IsNullOrEmpty(lastFilePath) == false)
                {
                    dir = System.IO.Path.GetDirectoryName(lastFilePath);

                    if (System.IO.Directory.Exists(dir) == false)
                        dir = null;
                }
            }
            catch
            {
            }

            return dir;
        }

        /// <summary>
        /// Let the user select a file to open
        /// -> return its path if file open was OK'ed
        ///    or return null on cancel.
        /// </summary>
        /// <param name="fileFilter"></param>
        /// <returns></returns>
        private string FileOpen(string fileFilter, string lastFilePath)
        {
            var explorer = ServiceLocator.ServiceContainer.Instance.GetService<IExplorer>();

            return explorer.FileOpen(fileFilter, lastFilePath, AppCore.MyDocumentsUserDir);
        }

        /// <summary>
        /// Save a file with a given path <paramref name="path"/> (that may be ommited -> results in SaveAs)
        /// using a given save function <paramref name="saveDocumentFunction"/> that takes a string parameter and returns bool on success.
        /// The <param name="saveAsFlag"/> can be set to true to indicate if whether SaveAs function is intended.
        /// The <param name="FileExtensionFilter"/> can be used to filter files when using a SaveAs dialog.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="saveDocumentFunction"></param>
        /// <param name="stringDiff"></param>
        /// <param name="saveAsFlag"></param>
        /// <param name="FileExtensionFilter"></param>
        /// <returns></returns>
        internal bool SaveDocumentFile(string path,
                                       Func<string, bool> saveDocumentFunction,
                                       bool saveAsFlag = false,
                                       string FileExtensionFilter = "")
        {
            var explorer = ServiceLocator.ServiceContainer.Instance.GetService<IExplorer>();

            return explorer.SaveDocumentFile(path,
                                             saveDocumentFunction,   // This delegate will do the actual saving
                                             AppCore.MyDocumentsUserDir,
                                             saveAsFlag, FileExtensionFilter);
        }

        /// <summary>
        /// Applies the current edit of a selected translation to the currently selected item in the collection.
        /// The command accepts string arrays of the size 2 or 4 as parameter to interpret them either as:
        /// 
        /// 2) value change in source and target or
        /// 4) value and comment change in source and target.
        /// </summary>
        /// <param name="p"></param>
        private void ApplyCurrentEdit_Executed(object p)
        {
            if (p == null)
                return;

            string[] parameters = p as string[];

            if (parameters == null)
                return;

            if (StringDiff.DiffSource == null)
                return;

            if (StringDiff.DiffSource.SelectedItem == null)
                return;

            if (parameters.Length == 2) // Values only change
            {
                StringDiff.DiffSource.SelectedItem.SetSourceValueComment(parameters[0]);
                StringDiff.DiffSource.SelectedItem.SetTargetValueComment(parameters[1]);
            }
            else
            {
                if (parameters.Length == 4) // Value and comment change
                {
                    StringDiff.DiffSource.SelectedItem.SetSourceValueComment(parameters[0], parameters[1]);
                    StringDiff.DiffSource.SelectedItem.SetTargetValueComment(parameters[2], parameters[3]);
                }
            }
        }

        #region delete entry command
        /// <summary>
        /// Delete the currently selected entries in the source/target grid view.
        /// </summary>
        private void DeleteSelectedEntries_Executed()
        {
            List<EntryDiffViewModel> l = new List<EntryDiffViewModel>(mStringDiff.DiffSource.SelectedItems);

            if (l.Count <= 0)  // Remove multiple items
                return;

            var result = MsgBox.Show(string.Format(LocultApp.Local.Strings.STR_CONFIRM_DELETING_ENTRIES, l.Count),
                                     LocultApp.Local.Strings.STR_CONFIRM_DELETING_ENTRIES_CAPTION,
                                     MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.No);

            if (result == MsgBoxResult.Yes)
                mStringDiff.DiffSource.RemoveEntries(l);
        }

        private bool DeleteSelectedEntries_CanExecute()
        {
            try
            {
                if (mStringDiff.DiffSource.SelectedItems == null)
                    return false;

                return false;
                ////return true; DELETE Function is experimental since datagrid should only edit of target column!
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Sets the dirty flag in dependence of what child items indicate.
        /// </summary>
        /// <param name="v"></param>
        public void TargetDocumentIsDirty(bool v, bool resetChildren)
        {
            IsDirty = v;  // This assumes that only target is editable!

            // 
            if (resetChildren == true)
                this.mStringDiff.DiffSource.SetIsDirty(v);
        }
        #endregion delete entry command

        internal void InitializeSettings(SettingsModel.Interfaces.IOptionGroup group)
        {
            // Initialize grid color options
            ColorGridOption = group.GetValue<bool>("ColorGridOption");
            AlternateGridColor = group.GetValue<Color>("AlternatingGridColor");
        }
        #endregion methods
    }
}
