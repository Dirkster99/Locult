namespace MSTranslate.Interfaces
{
    using System;

    public interface ILanguageCode
    {
        /// <summary>
        /// Gets the BCP 47 code for this language id item.
        /// </summary>
        string Bcp47_LangCode { get; }

        /// <summary>
        /// Gets the English name of this language.
        /// </summary>
        string Language { get; }

        /// <summary>
        /// Gets the English name of the are in which this language is spoken.
        /// </summary>
        string Area { get; }

        string FormatedString { get;  }
    }
}
