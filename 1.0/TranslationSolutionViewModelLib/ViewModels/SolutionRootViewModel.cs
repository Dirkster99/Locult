namespace TranslationSolutionViewModelLib.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// Represents the root viewmodel element os a solution. This element
    /// can have a path and file name and project viewmodels as direct children.
    /// </summary>
    public class SolutionRootViewModel : Base.SolutionItemBaseViewModel, ISolutionRoot
    {
        #region fields
        private readonly TranslatorSolution mSolutionModel;
        private readonly ObservableCollection<ISolutionItem> mChildren;
        private string mSolutionPath;
        private bool mIsDirty;
        private ItemExisits _Exists = ItemExisits.Unknown;
        #endregion fields

        #region contructors
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public SolutionRootViewModel()
        {
            mChildren = new ObservableCollection<ISolutionItem>();
            mSolutionModel = new TranslatorSolution();

            mSolutionPath = string.Empty;
            ItemName = string.Empty;
            mIsDirty = false;
        }
        #endregion contructors

        #region properties
        /// <summary>
        /// Gets an enumeration based identifier that tells the type of object in an enumeratable way.
        /// </summary>
        public override TypeOfSolutionItem TypeOfItem
        {
            get
            {
                return TypeOfSolutionItem.Root;
            }
        }

        /// <summary>
        /// Gets the actual name of the item that is represented by this object.
        /// </summary>
        public string ItemName
        {
            get
            {
                return mSolutionModel.Name;
            }

            set
            {
                if (mSolutionModel.Name != value)
                {
                    mSolutionModel.SetName(value);
                    RaisePropertyChanged(() => ItemName);
                    RaisePropertyChanged(() => ItemPathName);
                }
            }
        }

        /// <summary>
        /// Gets the file system path (if any) of this item.
        /// </summary>
        public string Path
        {
            get
            {
                return mSolutionPath;
            }

            set
            {
                if (mSolutionPath != value)
                {
                    mSolutionPath = value;
                    RaisePropertyChanged(() => Path);
                    RaisePropertyChanged(() => ItemPathName);
                }
            }
        }

        /// <summary>
        /// Gets the file system path and name (if any) of this item.
        /// </summary>
        public string ItemPathName
        {
            get
            {
                return Path + System.IO.Path.DirectorySeparatorChar + ItemName;
            }
        }

        /// <summary>
        /// Gets a comment (can be shown as tool tip) of the item that is represented by this object.
        /// </summary>
        public string ItemTip
        {
            get
            {
                string tip = string.Empty;

                if (string.IsNullOrEmpty(mSolutionModel.Comment) == false)
                    tip = mSolutionModel.Comment;

                return tip;
            }
        }

        /// <summary>
        /// Gets/sets an informal text based comment for this object.
        /// </summary>
        public string Comment
        {
            get
            {
                return mSolutionModel.Comment;
            }

            set
            {
                if (mSolutionModel.Comment != value)
                {
                    mSolutionModel.SetComment(value);
                    RaisePropertyChanged(() => Comment);
                    RaisePropertyChanged(() => ItemTip);
                }
            }
        }

        /// <summary>
        /// Gets a list of sub-children (or null if there are no children) for this item.
        /// </summary>
        public ObservableCollection<ISolutionItem> Children
        {
            get
            {
                return mChildren;
            }
        }

        /// <summary>
        /// Indicates whether the solution has unsafed edits or not.
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
                }
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
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the parent of a solution item. This parent link can be used to navigate the tree
        /// structure towards the root rather than just toward ist children.
        /// 
        /// The solution root does not have any parent (other than the viewmodel it lives in).
        /// Therefore, this method always returns null.
        /// </summary>
        public ISolutionItem GetParent()
        {
            return null;
        }

        /// <summary>
        /// Method is called when an item deep down in the solution tree
        /// has changed. Therefore, IsDirty is set to true.
        /// </summary>
        public void SolutionIsDirty()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Resets the dirty flag to false.
        /// Use this at end of construction/load data phase.
        /// </summary>
        public void SolutionIsDirty_Reset()
        {
            IsDirty = false;
        }

        /// <summary>
        /// Gets a reference to the wrapped underlying model.
        /// </summary>
        /// <returns></returns>
        public TranslatorSolution GetModel()
        {
            return mSolutionModel;
        }

        /// <summary>
        /// Adds a new project entry based on the given model.
        /// </summary>
        /// <param name="model"></param>
        public void AddProject(Project model)
        {
            var fr = new ProjectViewModel(model, this);
            var item = Children.SingleOrDefault(p => string.Compare(p.Path, fr.Path, true) == 0);

            if (item == null)
            {
                Children.Add(fr);                  // Add viewmodel and model into children collection
                mSolutionModel.AddProject(model);
                IsDirty = true;
            }
            else
            {
                var project = item as ProjectViewModel;

                if (project != null)
                {
                    if (project.SourceFile != null)
                    {
                        // Just adjust non key values if this item is already there
                        project.SourceFile.Comment = fr.SourceFile.Comment;
                        project.SourceFile.Type = fr.SourceFile.Type;

                        IsDirty = true;
                    }
                }
            }
        }

        /// <summary>
        /// Remove a targe file from the collection of target files in viewmodel and underlying model.
        /// </summary>
        /// <param name="frViewModel"></param>
        /// <returns></returns>
        public bool RemoveProject(ProjectViewModel frViewModel)
        {
            if (string.IsNullOrEmpty(frViewModel.ItemPathName) == true)
                return false;

            var item = Children.SingleOrDefault(p => string.Compare(p.Path, frViewModel.ItemPathName, true) == 0);

            if (item != null)
            {
                var projectVMColl = item as ProjectViewModel;

                var model = projectVMColl.GetModel();

                // Remove model and viewmodel from collection of target files
                var modelSuccess = mSolutionModel.RemoveProject(frViewModel.SourceFile.Path);
                var viewmodelSuccess = Children.Remove(projectVMColl);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Set a property to determine whether a file is actually exiosting or not.
        /// </summary>
        public void SetExists(ItemExisits newValue)
        {
            Exists = newValue;
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
        #endregion methods
    }
}
