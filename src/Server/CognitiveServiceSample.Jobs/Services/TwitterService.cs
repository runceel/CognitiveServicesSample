using CognitiveServicesSample.Commons;
using CoreTweet;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServiceSample.Jobs.Services
{
    public class TwitterService : ITwitterService
    {
        private TwitterSetting TwitterSetting { get; }
        private ILogger Logger { get; }
        private Tokens Tokens { get; set; }

        public TwitterService(IOptions<TwitterSetting> twitterSetting, ILogger logger)
        {
            this.TwitterSetting = twitterSetting.Value;
            this.Logger = logger;
        }

        public async Task InitializeAsync()
        {
            this.Logger.Info($"{nameof(TwitterService)}.{nameof(InitializeAsync)}()");
            this.Tokens = Tokens.Create(
                this.TwitterSetting.ConsumerKey,
                this.TwitterSetting.ConsumerSecret,
                this.TwitterSetting.AccessToken,
                this.TwitterSetting.AccessTokenSecret);
            await this.Tokens.Account.VerifyCredentialsAsync();
        }

        public async Task<IEnumerable<TwitterSearchResult>> SearchAsync(string filter)
        {
            this.Logger.Info($"{nameof(TwitterService)}.{nameof(SearchAsync)}({filter})");
            var results = await this.Tokens.Search.TweetsAsync(
                q => $"filter:images {filter} -RT", count => 500);
            return results.Where(x => x.Entities.Media != null)
                .Select(x => new TwitterSearchResult
                {
                    Id = x.Id,
                    User = x.User.ScreenName,
                    Text = x.Text,
                    Timestamp = x.CreatedAt,
                    Images = x.Entities.Media.Select(y => y.MediaUrl).ToArray(),
                });
        }
    }
}
