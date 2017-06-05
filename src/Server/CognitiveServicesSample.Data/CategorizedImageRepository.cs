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
    public class CategorizedImageRepository : ICategorizedImageRepository
    {
        private const string DatabaseId = "db";
        private const string CategorizedImageCollection = "CategorizedImageCollection";
        private const string CategoriesCollection = "CategoriesCollection";

        private CosmosDbSetting CosmosDbSetting { get; }
        private ILogger Logger { get; }

        public CategorizedImageRepository(IOptions<CosmosDbSetting> cosmosDbSetting, ILogger logger)
        {
            this.CosmosDbSetting = cosmosDbSetting.Value;
            this.Logger = logger;
        }

        public async Task InsertAsync(CategorizedImage data)
        {
            var client = await this.CreateClientAsync();
            await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategorizedImageCollection),
                data);
            this.Logger.Info($"{nameof(CategorizedImageRepository)}.{nameof(InsertAsync)}({data.TweetId})");
            var categoriesCount = await client.CreateDocumentQuery<Category>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoriesCollection),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(x => x.Name == data.Category)
                .CountAsync();
            this.Logger.Info($"categories query result count: {categoriesCount}");
            if (categoriesCount == 0)
            {
                this.Logger.Info($"{nameof(CategorizedImageRepository)}.{nameof(InsertAsync)}({data.TweetId}): Create category {data.Category}");
                await client.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoriesCollection),
                    new Category { Name = data.Category, JaName = data.JaCategory });
            }
        }

        public async Task<CategorizedImageLoadResponse> LoadAsync(string category, string continuation)
        {
            var client = await this.CreateClientAsync();
            var query = client.CreateDocumentQuery<CategorizedImage>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategorizedImageCollection),
                new FeedOptions { MaxItemCount = 50, RequestContinuation = continuation })
                .Where(x => x.Category == category)
                .OrderBy(x => x.TweetId)
                .AsDocumentQuery();
            var r = await query.ExecuteNextAsync<CategorizedImage>();
            return new CategorizedImageLoadResponse
            {
                Image = r.ToList(),
                Continues = r.ResponseContinuation,
            };
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var client = await this.CreateClientAsync();
            var query = client.CreateDocumentQuery<Category>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategoriesCollection),
                new FeedOptions { MaxItemCount = -1 })
                .Where(x => x.PartitionKey == Category.PartitionKeyValue)
                .AsDocumentQuery();
            var r = new List<Category>();
            do
            {
                r.AddRange((await query.ExecuteNextAsync<Category>()).ToList());
            }
            while (query.HasMoreResults);
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
                    Id = CategorizedImageCollection,
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
            return (await client.CreateDocumentQuery<CategorizedImage>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategorizedImageCollection),
                new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(x => x.TweetId == id)
                .CountAsync()) != 0;
        }

        public async Task<int> CountImageByCategoryAsync(string category)
        {
            var client = await this.CreateClientAsync();
            return await client.CreateDocumentQuery<CategorizedImage>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CategorizedImageCollection))
                .Where(x => x.Category == category)
                .CountAsync();
        }
    }
}
