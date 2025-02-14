﻿using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Caching;

namespace Nop.Plugin.NopStation.WebApi.Infrastructure.Cache
{
    public class ApiModelCacheDefaults 
    {
        /// <summary>
        /// Key for categories on the search page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string SearchCategoriesModelKey => "Nopstation.api.pres.search.categories-{0}-{1}-{2}";
        public static string SearchCategoriesPrefixCacheKey => "Nopstation.api.pres.search.categories";

        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static string ManufacturerNavigationModelKey => "Nopstation.api.pres.manufacturer.navigation-{0}-{1}-{2}-{3}";
        public static string ManufacturerNavigationPrefixCacheKey => "Nopstation.api.pres.manufacturer.navigation";

        /// <summary>
        /// Key for VendorNavigationModel caching
        /// </summary>
        public static string VendorNavigationModelKey => "Nopstation.api.pres.vendor.navigation";
        public static string VendorNavigationPrefixCacheKey => "Nopstation.api.pres.vendor.navigation";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured products
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string ManufacturerHasFeaturedProductsKey => "Nopstation.api.pres.manufacturer.hasfeaturedproducts-{0}-{1}-{2}";
        public static string ManufacturerHasFeaturedProductsPrefixCacheKeyById => "Nopstation.api.pres.manufacturer.hasfeaturedproducts-{0}-";

        /// <summary>
        /// Key for list of CategorySimpleModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public static string CategoryAllModelKey => "Nopstation.api.pres.category.all-{0}-{1}-{2}";
        public static string CategoryAllPrefixCacheKey => "Nopstation.api.pres.category.all";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of customer roles
        /// {1} : current store ID
        /// {2} : category ID
        /// </remarks>
        public static string CategoryNumberOfProductsModelKey => "Nopstation.api.pres.category.numberofproducts-{0}-{1}-{2}";
        public static string CategoryNumberOfProductsPrefixCacheKey => "Nopstation.api.pres.category.numberofproducts";

        /// <summary>
        /// Key for caching of a value indicating whether a category has featured products
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string CategoryHasFeaturedProductsKey => "Nopstation.api.pres.category.hasfeaturedproducts-{0}-{1}-{2}";
        public static string CategoryHasFeaturedProductsPrefixCacheKeyById => "Nopstation.api.pres.category.hasfeaturedproducts-{0}-";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static string CategoryBreadcrumbKey => "Nopstation.api.pres.category.breadcrumb-{0}-{1}-{2}-{3}";
        public static string CategoryBreadcrumbPrefixCacheKey => "Nopstation.api.pres.category.breadcrumb";

        /// <summary>
        /// Key for caching of subcategories of certain category
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// {4} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public static string CategorySubcategoriesKey => "Nopstation.api.pres.category.subcategories-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string CategorySubcategoriesPrefixCacheKey => "Nopstation.api.pres.category.subcategories";

        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : roles of the current user
        /// {1} : current store ID
        /// {2} : language ID
        /// {3} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public static string CategoryHomepageKey => "Nopstation.api.pres.category.homepage-{0}-{1}-{2}-{3}-{4}";
        public static string CategoryHomepagePrefixCacheKey => "Nopstation.api.pres.category.homepage";

        /// <summary>
        /// Key for Xml document of CategorySimpleModels caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CategoryXmlAllModelKey = "Nopstation.api.pres.categoryXml.all-{0}-{1}-{2}";
        public const string CategoryXmlAllPrefixCacheKey = "Nopstation.api.pres.categoryXml.all";

        /// <summary>
        /// Key for SpecificationAttributeOptionFilter caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of specification attribute option IDs
        /// {1} : language id
        /// </remarks>
        public static string SpecsFilterModelKey => "Nopstation.api.pres.filter.specs-{0}-{1}";
        public static string SpecsFilterPrefixCacheKey => "Nopstation.api.pres.filter.specs";

        /// <summary>
        /// Key for ProductBreadcrumbModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : comma separated list of customer roles
        /// {3} : current store ID
        /// </remarks>
        public static string ProductBreadcrumbModelKey => "Nopstation.api.pres.product.breadcrumb-{0}-{1}-{2}-{3}";
        public static string ProductBreadcrumbPrefixCacheKey => "Nopstation.api.pres.product.breadcrumb";
        public static string ProductBreadcrumbPrefixCacheKeyById => "Nopstation.api.pres.product.breadcrumb-{0}-";

        /// <summary>
        /// Key for ProductTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : comma separated list of customer roles
        /// {3} : current store ID
        /// </remarks>
        public static string ProductTagByProductModelKey => "Nopstation.api.pres.producttag.byproduct-{0}-{1}-{2}-{3}";
        public static string ProductTagByProductPrefixCacheKey => "Nopstation.api.pres.producttag.byproduct";

        /// <summary>
        /// Key for PopularProductTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductTagPopularModelKey => "Nopstation.api.pres.producttag.popular-{0}-{1}";
        public static string ProductTagPopularPrefixCacheKey => "Nopstation.api.pres.producttag.popular";

        /// <summary>
        /// Key for ProductManufacturers model caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static string ProductManufacturersModelKey => "Nopstation.api.pres.product.manufacturers-{0}-{1}-{2}-{3}";
        public static string ProductManufacturersPrefixCacheKey => "Nopstation.api.pres.product.manufacturers";
        public static string ProductManufacturersPrefixCacheKeyById => "Nopstation.api.pres.product.manufacturers-{0}-";

        /// <summary>
        /// Key for ProductSpecificationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// </remarks>
        public static string ProductSpecsModelKey => "Nopstation.api.pres.product.specs-{0}-{1}";
        public static string ProductSpecsPrefixCacheKey => "Nopstation.api.pres.product.specs";
        public static string ProductSpecsPrefixCacheKeyById => "Nopstation.api.pres.product.specs-{0}-";

        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public static string TopicModelBySystemNameKey => "Nopstation.api.pres.topic.details.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic id
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// {4} : show hidden records?
        /// </remarks>
        public static string TopicModelByIdKey => "Nopstation.api.pres.topic.details.byid-{0}-{1}-{2}-{3}-{4}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public static string TopicSenameBySystemName => "Nopstation.api.pres.topic.sename.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// {3} : comma separated list of customer roles
        /// </remarks>
        public static string TopicTitleBySystemName => "Nopstation.api.pres.topic.title.bysystemname-{0}-{1}-{2}-{3}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of customer roles
        /// </remarks>
        public static string TopicTopMenuModelKey => "Nopstation.api.pres.topic.topmenu-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// {2} : comma separated list of customer roles
        /// </remarks>
        public static string TopicFooterModelKey => "Nopstation.api.pres.topic.footer-{0}-{1}-{2}";
        public static string TopicPrefixCacheKey => "Nopstation.api.pres.topic";

        /// <summary>
        /// Key for CategoryTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : category template id
        /// </remarks>
        public static string CategoryTemplateModelKey => "Nopstation.api.pres.categorytemplate-{0}";
        public static string CategoryTemplatePrefixCacheKey => "Nopstation.api.pres.categorytemplate";

        /// <summary>
        /// Key for ManufacturerTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer template id
        /// </remarks>
        public static string ManufacturerTemplateModelKey => "Nopstation.api.pres.manufacturertemplate-{0}";
        public static string ManufacturerTemplatePrefixCacheKey => "Nopstation.api.pres.manufacturertemplate";

        /// <summary>
        /// Key for ProductTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : product template id
        /// </remarks>
        public static string ProductTemplateModelKey => "Nopstation.api.pres.producttemplate-{0}";
        public static string ProductTemplatePrefixCacheKey => "Nopstation.api.pres.producttemplate";

        /// <summary>
        /// Key for TopicTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : topic template id
        /// </remarks>
        public static string TopicTemplateModelKey => "Nopstation.api.pres.topictemplate-{0}";
        public static string TopicTemplatePrefixCacheKey => "Nopstation.api.pres.topictemplate";

        /// <summary>
        /// Key for bestsellers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public static CacheKey HomepageBestsellersIdsKey => new CacheKey("Nopstation.api.pres.bestsellers.homepage-{0}", HomepageBestsellersIdsPrefixCacheKey);
        public static string HomepageBestsellersIdsPrefixCacheKey => "Nopstation.api.pres.bestsellers.homepage";

        /// <summary>
        /// Key for "also purchased" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductsAlsoPurchasedIdsKey => "Nopstation.api.pres.alsopuchased-{0}-{1}";
        public static string ProductsAlsoPurchasedIdsPrefixCacheKey => "Nopstation.api.pres.alsopuchased";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public static CacheKey ProductsRelatedIdsKey => new CacheKey("Nopstation.api.pres.related-{0}-{1}",ProductsRelatedIdsPrefixCacheKey);
        public static string ProductsRelatedIdsPrefixCacheKey => "Nopstation.api.pres.related";

        /// <summary>
        /// Key for default product picture caching (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : isAssociatedProduct?
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static string ProductDefaultPictureModelKey => "Nopstation.api.pres.product.detailspictures-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string ProductDefaultPicturePrefixCacheKey => "Nopstation.api.pres.product.detailspictures";
        public static string ProductDefaultPicturePrefixCacheKeyById => "Nopstation.api.pres.product.detailspictures-{0}-";

        /// <summary>
        /// Key for product picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static string ProductDetailsPicturesModelKey => "Nopstation.api.pres.product.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string ProductDetailsPicturesPrefixCacheKey => "Nopstation.api.pres.product.picture";
        public static string ProductDetailsPicturesPrefixCacheKeyById => "Nopstation.api.pres.product.picture-{0}-";

        /// <summary>
        /// Key for product reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : current store ID
        /// </remarks>
        public static string ProductReviewsModelKey => "Nopstation.api.pres.product.reviews-{0}-{1}";
        public static string ProductReviewsPrefixCacheKey => "Nopstation.api.pres.product.reviews";
        public static string ProductReviewsPrefixCacheKeyById => "Nopstation.api.pres.product.reviews-{0}-";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static string ProductAttributePictureModelKey => "Nopstation.api.pres.productattribute.picture-{0}-{1}-{2}";
        public static string ProductAttributePicturePrefixCacheKey => "Nopstation.api.pres.productattribute.picture";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public static string ProductAttributeImageSquarePictureModelKey => "Nopstation.api.pres.productattribute.imagesquare.picture-{0}-{1}-{2}";
        public static string ProductAttributeImageSquarePicturePrefixCacheKey => "Nopstation.api.pres.productattribute.imagesquare.picture";

        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey CategoryPictureModelKey => new CacheKey ("Nopstation.api.pres.category.picture-{0}-{1}-{2}-{3}-{4}-{5}", CategoryPicturePrefixCacheKey, CategoryPicturePrefixCacheKeyById);
        public static string CategoryPicturePrefixCacheKey => "Nopstation.api.pres.category.picture";
        public static string CategoryPicturePrefixCacheKeyById => "Nopstation.api.pres.category.picture-{0}-";

        /// <summary>
        /// Key for manufacturer picture caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized manufacturer name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerPictureModelKey => new CacheKey ("Nopstation.api.pres.manufacturer.picture-{0}-{1}-{2}-{3}-{4}-{5}", ManufacturerPicturePrefixCacheKey, ManufacturerPicturePrefixCacheKeyById);
        public static string ManufacturerPicturePrefixCacheKey => "Nopstation.api.pres.manufacturer.picture";
        public static string ManufacturerPicturePrefixCacheKeyById => "Nopstation.api.pres.manufacturer.picture-{0}-";

        /// <summary>
        /// Key for vendor picture caching
        /// </summary>
        /// <remarks>
        /// {0} : vendor id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static string VendorPictureModelKey => "Nopstation.api.pres.vendor.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string VendorPicturePrefixCacheKey => "Nopstation.api.pres.vendor.picture";
        public static string VendorPicturePrefixCacheKeyById => "Nopstation.api.pres.vendor.picture-{0}-";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : shopping cart item id
        /// P.S. we could cache by product ID. it could increase performance.
        /// but it won't work for product attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static string CartPictureModelKey => "Nopstation.api.pres.cart.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public static string CartPicturePrefixCacheKey => "Nopstation.api.pres.cart.picture";

        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string HomepagePollsModelKey => "Nopstation.api.pres.poll.homepage-{0}-{1}";
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// {2} : current store ID
        /// </remarks>
        public static string PollBySystemNameModelKey => "Nopstation.api.pres.poll.systemname-{0}-{1}-{2}";
        public static string PollsPrefixCacheKey => "Nopstation.api.pres.poll";

        /// <summary>
        /// Key for blog tag list model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string BlogTagsModelKey => "Nopstation.api.pres.blog.tags-{0}-{1}";
        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string BlogMonthsModelKey => "Nopstation.api.pres.blog.months-{0}-{1}";
        public static string BlogPrefixCacheKey => "Nopstation.api.pres.blog";
        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static string BlogCommentsNumberKey => "Nopstation.api.pres.blog.comments.number-{0}-{1}-{2}";
        public static string BlogCommentsPrefixCacheKey => "Nopstation.api.pres.blog.comments";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string HomepageNewsModelKey => "Nopstation.api.pres.news.homepage-{0}-{1}";
        public static string NewsPrefixCacheKey => "Nopstation.api.pres.news";
        /// <summary>
        /// Key for number of news comments
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static string NewsCommentsNumberKey => "Nopstation.api.pres.news.comments.number-{0}-{1}-{2}";
        public static string NewsCommentsPrefixCacheKey => "Nopstation.api.pres.news.comments";

        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {1} : "empty" or "select" item
        /// {2} : language ID
        /// </remarks>
        public static string StateProvincesByCountryModelKey => "Nopstation.api.pres.stateprovinces.bycountry-{0}-{1}-{2}";
        public static string StateProvincesPrefixCacheKey => "Nopstation.api.pres.stateprovinces";

        /// <summary>
        /// Key for return request reasons
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string ReturnRequestReasonsModelKey => "Nopstation.api.pres.returnrequesreasons-{0}";
        public static string ReturnRequestReasonsPrefixCacheKey => "Nopstation.api.pres.returnrequesreasons";

        /// <summary>
        /// Key for return request actions
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string ReturnRequestActionsModelKey => "Nopstation.api.pres.returnrequestactions-{0}";
        public static string ReturnRequestActionsPrefixCacheKey => "Nopstation.api.pres.returnrequestactions";

        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : current theme
        /// {2} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public static string StoreLogoPath => "Nopstation.api.pres.logo-{0}-{1}-{2}";
        public static string StoreLogoPathPrefixCacheKey => "Nopstation.api.pres.logo";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public static string AvailableLanguagesModelKey => "Nopstation.api.pres.languages.all-{0}";
        public static string AvailableLanguagesPrefixCacheKey => "Nopstation.api.pres.languages";

        /// <summary>
        /// Key for available currencies
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string AvailableCurrenciesModelKey => "Nopstation.api.pres.currencies.all-{0}-{1}";
        public static string AvailableCurrenciesPrefixCacheKey => "Nopstation.api.pres.currencies";

        /// <summary>
        /// Key for caching of a value indicating whether we have checkout attributes
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : true - all attributes, false - only shippable attributes
        /// </remarks>
        public static string CheckoutAttributesExistKey => "Nopstation.api.pres.checkoutattributes.exist-{0}-{1}";
        public static string CheckoutAttributesPrefixCacheKey => "Nopstation.api.pres.checkoutattributes";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static string SitemapPageModelKey => "Nopstation.api.pres.sitemap.page-{0}-{1}-{2}";
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : sitemap identifier
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public static string SitemapSeoModelKey => "Nopstation.api.pres.sitemap.seo-{0}-{1}-{2}-{3}";
        public static string SitemapPrefixCacheKey => "Nopstation.api.pres.sitemap";

        /// <summary>
        /// Key for widget info
        /// </summary>
        /// <remarks>
        /// {0} : current customer ID
        /// {1} : current store ID
        /// {2} : widget zone
        /// {3} : current theme name
        /// </remarks>
        public static string WidgetModelKey => "Nopstation.api.pres.widget-{0}-{1}-{2}-{3}";
        public static string WidgetPrefixCacheKey => "Nopstation.api.pres.widget";
        public static string StringResourceKey => "Nopstation.api.stringresource-{0}";
        public static string StringResourcePrefixCacheKey => "Nopstation.api.stringresource";

        public static string SliderModelKey => "Nopstation.api.slider-{0}-{1}-{2}-{3}";
        public static string SliderPrefixCacheKey => "Nopstation.api.slider";
    }
}
