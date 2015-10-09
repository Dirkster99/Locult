namespace TranslationSolutionViewModelLib.ViewModels
{
    using Base;
    using System.Collections.ObjectModel;
    using TranslationSolutionViewModelLib.SolutionModelVisitors;
    using TranslatorSolutionLib.Models;

    /// <summary>
    /// Manages all solution relevant operations and holds the root of all solution items.
    /// </summary>
    public class SolutionViewModel : ViewModelBase
    {
        #region fields
        private readonly ObservableCollection<ISolutionItem> mChildren;
        private readonly SolutionRootViewModel mRoot;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public SolutionViewModel(TranslatorSolution solutionModel,
                                string solutionPath,
                                string solutionName)
        {
            mChildren = new ObservableCollection<ISolutionItem>();

            var modelExt = new ModelToViewModel();

            ////mRoot = new SolutionRootViewModel(solutionModel, solutionPath, solutionName);
            mRoot = modelExt.ToViewModel(solutionModel, solutionPath, solutionName);

            mChildren.Add(mRoot);
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the root item of the solution and its children.
        /// </summary>
        public ObservableCollection<ISolutionItem> Children
        {
            get
            {
                return mChildren;
            }
        }

        /// <summary>
        /// Gets the root item of the solution tree of items.
        /// </summary>
        public SolutionRootViewModel Root
        {
            get
            {
                return mRoot;
            }
        }
        #endregion properties
    }
}
