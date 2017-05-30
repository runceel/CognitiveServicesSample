using CognitiveServicesSample.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServiceSample.Jobs.Services
{
    public class AnalyzeService : IAnalyzeService
    {
        private AnalyzeSetting AnalyzeSetting { get; }
        private ITwitterService TwitterService { get; }
        private IVisionService VisionService { get; }
        private ICategolizedImageRepository CategolizedImageRepository { get; }

        public AnalyzeService(IOptions<AnalyzeSetting> analyzeSetting,
            ITwitterService twitterService,
            IVisionService visionService,
            ICategolizedImageRepository categolizedImageRepository)
        {
            this.AnalyzeSetting = analyzeSetting.Value;
            this.TwitterService = twitterService;
            this.VisionService = visionService;
            this.CategolizedImageRepository = categolizedImageRepository;
        }

        public async Task AnalyzeAsync()
        {
            async Task<IEnumerable<TwitterSearchResult>> FilterAsync(IEnumerable<TwitterSearchResult> tweets)
            {
                var r = new List<TwitterSearchResult>();
                foreach (var tweet in tweets)
                {
                    if (!await this.CategolizedImageRepository.IsExistTweet(tweet.Id))
                    {
                        r.Add(tweet);
                    }
                }
                return r;
            }

            var searchResults = await this.TwitterService.SearchAsync(this.AnalyzeSetting.Keyword);
            var categolizedImages = await this.VisionService.CategolizedImageAsync(await FilterAsync(searchResults));
            var tasks = categolizedImages.Select(x => this.CategolizedImageRepository.InsertAsync(x)).ToArray();
            await Task.WhenAll(tasks);
        }
    }
}
