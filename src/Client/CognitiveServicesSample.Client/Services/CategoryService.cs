using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesSample.Commons;
using System.Net.Http;
using Newtonsoft.Json;

namespace CognitiveServicesSample.Client.Services
{
    public class CategoryService : ICategoryService
    {
        private HttpClient Client { get; }

        public CategoryService(HttpClient client)
        {
            this.Client = client;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var res = await this.Client.GetAsync($"{Consts.ApiEndpoint}/api/Category");
            res.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<Category>>(await res.Content.ReadAsStringAsync());
        }
    }
}
