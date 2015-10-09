namespace MSTranslate.Interfaces
{
    using System;
    using System.Security;

    /// <summary>
    /// Defines an interface to an object that holds the required login
    /// data for Microsoft's translation service.
    /// </summary>
    public interface ILoginCredentials
    {
        Uri TranslationServiceUri { get; }
        SecureString User { get; }
        SecureString Password { get; }
    }
}
