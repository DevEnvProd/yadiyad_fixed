﻿using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.NopStation.WebApi.Models.ShoppingCart
{
    public class ShoppingCartApiModel
    {
        public ShoppingCartApiModel()
        {
            Cart = new ShoppingCartModel();
            OrderTotals = new OrderTotalsModel();
            EstimateShipping = new EstimateShippingModel();
        }

        public ShoppingCartModel Cart { get; set; }

        public OrderTotalsModel OrderTotals { get; set; }

        public string SelectedCheckoutAttributes { get; set; }

        public EstimateShippingModel EstimateShipping { get; set; }
    }
}
