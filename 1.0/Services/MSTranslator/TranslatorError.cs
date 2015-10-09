namespace MSTranslate
{
    using MSTranslate.Interfaces;

    /// <summary>
    /// Gets an enumeration that can be used to generate
    /// an error object with human readable information.
    /// </summary>
    internal enum TranslateErrorCode
    {
        Missing_URI_AuthenticationParameter = 100,
        Missing_Login_AuthenticationParameter = 101,
        Missing_Password_AuthenticationParameter = 102
    }

    /// <summary>
    /// Handles internal error messaging towards the application level.
    /// </summary>
    internal class TranslatorError : ITranslatorError
    {
        #region contructors
        /// <summary>
        /// Hidden class constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="hintForResolution"></param>
        protected TranslatorError(string message,
                                  string errorCode,
                                  string hintForResolution = "")
        {
            Message = message;
            ErrorCode = errorCode;
            HintForResolution = hintForResolution;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected TranslatorError()
        {
        }
        #endregion contructors

        #region properties
        /// <summary>
        /// Gets a human readable error message for a specific problem.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets a hint towards the most likely problem resolution for resolving this problem.
        /// </summary>
        public string HintForResolution { get; private set; }

        /// <summary>
        /// Gets a unique error code for tracking towards the cause of a specific problem.
        /// </summary>
        public string ErrorCode { get; private set; }
        #endregion properties

        /// <summary>
        /// Gets an error object based on a specific error code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Error object describing a failure situation.</returns>
        public static ITranslatorError GetErrorObject(TranslateErrorCode code)
        {
            switch (code)
            {
                case TranslateErrorCode.Missing_URI_AuthenticationParameter:
                    return new TranslatorError(Resources.Strings.STR_Missing_URI_AuthenticationParameter,
                                               Resources.Strings.STR_Missing_URI_AuthenticationParameter_Hint,
                                               code.ToString());

                case TranslateErrorCode.Missing_Login_AuthenticationParameter:
                    return new TranslatorError(Resources.Strings.STR_Missing_Login_AuthenticationParameter,
                                               Resources.Strings.STR_Missing_Login_AuthenticationParameter_Hint,
                                               code.ToString());

                case TranslateErrorCode.Missing_Password_AuthenticationParameter:
                    return new TranslatorError(Resources.Strings.STR_Missing_Password_AuthenticationParameter,
                                               Resources.Strings.STR_Missing_Password_AuthenticationParameter_Hint,
                                               code.ToString());

                default:
                    throw new System.NotImplementedException(code.ToString());
            }
        }
    }
}
