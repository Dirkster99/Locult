namespace LocultApp.ViewModels.StringSource
{
    using System;

    public interface IStringCollectionDiffViewModelParent
    {
        /// <summary>
        /// Tell the document base whether this document is dirty or not. The document base can then
        /// decide whether it is dirty (based on some other changed data or not.
        /// </summary>
        /// <param name="v"></param>
        void TargetDocumentIsDirty(bool v, bool resetChildren);

        /// <summary>
        /// Ends a currenlty running global process state and displays the suggested error items.
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="exp"></param>
        /// <param name="Message"></param>
        void EndProcessingSolutionWithError(string processID, Exception exp, string Message);
    }
}
