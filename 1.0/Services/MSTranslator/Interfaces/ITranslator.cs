namespace MSTranslate.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading;

    /// <summary>
    /// Determine whether the source or target file of a diff should be processed.
    /// </summary>
    public enum ProcessDestination
    {
        /// <summary>
        /// Process the source file.
        /// </summary>
        Source,

        /// <summary>
        /// Process the target file.
        /// </summary>
        Target
    }

    /// <summary>
    /// Defines an interface for an object that connects to a Microsoft Azure Web service
    /// to translate strings from one language into another.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Returns an alphabetically ordered List of language objects that represent
        /// languages and their language code supported by this service.
        /// </summary>
        List<ILanguageCode> LanguageList { get; }

        /// <summary>
        /// Get translated text from Bing Translator service.
        /// </summary>
        /// <param name="textToTranslate">Text to translate.</param>
        /// <param name="fromLang">Language to translate from.</param>
        /// <param name="toLang">Language to translate to.</param>
        /// <returns>Translated text.</returns>
        List<string> GetTranslatedText(string textToTranslate,
                                       string fromLang,
                                       string toLang,
                                       ILoginCredentials login);

        /// <summary>
        /// Creates an object that keeps login information for MS Translate API service
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        ILoginCredentials CreateLoginCredentials(Uri uri, SecureString user, SecureString password);

        /// <summary>
        /// Checks basic parameters of translation to make sure known error are dealt with in advance.
        /// </summary>
        /// <param name="TranslationServiceUri"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="sourceFileLanguage"></param>
        /// <param name="targetFileLanguage"></param>
        /// <param name="destination"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        ITranslatorError Check(string TranslationServiceUri,
                               SecureString user,
                               SecureString password,
                               string sourceFileLanguage, string targetFileLanguage,
                               ProcessDestination destination,
                               CancellationTokenSource cts);
    }
}
