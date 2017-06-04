using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesSample.Data
{
    public interface ICategolizedImageRepository
    {
        Task<CategolizedImageLoadResponse> LoadAsync(string category, string continuation);
        Task InsertAsync(CategolizedImage data);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<bool> IsExistTweet(long id);
        Task<int> CountImageByCategoryAsync(string category);
    }
}
