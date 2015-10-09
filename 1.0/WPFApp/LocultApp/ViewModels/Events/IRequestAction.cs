namespace LocultApp.ViewModels.Pages.StartPage
{
    using System;

    public interface IRequestAction
    {
        event LocultApp.ViewModels.Events.ApplicationRequestEventHandler RequestedAction;
    }
}
