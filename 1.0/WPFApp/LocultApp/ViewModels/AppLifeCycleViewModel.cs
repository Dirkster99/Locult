﻿namespace LocultApp.ViewModels
{
    using LocultApp;
    using LocultApp.ViewModels.Base;
    using LocultApp.ViewModels.Pages;
    using MsgBox;
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Implements application life cycle relevant properties and methods,
    /// such as: state for shutdown, shutdown_cancel, command for shutdown,
    /// and methods for save and load application configuration.
    /// </summary>
    public class AppLifeCycleViewModel : Base.ViewModelBase
    {
        #region fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool? mDialogCloseResult = null;
        private bool mShutDownInProgress = false;
        private bool mShutDownInProgress_Cancel = false;

        private ICommand mExitApp = null;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets a string for display of the application title.
        /// </summary>
        public string Application_Title
        {
            get
            {
                return "Locult";
            }
        }

        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        public bool? DialogCloseResult
        {
            get
            {
                return mDialogCloseResult;
            }

            private set
            {
                if (mDialogCloseResult != value)
                {
                    mDialogCloseResult = value;
                    RaisePropertyChanged(() => DialogCloseResult);
                }
            }
        }

        /// <summary>
        /// Gets a command to exit (end) the application.
        /// </summary>
        public ICommand ExitApp
        {
            get
            {
                if (mExitApp == null)
                {
                    mExitApp = new RelayCommand<object>((p) => AppExit_CommandExecuted(),
                                                        (p) => Closing_CanExecute());
                }

                return mExitApp;
            }
        }

        /// <summary>
        /// Get a path to the directory where the user store his documents
        /// </summary>
        public static string MyDocumentsUserDir
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public bool ShutDownInProgress_Cancel
        {
            get
            {
                return mShutDownInProgress_Cancel;
            }

            set
            {
                if (mShutDownInProgress_Cancel != value)
                    mShutDownInProgress_Cancel = value;
            }
        }
        #endregion properties

        #region methods
        #region Save Load Application configuration
        /// <summary>
        /// Save application settings when the application is being closed down
        /// </summary>
        public void SaveConfigOnAppClosed()
        {
            try
            {
                AppCore.CreateAppDataFolder();

                // Save App view model fields
                var settings = base.GetService<ISettingsManager>();

                //// settings.SessionData.LastActiveSourceFile = this.mStringDiff.SourceFilePath;
                //// settings.SessionData.LastActiveTargetFile = this.mStringDiff.TargetFilePath;

                // Save program options only if there are un-saved changes that need persistence
                // This can be caused when WPF theme was changed or something else
                // but should normally not occur as often as saving session data
                if (settings.Options.IsDirty == true)
                {
                    ////settings.SaveOptions(AppCore.DirFileAppSettingsData, settings.SettingData);
                    settings.Options.WriteXML(AppCore.DirFileAppSettingsData);
                }

                settings.SaveSessionData(AppCore.DirFileAppSessionData, settings.SessionData);
            }
            catch (Exception exp)
            {
                var msg = GetService<IMessageBoxService>();
                msg.Show(exp, LocultApp.Local.Strings.STR_UNEXPECTED_ERROR, MsgBoxButtons.OK, MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// Load configuration from persistence on startup of application
        /// </summary>
        public void LoadConfigOnAppStartup()
        {
            var settings = base.GetService<ISettingsManager>();
            try
            {
                // Re/Load program options and user profile session data to control global behaviour of program
                ////settings.LoadOptions(AppCore.DirFileAppSettingsData);
                settings.Options.ReadXML(AppCore.DirFileAppSettingsData);
            }
            catch
            {
            }

            try
            {
                settings.LoadSessionData(AppCore.DirFileAppSessionData);
            }
            catch
            { }

            try
            {
                settings.CheckSettingsOnLoad(SystemParameters.VirtualScreenLeft,
                                             SystemParameters.VirtualScreenTop);

                // Initialize view model fields
                //// this.StringDiff.SourceFilePath = settings.SessionData.LastActiveSourceFile;
                //// this.StringDiff.TargetFilePath = settings.SessionData.LastActiveTargetFile;
            }
            catch
            { }
        }
        #endregion Save Load Application configuration

        #region StartUp/ShutDown
        private void AppExit_CommandExecuted()
        {
            try
            {
                if (Closing_CanExecute() == true)
                {
                    mShutDownInProgress_Cancel = false;
                    OnRequestClose();
                }
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);

                var msg = GetService<IMessageBoxService>();
                msg.Show(exp, LocultApp.Local.Strings.STR_MSG_UnknownError_Caption,
                         MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton);
            }
        }

        private bool Closing_CanExecute()
        {
            return true;
        }
 
        /// <summary>
        /// Check if pre-requisites for closing application are available.
        /// Save session data on closing and cancel closing process if necessary.
        /// </summary>
        /// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
        public bool Exit_CheckConditions(object sender)
        {
            var msg = ServiceLocator.ServiceContainer.Instance.GetService<IMessageBoxService>();
            try
            {
                if (mShutDownInProgress == true)
                    return true;

                var docSource = sender as IDocumentCollection;

                if (docSource == null)
                    return true;

                IList<IDirtyDocument> dirtyDocs = docSource.GetDirtyDocuments();

                if (dirtyDocs == null)
                    return true;

                return PageManagerViewModel.GetOKToLeavePageWithoutSave(dirtyDocs);

                ////// Do layout serialization after saving/closing files
                ////// since changes implemented by shut-down process are otherwise lost
                ////try
                ////{
                ////    App.CreateAppDataFolder();
                ////    this.SerializeLayout(sender);            // Store the current layout for later retrieval
                ////}
                ////catch
                ////{
                ////}
            }
            catch (Exception exp)
            {
                logger.Error(exp.Message, exp);
                msg.Show(exp, Local.Strings.STR_UNEXPECTED_ERROR,
                                MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton);
                                //App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
            }

            return true;
        }

        #region RequestClose [event]
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        ////public event EventHandler ApplicationClosed;

        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        public void OnRequestClose(bool ShutDownAfterClosing = true)
        {
            try
            {
                if (ShutDownAfterClosing == true)
                {
                    if (mShutDownInProgress == false)
                    {
                        if (DialogCloseResult == null)
                            DialogCloseResult = true;      // Execute Closing event via attached property

                        if (mShutDownInProgress_Cancel == true)
                        {
                            mShutDownInProgress = false;
                            mShutDownInProgress_Cancel = false;
                            DialogCloseResult = null;
                        }
                    }
                 }
                else
                    mShutDownInProgress = true;

                CommandManager.InvalidateRequerySuggested();

                ////EventHandler handler = ApplicationClosed;
                ////if (handler != null)
                ////  handler(this, EventArgs.Empty);
            }
            catch (Exception exp)
            {
                mShutDownInProgress = false;

                logger.Error(exp.Message, exp);

                var msg = GetService<IMessageBoxService>();
                msg.Show(exp, Local.Strings.STR_MSG_UnknownError_Caption,
                         MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton);
            }
        }

        public void CancelShutDown()
        {
            DialogCloseResult = null;
        }
        #endregion // RequestClose [event]
        #endregion StartUp/ShutDown
        #endregion methods 
    }
}
