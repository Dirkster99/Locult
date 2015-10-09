namespace LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events
{
    using System;

    /// <summary>
    /// Class implements an event that can be used to inform sub-scribers about a change
    /// of the dirty flag of a document. This change can then be used to decide whether
    /// destruction of the corresponding document viewmodel class is OK or not.
    /// </summary>
    public class DocumentDirtyChangedEventArgs : EventArgs
    {
        public DocumentDirtyChangedEventArgs(bool isDirtyOldValue, bool isDirtyNewValue)
        {
            IsDirtyOldValue = isDirtyOldValue;
            IsDirtyNewValue = isDirtyNewValue;
        }

        /// <summary>
        /// Gets the old value for the IsDirty flag of the sender.
        /// </summary>
        public bool IsDirtyOldValue { get; private set; }

        /// <summary>
        /// Gets the new (and currently active) value for the IsDirty flag of the sender.
        /// </summary>
        public bool IsDirtyNewValue { get; private set; }
    }
}
