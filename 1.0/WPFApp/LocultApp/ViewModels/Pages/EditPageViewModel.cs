namespace LocultApp.ViewModels.Pages
{
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Events;
    using LocultApp.ViewModels.Pages.EditPageDocuments;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces;
    using LocultApp.ViewModels.Pages.Interfaces;
    using MsgBox;
    using ServiceLocator;
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using TranslationSolutionViewModelLib.ViewModels;
    using TranslatorSolutionLib.Models;

    public enum RequestActionOnLoad
    {
        LoadItems = 0,
        None = 1
    }

    /// <summary>
    /// This viewmodel manages the main controls necessary to
    /// edit string translation in a side by side view with
    /// another project view to let users manage their string
    /// file projects inside the current solution.
    /// </summary>
    public class EditPageViewModel : PageBaseViewModel, IDisposable, IDocumentCollection, IDocumentCanUnload
    {
        #region fields
        private bool mDisposed = false;
        private readonly SolutionViewModel mSolution = null;

        private DocumentViewModelBase mCurrentEditPage = null;

        private bool _IsCurrentDocumentDirty = false;

        private ISolutionItem mSolutionSelectedItem;
        private ICommand mSolutionItemSelectedCommand;

        private IProcessItems mProcessItems;
        private ApplicationRequestEventHandler RequestedAction;

        private object lockObject = new object();
        private bool DocumentViewIsLoaded = false;
        private bool DocumentViewModelIsLoading = false;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor from parameter. The given solution is expected to contain at least 1 project.
        /// </summary>
        public EditPageViewModel(TranslatorSolution solutionModel,
                                 string solutionPath,
                                 string solutionName,
                                 IProcessItems processItems,
                                 ApplicationRequestEventHandler requestedAction)
            : this()
        {
            RequestedAction = requestedAction;

            mProcessItems = processItems;
            mSolution = new SolutionViewModel(solutionModel, solutionPath, solutionName);

            mSolutionSelectedItem = null;
            CurrentEditPage = null;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected EditPageViewModel()
        {
            this.ActionOnLoad = RequestActionOnLoad.None;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the size of an icon being displayed in the application.
        /// </summary>
	    public int IconSize
	    {
		    get
            {
                return GetService<ISettingsManager>().Options.GetOptionValue<int>("Options", "DefaultIconSize");
            }
	    }

        /// <summary>
        /// Gets the solution viewmodel that manages the solution of this edit page.
        /// </summary>
        public SolutionViewModel Solution
        {
            get
            {
                return mSolution;
            }
        }

        /// <summary>
        /// Gets the viewmodel of the current page being shown within the edit page
        /// (there are more than 1 pages possible since user can select a solution tree entry
        /// and the displayed content should change in dependence of that).
        /// </summary>
        public DocumentViewModelBase CurrentEditPage
        {
            get
            {
                return mCurrentEditPage;
            }

            private set
            {
                if (mCurrentEditPage != value)
                {
                    mCurrentEditPage = value;
                    RaisePropertyChanged(() => CurrentEditPage);
                }
            }
        }

        /// <summary>
        /// Gets/set The dirty state of this document. This property can
        /// be bound to in TreeView attached behaviour for navigation.
        /// </summary>
        public bool IsCurrentDocumentDirty
        {
            get
            {
                return _IsCurrentDocumentDirty;
            }

            set
            {
                if (_IsCurrentDocumentDirty != value)
                {
                    _IsCurrentDocumentDirty = value;
                    RaisePropertyChanged(() => IsCurrentDocumentDirty);
                }
            }
        }

        /// <summary>
        /// Gets the currently selected item in the tree list of solution items.
        /// </summary>
        public ISolutionItem SolutionSelectedItem
        {
            get
            {
                return mSolutionSelectedItem;
            }

            set
            {
                if (mSolutionSelectedItem != value)
                {
                    mSolutionSelectedItem = value;
                    RaisePropertyChanged(() => SolutionSelectedItem);
                }
            }
        }

        /// <summary>
        /// Get/set command to select the current folder.
        /// Command is executed when SelectedItem of the solution
        /// is about to change via event handling.
        /// </summary>
        public ICommand SolutionItemSelectedCommand
        {
            get
            {
                if (mSolutionItemSelectedCommand == null)
                {
                    mSolutionItemSelectedCommand = new RelayCommand<object>(p =>
                    {
                        try
                        {
                            ISolutionItem item = null;

                            // Do not execute this if viewmodel selects the item and view reacts to it
                            // cause this in turn to react ...
                            if (IsDocumentViewLoaded() == false)
                                return;

                            var param = p as System.Windows.RoutedPropertyChangedEventArgs<object>;
                            if (param != null)
                                item = param.NewValue as ISolutionItem;
                            else
                                item = p as ISolutionItem;

                            SetSelectedSolutionItem(item);
                        }
                        catch (Exception exp)
                        {
                            var msg = ServiceContainer.Instance.GetService<IMessageBoxService>();
                            msg.Show(exp);
                        }
                    });
                }

                return mSolutionItemSelectedCommand;
            }
        }

        public RequestActionOnLoad ActionOnLoad { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// method executes via interface call when the attached view is unloaded.
        /// </summary>
        void IDocumentCanUnload.OnViewUnloaded()
        {
            lock (lockObject)
            {
                DocumentViewIsLoaded = false;
            }
        }

        /// <summary>
        /// method executes via interface call when the attached view is loaded.
        /// </summary>
        void IDocumentCanUnload.OnViewLoaded()
        {
            lock (lockObject)
            {
                DocumentViewIsLoaded = true;

                if (this.ActionOnLoad == RequestActionOnLoad.LoadItems)
                {
                    this.ActionOnLoad = RequestActionOnLoad.None;

                    var pageViewmodel = CurrentEditPage as EditTranslationsDocumentViewModel;
                    if (pageViewmodel != null)
                    {
                        ISolutionItem selectedParent = null;

                        if (SolutionSelectedItem != null)
                            selectedParent = SolutionSelectedItem.GetParent();

                        if (selectedParent != null)
                        {
                            LoadEditPage(pageViewmodel,
                                         selectedParent.ItemPathName,
                                         SolutionSelectedItem.ItemPathName);
                        }
                    }
                }
            }
        }

        private void LoadEditPage(EditTranslationsDocumentViewModel pageViewmodel,
                                  string sourceFilePath, string targetPath)
        {
            pageViewmodel.LoadEditPage(sourceFilePath, targetPath);

            // Attach this dirty flag only where navigation away from this item
            // would cause data lose via document edit -> Solution dirty flag is handled somewhere else
            IsCurrentDocumentDirty = (CurrentEditPage != null ? CurrentEditPage.IsDirty : false);
            pageViewmodel.DirtyFlagChangedEvent += CurrentEditPage_DirtyFlagChangedEvent;
        }

        /// <summary>
        /// Gets a collection of DIRTY documents that require saving (if any).
        /// A document is in this context a list of translations or a solution.
        /// 
        /// The returned list is empty if there are no dirty documents.
        /// </summary>
        /// <returns></returns>
        public IList<IDirtyDocument> GetDirtyDocuments()
        {
            List<IDirtyDocument> list = new List<IDirtyDocument>();

            // This implementation assumes that only the target file can be dirty if this document viewmodel is dirty
            // Editing Source and Target and making sure to save their data requires a redesign...
            if (CurrentEditPage != null)
            {
                if (CurrentEditPage is EditTranslationsDocumentViewModel)
                {
                    // Check whether a document has any dirty item that requires saving to avoid data loss
                    if (IsCurrentDocumentDirty == true)
                    {
                        string targetFile = null;

                        try
                        {
                            var editTranslation = CurrentEditPage as EditTranslationsDocumentViewModel;
                            targetFile = editTranslation.StringDiff.TargetFilePath;
                        }
                        catch
                        {
                        }

                        if (targetFile != null)
                            list.Add(new DirtyDocument(targetFile, true));
                    }
                }

                // Check whether solution has any dirty items that require saving to avoid data loss
                if (mSolution != null)
                {
                    if (mSolution.Root.IsDirty == true)
                    {
                        list.Add(new DirtyDocument(mSolution.Root.ItemPathName, true));
                    }
                }
            }

            return list;
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
                    DisposeCurrentEditPage();

                    Interlocked.Exchange(ref mProcessItems, null);
                    Interlocked.Exchange(ref RequestedAction, null);
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
        /// Executes when the dirty flag of the current document changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentEditPage_DirtyFlagChangedEvent(object sender, DocumentDirtyChangedEventArgs e)
        {
            IsCurrentDocumentDirty = e.IsDirtyNewValue;
        }

        /// <summary>
        /// Creates a new viewmodel for a given combination of parent and child <seealso cref="ISolutionItem"/>s.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        private DocumentViewModelBase CreateDocumentPageViewModel(ISolutionItem parent,
                                                                  ISolutionItem child,
                                                                  IProcessItems processItems)
        {
            if (child is ProjectViewModel)
                return new EditProjectDocumentViewModel(processItems, RequestedAction);

            if (child is SolutionRootViewModel)
                return new EditSolutionDocumentViewModel(processItems);

            // Is a target file under a project currently selected?
            var project = parent as ProjectViewModel;
            var fileref = child as FileReferenceViewModel;

            if (project != null && fileref != null)
            {
                var vm = new EditTranslationsDocumentViewModel(processItems);

                var optionsEngine = GetService<ISettingsManager>().Options;
                var group = optionsEngine.GetOptionGroup("Options");

                vm.InitializeSettings(group);

                return vm;

            }

            throw new System.NotImplementedException(string.Format("No default view available for {0} with child: {1}",
                                                     (parent == null ? "(null)" : parent.GetType().ToString()),
                                                     (child == null ? "(null)" : child.GetType().ToString())));
        }

        /// <summary>
        /// Dispose the current viewmodel of the current
        /// editpage and set the corresponding property to null.
        /// </summary>
        private void DisposeCurrentEditPage()
        {
            if (CurrentEditPage != null)
            {
                var page = CurrentEditPage as DocumentDirtyChangedViewModelBase;
                if (page != null)
                    page.DirtyFlagChangedEvent -= CurrentEditPage_DirtyFlagChangedEvent;
            }

            var currentPage = CurrentEditPage as IDisposable;

            // Dispose of the curently used resources and null'em out
            if (currentPage != null)
                currentPage.Dispose();

            Interlocked.Exchange(ref mCurrentEditPage, null);

            IsCurrentDocumentDirty = false;
        }

        private void SetSelectedSolutionItem(ISolutionItem item)
        {
            if (item == null)
                return;

            SolutionSelectedItem = item;
            DisposeCurrentEditPage();

            // Create new document according to new selection
            var parent = SolutionSelectedItem.GetParent();

            CurrentEditPage = CreateDocumentPageViewModel(parent, SolutionSelectedItem, this.mProcessItems);

            if (CurrentEditPage is EditSolutionDocumentViewModel ||
                CurrentEditPage is EditProjectDocumentViewModel)
            {
                // Create a new DocumentPageViewModel to edit SOLUTION or PROJECT properties
                var pageViewmodel = CurrentEditPage as IEditSolutionDocument;
                pageViewmodel.InitDocument(mSolution.Root, SolutionSelectedItem);
            }
            else
            {
                // Load source and target files belonging to this project (if any)
                // Is current document a EditTranslationsDocumentViewModel?
                // Then try to load default data for it...
                if (CurrentEditPage is EditTranslationsDocumentViewModel)
                {
                    // Create a new DocumentPageViewModel
                    var pageViewmodel = CurrentEditPage as EditTranslationsDocumentViewModel;

                    LoadEditPage(pageViewmodel, parent.Path, SolutionSelectedItem.Path);
                }
                else
                {
                    throw new System.NotImplementedException(string.Format("No default view available for {0} with parent: {1}",
                                                                mSolutionSelectedItem.GetType(),
                                                                (parent == null ? "(null)" : parent.GetType().ToString())));
                }
            }
        }

        /// <summary>
        /// Gets whether this viewmodel is currently loaded  as active document
        /// for viewing/editing or not.
        /// </summary>
        /// <returns></returns>
        private bool IsDocumentViewLoaded()
        {
            lock (lockObject)
            {
                return this.DocumentViewIsLoaded;
            }
        }
        #endregion methods

        #region private class
        /// <summary>
        /// Perform default selection to get users started without an additional extra click
        /// </summary>
        internal void PerformDefaultSeleciton()
        {
            mSolutionSelectedItem = null;
            ISolutionItem selectItemParent = null;
            ISolutionItem selectItem = null;

            if (mSolution.Children.Count > 0)
            {
                var root = mSolution.Children[0] as ISolutionItem;

                if (root != null)
                {
                    root.IsExpanded = true;
                    selectItem = root;

                    if (root.Children.Count > 0)
                    {
                        // Get first project
                        var item = root.SearchFirstItemByType( TypeOfSolutionItem.Project );

                        if (item != null)
                        {
                            if (item.Exists == ItemExisits.DoesExist)
                            {
                                selectItemParent = selectItem.GetParent();
                                selectItem = item;
                            }

                            item.IsExpanded = true;
                            var expandedItem = item.GetParent();
                            while (expandedItem != null)
                            {
                                expandedItem.IsExpanded = true;
                                expandedItem = expandedItem.GetParent();
                            }

                            item = selectItem.SearchFirstItemByType(TypeOfSolutionItem.File);

                            if (item != null)
                            {
                                if (item.Exists == ItemExisits.DoesExist)
                                {
                                    selectItemParent = selectItem.GetParent();
                                    selectItem = item;
                                }

                                item.IsExpanded = true;
                                expandedItem = item.GetParent();
                                while (expandedItem != null)
                                {
                                    expandedItem.IsExpanded = true;
                                    expandedItem = expandedItem.GetParent();
                                }
                            }
                        }
                    }
                }

                if (selectItem != null)
                {
                    selectItem.IsSelected = true;
                    SolutionSelectedItem = selectItem;
                }
            }

            CurrentEditPage = CreateDocumentPageViewModel(selectItem.GetParent(), selectItem, this.mProcessItems);

            var pageViewmodel = CurrentEditPage as EditTranslationsDocumentViewModel;

            if (pageViewmodel != null)
                this.ActionOnLoad = RequestActionOnLoad.LoadItems;

            // Load source and target files belonging to this project (if any)
            if (CurrentEditPage != null && selectItemParent != null && pageViewmodel != null)
            {
                if (selectItemParent.TypeOfItem == TypeOfSolutionItem.Project)
                {
                    var currentPage = CurrentEditPage as DocumentDirtyChangedViewModelBase;
                    if (currentPage != null)
                    {
                        // Is current document a EditTranslationsDocumentViewModel?
                        // Then try to load default data for it...
                        currentPage.DirtyFlagChangedEvent += CurrentEditPage_DirtyFlagChangedEvent;
                        ////pageViewmodel.LoadEditPage(project.SourceFile.Path, child.Path);
                    }
                }
            }
        }

        private class DirtyDocument : IDirtyDocument
        {
            public DirtyDocument(string filePathName, bool isDirty)
            {
                FilePathName = filePathName;
                IsDirty = isDirty;
            }

            public string FilePathName { get; private set; }

            public bool IsDirty { get; set; }
        }

        /// <summary>
        /// Goes through each file referenced in the solution and determines
        /// whether its present or not. This results in an update of the
        /// <seealso cref="ItemExisits"/> property of each item. A converter
        /// can then be bound to each items <seealso cref="ItemExisits"/> property
        /// to visualize the current state of the corresponding file to the user.
        /// </summary>
        /// <returns></returns>
        internal Task<bool> CheckSolutionState()
        {
            return Task.Factory.StartNew<bool>(() =>
            {
                if (this.mSolution == null)
                    return false;

                foreach (var item in this.mSolution.Children)
                    CheckSolutionState(item);

                return true;
            });
        }

        internal void CheckSolutionState(ISolutionItem item)
        {
            if (item == null)
                return;

            var itemExists = item as ISolutionItemExists;
            if (itemExists != null)
            {
                if (System.IO.File.Exists(item.ItemPathName) == true)
                    itemExists.SetExists(ItemExisits.DoesExist);
                else
                    itemExists.SetExists(ItemExisits.DoesNotExist);
            }

            foreach (var item1 in item.Children)
            {
                CheckSolutionState(item1);
            }
        }
        #endregion private class
    }
}
