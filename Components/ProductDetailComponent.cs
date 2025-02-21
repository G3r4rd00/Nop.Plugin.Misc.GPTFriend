using System;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Plugin.Misc.GPTFriend;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Shipping.EasyPost.Components
{
    /// <summary>
    /// Represents view component to render an additional block on the product details page in the admin area
    /// </summary>
    [ViewComponent(Name = GPTFriendDefaults.ProductDetailComponent)]
    public class ProductDetailComponent : NopViewComponent
    {
        #region Ctor

        public ProductDetailComponent()
        {
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
			return await Task.FromResult(new HtmlContentViewComponentResult(new HtmlString(widgetZone)));
			//return View("~/Plugins/Misc.GPTFriend/Views/ProductDetailButton.cshtml");
		}

        #endregion
    }
}