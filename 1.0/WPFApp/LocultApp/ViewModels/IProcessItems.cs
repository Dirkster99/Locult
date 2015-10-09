namespace LocultApp.ViewModels
{
    using System;

    /// <summary>
    /// Interface to define one an application wide base whether
    /// something is currently being processed on a global level
    /// (automated translation, load, save etc) or not.
    /// </summary>
    public interface IProcessItems
    {
        string CurrentProcessId { get; }
        bool IsProcessingSolution { get; }

        /// <summary>
        /// Register a new process as running on a global scale.
        /// </summary>
        /// <param name="newIsProcessingValue"></param>
        /// <param name="processID"></param>
        /// <returns></returns>
        bool SetIsProcessingSolution(bool newIsProcessingValue, string processID);

        /// <summary>
        /// Ends a currenlty running global process state and displays the suggested error items.
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="exp"></param>
        /// <param name="Message"></param>
        /// <param name="Caption"></param>
        void EndProcessingSolutionWithError(string processID, Exception exp, string Message);
    }
}