namespace LocultApp.Controls.Solution
{
    using LocultApp.Controls.Exception;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces;
    using System.Collections.ObjectModel;
    using System.Threading;
    using TranslationSolutionViewModelLib.ViewModels;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// The object from this class controls the view that is implemented to edit target file
    /// options when reviewing/updating project related target files content in the
    /// Project document of the Edit page.
    /// </summary>
    public class UpdateProjectTargetFilesViewModel : Base.UpdateTargetFilesBaseViewModel
    {
        #region fields
        private ProjectViewModel mProject;
        private IDefaultPath mDefaultPath;
        private ICanForwardExceptionsToDisplay mExceptionDisplay;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public UpdateProjectTargetFilesViewModel(ProjectViewModel project,
                                                 ICanForwardExceptionsToDisplay exceptionDisplay,
                                                 IDefaultPath defaultPath = null)
        {
            mProject = project;
            mExceptionDisplay = exceptionDisplay;
            mDefaultPath = defaultPath;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a collection of target files.
        /// </summary>
        public ObservableCollection<ISolutionItem> TargetFilesCollection
        {
            get
            {
                return mProject.Children;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Clear all data currently stored in this object
        /// </summary>
        internal void ClearData()
        {
            mProject.Children.Clear();
            mProject.SourceFile.Path = string.Empty;
            mProject.SourceFile.Comment = string.Empty;
            IsDirty = false;
        }

        /// <summary>
        /// Adds new target file references into the collection of target files.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="comment"></param>
        /// <param name="type"></param>
        protected override void AddTargetFileCommandExecuted(string path, string comment, string type,
                                                             bool updateFields = true)
        {
            try
            {
                mExceptionDisplay.ForwardExceptionToDisplay(null);

                if (string.IsNullOrEmpty(path) == true)
                    return;

                if (string.IsNullOrEmpty(type) == true)
                    return;

                IsValidFIlename(path);

                mProject.AddTargetFile(new FileReference(path, type, comment));
                IsDirty = true;
            }
            catch (System.Exception exp)
            {
                mExceptionDisplay.ForwardExceptionToDisplay(exp);
            }
        }

        /// <summary>
        /// Remove the given item from the colleciton of target files.
        /// </summary>
        /// <param name="fileReferenceViewModel"></param>
        /// <returns></returns>
        protected override bool RemoveTargetFileCommandExecuted(FileReferenceViewModel fileReferenceViewModel)
        {
            if (fileReferenceViewModel == null)
                return false;

            if (mProject.RemoveTargetFile(fileReferenceViewModel) == true)
            {
                IsDirty = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the path and file name of the first item in the colleciton of target files.
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultTargetFilePath()
        {
            string filePath = string.Empty;
            mExceptionDisplay.ForwardExceptionToDisplay(null);
            try
            {
                // Try to reset default path from collection of files if new path is empty
                if (TargetFilesCollection.Count > 0)
                    filePath = TargetFilesCollection[0].ItemPathName;
                else
                {
                    if (mDefaultPath != null)
                    {
                        filePath = mDefaultPath.GetDefaultPath();
                    }
                }

                return filePath;
            }
            catch (System.Exception exp)
            {
                mExceptionDisplay.ForwardExceptionToDisplay(exp);
            }

            return filePath;
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (mDisposed == false)
            {
                // not disposing these since they are references from parent items
                Interlocked.Exchange(ref mDefaultPath, null);
                Interlocked.Exchange(ref mProject, null);
                Interlocked.Exchange(ref mExceptionDisplay, null);
            }

            base.Dispose(disposing);
        }
        #endregion methods
    }
}
