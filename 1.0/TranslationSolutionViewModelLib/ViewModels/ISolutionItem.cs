namespace TranslationSolutionViewModelLib.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public enum ItemExisits
    {
        Unknown = 0,
        DoesExist = 1,
        DoesNotExist = 2
    };

    public enum TypeOfSolutionItem
    {
        Unknown = 0,
        Root = 1,
        Project = 2,
        File = 3
    };

    public interface ISolutionRoot : ISolutionItem, ISolutionItemExists
    {
        /// <summary>
        /// Calling this method indicates that an item deep
        /// down in the solution tree has changed.
        /// </summary>
        void SolutionIsDirty();

        /// <summary>
        /// Indicates whether the solution has unsafed edits or not.
        /// </summary>
        bool IsDirty { get; }
    }

    public interface ISolutionItem : ISolutionItemExists
    {
        #region properties
        /// <summary>
        /// Gets the actual name of the item that is represented by this object.
        /// </summary>
        string ItemName { get; }

        /// <summary>
        /// Gets the file system path (if any) of this item.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the file system path and name (if any) of this item.
        /// </summary>
        string ItemPathName { get; }

        /// <summary>
        /// Gets a comment (can be shown as tool tip) of the item that is represented by this object.
        /// </summary>
        string ItemTip { get; }

        /// <summary>
        /// Gets a list of sub-children (or null if there are no children) for this item.
        /// </summary>
        ObservableCollection<ISolutionItem> Children { get;  }

        /// <summary>
        /// Get/set whether this folder is currently selected or not.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Get/set whether this folder is currently expanded or not.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets an enumeration based identifier that tells the type of object in an enumeratable way.
        /// </summary>
        TypeOfSolutionItem TypeOfItem { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the parent of a solution item. This parent link can be used to navigate the tree
        /// structure towards the root rather than just toward ist children
        /// </summary>
        ISolutionItem GetParent();

        /// <summary>
        /// Searches for a given type of item among the childrens of this item.
        /// </summary>
        /// <param name="typeToSearch"></param>
        /// <returns>The item or null if this type of item is not among those children.</returns>
        ISolutionItem SearchFirstItemByType(TypeOfSolutionItem typeToSearch);
        #endregion methods
    }

    public interface ISolutionItemExists
    {
        /// <summary>
        /// Gets a property to determine whether a file is actually exiosting or not.
        /// </summary>
        ItemExisits Exists { get; }

        /// <summary>
        /// Set a property to determine whether a file is actually exiosting or not.
        /// </summary>
        void SetExists(ItemExisits newValue);
    }
}
