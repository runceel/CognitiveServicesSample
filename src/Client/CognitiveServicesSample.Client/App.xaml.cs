using Autofac;
using Prism.Autofac;
using Prism.Autofac.Forms;
using CognitiveServicesSample.Client.Views;
using Xamarin.Forms;
using System.Net.Http;
using CognitiveServicesSample.Client.Services;

namespace CognitiveServicesSample.Client
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override async void OnInitialized()
        {
            this.InitializeComponent();

            await this.NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new HttpClient());
            builder.RegisterType<CategoryService>().As<ICategoryService>().SingleInstance();

            builder.Update(this.Container);

            this.Container.RegisterTypeForNavigation<NavigationPage>();
            this.Container.RegisterTypeForNavigation<MainPage>();
        }
    }
}
