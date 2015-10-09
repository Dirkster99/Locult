namespace LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events
{
    using LocultApp.ViewModels.Events;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces;
    using System;

    /// <summary>
    /// Class implements document base functions which are properties and methods that all documents support.
    /// Documentation: http://www.codeproject.com/Articles/995629/A-Cancelable-WPF-MVVM-TreeView-for-Document-Naviga
    /// </summary>
    public class DocumentDirtyChangedViewModelBase : DocumentViewModelBase,
                                                     IDocumentDirtyChangedBase
    {
        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DocumentDirtyChangedViewModelBase(ApplicationRequestEventHandler requestedAction = null)
            : base(requestedAction)
        {
        }
        #endregion constructors

        #region events
        /// <summary>
        /// Is raised when a document viewmodel appears to be dirty
        /// (dirty flag is assumed to be set when user makes unsaved changes to the document).
        /// </summary>
        public event EventHandler<DocumentDirtyChangedEventArgs> DirtyFlagChangedEvent;
        #endregion events

        #region properties
        /// <summary>
        /// Gets/set the dirty state of a document.
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty;
            }

            set
            {
                if (base.IsDirty != value)
                {
                    if (DirtyFlagChangedEvent != null)
                        DirtyFlagChangedEvent(this, new DocumentDirtyChangedEventArgs(base.IsDirty, value));

                    base.IsDirty = value;
                }
            }
        }
        #endregion properties
    }
}
