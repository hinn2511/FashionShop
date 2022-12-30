using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Params;
using API.Entities;
using API.Entities.WebPageModel;
using API.Extensions;
using API.Helpers;
using API.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.ArticleRepository
{
    public class ArticleRepository : GenericRepository<Article>, IArticleRepository
    {
        private readonly DataContext _context;
        public ArticleRepository(DataContext context, DbSet<Article> set) : base(context, set)
        {
            _context = context;

        }

        public async Task<PagedList<Article>> GetArticlesAsync(CustomerArticleParams articleParams)
        {
            var query = _context.Articles.AsQueryable();

            query = query.Where(x => x.Status == Status.Active);

            if (articleParams.ContentTypes.Contains(ContentType.Information))
                articleParams.ContentTypes.Remove(ContentType.Information);
            
            query = query.Where(x => articleParams.ContentTypes.Contains(x.ContentType));

            if (!string.IsNullOrEmpty(articleParams.Query))
            {
                var words = articleParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.Headline.ToUpper().Contains(word));
                }
            }


            if (articleParams.OrderBy == OrderBy.Ascending)
            {
                query = articleParams.Field switch
                {
                    "PublishedDate" => query.OrderBy(p => p.DateCreated),
                    _ => query.OrderBy(p => p.DateCreated)
                };
            }
            else
            {
                query = articleParams.Field switch
                {
                     "PublishedDate" => query.OrderByDescending(p => p.DateCreated),
                    _ => query.OrderByDescending(p => p.DateCreated)
                };
            }

            query = query.Include(x => x.User);

            return await PagedList<Article>.CreateAsync(query, articleParams.PageNumber, articleParams.PageSize);
        }

        public async Task<PagedList<Article>> GetArticlesAsync(AdminArticleParams articleParams)
        {
            var query = _context.Articles.AsQueryable();

            query = query.Where(x => articleParams.ArticleStatus.Contains(x.Status));
                
            query = query.Where(x => articleParams.ContentTypes.Contains(x.ContentType));

            if (!string.IsNullOrEmpty(articleParams.Query))
            {
                var words = articleParams.Query.RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    query = query.Where(x => x.Headline.ToUpper().Contains(word));
                }
            }


            if (articleParams.OrderBy == OrderBy.Ascending)
            {
                query = articleParams.Field switch
                {
                    "PublishedDate" => query.OrderBy(p => p.DateCreated),
                    "PublishedBy" => query.OrderBy(p => p.User.FirstName),
                    "Headline" => query.OrderBy(p => p.Headline),
                    "HeadlineSlug" => query.OrderBy(p => p.Status),
                    "Status" => query.OrderBy(p => p.Status),
                    "View" => query.OrderBy(p => p.View),
                    "Promoted" => query.OrderBy(p => p.EditorChoice),
                    _ => query.OrderBy(p => p.DateCreated)
                };
            }
            else
            {
                query = articleParams.Field switch
                {
                    "PublishedDate" => query.OrderByDescending(p => p.DateCreated),
                    "PublishedBy" => query.OrderByDescending(p => p.User.FirstName),
                    "Headline" => query.OrderByDescending(p => p.Headline),
                    "HeadlineSlug" => query.OrderByDescending(p => p.Status),
                    "Status" => query.OrderByDescending(p => p.Status),
                    "View" => query.OrderByDescending(p => p.View),
                    "Promoted" => query.OrderByDescending(p => p.EditorChoice),
                    _ => query.OrderByDescending(p => p.DateCreated)
                };
            }

            query = query.Include(x => x.User);

            return await PagedList<Article>.CreateAsync(query, articleParams.PageNumber, articleParams.PageSize);
        }

        public async Task<IEnumerable<Article>> GetMostViewedArticlesAsync(int top)
        {
            return await _context.Articles.Where(x => ( x.ContentType == ContentType.Blog || x.ContentType == ContentType.Promotion ) && x.Status == Status.Active ).Include(x => x.User).OrderByDescending(x => x.View).Take(top).ToListAsync();
        }
    }
}