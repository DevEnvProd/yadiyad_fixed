#pragma checksum "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\NopStationCore\PluginInfo.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b09b041bdc381a0220fdb4eaa3a01b81a443cfec"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_NopStationCore_PluginInfo), @"mvc.1.0.view", @"/Views/NopStationCore/PluginInfo.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
#nullable restore
#line 6 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Mvc.ViewFeatures;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Web.Framework.UI;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Web.Framework.Extensions;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using System.Text.Encodings.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Services.Events;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Web.Framework.Events;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Web.Framework.Infrastructure;

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Plugin.NopStation.Core.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Core.Domain.Common;

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Web.Framework.Models.DataTables;

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Mvc.Rendering;

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\_ViewImports.cshtml"
using Nop.Core.Infrastructure;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b09b041bdc381a0220fdb4eaa3a01b81a443cfec", @"/Views/NopStationCore/PluginInfo.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9223c6fc2cefdf91f719f82910606de79df8ff65", @"/Views/_ViewImports.cshtml")]
    public class Views_NopStationCore_PluginInfo : Nop.Web.Framework.Mvc.Razor.NopRazorPage<IEnumerable<PluginInfoModel>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
#nullable restore
#line 3 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\NopStationCore\PluginInfo.cshtml"
  
    Layout = "_AdminLayout";

    //page title
    ViewBag.PageTitle = T("Admin.NopStation.Core.PluginInfo").Text;
    //active menu item (system name)
    Html.SetActiveMenuItemSystemName("NopStationCore.PluginInfo");

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<div class=\"content-header clearfix\">\n    <h1 class=\"pull-left\">\n        ");
#nullable restore
#line 14 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\NopStationCore\PluginInfo.cshtml"
   Write(T("Admin.NopStation.Core.PluginInfo"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    </h1>\n</div>\n\n<div class=\"content\">\n    <div class=\"form-horizontal\">\n        <div class=\"panel-group\">\n            <div class=\"panel panel-default\">\n                <div class=\"panel-body\">\n");
#nullable restore
#line 23 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\NopStationCore\PluginInfo.cshtml"
                     foreach (var item in Model)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <span>");
#nullable restore
#line 25 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\NopStationCore\PluginInfo.cshtml"
                         Write(item.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral(", Version=");
#nullable restore
#line 25 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\NopStationCore\PluginInfo.cshtml"
                                              Write(item.Version);

#line default
#line hidden
#nullable disable
            WriteLiteral(" (");
#nullable restore
#line 25 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\NopStationCore\PluginInfo.cshtml"
                                                              Write(item.FileName);

#line default
#line hidden
#nullable disable
            WriteLiteral(")</span><br />\n");
#nullable restore
#line 26 "C:\Users\chong\Downloads\Yadiyad\src\Plugins\Nop.Plugin.NopStation.Core\Views\NopStationCore\PluginInfo.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </div>\n            </div>\n        </div>\n    </div>\n</div>\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Nop.Core.IWorkContext workContext { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Nop.Services.Common.IGenericAttributeService genericAttributeService { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<PluginInfoModel>> Html { get; private set; }
    }
}
#pragma warning restore 1591
