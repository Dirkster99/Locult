namespace LocultApp.Converter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Source: http://stackoverflow.com/questions/15952812/multiple-command-parameters-wpf-button-object
    /// </summary>
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    public class MultiParametersConverter : MarkupExtension, IMultiValueConverter
    {
        #region field
        private static MultiParametersConverter converter;
        #endregion field

        #region constructor
        /// <summary>
        /// Standard Constructor
        /// </summary>
        public MultiParametersConverter()
        {
        }
        #endregion constructor

        #region MarkupExtension
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
                converter = new MultiParametersConverter();
            }

            return converter;
        }
        #endregion MarkupExtension

        #region IMultiValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if ((values is object[]) == false)
                return Binding.DoNothing;

            if (values.Length == 0)
                return false;

            string[] ret = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                ret[i] = values[i] as string;
            }

            return ret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion IMultiValueConverter
    }
}
