namespace LocultApp.Controls.Exception
{
    using LocultApp.ViewModels.Base;
    using MSTranslate.Interfaces;
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Implements a viewmodel for displaying messages relevant to exceptions to the user.
    /// </summary>
    public class ExceptionViewModel : ViewModels.Base.ViewModelBase, IExceptionViewModel
    {
        #region fields
        private Exception mInnerException;
        private string mErrorMessage, mErrorStackTrace, mErrorTip;
        private bool mIsVisible = false, mIsErrorTipVisible = false;
        private ICommand mCloseExceptionViewCommand;
        #endregion fields

        #region constructors
        public ExceptionViewModel(Exception exp = null)
        {
            this.mInnerException = exp;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets an error message for display.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return mErrorMessage;
            }

            private set
            {
                if (mErrorMessage != value)
                {
                    mErrorMessage = value;
                    RaisePropertyChanged(() => ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Gets an error stacktrace for display.
        /// </summary>
        public string ErrorStackTrace
        {
            get
            {
                return mErrorStackTrace;
            }

            private set
            {
                if (mErrorStackTrace != value)
                {
                    mErrorStackTrace = value;
                    RaisePropertyChanged(() => ErrorStackTrace);
                }
            }
        }

        /// <summary>
        /// Gets whether an error tip/hint towards resolution for this (known) error is visible or not.
        /// </summary>
        public bool IsErrorTipVisible
        {
            get
            {
                return mIsErrorTipVisible;
            }

            private set
            {
                if (mIsErrorTipVisible != value)
                {
                    mIsErrorTipVisible = value;
                    RaisePropertyChanged(() => IsErrorTipVisible);
                }
            }
        }

        /// <summary>
        /// Gets an error tip/hint towards resolution for this (known) error.
        /// </summary>
        public string ErrorTip
        {
            get
            {
                return mErrorTip;
            }

            private set
            {
                if (mErrorTip != value)
                {
                    mErrorTip = value;
                    RaisePropertyChanged(() => ErrorTip);
                }
            }
        }

        /// <summary>
        /// Gets the exception error object that caused the error.
        /// </summary>
        public Exception InnerException
        {
            get
            {
                return mInnerException;
            }

            private set
            {
                if (mInnerException != value)
                {
                    mInnerException = value;

                    ShowExceptionView((value != null));

                    RaisePropertyChanged(() => InnerException);
                    RaisePropertyChanged(() => IsExceptionVisible);
                }
            }
        }

        /// <summary>
        /// Gets whether the exception should currently be displayed ot not.
        /// </summary>
        public bool IsExceptionVisible
        {
            get
            {
                return mIsVisible;
            }

            private set
            {
                if (mIsVisible != value)
                {
                    mIsVisible = value;
                    RaisePropertyChanged(() => IsExceptionVisible);
                }
            }
        }

        /// <summary>
        /// Gets a command that can make the current exception view invisible.
        /// </summary>
        public ICommand CloseExceptionViewCommand
        {
            get
            {
                if (mCloseExceptionViewCommand == null)
                {
                    mCloseExceptionViewCommand = new RelayCommand<object>((p) =>
                    {
                        ShowExceptionView(false);
                    });
                }

                return mCloseExceptionViewCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets an exception object to show in exception display.
        /// </summary>
        /// <param name="exp"></param>
        public void SetExceptionForDisplay(Exception exp)
        {
            InnerException = exp;
            ErrorTip = null;
            IsErrorTipVisible = false;
            ErrorMessage = ErrorStackTrace = Local.Strings.STR_Unknown_Error;

            if (exp != null)
            {
                Exception innerException = exp;

                while (innerException.InnerException != null)
                    innerException = innerException.InnerException;

                // Set original message thrown if its available
                ErrorMessage = innerException.Message;
                ErrorStackTrace = innerException.StackTrace;

                if (innerException is ITranslatorError)
                {
                    var knownErrorType = innerException as ITranslatorError;

                    IsErrorTipVisible = true;
                    ErrorTip = string.Format("{0}: {2} - {1}", Local.Strings.STR_ERROR_CODE,
                                                               knownErrorType.ErrorCode,
                                                               knownErrorType.HintForResolution);
                }
            }
        }

        /// <summary>
        /// Determines if current exception shall be shown or not.
        /// </summary>
        /// <param name="show"></param>
        public void ShowExceptionView(bool show)
        {
            IsExceptionVisible = show;
        }
        #endregion methods
    }
}
