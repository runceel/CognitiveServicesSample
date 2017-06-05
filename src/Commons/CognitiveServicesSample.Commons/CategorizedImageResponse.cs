using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesSample.Commons
{
    public class CategorizedImageResponse
    {
        public IEnumerable<CategorizedImage> CategorizedImages { get; set; }
        public string Continuation { get; set; }
    }
}
