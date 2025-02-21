using System.Collections;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.GPTFriend
{
    /// <summary>
    /// Represents a plugin settings
    /// </summary>
    public class GPTFriendSettings : ISettings
    {
        public string GptKey { get; set; }

        public string GptModel { get; set; } 

        public IList<SelectListItem> GptModels { get; set; }


        public int GptMaxTokens { get; set; }
        public int GptDelay { get; set; } 


        public string GptFamiliasSystemMessage { get; set; }
		


		public string GptMarcasSystemMessage { get; set; }

        public string GptProductSystemMessage { get; set; }
        public string GptProductCategorySystemMessage { get; set; }
    }
}