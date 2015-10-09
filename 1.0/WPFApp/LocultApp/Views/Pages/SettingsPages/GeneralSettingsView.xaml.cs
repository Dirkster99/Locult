namespace LocultApp.Views.Pages.SettingsPages
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for GeneralSettingsView.xaml
    /// </summary>
    public partial class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView()
        {
            InitializeComponent();
        }

        private void btStandardAlphaAccept_Click(object sender, RoutedEventArgs e)
        {
////            cdStandardAlpha.Color = cpStandardWithAlpha.SelectedColor;
////            exStandardAlpha.IsExpanded = false;
        }

        private void btStandardAlphaCancel_Click(object sender, RoutedEventArgs e)
        {
////            exStandardAlpha.IsExpanded = false;
        }

        private void exStandardAlpha_Expanded(object sender, RoutedEventArgs e)
        {
////            cpStandardWithAlpha.InitialColor = cdStandardAlpha.Color;
////            cpStandardWithAlpha.SelectedColor = cdStandardAlpha.Color;
        }
    }
}
