namespace MSTranslate
{
    using MSTranslate.ExtensionMethods;
    using MSTranslate.Interfaces;
    using MSTranslate.Service;
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using System.Net;
    using System.Security;

    /// <summary>
    /// Connects to a Microsoft Azure Web service to translate strings from one language into another.
    /// </summary>
    public class Translator : ITranslator
    {
        #region fields
        private List<ILanguageCode> mLangList = null;
        #endregion fields

        #region constructor
        /// <summary>
        /// Standard class constructor
        /// </summary>
        public Translator()
        {
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Returns an alphabetically ordered List of language objects that represent
        /// languages and their language code supported by this service.
        /// </summary>
        List<ILanguageCode> ITranslator.LanguageList
        {
            get
            {
                if (mLangList == null)
                {
                    var bcp47 = new Codes();
                    var list = new List<ILanguageCode>();

                    foreach (var item in bcp47.list)
                    {
                        var cultureLanguage = item as CultureLanguage;
                        if (cultureLanguage != null)
                        {
                            // add a copy of this and avoid its sub-items
                            list.Add(new CultureLanguage(cultureLanguage));

                            foreach (var clItem in cultureLanguage.list)
                            {
                                list.Add(new CultureLanguage(clItem.langCode, clItem.lang, clItem.country));
                            }
                        }
                        else
                        {
                            var country = item as Country;
                            if (country != null)
                            {
                                foreach (var countryItem in country.list)
                                {
                                    // add a copy of this
                                    list.Add(new CultureLanguage(countryItem.name,
                                                                 countryItem.language,
                                                                 countryItem.country));
                                }
                            }
                        }
                    }

                    mLangList = list.OrderBy(l => l.Bcp47_LangCode).ToList();
                }

                return mLangList;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Creates an object that keeps login information for MS Translate API service
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        Interfaces.ILoginCredentials ITranslator.CreateLoginCredentials(Uri uri,
                                                                        SecureString user,
                                                                        SecureString password)
        {
            return new LoginCredentials
            (
                // this is the service root uri for the Microsoft Translator service  
                //new Uri("https://api.datamarket.azure.com/Bing/MicrosoftTranslator/"),
                //
                // this is the Account Key generated for an app (not all  characters are real key characters)
                // "oHMSFuynxyzp26KOmysK7WMF2DakuBkF2BhPFSNQP/g"

                uri, user, password
            );

        }

        /// <summary>
        /// Get translated text from Bing Translator service.
        /// </summary>
        /// <param name="textToTranslate">Text to translate.</param>
        /// <param name="fromLang">Language to translate from.</param>
        /// <param name="toLang">Language to translate to.</param>
        /// <returns>Translated text.</returns>
        List<string> ITranslator.GetTranslatedText(string textToTranslate,
                                                   string fromLang,
                                                   string toLang,
                                                   ILoginCredentials login)
        {
            if (login == null)
                return null;

            // the TranslatorContainer gives us access to the Microsoft Translator services 
            MSTranslate.Service.TranslatorContainer tc = new MSTranslate.Service.TranslatorContainer(login.TranslationServiceUri);

            // Give the TranslatorContainer access to your subscription 
            tc.Credentials = new NetworkCredential(SecureStringExtensionMethod.ConvertToUnsecureString(login.User),
                                                   login.Password);

            try
            {
                // Generate the query 
                DataServiceQuery<Translation> translationQuery = tc.Translate(textToTranslate, toLang, fromLang);

                // Call the query and get the results as a List 
                var translationResults = translationQuery.Execute().ToList();

                // Verify there was a result 
                if (translationResults.Count() <= 0)
                    return new List<string>();

                // In case there were multiple results, pick the first one 
                var translationResult = translationResults.First();

                List<string> ret = new List<string>();

                for (int i = 0; i < translationResults.Count(); i++)
                {
                    ret.Add(translationResult.Text);
                }

                return ret;
            }
            catch (Exception exc)
            {
                throw new Exception(string.Format("Translation text:'{0}', toLang: '{1}', fromLang '{2}'", textToTranslate, toLang, fromLang), exc);
            }
        }

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
        /// <returns>Return null if everythings OK. Returns an error object describing a problem
        /// if a pre-requisite parameter is not as required.</returns>
        ITranslatorError ITranslator.Check(string translationServiceUri,
                                           SecureString user,
                                           SecureString password,
                                           string sourceFileLanguage,
                                           string targetFileLanguage,
                                           ProcessDestination destination,
                                           System.Threading.CancellationTokenSource cts)
        {
            if (IsNullOrEmpty(user) == true)
                return TranslatorError.GetErrorObject(TranslateErrorCode.Missing_Login_AuthenticationParameter);

            if (IsNullOrEmpty(password) == true)
                return TranslatorError.GetErrorObject(TranslateErrorCode.Missing_Login_AuthenticationParameter);

            try
            {
                // Check if Uri appears to be formatted well and generate error if not.
                new Uri(translationServiceUri);
            }
            catch
            {
                return TranslatorError.GetErrorObject(TranslateErrorCode.Missing_URI_AuthenticationParameter);
            }

            return null;
        }

        /// <summary>
        /// Returns true if secure string appears to be empty, otherwise false.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool IsNullOrEmpty(SecureString s)
        {
            if (s == null)
                return true;

            if (s.Length == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Returns true if secure string appears to be empty, otherwise false.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool IsNullOrEmpty(Uri s)
        {
            if (s == null)
                return true;

            if (string.IsNullOrEmpty(s.AbsolutePath) == true)
                return true;

            return false;
        }
        #endregion methods
    }
}
