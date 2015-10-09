namespace LocultMetro
{
    using System.Windows.Media;
    using FirstFloor.ModernUI.Presentation;
    using System.Windows.Input;
    using FirstFloor.ModernUI.Windows.Controls;
    using System.Windows;
    using FirstFloor.ModernUI.Windows.Navigation;
    using System;
    using FirstFloor.ModernUI.Windows;
    using System.Windows.Media.Animation;
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private Storyboard backgroundAnimation;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.DefaultStyleKey = typeof(MainWindow);

            // create empty collections
            SetCurrentValue(MenuLinkGroupsProperty, new LinkGroupCollection());
            SetCurrentValue(TitleLinksProperty, new LinkCollection());

            // associate window commands with this instance
#if NET4
            this.CommandBindings.Add(new CommandBinding(System.Windows.SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(System.Windows.SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(System.Windows.SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(System.Windows.SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
#else
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
#endif
            // associate navigate link command with this instance
            this.CommandBindings.Add(new CommandBinding(LinkCommands.NavigateLink, OnNavigateLink, OnCanNavigateLink));

            // listen for theme changes
            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;

            this.InitializeComponent();
        }

        /// <summary>
        /// Raises the System.Windows.Window.Closed event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // detach event handler
            AppearanceManager.Current.PropertyChanged -= OnAppearanceManagerPropertyChanged;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // retrieve BackgroundAnimation storyboard
            var border = GetTemplateChild("WindowBorder") as Border;
            if (border != null)
            {
                this.backgroundAnimation = border.Resources["BackgroundAnimation"] as Storyboard;

                if (this.backgroundAnimation != null)
                {
                    this.backgroundAnimation.Begin();
                }
            }
        }

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // start background animation if theme has changed
            if (e.PropertyName == "ThemeSource" && this.backgroundAnimation != null)
            {
                this.backgroundAnimation.Begin();
            }
        }

        private void OnCanNavigateLink(object sender, CanExecuteRoutedEventArgs e)
        {
            // true by default
            e.CanExecute = true;

            if (this.LinkNavigator != null && this.LinkNavigator.Commands != null)
            {
                // in case of command uri, check if ICommand.CanExecute is true
                Uri uri;
                string parameter;
                string targetName;

                // TODO: CanNavigate is invoked a lot, which means a lot of parsing. need improvements??
                if (NavigationHelper.TryParseUriWithParameters(e.Parameter, out uri, out parameter, out targetName))
                {
                    ICommand command;
                    if (this.LinkNavigator.Commands.TryGetValue(uri, out command))
                    {
                        e.CanExecute = command.CanExecute(parameter);
                    }
                }
            }
        }

        private void OnNavigateLink(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.LinkNavigator != null)
            {
                Uri uri;
                string parameter;
                string targetName;

                if (NavigationHelper.TryParseUriWithParameters(e.Parameter, out uri, out parameter, out targetName))
                {
                    this.LinkNavigator.Navigate(uri, e.Source as FrameworkElement, parameter);
                }
            }
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            SystemCommands.CloseWindow(this);
#else
            System.Windows.SystemCommands.CloseWindow(this);
#endif
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            SystemCommands.MaximizeWindow(this);
#else
            System.Windows.SystemCommands.MaximizeWindow(this);
#endif
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            SystemCommands.MinimizeWindow(this);
#else
            System.Windows.SystemCommands.MinimizeWindow(this);
#endif
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            SystemCommands.RestoreWindow(this);
#else
            System.Windows.SystemCommands.RestoreWindow(this);
#endif
        }
    }
}
