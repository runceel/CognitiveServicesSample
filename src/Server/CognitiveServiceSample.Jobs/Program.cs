using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using CognitiveServiceSample.Jobs.Services;
using System.Configuration;
using CognitiveServicesSample.Data;
using Microsoft.Extensions.Options;

namespace CognitiveServiceSample.Jobs
{
    class Program
    {
        static void Main()
        {
            // settings
            var translatorSetting = new TranslatorSetting
            {
                APIKey = ConfigurationManager.AppSettings["TranslatorAPIKey"],
            };
            var twitterSetting = new TwitterSetting
            {
                ConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"],
                AccessToken = ConfigurationManager.AppSettings["TwitterAccessToken"],
                AccessTokenSecret = ConfigurationManager.AppSettings["TwitterAccessTokenSecret"],
            };
            var visionSetting = new VisionSetting
            {
                APIKey = ConfigurationManager.AppSettings["VisionAPIAPIKey"],
                Endpoint = ConfigurationManager.AppSettings["VisionAPIEndpoint"],
            };
            var cosmosDbSetting = new CosmosDbSetting
            {
                EndpointUri = ConfigurationManager.AppSettings["CosmosDbEndpointUri"],
                PrimaryKey = ConfigurationManager.AppSettings["CosmosDbPrimaryKey"],
            };
            var analyzeSetting = new AnalyzeSetting
            {
                Keyword = ConfigurationManager.AppSettings["AnalyzeKeyword"],
            };

            // services
            var translatorService = new TranslatorService(Options.Create(translatorSetting));
            var twitterService = new TwitterService(Options.Create(twitterSetting));
            var visionService = new VisionService(Options.Create(visionSetting), translatorService);
            var categolizedImageRepository = new CategolizedImageRepository(Options.Create(cosmosDbSetting));
            var analyzeService = new AnalyzeService(Options.Create(analyzeSetting),
                twitterService,
                visionService,
                categolizedImageRepository);

            // run
            analyzeService.AnalyzeAsync().Wait();
        }
    }
}
