﻿namespace Simple_Eshop_Admin_Page.Models.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<int> AddCategoryAsync(Category category);
        Task<int> UpdateCategoryAsync(Category category);
        Task<int> DeleteCategoryAsync(int id);
        Task<int> UpdateCategoryNamesAsync(Category category);
    }
}
