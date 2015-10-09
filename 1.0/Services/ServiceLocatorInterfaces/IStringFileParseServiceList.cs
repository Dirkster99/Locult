namespace ServiceLocatorInterfaces
{
    using System.Collections.Generic;

    public interface IStringFileParseServiceList
    {
        /// <summary>
        /// Gets the complete dictionary of services stored in this container
        /// </summary>
        System.Collections.Generic.Dictionary<string, IStringFileParser> Services { get; }

        /// <summary>
        /// Select a service by its key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IStringFileParser Select(string key);

        /// <summary>
        /// Enumerates all keys that are available with a service of this type.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetKeys();

        /// <summary>
        /// Enumerates all services that are available with a service of this type.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IStringFileParser> GetValues();
    }
}
