namespace TranslationSolutionViewModelLib.ViewModels.Base
{
    /// <summary>
    /// Provides a base implementation for items that are displayed in the solution view.
    /// </summary>
    public abstract class SolutionItemBaseViewModel : ViewModelBase
    {
        #region fields
        private bool mIsSelected = false;
        private bool mIsExpanded = false;
        #endregion fields

        #region properties
        /// <summary>
        /// Get/set whether this folder is currently selected or not.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return mIsSelected;
            }

            set
            {
                if (mIsSelected != value)
                {
                    mIsSelected = value;
                    RaisePropertyChanged(() => IsSelected);
                }
            }
        }

        /// <summary>
        /// Gets an enumeration based identifier that tells the type of object in an enumeratable way.
        /// </summary>
        public abstract TypeOfSolutionItem TypeOfItem
        {
            get;
        }

        /// <summary>
        /// Get/set whether this folder is currently expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return mIsExpanded;
            }

            set
            {
                if (mIsExpanded != value)
                {
                    mIsExpanded = value;
                    RaisePropertyChanged(() => IsExpanded);
                }
            }
        }
        #endregion properties
    }
}
