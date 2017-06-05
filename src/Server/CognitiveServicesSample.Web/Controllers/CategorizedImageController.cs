using CognitiveServicesSample.Commons;
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
    public class CategorizedImageController : ApiController
    {
        private ICategorizedImageRepository CategorizedImageRepository { get; }
        private ILogger Logger { get; }

        public CategorizedImageController(ICategorizedImageRepository categorizedImageRepository, ILogger logger)
        {
            this.CategorizedImageRepository = categorizedImageRepository;
            this.Logger = logger;
        }

        public async Task<IHttpActionResult> Get(string category, string continuation = null)
        {
            this.Logger.Info($"{nameof(CategorizedImageController)}.{nameof(Get)}({category}, {continuation})");

            var r = await this.CategorizedImageRepository.LoadAsync(category, continuation);
            return Ok(new Commons.CategorizedImageResponse
            {
                CategorizedImages = r.Image.Select(x => new Commons.CategorizedImage
                {
                    Id = x.Id,
                    Category = x.Category,
                    Description = x.Description,
                    Image = x.Image,
                    JaCategory = x.JaCategory,
                    JaDescription = x.JaDescription,
                    Text = x.Text,
                    ThemeColor = x.ThemeColor,
                    TweetId = x.TweetId,
                }),
                Continuation = r.Continues,
            });
        }
    }
}
