namespace LocultApp.ViewModels.Pages.EditPageDocuments
{
    using ExplorerLib;
    using LocultApp;
    using LocultApp.Controls.Exception;
    using LocultApp.Controls.Solution;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Events;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces;
    using System;
    using System.Threading;
    using System.Windows.Input;
    using TranslationSolutionViewModelLib.ViewModels;


    /// <summary>
    /// This viewmodel manages control states of the <seealso cref="EditProjectDocument"/> view.
    /// This document view represent project properties and is shown when the user is supposed to
    /// edit/view these translation project properties.
    /// </summary>
    public class EditProjectDocumentViewModel : LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events.DocumentDirtyChangedViewModelBase,
                                                IEditSolutionDocument,
                                                IDefaultPath,
                                                ICanForwardExceptionsToDisplay
    {
        #region fields
        private UpdateProjectTargetFilesViewModel mUpdateTargetFiles;
        private ISolutionRoot mRoot;
        private ProjectViewModel mProject;
        private ICommand mBrowseSourceFileCommand;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public EditProjectDocumentViewModel(IProcessItems processItems,
                                            ApplicationRequestEventHandler requestedAction)
           : base(requestedAction)
        {
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a viewmodel object that can manage a target file collection.
        /// </summary>
        public UpdateProjectTargetFilesViewModel UpdateTargetFiles
        {
            get
            {
                return mUpdateTargetFiles;
            }

            private set
            {
                if (mUpdateTargetFiles != value)
                {
                    mUpdateTargetFiles = value;
                    RaisePropertyChanged(() => UpdateTargetFiles);
                }
            }
        }

        /// <summary>
        /// Gets the path and name of a project.
        /// </summary>
        public string SourceFilePathName
        {
            get
            {
                if (mProject != null)
                    return mProject.SourceFile.Path;

                return null;
            }

            set
            {
                if (mProject == null)
                    return;

                if (mProject.SourceFile.Path != value)
                {
                    mProject.SourceFile.Path = value;
                    RaisePropertyChanged(() => SourceFilePathName);
                    RaisePropertyChanged(() => SourceFileName);
                    SetSolutionIsDirty();
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets the name of a source file for this object.
        /// </summary>
        public string SourceFileName
        {
            get
            {
                if (mProject == null)
                    return null;

                return mProject.ItemName;
            }

            set
            {
                if (mProject == null)
                    return;

                try
                {
                    if (mProject.ItemName != value)
                    {
                        mProject.ItemName = value;
                        RaisePropertyChanged(() => SourceFileName);
                        SetSolutionIsDirty();
                    }
                }
                catch
                {
                }
            }

        }

        /// <summary>
        /// Gets the path of a source file for this object.
        /// </summary>
        public string SourceFilePath
        {
            get
            {
                if (mProject != null)
                    return mProject.SourceFilePath;

                return null;
            }

            set
            {
                if (mProject == null)
                    return;

                if (mProject.SourceFilePath != value)
                {
                    mProject.SourceFilePath = value;
                    RaisePropertyChanged(() => SourceFilePath);
                    RaisePropertyChanged(() => SourceFilePathName);
                    SetSolutionIsDirty();
                }
            }
        }

        /// <summary>
        /// Gets/sets an inofrmal text based comment for this object.
        /// </summary>
        public string Comment
        {
            get
            {
                if (mProject != null)
                    return mProject.SourceFile.Comment;

                return null;
            }

            set
            {
                if (mProject == null)
                    return;

                if (mProject.SourceFile.Comment != value)
                {
                    mProject.SourceFile.Comment = value;
                    RaisePropertyChanged(() => Comment);
                    SetSolutionIsDirty();
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets a command to select the source file of this project.
        /// </summary>
        public ICommand BrowseSourceFileCommand
        {
            get
            {
                if (mBrowseSourceFileCommand == null)
                {
                    mBrowseSourceFileCommand = new RelayCommand<object>((p) =>
                        {
                            var explorer = ServiceLocator.ServiceContainer.Instance.GetService<IExplorer>();

                            string nextFilePath = explorer.FileOpen(EditTranslationsDocumentViewModel.StringResourceFileFilter,
                                                                    SourceFilePathName,
                                                                    AppCore.MyDocumentsUserDir);

                            if (string.IsNullOrEmpty(nextFilePath) == false)
                            {
                                if (nextFilePath != SourceFilePath)
                                    SourceFilePath = nextFilePath;
                            }
                        });
                }

                return mBrowseSourceFileCommand;
            }
        }

        private ProjectViewModel Project
        {
            get
            {
                return mProject;
            }

            set
            {
                if (mProject != value)
                {
                    mProject = value;
                    RaisePropertyChanged(() => Project);
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
        public void InitDocument(ISolutionRoot root, ISolutionItem project)
        {
            Project = project as ProjectViewModel;
            if (Project == null)
            {
                throw new System.NotSupportedException("Only ProjectViewModel parameter is supported.");
            }

            mRoot = root;

            UpdateTargetFiles = new UpdateProjectTargetFilesViewModel(Project, this, this);

            mUpdateTargetFiles.DirtyFlagChangedEvent -= UpdateTargetFiles_DirtyFlagChangedEvent;

            //// Wrap this.SourceFilePathName => this.mProject.SourceFile.Path;
            //// this.Comment                 => this.mProject.SourceFile.Comment;

            IsDirty = mUpdateTargetFiles.IsDirty = false;
            mUpdateTargetFiles.DirtyFlagChangedEvent += UpdateTargetFiles_DirtyFlagChangedEvent;
        }

        string IDefaultPath.GetDefaultPath()
        {
            return SourceFilePath;
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
        protected override void Dispose(bool disposing)
        {
            if (mDisposed == false)
            {
                if (disposing == true)
                {
                    if (mUpdateTargetFiles != null)
                    {
                        // Dispose of the curently used resources
                        mUpdateTargetFiles.DirtyFlagChangedEvent -= UpdateTargetFiles_DirtyFlagChangedEvent;

                        mUpdateTargetFiles.Dispose();

                        // NUll 'em out boys
                        Interlocked.Exchange(ref mUpdateTargetFiles, null);
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

        // Disposable types implement a finalizer.
        ~EditProjectDocumentViewModel()
        {
            Dispose(false);
        }

        /// <summary>
        /// Method executes when a viewmodel that lives in this
        /// viewmodel report a change in its dirty state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTargetFiles_DirtyFlagChangedEvent(object sender, DocumentDirtyChangedEventArgs e)
        {
            // Make sure we keep dirty state if it is already there
            if (IsDirty == false)
            {
                IsDirty = e.IsDirtyNewValue;
            }

            SetSolutionIsDirty();
        }

        private void SetSolutionIsDirty()
        {
            if (mRoot != null)
                mRoot.SolutionIsDirty();
        }
        #endregion methods
    }
}
