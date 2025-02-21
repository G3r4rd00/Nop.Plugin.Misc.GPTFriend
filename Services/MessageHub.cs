



//using Microsoft.AspNetCore.Mvc.Infrastructure;
//using Microsoft.AspNetCore.Mvc.Routing;
//using Microsoft.AspNetCore.SignalR;
//using Newtonsoft.Json;
//using Nop.Core;
//using Nop.Data;
//using Nop.Plugin.Misc.GPTFriend.Domain;
//using Nop.Plugin.Misc.GPTFriend.Services.GPTAssistant.Models;
//using Nop.Services.Catalog;
//using Nop.Services.Logging;
//using Nop.Services.Media;
//using Nop.Services.Seo;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Nop.Plugin.Misc.GPTFriend.Services
//{
    
//    public class MessageHub : Hub
//    {
//		private readonly ILogger _logger;
//        private readonly IRepository<GPTFriendChatMessage> _gptFriendChatMessageRepository;
//        private readonly IHubContext<MessageHub> _messageHubContext;
//        private readonly IProductService _productService;
//        private readonly IPictureService _pictureService;
//		private readonly IUrlRecordService _urlRecordService;
//		private readonly IUrlHelperFactory _urlHelperFactory;
//		private readonly IActionContextAccessor _actionContextAccessor;
//		private readonly IStoreContext _storeContext;

//		private static Dictionary<string, string> userConIdMap = new Dictionary<string, string>();
//		private static Dictionary<string, string> threadUserId = new Dictionary<string, string>();
		

//		public MessageHub(ILogger logger, IRepository<GPTFriendChatMessage> gptFriendChatMessageRepository, IStoreContext storeContext, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, IUrlRecordService urlRecordService, IPictureService pictureService, IProductService productService, IHubContext<MessageHub> messageHubContext)
//        {
//			_logger = logger;
//			_gptFriendChatMessageRepository = gptFriendChatMessageRepository;
//			_storeContext = storeContext;
//			_actionContextAccessor = actionContextAccessor;
//            _urlHelperFactory = urlHelperFactory;
//            _urlRecordService = urlRecordService;
//			_pictureService = pictureService;
//            _productService = productService;
//            _messageHubContext = messageHubContext;    
//        }
        
//        public override Task OnConnectedAsync()
//        {
//            var userId = Context.GetHttpContext().Request.Query["userId"];
//            Groups.AddToGroupAsync(Context.ConnectionId, userId).Wait();
//            userConIdMap[Context.ConnectionId] = userId;

//            if (userId != "Admin")
//                _messageHubContext.Clients.Group("Admin").SendAsync("ImAlive", userId).Wait();
            
//            return base.OnConnectedAsync();
//        }

//        public override Task OnDisconnectedAsync(Exception exception)
//        {
//            return base.OnDisconnectedAsync(exception);
//        }


//		public async Task UserSendMessage(string message)
//        {
//			string UserId = userConIdMap[this.Context.ConnectionId];
//			try
//            {
//				GPTFriendChatMessage ent = new GPTFriendChatMessage()
//				{
//					FromId = "User",
//					Text = message,
//					ChatId = UserId,
//					CreatedOnUtc = DateTime.UtcNow
//				};
//				await _gptFriendChatMessageRepository.InsertAsync(ent);



//				var assistant = new GPTAssistant.GptAssistant();
//				string thread = "";
//				if (!threadUserId.TryGetValue(UserId, out thread))
//				{
//					thread = await assistant.GetThreadId();
//					threadUserId[UserId] = thread;
//				}

//				await assistant.SendMessage(thread, "user", message);

//				string run_id = await assistant.Run(thread);

//				string status;
//				do
//				{
//					status = await assistant.GetStatus(thread, run_id);
//					if (status == "queued" || status == "in_progress")
//						System.Threading.Thread.Sleep(1000);
//				} while (status == "queued" || status == "in_progress");

//				if (status != "completed")
//				{
//					await _messageHubContext.Clients.Group(UserId).SendAsync("Message", "Error", "Admin");
//					return;
//				}

//				RootObject response = await assistant.GetMessages(thread, run_id);
//				var assistant_messages = response.Data.Where(r => r.Role == "assistant").ToArray();
//				string m = assistant_messages.First().Content.Last().Text.Value;

//				ent = new GPTFriendChatMessage()
//				{
//					FromId = "GPT",
//					Text = m,
//					ChatId = UserId,
//					CreatedOnUtc = DateTime.UtcNow
//				};
//				await _gptFriendChatMessageRepository.InsertAsync(ent);

//				var r = GptFriendService.GetJson(m);

//				if (!string.IsNullOrEmpty(r.json))
//				{
//					dynamic json = JsonConvert.DeserializeObject<dynamic>(r.json);
//					string res = json.respuesta;
//					await _messageHubContext.Clients.Group(UserId).SendAsync("Message", res, "Admin");
//					string html = string.Empty;
//					foreach (int id in json.articulos)
//					{
//						var p = await _productService.GetProductByIdAsync(id);
//						if (p == null)
//							continue;


//						var productPicture = (await _pictureService.GetPicturesByProductIdAsync(p.Id, 1)).FirstOrDefault();
//						string imageUrl = (await _pictureService.GetPictureUrlAsync(productPicture)).Url;

//						// Genera la URL amigable (SEO friendly URL)
//						var productUrl = await _urlRecordService.GetSeNameAsync(p);

//						html = html + @$"
//                    <div class='articulo'>
//                        <div class='article'>
//                            <img src='{imageUrl}'/>
//                            <div class='info'>
//                                <div class='product_name'>
//                                    <a href='/{productUrl}' target='_blank'>{p.Name}</a>
//                                </div>
//                                <div class='price'>{p.Price.ToString("N2")} €</div>
//                            </div>
//                        </div>
//                    </div>
//                    ";
//					}
//					await _messageHubContext.Clients.Group(UserId).SendAsync("Message", html, "Admin");
//				}
//				else
//				{
//					await _messageHubContext.Clients.Group(UserId).SendAsync("Message", r.text, "Admin");
//				}
//			}
//			catch(Exception ex)
//            {
//				await _logger.ErrorAsync(ex.Message, ex);
//				await _messageHubContext.Clients.Group(UserId).SendAsync("Message", "ERROR", "Admin");
//			}
            
            
			
//        }

//    }
//}
