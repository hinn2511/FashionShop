using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.Request.ArticleRequest;
using API.DTOs.Response;
using API.DTOs.Response.ArticleResponse;
using API.Entities;
using API.Entities.WebPageModel;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
    public class ArticleController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region customer

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetArticlesAsCustomer([FromQuery] CustomerArticleParams articleParams)
        {

            var articles = await _unitOfWork.ArticleRepository.GetArticlesAsync(articleParams);

            Response.AddPaginationHeader(articles.CurrentPage, articles.PageSize, articles.TotalCount, articles.TotalPages);

            var result = _mapper.Map<List<CustomerArticleResponse>>(articles.ToList());

            return Ok(result);

        }

        [AllowAnonymous]
        [HttpGet("{articleId}")]
        public async Task<ActionResult> GetArticlesAsCustomer(int articleId)
        {

            var article = await _unitOfWork.ArticleRepository.GetFirstByAndIncludeAsync(x => x.Id == articleId, "User", false);

            if (article == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            var result = _mapper.Map<CustomerArticleDetailResponse>(article);

            article.View = article.View + 1;

            _unitOfWork.ArticleRepository.Update(article);

            if (await _unitOfWork.Complete())
            {
                return Ok(result);
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while adding the article."));
        }

        [AllowAnonymous]
        [HttpGet("most-viewed")]
        public async Task<ActionResult> GetTrendingArticlesAsCustomer([FromQuery] int top)
        {

            var articles = await _unitOfWork.ArticleRepository.GetMostViewedArticlesAsync(top);

            var result = _mapper.Map<List<CustomerArticleResponse>>(articles);

            return Ok(result);

        }

        [AllowAnonymous]
        [HttpGet("editor-choice")]
        public async Task<ActionResult> GetEditorChoiceArticlesAsCustomer()
        {

            var articles = await _unitOfWork.ArticleRepository.GetAllBy(x => x.EditorChoice && x.Status == Status.Active);

            var result = _mapper.Map<List<CustomerArticleResponse>>(articles);

            return Ok(result);

        }

        #endregion



        #region manager

        [HttpGet("all")]
        public async Task<ActionResult> GetArticlesAsManager([FromQuery] AdminArticleParams articleParams)
        {

            var articles = await _unitOfWork.ArticleRepository.GetArticlesAsync(articleParams);

            Response.AddPaginationHeader(articles.CurrentPage, articles.PageSize, articles.TotalCount, articles.TotalPages);

            var result = _mapper.Map<List<AdminArticleResponse>>(articles.ToList());

            return Ok(result);

        }

        [HttpGet("{articleId}/detail")]
        public async Task<ActionResult> GetArticlesAsManager(int articleId)
        {
            var article = await _unitOfWork.ArticleRepository.GetFirstBy(x => x.Id == articleId);

            if (article == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            var result = _mapper.Map<AdminArticleDetailResponse>(article);

            return Ok(result);


        }

        [HttpPost("create")]
        public async Task<ActionResult> AddArticle(CreateArticleRequest createArticleRequest)
        {

            var article = new Article();
            _mapper.Map(createArticleRequest, article);

            article.HeadlineSlug = article.Headline.GenerateSlug();
            await CheckIfSlugExisted(article);

            article.ContentAbstract = article.Content.StripHTML();
            article.AddCreateInformation(GetUserId());

            article.UserId = GetUserId();

            _unitOfWork.ArticleRepository.Insert(article);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while adding the article."));
        }

        [HttpPut("edit/{id}")]
        public async Task<ActionResult> UpdateArticle(int id, UpdateArticleRequest updateArticleRequest)
        {
            var article = await _unitOfWork.ArticleRepository.GetById(id);

            if (article == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            var userId = article.UserId;

            _mapper.Map(updateArticleRequest, article);

            article.Id = id;
            article.HeadlineSlug = article.Headline.GenerateSlug();
            await CheckIfSlugExisted(article);

            article.ContentAbstract = article.Content.StripHTML();
            article.UserId = article.UserId;
            article.AddUpdateInformation(GetUserId());

            _unitOfWork.ArticleRepository.Update(article);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while updating the article."));
        }

        [HttpDelete("soft-delete")]
        public async Task<ActionResult> SoftDeleteArticle(DeleteArticlesRequest deleteArticlesRequest)
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllBy(x => deleteArticlesRequest.Ids.Contains(x.Id));

            if (articles == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            foreach (var article in articles)
            {
                if (article.Status == Status.Deleted)
                {
                    continue;
                }
                article.AddDeleteInformation(GetUserId());
            }

            _unitOfWork.ArticleRepository.Update(articles);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while deleting articles."));
        }

        [HttpDelete("hard-delete")]
        public async Task<ActionResult> HardDeleteArticle(DeleteArticlesRequest deleteArticlesRequest)
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllBy(x => deleteArticlesRequest.Ids.Contains(x.Id));

            if (articles == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            _unitOfWork.ArticleRepository.Delete(articles);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while deleting articles."));
        }

        [HttpPut("hide")]
        public async Task<ActionResult> HidingArticle(HideArticlesRequest hideArticlesRequest)
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllBy(x => hideArticlesRequest.Ids.Contains(x.Id) && x.Status == Status.Active);

            if (articles == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            foreach (var article in articles)
            {
                article.AddHiddenInformation(GetUserId());

            }

            _unitOfWork.ArticleRepository.Update(articles);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Successfully hide {articles.Count()} article(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while active articles."));
        }

        [HttpPut("activate")]
        public async Task<ActionResult> ActiveArticle(HideArticlesRequest hideArticlesRequest)
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllBy(x => hideArticlesRequest.Ids.Contains(x.Id) && x.Status == Status.Hidden);

            if (articles == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            foreach (var article in articles)
            {
                article.Status = Status.Active;
                article.AddUpdateInformation(GetUserId());
            }

            _unitOfWork.ArticleRepository.Update(articles);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Successfully unhide {articles.Count()} article(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while hiding articles."));
        }

        [HttpPut("remove-editor-choice")]
        public async Task<ActionResult> RemoveEditorChoiceForArticle(EditorChoiceRequest editorChoiceRequest)
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllBy(x => editorChoiceRequest.Ids.Contains(x.Id));

            if (articles == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            foreach (var article in articles)
            {
                if (article.EditorChoice)
                    article.EditorChoice = false;
            }

            _unitOfWork.ArticleRepository.Update(articles);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully remove editor choice for {articles.Count()} article(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while remove editor choice for articles."));
        }

        [HttpPut("set-editor-choice")]
        public async Task<ActionResult> SetEditorChoiceForArticle(EditorChoiceRequest editorChoiceRequest)
        {
            var articles = await _unitOfWork.ArticleRepository.GetAllBy(x => editorChoiceRequest.Ids.Contains(x.Id));

            if (articles == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Article not found"));

            foreach (var article in articles)
            {
                if (!article.EditorChoice)
                    article.EditorChoice = true;
            }

            _unitOfWork.ArticleRepository.Update(articles);

            if (await _unitOfWork.Complete())
            {
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Successfully set editor choice for {articles.Count()} article(s)."));
            }
            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "An error occurred while set editor choice for articles."));
        }


        #endregion

        #region private method
        private async Task CheckIfSlugExisted(Article article)
        {
            var articlesExisted = await _unitOfWork.ArticleRepository.GetAllBy(x => x.HeadlineSlug == article.HeadlineSlug);

            var count = articlesExisted.Count();
            if (count > 0)
                article.HeadlineSlug = $"{article.HeadlineSlug}-{++count}";
        }
        #endregion
    }
}