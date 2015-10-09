namespace LocultApp.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Multivalue converter converts a Boolean value into a transparent color (on false)
    /// or a supplied target color (on true).
    /// </summary>
    [ValueConversion(typeof(object), typeof(System.Windows.Media.SolidColorBrush))]
    public class AlternateColorConverter : IMultiValueConverter
    {
        #region constructor
        /// <summary>
        /// Standard Constructor
        /// </summary>
        public AlternateColorConverter()
        {
        }
        #endregion constructor

        #region IMultiValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if ((values is object[]) == false)
                return Binding.DoNothing;

            bool showAlternateGridColor = true;
            Color alternateGridColor = Color.FromScRgb(0x10, 0x00, 0x00, 0x00); // Alternating color

            if (values.Length >= 1)
            {
                if (values[0] is bool)
                    showAlternateGridColor = (bool)values[0];

                if (values[0] is Color)
                    alternateGridColor = (Color)values[0];
            }

            if (values.Length >= 2)
            {
                if (values[1] is bool)
                    showAlternateGridColor = (bool)values[1];

                if (values[1] is Color)
                    alternateGridColor = (Color)values[1];
            }

            // Not showing alternating colors equates to showing transparent colors
            if (showAlternateGridColor == false)
                return new System.Windows.Media.SolidColorBrush(Color.FromScRgb(0, 0, 0, 255)); // Transparent

            // Showing alternating grid color
            return new System.Windows.Media.SolidColorBrush(alternateGridColor);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion IMultiValueConverter
    }
}
