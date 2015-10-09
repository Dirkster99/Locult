namespace LocultApp.ViewModels.Pages.SettingsPages
{
    using FolderBrowser;
    using FolderBrowser.Dialogs.Interfaces;
    using FolderBrowser.FileSystem.Interfaces;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events;
    using Settings.Interfaces;
    using SettingsModel.Interfaces;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Class manages all viemodel properties and methids necessary to edit a colleciton of path bookmarks.
    /// </summary>
    public class EditBookmarksViewModel : SettingsPageBaseViewModel
    {
        #region fields
        private IBookmarkedLocationsViewModel mBookmarkedLocation = null;
        private IDropDownViewModel mDropDownBrowser = null;

        private string mBookmarkSelected;
        private ICommand mAddFolderCommand;
        private ICommand mRemoveFolderCommand;
        private ICommand mUpFolderCommand;
        private ICommand mDownFolderCommand;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="settingsManager"></param>
        public EditBookmarksViewModel(string displayTitle)
            : base(displayTitle)
        {
            Bookmarks = new ObservableCollection<string>();
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

        /// <summary>
        /// Gets a collection of folder bookmarks
        /// </summary>
        public ObservableCollection<string> Bookmarks { get; private set; }

        /// <summary>
        /// Gets/sets the currently selected path to a folder.
        /// </summary>
        public string BookmarkSelected
        {
            get
            {
                return mBookmarkSelected;
            }

            set
            {
                if (mBookmarkSelected != value)
                {
                    mBookmarkSelected = value;
                    RaisePropertyChanged(() => BookmarkSelected);

                    ////IsDirty = true; -> Changing the selected item does not result in a real change
                    //// Changing the value of the selected item results in a change :-)
                }
            }
        }

        /// <summary>
        /// Gets a command that expects a string commandparemter
        /// and adds that string from the collection of bookmarked
        /// folders.
        /// </summary>
        public ICommand AddFolderCommand
        {
            get
            {
                if (mAddFolderCommand == null)
                {
                    mAddFolderCommand = new RelayCommand<object>((p) =>
                    {
                        string path = p as string;

                        if (path == null)
                            return;

                        var items = Bookmarks.SingleOrDefault(item => string.Compare(item, path, true) == 0);

                        // Make list of paths a list of unique paths
                        if (items != null)
                        {
                            if (items.Count() > 0)
                                return;
                        }

                        Bookmarks.Add(path);
                        IsDirty = true;
                    });
                }

                return mAddFolderCommand;
            }
        }

        /// <summary>
        /// Gets a command that expects a string commandparemter
        /// and removes that string from the collection of bookmarked
        /// folders.
        /// </summary>
        public ICommand RemoveFolderCommand
        {
            get
            {
                if (mRemoveFolderCommand == null)
                {
                    mRemoveFolderCommand = new RelayCommand<object>((p) =>
                    {
                        string path = p as string;

                        if (path == null)
                            return;

                        Bookmarks.Remove(path);
                        IsDirty = true;
                    });
                }

                return mRemoveFolderCommand;
            }
        }

        public ICommand DownFolderCommand
        {
            get
            {
                if (mDownFolderCommand == null)
                {
                    mDownFolderCommand = new RelayCommand<object>((p) =>
                    {
                        string path = p as string;

                        if (path == null)
                            return;

                        int oldindex = Bookmarks.IndexOf(path);
                        if (oldindex < Bookmarks.Count - 1)
                        {
                            Bookmarks.Remove(path);
                            Bookmarks.Insert(oldindex + 1, path);
                            BookmarkSelected = path;
                            IsDirty = true;
                        }
                    });
                }

                return mDownFolderCommand;
            }
        }

        public ICommand UpFolderCommand
        {
            get
            {
                if (mUpFolderCommand == null)
                {
                    mUpFolderCommand = new RelayCommand<object>((p) =>
                    {
                        string path = p as string;

                        if (path == null)
                            return;

                        int oldindex = Bookmarks.IndexOf(path);
                        if (oldindex > 0)
                        {
                            Bookmarks.Remove(path);
                            Bookmarks.Insert(oldindex - 1, path);
                            BookmarkSelected = path;
                            IsDirty = true;
                        }
                    });
                }

                return mUpFolderCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Reset the view model to those options that are going to be presented for editing.
        /// </summary>
        /// <param name="settingData"></param>
        public override void ApplyOptionsFromModel(IEngine optionsEngine)
        {
            var group = optionsEngine.GetOptionGroup("Options");

            Bookmarks.Clear();
            EditBookmarksViewModel.LoadOptionsFromModel(group, Bookmarks);

            if (Bookmarks.Count > 0)
                BookmarkSelected = Bookmarks[0];

            // Construct bookmark collection and folder browser viewmodels
            BookmarkedLocations = this.ConstructBookmarks();
            DropDownBrowser = InitializeDropDownBrowser(BookmarkSelected);
        }

        /// <summary>
        /// Save changed settings back to model for further
        /// application and persistence in file system.
        /// </summary>
        /// <param name="settingData"></param>
        public override void SaveOptionsToModel(IEngine optionsEngine)
        {
            var group = optionsEngine.GetOptionGroup("Options");

            SaveOptionsToModel(group);
        }

        /// <summary>
        /// Reset the view model to those options that are going to be presented for editing.
        /// </summary>
        /// <param name="settingData"></param>
        internal static void LoadOptionsFromModel(IOptionGroup optGroup, IList collection)
        {
            var opt = optGroup.GetOptionDefinition("BookmarkedFolders");

            foreach (var item in opt.List_GetListOfKeyValues())
            {
                string sValue = item.Value as string;

                if (sValue != null)
                    collection.Add(sValue);
            }
        }

        internal static void LoadBookMarkOptionsFromModel(IEngine options
                                                         ,IBookmarkedLocationsViewModel ret)
        {
            var optGroup = options.GetOptionGroup("Options");
            var opt = optGroup.GetOptionDefinition("BookmarkedFolders");

            foreach (var item in opt.List_GetListOfKeyValues())
            {
                string sValue = item.Value as string;

                if (sValue != null)
                    ret.AddFolder(sValue);
            }
        }

        /// <summary>
        /// Constructs a few initial entries for
        /// the recent folder collection that implements folder bookmarks.
        /// </summary>
        /// <returns></returns>
        private IBookmarkedLocationsViewModel ConstructBookmarks()
        {
            IBookmarkedLocationsViewModel ret = FolderBrowserFactory.CreateReceentLocationsViewModel();

            // Copy items from current bookmark collection into bookmark collection of folder browser
            foreach (var item in Bookmarks)
                ret.AddFolder(item);

            if (ret.DropDownItems.Count > 0)
                ret.SelectedItem = ret.DropDownItems[0];

            return ret;
        }

        private string UpdateCurrentPath()
        {
            return this.BookmarkSelected;
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
                treeBrowser.InitialPath = this.BookmarkSelected;

            treeBrowser.SetSpecialFoldersVisibility(true);

            var dlgVM = FolderBrowserFactory.CreateDropDownViewModel(treeBrowser, BookmarkedLocations, this.DropDownClosedResult);

            dlgVM.UpdateInitialPath = this.UpdateCurrentPath;
            dlgVM.UpdateInitialBookmarks = this.UpdateBookmarks;

            dlgVM.ButtonLabel = Local.Strings.STR_SELECT_A_FOLDER;

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
                this.BookmarkSelected = selectedPath;
            }
        }

        /// <summary>
        /// Save changed settings back to model for further
        /// application and persistence in file system.
        /// </summary>
        /// <param name="settingData"></param>
        private void SaveOptionsToModel(IOptionGroup optionsOptionGroup)
        {
            var schema = optionsOptionGroup.GetOptionDefinition("BookmarkedFolders");
            schema.List_Clear();

            if (Bookmarks.Count > 0)
            {
                foreach (var item in Bookmarks)
                    schema.List_AddValue(item, item);
            }
        }

        internal static bool SaveBookmarksToModel(IEnumerable<IFSItemViewModel> collection,
                                                  IOptionGroup optionsOptionGroup)
        {
            var schema = optionsOptionGroup.GetOptionDefinition("BookmarkedFolders");

            bool listsAreEqual = true;
            int iCount = 0;
            foreach (var item in collection)
            {
                object val;
                if (schema.List_TryGetValue(item.FullPath, out val) == false)
                {
                    listsAreEqual = false;
                    break;
                }

                iCount++;
            }

            if (listsAreEqual == true)
            {
                if (iCount == collection.Count())
                    return false;
            }

            // Clear list and copy all values since they differ
            schema.List_Clear();
            foreach (var item in collection)
                schema.List_AddValue(item.FullPath, item.FullPath);

            optionsOptionGroup.SetUndirty(true);

            return true;
        }
        #endregion methods
    }
}
