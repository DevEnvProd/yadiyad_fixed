﻿using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Home;

namespace YadiYad.Pro.Web.Models.Customer
{
    public partial class ContactUsProModel : BaseNopModel
    {
        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("ContactUs.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("ContactUs.Subject")]
        public string SubjectSelect { get; set; }
        [NopResourceDisplayName("ContactUs.Subject")]
        public string SubjectOther { get; set; }
        public int SubjectId { get; set; }

        public bool SubjectEnabled { get; set; }

        [NopResourceDisplayName("ContactUs.Enquiry")]
        public string Enquiry { get; set; }

        [NopResourceDisplayName("ContactUs.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
        public ContactUsType Type { get; set; }
    }
}
