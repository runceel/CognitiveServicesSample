using CognitiveServicesSample.Commons;
using System;
using Xamarin.Forms;

namespace CognitiveServicesSample.Client.Views
{
    public partial class CategolizedImagePage : ContentPage
    {
        public CategolizedImagePage()
        {
            InitializeComponent();
        }

        private void ClosePreview(object sender, EventArgs e)
        {
            this.frameTappedCategolizedImageHost.IsVisible = false;
            this.imagePreview.Source = null;
            this.labelJaDescription.Text = "";
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var categolizedImage = (CategolizedImage)e.Item;
            this.imagePreview.Source = ImageSource.FromUri(new Uri(categolizedImage.Image));
            this.labelJaDescription.Text = categolizedImage.JaDescription;
            this.frameTappedCategolizedImageHost.IsVisible = true;
        }
    }
}
