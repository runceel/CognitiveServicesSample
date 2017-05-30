using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesSample.Data
{
    public interface ICategolizedImageRepository
    {
        Task<IEnumerable<CategolizedImage>> LoadAsync(int skip, int pageSize, string category);
        Task InsertAsync(CategolizedImage data);
        Task<IEnumerable<Category>> GetCategoriesAsync();
    }
}
