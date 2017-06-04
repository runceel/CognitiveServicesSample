using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesSample.Data
{
    public class CategolizedImageLoadResponse
    {
        public IEnumerable<CategolizedImage> Image { get; set; }
        public string Continues { get; set; }
    }
}
