﻿using System;
using System.Collections.Generic;

#nullable disable

namespace T2010A_WEBAPI.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
