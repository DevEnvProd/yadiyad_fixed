﻿using System;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using Nop.Web.Models.Media;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request model
    /// </summary>
    public partial class ReturnRequestModel : BaseNopEntityModel
    {
        public ReturnRequestModel()
        {
            ProductPictures = new List<PictureModel>();
        }
        #region Properties

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.CustomNumber")]
        public string CustomNumber { get; set; }
        
        public int OrderId { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Customer")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Customer")]
        public string CustomerInfo { get; set; }

        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Product")]
        public string ProductName { get; set; }

        public string AttributeInfo { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Quantity")]
        public int Quantity { get; set; }
        
        [NopResourceDisplayName("Admin.ReturnRequests.Fields.ReasonForReturn")]
        public string ReasonForReturn { get; set; }
        
        [NopResourceDisplayName("Admin.ReturnRequests.Fields.RequestedAction")]
        public string RequestedAction { get; set; }
        
        [NopResourceDisplayName("Admin.ReturnRequests.Fields.CustomerComments")]
        public string CustomerComments { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.UploadedFile")]
        public Guid UploadedFileGuid { get; set; }
        
        [NopResourceDisplayName("Admin.ReturnRequests.Fields.StaffNotes")]
        public string StaffNotes { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Status")]
        public int ReturnRequestStatusId { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.Status")]
        public string ReturnRequestStatusStr { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.ReturnStatus")]
        public int ReturnStatus { get; set; }
        public string ReturnStatusText { get; set; }

        [NopResourceDisplayName("Admin.ReturnRequests.Fields.ActionAfterReturn")]
        public int ActionAfterReturn { get; set; }
        public int GroupReturnRequestId { get; set; }
        public IList<ReturnRequestImage> PictureIds { get; set; }
        public IList<PictureModel> ProductPictures { get; set; }
        public decimal UnitPrice { get; set; }
        #endregion
    }
}