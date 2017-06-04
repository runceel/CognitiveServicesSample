using CognitiveServicesSample.Client.Services;
using CognitiveServicesSample.Commons;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveServicesSample.Client.ViewModels
{
    public class CategolizedImagePageViewModel : BindableBase, INavigationAware
    {
        private ICategoryService CategoryService { get; }
        private IPageDialogService PageDialogService { get; }
        private string Category { get; set; }
        private string Continuation { get; set; }
        public ObservableCollection<CategolizedImage> CategolizedImages { get; } = new ObservableCollection<CategolizedImage>();

        private bool IsFirstLoadingRequest { get; set; } = true;

        private bool isBusy;

        public bool IsBusy
        {
            get { return this.isBusy; }
            set { this.SetProperty(ref this.isBusy, value); }
        }

        private string jaCategory;

        public string JaCategory
        {
            get { return this.jaCategory; }
            set { this.SetProperty(ref this.jaCategory, value); }
        }

        public DelegateCommand LoadCategolizedImagesCommand { get; }
        

        public CategolizedImagePageViewModel(ICategoryService categoryService,
            IPageDialogService pageDialogService)
        {
            this.CategoryService = categoryService;
            this.PageDialogService = pageDialogService;

            this.LoadCategolizedImagesCommand = new DelegateCommand(async () => await this.LoadCategolizedImagesExecuteAsync());
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("category") && parameters.ContainsKey("jaCategory"))
            {
                this.Category = (string)parameters["category"];
                this.JaCategory = (string)parameters["jaCategory"];
                this.Continuation = null;
                this.CategolizedImages.Clear();
                this.LoadCategolizedImagesCommand.Execute();
            }
        }

        private async Task LoadCategolizedImagesExecuteAsync()
        {
            this.IsBusy = true;
            try
            {
                if (this.IsLoadMore())
                {
                    var res = await this.CategoryService.LoadCategolizedImagesAsync(this.Category, this.Continuation);
                    this.Continuation = res.Continuation;
                    foreach (var r in res.CategolizedImages)
                    {
                        this.CategolizedImages.Add(r);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await this.PageDialogService.DisplayAlertAsync("Error", "Network error", "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private bool IsLoadMore()
        {
            if (this.IsFirstLoadingRequest)
            {
                this.IsFirstLoadingRequest = false;
                return true;
            }

            return !string.IsNullOrWhiteSpace(this.Continuation);
        }
    }
}
