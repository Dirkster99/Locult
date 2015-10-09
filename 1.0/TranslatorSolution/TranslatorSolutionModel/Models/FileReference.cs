namespace TranslatorSolutionLib.Models
{
    using TranslatorSolutionLib.Models.Interfaces;

    public class FileReference : IModelPart
    {
        #region constructors
        /// <summary>
        /// Class constructor from parameters
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="comment"></param>
        public FileReference(string path,
                             string type,
                             string comment = null)
        {
            Path = path;
            Type = type;
            Comment = comment;
        }

        /// <summary>
        /// Class construtor
        /// </summary>
        public FileReference()
        {
            Path = Type = Comment = null;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="copyThis"></param>
        public FileReference(FileReference copyThis)
            : this()
        {
            if (copyThis == null)
                return;

            Comment = copyThis.Comment;
            Path = copyThis.Path;
            Type = copyThis.Type;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the path to the referenced file.
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// Gets the type of file reference (eg.: 'RESX').
        /// </summary>
        public string Type { get; protected set; }

        /// <summary>
        /// Gets a comment that can be stored with this file reference.
        /// </summary>
        public string Comment { get; protected set; }
        #endregion properties

        #region methods
        public void SetPath(string stringValue)
        {
            Path = stringValue;
        }

        public void SetType(string stringValue)
        {
            Type = stringValue;
        }

        public void SetComment(string stringValue)
        {
            Comment = stringValue;
        }
        #endregion methods

        /// <summary>
        /// Implements a method that accepts a visitor according to the visitor pattern.
        /// </summary>
        /// <param name="modelVisitor"></param>
        /// <param name="objCursor">May be an object that represents the current
        /// traversal state or me be null. This parameter should (at best) be
        /// handled from visited element back to visitor without being further
        /// evaluated.</param>
        void IModelPart.Accept(IVisitor modelVisitor, object objCursor)
        {
            modelVisitor.Visit(this, objCursor);
        }
    }
}
