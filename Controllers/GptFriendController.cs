
using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;
using Nop.Plugin.Misc.GPTFriend.Services;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Services.Localization;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.EMMA;

namespace Nop.Plugin.Misc.GPTFriend.Controllers
{
	[AutoValidateAntiforgeryToken]
	public class GPTFriendController : BasePluginController
	{

		#region Fields
		private readonly GptFriendService _service;
		private readonly IStoreContext _storeContext;
		private readonly ISettingService _settingService;
		private readonly INotificationService _notificationService;
		private readonly ILocalizationService _localizationService;
		#endregion

		#region Ctor

		public GPTFriendController(GptFriendService service, IStoreContext storeContext, ISettingService settingService, INotificationService notificationService, ILocalizationService localizationService)
		{
			_service = service;
			_storeContext = storeContext;
			_settingService = settingService;
			_notificationService = notificationService;
			_localizationService = localizationService;
		}
		#endregion

		#region Methods

		[AuthorizeAdmin]
		[Area(AreaNames.Admin)]
		public async Task<IActionResult> SetGPTDescriptions()
        {
			var storeId = _storeContext.GetActiveStoreScopeConfigurationAsync().Result;
			GPTFriendSettings model = _settingService.LoadSettingAsync<GPTFriendSettings>(storeId).Result;
			await _service.FillAllProducts(model);

			return await Configure();
		}

		[AuthorizeAdmin]
		[Area(AreaNames.Admin)]
		public IActionResult FeedDetail(string FeedId)
		{
			string html = "";
			return Content(html);
		}

		[AuthorizeAdmin]
		[Area(AreaNames.Admin)]
		public async Task<IActionResult> SetGPTBrandDescriptions()
		{
			var storeId = _storeContext.GetActiveStoreScopeConfigurationAsync().Result;
			GPTFriendSettings model = _settingService.LoadSettingAsync<GPTFriendSettings>(storeId).Result;
			await _service.FillManufacturers(model, true);

			return await Configure();
		}

		[AuthorizeAdmin]
		[Area(AreaNames.Admin)]
		public async Task<IActionResult> SetGPTFamilyDescriptions()
		{
			var storeId = _storeContext.GetActiveStoreScopeConfigurationAsync().Result;
			GPTFriendSettings model = _settingService.LoadSettingAsync<GPTFriendSettings>(storeId).Result;
			await _service.FillAllFamilys(model);

			return await Configure();
		}
		

		[AuthorizeAdmin]
        [Area(AreaNames.Admin)]
		public async Task<IActionResult> Configure()
        {
			var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
			GPTFriendSettings model = await _settingService.LoadSettingAsync<GPTFriendSettings>(storeId);
			model.GptModels = new SelectListItem[]
			{
				new SelectListItem(){Selected = model.GptModel == "gpt-3.5-turbo-0125", Text  = "gpt-3.5-turbo-0125", Value = "gpt-3.5-turbo-0125"},
				new SelectListItem(){Selected = model.GptModel == "gpt-4", Text  = "gpt-4", Value = "gpt-4"},
                new SelectListItem(){Selected = model.GptModel == "gpt-4-turbo", Text  = "gpt-4-turbo", Value = "gpt-4-turbo"},
                new SelectListItem(){Selected = model.GptModel == "gpt-4o", Text  = "gpt-4o", Value = "gpt-4o"},
                new SelectListItem(){Selected = model.GptModel == "gpt-4o-mini", Text  = "gpt-4o-mini", Value = "gpt-4o-mini"},
                new SelectListItem(){Selected = model.GptModel == "o1-preview", Text  = "o1-preview", Value = "o1-preview"}
            }; 

            return View("~/Plugins/Misc.GPTFriend/Views/Configure.cshtml", model);
		}

        public async Task<IActionResult> Chat()
		{
            return View("~/Plugins/Misc.GPTFriend/Views/UserChat.cshtml");
		}

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
		public async Task<IActionResult> Configure(GPTFriendSettings model)
        {
            if (!ModelState.IsValid)
                return await Configure();


			var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
			await _settingService.SaveSettingAsync(model, storeId);
			await _settingService.ClearCacheAsync();

			_notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

			return await Configure();
        }

        #endregion
    }
}