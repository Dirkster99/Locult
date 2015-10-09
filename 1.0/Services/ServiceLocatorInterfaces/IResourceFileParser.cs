namespace ServiceLocatorInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;


    /// <summary>
    /// Interface to read string content from a resx file.
    /// </summary>
    public interface IStringFileParser
    {
        /// <summary>
        /// Read Resource file entries and return them via yield statement.
        /// </summary>
        /// <param name="stringFile"></param>
        /// <returns></returns>
        IEnumerable<Tuple<string, string, string>> LoadFile(string stringFile,
                                                            CancellationTokenSource cts = null);

        /// <summary>
        /// Save Resource file entries and return number of entries saved.
        /// </summary>
        /// <param name="stringFile"></param>
        /// <returns></returns>
        int SaveFile(string pathFileName, IEnumerable<Tuple<string, string, string>> saveList,
                     CancellationTokenSource cts = null);
    }
}
