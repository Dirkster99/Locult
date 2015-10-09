namespace LocultApp.ViewModels.StringSource
{
    /// <summary>
    /// Interface to communicate back to parent
    /// when this detail has changed via <seealso cref="IEditableObject"/> method.
    /// </summary>
    internal interface IEntryDiffViewModelParent
    {
        /// <summary>
        /// This method can be invoked to tell parent that details in this item have changed
        /// and may need synchronization with with other views of the same detail.
        /// </summary>
        /// <param name="thisEntryHasChanged"></param>
        void UpdateDiffEntry(EntryDiffViewModel thisEntryHasChanged);
    }
}
