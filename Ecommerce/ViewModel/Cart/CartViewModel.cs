﻿using Ecommerce.Models;

namespace Ecommerce.ViewModel.Cart
{
    public class CartViewModel : BaseCart
    {
        public List<CartItemViewModel>? CartItems { get; set; }

    }
}
