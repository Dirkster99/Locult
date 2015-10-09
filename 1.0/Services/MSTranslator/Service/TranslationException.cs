namespace MSTranslate.Service
{
    using MSTranslate.Interfaces;
    using System;

    /// <summary>
    /// Implements an application specific error and exception class
    /// for advanced error handling within the application.
    /// </summary>
    public class TranslationException : Exception, ITranslatorError
    {
        #region constructors
        /// <summary>
        /// Class constructor from <seealso cref="ITranslatorError"/> interface.
        /// </summary>
        /// <param name="errorCode"></param>
        public TranslationException(ITranslatorError errorCode)
            : base(errorCode.Message)
        {
            HintForResolution = errorCode.HintForResolution;
            ErrorCode = errorCode.ErrorCode;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="hintForResolution"></param>
        public TranslationException(string message,
                               string errorCode,
                               string hintForResolution = "")
            : base(message)
        {
            HintForResolution = hintForResolution;
            ErrorCode = errorCode;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a hint towards the most likely problem resolution for resolving this problem.
        /// </summary>
        public string HintForResolution { get; private set; }

        /// <summary>
        /// Gets a unique error code for tracking towards the cause of a specific problem.
        /// </summary>
        public string ErrorCode { get; private set; }
        #endregion properties
    }
}
