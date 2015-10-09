namespace LocultApp.Converter
{
    using System;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Convert the properties of a language coded object into a nice human readable string.
    /// </summary>
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    public class LangCodesToStringMultiValueConverter : MarkupExtension, IMultiValueConverter
    {
        private static LangCodesToStringMultiValueConverter converter;

        /// <summary>
        /// Converter class
        /// </summary>
        public LangCodesToStringMultiValueConverter()
        {
        }

        #region IValueConverter Members
        /// <summary>
        /// When implemented in a derived class, returns an object that is provided
        /// as the value of the target property for this markup extension.
        /// 
        /// When a XAML processor processes a type node and member value that is a markup extension,
        /// it invokes the ProvideValue method of that markup extension and writes the result into the
        /// object graph or serialization stream. The XAML object writer passes service context to each
        /// such implementation through the serviceProvider parameter.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new LangCodesToStringMultiValueConverter();
            }

            return converter;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if (values.Length != 3)
                return Binding.DoNothing;

            string langCode = values[0] as string;
            string language = values[1] as string;
            string area     = values[2] as string;

            if (string.IsNullOrEmpty(area))
                return string.Format("{0} {1}", langCode, language);
            else
                return string.Format("{0} {1} ({2})", langCode, language, area);
        }

        /// <summary>
        /// Disabled convert back method (throws an exception upon being called)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
