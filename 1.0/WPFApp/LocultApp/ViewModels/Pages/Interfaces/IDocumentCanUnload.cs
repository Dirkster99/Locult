namespace LocultApp.ViewModels.Pages.Interfaces
{
    /// <summary>
    /// This interface can be implemented on viewmodels that want explicit hints
    /// when view are done initializing or unloading.
    /// </summary>
    public interface IDocumentCanUnload
    {
        /// <summary>
        /// method executes via interface call when the attached view is unloaded.
        /// </summary>
        void OnViewUnloaded();

        /// <summary>
        /// method executes via interface call when the attached view is loaded.
        /// </summary>
        void OnViewLoaded();
    }
}
