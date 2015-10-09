namespace Settings.Interfaces
{
    using System;
    using System.Security;

    /// <summary>
    /// Defines an interface that can be used to determine login properties
    /// for Microsoft's Azure translation service.
    /// </summary>
    public interface ITranslationService
    {
        /// <summary>
        /// Gets/sets the address of Microsoft's automated translation service.
        /// This should be: https://api.datamarket.azure.com/Bing/MicrosoftTranslator/
        /// </summary>
        string TranslationServiceUri { get; set; }

        /// <summary>
        /// Gets/sets the user of name of the API.
        /// </summary>
        SecureString TranslationServiceUser { get; set; }

        /// <summary>
        /// Gets/sets the user of password of the API.
        /// </summary>
        SecureString TranslationServicePassword { get; set; }
    }
}
