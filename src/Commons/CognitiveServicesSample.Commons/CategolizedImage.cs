using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesSample.Commons
{
    public class CategolizedImage
    {
        public string Id { get; set; }
        public long TweetId { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public string JaCategory { get; set; }
        public string Description { get; set; }
        public string JaDescription { get; set; }
        public string ThemeColor { get; set; }
    }
}
