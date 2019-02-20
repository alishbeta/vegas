﻿using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Validators.Shipping;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a warehouse model
    /// </summary>
    [Validator(typeof(WarehouseValidator))]
    public partial class WarehouseModel : BaseNopEntityModel
    {
        #region Ctor

        public WarehouseModel()
        {
            this.Address = new AddressModel();
            this.Pictures = new List<PictureIdModel>();
            this.AddPictureModel = new PictureIdModel();
        }

        #endregion
        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Picture.Number")]
        public List<PictureIdModel> Pictures { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Picture.Number")]
        public PictureIdModel AddPictureModel { get; set; }

        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Hidden")]
        public bool Hidden { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.WarehouseDescription")]
        public string WarehouseDescription { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Address")]
        public AddressModel Address { get; set; }

        #endregion

        public partial class PictureIdModel
        {
            public PictureIdModel()
            {
                this.PictureId = 0;
            }
            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Picture.Id")]
            public int? PictureId { get; set; }
        }
    }

}