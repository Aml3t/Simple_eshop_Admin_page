﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace Simple_Eshop_Admin_Page.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BethanysPieShopDbContext _bethanysPieShopDbContext;

        private IMemoryCache _memoryCache;
        private const string AllCategoriesCacheName = "AllCategories";

        public CategoryRepository(BethanysPieShopDbContext bethanysPieShopDbContext,
            IMemoryCache memoryCache)
        {
            _bethanysPieShopDbContext = bethanysPieShopDbContext;
            _memoryCache = memoryCache;
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            bool categoryWithSameNameExists = await
                _bethanysPieShopDbContext.Categories.AnyAsync(c => c.Name == category.Name);

            if (categoryWithSameNameExists)
            {
                throw new Exception("The entered category already exists");
            }

            _bethanysPieShopDbContext.Categories.Add(category);

            return await _bethanysPieShopDbContext.SaveChangesAsync();
        }

        public async IEnumerable<Category> GetAllCategories()
        {
            return _bethanysPieShopDbContext.Categories
                .AsNoTracking()
                .OrderBy(p => p.CategoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            List<Category> allCategories = null;

            if (!_memoryCache.TryGetValue(AllCategoriesCacheName, out allCategories))
            {
                allCategories = await _bethanysPieShopDbContext.Categories
                    .AsNoTracking()
                    .OrderBy(c => c.CategoryId)
                    .ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60));

                _memoryCache.Set(AllCategoriesCacheName, allCategories,
                    cacheEntryOptions);
            }
            return allCategories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _bethanysPieShopDbContext.Categories
                .Include(p => p.Pies)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<int> UpdateCategoryAsync(Category category)
        {
            bool categoryWithSameNameExists = await
                _bethanysPieShopDbContext.Categories
                .AnyAsync(c => c.Name == category.Name
                && c.CategoryId != category.CategoryId);

            if (categoryWithSameNameExists)
            {
                throw new Exception("A category with the same name already exists");
            }

            var categoryToUpdate = await
                _bethanysPieShopDbContext.Categories.FirstOrDefaultAsync
                (c => c.CategoryId == category.CategoryId);

            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = category.Name;
                categoryToUpdate.Description = category.Description;

                _bethanysPieShopDbContext.Categories.Update(categoryToUpdate);
                return await _bethanysPieShopDbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("The category to update can't be found");
            }

        }

        public async Task<int> DeleteCategoryAsync(int id)
        {
            var categoryToDelete = await _bethanysPieShopDbContext.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            var piesInCategory = _bethanysPieShopDbContext.Pies
                .Any(p => p.CategoryId == id);

            if (piesInCategory is true)
            {
                throw new Exception("Pies exist in this repository. Please delete all pies" +
                    "in this category before deleting the category");
            }

            if (categoryToDelete != null)
            {
                _bethanysPieShopDbContext.Categories.Remove(categoryToDelete);
                return await _bethanysPieShopDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"The category to delete can't be found.");
            }
        }

    }
}
