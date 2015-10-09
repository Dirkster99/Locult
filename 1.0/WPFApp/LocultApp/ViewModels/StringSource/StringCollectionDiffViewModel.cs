namespace LocultApp.ViewModels.StringSource
{
    using LocultApp.Models;
    using LocultApp.ViewModels.Base;
    using MsgBox;
    using MSTranslate.Interfaces;
    using MSTranslate.Service;
    using ServiceLocator;
    using ServiceLocatorInterfaces;
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Windows.Input;
    using WPFProcessingLib;
    using WPFProcessingLib.Interfaces;

    public class StringCollectionDiffViewModel : Base.ViewModelBase, IDisposable
    {
        #region fields
        private StringCollectionViewModel mFileSource = null;

        private string mSourceFilePath = null;
        private string mTargetFilePath = null;

        private string mDefaultSourceFileBCP47 = "en-US";
        private ILanguageCode mSelectedSourceLanguage = null;

        private string mDefaultTargetFileBCP47 = "en-US";
        private ILanguageCode mSelectedTargetLanguage = null;

        private static object lockObject = new object();
        private bool mDisposed = false;

        private IStringCollectionDiffViewModelParent mDocumentBase = null;
        private IProcessViewModel mProcessAction;

        private ICommand mCancelTranslation;
        private ICommand mTranslateCommand;

        private IProcessItems mProcessItems;
        private readonly string mLoadFiles4NewTargetProcessID = "Load4NewTarget {29718EF1-D6D8-4A68-9339-06C963440D12}";
        private readonly string mLoadFiles4DiffProcessID = "LoadFiles4Diff {29718EF1-D6D8-4A68-9339-06C963440D12}";
        private readonly string mSaveFileProcessID = "SaveFile {29718EF1-D6D8-4A68-9339-06C963440D12}";
        private readonly string mTranslateProcessID = "TranslateEntries {29718EF1-D6D8-4A68-9339-06C963440D12}";
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="document"></param>
        public StringCollectionDiffViewModel(IStringCollectionDiffViewModelParent document,
                                             IProcessItems processItems)
            : this()
        {
            mProcessItems = processItems;
            mDocumentBase = document;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public StringCollectionDiffViewModel()
            : base()
        {
            mProcessAction = ProcessFactory.CreateProcessViewModel();
        }
        #endregion constructors

        #region properties
        public StringCollectionViewModel DiffSource
        {
            get
            {
                return mFileSource;
            }

            private set
            {
                if (mFileSource != value)
                {
                    mFileSource = value;
                    RaisePropertyChanged(() => DiffSource);
                }
            }
        }

        /// <summary>
        /// Gets whether the viewmodel is currently processing data or not.
        /// </summary>
        public IProcessViewModel ProcessAction
        {
            get
            {
                return mProcessAction;
            }
        }

        /// <summary>
        /// Get whether there are data items in the FileSource collection or not
        /// (there may be no items to display if filter is applied but thats a different issue)
        /// </summary>
        public bool FileSourceHasData
        {
            get
            {
                return (DiffSource != null && DiffSource.HasData);
            }
        }

        /// <summary>
        /// Get a list of language codes and their names for translation.
        /// </summary>
        public List<ILanguageCode> LanguageList
        {
            get
            {
                var translator = GetService<ITranslator>();
                return translator.LanguageList;
            }
        }

        #region Translation Commands
        /// <summary>
        /// Gets a command that can cancel the translation processing initiated by the TranslateCommand.
        /// </summary>
        public ICommand CancelTranslationCommand
        {
            get
            {
                if (mCancelTranslation == null)
                {
                    mCancelTranslation = new RelayCommand<object>(
                        (p) =>
                        {
                            if (this.mProcessAction != null)
                                this.mProcessAction.Cancel();

                        },
                        (p) =>
                        {
                            if (this.mProcessAction != null)
                            {
                                if (this.mProcessAction.IsCancelable == true)
                                    return this.mProcessAction.IsProcessing;
                            }

                            return false;
                        });
                }

                return mCancelTranslation;
            }
        }

        /// <summary>
        /// Get command to translate selected string entries using an automated
        /// API that provides translation proposals via machine learning implementation
        /// on the Internet.
        /// 
        /// This command expects a translation direction parameter <seealso cref="ProcessDestination"/>.
        /// Translation direction is otherwise, by default, from source to destination.
        /// </summary>
        public ICommand TranslateCommand
        {
            get
            {
                if (mTranslateCommand == null)
                    mTranslateCommand = new RelayCommand<object>((p) =>
                    {
                        // This defines whethe we translate from source to destination or vice versa
                        ProcessDestination destination = ProcessDestination.Target;

                        if (p != null)
                        {
                            if (p is ProcessDestination)
                                destination = (ProcessDestination)p;
                        }

                        string sourceBCP47_LangCode = (SelectedSourceLanguage != null ? SelectedSourceLanguage.Bcp47_LangCode : SourceFileLanguage);
                        string targetBCP47_LangCode = (SelectedTargetLanguage != null ? SelectedTargetLanguage.Bcp47_LangCode : TargetFileLanguage);

                        this.TranslateSelectedEntriesAsync(sourceBCP47_LangCode, targetBCP47_LangCode, destination);
                    },
                (p) =>
                {
                    try
                    {
                        if (DiffSource != null)
                            return (DiffSource.SelectedItems.Count != 0);
                    }
                    catch
                    {
                    }

                    return false;
                });

                return mTranslateCommand;
            }
        }
        #endregion Translation Commands

        #region Diff properties
        #region Source properties
        /// <summary>
        /// Return country region portion of file name
        /// eg: DE-de from Myfile.DE-de.resx
        /// </summary>
        public string SourceFileLanguageRegion
        {
            get
            {
                return BCP_47.ResolveRegionCultureFromFileName(SourceFilePath, mDefaultSourceFileBCP47);
            }
        }

        /// <summary>
        /// Return language portion of file name
        /// eg: de-DE from Myfile.de-DE.resx
        /// </summary>
        public string SourceFileLanguage
        {
            get
            {
                return BCP_47.GetLanguageRegionFromFileName(SourceFilePath, mDefaultSourceFileBCP47);
            }
        }

        /// <summary>
        /// Gets the selected language of the source language of translation.
        /// </summary>
        public ILanguageCode SelectedSourceLanguage
        {
            get
            {
                return mSelectedSourceLanguage;
            }

            set
            {
                if (mSelectedSourceLanguage != value)
                {
                    mSelectedSourceLanguage = value;
                    RaisePropertyChanged(() => SelectedSourceLanguage);
                }
            }
        }

        /// <summary>
        /// Get target file path
        /// </summary>
        public string SourceFilePath
        {
            get
            {
                return mSourceFilePath;
            }

            set
            {
                if (mSourceFilePath != value)
                {

                    mSourceFilePath = value;
                    RaisePropertyChanged(() => SourceFilePath);
                    RaisePropertyChanged(() => SourceFileName);
                    RaisePropertyChanged(() => SourceFileLanguageRegion);
                }
            }
        }

        /// <summary>
        /// Gets the file name of the source of translation.
        /// </summary>
        public string SourceFileName
        {
            get
            {
                try
                {
                    return System.IO.Path.GetFileName(SourceFilePath);
                }
                catch
                {
                }

                return SourceFilePath;
            }
        }
        #endregion Source properties

        #region Target properties
        /// <summary>
        /// Return country region portion of file name
        /// eg: DE-de from Myfile.DE-de.resx
        /// </summary>
        public string TargetFileLanguageRegion
        {
            get
            {
                return BCP_47.ResolveRegionCultureFromFileName(TargetFilePath, mDefaultTargetFileBCP47);
            }
        }

        /// <summary>
        /// Return language region portion of file name
        /// eg: de from Myfile.de-DE.resx
        /// </summary>
        public string TargetFileLanguage
        {
            get
            {
                return BCP_47.GetLanguageRegionFromFileName(TargetFilePath, mDefaultTargetFileBCP47);
            }
        }

        /// <summary>
        /// Gets the selected language of the target language of translation.
        /// </summary>
        public ILanguageCode SelectedTargetLanguage
        {
            get
            {
                return mSelectedTargetLanguage;
            }

            set
            {
                if (mSelectedTargetLanguage != value)
                {
                    mSelectedTargetLanguage = value;
                    RaisePropertyChanged(() => SelectedTargetLanguage);
                }
            }
        }

        /// <summary>
        /// Get source file path
        /// </summary>
        public string TargetFilePath
        {
            get
            {
                return mTargetFilePath;
            }

            set
            {
                if (mTargetFilePath != value)
                {
                    mTargetFilePath = value;
                    RaisePropertyChanged(() => TargetFilePath);
                    RaisePropertyChanged(() => TargetFileName);
                    RaisePropertyChanged(() => TargetFileLanguageRegion);
                }
            }
        }


        public string TargetFileName
        {
            get
            {
                try
                {
                    return System.IO.Path.GetFileName(TargetFilePath);
                }
                catch
                {
                }

                return TargetFilePath;
            }
        }
        #endregion Target properties
        #endregion Diff properties
        #endregion properties

        #region methods
        #region Load Two Files for Diff
        /// <summary>
        /// Load a log4net log file to display its content through this ViewModel.
        /// </summary>
        /// <param name="paths">file path</param>
        public void LoadFiles(string sourcePath, string targetPath)
        {
            mProcessAction.StartCancelableProcess(
                  cts =>
                  {
                      if (mProcessItems.SetIsProcessingSolution(true, mLoadFiles4DiffProcessID) == false)
                          throw new Exception(string.Format(LocultApp.Local.Strings.STR_BLOCKED_SOLUTION_TASK, mProcessItems.CurrentProcessId));

                      try
                      {
                          LoadFilesForDiff(sourcePath, targetPath, true, cts);
                      }
                      finally
                      {
                          if (mProcessItems.SetIsProcessingSolution(false, mLoadFiles4DiffProcessID) == false)
                              throw new Exception(string.Format(LocultApp.Local.Strings.STR_FAILED_RESET_OF_SOLUTION_TASK, mProcessItems.CurrentProcessId));
                      }
                  },
                  LoadFinishedEvent,
                    LocultApp.Local.Strings.STR_CANCEL_LOAD_MESSAGE
                  );
        }

        /// <summary>
        /// Load two string resources asynchronously where the second file is only
        /// created in memory and does not exist in file system until it is saved
        /// for the first time.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        internal void LoadFilesWithNewTarget(string sourcePath, string targetPath)
        {
            mProcessAction.StartCancelableProcess(
            cts =>
            {
                if (mProcessItems.SetIsProcessingSolution(true, mLoadFiles4NewTargetProcessID) == false)
                    throw new Exception(string.Format(Local.Strings.STR_BLOCKED_SOLUTION_TASK, mProcessItems.CurrentProcessId));

                try
                {
                    LoadFilesForDiff(sourcePath, targetPath, false, cts);
                }
                finally
                {
                    if (mProcessItems.SetIsProcessingSolution(false, mLoadFiles4NewTargetProcessID) == false)
                        throw new Exception(string.Format(Local.Strings.STR_FAILED_RESET_OF_SOLUTION_TASK, mProcessItems.CurrentProcessId));
                }
            },
            LoadFinishedEvent,
                    LocultApp.Local.Strings.STR_CANCEL_LOAD_MESSAGE);
        }

        /// <summary>
        /// This is a callback method that is always called when
        /// the internal load process is finished
        /// (even when it failed to finish after initialization).
        /// </summary>
        /// <param name="loadWasSuccessful"></param>
        private void LoadFinishedEvent(bool loadWasSuccessful,
                                       Exception exp, string message)
        {
            if (loadWasSuccessful == false)
            {
                if (mDocumentBase != null)
                    mDocumentBase.EndProcessingSolutionWithError(mLoadFiles4NewTargetProcessID, exp, message);
                else
                {
                    if (exp != null)
                        GetService<IMessageBoxService>().Show(exp, message);
                }

                return;
            }

            SelectedSourceLanguage = null;
            string lngBCP47 = SourceFileLanguage;
            SelectedSourceLanguage = LanguageList.SingleOrDefault(item => item.Bcp47_LangCode == lngBCP47);

            // Make a second attempt if first attempt did not find matches (this is necessary for Chinese and Serbian)
            if (SelectedSourceLanguage == null)
                SelectedSourceLanguage = LanguageList.SingleOrDefault(item => item.Bcp47_LangCode == SourceFileLanguageRegion);

            lngBCP47 = TargetFileLanguage;
            SelectedTargetLanguage = LanguageList.SingleOrDefault(item => item.Bcp47_LangCode == lngBCP47);

            // Make a second attempt if first attempt did not find matches (this is necessary for Chinese and Serbian)
            if (SelectedTargetLanguage == null)
                SelectedTargetLanguage = LanguageList.SingleOrDefault(item => item.Bcp47_LangCode == TargetFileLanguageRegion);

            RaisePropertyChanged(() => FileSourceHasData);
        }

        /// <summary>
        /// Loads a source and target file from the file system into an internal
        /// collection such that a 'diff' on each string can be computed. A 'diff'
        /// is in this context an answer to the question:
        /// 1> Whether all source language strings can be mapped into a target language string
        /// 2> Whether all target language strings can be mapped into a source language string
        /// 3> Whether all source to target language string mappings have been verified by a translator (native speaker)
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="bLoadTarget"></param>
        public void LoadFilesForDiff(string sourcePath,
                                     string targetPath,
                                     bool bLoadTarget = true,
                                     CancellationTokenSource cancelTokenSource = null)
        {
            Dictionary<string, EntryDiffViewModel> dic = null;
            ObservableCollection<Entry> items = null;

            try
            {
                var reader = ServiceContainer.Instance.GetService<IStringFileParseServiceList>();
                var resxReader = reader.Select("RESX");

                if (cancelTokenSource != null)
                    cancelTokenSource.Token.ThrowIfCancellationRequested();

                if (System.IO.File.Exists(sourcePath) == false)
                    throw new Exception(string.Format(Local.Strings.CantAccessFile_Error_Text, sourcePath));

                items = new ObservableCollection<Entry>();

                int iRowId = 0;

                foreach (var item in resxReader.LoadFile(sourcePath, cancelTokenSource))
                {
                    if (cancelTokenSource != null)
                        cancelTokenSource.Token.ThrowIfCancellationRequested();

                    items.Add(new Entry(item.Item1, item.Item2, item.Item3));
                }

                // Insert entries from first file into dictionary since we now know the number of entries
                dic = new Dictionary<string, EntryDiffViewModel>(items.Count);
                foreach (var item in items)
                {
                    if (cancelTokenSource != null)
                        cancelTokenSource.Token.ThrowIfCancellationRequested();

                    dic.Add(item.KeyString, new EntryDiffViewModel(item.KeyString,
                                                                   item.ValueString,
                                                                   item.CommentString,
                                                                   iRowId,
                                                                   TypeOfDiff.SourceOnly));

                    iRowId++;
                }
            }
            catch
            {
                cancelTokenSource.Cancel();
                throw;
            }

            try
            {
                var resxReader = ServiceContainer.Instance.GetService<IStringFileParseServiceList>().Select("RESX");

                if (cancelTokenSource != null)
                    cancelTokenSource.Token.ThrowIfCancellationRequested();

                // Target file may not exist, yet, so we do not load it but create it from scratch when saving.
                if (bLoadTarget == true)
                {
                    if (System.IO.File.Exists(sourcePath) == false)
                        throw new Exception(string.Format(LocultApp.Local.Strings.CantAccessFile_Error_Text, targetPath));

                    int iRowId = 0; // Load strings from target file

                    foreach (var item in resxReader.LoadFile(targetPath, cancelTokenSource))
                    {
                        if (cancelTokenSource != null)
                            cancelTokenSource.Token.ThrowIfCancellationRequested();

                        EntryDiffViewModel o;
                        dic.TryGetValue(item.Item1, out o); // Have we seen this key entry before ???

                        // Either construct a new entry or set target entries
                        if (o == null)
                        {
                            // Insert Key, value, comment strings into collection
                            var newItem = new EntryDiffViewModel(item.Item1, item.Item2, item.Item3, iRowId,
                                                                 TypeOfDiff.TargetOnly);
                            dic.Add(item.Item1, newItem);
                        }
                        else
                            o.SetTargetValueComment(item.Item2, item.Item3);

                        iRowId++;
                    }
                }

                // Doing it this way to keep GUI and backend thread seperated as long as possible...
                StringCollectionViewModel s = new StringCollectionViewModel(mDocumentBase);
                s.RebuildLogView(dic.Values);

                if (DiffSource != null)
                {
                    mFileSource.Dispose();
                    Interlocked.Exchange(ref mFileSource, null);
                }

                DiffSource = s;
                ////this.DiffSource.SelectAllElements();
            }
            catch (Exception)
            {
                cancelTokenSource.Cancel();
                throw;
            }
        }
        #endregion Load Two Files for Diff

        #region Save a file
        /// <summary>
        /// Save the source file of the string diff view.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public bool SaveSourceFile(string sourcePath)
        {
            SaveFile(sourcePath, ProcessDestination.Source);

            return true;
        }

        /// <summary>
        /// Save the target file of the string diff view.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public bool SaveTargetFile(string sourcePath)
        {
            SaveFile(sourcePath, ProcessDestination.Target);

            return true;
        }

        /// <summary>
        /// Save a source or target file of the diff view.
        /// </summary>
        /// <param name="paths">file path</param>
        public void SaveFile(string filePath, ProcessDestination fDestin)
        {
            Action<CancellationTokenSource> processingRequest;

            switch (fDestin)
            {
                case ProcessDestination.Source:
                    processingRequest = cts =>
                    {
                        if (mProcessItems.SetIsProcessingSolution(true, mSaveFileProcessID) == false)
                            throw new Exception(string.Format(Local.Strings.STR_BLOCKED_SOLUTION_TASK, mProcessItems.CurrentProcessId));

                        try
                        {
                            DiffSource.SaveSource(filePath);
                        }
                        finally
                        {
                            if (mProcessItems.SetIsProcessingSolution(false, mSaveFileProcessID) == false)
                                throw new Exception(string.Format(Local.Strings.STR_FAILED_RESET_OF_SOLUTION_TASK, mProcessItems.CurrentProcessId));
                        }
                    };
                    break;

                case ProcessDestination.Target:
                    processingRequest = cts =>
                    {
                        if (mProcessItems.SetIsProcessingSolution(true, mSaveFileProcessID) == false)
                            throw new Exception(string.Format(Local.Strings.STR_BLOCKED_SOLUTION_TASK, mProcessItems.CurrentProcessId));

                        try
                        {
                            DiffSource.SaveTarget(filePath);
                        }
                        finally
                        {
                            if (mProcessItems.SetIsProcessingSolution(false, mSaveFileProcessID) == false)
                                throw new Exception(string.Format(Local.Strings.STR_FAILED_RESET_OF_SOLUTION_TASK, mProcessItems.CurrentProcessId));
                        }
                    };
                    break;

                default:
                    throw new NotImplementedException(fDestin.ToString());
            }

            mProcessAction.StartCancelableProcess(processingRequest,
                  SaveFinishedEvent,
                    Local.Strings.STR_CANCEL_SAVE_MESSAGE);
        }

        /// <summary>
        /// This is a callback method that is always called when
        /// the internal load process is finished
        /// (even when it failed to finish after initialization).
        /// </summary>
        /// <param name="saveWasSuccessful"></param>
        private void SaveFinishedEvent(bool saveWasSuccessful,
                                       Exception exp, string message)
        {
            RaisePropertyChanged(() => FileSourceHasData);

            if (saveWasSuccessful == false)
            {
                if (mDocumentBase != null)
                    mDocumentBase.EndProcessingSolutionWithError(mSaveFileProcessID, exp, message);
                if (exp != null)
                    GetService<IMessageBoxService>().Show(exp, message);

                return;
            }

            if (mDocumentBase != null)
                mDocumentBase.TargetDocumentIsDirty(false, true); // This assumes that only target is editable!
        }
        #endregion Save a file

        #region translate
        /// <summary>
        /// Save a source or target file of the diff view.
        /// </summary>
        /// <param name="paths">file path</param>
        private void TranslateSelectedEntriesAsync(string sourceFileLanguage, string targetFileLanguage,
                                                   ProcessDestination destination = ProcessDestination.Target)
        {
            mProcessAction.StartCancelableProcess(
                cts =>
                {
                    if (mProcessItems.SetIsProcessingSolution(true, mTranslateProcessID) == false)
                        throw new Exception(string.Format(Local.Strings.STR_BLOCKED_SOLUTION_TASK, mProcessItems.CurrentProcessId));

                    try
                    {
                        var translator = GetService<ITranslator>();
                        var settings = GetService<ISettingsManager>();

                        // Check for known errors and return problem/resolution hints if available
                        var error = translator.Check(
                                  settings.Options.GetOptionValue<string>("Options", "MSTranslate.TranslationServiceUri"),
                                  settings.Options.GetOptionValue<SecureString>("Options", "MSTranslate.TranslationServiceUser"),
                                  settings.Options.GetOptionValue<SecureString>("Options", "MSTranslate.TranslationServicePassword"),
                                  sourceFileLanguage, targetFileLanguage, destination, cts);

                        if (error != null)
                            throw new TranslationException(error);

                        var login = translator.CreateLoginCredentials(
                                  new Uri(settings.Options.GetOptionValue<string>("Options", "MSTranslate.TranslationServiceUri")),
                                  settings.Options.GetOptionValue<SecureString>("Options", "MSTranslate.TranslationServiceUser"),
                                  settings.Options.GetOptionValue<SecureString>("Options", "MSTranslate.TranslationServicePassword"));

                        TranslateSelectedEntries(login, sourceFileLanguage, targetFileLanguage, destination, cts);

                        if (cts != null)
                            cts.Token.ThrowIfCancellationRequested();
                    }
                    finally
                    {
                        if (mProcessItems.SetIsProcessingSolution(false, mTranslateProcessID) == false)
                            throw new Exception(string.Format(Local.Strings.STR_FAILED_RESET_OF_SOLUTION_TASK, mProcessItems.CurrentProcessId));
                    }
                },
                TranslationFinishedEvent,
                    LocultApp.Local.Strings.STR_CANCEL_SAVE_MESSAGE);
        }

        /// <summary>
        /// This is a callback method that is always called when
        /// the internal load process is finished
        /// (even when it failed to finish after initialization).
        /// </summary>
        /// <param name="translationWasSuccessful"></param>
        private void TranslationFinishedEvent(bool translationWasSuccessful,
                                              Exception exp, string message)
        {
            DiffSource.SelectedItemBuffer = null;

            if (DiffSource.SelectedItems.Count == 1)
            {
                // Update deatails input view after changing selected item
                DiffSource.SelectedItemBuffer = DiffSource.SelectedItems[0];
            }

            if (translationWasSuccessful == false)
            {
                // Prefer build-in display over message box display
                if (mDocumentBase != null)
                    mDocumentBase.EndProcessingSolutionWithError(mTranslateProcessID, exp, message);
                else
                {
                    if (exp != null)
                      GetService<IMessageBoxService>().Show(exp, message);
                }

                return;
            }

            if (mDocumentBase != null)
                mDocumentBase.TargetDocumentIsDirty(true, false);
        }

        /// <summary>
        /// This method translates all selected entries
        /// from source to target <paramref name="destination"/>=Target
        /// or vice versa.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="sourceFileLanguage"></param>
        /// <param name="targetFileLanguage"></param>
        private void TranslateSelectedEntries(ILoginCredentials login,
                                             string sourceFileLanguage,
                                             string targetFileLanguage,
                                             ProcessDestination destination = ProcessDestination.Target,
                                             CancellationTokenSource cts = null)
        {
            foreach (EntryDiffViewModel diff in DiffSource.SelectedItems)
            {
                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                List<string> l = null;

                var translator = GetService<ITranslator>();

                switch (destination)
                {
                    case ProcessDestination.Source:
                        l = translator.GetTranslatedText(diff.TargetValue,
                                                        targetFileLanguage, sourceFileLanguage,
                                                        login);
                        break;

                    case ProcessDestination.Target:
                        l = translator.GetTranslatedText(diff.SourceValue,
                                                        sourceFileLanguage, targetFileLanguage,
                                                        login);
                        break;

                    default:
                        throw new NotImplementedException(destination.ToString());
                }

                string translation = string.Empty;
                string newLine = string.Empty;

                foreach (var item in l)
                {
                    translation += newLine + item;
                    newLine = "\n";
                }


                switch (destination)
                {
                    case ProcessDestination.Source:
                        diff.SetSourceValueComment(translation, (string.IsNullOrEmpty(diff.SourceComment) == true ?
                                                                 diff.TargetComment : diff.SourceComment));
                        break;

                    case ProcessDestination.Target:
                        diff.SetTargetValueComment(translation, (string.IsNullOrEmpty(diff.TargetComment) == true ?
                                                                 diff.SourceComment : diff.TargetComment));
                        break;

                    default:
                        throw new NotImplementedException(destination.ToString());
                }
            }

            // Update deatils input view after changing selected item
            DiffSource.SelectedItemBuffer = null;

            if (cts != null)
                cts.Token.ThrowIfCancellationRequested();

            if (DiffSource.SelectedItems.Count == 1)
            {
                // Update deatils input view after changing selected item
                DiffSource.SelectedItemBuffer = DiffSource.SelectedItems[0];
            }
        }
        #endregion translate

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
                    // Dispose of the curently used resources
                    if (mProcessAction != null)
                    {
                        mProcessAction.Dispose();
                        Interlocked.Exchange(ref mProcessAction, null);
                    }

                    if (mFileSource != null)
                    {
                        mFileSource.Dispose();
                        Interlocked.Exchange(ref mFileSource, null);
                    }

                    Interlocked.Exchange(ref mDocumentBase, null);
                    Interlocked.Exchange(ref mProcessItems, null);
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            //// base.Dispose(disposing);
        }

        /// <summary>
        /// Class finalizer
        /// </summary>
        ~StringCollectionDiffViewModel()
        {
            Dispose(true);
        }
        #endregion methods
    }
}
