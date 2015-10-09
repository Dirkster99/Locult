namespace LocultApp.ViewModels.Pages
{
    using AppResourcesLib;
    using LocultApp.ViewModels.Pages.EditPageDocuments.Base.Events;
    using LocultApp.ViewModels.Pages.SettingsPages;
    using LocultApp.Views.Pages.SettingsPages;
    using SettingsModel.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    public class SettingsPageViewModel : PageBaseViewModel, IDisposable
    {
        #region fields
        private bool mDisposed = false;
        private bool _IsDirty = false;

        private SortedList<uint, IPageTemplateModel> _PageTemplateModels = null;
        private ObservableCollection<SettingsPageBaseViewModel> _Pages = null;
        private SettingsPageBaseViewModel _SelectedPage = null;
        private SettingsTemplateSelector _PageTemplateSelector;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="settingsManager"></param>
        public SettingsPageViewModel(List<IPageTemplateModel> optionalPages = null)
        {
            var reslocService = GetService<IResourceLocator>();
            _PageTemplateModels = new SortedList<uint, IPageTemplateModel>();

            _PageTemplateModels.Add(0, reslocService.GetNewPageTemplateModel(
                                   Assembly.GetAssembly(typeof(GeneralSettingsView)).GetName().Name,
                                   "Views/Pages/SettingsPages/DataTemplates.xaml",
                                    "GeneralSettingsTemplate",
                                    new GeneralSettingsViewModel("General"),
                                    "Options", 0));

            _PageTemplateModels.Add(9999999, reslocService.GetNewPageTemplateModel(
                       Assembly.GetAssembly(typeof(AboutViewModel)).GetName().Name,
                       "Views/Pages/SettingsPages/DataTemplates.xaml",
                        "AboutSettingsTemplate",
                        new AboutViewModel(),
                        "About", 9999999));

            // Add optional page models in addition to those already implemented above.
            if (optionalPages != null)
            {
                // Icon size can be setup on Appearance page if application contains that setting page
                // So, we hide it from General page if Appearance page should be available
                if (optionalPages.Count > 0)
                    SetGeneralIconSizeVisibility(false);

                foreach (var item in optionalPages)
                    _PageTemplateModels.Add(item.SortPriority, item);
            }

            _Pages = new ObservableCollection<SettingsPageBaseViewModel>();
            _PageTemplateSelector = new SettingsTemplateSelector();
            foreach (var item in _PageTemplateModels)
            {
                var template = reslocService.GetResource<DataTemplate>(
                                                                item.Value.AssemblyName,
                                                                item.Value.ResourceFilename,
                                                                item.Value.ResourceKey) as DataTemplate;

                // Register a DataTemplate for each settings view page to map between view and model
                _PageTemplateSelector.RegisterDataTemplate(item.Value.ViewModelInstance.GetType(), template);

                var vm = item.Value.ViewModelInstance as SettingsPageBaseViewModel;

                if (vm != null)
                {
                    vm.IsDirty = false;
                    _Pages.Add(vm);

                    if (_SelectedPage == null)
                        _SelectedPage = (vm);

                    // Is current document a EditTranslationsDocumentViewModel?
                    // Then try to load default data for it...
                    vm.DirtyFlagChangedEvent += SettingsPage_DirtyFlagChangedEvent;
                }
            }

            _IsDirty = false;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the page template selector object that translates a selected viewmodel
        /// into an associated view.
        /// </summary>
        public SettingsTemplateSelector PageTemplateSelector
        {
            get
            {
                return _PageTemplateSelector;
            }
        }

        /// <summary>
        /// Gets the viewmodels of all setting pages displayed 'above' this settings view model.
        /// </summary>
        public ObservableCollection<SettingsPageBaseViewModel> Pages
        {
            get
            {
                return _Pages;
            }
        }

        /// <summary>
        /// Gets/sets the viewmodel for the currenlty selected settings page.
        /// </summary>
        public SettingsPageBaseViewModel SelectedPage
        {
            get
            {
                return _SelectedPage;
            }

            set
            {
                if (_SelectedPage != value)
                {
                    _SelectedPage = value;
                    RaisePropertyChanged(() => SelectedPage);
                }
            }
        }

        /// <summary>
        /// Gets/set the dirty state of a document.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }

            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;
                    base.RaisePropertyChanged(() => IsDirty);
                }
            }
        }

        public string PageDisplayName
        {
            get
            {
                return Local.Strings.STR_PROGRAM_SETTINGS_PAGE_TITLE;
            }
        }

        public string PageDisplayName_Tip
        {
            get
            {
                return LocultApp.Local.Strings.STR_PROGRAM_SETTINGS_PAGE_TITLE_TT;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Reset all view model pages to those options that are going to be presented for editing.
        /// </summary>
        /// <param name="optionsModel"></param>
        public void LoadOptionsFromModel(IEngine optionsModel)
        {
            foreach (var item in optionsModel.GetOptionGroups())
            {
                var pageTemplate = _PageTemplateModels.SingleOrDefault(pageItem => item.Name == pageItem.Value.ModelKeyName);

                if (pageTemplate.Value != null)
                    this.LoadOptionsFromModel(optionsModel, pageTemplate.Value.ViewModelInstance.GetType());
                else
                    throw new NotImplementedException(item.Name);
            }
        }

        /// <summary>
		/// Reset the view model to those options that are going to be presented for editing.
		/// </summary>
		/// <param name="settingData"></param>
        public void LoadOptionsFromModel(IEngine optionsModel, Type viewmodelType)
        {
            DocumentDirtyChangedViewModelBase settingsViewModel = _Pages.SingleOrDefault(item => item.GetType() == viewmodelType);

            if (settingsViewModel != null)
	        {
                var vm = settingsViewModel as SettingsPageBaseViewModel;

                vm.DirtyFlagChangedEvent -= SettingsPage_DirtyFlagChangedEvent;

                vm.ApplyOptionsFromModel(optionsModel);
                vm.IsDirty = false;
                vm.DirtyFlagChangedEvent += SettingsPage_DirtyFlagChangedEvent;
	        }
        }

        /// <summary>
        /// Reset all view model pages to those options that are going to be presented for editing.
        /// </summary>
        /// <param name="optionsModel"></param>
        public void SaveOptionsToModel(IEngine optionsModel)
        {
            foreach (var item in _Pages)
            {
                item.SaveOptionsToModel(optionsModel);
            }
        }

        /// <summary>
        /// Determines whether the IconSize editid control is visible in the General Settings page.
        /// </summary>
        public void SetGeneralIconSizeVisibility(bool IsVisibleInGeneralSettingsPage)
        {
            var vm = _PageTemplateModels.SingleOrDefault(item => item.Value.ModelKeyName == "Options");
            var viewModel = vm.Value.ViewModelInstance as GeneralSettingsViewModel;
            viewModel.IsIconSizeEditVisible = IsVisibleInGeneralSettingsPage;
        }

        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed == false)
            {
                if (disposing == true)
                {
                    for (int i = 0; i < _Pages.Count; i++)
                    {
                        var vm = _Pages[i] as DocumentDirtyChangedViewModelBase;

                        if (vm != null)
                            vm.DirtyFlagChangedEvent -= SettingsPage_DirtyFlagChangedEvent;

                        if (vm is IDisposable)
                            (vm as IDisposable).Dispose();

                        _Pages[i] = null;
                    }
                }
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }

        private void SettingsPage_DirtyFlagChangedEvent(object sender, EditPageDocuments.Base.Events.DocumentDirtyChangedEventArgs e)
        {
            if (e.IsDirtyNewValue == true)
                IsDirty = e.IsDirtyNewValue;
        }
        #endregion methods
    }
}
