namespace LocultApp
{
    using AppResourcesLib;
    using ExplorerLib;
    using MsgBox;
    using MSTranslate;
    using MSTranslate.Interfaces;
    using ResourceFileLib;
    using ServiceLocator;
    using ServiceLocatorInterfaces;
    using Settings;
    using Settings.Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// Creates and initializes all services.
    /// </summary>
    public static class ServiceInjector
    {
        /// <summary>
        /// Loads service objects into the ServiceContainer on startup.
        /// </summary>
        /// <returns>Returns the current <seealso cref="ServiceContainer"/> instance
        /// to let caller work with service container items right after creation.</returns>
        public static ServiceContainer InjectServices()
        {
            ServiceContainer.Instance.AddService<IMessageBoxService>(new MessageBoxService());
            ServiceContainer.Instance.AddService<IResourceLocator>(new ResourceLocator());
            ServiceContainer.Instance.AddService<IExplorer>(new Explorer());
            ServiceContainer.Instance.AddService<ISettingsManager>(new SettingsManager());

            ServiceContainer.Instance.AddService<ITranslator>(new Translator());

            // Create a list of services with the type of same interface and add services into this list
            var stringFileParsers = new Dictionary<string, IStringFileParser>();
            stringFileParsers.Add("RESX", new ResourceFileParser());
            var servicList = new StringFileParseServiceList( stringFileParsers );

            ServiceContainer.Instance.AddService<IStringFileParseServiceList>(servicList);

            return ServiceContainer.Instance;
        }
    }
}
