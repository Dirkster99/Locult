namespace TranslationSolutionViewModelLib.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using TranslatorSolutionLib.Models;

    public class FileReferenceViewModel : Base.SolutionItemBaseViewModel, ISolutionItem
    {
        #region fields
    	private readonly FileReference mFile = null;
        private ISolutionItem mParent;
        private ItemExisits _Exists = ItemExisits.Unknown;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor from parameter
        /// </summary>
        /// <param name="file"></param>
        public FileReferenceViewModel(FileReference file, ISolutionItem parent)
            : this()
        {
            mParent = parent;
            mFile = new FileReference(file);
        }

        /// <summary>
        /// Class construtor
        /// </summary>
        protected FileReferenceViewModel()
        {
            mFile = null;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="copyThis"></param>
        public FileReferenceViewModel(FileReferenceViewModel copyThis)
            : this()
        {
            if (copyThis == null)
                return;

            mFile = new FileReference(copyThis.mFile);
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
                return TypeOfSolutionItem.File;
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
                    return System.IO.Path.GetFileName(Path);
                }
                catch
                {
                    return Path;
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
                return Comment;
            }
        }

        /// <summary>
        /// Gets the file system path (if any) of this item.
        /// </summary>
        public string Path
        {
        	get
       		{
        		return mFile.Path;
        	}
        	
        	set
        	{
        		if (mFile.Path != value)
      			{
                    mFile.SetPath(value);
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
                return Path;
            }
        }

        /// <summary>
        /// A file item does not have any children - therefore, this property is always null.
        /// </summary>
        public ObservableCollection<ISolutionItem> Children
        {
            get
            {
                // This collection does not have any children items, so we
                // just return an empty collection to avoid NullReference issues
                return new ObservableCollection<ISolutionItem>();
            }
        }

        /// <summary>
        /// Gets the type of file that is represented by this viewmodel.
        /// </summary>
        public string Type
        {
        	get
       		{
        		return mFile.Type;
        	}
        	
        	set
        	{
        		if (mFile.Type != value)
      			{
                    mFile.SetType(value);
                    RaisePropertyChanged(() => Type);
        		}
        	}
        }

        /// <summary>
        /// Gets/set a comment text that can be shown to describe a file in an informal way.
        /// </summary>
        public string Comment
        {
        	get
       		{
        		return mFile.Comment;
        	}
        	
        	
        	set
        	{
        		if (mFile.Comment != value)
      			{
                    mFile.SetComment(value);
                    RaisePropertyChanged(() => Comment);
                    RaisePropertyChanged(() => ItemTip);
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
        /// Method gets a copy of the current model inside this viewmodel.
        /// </summary>
        /// <returns></returns>
        public FileReference GetModel()
        {
            return new FileReference(Path, Type, Comment);
        }

        /// <summary>
        /// Gets the parent of a solution item. This parent link can be used to navigate the tree
        /// structure towards the root rather than just toward ist children
        /// </summary>
        public ISolutionItem GetParent()
        {
            return mParent;
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
        /// <returns>Always null since this type of item does not have children.</returns>
        public ISolutionItem SearchFirstItemByType(TypeOfSolutionItem typeToSearch)
        {
            return null;
        }
        #endregion methods
    }
}
