﻿using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class OrderTotalsViewComponent : NopViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public OrderTotalsViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public IViewComponentResult Invoke(bool isEditable, bool? isShowShipping = null, bool? isShowTax = null, bool? isShowDiscount = null)
        {
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id, onlySelectedItems:true);

            var model = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, isEditable);

            if(isShowShipping == false)
            {
                model.HideShippingTotal = true;
            }
            if (isShowTax == false)
            {
                model.DisplayTax = false;
            }
            if(isShowDiscount == false)
            {
                model.HideDiscountTotal = true;
            }
            return View(model);
        }
    }
}
