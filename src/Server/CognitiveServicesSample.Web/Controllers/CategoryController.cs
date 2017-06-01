using CognitiveServicesSample.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CognitiveServicesSample.Web.Controllers
{
    public class CategoryController : ApiController
    {
        private ICategolizedImageRepository CategolizedImageRepository { get; }

        public CategoryController(ICategolizedImageRepository categolizedImageRepository)
        {
            this.CategolizedImageRepository = categolizedImageRepository;
        }

        public async Task<IHttpActionResult> Get()
        {
            var categories = await this.CategolizedImageRepository.GetCategoriesAsync();
            var tasks = categories.Select(async x => new Commons.Category
            {
                Name = x.Name,
                JaName = x.JaName,
                ImageCount = await this.CategolizedImageRepository.CountImageByCategoryAsync(x.Name),
            })
            .ToArray();
            await Task.WhenAll(tasks);
            return Ok(tasks.Select(x => x.Result));
        }
    }
}
