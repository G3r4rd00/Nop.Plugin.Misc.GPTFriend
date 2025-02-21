
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core;


namespace Nop.Plugin.Misc.GPTFriend.Domain
{
	
	public class GPTFriendChatMessage : BaseEntity
	{
		[Required]
		public string ChatId { get; set; }

		[Required]
		public string FromId { get; set; }

		[Required]
		public string Text { get; set; }



		[Required]
		public DateTime CreatedOnUtc { get; set; }

	}


}
