﻿using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.Core.Models
{
    public class CoreLocaleResourceModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.NopStation.Core.Resources.Fields.Name")]
        public string ResourceName { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Core.Resources.Fields.Value")]
        public string ResourceValue { get; set; }

        public string ResourceNameLanguageId { get; set; }

        public int LanguageId { get; set; }

        #endregion
    }
}