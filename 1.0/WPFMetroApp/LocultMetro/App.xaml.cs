namespace LocultMetro
{
    using AppResourcesLib;
    using FirstFloor.ModernUI.Presentation;
    using LocultApp;
    using LocultApp.ViewModels;
    using LocultApp.ViewModels.Pages.SettingsPages;
    using LocultMetro.Views;
    using log4net;
    using log4net.Config;
    using MsgBox;
    using Settings.Interfaces;
    using Settings.UserProfile;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region fields
        protected static log4net.ILog Logger;

        private Window mMainWin;
        #endregion fields

        #region constructors
        static App()
        {
            XmlConfigurator.Configure();
            Logger = LogManager.GetLogger("default");

            // Create service model to ensure available services
            ServiceInjector.InjectServices();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the matching application viewmodel that is
        /// attached to the mainwindow of this application.
        /// </summary>
        internal AppViewModel Workspace
        {
            get
            {
                return mMainWin.DataContext as AppViewModel;
            }

            private set
            {
                if (mMainWin.DataContext != value)
                    mMainWin.DataContext = value;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Restore the applications window from minimized state
        /// into non-minimzed state and send it to the top to make
        /// sure its visible for the user.
        /// </summary>
        public static void RestoreCurrentMainWindow()
        {
            if (Application.Current != null)
            {
                if (Application.Current.MainWindow != null)
                {
                    Window win = Application.Current.MainWindow;

                    if (win.IsVisible == false)
                        win.Show();

                    if (win.WindowState == WindowState.Minimized)
                        win.WindowState = WindowState.Normal;

                    win.Topmost = true;
                    win.Topmost = false;
                }
            }
        }

        /// <summary>
        /// Check if end of application session should be canceled or not
        /// (we may have gotten here through unhandled exceptions - so we
        /// display it and attempt CONTINUE so user can save his data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            base.OnSessionEnding(e);

            try
            {
                try
                {
                    Logger.Error(string.Format(CultureInfo.InvariantCulture,
                                 "The {0} application received request to shutdown: {1}.",
                                 Application.ResourceAssembly.GetName(), e.ReasonSessionEnding.ToString()));
                }
                catch
                {
                }

                if (mMainWin != null)
                {
                    if (mMainWin.DataContext != null)
                    {
                        // Close all open files and check whether application is ready to close
                        if (Workspace.AppLifeCycle.Exit_CheckConditions(mMainWin.DataContext) == true)
                        {
                            e.Cancel = false;
                            Workspace.AppLifeCycle.SaveConfigOnAppClosed();
                        }
                        else
                            e.Cancel = Workspace.AppLifeCycle.ShutDownInProgress_Cancel = true;
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }

        /// <summary>
        /// This is the first bit of code being executed when the application is invoked (main entry point).
        /// 
        /// Use the <paramref name="e"/> parameter to evaluate command line options.
        /// Invoking a program with an associated file type extension (eg.: *.txt) in Explorer
        /// results, for example, in executing this function with the path and filename being
        /// supplied in <paramref name="e"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // Set shutdown mode here (and reset further below) to enable showing custom dialogs (messageboxes)
                // durring start-up without shutting down application when the custom dialogs (messagebox) closes
                ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            }
            catch
            {
            }

            var settings = GetService<ISettingsManager>(); // add the default themes

            try // Program message box service for Modern UI (Metro Light and Dark)
            {
                var msgBox = GetService<IMessageBoxService>();
                msgBox.Style = MsgBoxStyle.WPFThemed;

                // Add theming models
                settings.Themes.AddThemeInfo("dark", new Uri("/LocultMetro;component/Themes/ModernUIDark.xaml", UriKind.RelativeOrAbsolute)); // AppearanceManager.DarkThemeSource );
                settings.Themes.AddThemeInfo("light", new Uri("/LocultMetro;component/Themes/ModernUILight.xaml", UriKind.RelativeOrAbsolute)); // AppearanceManager.LightThemeSource
            }
            catch
            {
            }

            // Create a general settings model to make sure the app is at least governed by defaults
            // if there are no customized settings on first ever start-up of application
            var options = settings.Options;
            SettingDefaults.CreateGeneralSettings(settings);
            MetroSettingsDefaults.CreateAppearanceSettings(options, settings);

            settings.Options.SetUndirty();

            AppLifeCycleViewModel lifeCycle = null;
            AppViewModel app = null;

            try
            {
                lifeCycle = new AppLifeCycleViewModel();
                lifeCycle.LoadConfigOnAppStartup();

                var selectedLanguage = settings.Options.GetOptionValue<string>("Options", "LanguageSelected");
                Thread.CurrentThread.CurrentCulture = new CultureInfo(selectedLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);

                app = new AppViewModel(lifeCycle);

                if (settings.Options.GetOptionValue<string>("Appearance", "ThemeDisplayName") == "dark")
                    AppearanceManager.Current.DarkThemeCommand.Execute(null);
                else
                {
                    settings.Options.SetOptionValue("Appearance", "ThemeDisplayName", "light");
                    AppearanceManager.Current.LightThemeCommand.Execute(null);
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }


            // Create the optional appearance viewmodel and apply
            // current settings to start-up with correct colors etc...
            var appearSettings = new AppearanceViewModel(settings.Themes);

            try
            {
                appearSettings.ApplyOptionsFromModel(settings.Options);
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            try
            {
                // Register this settings page viewmodel and view
                var reslocService = GetService<IResourceLocator>();
                app.PageManager.AddSettingsPageViewModel(reslocService.GetNewPageTemplateModel(
                           Assembly.GetAssembly(typeof(SettingsAppearance)).GetName().Name,
                           "Views/DataTemplates.xaml",
                            "AppAppearanceSettingsTemplate",
                            appearSettings,
                            "Appearance", 1000));
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            try
            {
                Application.Current.MainWindow = mMainWin = new MainWindow();
                ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;

                AppCore.CreateAppDataFolder();

                if (mMainWin != null && app != null)
                {
                    mMainWin.Loaded += mMainWin_Loaded;

                    mMainWin.Closing += OnClosing;

                    // When the ViewModel asks to be closed, close the window.
                    // Source: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
                    mMainWin.Closed += delegate
                    {
                        // Save session data and close application
                        OnClosed(Workspace, mMainWin);
                    
                        var dispose = Workspace as IDisposable;
                        if (dispose != null)
                        {
                            dispose.Dispose();
                            Workspace = null;
                        }
                    };

                    ConstructMainWindowSession(app, mMainWin);
                    mMainWin.Show();
                }

                double fontSize = (double)settings.Options.GetOptionValue<int>("Appearance", "DefaultFontSize");
                double keyFixedFontSize = (double)settings.Options.GetOptionValue<int>("Appearance", "FixedFontSize");
                
                Application.Current.Resources[AppearanceManager.KeyDefaultFontSize] = fontSize;
                Application.Current.Resources[AppearanceManager.KeyFixedFontSize] = keyFixedFontSize;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }


        /// <summary>
        /// Method executes when the Main Application Window is loaded for the very first time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mMainWin_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = sender as FrameworkElement;

            if (mainWindow != null)
                mMainWin.Loaded -= mMainWin_Loaded;

            var settings = GetService<ISettingsManager>();

            Workspace.InitAppViewModel(
                settings.Options.GetOptionValue<bool>("Options", "ReloadOpenFilesFromLastSession"),
                settings.SessionData.LastActiveSolution);
        }

        /// <summary>
        /// Save session data on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                AppViewModel wsVM = mMainWin.DataContext as AppViewModel;

                if (wsVM != null)
                {
                    // Close all open files and check whether application is ready to close
                    if (wsVM.AppLifeCycle.Exit_CheckConditions(wsVM) == true)
                    {
                        // (other than exception and error handling)
                        wsVM.AppLifeCycle.OnRequestClose(true);

                        e.Cancel = false;
                    }
                    else
                    {
                        wsVM.AppLifeCycle.CancelShutDown();
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }

        /// <summary>
        /// Execute closing function and persist session data to be reloaded on next restart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(AppViewModel appVM, Window win)
        {
            try
            {
                var settings = GetService<ISettingsManager>();

                // Persist window position, width and height from this session
                settings.SessionData.MainWindowPosSz =
                  new ViewPosSizeModel(win.Left, win.Top, win.Width, win.Height,
                                       (win.WindowState == WindowState.Maximized ? true : false));

                // Save/initialize program options that determine global programm behaviour
                appVM.AppLifeCycle.SaveConfigOnAppClosed();
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
                var msg = GetService<IMessageBoxService>();

                msg.Show(exp.ToString(), LocultApp.Local.Strings.STR_MSG_UnknownError_InShutDownProcess,
                                MsgBox.MsgBoxButtons.OK, MsgBox.MsgBoxImage.Error);
            }
        }

        /// <summary>
        /// COnstruct MainWindow an attach datacontext to it.
        /// </summary>
        /// <param name="workSpace"></param>
        /// <param name="win"></param>
        private void ConstructMainWindowSession(AppViewModel workSpace, Window win)
        {
            try
            {
                win.DataContext = workSpace;
                var settings = GetService<ISettingsManager>();

                // Establish command binding to accept user input via commanding framework
                // workSpace.InitCommandBinding(win);

                win.Left = settings.SessionData.MainWindowPosSz.X;
                win.Top = settings.SessionData.MainWindowPosSz.Y;
                win.Width = settings.SessionData.MainWindowPosSz.Width;
                win.Height = settings.SessionData.MainWindowPosSz.Height;
                win.WindowState = (settings.SessionData.MainWindowPosSz.IsMaximized == true ? WindowState.Maximized : WindowState.Normal);

                string lastActiveFile = settings.SessionData.LastActiveSolution;

                MainWindow mainWin = win as MainWindow;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }

        public TServiceContract GetService<TServiceContract>() where TServiceContract : class
        {
            return ServiceLocator.ServiceContainer.Instance.GetService<TServiceContract>();
        }
        #endregion methods
    }
}
