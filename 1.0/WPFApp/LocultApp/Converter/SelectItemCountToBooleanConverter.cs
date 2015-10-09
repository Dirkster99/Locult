namespace LocultApp.Converter
{
  using System;
  using System.Windows.Data;
  using System.Windows.Markup;
  using LocultApp.ViewModels.StringSource;

  /// <summary>
  /// XAML mark up extension to convert an int (count) value to Boolean.
  /// </summary>
  [MarkupExtensionReturnType(typeof(IValueConverter))]
  [ValueConversion(typeof(int), typeof(Boolean))]
  public class SelectItemCountToBooleanConverter : MarkupExtension, IValueConverter
  {
    #region field
    private static SelectItemCountToBooleanConverter converter;
    #endregion field

    #region constructor
    /// <summary>
    /// Standard Constructor
    /// </summary>
    public SelectItemCountToBooleanConverter()
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
        converter = new SelectItemCountToBooleanConverter();
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
        return false;

      if ((value is int) == false)
        return false;

      int iValue = (int)value;

      if (iValue == 1)
        return true;

      return false;
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
