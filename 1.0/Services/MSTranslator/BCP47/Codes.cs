namespace MSTranslate
{
    using MSTranslate.Interfaces;
    using System.Collections.Generic;

    internal class CultureLanguage : ILanguageCode
    {
        public CultureLanguage()
        {
            list = new List<CultureLanguageCountry>();
        }

        public CultureLanguage(string langCode, string lang, string area = "")
            : this()
        {
            this.Bcp47_LangCode = langCode;
            this.Language = lang;
            this.Area = area;
        }

        public CultureLanguage(CultureLanguage copyThis)
            : this()
        {
            if (copyThis == null)
                return;

            Bcp47_LangCode = copyThis.Bcp47_LangCode;
            Language = copyThis.Language;
            Area = copyThis.Area;
        }

        public string Bcp47_LangCode { get; private set; }
        public string Language { get; private set; }

        public string Area { get; private set; }

        public List<CultureLanguageCountry> list { get; private set; }

        public string FormatedString
        {
            get
            {
                return string.Format("{0} {1} ({2})", Bcp47_LangCode, this.Language, this.Area);
            }
        }
    }

    internal class CultureLanguageCountry
    {
        public CultureLanguageCountry(string langCode, string lang, string country)
        {
            this.langCode = langCode;
            this.lang = lang;
            this.country = country;
        }

        public string langCode { get; set; }
        public string lang { get; set; }
        public string country { get; set; }
    }

    internal class Country
    {
        public Country(string name)
        {
            this.name = name;
        }

        public string name { get; set; }

        internal List<CultureLanguageArea> list = new List<CultureLanguageArea>();
    }

    internal class CultureLanguageArea
    {
        public CultureLanguageArea(string name, string language, string area)
        {
            this.name = name;
            this.language = language;
            country = area;
        }

        public string name { get; set; }
        public string language { get; set; }
        public string country { get; set; }
    }

    /// <summary>
    /// Class was generated via XML transformation
    /// Data Source: https://msdn.microsoft.com/en-us/library/windows/apps/system.globalization.cultureinfo%28v=vs.105%29.aspx
    /// 
    /// Created: 6/4/2015 6:09:07 PM
    /// </summary>
    internal class Codes
    {
        public Codes()
        {
            list = CreateList();
        }

        public List<object> list { get; private set; }

        /// <summary>
        /// Method creates a list of CultureLanguage and Country objects
        /// (and their children) and returns it.
        /// </summary>
        /// <returns></returns>
        internal List<object> CreateList()
        {
            List<object> list = new List<object>();
            CultureLanguage item;
            Country countryItem;

            item = new CultureLanguage(langCode: "af", lang: "Afrikaans");
            item.list.Add(new CultureLanguageCountry(langCode: "af-ZA", lang: "Afrikaans", country: "South Africa"));
            list.Add(item);

            item = new CultureLanguage(langCode: "sq", lang: "Albanian");
            item.list.Add(new CultureLanguageCountry(langCode: "sq-AL", lang: "Albanian", country: "Albania"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ar", lang: "Arabic");
            item.list.Add(new CultureLanguageCountry(langCode: "ar-DZ", lang: "Arabic", country: "Algeria"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-BH", lang: "Arabic", country: "Bahrain"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-EG", lang: "Arabic", country: "Egypt"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-IQ", lang: "Arabic", country: "Iraq"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-JO", lang: "Arabic", country: "Jordan"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-KW", lang: "Arabic", country: "Kuwait"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-LB", lang: "Arabic", country: "Lebanon"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-LY", lang: "Arabic", country: "Libya"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-MA", lang: "Arabic", country: "Morocco"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-OM", lang: "Arabic", country: "Oman"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-QA", lang: "Arabic", country: "Qatar"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-SA", lang: "Arabic", country: "Saudi Arabia"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-SY", lang: "Arabic", country: "Syria"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-TN", lang: "Arabic", country: "Tunisia"));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-AE", lang: "Arabic", country: "U.A.E."));
            item.list.Add(new CultureLanguageCountry(langCode: "ar-YE", lang: "Arabic", country: "Yemen"));
            list.Add(item);

            item = new CultureLanguage(langCode: "hy", lang: "Armenian");
            item.list.Add(new CultureLanguageCountry(langCode: "hy-AM", lang: "Armenian", country: "Armenia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "az", lang: "Azeri");
            item.list.Add(new CultureLanguageCountry(langCode: "az-Cyrl-AZ", lang: "Azeri", country: "Azerbaijan, Cyrillic"));
            item.list.Add(new CultureLanguageCountry(langCode: "az-Latn-AZ", lang: "Azeri", country: "Azerbaijan, Latin"));
            list.Add(item);

            item = new CultureLanguage(langCode: "eu", lang: "Basque");
            item.list.Add(new CultureLanguageCountry(langCode: "eu-ES", lang: "Basque", country: "Basque"));
            list.Add(item);

            item = new CultureLanguage(langCode: "be", lang: "Belarusian");
            item.list.Add(new CultureLanguageCountry(langCode: "be-BY", lang: "Belarusian", country: "Belarus"));
            list.Add(item);

            item = new CultureLanguage(langCode: "bg", lang: "Bulgarian");
            item.list.Add(new CultureLanguageCountry(langCode: "bg-BG", lang: "Bulgarian", country: "Bulgaria"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ca", lang: "Catalan");
            item.list.Add(new CultureLanguageCountry(langCode: "ca-ES", lang: "Catalan", country: "Catalan"));
            list.Add(item);

            item = new CultureLanguage(langCode: "hr", lang: "Croatian");
            item.list.Add(new CultureLanguageCountry(langCode: "hr-BA", lang: "Croatian", country: "Bosnia and Herzegovina"));
            item.list.Add(new CultureLanguageCountry(langCode: "hr-HR", lang: "Croatian", country: "Croatia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "cs", lang: "Czech");
            item.list.Add(new CultureLanguageCountry(langCode: "cs-CZ", lang: "Czech", country: "Czech Republic"));
            list.Add(item);

            item = new CultureLanguage(langCode: "da", lang: "Danish");
            item.list.Add(new CultureLanguageCountry(langCode: "da-DK", lang: "Danish", country: "Denmark"));
            list.Add(item);

            item = new CultureLanguage(langCode: "dv", lang: "Divehi");
            item.list.Add(new CultureLanguageCountry(langCode: "dv-MV", lang: "Divehi", country: "Maldives"));
            list.Add(item);

            item = new CultureLanguage(langCode: "nl", lang: "Dutch");
            item.list.Add(new CultureLanguageCountry(langCode: "nl-BE", lang: "Dutch", country: "Belgium"));
            item.list.Add(new CultureLanguageCountry(langCode: "nl-NL", lang: "Dutch", country: "Netherlands"));
            list.Add(item);

            item = new CultureLanguage(langCode: "en", lang: "English");
            item.list.Add(new CultureLanguageCountry(langCode: "en-AU", lang: "English", country: "Australia"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-BZ", lang: "English", country: "Belize"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-CA", lang: "English", country: "Canada"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-029", lang: "English", country: "Caribbean"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-IE", lang: "English", country: "Ireland"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-JM", lang: "English", country: "Jamaica"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-NZ", lang: "English", country: "New Zealand"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-PH", lang: "English", country: "Philippines"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-ZA", lang: "English", country: "South Africa"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-TT", lang: "English", country: "Trinidad and Tobago"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-GB", lang: "English", country: "United Kingdom"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-US", lang: "English", country: "United States"));
            item.list.Add(new CultureLanguageCountry(langCode: "en-ZW", lang: "English", country: "Zimbabwe"));
            list.Add(item);

            item = new CultureLanguage(langCode: "et", lang: "Estonian");
            item.list.Add(new CultureLanguageCountry(langCode: "et-EE", lang: "Estonian", country: "Estonia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "fo", lang: "Faroese");
            item.list.Add(new CultureLanguageCountry(langCode: "fo-FO", lang: "Faroese", country: "Faroe Islands"));
            list.Add(item);

            item = new CultureLanguage(langCode: "fa", lang: "Farsi");
            item.list.Add(new CultureLanguageCountry(langCode: "fa-IR", lang: "Farsi", country: "Iran"));
            list.Add(item);

            item = new CultureLanguage(langCode: "fi", lang: "Finnish");
            item.list.Add(new CultureLanguageCountry(langCode: "fi-FI", lang: "Finnish", country: "Finland"));
            list.Add(item);

            item = new CultureLanguage(langCode: "fr", lang: "French");
            item.list.Add(new CultureLanguageCountry(langCode: "fr-BE", lang: "French", country: "Belgium"));
            item.list.Add(new CultureLanguageCountry(langCode: "fr-CA", lang: "French", country: "Canada"));
            item.list.Add(new CultureLanguageCountry(langCode: "fr-FR", lang: "French", country: "France"));
            item.list.Add(new CultureLanguageCountry(langCode: "fr-LU", lang: "French", country: "Luxembourg"));
            item.list.Add(new CultureLanguageCountry(langCode: "fr-MC", lang: "French", country: "Monaco"));
            item.list.Add(new CultureLanguageCountry(langCode: "fr-CH", lang: "French", country: "Switzerland"));
            list.Add(item);

            item = new CultureLanguage(langCode: "gl", lang: "Galician");
            item.list.Add(new CultureLanguageCountry(langCode: "gl-ES", lang: "Galician", country: "Spain"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ka", lang: "Georgian");
            item.list.Add(new CultureLanguageCountry(langCode: "ka-GE", lang: "Georgian", country: "Georgia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "de", lang: "German");
            item.list.Add(new CultureLanguageCountry(langCode: "de-AT", lang: "German", country: "Austria"));
            item.list.Add(new CultureLanguageCountry(langCode: "de-DE", lang: "German", country: "Germany"));
            item.list.Add(new CultureLanguageCountry(langCode: "de-DE_phoneb", lang: "German", country: "Germany, phone book sort"));
            item.list.Add(new CultureLanguageCountry(langCode: "de-LI", lang: "German", country: "Liechtenstein"));
            item.list.Add(new CultureLanguageCountry(langCode: "de-LU", lang: "German", country: "Luxembourg"));
            item.list.Add(new CultureLanguageCountry(langCode: "de-CH", lang: "German", country: "Switzerland"));
            list.Add(item);

            item = new CultureLanguage(langCode: "el", lang: "Greek");
            item.list.Add(new CultureLanguageCountry(langCode: "el-GR", lang: "Greek", country: "Greece"));
            list.Add(item);

            item = new CultureLanguage(langCode: "gu", lang: "Gujarati");
            item.list.Add(new CultureLanguageCountry(langCode: "gu-IN", lang: "Gujarati", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "he", lang: "Hebrew");
            item.list.Add(new CultureLanguageCountry(langCode: "he-IL", lang: "Hebrew", country: "Israel"));
            list.Add(item);

            item = new CultureLanguage(langCode: "hi", lang: "Hindi");
            item.list.Add(new CultureLanguageCountry(langCode: "hi-IN", lang: "Hindi", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "hu", lang: "Hungarian");
            item.list.Add(new CultureLanguageCountry(langCode: "hu-HU", lang: "Hungarian", country: "Hungary"));
            list.Add(item);

            item = new CultureLanguage(langCode: "is", lang: "Icelandic");
            item.list.Add(new CultureLanguageCountry(langCode: "is-IS", lang: "Icelandic", country: "Iceland"));
            list.Add(item);

            item = new CultureLanguage(langCode: "id", lang: "Indonesian");
            item.list.Add(new CultureLanguageCountry(langCode: "id-ID", lang: "Indonesian", country: "Indonesia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "it", lang: "Italian");
            item.list.Add(new CultureLanguageCountry(langCode: "it-IT", lang: "Italian", country: "Italy"));
            item.list.Add(new CultureLanguageCountry(langCode: "it-CH", lang: "Italian", country: "Switzerland"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ja", lang: "Japanese");
            item.list.Add(new CultureLanguageCountry(langCode: "ja-JP", lang: "Japanese", country: "Japan"));
            list.Add(item);

            item = new CultureLanguage(langCode: "kn", lang: "Kannada");
            item.list.Add(new CultureLanguageCountry(langCode: "kn-IN", lang: "Kannada", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "kk", lang: "Kazakh");
            item.list.Add(new CultureLanguageCountry(langCode: "kk-KZ", lang: "Kazakh", country: "Kazakhstan"));
            list.Add(item);

            item = new CultureLanguage(langCode: "kok", lang: "Konkani");
            item.list.Add(new CultureLanguageCountry(langCode: "kok-IN", lang: "Konkani", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ko", lang: "Korean");
            item.list.Add(new CultureLanguageCountry(langCode: "ko-KR", lang: "Korean", country: "Korea"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ky", lang: "Kyrgyz");
            item.list.Add(new CultureLanguageCountry(langCode: "ky-KG", lang: "Kyrgyz", country: "Kyrgyzstan"));
            list.Add(item);

            item = new CultureLanguage(langCode: "lv", lang: "Latvian");
            item.list.Add(new CultureLanguageCountry(langCode: "lv-LV", lang: "Latvian", country: "Latvia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "lt", lang: "Lithuanian");
            item.list.Add(new CultureLanguageCountry(langCode: "lt-LT", lang: "Lithuanian", country: "Lithuania"));
            list.Add(item);

            item = new CultureLanguage(langCode: "mk", lang: "Macedonian");
            item.list.Add(new CultureLanguageCountry(langCode: "mk-MK", lang: "Macedonian", country: "Macedonia, FYROM"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ms", lang: "Malay");
            item.list.Add(new CultureLanguageCountry(langCode: "ms-BN", lang: "Malay", country: "Brunei Darussalam"));
            item.list.Add(new CultureLanguageCountry(langCode: "ms-MY", lang: "Malay", country: "Malaysia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "mr", lang: "Marathi");
            item.list.Add(new CultureLanguageCountry(langCode: "mr-IN", lang: "Marathi", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "mn", lang: "Mongolian");
            item.list.Add(new CultureLanguageCountry(langCode: "mn-MN", lang: "Mongolian", country: "Mongolia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "no", lang: "Norwegian");
            item.list.Add(new CultureLanguageCountry(langCode: "nb-NO", lang: "Norwegian", country: "Bokmål, Norway"));
            item.list.Add(new CultureLanguageCountry(langCode: "nn-NO", lang: "Norwegian", country: "Nynorsk, Norway"));
            list.Add(item);

            item = new CultureLanguage(langCode: "pl", lang: "Polish");
            item.list.Add(new CultureLanguageCountry(langCode: "pl-PL", lang: "Polish", country: "Poland"));
            list.Add(item);

            item = new CultureLanguage(langCode: "pt", lang: "Portuguese");
            item.list.Add(new CultureLanguageCountry(langCode: "pt-BR", lang: "Portuguese", country: "Brazil"));
            item.list.Add(new CultureLanguageCountry(langCode: "pt-PT", lang: "Portuguese", country: "Portugal"));
            list.Add(item);

            item = new CultureLanguage(langCode: "pa", lang: "Punjabi");
            item.list.Add(new CultureLanguageCountry(langCode: "pa-IN", lang: "Punjabi", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ro", lang: "Romanian");
            item.list.Add(new CultureLanguageCountry(langCode: "ro-RO", lang: "Romanian", country: "Romania"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ru", lang: "Russian");
            item.list.Add(new CultureLanguageCountry(langCode: "ru-RU", lang: "Russian", country: "Russia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "sa", lang: "Sanskrit");
            item.list.Add(new CultureLanguageCountry(langCode: "sa-IN", lang: "Sanskrit", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "sk", lang: "Slovak");
            item.list.Add(new CultureLanguageCountry(langCode: "sk-SK", lang: "Slovak", country: "Slovakia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "sl", lang: "Slovenian");
            item.list.Add(new CultureLanguageCountry(langCode: "sl-SI", lang: "Slovenian", country: "Slovenia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "es", lang: "Spanish");
            item.list.Add(new CultureLanguageCountry(langCode: "es-AR", lang: "Spanish", country: "Argentina"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-BO", lang: "Spanish", country: "Bolivia"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-CL", lang: "Spanish", country: "Chile"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-CO", lang: "Spanish", country: "Colombia"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-CR", lang: "Spanish", country: "Costa Rica"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-DO", lang: "Spanish", country: "Dominican Republic"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-EC", lang: "Spanish", country: "Ecuador"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-SV", lang: "Spanish", country: "El Salvador"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-GT", lang: "Spanish", country: "Guatemala"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-HN", lang: "Spanish", country: "Honduras"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-MX", lang: "Spanish", country: "Mexico"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-NI", lang: "Spanish", country: "Nicaragua"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-PA", lang: "Spanish", country: "Panama"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-PY", lang: "Spanish", country: "Paraguay"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-PE", lang: "Spanish", country: "Peru"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-PR", lang: "Spanish", country: "Puerto Rico"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-ES", lang: "Spanish", country: "Spain"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-ES_tradnl", lang: "Span", country: "Span, Traditional Sort"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-UY", lang: "Spanish", country: "Uruguay"));
            item.list.Add(new CultureLanguageCountry(langCode: "es-VE", lang: "Spanish", country: "Venezuela"));
            list.Add(item);

            item = new CultureLanguage(langCode: "sw", lang: "Swahili");
            item.list.Add(new CultureLanguageCountry(langCode: "sw-KE", lang: "Swahili", country: "Kenya"));
            list.Add(item);

            item = new CultureLanguage(langCode: "sv", lang: "Swedish");
            item.list.Add(new CultureLanguageCountry(langCode: "sv-FI", lang: "Swedish", country: "Finland"));
            item.list.Add(new CultureLanguageCountry(langCode: "sv-SE", lang: "Swedish", country: "Sweden"));
            list.Add(item);

            item = new CultureLanguage(langCode: "syr", lang: "Syriac");
            item.list.Add(new CultureLanguageCountry(langCode: "syr-SY", lang: "Syriac", country: "Syria"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ta", lang: "Tamil");
            item.list.Add(new CultureLanguageCountry(langCode: "ta-IN", lang: "Tamil", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "tt", lang: "Tatar");
            item.list.Add(new CultureLanguageCountry(langCode: "tt-RU", lang: "Tatar", country: "Russia"));
            list.Add(item);

            item = new CultureLanguage(langCode: "te", lang: "Telugu");
            item.list.Add(new CultureLanguageCountry(langCode: "te-IN", lang: "Telugu", country: "India"));
            list.Add(item);

            item = new CultureLanguage(langCode: "th", lang: "Thai");
            item.list.Add(new CultureLanguageCountry(langCode: "th-TH", lang: "Thai", country: "Thailand"));
            list.Add(item);

            item = new CultureLanguage(langCode: "tr", lang: "Turkish");
            item.list.Add(new CultureLanguageCountry(langCode: "tr-TR", lang: "Turkish", country: "Turkey"));
            list.Add(item);

            item = new CultureLanguage(langCode: "uk", lang: "Ukrainian");
            item.list.Add(new CultureLanguageCountry(langCode: "uk-UA", lang: "Ukrainian", country: "Ukraine"));
            list.Add(item);

            item = new CultureLanguage(langCode: "ur", lang: "Urdu");
            item.list.Add(new CultureLanguageCountry(langCode: "ur-PK", lang: "Urdu", country: "Pakistan"));
            list.Add(item);

            item = new CultureLanguage(langCode: "uz", lang: "Uzbek");
            item.list.Add(new CultureLanguageCountry(langCode: "uz-Cyrl-UZ", lang: "Uzbek", country: "Uzbekistan, Cyrillic"));
            item.list.Add(new CultureLanguageCountry(langCode: "uz-Latn-UZ", lang: "Uzbek", country: "Uzbekistan, Latin"));
            list.Add(item);

            item = new CultureLanguage(langCode: "vi", lang: "Vietnamese");
            item.list.Add(new CultureLanguageCountry(langCode: "vi-VN", lang: "Vietnamese", country: "Vietnam"));
            list.Add(item);

            countryItem = new Country(name: "China");
            countryItem.list.Add(new CultureLanguageArea(name: "zh-HK", language: "Chinese", area: "Hong Kong SAR, PRC"));
            countryItem.list.Add(new CultureLanguageArea(name: "zh-MO", language: "Chinese", area: "Macao SAR"));
            countryItem.list.Add(new CultureLanguageArea(name: "zh-SG", language: "Chinese", area: "Singapore"));
            countryItem.list.Add(new CultureLanguageArea(name: "zh-TW", language: "Chinese", area: "Taiwan"));
            countryItem.list.Add(new CultureLanguageArea(name: "zh-CN", language: "Chinese", area: "PRC"));
            countryItem.list.Add(new CultureLanguageArea(name: "zh-CHS", language: "Chinese", area: "Simplified"));
            countryItem.list.Add(new CultureLanguageArea(name: "zh-CHT", language: "Chinese", area: "Traditional"));
            list.Add(countryItem);

            countryItem = new Country(name: "Serbia");
            countryItem.list.Add(new CultureLanguageArea(name: "sr-Cyrl-CS", language: "Serbian", area: "Serbia, Cyrillic"));
            countryItem.list.Add(new CultureLanguageArea(name: "sr-Latn-CS", language: "Serbian", area: "Serbia, Latin"));
            list.Add(countryItem);

            return list;
        }
    }
}
