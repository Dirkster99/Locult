namespace TranslationSolutionViewModelLib.SolutionModelVisitors
{
    using System;
    using TranslatorSolutionLib.Models.Interfaces;
    using TranslationSolutionViewModelLib.ViewModels;

    /// <summary>
    /// Object implements the visitor pattern to navigate the model and translate
    /// the current model into a resulting viewmodel representation.
    /// </summary>
    public class ModelToViewModel : IVisitor
    {
        /// <summary>
        /// Construct and return a SolutionRootViewModel from a solution model object.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="solutionPath"></param>
        /// <param name="solutionName"></param>
        /// <returns></returns>
        public SolutionRootViewModel ToViewModel(IModelPart modelSolution,
                                                 string solutionPath,
                                                 string solutionName)
        {
            try
            {
                var vm = new SolutionRootViewModel();

                var param = new VMParams(solutionPath, solutionName);
                param.CurrentItem = vm;

                modelSolution.Accept(this, param);

                return vm;
            }
            catch
            {
            }

            return null;
        }

        #region Visitor Interface Implementation
        /// <summary>
        /// Visit a solotion model object with the given cursor in <paramref name="cursorParams"/>.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(TranslatorSolutionLib.Models.TranslatorSolution modelSolution, object vmParam)
        {
            var param = vmParam as VMParams;
            var vm = param.CurrentItem as SolutionRootViewModel;

            foreach (var item in modelSolution.Projects)
            {
                var vmProject = new ProjectViewModel();
                vm.AddProject(item);
                
                //// (vmProject as IModelPart).Accept(this, item);
            }

            vm.Path = param.SolutionPath;
            vm.ItemName = param.SolutionName;

            vm.SolutionIsDirty_Reset();
        }

        /// <summary>
        /// Visit a project model object with the given cursor in <paramref name="cursorParams"/>.
        /// Methods not implemented.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(TranslatorSolutionLib.Models.Project modelProject, object vmParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a file reference model object with the given cursor in <paramref name="cursorParams"/>.
        /// Methods not implemented.
        /// </summary>
        /// <param name="modelSolution"></param>
        /// <param name="cursorParams"></param>
        void IVisitor.Visit(TranslatorSolutionLib.Models.FileReference modelSolution, object vmParam)
        {
            throw new NotImplementedException();
        }
        #endregion Visitor Interface Implementation

        #region private classes
        /// <summary>
        /// Simple parameter class that helps navigating the model while maintaining state.
        /// </summary>
        private class VMParams
        {
            #region constructors
            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="solutionPath"></param>
            /// <param name="solutionName"></param>
            public VMParams(string solutionPath,
                             string solutionName)
            {
                SolutionPath = solutionPath;
                SolutionName = solutionName;
            }

            /// <summary>
            /// Hidden class constructor
            /// </summary>
            protected VMParams()
            {
            }
            #endregion constructors

            /// <summary>
            /// Gets the path of the solution represented by the model.
            /// </summary>
            public string SolutionPath { get; private set; }

            /// <summary>
            /// Gets the name of the solution file represented by the model.
            /// </summary>
            public string SolutionName { get; private set; }

            /// <summary>
            /// Gets/sets the current (parent) object being navigated to help
            /// maintaining state while browsing the composite model structure.
            /// </summary>
            public object CurrentItem { get; set; }
        }
        #endregion private classes
    }
}
