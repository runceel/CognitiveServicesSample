using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesSample.Data
{
    public interface ICategorizedImageRepository
    {
        Task<CategorizedImageLoadResponse> LoadAsync(string category, string continuation);
        Task InsertAsync(CategorizedImage data);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<bool> IsExistTweet(long id);
        Task<int> CountImageByCategoryAsync(string category);
    }
}
