namespace LocultApp.Views.Pages
{
    using LocultApp.ViewModels.Pages.Interfaces;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for EditPage.xaml
    /// </summary>
    public partial class EditPage : UserControl
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsEnablePage.
        /// This enables forwarding global IsEnabled=false request to selected elements rather than ALL children
        /// This in turn enables visbility and enabling of cancel buttons at their specific location.
        /// </summary>
        private static readonly DependencyProperty IsEnablePageProperty =
            DependencyProperty.Register("IsEnablePage", typeof(bool), typeof(EditPage), new PropertyMetadata(true));

        /// <summary>
        /// Class constructor
        /// </summary>
        public EditPage()
        {
            InitializeComponent();
            this.Loaded += EditPage_Loaded;
            this.Unloaded += EditPage_Unloaded;
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsEnablePage.
        /// This enables forwarding global IsEnabled=false request to selected elements rather than ALL children
        /// This in turn enables visbility and enabling of cancel buttons at their specific location.
        /// </summary>
        public bool IsEnablePage
        {
            get { return (bool)GetValue(IsEnablePageProperty); }
            set { SetValue(IsEnablePageProperty, value); }
        }

        private void EditPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetLoadStatus(true);
        }

        private void EditPage_Unloaded(object sender, RoutedEventArgs e)
        {
            SetLoadStatus(false);
        }

        private void SetLoadStatus(bool isLoaded)
        {
            if (this.DataContext != null)
            {
                var context = DataContext as IDocumentCanUnload;

                if (context != null)
                {
                    if (isLoaded == true)
                        context.OnViewLoaded();
                    else
                        context.OnViewLoaded();
                }
            }
        }
    }
}
