namespace TranslationSolutionViewModelLib.SolutionModelVisitors
{
    using TranslatorSolutionLib.Models;
    using TranslatorSolutionLib.Models.Interfaces;
    using TranslatorSolutionLib.XMLModels;

    /// <summary>
    /// Object implements the visitor pattern to navigate the model and translate
    /// the current model into a resulting XML string representation.
    /// </summary>
    public class ModelToXML : IVisitor
    {
        /// <summary>
        /// Translates the current model <paramref name="modelSolution"/> into an
        /// XML compliant string representation.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <returns></returns>
        public string ToString(IModelPart modelSolution)
        {
            var solution = new TranslatorSolutionLib.XMLModels.TranslatorSolution();

            modelSolution.Accept(this, solution);

            return XmlConverter<TranslatorSolutionLib.XMLModels.TranslatorSolution>.ToXML(solution);
        }

        #region Visitor Interface Implementation
        /// <summary>
        /// Visit a solotion model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(TranslatorSolutionLib.Models.TranslatorSolution modelSolution, object cursorParams)
        {
            var sol = cursorParams as TranslatorSolutionLib.XMLModels.TranslatorSolution;

            sol.Version = modelSolution.Version;
            sol.Comment = modelSolution.Comment;
            sol.Name = modelSolution.Name;
            sol.Project = new PROJECT_TAG[modelSolution.Projects.Count];

            for (int i = 0; i < modelSolution.Projects.Count; i++)
            {
                sol.Project[i] = new PROJECT_TAG();
                (modelSolution.Projects[i] as IModelPart).Accept(this, sol.Project[i]);
            }
        }

        /// <summary>
        /// Visit a project model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(Project modelProject, object cursorParams)
        {
            var proj = cursorParams as TranslatorSolutionLib.XMLModels.PROJECT_TAG;

            proj.SourceFile = new FILE_TAG();
            (modelProject.SourceFile as IModelPart).Accept(this, proj.SourceFile);

            proj.TargetFiles = new FILE_TAG[modelProject.TargetFiles.Count];

            for (int j = 0; j < modelProject.TargetFiles.Count; j++)
            {
                proj.TargetFiles[j] = new FILE_TAG();
                (modelProject.TargetFiles[j] as IModelPart).Accept(this, proj.TargetFiles[j]);
            }
        }

        /// <summary>
        /// Visit a file reference model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(FileReference modelFileRef, object cursorParams)
        {
            var fr = cursorParams as TranslatorSolutionLib.XMLModels.FILE_TAG;

            fr.Comment = modelFileRef.Comment;
            fr.Path = modelFileRef.Path;
            fr.Type = modelFileRef.Type;
        }
        #endregion Visitor Interface Implementation
    }
}
