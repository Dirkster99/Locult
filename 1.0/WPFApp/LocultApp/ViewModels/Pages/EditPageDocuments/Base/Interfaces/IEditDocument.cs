namespace LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces
{
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Interfaces;
    using TranslationSolutionViewModelLib.ViewModels;

    /// <summary>
    /// Defines an interface for documents that are displayed
    /// when a node in the solution viewmodel is selected.
    /// </summary>
    public interface IEditSolutionDocument : IDocumentBase
    {
        #region properties
        /// <summary>
        /// Gets the path and name of a new solution.
        /// </summary>
        string SourceFilePathName { get; }

        /// <summary>
        /// Gets the name of a source file for this object.
        /// </summary>
        string SourceFileName { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Initalizes the document viewmodel with data from the supplied solution or project.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="root"></param>
        void InitDocument(ISolutionRoot root, ISolutionItem item);
        #endregion methods
    }
}
