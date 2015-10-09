namespace LocultApp.Controls.Exception
{
    /// <summary>
    /// Implement properties and methods to display data relevant to exceptions (when any occur).
    /// Viewmodels that implement this can use the <seealso cref="ExceptionView"/> to display
    /// errors on there own (without messagebox).
    /// </summary>
    public interface ICanDisplayException
    {
        IExceptionViewModel ViewException { get; }
    }
}
