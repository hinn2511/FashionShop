using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities.WebPageModel;

namespace API.Repository.ConfigurationRepository
{
    public interface IConfigurationRepository
    {
        #region home page
        void Create(HomePage homePage);
        void Update(HomePage homePage);
        Task<HomePage> GetActiveHomePage();
        Task<HomePage> GetHomePageById(int id);

        #endregion

        #region carousel
        Task BulkCreate(IEnumerable<Carousel> carousels);

        #endregion

        #region feature category
        Task BulkCreate(IEnumerable<FeatureCategory> featureCategories);

        #endregion

        #region feature product
        Task BulkCreate(IEnumerable<FeatureProduct> featureProducts);

        #endregion
    }
}