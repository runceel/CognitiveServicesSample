using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesSample.Data
{
    public class Category
    {
        public const string PartitionKeyValue = "category";

        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string JaName { get; set; }
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; } = PartitionKeyValue;
    }
}
