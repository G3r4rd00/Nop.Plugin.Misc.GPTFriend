
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Plugin.Misc.GPTFriend.Components;

namespace Nop.Plugin.Misc.GPTFriend
{   
    public class GPTFriend : BasePlugin, IMiscPlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly WidgetSettings _widgetSettings;
        private readonly ISettingService _settingService;

        public bool HideInWidgetList => true;

        public GPTFriend(  IWebHelper webHelper, ILocalizationService localizationService, WidgetSettings widgetSettings, ISettingService settingService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _widgetSettings = widgetSettings;
            _settingService = settingService;
        }


		public string GetWidgetViewComponentName(string widgetZone)
		{
			return  GPTFriendDefaults.ProductDetailComponent;
		}

		public Type GetWidgetViewComponent(string widgetZone)
		{
			if (widgetZone == null)
				throw new ArgumentNullException(nameof(widgetZone));

			return typeof(ChatViewComponent);
		}

		public async Task<IList<string>> GetWidgetZonesAsync()
		{
            return new List<string> { "footer" };
		}


        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/GPTFriend/Configure";
        }

        public override async Task InstallAsync()
        {
            //locales
            //await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            //{
            //    ["Plugins.Misc.GPTFriend.Fields.PhoneNumber"] = "Phone number"
            //});

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(GPTFriendDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(GPTFriendDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

		
	}
}