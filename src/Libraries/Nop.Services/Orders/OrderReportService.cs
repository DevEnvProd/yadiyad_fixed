﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Helpers;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order report service
    /// </summary>
    public partial class OrderReportService : IOrderReportService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;

        #endregion

        #region Ctor

        public OrderReportService(CatalogSettings catalogSettings,
            IDateTimeHelper dateTimeHelper,
            IRepository<Address> addressRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<StoreMapping> storeMappingRepository)
        {
            _catalogSettings = catalogSettings;
            _dateTimeHelper = dateTimeHelper;
            _addressRepository = addressRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderNoteRepository = orderNoteRepository;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            _storeMappingRepository = storeMappingRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get "order by country" report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Order status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>Result</returns>
        public virtual IList<OrderByCountryReportLine> GetCountryReport(int storeId, OrderStatus? os,
            PaymentStatus? ps, ShippingStatus? ss, DateTime? startTimeUtc, DateTime? endTimeUtc)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            var query = _orderRepository.Table;
            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (orderStatusId.HasValue)
                query = query.Where(o => o.OrderStatusId == orderStatusId.Value);
            if (paymentStatusId.HasValue)
                query = query.Where(o => o.PaymentStatusId == paymentStatusId.Value);
            if (shippingStatusId.HasValue)
                query = query.Where(o => o.ShippingStatusId == shippingStatusId.Value);
            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);
            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);

            var report = (from oq in query
                          join a in _addressRepository.Table on oq.BillingAddressId equals a.Id
                          group oq by a.CountryId
                          into result
                          select new
                          {
                              CountryId = result.Key,
                              TotalOrders = result.Count(),
                              SumOrders = result.Sum(o => o.OrderTotal)
                          })
                .OrderByDescending(x => x.SumOrders)
                .Select(r => new OrderByCountryReportLine
                {
                    CountryId = r.CountryId,
                    TotalOrders = r.TotalOrders,
                    SumOrders = r.SumOrders
                }).ToList();

            return report;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="orderId">Order identifier; pass 0 to ignore this parameter</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="osIds">Order status identifiers</param>
        /// <param name="psIds">Payment status identifiers</param>
        /// <param name="ssIds">Shipping status identifiers</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="billingPhone">Billing phone. Leave empty to load all records.</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        public virtual OrderAverageReportLine GetOrderAverageReportLine(int storeId = 0,
            int vendorId = 0, int productId = 0, int warehouseId = 0, int billingCountryId = 0,
            int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingPhone = null, string billingEmail = null, string billingLastName = "", string orderNotes = null)
        {
            var query = _orderRepository.Table;

            query = query.Where(o => !o.Deleted);
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (orderId > 0)
                query = query.Where(o => o.Id == orderId);

            if (vendorId > 0)
                query = from o in query
                    join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                    join p in _productRepository.Table on oi.ProductId equals p.Id
                    where p.VendorId == vendorId
                    select o;

            if (productId > 0)
                query = from o in query
                    join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                    where oi.ProductId == productId
                    select o;

            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;

                query = from o in query
                    join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                    join p in _productRepository.Table on oi.ProductId equals p.Id
                    join pwi in _productWarehouseInventoryRepository.Table on p.Id equals pwi.ProductId
                    where
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (p.ManageInventoryMethodId == manageStockInventoryMethodId && p.UseMultipleWarehouses && pwi.WarehouseId == warehouseId) ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((p.ManageInventoryMethodId != manageStockInventoryMethodId || !p.UseMultipleWarehouses) && p.WarehouseId == warehouseId)
                    select o;
            }

            query = from o in query
                join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
                where 
                    (billingCountryId <= 0 || (oba.CountryId == billingCountryId)) &&
                    (string.IsNullOrEmpty(billingPhone) || (!string.IsNullOrEmpty(oba.PhoneNumber) && oba.PhoneNumber.Contains(billingPhone))) &&
                    (string.IsNullOrEmpty(billingEmail) || (!string.IsNullOrEmpty(oba.Email) && oba.Email.Contains(billingEmail))) &&
                    (string.IsNullOrEmpty(billingLastName) || (!string.IsNullOrEmpty(oba.LastName) && oba.LastName.Contains(billingLastName)))                            
                select o;

            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));

            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));

            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));

            if (startTimeUtc.HasValue)
                query = query.Where(o => startTimeUtc.Value <= o.CreatedOnUtc);

            if (endTimeUtc.HasValue)
                query = query.Where(o => endTimeUtc.Value >= o.CreatedOnUtc);

            if (!string.IsNullOrEmpty(orderNotes))
                query = from o in query
                    join n in _orderNoteRepository.Table on o.Id equals n.OrderId
                    where n.Note.Contains(orderNotes)
                    select o;

            var item = (from oq in query
                group oq by 1
                into result
                select new
                {
                    OrderCount = result.Count(),
                    OrderShippingExclTaxSum = result.Sum(o => o.OrderShippingExclTax),
                    OrderPaymentFeeExclTaxSum = result.Sum(o => o.PaymentMethodAdditionalFeeExclTax),
                    OrderTaxSum = result.Sum(o => o.OrderTax),
                    OrderTotalSum = result.Sum(o => o.OrderTotal),
                    OrederRefundedAmountSum = result.Sum(o => o.RefundedAmount),
                }).Select(r => new OrderAverageReportLine
                    {
                        CountOrders = r.OrderCount,
                        SumShippingExclTax = r.OrderShippingExclTaxSum,
                        OrderPaymentFeeExclTaxSum = r.OrderPaymentFeeExclTaxSum,
                        SumTax = r.OrderTaxSum,
                        SumOrders = r.OrderTotalSum,
                        SumRefundedAmount = r.OrederRefundedAmountSum
                }).FirstOrDefault();

            item ??= new OrderAverageReportLine
            {
                CountOrders = 0,
                SumShippingExclTax = decimal.Zero,
                OrderPaymentFeeExclTaxSum = decimal.Zero,
                SumTax = decimal.Zero,
                SumOrders = decimal.Zero
            };
            return item;
        }

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Order status</param>
        /// <returns>Result</returns>
        public virtual OrderAverageReportLineSummary OrderAverageReport(int storeId, OrderStatus os)
        {
            var item = new OrderAverageReportLineSummary
            {
                OrderStatus = os
            };
            var orderStatuses = new List<int> { (int)os };

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;

            //today
            var t1 = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            DateTime? startTime1 = _dateTimeHelper.ConvertToUtcTime(t1, timeZone);
            var todayResult = GetOrderAverageReportLine(storeId,
                osIds: orderStatuses,
                startTimeUtc: startTime1);
            item.SumTodayOrders = todayResult.SumOrders;
            item.CountTodayOrders = todayResult.CountOrders;

            //week
            var fdow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var today = new DateTime(nowDt.Year, nowDt.Month, nowDt.Day);
            var t2 = today.AddDays(-(today.DayOfWeek - fdow));
            DateTime? startTime2 = _dateTimeHelper.ConvertToUtcTime(t2, timeZone);
            var weekResult = GetOrderAverageReportLine(storeId,
                osIds: orderStatuses,
                startTimeUtc: startTime2);
            item.SumThisWeekOrders = weekResult.SumOrders;
            item.CountThisWeekOrders = weekResult.CountOrders;

            //month
            var t3 = new DateTime(nowDt.Year, nowDt.Month, 1);
            DateTime? startTime3 = _dateTimeHelper.ConvertToUtcTime(t3, timeZone);
            var monthResult = GetOrderAverageReportLine(storeId,
                osIds: orderStatuses,
                startTimeUtc: startTime3);
            item.SumThisMonthOrders = monthResult.SumOrders;
            item.CountThisMonthOrders = monthResult.CountOrders;

            //year
            var t4 = new DateTime(nowDt.Year, 1, 1);
            DateTime? startTime4 = _dateTimeHelper.ConvertToUtcTime(t4, timeZone);
            var yearResult = GetOrderAverageReportLine(storeId,
                osIds: orderStatuses,
                startTimeUtc: startTime4);
            item.SumThisYearOrders = yearResult.SumOrders;
            item.CountThisYearOrders = yearResult.CountOrders;

            //all time
            var allTimeResult = GetOrderAverageReportLine(storeId, osIds: orderStatuses);
            item.SumAllTimeOrders = allTimeResult.SumOrders;
            item.CountAllTimeOrders = allTimeResult.CountOrders;

            return item;
        }

        /// <summary>
        /// Get best sellers report
        /// </summary>
        /// <param name="storeId">Store identifier (orders placed in a specific store); 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all records</param>
        /// <param name="orderBy">1 - order by quantity, 2 - order by total amount</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Result</returns>
        public virtual IPagedList<BestsellersReportLine> BestSellersReport(
            int categoryId = 0, int manufacturerId = 0,
            int storeId = 0, int vendorId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            OrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
            int billingCountryId = 0,
            OrderByEnum orderBy = OrderByEnum.OrderByQuantity,
            int pageIndex = 0, int pageSize = int.MaxValue,
            bool showHidden = false)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;

            var query1 = from orderItem in _orderItemRepository.Table
                         join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                         join p in _productRepository.Table on orderItem.ProductId equals p.Id
                         join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
                         join pc in _productCategoryRepository.Table on p.Id 
                             equals pc.ProductId 
                             into p_pc
                from pc in p_pc.DefaultIfEmpty()
                         join pm in _productManufacturerRepository.Table on p.Id 
                             equals pm.ProductId 
                             into p_pm
                from pm in p_pm.DefaultIfEmpty()
                         where (storeId == 0 || storeId == o.StoreId) &&
                               (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                               (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                               (o.OrderStatusId == (int)OrderStatus.Complete) &&
                               !o.Deleted &&
                               !p.Deleted &&
                               (showHidden || p.Published)
                         select orderItem;

            var query2 =
                //group by products
                from orderItem in query1
                group orderItem by orderItem.ProductId into g
                select new BestsellersReportLine
                {
                    ProductId = g.Key,
                    TotalAmount = g.Sum(x => x.PriceExclTax),
                    TotalQuantity = g.Sum(x => x.Quantity)
                };

            query2 = orderBy switch
            {
                OrderByEnum.OrderByQuantity => query2.OrderByDescending(x => x.TotalQuantity),
                OrderByEnum.OrderByTotalAmount => query2.OrderByDescending(x => x.TotalAmount),
                _ => throw new ArgumentException("Wrong orderBy parameter", nameof(orderBy)),
            };
            var result = new PagedList<BestsellersReportLine>(query2, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// Gets a list of products (identifiers) purchased by other customers who purchased a specified product
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="productId">Product identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only products marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
        public virtual int[] GetAlsoPurchasedProductsIds(int storeId, int productId,
            int recordsToReturn = 5, bool visibleIndividuallyOnly = true, bool showHidden = false)
        {
            if (productId == 0)
                throw new ArgumentException("Product ID is not specified");

            //this inner query should retrieve all orders that contains a specified product ID
            var query1 = from orderItem in _orderItemRepository.Table
                         where orderItem.ProductId == productId
                         select orderItem.OrderId;

            var query2 = from orderItem in _orderItemRepository.Table
                         join p in _productRepository.Table on orderItem.ProductId equals p.Id
                         join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                         where query1.Contains(orderItem.OrderId) &&
                         p.Id != productId &&
                         (showHidden || p.Published) &&
                         !o.Deleted &&
                         (storeId == 0 || o.StoreId == storeId) &&
                         !p.Deleted &&
                         (!visibleIndividuallyOnly || p.VisibleIndividually)
                         select new { orderItem, p };

            var query3 = from orderItem_p in query2
                         group orderItem_p by orderItem_p.p.Id into g
                         select new
                         {
                             ProductId = g.Key,
                             ProductsPurchased = g.Sum(x => x.orderItem.Quantity)
                         };
            query3 = query3.OrderByDescending(x => x.ProductsPurchased);

            if (recordsToReturn > 0)
                query3 = query3.Take(recordsToReturn);

            var report = query3.ToList();

            var ids = new List<int>();
            foreach (var reportLine in report)
                ids.Add(reportLine.ProductId);

            return ids.ToArray();
        }

        /// <summary>
        /// Gets a list of products that were never sold
        /// </summary>
        /// <param name="vendorId">Vendor identifier (filter products by a specific vendor); 0 to load all records</param>
        /// <param name="storeId">Store identifier (filter products by a specific store); 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Products</returns>
        public virtual IPagedList<Product> ProductsNeverSold(int vendorId = 0, int storeId = 0,
            int categoryId = 0, int manufacturerId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            //this inner query should retrieve all purchased product identifiers
            var query_tmp = (from orderItem in _orderItemRepository.Table
                             join o in _orderRepository.Table on orderItem.OrderId equals o.Id
                             where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                                   (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                                   !o.Deleted
                             select orderItem.ProductId).Distinct();

            var simpleProductTypeId = (int)ProductType.SimpleProduct;

            var query = from p in _productRepository.Table
                        join pm in _productManufacturerRepository.Table on p.Id 
                            equals pm.ProductId 
                            into p_pm
                from pm in p_pm.DefaultIfEmpty()
                        join pc in _productCategoryRepository.Table on p.Id 
                            equals pc.ProductId 
                            into p_pc
                from pc in p_pc.DefaultIfEmpty()
                        where !query_tmp.Contains(p.Id) &&
                              //include only simple products
                              p.ProductTypeId == simpleProductTypeId &&
                              !p.Deleted &&
                              (vendorId == 0 || p.VendorId == vendorId) &&
                              //(categoryId == 0 || p.ProductCategories.Count(pc => pc.CategoryId == categoryId) > 0) &&
                              //(manufacturerId == 0 || p.ProductManufacturers.Count(pm => pm.ManufacturerId == manufacturerId) > 0) &&
                              (manufacturerId == 0 || pm.ManufacturerId == manufacturerId) &&
                              (categoryId == 0 || pc.CategoryId == categoryId) &&
                              (showHidden || p.Published)
                        select p;

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = from p in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = p.Id, c2 = nameof(Product) } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into p_sm
                        from sm in p_sm.DefaultIfEmpty()
                        where !p.LimitedToStores || storeId == sm.StoreId
                        select p;
            }

            query = query.OrderBy(p => p.Name);

            var products = new PagedList<Product>(query, pageIndex, pageSize);
            return products;
        }

        /// <summary>
        /// Get profit report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to ignore this parameter</param>
        /// <param name="orderId">Order identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="osIds">Order status identifiers; null to load all records</param>
        /// <param name="psIds">Payment status identifiers; null to load all records</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all records</param>
        /// <param name="billingPhone">Billing phone. Leave empty to load all records.</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>Result</returns>
        public virtual decimal ProfitReport(int storeId = 0, int vendorId = 0, int productId = 0,
            int warehouseId = 0, int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingPhone = null, string billingEmail = null, string billingLastName = "", string orderNotes = null)
        {
            var dontSearchPhone = string.IsNullOrEmpty(billingPhone);
            var dontSearchEmail = string.IsNullOrEmpty(billingEmail);
            var dontSearchLastName = string.IsNullOrEmpty(billingLastName);
            var dontSearchOrderNotes = string.IsNullOrEmpty(orderNotes);
            var dontSearchPaymentMethods = string.IsNullOrEmpty(paymentMethodSystemName);

            var orders = _orderRepository.Table;
            if (osIds != null && osIds.Any())
                orders = orders.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                orders = orders.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                orders = orders.Where(o => ssIds.Contains(o.ShippingStatusId));

            var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;

            var query = from orderItem in _orderItemRepository.Table
                join o in orders on orderItem.OrderId equals o.Id
                join p in _productRepository.Table on orderItem.ProductId equals p.Id
                join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
                where (storeId == 0 || storeId == o.StoreId) &&
                      (orderId == 0 || orderId == o.Id) &&
                      (billingCountryId == 0 || (oba.CountryId == billingCountryId)) &&
                      (dontSearchPaymentMethods || paymentMethodSystemName == o.PaymentMethodSystemName) &&
                      (!startTimeUtc.HasValue || startTimeUtc.Value <= o.CreatedOnUtc) &&
                      (!endTimeUtc.HasValue || endTimeUtc.Value >= o.CreatedOnUtc) &&
                      !o.Deleted &&
                      (vendorId == 0 || p.VendorId == vendorId) &&
                      (productId == 0 || orderItem.ProductId == productId) &&
                      (warehouseId == 0 ||
                          //"Use multiple warehouses" enabled
                          //we search in each warehouse
                          p.ManageInventoryMethodId == manageStockInventoryMethodId &&
                          p.UseMultipleWarehouses &&
                          _productWarehouseInventoryRepository.Table.Any(pwi =>
                              pwi.ProductId == orderItem.ProductId && pwi.WarehouseId == warehouseId)
                          ||
                          //"Use multiple warehouses" disabled
                          //we use standard "warehouse" property
                          (p.ManageInventoryMethodId != manageStockInventoryMethodId ||
                           !p.UseMultipleWarehouses) &&
                          p.WarehouseId == warehouseId) &&
                      //we do not ignore deleted products when calculating order reports
                      //(!p.Deleted)
                      (dontSearchPhone || (!string.IsNullOrEmpty(oba.PhoneNumber) &&
                                           oba.PhoneNumber.Contains(billingPhone))) &&
                      (dontSearchEmail || (!string.IsNullOrEmpty(oba.Email) && oba.Email.Contains(billingEmail))) &&
                      (dontSearchLastName ||
                       (!string.IsNullOrEmpty(oba.LastName) && oba.LastName.Contains(billingLastName))) &&
                      (dontSearchOrderNotes || _orderNoteRepository.Table.Any(oNote =>
                           oNote.OrderId == o.Id && oNote.Note.Contains(orderNotes)))
                select orderItem;

            var productCost = Convert.ToDecimal(query.Sum(orderItem => (decimal?)orderItem.OriginalProductCost * orderItem.Quantity));

            var reportSummary = GetOrderAverageReportLine(
                storeId,
                vendorId,
                productId,
                warehouseId,
                billingCountryId,
                orderId,
                paymentMethodSystemName,
                osIds,
                psIds,
                ssIds,
                startTimeUtc,
                endTimeUtc,
                billingPhone,
                billingEmail,
                billingLastName,
                orderNotes);

            var profit = reportSummary.SumOrders
                         - reportSummary.SumShippingExclTax
                         - reportSummary.OrderPaymentFeeExclTaxSum
                         - reportSummary.SumTax
                         - reportSummary.SumRefundedAmount
                         - productCost;
            return profit;
        }

        #endregion
    }
}