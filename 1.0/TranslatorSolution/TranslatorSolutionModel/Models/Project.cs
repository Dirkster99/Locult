namespace TranslatorSolutionLib.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using TranslatorSolutionLib.Models.Interfaces;

    /// <summary>
    /// Models a translation project within a translation solution.
    /// </summary>
    public class Project : IModelPart
    {
        #region fields
        public const string DefaultComment = "A translation project from English into other languages.";

        private Dictionary<string, FileReference> _TargetFiles = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Construct a project from a source file reference.
        /// </summary>
        /// <param name="sourceFile"></param>
        public Project(FileReference sourceFile)
            : this()
        {
            SetSourceFile(sourceFile);
        }
        /// <summary>
        /// Class constructor
        /// </summary>
        public Project()
        {
            _TargetFiles = new Dictionary<string, FileReference>();
            SourceFile = null;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copyThis"></param>
        public Project(Project copyThis)
            : this()
        {
            if (copyThis == null)
                return;

            SourceFile = new FileReference(copyThis.SourceFile);

            TargetFiles.Clear();
            foreach (var item in copyThis.TargetFiles)
            {
                _TargetFiles.Add(item.Path, new FileReference(item));
            }
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a reference to a source file. A source file is the source of translation
        /// for the list of translation files.
        /// </summary>
        public FileReference SourceFile { get; protected set; }

        /// <summary>
        /// Gets a list of target files that should be translated from the source file.
        /// </summary>
        public IList<FileReference> TargetFiles
        {
            get
            {
                return _TargetFiles.Values.ToList();
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Resets the current source file for this translation project.
        /// </summary>
        /// <param name="name"></param>
        public void SetSourceFile(FileReference sourceFile)
        {
            SourceFile = new FileReference(sourceFile);
        }

        /// <summary>
        /// Adds a new translation target file into the list of translation projects.
        /// </summary>
        /// <param name="newFile"></param>
        public void AddTargetFile(FileReference newFile)
        {
            _TargetFiles.Add(newFile.Path, newFile);
        }

        /// <summary>
        /// Updates the contents of a translation target file with the
        /// contents of the given translation target file.
        /// </summary>
        /// <param name="newFile"></param>
        /// <returns></returns>
        public bool UpdateTargetFile(FileReference newFile)
        {
            FileReference o;

            if (_TargetFiles.TryGetValue(newFile.Path, out o) == true)
            {
                o = new FileReference(newFile);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to retrieve a given translation target file by its
        /// source file path or null if translation target file was not present.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public FileReference GetTargetFile(string sourceFilePath)
        {
            FileReference o;

            _TargetFiles.TryGetValue(sourceFilePath, out o);

            return o;
        }

        /// <summary>
        /// Removes a translation target file from the list of translation target files.
        /// in this solution.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public bool RemoveTargetFile(string sourceFilePath)
        {
            return _TargetFiles.Remove(sourceFilePath);
        }

        /// <summary>
        /// Sets all paths relative to the given path if both paths are based on the same device.
        /// </summary>
        /// <param name="solutionPathName"></param>
        internal void SetRelativePath(string solutionPathName)
        {
            SourceFile.SetPath(TranslatorSolution.GetRelativePath(SourceFile.Path, solutionPathName));

            if (TargetFiles != null)
            {
                foreach (var item in TargetFiles)
                    item.SetPath(TranslatorSolution.GetRelativePath(item.Path, solutionPathName));
            }
        }

        /// <summary>
        /// Sets all paths absolute to a given path if both paths are based on the same device.
        /// </summary>
        /// <param name="solutionPathName"></param>
        internal void SetAbsolutePath(string solutionPathName)
        {
            // Combine and normalize source file path to make it human readable and rooted
            var solutionPath = System.IO.Path.GetDirectoryName(solutionPathName);
            var path = System.IO.Path.Combine(solutionPath, SourceFile.Path);

            SourceFile.SetPath(System.IO.Path.GetFullPath(path));

            if (TargetFiles != null)
            {
                foreach (var item in TargetFiles)
                {
                    // Combine and normalize paths to make them human readable and rooted
                    path = System.IO.Path.Combine(solutionPath, item.Path);
                    item.SetPath(System.IO.Path.GetFullPath(path));
                }
            }
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
