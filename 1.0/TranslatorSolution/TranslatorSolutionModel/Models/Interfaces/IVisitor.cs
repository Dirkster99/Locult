namespace TranslatorSolutionLib.Models.Interfaces
{
    /// <summary>
    /// Implements a visitor pattern enabled interface that supports visiting and
    /// manipulating each item in the model of the solution.
    /// </summary>
    public interface IVisitor
    {
        /// <summary>
        /// Visit a solotion model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void Visit(TranslatorSolutionLib.Models.TranslatorSolution modelSolution, object solution);

        /// <summary>
        /// Visit a project model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void Visit(TranslatorSolutionLib.Models.Project modelProject, object solution);

        /// <summary>
        /// Visit a file reference model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void Visit(TranslatorSolutionLib.Models.FileReference modelSolution, object solution);
    }
}
