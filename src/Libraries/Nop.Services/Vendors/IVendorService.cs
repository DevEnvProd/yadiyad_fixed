﻿using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using YadiYad.Pro.Core.Domain.Common;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Vendor service interface
    /// </summary>
    public partial interface IVendorService
    {
        /// <summary>
        /// Gets a vendor by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor</returns>
        Vendor GetVendorById(int vendorId);

        /// <summary>
        /// Gets a vendors by product identifiers
        /// </summary>
        /// <param name="productIds">Array of product identifiers</param>
        /// <returns>Vendors</returns>
        IList<Vendor> GetVendorsByProductIds(int[] productIds);

        /// <summary>
        /// Gets a vendor by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Vendor</returns>
        Vendor GetVendorByProductId(int productId);

        /// <summary>
        /// Delete a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void DeleteVendor(Vendor vendor);

        /// <summary>
        /// Gets all vendors
        /// </summary>
        /// <param name="name">Vendor name</param>
        /// <param name="email">Vendor email</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendors</returns>
        IPagedList<Vendor> GetAllVendors(string name = "", string email = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets vendors
        /// </summary>
        /// <param name="vendorIds">Vendor identifiers</param>
        /// <returns>Vendors</returns>
        IList<Vendor> GetVendorsByIds(int[] vendorIds);

        /// <summary>
        /// Inserts a vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void InsertVendor(Vendor vendor);

        /// <summary>
        /// Updates the vendor
        /// </summary>
        /// <param name="vendor">Vendor</param>
        void UpdateVendor(Vendor vendor);

        /// <summary>
        /// Gets a vendor note
        /// </summary>
        /// <param name="vendorNoteId">The vendor note identifier</param>
        /// <returns>Vendor note</returns>
        VendorNote GetVendorNoteById(int vendorNoteId);

        /// <summary>
        /// Gets all vendor notes
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Vendor notes</returns>
        IPagedList<VendorNote> GetVendorNotesByVendor(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Deletes a vendor note
        /// </summary>
        /// <param name="vendorNote">The vendor note</param>
        void DeleteVendorNote(VendorNote vendorNote);

        /// <summary>
        /// Inserts a vendor note
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        void InsertVendorNote(VendorNote vendorNote);

        /// <summary>
        /// Formats the vendor note text
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Formatted text</returns>
        string FormatVendorNoteText(VendorNote vendorNote);

        /// <summary>
        /// Gets a vendor picture
        /// </summary>
        /// <param name="vendorPictureId">The vendor picture identifier</param>
        /// <returns>Vendor picture</returns>
        VendorPicture GetVendorPictureById(int vendorPictureId);

        /// <summary>
        /// Gets all vendor pictures
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Vendor pictures</returns>
        IPagedList<VendorPicture> GetVendorPicturesByVendorId(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Deletes a vendor picture
        /// </summary>
        /// <param name="vendorPicture">The vendor picture</param>
        void DeleteVendorPicture(VendorPicture vendorPicture);

        /// <summary>
        /// Inserts a vendor picture
        /// </summary>
        /// <param name="vendorPicture">Vendor picture</param>
        void InsertVendorPicture(VendorPicture vendorPicture);

        /// <summary>
        /// Updates a vendor picture
        /// </summary>
        /// <param name="vendorPicture">Vendor picture</param>
        void UpdateVendorPicture(VendorPicture vendorPicture);

        VendorAttributeValue GetVendorAttributeValue(Vendor vendor, string attributeName);
        IList<VendorAttributeValue> GetVendorAttributes(Vendor vendor);

        Vendor GetVendorByOrderId(int orderId);
        int GetVendorCustomerIdByOrderId(int orderId);
        bool GetVendorOnlineStatus(Vendor vendor);
        decimal GetVendorMaxShipmentWeight(string categoryVendorName, bool deliverByCar);
        int GetCustomerIdByVendor(Vendor vendor);
    }
}