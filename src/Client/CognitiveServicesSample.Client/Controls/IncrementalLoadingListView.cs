using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CognitiveServicesSample.Client.Controls
{
    public class IncrementalLoadingListView : ListView
    {
        public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create(
            nameof(LoadMoreCommand),
            typeof(ICommand),
            typeof(IncrementalLoadingListView));

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)this.GetValue(LoadMoreCommandProperty); }
            set { this.SetValue(LoadMoreCommandProperty, value); }
        }

        public IncrementalLoadingListView()
        {
            this.ItemAppearing += this.IncrementalLoadingListView_ItemAppearing;
        }

        private void IncrementalLoadingListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var lastItem = this.ItemsSource?.OfType<object>().LastOrDefault();
            if (lastItem == null)
            {
                return;
            }

            if (lastItem == e.Item && (this.LoadMoreCommand?.CanExecute(e.Item) ?? false))
            {
                this.LoadMoreCommand.Execute(e.Item);
            }
        }
    }
}
