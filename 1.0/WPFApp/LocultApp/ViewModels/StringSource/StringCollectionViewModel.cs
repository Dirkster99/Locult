namespace LocultApp.ViewModels.StringSource
{
    using ServiceLocator;
    using ServiceLocatorInterfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    public class StringCollectionViewModel : Base.ViewModelBase, IEntryDiffViewModelParent, IDisposable
    {
        #region fields
        private string mFileName = null;
        private ObservableCollection<EntryDiffViewModel> mRows;
        ////private readonly MultiSelectCollectionView<EntryDiffViewModel> mRowViewModels;

        private EntryDiffViewModel mSelectedItem;
        private EntryDiffViewModel mSelectedItemBuffer;
        private ObservableCollection<EntryDiffViewModel> mSelectedItems = new ObservableCollection<EntryDiffViewModel>();
        private bool mDisposed = false;

        private bool mIsDirty = false;
        private IStringCollectionDiffViewModelParent mDocumentBase = null;

        //private EvaluateLoadResult loadResultCallback = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor from file name
        /// </summary>
        /// <param name="fileName"></param>
        public StringCollectionViewModel(IStringCollectionDiffViewModelParent document)
        {
            mDocumentBase = document;
            mFileName = string.Empty;
            mRows = new ObservableCollection<EntryDiffViewModel>();

            // this.mRowViewModels = new MultiSelectCollectionView<EntryDiffViewModel>(this.mRows);
        }
        #endregion

        #region events
        public event EventHandler SelectedItemChanged;
        #endregion events

        #region properties
        /// <summary>
        /// Get/set file name of this string resource file.
        /// </summary>
        public string FileName
        {
            get
            {
                return mFileName;
            }

            set
            {
                if (mFileName != value)
                {
                    mFileName = value;
                    RaisePropertyChanged(() => SelectedItem);

                    OnSelectedItemChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// This property represents the datagrid viewmodel part and enables sorting and filtering
        /// being implemented in the viewmodel class.
        /// </summary>
        public CollectionView LogView { get; private set; }

        /// <summary>
        /// Get whether there are data items in the collection or not
        /// (there may be no items to display if filter is applied but thats a different issue)
        /// </summary>
        internal bool HasData
        {
            get
            {
                if (mRows != null)
                    return mRows.Count != 0;

                return false;
            }
        }

        /// <summary>
        /// LogItems property which is the main list of logitems
        /// (this property is bound to a view via CollectionView property)
        /// </summary>
        public ObservableCollection<EntryDiffViewModel> RowEntryViewModels
        {
            get
            {
                return mRows;
            }

            ////set
            ////{
            ////    this.mRowViewModels = value;
            ////}
        }

        /// <summary>
        /// SelectedItem Property
        /// </summary>
        public EntryDiffViewModel SelectedItem
        {
            get
            {
                return mSelectedItem;
            }

            set
            {
                if (mSelectedItem != value)
                {
                    mSelectedItem = value;
                    SelectedItemBuffer = new EntryDiffViewModel(value) { Parent = null };
                    RaisePropertyChanged(() => SelectedItem);

                    OnSelectedItemChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get/set selected item buffer which is used to make edits in a buffer
        /// and commit changes when user executes the corresponding command.
        /// </summary>
        public EntryDiffViewModel SelectedItemBuffer
        {
            get
            {
                return mSelectedItemBuffer;
            }

            set
            {
                if (mSelectedItemBuffer != value)
                {
                    mSelectedItemBuffer = value;
                    RaisePropertyChanged(() => SelectedItemBuffer);
                }
            }
        }

        /// <summary>
        /// Get set of currently selected items (when multiple items are selected).
        /// </summary>
        public ObservableCollection<EntryDiffViewModel> SelectedItems
        {
            get
            {
                return mSelectedItems;
            }
        }

        /// <summary>
        /// Get number of items currently selected.
        /// </summary>
        public int SelectedItemsCount
        {
            get
            {
                return mSelectedItems.Count;
            }
        }

        /// <summary>
        /// Gets/set the dirty state of a document.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return mIsDirty;
            }

            private set
            {
                if (mIsDirty != value)
                {
                    mIsDirty = value;
                    RaisePropertyChanged(() => IsDirty);

                    if (mDocumentBase != null && value == true)
                        mDocumentBase.TargetDocumentIsDirty(value, false);  // This assumes that only target is editable!
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// This method can be invoked to tell parent that details in this item have changed
        /// and may need synchronization with other views of the same detail.
        /// </summary>
        /// <param name="thisEntryHasChanged"></param>
        void IEntryDiffViewModelParent.UpdateDiffEntry(EntryDiffViewModel thisEntryHasChanged)
        {
            SelectedItemBuffer = new EntryDiffViewModel(SelectedItem) { Parent = null };
            IsDirty = true;
        }

        /// <summary>
        /// Keep the Actual selected logitems then refresh the view and reset the selected log item
        /// </summary>
        public void RefreshView()
        {
            EntryDiffViewModel l = SelectedItem;
            SelectedItem = null;
            if (LogView != null)
            {
                LogView.Refresh();
                RaisePropertyChanged(() => LogView);

                // Attempt to restore selected item if there was one before
                // and if it is not part of the filtered set of items
                // (ScrollItemBehaviour may scroll it into view when filter is applied)
                if (l != null)
                {
                    if (OnFilterLogItems(l))
                        SelectedItem = l;
                }
            }

            UpdateFilteredCounters(LogView);

            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Selects all rows in the list.
        /// </summary>
        public void SelectAllElements()
        {
            if (RowEntryViewModels == null)
                return;

            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            (Action)delegate
            {
                mSelectedItems.Clear();

                foreach (var item in RowEntryViewModels)
                {
                    mSelectedItems.Add(item);
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
                    Interlocked.Exchange(ref mRows, null);
                    Interlocked.Exchange(ref mSelectedItem, null);
                    Interlocked.Exchange(ref mSelectedItemBuffer, null);
                    Interlocked.Exchange(ref mSelectedItems, null);
                    Interlocked.Exchange(ref mDocumentBase, null);
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            //// base.Dispose(disposing);
        }

        protected void OnSelectedItemChanged(EventArgs e)
        {
            EventHandler handler = SelectedItemChanged;

            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Build a CollectionView on top of an observable collection.
        /// This is required to implemented Filter and Group Features
        /// for a DataGrid in an MVVM fashion.
        /// </summary>
        /// <param name="items"></param>
        internal void RebuildLogView(IEnumerable<EntryDiffViewModel> items)
        {
            // Note using the application dispatcher thread here will cause problems
            // when manipulating the observablecollection
            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            (Action)delegate
            {
                if (RowEntryViewModels != null)
                {
                    foreach (EntryDiffViewModel item in items)
                    {
                        item.Parent = this;
                        mRows.Add(item);
                    }
                }
                //// else
                ////    RowEntryViewModels = new MultiSelectCollectionView<EntryDiffViewModel>();

                LogView = (CollectionView)CollectionViewSource.GetDefaultView(RowEntryViewModels);
                LogView.Filter = OnFilterLogItems;

                RefreshView();
            });
        }

        /// <summary>
        /// Remove entries from main collection of entries.
        /// </summary>
        /// <param name="l"></param>
        internal void RemoveEntries(List<EntryDiffViewModel> l)
        {
            if (l == null)
                return;

            if (l.Count <= 0)               // Remove multiple items
                return;

            Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            (Action)delegate
            {
            foreach (var item in l)
                {
                    mRows.Remove(item);
                }
            });
        }

        private void UpdateFilteredCounters(ICollectionView view)
        {
            /***
            if (view != null)
            {
              IEnumerable<LogEntryRowViewModel> fltList = view.Cast<LogEntryRowViewModel>();
              if (fltList != null)
              {
                ItemsFilterCount = fltList.Count();

                ItemsDebugFilterCount = (from it in fltList
                                         where it.Entry.LevelIndex.Equals(LevelIndex.DEBUG)
                                         select it).Count();

                ItemsInfoFilterCount = (from it in fltList
                                        where it.Entry.LevelIndex.Equals(LevelIndex.INFO)
                                        select it).Count();

                ItemsWarnFilterCount = (from it in fltList
                                        where it.Entry.LevelIndex.Equals(LevelIndex.WARN)
                                        select it).Count();

                ItemsErrorFilterCount = (from it in fltList
                                         where it.Entry.LevelIndex.Equals(LevelIndex.ERROR)
                                         select it).Count();

                ItemsFatalFilterCount = (from it in fltList
                                         where it.Entry.LevelIndex.Equals(LevelIndex.FATAL)
                                         select it).Count();
              }
            }
            else
            {
              ItemsFilterCount = 0;
              ItemsDebugFilterCount = 0;
              ItemsInfoFilterCount = 0;
              ItemsWarnFilterCount = 0;
              ItemsErrorFilterCount = 0;
              ItemsFatalFilterCount = 0;
            }
             ***/
        }

        /// <summary>
        /// Determine if an item in the observable collection is to be filtered or not.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns true if item is not filtered and false otherwise</returns>
        private bool OnFilterLogItems(object item)
        {
            /***
              var logitemVm = item as LogEntryRowViewModel;

              if (logitemVm == null)
                return true; // Item is not filtered

              // Evaluate text filters if we are in filter mode, otherwise, display EVERY item!
              if (IsFiltered)
              {
                if (MatchTextFilterColumn(mDataGridColumns, logitemVm.Entry) == false)
                  return false;
                if (_filterViewModel.Evaluate(logitemVm.Entry) == false)
                  return false;
              }

              if (SelectAll == false)
              {
                switch (logitemVm.Entry.LevelIndex)
                {
                  case LevelIndex.DEBUG:
                    return mShowLevelDebug;

                  case LevelIndex.INFO:
                    return mShowLevelInfo;

                  case LevelIndex.WARN:
                    return mShowLevelWarn;

                  case LevelIndex.ERROR:
                    return mShowLevelError;

                  case LevelIndex.FATAL:
                    return mShowLevelFatal;
                }
              }
             **/

            return true;
        }

        internal void SetIsDirty(bool val)
        {
            IsDirty = val;
        }

        #region save methods
        /// <summary>
        /// Save source part of diff collection of diff to given file name.
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <returns></returns>
        internal bool SaveSource(string pathFileName)
        {
            if (pathFileName == null)
                return false;

            if (mRows == null)
                return false;

            //
            // Re-sort to save as close as possible to original order of entries
            //
            // Source: http://stackoverflow.com/questions/1101841/linq-how-to-perform-max-on-a-property-of-all-objects-in-a-collection-and-ret
            var itemMax = mRows.Max(y => y.SourceRowId);
            ////var itemsMax = this.RowEntryViewModels.Where(x => x.SourceRowId == itemMax);

            int maxRowID = itemMax + 1;
            SortedList<int, Tuple<string, string, string>> saveList = new SortedList<int, Tuple<string, string, string>>();

            foreach (var item in mRows)
            {
                if (item.IsSourceSet == false)
                {
                    if (item.Difference != Models.TypeOfDiff.SourceAndTarget)
                        continue;
                }   

                if (item.SourceRowId == -1)
                {
                    item.SourceRowId = maxRowID;
                    maxRowID++;
                }

                saveList.Add(item.SourceRowId, new Tuple<string, string, string>(item.Key,
                                                                                (string.IsNullOrEmpty(item.SourceValue) ? string.Empty
                                                                                                                        : item.SourceValue),

                                                                                (string.IsNullOrEmpty(item.SourceComment) ? string.Empty
                                                                                                                        : item.SourceComment)));
            }

            var resXSaver = ServiceContainer.Instance.GetService<IStringFileParseServiceList>().Select("RESX");

            // Save sorted entries 
            resXSaver.SaveFile(pathFileName, saveList.Values);

            return false;
        }

        /// <summary>
        /// Save target part of diff collection of diff to given file name.
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <returns></returns>
        internal bool SaveTarget(string pathFileName,
                                 CancellationTokenSource cts = null)
        {
            if (pathFileName == null)
                return false;

            if (RowEntryViewModels == null)
                return false;

            if (cts != null)
                cts.Token.ThrowIfCancellationRequested();

            // Re-sort to save as close as possible to original order of entries
            // Source: http://stackoverflow.com/questions/1101841/linq-how-to-perform-max-on-a-property-of-all-objects-in-a-collection-and-ret
            var itemMax = mRows.Max(y => y.TargetRowId);
            ////var itemsMax = this.RowEntryViewModels.Where(x => x.TargetRowId == itemMax);

            int maxRowID = itemMax + 1;
            SortedList<int, Tuple<string, string, string>> saveList = new SortedList<int, Tuple<string, string, string>>();

            foreach (var item in mRows)
            {
                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                if (item.IsTargetSet == false)
                {
                    if (item.Difference != Models.TypeOfDiff.SourceAndTarget)
                        continue;
                }   

                if (item.TargetRowId == -1)
                {
                    item.TargetRowId = maxRowID;
                    maxRowID++;
                }

                saveList.Add(item.TargetRowId, new Tuple<string, string, string>(item.Key,
                                                                                 (string.IsNullOrEmpty(item.TargetValue) ? string.Empty
                                                                                                                         : item.TargetValue),

                                                                                 (string.IsNullOrEmpty(item.TargetComment) ? string.Empty
                                                                                                                           : item.TargetComment)));
            }

            var resXSaver = ServiceContainer.Instance.GetService<IStringFileParseServiceList>().Select("RESX");

            if (cts != null)
                cts.Token.ThrowIfCancellationRequested();

            // Save sorted entries 
            resXSaver.SaveFile(pathFileName, saveList.Values, cts);

            return false;
        }
        #endregion save methods
        #endregion methods
    }
}
