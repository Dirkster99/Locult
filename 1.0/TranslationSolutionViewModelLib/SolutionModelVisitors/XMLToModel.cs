namespace TranslationSolutionViewModelLib.SolutionModelVisitors
{
    using TranslatorSolutionLib.Models;
    using TranslatorSolutionLib.Models.Interfaces;
    using TranslatorSolutionLib.XMLModels;

    /// <summary>
    /// Object implements the visitor pattern to navigate the model and translate
    /// a given XML string into a model representation.
    /// </summary>
    public class XMLToModel : IVisitor
    {
        /// <summary>
        /// Attempt to parse the given XML and set correxponding values in this object.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public bool SetXMLString(TranslatorSolutionLib.Models.TranslatorSolution sol,
                                 string xml,
                                 string solutionPathName = null)
        {
            if (string.IsNullOrEmpty(xml) == true)
                return false;

            string path = solutionPathName;

            try
            {
                path = System.IO.Path.GetDirectoryName(solutionPathName);
            }
            catch
            {
            }

            var param = new XMLParams(xml, path);

            (sol as IModelPart).Accept(this, param);

            return true;
        }

        #region Visitor Interface Implementation
        /// <summary>
        /// Visit a solotion model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(TranslatorSolutionLib.Models.TranslatorSolution modelSolution,
                            object cursorParams)
        {
            var par = cursorParams as XMLParams;

            var solution = XmlConverter<TranslatorSolutionLib.XMLModels.TranslatorSolution>.ToObject
                (par.XML, "TranslatorSolutionXML.Resources.TranslatorSolution.xsd");

            if (solution == null)
                return;

            // This should be analyzed for backward compatibility when there are more than 1 versions around...
            //// this.Version = solution.Version;

            modelSolution.SetComment(solution.Comment);
            modelSolution.SetName(solution.Name);

            // Combine and normalize source file path to make it human readable and rooted
            string solutionPath = null;

            if (par.SolutionPathName != null)
                solutionPath = System.IO.Path.GetDirectoryName(par.SolutionPathName);

            foreach (var item in solution.Project)
            {
                var project = new Project();

                par.CurrentItem = item;
                (project as IModelPart).Accept(this, par);

                // This does not work here since setting absolute or relative path does not reset the key item!
                ////if (string.IsNullOrEmpty(solutionPathName) == false)
                ////  project.SetAbsolutePath(solutionPathName);

                modelSolution.AddProject(project);
            }
        }

        /// <summary>
        /// Visit a project model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(Project modelProject,
                            object cursorParams)
        {
            var par = cursorParams as XMLParams;
            var projectTAG = par.CurrentItem as PROJECT_TAG;

            string sourceFilePath = projectTAG.SourceFile.Path;

            var fileRef = new FileReference();
            par.CurrentItem = projectTAG.SourceFile;
            (fileRef as IModelPart).Accept(this, par);

            modelProject.SetSourceFile(fileRef);

            if (projectTAG.TargetFiles != null)
            {
                foreach (var itemTargetFile in projectTAG.TargetFiles)
                {
                    sourceFilePath = itemTargetFile.Path;

                    fileRef = new FileReference();
                    par.CurrentItem = itemTargetFile;
                    (fileRef as IModelPart).Accept(this, par);

                    modelProject.AddTargetFile(fileRef);
                }
            }
        }

        /// <summary>
        /// Visit a file reference model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(FileReference modelSolution, object cursorParams)
        {
            var par = cursorParams as XMLParams;
            var fileReferennceTAG = par.CurrentItem as FILE_TAG;
            var sourceFilePath = fileReferennceTAG.Path;

            if (par.SolutionPathName != null)         // Comvert to absolute path
            {
                string path = System.IO.Path.Combine(par.SolutionPathName, sourceFilePath);
                sourceFilePath = System.IO.Path.GetFullPath(path);
            }

            modelSolution.SetPath(sourceFilePath);
            modelSolution.SetType(fileReferennceTAG.Type);
            modelSolution.SetComment(fileReferennceTAG.Comment);
        }
        #endregion Visitor Interface Implementation

        #region private classes
        /// <summary>
        /// Simple parameter class that helps navigating the model while maintaining state.
        /// </summary>
        private class XMLParams
        {
            #region constructors
            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="xml"></param>
            /// <param name="solutionPathName"></param>
            public XMLParams(string xml, string solutionPathName)
            {
                XML = xml;
                SolutionPathName = solutionPathName;
            }

            /// <summary>
            /// Hidden class constructor
            /// </summary>
            protected XMLParams()
            {
            }
            #endregion constructors

            /// <summary>
            /// Gets the XML string representation that is loading via visitor pattern.
            /// </summary>
            public string XML { get; private set; }

            /// <summary>
            /// Gets the path to the file being saved.
            /// Supplying this parameter enables computation of relative paths.
            /// </summary>
            public string SolutionPathName { get; private set; }

            /// <summary>
            /// Gets/sets the current (parent) object being navigated to help
            /// maintaining state while browsing the composite model structure.
            /// </summary>
            public object CurrentItem { get; set; }
        }
        #endregion private classes
    }
}
