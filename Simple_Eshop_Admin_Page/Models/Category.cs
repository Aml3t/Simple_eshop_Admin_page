﻿namespace Simple_Eshop_Admin_Page.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DateAdded { get; set; }

    }
}
