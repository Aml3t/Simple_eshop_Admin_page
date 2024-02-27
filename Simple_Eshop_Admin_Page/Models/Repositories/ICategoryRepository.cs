﻿namespace Simple_Eshop_Admin_Page.Models.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();

        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}