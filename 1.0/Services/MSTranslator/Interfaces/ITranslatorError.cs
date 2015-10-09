namespace MSTranslate.Interfaces
{
    /// <summary>
    /// Define an interface to an object that can handle advanced
    /// error handling for MS Translator sub-system processing.
    /// </summary>
    public interface ITranslatorError
    {
        /// <summary>
        /// Gets a human readable error message for a specific problem.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets a hint towards the most likely problem resolution for resolving this problem.
        /// </summary>
        string HintForResolution { get; }

        /// <summary>
        /// Gets a unique error code for tracking towards the cause of a specific problem.
        /// </summary>
        string ErrorCode { get; }
    }
}
