

using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

using Nop.Web.Framework.Components;
using Nop.Services.Logging;


namespace Nop.Plugin.Misc.GPTFriend.Components
{
	[ViewComponent(Name = GPTFriendDefaults.VIEW_COMPONENT_NAME)]
	/// <summary>
	/// Represents the view component to display logo
	/// </summary>
	public class ChatViewComponent : NopViewComponent
    {
        #region Fields
        
		
		private readonly ILogger _logger;

		#endregion

		#region Ctor

		public ChatViewComponent(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

     
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            return await Task.FromResult(View("~/Plugins/Misc.GPTFriend/Views/ButtonFooter.cshtml"));
		}

        #endregion
    }
}