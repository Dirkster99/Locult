namespace LocultApp.Converter
{
  using System;
  using System.Windows.Data;
  using System.Windows.Markup;

  /// <summary>
  /// Convert string.empty into null value to indicate
  /// presentation level that we really have nothing to show here.
  /// </summary>
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  [ValueConversion(typeof(string), typeof(string))]
  public class EmptyStringToNullConverter : MarkupExtension, IValueConverter
  {
    #region field
    private static EmptyStringToNullConverter converter;
    #endregion field

    #region constructor
    /// <summary>
    /// Standard Constructor
    /// </summary>
    public EmptyStringToNullConverter()
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
        converter = new EmptyStringToNullConverter();
      }
  
      return converter;
    }
    #endregion MarkupExtension

    #region IValueConverter
    /// <summary>
    /// Null to visibility conversion method
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return Binding.DoNothing;

      if ((value is string) == false)
        return Binding.DoNothing;

      string s = value as string;

      if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
        return null;

      return s;
    }

    /// <summary>
    /// Visibility to Null conversion method (is not implemented)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return Binding.DoNothing;
    }
    #endregion IValueConverter
  }
}
