using CognitiveServicesSample.Client.Services;
using CognitiveServicesSample.Commons;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveServicesSample.Client.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private ICategoryService CategoryService { get; }
        private IPageDialogService PageDialogService { get; }

        private bool isBusy;

        public bool IsBusy
        {
            get { return this.isBusy; }
            set { this.SetProperty(ref this.isBusy, value); }
        }

        private IEnumerable<Category> categories;

        public IEnumerable<Category> Categories
        {
            get { return this.categories; }
            set { this.SetProperty(ref this.categories, value); }
        }

        public DelegateCommand LoadCategoriesCommand { get; }

        public MainPageViewModel(ICategoryService categoryService,
            IPageDialogService pageDialogService)
        {
            this.CategoryService = categoryService;
            this.PageDialogService = pageDialogService;

            this.LoadCategoriesCommand = new DelegateCommand(async () => await this.LoadCategoriesExecuteAsync());
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (this.Categories == null)
            {
                this.LoadCategoriesCommand.Execute();
            }
        }

        private async Task LoadCategoriesExecuteAsync()
        {
            this.IsBusy = true;
            try
            {
                this.Categories = await this.CategoryService.GetCategoriesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await this.PageDialogService.DisplayAlertAsync("Error",
                    "Network error.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

    }
}
