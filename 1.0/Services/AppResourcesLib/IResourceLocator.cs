namespace AppResourcesLib
{
    using System;

    /// <summary>
    /// Locate resources ín any assembly and return their reference.
    /// This interface can, for example, be used to define methods that load a DataTemplate instance
    /// from an XAML reference.
    /// 
    /// That is, the XAML is referenced as URI string (and the XAML itself can live in an extra assembly).
    /// The returned instance can be consumed in a 'code behind' viewmodel context.
    /// </summary>
    public interface IResourceLocator
    {
        /// <summary>
        /// Gets the first matching resource of the requested type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="resourceFilename">The resource filename.</param>
        /// <returns></returns>
        T GetResource<T>(string assemblyName, string resourceFilename) where T : class;

        /// <summary>
        /// Gets the resource by its name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="resourceFilename">The resource filename.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        T GetResource<T>(string assemblyName, string resourceFilename, string name) where T : class;

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
        IPageTemplateModel GetNewPageTemplateModel
        (
            string assemblyName,
            string resourceFilename,
            string resourceKey,
            object viewmodelInstance,
            string modelKeyName,
            uint sortPriority
        );
    }
}
