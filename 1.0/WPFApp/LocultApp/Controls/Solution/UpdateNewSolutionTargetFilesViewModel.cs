namespace LocultApp.Controls.Solution
{
    using LocultApp.Controls.Exception;
    using LocultApp.Controls.Solution.Base;
    using System.Collections.ObjectModel;
    using System.Linq;
    using TranslationSolutionViewModelLib.ViewModels;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// The object from this class controls the view that uis implemented to edit target file
    /// options when creating new solution/project content with the New Solution option in the start page.
    /// </summary>
    public class UpdateNewSolutionTargetFilesViewModel : Base.UpdateTargetFilesBaseViewModel
    {
        #region fields
        private readonly ObservableCollection<FileReferenceViewModel> mTargetFilesCollection = null;

        ICanForwardExceptionsToDisplay mExceptionDisplay;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public UpdateNewSolutionTargetFilesViewModel(ICanForwardExceptionsToDisplay exceptionDisplay)
        {
            mExceptionDisplay = exceptionDisplay;
            mTargetFilesCollection = new ObservableCollection<FileReferenceViewModel>();

            ClearData();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a collection of target files.
        /// </summary>
        public ObservableCollection<FileReferenceViewModel> TargetFilesCollection
        {
            get
            {
                return mTargetFilesCollection;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Call this function to generate sample data entries for
        /// simple start-up with meaningful sample data entries.
        /// </summary>
        public void InitTargetSampleData(string fileNamePath, string comment, string type)
        {
            AddTargetFileCommandExecuted(fileNamePath, comment, type);

            IsDirty = false;
        }

        /// <summary>
        /// Clear all data currently stored in this object
        /// </summary>
        internal void ClearData()
        {
            mTargetFilesCollection.Clear();
            base.mNewEditTarget = string.Empty;
            base.mNewEditTargetComment = string.Empty;
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
            if (string.IsNullOrEmpty(path) == true)
                return;

            if (string.IsNullOrEmpty(type) == true)
                return;

            AddTargetFileCommandExecuted(new FileReferenceViewModel(new FileReference(path, type, comment), null));
        }

        /// <summary>
        /// Adds new target file references into the collection of target files.
        /// </summary>
        /// <param name="fr"></param>
        internal void AddTargetFileCommandExecuted(FileReferenceViewModel fr,
                                                   bool updateFields = true)
        {
            mExceptionDisplay.ForwardExceptionToDisplay(null);
            try
            {
                if (string.IsNullOrEmpty(fr.Path) == true)
                    return;

                if (string.IsNullOrEmpty(fr.Type) == true)
                    return;

                IsValidFIlename(fr.Path);

                var item = mTargetFilesCollection.SingleOrDefault(p => string.Compare(p.Path, fr.Path, true) == 0);

                if (item == null)
                    mTargetFilesCollection.Add(new FileReferenceViewModel(fr));
                else
                {
                    if (updateFields == true)
                    {
                        // Just adjust non key values if this item is already there
                        item.Comment = fr.Comment;
                        item.Type = fr.Type;
                    }
                }

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

            if(mTargetFilesCollection.Remove(fileReferenceViewModel) == true)
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

            // Try to reset default path from collection of files if new path is empty
            if (TargetFilesCollection.Count > 0)
                filePath = TargetFilesCollection[0].ItemPathName;

            return filePath;
        }
        #endregion methods
    }
}
