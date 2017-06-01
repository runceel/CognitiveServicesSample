using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesSample.Data
{
    public class CategolizedImage
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty(PropertyName = "tweetId")]
        public long TweetId { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }
        [JsonProperty(PropertyName = "jaCategory")]
        public string JaCategory { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "jaDescription")]
        public string JaDescription { get; set; }
        [JsonProperty(PropertyName = "themeColor")]
        public string ThemeColor { get; set; }
    }
}
