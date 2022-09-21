using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities.WebPageModel;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.ConfigurationRepository
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly DataContext _context;

        public ConfigurationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task BulkCreate(IEnumerable<Carousel> carousels)
        {
            await _context.Carousels.AddRangeAsync(carousels);
        }

        public async Task BulkCreate(IEnumerable<FeatureCategory> featureCategories)
        {
            await _context.FeatureCategories.AddRangeAsync(featureCategories);
        }

        public async Task BulkCreate(IEnumerable<FeatureProduct> featureProducts)
        {
            await _context.FeatureProducts.AddRangeAsync(featureProducts);
        }

        #region home page

        public void Create(HomePage homePage)
        {
            _context.HomePages.Add(homePage);
        }

        public void Update(HomePage homePage)
        {
            _context.Entry(homePage).State = EntityState.Modified;
        }

        public async Task<HomePage> GetActiveHomePage()
        {
            return await _context.HomePages
                        .Include(x => x.Carousels)
                        .Include(x=> x.FeatureCategories)
                        .Include(x => x.FeatureProducts)
                        .FirstOrDefaultAsync(x => x.IsActive);
                                
        }

        public async Task<HomePage> GetHomePageById(int id)
        {
            return await _context.HomePages                       
                        .FirstOrDefaultAsync(x => x.Id == id);                                
        }

        #endregion
    }
}