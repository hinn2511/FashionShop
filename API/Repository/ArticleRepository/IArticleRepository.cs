using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities.WebPageModel;
using API.Helpers;
using API.Repository.GenericRepository;

namespace API.Repository.ArticleRepository
{
    public interface IArticleRepository : IGenericRepository<Article>
    {
        Task<PagedList<Article>> GetArticlesAsync(CustomerArticleParams articleParams);
        Task<PagedList<Article>> GetArticlesAsync(AdminArticleParams articleParams);
        Task<IEnumerable<Article>> GetMostViewedArticlesAsync(int top);
    }
}