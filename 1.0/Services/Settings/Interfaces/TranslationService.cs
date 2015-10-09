namespace Settings.Translate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "TranslationService", IsNullable = false)]
    public class TranslationService : Settings.Interfaces.ITranslationService
    {
        #region fields
        private string mTranslationServiceUri;
        private SecureString mTranslationServicePassword;
        private SecureString mTranslationServiceUser;
        private bool mIsDirty;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public TranslationService(string translationServiceUri = "https://api.datamarket.azure.com/Bing/MicrosoftTranslator/",
                                  SecureString translationServiceUser = null,
                                  SecureString translationServicePassword = null)
            :this()
        {
            mTranslationServiceUri = translationServiceUri;
            mTranslationServiceUser = translationServiceUser;

            if (translationServicePassword != null)
                mTranslationServicePassword = translationServicePassword;

            mIsDirty = false;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public TranslationService()
        {
            mTranslationServiceUri = "https://api.datamarket.azure.com/Bing/MicrosoftTranslator/";
            mTranslationServiceUser = new SecureString();
            mTranslationServicePassword = new SecureString();
            mIsDirty = false;
        }

        /// <summary>
        /// Copy Class constructor
        /// Copy all field values and set IsDirty = false
        /// </summary>
        public TranslationService(TranslationService copyThis)
            :this()
        {
            if (copyThis == null)
                return;

            this.TranslationServiceUri = copyThis.TranslationServiceUri;
            this.TranslationServicePassword = copyThis.TranslationServicePassword.Copy();
            this.TranslationServiceUser = copyThis.TranslationServiceUser.Copy();
            this.IsDirty = false;
        }
        #endregion constructors

        #region properties
        [XmlElement("TranslationServiceUri")]
        public string TranslationServiceUri
        {
            get
            {
                return mTranslationServiceUri;
            }

            set
            {
                if (mTranslationServiceUri != value)
                {
                    mTranslationServiceUri = value;
                }
            }
        }

        [XmlElement("TranslationServiceUser")]
        public SecureString TranslationServiceUser
        {
            get
            {
                return mTranslationServiceUser;
            }

            set
            {
                if (mTranslationServiceUser != value)
                {
                    mTranslationServiceUser = value;
                    IsDirty = true;
                }
            }
        }

        [XmlElement("TranslationServicePassword")]
        public SecureString TranslationServicePassword
        {
            get
            {
                return mTranslationServicePassword;
            }

            set
            {
                if (mTranslationServicePassword != value)
                {
                    mTranslationServicePassword = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Get/set whether the settings stored in this instance have been
        /// changed and need to be saved when program exits (at the latest).
        /// </summary>
        [XmlIgnore]
        public bool IsDirty
        {
            get
            {
                return mIsDirty;
            }

            set
            {
                if (mIsDirty != value)
                    mIsDirty = value;
            }
        }
        #endregion properties
    }
}
