namespace AppResourcesLib
{
    using System;

    /// <summary>
    /// Defines an interface for a model of an resource item of a view.
    /// 
    /// This can be any <seealso cref="DataTemplate"/>
    /// in any assembly. The application uses this model to find the resource, load it,
    /// instanciate it, and associate it with a viewmodel -> model (via DataTemplate).
    /// </summary>
    public interface IPageTemplateModel
    {
        /// <summary>
        /// Gets the name of the assembly in which this resource is located.
        /// </summary>
        string AssemblyName { get; }

        /// <summary>
        /// Gets the file name of resource which this resource is located.
        /// </summary>
        string ResourceFilename { get; }

        /// <summary>
        /// Gets the key of the resource this item referes to.
        /// </summary>
        string ResourceKey { get; }

        /// <summary>
        /// Gets the type of viewmodel instance that is used to view-model it.
        /// </summary>
        object ViewModelInstance { get; }

        /// <summary>
        /// Gets a unique key that identifies this model.
        /// </summary>
        string ModelKeyName { get; }

        /// <summary>
        /// Gets a priority for this model. This priority allows sorted display
        /// when listing contents of several models in order or imporance
        /// (lowest number is most important).
        /// </summary>
        uint SortPriority { get; }
    }
}
