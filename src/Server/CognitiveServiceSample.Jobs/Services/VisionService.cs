using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesSample.Data;
using Microsoft.ProjectOxford.Vision;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using CognitiveServicesSample.Commons;

namespace CognitiveServiceSample.Jobs.Services
{
    public class VisionService : IVisionService
    {
        private IVisionServiceClient VisionServiceClient { get; }
        private ITranslatorService TranslatorService { get; }
        private ILogger Logger { get; }

        public VisionService(IOptions<VisionSetting> visionSetting, ITranslatorService translatorService, ILogger logger)
        {
            this.VisionServiceClient = new VisionServiceClient(
                visionSetting.Value.APIKey,
                visionSetting.Value.Endpoint);
            this.TranslatorService = translatorService;
            this.Logger = logger;
        }

        public async Task<IEnumerable<CategolizedImage>> CategolizedImageAsync(IEnumerable<TwitterSearchResult> tweets)
        {
            this.Logger.Info($"{nameof(VisionService)}.{nameof(CategolizedImageAsync)}(tweets.count = {tweets.Count()})");
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
                    if (r.Categories == null)
                    {
                        continue;
                    }

                    var jpCaption = await this.TranslatorService.TranslateToJapaneseAsync(r.Description?.Captions.FirstOrDefault()?.Text ?? "");
                    var tasks = r.Categories.Select(async x => new CategolizedImage
                    {
                        Image = image.image,
                        Category = x.Name,
                        JaCategory = await this.TranslatorService.TranslateToJapaneseAsync(x.Name.Replace("_", " ").Trim()),
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
                    this.Logger.Error($"Error: {nameof(VisionService)}.{nameof(CategolizedImageAsync)}(tweets.count = {tweets.Count()}): {ex}");
                }
                // 10 call / 1sec
                await Task.Delay(110);
            }
            return results;
        }
    }
}
