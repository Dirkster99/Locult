namespace AppResourcesLib
{
    using System;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Locate resources ín any assembly and return their reference.
    /// This class can, for example, be used to load a DataTemplate instance from an XAML reference.
    /// That is, the XAML is referenced as URI string (and the XAML itself can live in an extra assembly).
    /// The returned instance can be consumed in a 'code behind' context.
    /// </summary>
    public class ResourceLocator : IResourceLocator
    {
        /// <summary>
        /// Gets the first matching resource of the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="resourceFilename">The resource filename.</param>
        /// <returns></returns>
        public T GetResource<T>(string assemblyName, string resourceFilename) where T : class
        {
            return GetResource<T>(assemblyName, resourceFilename, string.Empty);
        }

        /// <summary>
        /// Gets the resource by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="resourceFilename">The resource filename.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public T GetResource<T>(string assemblyName,
                                string resourceFilename,
                                string name) where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(resourceFilename))
                    return default(T);

                string uriPath = string.Format("/{0};component/{1}", assemblyName, resourceFilename);
                Uri uri = new Uri(uriPath, UriKind.Relative);
                ResourceDictionary resource = Application.LoadComponent(uri) as ResourceDictionary;

                if (resource == null)
                    return default(T);

                if (!string.IsNullOrEmpty(name))
                {
                    if (resource.Contains(name))
                        return resource[name] as T;

                    return default(T);
                }

                return resource.Values.OfType<T>().FirstOrDefault();
            }
            catch(Exception exp)
            {
                throw new Exception(string.Format("Error Loading resource '{0}': {1}", "Exception:", exp.Message, exp));
            }
        }

        /// <summary>
        /// Create a new page model entry from given parameters
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="resourceFilename"></param>
        /// <param name="resourceKey"></param>
        /// <param name="viewmodelInstance"></param>
        /// <param name="sortPriority">priority for this model. This priority allows sorted display
        /// when listing contents of several models in order or imporance
        /// (lowest number is most important).</param>
        /// <returns></returns>
        public IPageTemplateModel GetNewPageTemplateModel
        (
            string assemblyName,
            string resourceFilename,
            string resourceKey,
            object viewmodelInstance,
            string modelKeyName,
            uint sortPriority
        )
        {
            return new PageTemplateModel
            (
                assemblyName,
                resourceFilename,
                resourceKey,
                viewmodelInstance,
                modelKeyName,
                sortPriority
            );
        }

    }
}
