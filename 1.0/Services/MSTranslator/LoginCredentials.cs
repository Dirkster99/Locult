namespace MSTranslate
{
    using System;
    using MSTranslate.Interfaces;
    using System.Security;

    /// <summary>
    /// Class supplies the login credentials for the Microsoft Translation Service.
    /// </summary>
    internal class LoginCredentials : ILoginCredentials
    {
        SecureString mUser = null, mPassword = null;

        /// <summary>
        /// Public class constructor
        /// </summary>
        /// <param name="translationServiceUri"></param>
        /// <param name="accountKey"></param>
        public LoginCredentials(Uri translationServiceUri,
                                SecureString user,
                                SecureString password)
            : this()
        {
            TranslationServiceUri = translationServiceUri;

            if (user != null)
                mUser = user.Copy();

            if (password != null)
                mPassword = password.Copy();
        }

        /// <summary>
        /// Non-public class constructor
        /// </summary>
        protected LoginCredentials()
        {
        }

        public Uri TranslationServiceUri { get; private set; }

        public SecureString User
        {
            get
            {
                if (mUser == null)
                    return null;

                return mUser.Copy();
            }
        }

        public SecureString Password
        {
            get
            {
                if (mPassword == null)
                    return null;

                return mPassword.Copy();
            }
        }
    }
}
