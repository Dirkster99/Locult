namespace LocultApp.Controls.Exception
{
    /// <summary>
    /// Implement properties and methods to forward excption to a viewmodel that can
    /// display data relevant to exceptions (when any occur).
    /// </summary>
    public interface ICanForwardExceptionsToDisplay
    {
        /// <summary>
        /// Displays an exception via a dedicated error display or clears
        /// the current error display if <paramref name="exp"/> is null.
        /// </summary>
        /// <param name="exp"></param>
        void ForwardExceptionToDisplay(System.Exception exp);
    }
}
