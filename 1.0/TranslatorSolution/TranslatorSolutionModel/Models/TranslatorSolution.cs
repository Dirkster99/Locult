namespace TranslatorSolutionLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TranslatorSolutionLib.Models.Interfaces;
    using TranslatorSolutionLib.XMLModels;

    public class TranslatorSolution : IModelPart
    {
        #region fields
        public const string _Version = "1.00.00";
        public const string _DefaultName = "New Locult Solution";
        public const string _DefaultSolutionExtension = ".locult";
        public const string _DefaultComment = "This is a translation solution. It can refer to several files that require translation into multiple languages. ";

        private Dictionary<string, Project> _Projects = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor from parameters
        /// </summary>
        public TranslatorSolution(string name = null,
                                  string comment = null)
            : this()
        {
            SetName(name);
            SetComment(name);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public TranslatorSolution()
        {
            InitSolution();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copyThis"></param>
        public TranslatorSolution(TranslatorSolution copyThis)
            : this()
        {
            if (copyThis == null)
                return;

            SetName(copyThis.Name);
            SetComment(copyThis.Comment);

            foreach (var item in copyThis.Projects)
            {
                AddProject(new Project(item));
            }
        }
        #endregion constructors

        #region properties
        public string Version
        {
            get
            {
                return TranslatorSolution._Version;
            }
        }

        /// <summary>
        /// Gets/sets a text based name for this object.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets/sets an informal text based comment for this object.
        /// </summary>
        public string Comment { get; protected set; }

        /// <summary>
        /// Gets a list of projects in this solution
        /// </summary>
        public IList<Project> Projects
        {
            get
            {
                return _Projects.Values.ToList();
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Converts a rooted path to a relative path if available.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static string GetRelativePath(string s1, string s2)
        {
            try
            {
                var uri = new Uri(s2);

                return uri.MakeRelativeUri(new Uri(s1)).ToString();
            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
                Console.WriteLine(exp.StackTrace);

                return s1;
            }
        }

        /// <summary>
        /// Resets the current name string for this solution.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name = null)
        {
            Name = (name == null ? _DefaultName : name);
        }

        /// <summary>
        /// Resets the current comment string for this solution.
        /// </summary>
        /// <param name="comment"></param>
        public void SetComment(string comment = null)
        {
            Comment = (comment == null ? _DefaultComment : comment);
        }

        /// <summary>
        /// Adds a new project into the list of translation projects.
        /// </summary>
        /// <param name="newProject"></param>
        public void AddProject(Project newProject)
        {
            _Projects.Add(newProject.SourceFile.Path, newProject);
        }

        /// <summary>
        /// Updates the contents of a project with the
        /// contents of the given project.
        /// </summary>
        /// <param name="newProject"></param>
        /// <returns></returns>
        public bool UpdateProject(Project newProject)
        {
            Project o;

            if (_Projects.TryGetValue(newProject.SourceFile.Path, out o) == true)
            {
                o = new Project(newProject);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to retireve a given project by its source file path
        /// or null if project was not present.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public Project GetProject(string sourceFilePath)
        {
            Project o;

            _Projects.TryGetValue(sourceFilePath, out o);

            return o;
        }

        /// <summary>
        /// Removes a project from the list of translation projects
        /// in this solution.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public bool RemoveProject(string sourceFilePath)
        {
            return _Projects.Remove(sourceFilePath);
        }

        /// <summary>
        /// Copies all solution items into a new object and computes a relative
        /// path to <paramref name="solutionPathName"/> if this is supplied.
        /// </summary>
        /// <param name="solutionPathName"></param>
        /// <returns>new solution object that is a copy of this object.</returns>
        public TranslatorSolution CopySolution(string solutionPathName = null)
        {
            var solution = new TranslatorSolution(this);

            // Make all paths relative to location of solution if supplied parameter is available
            if (solution.Projects.Count > 0 && string.IsNullOrEmpty(solutionPathName) == false)
            {
                foreach (var item in solution._Projects.Values)
                    item.SetRelativePath(solutionPathName);
            }

            return solution;
        }

        /// <summary>
        /// Initialize object with default values.
        /// </summary>
        private void InitSolution()
        {
            Name = Comment = null;

            if (_Projects == null)
                _Projects = new Dictionary<string, Project>();
            else
                _Projects.Clear();
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
