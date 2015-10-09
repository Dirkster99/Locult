namespace LocultApp.ViewModels.Events
{
    using System;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// Defines types of requests that can be communicated across the application.
    /// </summary>
    public enum ApplicationRequest
    {
        // Open a solution file from a given location.
        OpenSolution = 0,
        NewSolution = 1,
        DisplayException = 2,
        InitProcessing = 3
    }

    /// <summary>
    /// Delegate event handler for <seealso cref="EventArgs"/> class below.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ApplicationRequestEventHandler(object sender, ApplicationRequestEvent e);

    /// <summary>
    /// Defines an event that can enable application wide events to open a file from a given location.
    /// </summary>
    public class ApplicationRequestEvent : EventArgs
    {
        #region constructors
        public ApplicationRequestEvent(ApplicationRequest request,
                                       string fileLocation,
                                       TranslatorSolution solution = null)
        {
            Request = request;
            FileLocation = fileLocation;
            Solution = solution;
        }

        public ApplicationRequestEvent(ApplicationRequest request,
                                       object result = null)
        {
            Request = request;
            Result = result;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a property to define the kind of request by the set enum value.
        /// </summary>
        public ApplicationRequest Request { get; protected set; }

        /// <summary>
        /// Gets a property to define file path and name to apply this request to.
        /// </summary>
        public string FileLocation { get; protected set; }

        public TranslatorSolution Solution { get; protected set; }

        /// <summary>
        /// Can be used to return any object of interest (receiver will have to
        /// know how to decode by <seealso cref="ApplicationRequest"/> enumeration.
        /// </summary>
        public object Result { get; protected set; }
        #endregion properties
    }
}
