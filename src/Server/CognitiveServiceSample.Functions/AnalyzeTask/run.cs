using CognitiveServiceSample.Jobs.Services;
using CognitiveServicesSample.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Options;
using System;
using System.Configuration;

namespace CognitiveServiceSample.Functions
{
    public class AnalyzeFunctions
    {
        public static void Run(TimerInfo myTimer, TraceWriter log)
        {
            try
            {
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
                var logger = new TraceWriterLogger(log);
                var translatorService = new TranslatorService(Options.Create(translatorSetting), logger);
                var twitterService = new TwitterService(Options.Create(twitterSetting), logger);
                var visionService = new VisionService(Options.Create(visionSetting), translatorService, logger);
                var categolizedImageRepository = new CategorizedImageRepository(Options.Create(cosmosDbSetting), logger);
                var analyzeService = new AnalyzeService(Options.Create(analyzeSetting),
                    twitterService,
                    visionService,
                    categolizedImageRepository, 
                    logger);

                // initialize
                twitterService.InitializeAsync().Wait();

                // run
                analyzeService.AnalyzeAsync().Wait();
            }
            catch (Exception ex)
            {
                log.Error($"Error {ex.ToString()}");
            }
        }
    }
}
