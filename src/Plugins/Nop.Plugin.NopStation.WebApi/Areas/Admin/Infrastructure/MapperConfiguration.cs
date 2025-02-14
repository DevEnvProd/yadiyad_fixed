﻿using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Infrastructure
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        #region Ctor

        public MapperConfiguration()
        {
            CreateMap<WebApiSettings, ConfigurationModel>()
                .ForMember(model => model.CheckIat_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DefaultCategoryIcon_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IOSProductPriceTextSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IonicProductPriceTextSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AndroidProductPriceTextSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowHomepageSlider_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SliderAutoPlay_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SliderAutoPlayTimeout_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EnableCORS_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EnableJwtSecurity_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.MaximumNumberOfHomePageSliders_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfHomepageCategoryProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NumberOfManufacturers_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowBestsellersOnHomepage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowFeaturedProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowHomepageCategoryProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowManufacturers_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSubCategoryProducts_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TokenKey_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TokenSecondsValid_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TokenSecret_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AndriodForceUpdate_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AndroidVersion_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PlayStoreUrl_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IOSForceUpdate_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.IOSVersion_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.AppStoreUrl_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LogoId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LogoSize_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowChangeBaseUrlPanel_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.PrimaryThemeColor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BottomBarActiveColor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BottomBarBackgroundColor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BottomBarInactiveColor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TopBarBackgroundColor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.TopBarTextColor_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ActiveStoreScopeConfiguration, options => options.Ignore());
            CreateMap<ConfigurationModel, WebApiSettings>();

            CreateMap<ApiSlider, SliderModel>()
                .ForMember(model => model.ActiveEndDate, options => options.Ignore())
                .ForMember(model => model.ActiveStartDate, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.SliderTypeStr, options => options.Ignore())
                .ForMember(model => model.AvailableSliderTypes, options => options.Ignore());
            CreateMap<SliderModel, ApiSlider>()
                .ForMember(entity => entity.ActiveEndDateUtc, options => options.Ignore())
                .ForMember(entity => entity.ActiveStartDateUtc, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

            CreateMap<ApiDevice, DeviceModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.UpdatedOn, options => options.Ignore())
                .ForMember(model => model.DeviceTypeStr, options => options.Ignore());
            CreateMap<DeviceModel, ApiDevice>()
                .ForMember(entity => entity.DeviceType, options => options.Ignore())
                .ForMember(entity => entity.UpdatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());

            CreateMap<ApiCategoryIcon, CategoryIconModel>()
                .ForMember(model => model.IconId, options => options.MapFrom(s => s.PictureId))
                .ForMember(model => model.CategoryName, options => options.Ignore())
                .ForMember(model => model.PictureUrl, options => options.Ignore())
                .ForMember(model => model.CategoryBannerUrl, options => options.Ignore());
            CreateMap<CategoryIconModel, ApiCategoryIcon>()
                .ForMember(model => model.PictureId, options => options.MapFrom(s => s.IconId));
        }

        #endregion

        #region Properties

        public int Order => 0;

        #endregion
    }
}