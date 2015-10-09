namespace TranslationSolutionViewModelLib.ViewModels
{
    using System.Collections.ObjectModel;
    using TranslatorSolutionLib.Models;
    using System.Linq;
    using System;

    public class ProjectViewModel : Base.SolutionItemBaseViewModel, ISolutionItem
    {
        #region fields
        private readonly Project mProjectModel = null;
        private readonly ObservableCollection<ISolutionItem> mTargetFiles = null;
        private FileReferenceViewModel mSourceFile = null;
        private ISolutionItem mParent;
        private ItemExisits _Exists = ItemExisits.Unknown;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public ProjectViewModel(Project projectModel, ISolutionItem parent)
            : this()
        {
            mParent = parent;

            // Copy target file entries from model into ViewModel
            foreach (var item in projectModel.TargetFiles)
                mTargetFiles.Add(new FileReferenceViewModel(item, this));

            mProjectModel = projectModel;

            if (mProjectModel.SourceFile != null)
                mSourceFile = new FileReferenceViewModel(mProjectModel.SourceFile, this);
        }

        /// <summary>
        /// Standard Constructor.
        /// </summary>
        public ProjectViewModel()
        {
            mParent = null;
            mTargetFiles = new ObservableCollection<ISolutionItem>();

            mProjectModel = null;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets an enumeration based identifier that tells the type of object in an enumeratable way.
        /// </summary>
        public override TypeOfSolutionItem TypeOfItem
        {
            get
            {
                return TypeOfSolutionItem.Project;
            }
        }

        /// <summary>
        /// Gets a source file viewmodel for this project.
        /// </summary>
        public FileReferenceViewModel SourceFile
        {
            get
            {
                return mSourceFile;
            }
        }

        /// <summary>
        /// Gets the actual name of the item that is represented by this object.
        /// </summary>
        public string ItemName
        {
            get
            {
                try
                {
                    return (SourceFile == null ? string.Empty : System.IO.Path.GetFileName(SourceFile.Path));
                }
                catch
                {
                    return SourceFile.Path;
                }
            }

            set
            {
                if (SourceFile == null)
                    return;

                try
                {
                    var name = System.IO.Path.GetFileName(SourceFile.Path);

                    if (name != value)
                    {
                        var dir = System.IO.Path.GetDirectoryName(mSourceFile.Path);
                        SourceFile.Path = System.IO.Path.Combine(dir, value);

                        RaisePropertyChanged(() => SourceFile);
                        RaisePropertyChanged(() => ItemName);
                        RaisePropertyChanged(() => ItemPathName);
                        RaisePropertyChanged(() => Path);
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Gets the file system path (if any) of the project source file.
        /// </summary>
        public string Path
        {
            get
            {
                if (mSourceFile != null)
                    return mSourceFile.Path;

                return null;
            }
        }

        /// <summary>
        /// Gets the file system path and name (if any) of this item.
        /// </summary>
        public string ItemPathName
        {
            get
            {
                return Path;
            }
        }

        public string SourceFilePath
        {
            get
            {
                if (mSourceFile != null)
                {
                    try
                    {
                        return System.IO.Path.GetDirectoryName(SourceFile.Path);
                    }
                    catch
                    {
                    }
                }

                return null;
            }

            set
            {
                if (mSourceFile == null)
                    return;

                var dir = System.IO.Path.GetDirectoryName(SourceFile.Path);
                if (dir != value)
                {
                    var name = System.IO.Path.GetFileName(SourceFile.Path);
                    SourceFile.Path = System.IO.Path.Combine(value, name);
                    RaisePropertyChanged(() => Path);
                    RaisePropertyChanged(() => SourceFilePath);
                    RaisePropertyChanged(() => ItemTip);
                }
            }
        }

        /// <summary>
        /// Gets a comment (can be shown as tool tip) of the item that is represented by this object.
        /// </summary>
        public string ItemTip
        {
            get
            {
                return SourceFile.Comment;
            }
        }

        /// <summary>
        /// The child entries of a project are the target files.
        /// Therefore, this gets the list of target files for translation.
        /// </summary>
        public ObservableCollection<ISolutionItem> Children
        {
            get
            {
                return mTargetFiles;
            }
        }

        /// <summary>
        /// Gets a property to determine whether a file is actually exiosting or not.
        /// </summary>
        public ItemExisits Exists
        {
            get
            {
                return _Exists;
            }

            private set
            {
                if (_Exists != value)
                {
                    _Exists = value;
                    RaisePropertyChanged(() => Exists);
                }
            }
        }

        /// <summary>
        /// Searches for a given type of item among the childrens of this item.
        /// </summary>
        /// <param name="typeToSearch"></param>
        /// <returns></returns>
        public ISolutionItem SearchFirstItemByType(TypeOfSolutionItem typeToSearch)
        {
            if (Children != null)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i].TypeOfItem == typeToSearch)
                        return Children[i];

                    var typedChild = Children[i].SearchFirstItemByType(typeToSearch);
                    if (typedChild != null)
                        return typedChild;
                }
            }

            return null;
        }

        #endregion properties

        #region methods
        /// <summary>
        /// Gets the parent of a solution item. This parent link can be used to navigate the tree
        /// structure towards the root rather than just toward ist children
        /// </summary>
        public ISolutionItem GetParent()
        {
            return mParent;
        }

        /// <summary>
        /// Add a targe file to the collection of target files in viewmodel and underlying model.
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="updateFields"></param>
        public void AddTargetFile(FileReference fr, bool updateFields = true)
        {
            if (string.IsNullOrEmpty(fr.Path) == true)
                return;

            if (string.IsNullOrEmpty(fr.Type) == true)
                return;

            var frViewModel = new FileReferenceViewModel(fr, this);

            var item = Children.SingleOrDefault(p => string.Compare(p.Path, fr.Path, true) == 0);

            if (item == null)
            {
                mProjectModel.AddTargetFile(fr);
                Children.Add(frViewModel);
            }
            else
            {
                var fileRefViewModel = item as FileReferenceViewModel;

                if (fileRefViewModel != null)
                {
                    if (updateFields == true)
                    {
                        // Just adjust non key values if this item is already there
                        fileRefViewModel.Comment = fr.Comment;
                        fileRefViewModel.Type = fr.Type;
                    }
                }
            }
        }

        /// <summary>
        /// Remove a targe file from the collection of target files in viewmodel and underlying model.
        /// </summary>
        /// <param name="frViewModel"></param>
        /// <returns></returns>
        public bool RemoveTargetFile(FileReferenceViewModel frViewModel)
        {
            if (string.IsNullOrEmpty(frViewModel.ItemPathName) == true)
                return false;

            if (string.IsNullOrEmpty(frViewModel.Type) == true)
                return false;

            var item = Children.SingleOrDefault(p => string.Compare(p.Path, frViewModel.ItemPathName, true) == 0);

            if (item != null)
            {
                var frVMColl = item as FileReferenceViewModel;

                var model = frVMColl.GetModel();

                var projectModel = mProjectModel.TargetFiles.SingleOrDefault(p => string.Compare(p.Path, frViewModel.ItemPathName, true) == 0);

                // Remove model and viewmodel from collection of target files
                mProjectModel.RemoveTargetFile(projectModel.Path);
                Children.Remove(frVMColl);

                return true;
            }

            return false;
        }

        internal Project GetModel()
        {
            return mProjectModel;
        }

        /// <summary>
        /// Set a property to determine whether a file is actually exiosting or not.
        /// </summary>
        public void SetExists(ItemExisits newValue)
        {
            Exists = newValue;
        }
        #endregion methods
    }
}
