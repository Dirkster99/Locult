namespace ResourceFileLib
{
    using ServiceLocatorInterfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Resources;
    using System.Threading;

    public class ResourceFileParser : IStringFileParser
    {
        /// <summary>
        /// Read Resource file entries and return them via yield statement.
        /// </summary>
        /// <param name="stringFile"></param>
        /// <returns>IEnumerable<Tuple<string, string, string>>
        /// Item1 Key,
        /// Item2 value,
        /// Item3 comment
        /// </returns>
        public IEnumerable<Tuple<string, string, string>> LoadFile(string pathFileName,
                                                                   CancellationTokenSource cts = null)
        {
            using (ResXResourceReader reader = new ResXResourceReader(pathFileName))
            {
                bool useDataNodes = true;

                reader.UseResXDataNodes = useDataNodes;

                // Enumerate string resource entries using default enumerator IEnumerable.GetEnumerator().
                foreach (DictionaryEntry entry in reader)
                {
                    if (cts != null)
                        cts.Token.ThrowIfCancellationRequested();

                    // Use a null type resolver.
                    ITypeResolutionService typeres = null;
                    ResXDataNode dnode;

                    // Display from node info.
                    dnode = entry.Value as ResXDataNode;

                    // yield Key, value, comment strings as part of the collection
                    Tuple<string, string, string> resultEntry =
                             new Tuple<string, string, string>(dnode.Name,
                                                             dnode.GetValue(typeres) as string,
                                                             dnode.Comment);

                    yield return resultEntry;
                }
            }
        }

        /// <summary>
        /// Read Resource file entries and return them via yield statement.
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <param name="saveList">IEnumerable<Tuple<string, string, string>>
        /// Item1 Key,
        /// Item2 value,
        /// Item3 comment
        /// </param>
        /// <returns>number of saved entries</returns>
        public int SaveFile(string pathFileName, IEnumerable<Tuple<string, string, string>> saveList,
                            CancellationTokenSource cts = null)
        {
            int count = 0;
            ResXResourceWriter rw = null;
            string key = "", value = "", comment = "";

            try
            {
                rw = new ResXResourceWriter(pathFileName);
                try
                {
                    // Go through collection and save each key, value, comment entry in turn
                    foreach (var item in saveList)
                    {
                        if (cts != null)
                            cts.Token.ThrowIfCancellationRequested();

                        key = item.Item1;
                        value = item.Item2;
                        comment = item.Item3;

                        ResXDataNode node = new ResXDataNode(key, value) { Comment = comment };

                        rw.AddResource(node);
                        count++;
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("key: '{0}', value: '{1}', comment '{2}'",
                                                        key, value, comment), exp);
                }
                finally
                {
                    try
                    {
                        rw.Generate();
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (rw != null)
                    rw.Close();
            }

            return count;
        }
    }
}
