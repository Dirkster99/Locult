namespace LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces
{
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events;
    using System;

    /// <summary>
    /// This interface declares base events and properties of documents being displayed.
    /// These documents can tell their parent via events when and if they become dirty.
    /// PArents can then react to this fact before disposing of the changed document object.
    /// </summary>
    public interface IDocumentDirtyChangedBase
    {
        /// <summary>
        /// Is raised when a document viewmodel appears to be dirty
        /// (dirty flag is assumed to be set when user makes unsaved changes to the document).
        /// </summary>
        event EventHandler<DocumentDirtyChangedEventArgs> DirtyFlagChangedEvent;

        #region properties
        /// <summary>
        /// Gets/set the dirty state of a document.
        /// </summary>
        bool IsDirty { get; set; }
        #endregion properties
    }
}
