using CognitiveServicesSample.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServiceSample.Jobs.Services
{
    public interface IVisionService
    {
        Task<IEnumerable<CategorizedImage>> CategolizedImageAsync(IEnumerable<TwitterSearchResult> tweets);
    }
}
