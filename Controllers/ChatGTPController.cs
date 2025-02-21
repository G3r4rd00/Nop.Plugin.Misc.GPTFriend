using Microsoft.AspNetCore.Mvc;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.GPTFriend.Controllers
{
    public class ChatGPTController : BasePluginController
	{
		private readonly ILogger _logger;


		public ChatGPTController(ILogger logger)
		{
			_logger = logger;
		}

		[Route("ChatGTP/Assistant")]
		[Route("ChatGPT/Assistant")]
		public async Task<IActionResult> Assistant()
		{
			return await Task.FromResult(View("~/Plugins/Misc.GPTFriend/Views/UserChat.cshtml"));
		}
	}
}
