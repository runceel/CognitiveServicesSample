using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesSample.Data
{
    public class CategolizedImage
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("tweetId")]
        public long TweetId { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("jaCategory")]
        public string JaCategory { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("jaDescription")]
        public string JaDescription { get; set; }
        [JsonProperty("themeColor")]
        public string ThemeColor { get; set; }
    }
}
