using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServiceSample.Jobs.Services
{
    public interface ITranslatorService
    {
        Task<string> TranslateToJapaneseAsync(string en);
    }
}
