namespace ServiceLocator
{
    using ServiceLocatorInterfaces;
    using System.Collections.Generic;

    /// <summary>
    /// Simple container class to hold/enumerate all classes that implement the same interface.
    /// </summary>
    public class StringFileParseServiceList : IStringFileParseServiceList
    {
        #region fields
        private readonly Dictionary<string, IStringFileParser> _listStringFileParserService = null;
        private readonly object _lock = new object();
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="list"></param>
        public StringFileParseServiceList(Dictionary<string, IStringFileParser> list)
        {
            _listStringFileParserService = new Dictionary<string, IStringFileParser>();

            foreach (var item in list)
                _listStringFileParserService.Add(item.Key, item.Value);
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the complete dictionary of services stored in this container
        /// </summary>
        public Dictionary<string, IStringFileParser> Services
        {
            get
            {
                lock (_lock)
                {
                    return _listStringFileParserService;
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Select a service by its key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IStringFileParser Select(string key)
        {
            IStringFileParser o;

            lock (_lock)
            {
                _listStringFileParserService.TryGetValue(key, out o);
            }

            return o;
        }

        /// <summary>
        /// Enumerates all keys that are available with a service of this type.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetKeys()
        {
            lock (_lock)
            {
                return _listStringFileParserService.Keys;
            }
        }

        /// <summary>
        /// Enumerates all services that are available with a service of this type.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IStringFileParser> GetValues()
        {
            lock (_lock)
            {
                return _listStringFileParserService.Values;
            }
        }
        #endregion methods
    }
}
