namespace LocultApp.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using TranslationSolutionViewModelLib.ViewModels;

    /// <summary>
    /// Class implements a multivalue converter XAML Extension that converts a given type of solution item
    /// and state into a corresponding reference to an (image) resource.
    /// </summary>
    [ValueConversion(typeof(TranslationSolutionViewModelLib.ViewModels.ItemExisits), typeof(object))]
    [ValueConversion(typeof(TranslationSolutionViewModelLib.ViewModels.TypeOfSolutionItem), typeof(object))]
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    public class SolutionItemEnumToImage : MarkupExtension, IMultiValueConverter
    {
        private static SolutionItemEnumToImage converter;

        public SolutionItemEnumToImage() : base()
        {
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension. 
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">
        /// Object that can provide services for the markup extension.
        /// </param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new SolutionItemEnumToImage();
            }

            return converter;
        }

        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Check input parameter types
            if (values == null)
                return Binding.DoNothing;

            if (values.Length != 2)
                return Binding.DoNothing;

            TypeOfSolutionItem type = TypeOfSolutionItem.Unknown;
            bool typeIsAssigned = false;
            ItemExisits state = ItemExisits.Unknown;
            bool stateIsAssigned = false;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is TypeOfSolutionItem)
                {
                    type = (TypeOfSolutionItem)values[i];
                    typeIsAssigned = true;
                }
                else
                {
                    if (values[i] is ItemExisits)
                    {
                        state = (ItemExisits)values[i];
                        stateIsAssigned = true;
                    }
                }
            }

            if (typeIsAssigned == false)
                throw new ArgumentException(string.Format("Expected argument: '{0}' was not supplied.", type.GetType().ToString()));

            if (stateIsAssigned == false)
                throw new ArgumentException(string.Format("Expected argument: '{0}' was not supplied.", state.GetType().ToString()));

            if (targetType != typeof(System.Windows.Media.ImageSource))
                throw new ArgumentException("Invalid return type. Expected return type: System.Windows.Media.ImageSource");

            string resourceUri = string.Empty;

            string stateExtension = string.Empty;

            switch (state)
            {
                case ItemExisits.Unknown:
                    stateExtension = "Unknown";
                    break;
                case ItemExisits.DoesExist:
                    stateExtension = string.Empty;
                    break;
                case ItemExisits.DoesNotExist:
                    stateExtension = "Problem";
                    break;

                default:
                    throw new NotImplementedException(state.ToString());
            }

            string typeURL = string.Empty;

            switch (type)
            {
                case TypeOfSolutionItem.Root:
                    typeURL = "Icon_Solution";
                    break;
                case TypeOfSolutionItem.Project:
                    typeURL = "Icon_Project";
                    break;
                case TypeOfSolutionItem.File:
                    typeURL = "Icon_Documents";
                    break;
                case TypeOfSolutionItem.Unknown:
                default:
                    throw new NotImplementedException(state.ToString());
            }
            string resourceKey = string.Format("{0}{1}", typeURL,
                                                         (string.IsNullOrEmpty(stateExtension) ? string.Empty :
                                                                                                 "_" + stateExtension));
            return Application.Current.FindResource(resourceKey);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This converter supports only one-way conversion");
        }
        #endregion IMultiValueConverter
    }
}
