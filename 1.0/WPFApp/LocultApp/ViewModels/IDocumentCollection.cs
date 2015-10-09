namespace LocultApp.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface declares base events and properties of all documents being displayed.
    /// </summary>
    public interface IDirtyDocument
    {
        /// <summary>
        /// Gets/set the dirty state of a document.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Gets the file path and file name of an item that may or my not require saving.
        /// </summary>
        string FilePathName { get;  }
    }

    /// <summary>
    /// Interface implements a method for getting a hold of all currently active documents.
    /// This collection can be used to determine whether there is a document that has unsafed
    /// changes (IsDirty == true).
    /// This in turn can be useful on Application ShutDown to determine whether the changed
    /// document requires saving before shutting the applicaiton and loosing data.
    /// </summary>
    public interface IDocumentCollection
    {
        /// <summary>
        /// Get a list of all documents currently present and dirty.
        /// The returned list is empty if there are no dirty documents.
        /// 
        /// This design assumes that each displayed document can have only one persistance/dirty flag
        /// That is we will have to extend this if the source document of a StringCollectionDiffViewModel class
        /// Should ever be editable
        /// </summary>
        /// <returns></returns>
        IList<IDirtyDocument> GetDirtyDocuments();
    }
}
