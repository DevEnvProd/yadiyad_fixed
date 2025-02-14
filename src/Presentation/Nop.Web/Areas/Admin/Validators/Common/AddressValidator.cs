﻿using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Common
{
    public partial class AddressValidator : BaseNopValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.FirstName.Required"))
                .When(x => x.FirstNameEnabled && x.FirstNameRequired);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.LastName.Required"))
                .When(x => x.LastNameEnabled && x.LastNameRequired);
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Email.Required"))
                .When(x => x.EmailEnabled && x.EmailRequired);
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"))
                .When(x => x.EmailEnabled && x.EmailRequired);
            RuleFor(x => x.Company)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Company.Required"))
                .When(x => x.CompanyEnabled && x.CompanyRequired);
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Country.Required"))
                .When(x => x.CountryEnabled && x.CountryRequired);
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Country.Required"))
                .When(x => x.CountryEnabled && x.CountryRequired);
            RuleFor(x => x.County)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.County.Required"))
                .When(x => x.CountyEnabled && x.CountyRequired);
            RuleFor(x => x.StateProvinceId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.State.Required"))
                .When(x => x.StateProvinceEnabled);
            RuleFor(x => x.StateProvinceId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.State.Required"))
                .When(x => x.StateProvinceEnabled);
            RuleFor(x => x.County)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.County.Required"))
                .When(x => x.CountyEnabled && x.CountyRequired);
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.City.Required"))
                .When(x => x.CityEnabled && x.CityRequired);
            RuleFor(x => x.Address1)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Address1.Required"))
                .When(x => x.StreetAddressEnabled && x.StreetAddressRequired);
            RuleFor(x => x.Address2)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Address2.Required"))
                .When(x => x.StreetAddress2Enabled && x.StreetAddress2Required);
            RuleFor(x => x.ZipPostalCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.ZipPostalCode.Required"))
                .When(x => x.ZipPostalCodeEnabled && x.ZipPostalCodeRequired);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.PhoneNumber.Required"))
                .When(x => x.PhoneEnabled && x.PhoneRequired);
            RuleFor(x => x.FaxNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.FaxNumber.Required"))
                .When(x => x.FaxEnabled && x.FaxRequired);
            RuleFor(x => x.Longitude)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Longitude.Required"))
                .When(x => x.GeolocationEnabled && x.GeolocationRequired);
            RuleFor(x => x.Latitude)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Latitude.Required"))
                .When(x => x.GeolocationEnabled && x.GeolocationRequired);

            SetDatabaseValidationRules<Address>(dataProvider);
        }
    }
}