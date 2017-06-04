using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServicesSample.Commons
{
    public class CategolizedImageResponse
    {
        public IEnumerable<CategolizedImage> CategolizedImages { get; set; }
        public string Continuation { get; set; }
    }
}
