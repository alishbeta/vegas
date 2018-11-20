using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Components
{
    public class OrderSummaryViewComponent : NopViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IRewardPointService _rewardPointService;

        public OrderSummaryViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext,
            IRewardPointService rewardPointService)
        {
            this._shoppingCartModelFactory = shoppingCartModelFactory;
            this._storeContext = storeContext;
            this._workContext = workContext;
            this._rewardPointService = rewardPointService;
        }

        public IViewComponentResult Invoke(bool? prepareAndDisplayOrderReviewData, ShoppingCartModel overriddenModel)
        {
            ViewBag.BonusesBalance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
            //use already prepared (shared) model
            if (overriddenModel != null)
                return View(overriddenModel);

            //if not passed, then create a new model
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

			var model = new ShoppingCartModel();
            model = _shoppingCartModelFactory.PrepareShoppingCartModel(model, cart,
                isEditable: false,
                prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
            return View(model);
        }
    }
}
