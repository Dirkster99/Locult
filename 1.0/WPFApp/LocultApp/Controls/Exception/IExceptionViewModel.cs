namespace LocultApp.Controls.Exception
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    public interface IExceptionViewModel : INotifyPropertyChanged
    {
        string ErrorMessage { get; }
        string ErrorStackTrace { get; }
        Exception InnerException { get; }
        bool IsExceptionVisible { get; }

        ICommand CloseExceptionViewCommand { get; }

        void ShowExceptionView(bool show);
        void SetExceptionForDisplay(Exception exp);
    }
}
