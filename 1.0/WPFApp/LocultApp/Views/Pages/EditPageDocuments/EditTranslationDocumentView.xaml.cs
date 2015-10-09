namespace LocultApp.Views.Pages.EditPageDocuments
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainOverview.xaml
    /// </summary>
    public partial class EditTranslationDocumentView : UserControl
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsEnablePage.
        /// This enables forwarding global IsEnabled=false request to selected elements rather than ALL children
        /// This in turn enables visbility and enabling of cancel buttons at their specific location.
        /// </summary>
        private static readonly DependencyProperty IsEnablePageProperty =
            DependencyProperty.Register("IsEnablePage", typeof(bool), typeof(EditTranslationDocumentView), new PropertyMetadata(true));

        /// <summary>
        /// Class constructor
        /// </summary>
        public EditTranslationDocumentView()
        {
            InitializeComponent();
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
    }
}
