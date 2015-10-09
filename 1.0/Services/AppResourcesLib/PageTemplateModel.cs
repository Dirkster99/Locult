namespace AppResourcesLib
{
    using System;

    /// <summary>
    /// Models a resource item of a view. This can be any <seealso cref="DataTemplate"/>
    /// in any assembly. The application uses this model to find the resource, load it,
    /// instanciate it, and associate it with a viewmodel -> model (via DataTemplate).
    /// </summary>
    internal class PageTemplateModel : IPageTemplateModel
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="resourceFilename"></param>
        /// <param name="resourceKey"></param>
        /// <param name="viewmodelInstance"></param>
        /// <param name="modelKeyName"></param>
        public PageTemplateModel
        (
            string assemblyName,
            string resourceFilename,
            string resourceKey,
            object viewmodelInstance,
            string modelKeyName,
            uint sortPriority
        )
        {
            AssemblyName = assemblyName;
            ResourceFilename = resourceFilename;
            ResourceKey = resourceKey;
            ViewModelInstance = viewmodelInstance;
            ModelKeyName = modelKeyName;

            SortPriority = sortPriority;
        }

        /// <summary>
        /// Gets the name of the assembly in which this resource is located.
        /// </summary>
        public string AssemblyName     { get; private set; }

        /// <summary>
        /// Gets the file name of resource which this resource is located.
        /// </summary>
        public string ResourceFilename { get; private set; }

        /// <summary>
        /// Gets the key of the resource this item referes to.
        /// </summary>
        public string ResourceKey { get; private set; }

        /// <summary>
        /// Gets the type of viewmodel instance that is used to view-model it.
        /// </summary>
        public object ViewModelInstance { get; private set; }

        /// <summary>
        /// Gets a unique key that identifies this model.
        /// </summary>
        public string ModelKeyName { get; private set; }

        /// <summary>
        /// Gets a priority for this model. This priority allows sorted display
        /// when listing contents of several models in order or imporance
        /// (lowest number is most important).
        /// </summary>
        public uint SortPriority { get; private set; }
    }
}
