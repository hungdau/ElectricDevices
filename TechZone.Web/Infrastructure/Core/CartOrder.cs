﻿using System;
using System.Collections.Generic;
using TechZone.Web.Models;

namespace TechZone.Web.Infrastructure.Core
{
    public class CartOrder
    {
        public string Id { set; get; }
        public DateTime? OrderDate { set; get; }
        public bool Status { set; get; }
        public IEnumerable<ShoppingCartViewModel> Cart { set; get; }
    }
}