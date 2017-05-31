using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using CognitiveServicesSample.Commons;
using Microsoft.Azure.Documents.Linq;

namespace CognitiveServicesSample.Data
{
    public class CategolizedImageRepository : ICategolizedImageRepository
    {
        private const string DatabaseId = "db";
        private const string CategolizedImageCollection = "CategolizedImageCollection";
        private const string CategoriesCollection = "CategoriesCollection";

        private CosmosDbSetting CosmosDbSetting { get; }
        private ILogger Logger { get; }

        public CategolizedImageRepository(IOptions<CosmosDbSetting> cosmosDbSetting, ILogger logger)
        {
            this.CosmosDbSetting = cosmosDbSetting.Value;
            this.Logger = logger;
        }

        public async Task InsertAsync(CategolizedImage data)
        {
            var client = await this.CreateClientAsync();
            await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategolizedImageCollection),
                data);
            this.Logger.Info($"{nameof(CategolizedImageRepository)}.{nameof(InsertAsync)}({data.TweetId})");
            var categoriesCount = await client.CreateDocumentQuery<Category>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoriesCollection),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(x => x.PartitionKey == Category.PartitionKeyValue && x.Name == data.Category)
                .CountAsync();
            this.Logger.Info($"categories query result count: {categoriesCount}");
            if (categoriesCount == 0)
            {
                this.Logger.Info($"{nameof(CategolizedImageRepository)}.{nameof(InsertAsync)}({data.TweetId}): Create category {data.Category}");
                await client.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoriesCollection),
                    new Category { Name = data.Category, JaName = data.JaCategory });
            }
        }

        public async Task<IEnumerable<CategolizedImage>> LoadAsync(int skip, int pageSize, string category)
        {
            var client = await this.CreateClientAsync();
            var query = client.CreateDocumentQuery<CategolizedImage>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategolizedImageCollection),
                new FeedOptions { MaxItemCount = -1 })
                .Where(x => x.Category == category)
                .OrderBy(x => x.TweetId)
                .Skip(skip)
                .Take(pageSize)
                .AsDocumentQuery();
            var r = new List<CategolizedImage>();
            while (query.HasMoreResults)
            {
                r.AddRange((await query.ExecuteNextAsync<CategolizedImage>()).ToList());
            }
            return r.AsEnumerable();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var client = await this.CreateClientAsync();
            var query = client.CreateDocumentQuery<Category>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategolizedImageCollection),
                new FeedOptions { MaxItemCount = -1 })
                .Where(x => x.PartitionKey == Category.PartitionKeyValue)
                .AsDocumentQuery();
            var r = new List<Category>();
            while (query.HasMoreResults)
            {
                r.AddRange((await query.ExecuteNextAsync<Category>()).ToList());
            }
            return r.AsEnumerable();
        }

        private async Task<DocumentClient> CreateClientAsync()
        {
            var client = new DocumentClient(new Uri(this.CosmosDbSetting.EndpointUri), this.CosmosDbSetting.PrimaryKey);
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });

            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseId),
                new DocumentCollection
                {
                    Id = CategolizedImageCollection,
                    PartitionKey =
                    {
                        Paths =
                        {
                            "/category",
                        },
                    },
                });
            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseId),
                new DocumentCollection
                {
                    Id = CategoriesCollection,
                    PartitionKey =
                    {
                        Paths =
                        {
                            "/partitionKey",
                        },
                    },
                });

            return client;
        }

        public async Task<bool> IsExistTweet(long id)
        {
            var client = await this.CreateClientAsync();
            return (await client.CreateDocumentQuery<CategolizedImage>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategolizedImageCollection),
                new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(x => x.TweetId == id)
                .CountAsync()) != 0;
        }

        public async Task<int> CountImageByCategoryAsync(string category)
        {
            var client = await this.CreateClientAsync();
            return await client.CreateDocumentQuery<CategolizedImage>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategolizedImageCollection))
                .Where(x => x.Category == category)
                .CountAsync();
        }
    }
}
