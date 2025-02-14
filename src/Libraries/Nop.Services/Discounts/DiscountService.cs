﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Discount service
    /// </summary>
    public partial class DiscountService : IDiscountService
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountPluginManager _discountPluginManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<DiscountRequirement> _discountRequirementRepository;
        private readonly IRepository<DiscountUsageHistory> _discountUsageHistoryRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public DiscountService(ICacheKeyService cacheKeyService,
            ICustomerService customerService,
            IDiscountPluginManager discountPluginManager,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IProductService productService,
            IRepository<Discount> discountRepository,
            IRepository<DiscountRequirement> discountRequirementRepository,
            IRepository<DiscountUsageHistory> discountUsageHistoryRepository,
            IRepository<Order> orderRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext)
        {
            _cacheKeyService = cacheKeyService;
            _customerService = customerService;
            _discountPluginManager = discountPluginManager;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _productService = productService;
            _discountRepository = discountRepository;
            _discountRequirementRepository = discountRequirementRepository;
            _discountUsageHistoryRepository = discountUsageHistoryRepository;
            _orderRepository = orderRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get discount validation result
        /// </summary>
        /// <param name="requirements">Collection of discount requirement</param>
        /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
        /// <param name="customer">Customer</param>
        /// <param name="errors">Errors</param>
        /// <returns>True if result is valid; otherwise false</returns>
        protected bool GetValidationResult(IEnumerable<DiscountRequirement> requirements,
            RequirementGroupInteractionType groupInteractionType, Customer customer, List<string> errors)
        {
            var result = false;

            foreach (var requirement in requirements)
            {
                if (requirement.IsGroup)
                {
                    var childRequirements = GetDiscountRequirementsByParent(requirement);
                    //get child requirements for the group
                    var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
                    result = GetValidationResult(childRequirements, interactionType, customer, errors);
                }
                else
                {
                    //or try to get validation result for the requirement
                    var requirementRulePlugin = _discountPluginManager
                        .LoadPluginBySystemName(requirement.DiscountRequirementRuleSystemName, customer, _storeContext.CurrentStore.Id);
                    if (requirementRulePlugin == null)
                        continue;

                    var ruleResult = requirementRulePlugin.CheckRequirement(new DiscountRequirementValidationRequest
                    {
                        DiscountRequirementId = requirement.Id,
                        Customer = customer,
                        Store = _storeContext.CurrentStore
                    });

                    //add validation error
                    if (!ruleResult.IsValid)
                    {
                        var userError = !string.IsNullOrEmpty(ruleResult.UserError)
                            ? ruleResult.UserError
                            : _localizationService.GetResource("ShoppingCart.Discount.CannotBeUsed");
                        errors.Add(userError);
                    }

                    result = ruleResult.IsValid;
                }

                //all requirements must be met, so return false
                if (!result && groupInteractionType == RequirementGroupInteractionType.And)
                    return false;

                //any of requirements must be met, so return true
                if (result && groupInteractionType == RequirementGroupInteractionType.Or)
                    return true;
            }

            return result;
        }

        #endregion

        #region Methods

        #region Discounts

        /// <summary>
        /// Delete discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void DeleteDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            _discountRepository.Delete(discount);

            //event notification
            _eventPublisher.EntityDeleted(discount);
        }

        /// <summary>
        /// Gets a discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Discount</returns>
        public virtual Discount GetDiscountById(int discountId)
        {
            if (discountId == 0)
                return null;

            return _discountRepository.ToCachedGetById(discountId);
        }
        public virtual List<Discount> GetPotentialDiscounts(List<int> vendorIds, List<DiscountType> discountTypes, int customerId)
        {
            var discountTypeIds = discountTypes != null && discountTypes.Count > 0 ? discountTypes.Select(y => (int)y).ToList() : new List<int>();
            var discounts = _discountRepository.Table
                .Where(x => 
                    vendorIds != null 
                    && vendorIds.Count > 0 
                    && ((x.VendorId.HasValue 
                        && vendorIds.Contains(x.VendorId.Value))
                        || x.VendorId == null))
                .Where(x => discountTypeIds.Count > 0 ? discountTypeIds.Contains(x.DiscountTypeId) : true)
                //.Where(x => x.MinimmumSubTotalAmount.HasValue)
                .Where(x =>
                    (!x.StartDateUtc.HasValue || x.StartDateUtc <= DateTime.UtcNow) &&
                    (!x.EndDateUtc.HasValue || x.EndDateUtc >= DateTime.UtcNow))
                .ToList();

            // filter out discounts used exceed limit
            var discountWithLimitationIds = discounts
                .Where(x => x.DiscountLimitation == DiscountLimitationType.NTimesOnly || x.DiscountLimitation == DiscountLimitationType.NTimesPerCustomer)
                .Select(x => x.Id).ToList();
            if (discountWithLimitationIds.Count > 0)
            {
                var discountUsage = from duh in _discountUsageHistoryRepository.Table
                    join order in _orderRepository.Table on duh.OrderId equals order.Id
                    where !order.Deleted 
                    && discountWithLimitationIds.Contains(duh.DiscountId)
                    select new {
                        DiscountId = duh.DiscountId,
                        CustomerId = order.CustomerId,
                    };
                var discountUsageGroupped = discountUsage
                    .ToList()
                    .GroupBy(x => x.DiscountId)
                    .Select(x => new
                    {
                        DiscountId = x.Key,
                        TotalUsage = x.Count(),
                        UsageByCustomer = x.GroupBy(x => x.CustomerId)
                        .Select(y => new
                        {
                            CustomerId = y.Key,
                            TotalUsage = y.Count()
                        })
                    })
                    .ToList();
                discounts = discounts
                    .Where(x => x.DiscountLimitation == DiscountLimitationType.Unlimited
                    || (x.DiscountLimitation != DiscountLimitationType.Unlimited && discountUsageGroupped.Where(y => y.DiscountId == x.Id).FirstOrDefault() == null)
                    || (x.DiscountLimitation == DiscountLimitationType.NTimesOnly && discountUsageGroupped.Where(y => y.DiscountId == x.Id).First().TotalUsage < x.LimitationTimes)
                    || (x.DiscountLimitation == DiscountLimitationType.NTimesPerCustomer && (
                                discountUsageGroupped.Where(y => y.DiscountId == x.Id).First().UsageByCustomer.Where(y => y.CustomerId == customerId).FirstOrDefault() == null
                                || discountUsageGroupped.Where(y => y.DiscountId == x.Id).First().UsageByCustomer.Where(y => y.CustomerId == customerId).First().TotalUsage < x.LimitationTimes
                            )
                        )
                    )
                    .ToList();
            }

            var potentialDiscount = discounts
                .GroupBy(x => new { VendorId = x.VendorId, DiscountType = x.DiscountType } )
                .Select(x =>
                    x.OrderByDescending(y =>
                    {
                        int estimateShippingFee = 15;
                        var discountAmount = y.UsePercentage ? estimateShippingFee * y.DiscountPercentage : y.DiscountAmount;
                        return y.MaximumDiscountAmount.HasValue && discountAmount > y.MaximumDiscountAmount.Value ? y.MaximumDiscountAmount.Value : discountAmount;
                    })
                    .First()
                )
                .ToList();

            return potentialDiscount;
        }

        /// <summary>
        /// Gets all discounts
        /// </summary>
        /// <param name="discountType">Discount type; pass null to load all records</param>
        /// <param name="couponCode">Coupon code to find (exact match); pass null or empty to load all records</param>
        /// <param name="discountName">Discount name; pass null or empty to load all records</param>
        /// <param name="vendorId">Vendor Id; pass 0 or empty to load all records</param>
        /// <param name="showHidden">A value indicating whether to show expired and not started discounts</param>
        /// <param name="startDateUtc">Discount start date; pass null to load all records</param>
        /// <param name="endDateUtc">Discount end date; pass null to load all records</param>
        /// <returns>Discounts</returns>
        public virtual IList<Discount> GetAllDiscounts(DiscountType? discountType = null,
            string couponCode = null, int vendorId = 0, string discountName = null, bool showHidden = false,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null)
        {
            //we load all discounts, and filter them using "discountType" parameter later (in memory)
            //we do it because we know that this method is invoked several times per HTTP request with distinct "discountType" parameter
            //that's why let's access the database only once
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopDiscountDefaults.DiscountAllCacheKey
                , showHidden, couponCode ?? string.Empty, discountName ?? string.Empty);

            var query = _discountRepository.Table;

            if (!showHidden)
            {
                query = query.Where(discount =>
                    (!discount.StartDateUtc.HasValue || discount.StartDateUtc <= DateTime.UtcNow) &&
                    (!discount.EndDateUtc.HasValue || discount.EndDateUtc >= DateTime.UtcNow));
            }

            //filter by coupon code
            if (!string.IsNullOrEmpty(couponCode))
                query = query.Where(discount => discount.CouponCode == couponCode);
            
            //filter by name
            if (!string.IsNullOrEmpty(discountName))
                query = query.Where(discount => discount.Name.Contains(discountName));

            query = query.OrderBy(discount => discount.Name).ThenBy(discount => discount.Id);

            query = query.ToCachedList(cacheKey).AsQueryable();

            //we know that this method is usually invoked multiple times
            //that's why we filter discounts by type and dates on the application layer
            if (discountType.HasValue)
                query = query.Where(discount => discount.DiscountType == discountType.Value);

            //filter by dates
            if (startDateUtc.HasValue)
                query = query.Where(discount =>
                    !discount.StartDateUtc.HasValue || discount.StartDateUtc >= startDateUtc.Value);
            if (endDateUtc.HasValue)
                query = query.Where(discount =>
                    !discount.EndDateUtc.HasValue || discount.EndDateUtc <= endDateUtc.Value);

            if (vendorId > 0)
                query = query.Where(x => x.VendorId == vendorId);
            
            return query.ToList();
        }

        /// <summary>
        /// Gets discounts applied to entity
        /// </summary>
        /// <typeparam name="T">Type based on <see cref="DiscountMapping" /></typeparam>
        /// <param name="entity">Entity which supports discounts (<see cref="IDiscountSupported{T}" />)</param>
        /// <returns>List of discounts</returns>
        public virtual IList<Discount> GetAppliedDiscounts<T>(IDiscountSupported<T> entity) where T : DiscountMapping
        {
            var _discountMappingRepository = EngineContext.Current.Resolve<IRepository<T>>();

            return (from d in _discountRepository.Table
                    join ad in _discountMappingRepository.Table on d.Id equals ad.DiscountId
                    where ad.EntityId == entity.Id
                    select d).ToList();
        }

        /// <summary>
        /// Inserts a discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void InsertDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            _discountRepository.Insert(discount);

            //event notification
            _eventPublisher.EntityInserted(discount);
        }

        /// <summary>
        /// Updates the discount
        /// </summary>
        /// <param name="discount">Discount</param>
        public virtual void UpdateDiscount(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            _discountRepository.Update(discount);

            //event notification
            _eventPublisher.EntityUpdated(discount);
        }

        #endregion

        #region Discounts (caching)
        
        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="amount">Amount</param>
        /// <returns>The discount amount</returns>
        public virtual decimal GetDiscountAmount(Discount discount, decimal amount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //calculate discount amount
            decimal result;
            if (discount.UsePercentage)
                result = (decimal)((float)amount * (float)discount.DiscountPercentage / 100f);
            else
                result = discount.DiscountAmount;

            //validate maximum discount amount
            if (discount.UsePercentage &&
                discount.MaximumDiscountAmount.HasValue &&
                result > discount.MaximumDiscountAmount.Value)
                result = discount.MaximumDiscountAmount.Value;

            if (result >= amount)
                result = amount;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Get preferred discount (with maximum discount value)
        /// </summary>
        /// <param name="discounts">A list of discounts to check</param>
        /// <param name="amount">Amount (initial value)</param>
        /// <param name="discountAmount">Discount amount</param>
        /// <returns>Preferred discount</returns>
        public virtual List<Discount> GetPreferredDiscount(IList<Discount> discounts,
            decimal amount, out decimal discountAmount)
        {
            if (discounts == null)
                throw new ArgumentNullException(nameof(discounts));

            var result = new List<Discount>();
            discountAmount = decimal.Zero;
            if (!discounts.Any())
                return result;

            //first we check simple discounts
            foreach (var discount in discounts)
            {
                var currentDiscountValue = GetDiscountAmount(discount, amount);
                if (currentDiscountValue <= discountAmount)
                    continue;

                discountAmount = currentDiscountValue;

                result.Clear();
                result.Add(discount);
            }
            //now let's check cumulative discounts
            //right now we calculate discount values based on the original amount value
            //please keep it in mind if you're going to use discounts with "percentage"
            var cumulativeDiscounts = discounts.Where(x => x.IsCumulative).OrderBy(x => x.Name).ToList();
            if (cumulativeDiscounts.Count <= 1)
                return result;

            var cumulativeDiscountAmount = cumulativeDiscounts.Sum(d => GetDiscountAmount(d, amount));
            if (cumulativeDiscountAmount <= discountAmount)
                return result;

            discountAmount = cumulativeDiscountAmount;

            result.Clear();
            result.AddRange(cumulativeDiscounts);

            return result;
        }

        /// <summary>
        /// Check whether a list of discounts already contains a certain discount instance
        /// </summary>
        /// <param name="discounts">A list of discounts</param>
        /// <param name="discount">Discount to check</param>
        /// <returns>Result</returns>
        public virtual bool ContainsDiscount(IList<Discount> discounts, Discount discount)
        {
            if (discounts == null)
                throw new ArgumentNullException(nameof(discounts));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            foreach (var dis1 in discounts)
                if (discount.Id == dis1.Id)
                    return true;

            return false;
        }
        #endregion

        #region Discount requirements

        /// <summary>
        /// Get all discount requirements
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="topLevelOnly">Whether to load top-level requirements only (without parent identifier)</param>
        /// <returns>Requirements</returns>
        public virtual IList<DiscountRequirement> GetAllDiscountRequirements(int discountId = 0, bool topLevelOnly = false)
        {
            var query = _discountRequirementRepository.Table;

            //filter by discount
            if (discountId > 0)
                query = query.Where(requirement => requirement.DiscountId == discountId);

            //filter by top-level
            if (topLevelOnly)
                query = query.Where(requirement => !requirement.ParentId.HasValue);

            query = query.OrderBy(requirement => requirement.Id);

            return query.ToList();
        }

        /// <summary>
        /// Get a discount requirement
        /// </summary>
        /// <param name="discountRequirementId">Discount requirement identifier</param>
        public virtual DiscountRequirement GetDiscountRequirementById(int discountRequirementId)
        {
            if (discountRequirementId == 0)
                return null;

            return _discountRequirementRepository.ToCachedGetById(discountRequirementId);
        }

        /// <summary>
        /// Gets child discount requirements
        /// </summary>
        /// <param name="discountRequirement">Parent discount requirement</param>
        public virtual IList<DiscountRequirement> GetDiscountRequirementsByParent(DiscountRequirement discountRequirement)
        {
            if (discountRequirement is null)
                throw new ArgumentNullException(nameof(discountRequirement));

            return _discountRequirementRepository.Table.Where(dr => dr.ParentId == discountRequirement.Id).ToList();
        }

        /// <summary>
        /// Delete discount requirement
        /// </summary>
        /// <param name="discountRequirement">Discount requirement</param>
        /// <param name="recursive">A value indicating whether to recursively delete child requirements</param>
        public virtual void DeleteDiscountRequirement(DiscountRequirement discountRequirement, bool recursive = false)
        {
            if (discountRequirement == null)
                throw new ArgumentNullException(nameof(discountRequirement));

            if (recursive && GetDiscountRequirementsByParent(discountRequirement) is IList<DiscountRequirement> children && children.Any())
            {
                foreach (var child in children)
                    DeleteDiscountRequirement(child, true);
            }

            _discountRequirementRepository.Delete(discountRequirement);

            //event notification
            _eventPublisher.EntityDeleted(discountRequirement);
        }

        /// <summary>
        /// Inserts a discount requirement
        /// </summary>
        /// <param name="discountRequirement">Discount requirement</param>
        public virtual void InsertDiscountRequirement(DiscountRequirement discountRequirement)
        {
            if (discountRequirement is null)
                throw new ArgumentNullException(nameof(discountRequirement));

            _discountRequirementRepository.Insert(discountRequirement);

            //event notification
            _eventPublisher.EntityInserted(discountRequirement);
        }

        /// <summary>
        /// Updates a discount requirement
        /// </summary>
        /// <param name="discountRequirement">Discount requirement</param>
        public virtual void UpdateDiscountRequirement(DiscountRequirement discountRequirement)
        {
            if (discountRequirement is null)
                throw new ArgumentNullException(nameof(discountRequirement));

            _discountRequirementRepository.Update(discountRequirement);

            //event notification
            _eventPublisher.EntityUpdated(discountRequirement);
        }

        #endregion

        #region Validation
        
        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discount validation result</returns>
        public virtual DiscountValidationResult ValidateDiscount(Discount discount, Customer customer)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var couponCodesToValidate = _customerService.ParseAppliedDiscountCouponCodes(customer);
            return ValidateDiscount(discount, customer, couponCodesToValidate);
        }

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <param name="couponCodesToValidate">Coupon codes to validate</param>
        /// <returns>Discount validation result</returns>
        public virtual DiscountValidationResult ValidateDiscount(Discount discount, Customer customer, string[] couponCodesToValidate)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //invalid by default
            var result = new DiscountValidationResult();

            //check coupon code
            if (discount.RequiresCouponCode)
            {
                if (string.IsNullOrEmpty(discount.CouponCode))
                    return result;

                if (couponCodesToValidate == null)
                    return result;

                if (!couponCodesToValidate.Any(x => x.Equals(discount.CouponCode, StringComparison.InvariantCultureIgnoreCase)))
                    return result;
            }

            //Do not allow discounts applied to order subtotal or total when a customer has gift cards in the cart.
            //Otherwise, this customer can purchase gift cards with discount and get more than paid ("free money").
            if (discount.DiscountType == DiscountType.AssignedToOrderSubTotal ||
                discount.DiscountType == DiscountType.AssignedToOrderTotal)
            {
                var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
                var cart = shoppingCartService.GetShoppingCart(customer, ShoppingCartType.ShoppingCart, storeId: _storeContext.CurrentStore.Id, onlySelectedItems:true);

                var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();
                
                if (_productService.HasAnyGiftCardProduct(cartProductIds))
                {
                    result.Errors = new List<string> { _localizationService.GetResource("ShoppingCart.Discount.CannotBeUsedWithGiftCards") };
                    return result;
                }
            }

            //check date range
            var now = DateTime.UtcNow;
            if (discount.StartDateUtc.HasValue)
            {
                var startDate = DateTime.SpecifyKind(discount.StartDateUtc.Value, DateTimeKind.Utc);
                if (startDate.CompareTo(now) > 0)
                {
                    result.Errors = new List<string> { _localizationService.GetResource("ShoppingCart.Discount.NotStartedYet") };
                    return result;
                }
            }

            if (discount.EndDateUtc.HasValue)
            {
                var endDate = DateTime.SpecifyKind(discount.EndDateUtc.Value, DateTimeKind.Utc);
                if (endDate.CompareTo(now) < 0)
                {
                    result.Errors = new List<string> { _localizationService.GetResource("ShoppingCart.Discount.Expired") };
                    return result;
                }
            }

            //discount limitation
            switch (discount.DiscountLimitation)
            {
                case DiscountLimitationType.NTimesOnly:
                    {
                        var usedTimes = GetAllDiscountUsageHistory(discount.Id, null, null, 0, 1).TotalCount;
                        if (usedTimes >= discount.LimitationTimes)
                            return result;
                    }

                    break;
                case DiscountLimitationType.NTimesPerCustomer:
                    {
                        if (_customerService.IsRegistered(customer))
                        {
                            var usedTimes = GetAllDiscountUsageHistory(discount.Id, customer.Id, null, 0, 1).TotalCount;
                            if (usedTimes >= discount.LimitationTimes)
                            {
                                result.Errors = new List<string> { _localizationService.GetResource("ShoppingCart.Discount.CannotBeUsedAnymore") };
                                return result;
                            }
                        }
                    }

                    break;
                case DiscountLimitationType.Unlimited:
                default:
                    break;
            }

            //discount requirements
            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopDiscountDefaults.DiscountRequirementModelCacheKey, discount);

            var requirements = _staticCacheManager.Get(key, () => GetAllDiscountRequirements(discount.Id, true));

            //get top-level group
            var topLevelGroup = requirements.FirstOrDefault();
            if (topLevelGroup == null || (topLevelGroup.IsGroup && !GetDiscountRequirementsByParent(topLevelGroup).Any()) || !topLevelGroup.InteractionType.HasValue)
            {
                //there are no requirements, so discount is valid
                result.IsValid = true;
                return result;
            }

            //requirements exist, let's check them
            var errors = new List<string>();
            result.IsValid = GetValidationResult(requirements, topLevelGroup.InteractionType.Value, customer, errors);

            //set errors if result is not valid
            if (!result.IsValid)
                result.Errors = errors;

            return result;
        }

        #endregion

        #region Discount usage history

        /// <summary>
        /// Gets a discount usage history record
        /// </summary>
        /// <param name="discountUsageHistoryId">Discount usage history record identifier</param>
        /// <returns>Discount usage history</returns>
        public virtual DiscountUsageHistory GetDiscountUsageHistoryById(int discountUsageHistoryId)
        {
            if (discountUsageHistoryId == 0)
                return null;

            return _discountUsageHistoryRepository.GetById(discountUsageHistoryId);
        }

        /// <summary>
        /// Gets all discount usage history records
        /// </summary>
        /// <param name="discountId">Discount identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Discount usage history records</returns>
        public virtual IPagedList<DiscountUsageHistory> GetAllDiscountUsageHistory(int? discountId = null,
            int? customerId = null, int? orderId = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var discountUsageHistory = _discountUsageHistoryRepository.Table;

            //filter by discount
            if (discountId.HasValue && discountId.Value > 0)
                discountUsageHistory = discountUsageHistory.Where(historyRecord => historyRecord.DiscountId == discountId.Value);

            //filter by customer
            if (customerId.HasValue && customerId.Value > 0)
                discountUsageHistory = from duh in discountUsageHistory
                    join order in _orderRepository.Table on duh.OrderId equals order.Id
                    where order.CustomerId == customerId
                    select duh;

            //filter by order
            if (orderId.HasValue && orderId.Value > 0)
                discountUsageHistory = discountUsageHistory.Where(historyRecord => historyRecord.OrderId == orderId.Value);

            //ignore deleted orders
            discountUsageHistory = from duh in discountUsageHistory
                join order in _orderRepository.Table on duh.OrderId equals order.Id
                where !order.Deleted
                select duh;

            //order
            discountUsageHistory = discountUsageHistory.OrderByDescending(historyRecord => historyRecord.CreatedOnUtc).ThenBy(historyRecord => historyRecord.Id);

            return new PagedList<DiscountUsageHistory>(discountUsageHistory, pageIndex, pageSize);
        }

        /// <summary>
        /// Insert discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        public virtual void InsertDiscountUsageHistory(DiscountUsageHistory discountUsageHistory)
        {
            if (discountUsageHistory == null)
                throw new ArgumentNullException(nameof(discountUsageHistory));

            _discountUsageHistoryRepository.Insert(discountUsageHistory);

            //event notification
            _eventPublisher.EntityInserted(discountUsageHistory);
        }

        /// <summary>
        /// Update discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        public virtual void UpdateDiscountUsageHistory(DiscountUsageHistory discountUsageHistory)
        {
            if (discountUsageHistory == null)
                throw new ArgumentNullException(nameof(discountUsageHistory));

            _discountUsageHistoryRepository.Update(discountUsageHistory);

            //event notification
            _eventPublisher.EntityUpdated(discountUsageHistory);
        }

        /// <summary>
        /// Delete discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        public virtual void DeleteDiscountUsageHistory(DiscountUsageHistory discountUsageHistory)
        {
            if (discountUsageHistory == null)
                throw new ArgumentNullException(nameof(discountUsageHistory));

            _discountUsageHistoryRepository.Delete(discountUsageHistory);

            //event notification
            _eventPublisher.EntityDeleted(discountUsageHistory);
        }

        #endregion

        #endregion
    }
}