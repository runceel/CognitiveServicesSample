using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesSample.Data;
using Microsoft.ProjectOxford.Vision;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace CognitiveServiceSample.Jobs.Services
{
    public class VisionService : IVisionService
    {
        private IVisionServiceClient VisionServiceClient { get; }
        private ITranslatorService TranslatorService { get; }

        public VisionService(IOptions<VisionSetting> visionSetting, ITranslatorService translatorService)
        {
            this.VisionServiceClient = new VisionServiceClient(
                visionSetting.Value.APIKey,
                visionSetting.Value.Endpoint);
            this.TranslatorService = translatorService;
        }

        public async Task<IEnumerable<CategolizedImage>> CategolizedImageAsync(IEnumerable<TwitterSearchResult> tweets)
        {
            var images = tweets.SelectMany(x => x.Images.Select(y => (image: y, tweet: x)));
            var results = new List<CategolizedImage>();
            foreach (var image in images)
            {
                try
                {
                    var r = await this.VisionServiceClient.AnalyzeImageAsync(image.image, visualFeatures: new[]
                    {
                        VisualFeature.Color,
                        VisualFeature.Description,
                        VisualFeature.Categories,
                    });
                    var jpCaption = await this.TranslatorService.TranslateToJapaneseAsync(r.Description?.Captions.FirstOrDefault()?.Text ?? "");
                    var tasks = r.Categories.Select(async x => new CategolizedImage
                    {
                        Image = image.image,
                        Category = x.Name,
                        JaCategory = await this.TranslatorService.TranslateToJapaneseAsync(x.Name),
                        TweetId = image.tweet.Id,
                        Text = image.tweet.Text,
                        Description = r.Description?.Captions.FirstOrDefault()?.Text,
                        JaDescription = jpCaption,
                        ThemeColor = $"#{r.Color.AccentColor}",
                    }).ToArray();
                    await Task.WhenAll(tasks);
                    results.AddRange(tasks.Select(x => x.Result));
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine(ex);
                }
                // 10 call / 1sec
                await Task.Delay(110);
            }
            return results;
        }
    }
}
