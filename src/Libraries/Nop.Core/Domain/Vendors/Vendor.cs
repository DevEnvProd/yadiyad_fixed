﻿using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ShippingShuq;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Represents a vendor
    /// </summary>
    public partial class Vendor : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active
        /// </summary>
        public bool Active { get; set; }
        public bool Online { get; set; }
        public DateTime? OfflineUntil { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        public string PageSizeOptions { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the category
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the courierId
        /// </summary>
        public int CourierId { get; set; }

        #endregion
        
        #region Custom properties

        /// <summary>
        /// Gets or sets the courier
        /// </summary>
        public ShippingCourierEnum.Courier Courier
        {
            get => (ShippingCourierEnum.Courier)CourierId;
            set => CourierId = (int)value;
        }

        #endregion
    }
}
