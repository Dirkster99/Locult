namespace LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces
{
    using System;

    /// <summary>
    /// Implements a basic documnet object interface.
    /// </summary>
    public interface IDocumentBase
    {
        #region properties
        /// <summary>
        /// Gets/set the dirty state of a document.
        /// </summary>
        bool IsDirty { get; set; }
        #endregion properties
    }
}
