
using Newtonsoft.Json;
using Nop.Plugin.Misc.GPTFriend.Services.GPTAssistant.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.GPTFriend.Services.GPTAssistant
{
	public class GptAssistant
	{
		string apiKey = "";
		string assistantId = "";

		public  async Task<string> GetThreadId()
		{
			string thread_id;
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
				client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

				var response = await client.PostAsync($"https://api.openai.com/v1/threads", null);
				var responseString = await response.Content.ReadAsStringAsync();
				dynamic obj = JsonConvert.DeserializeObject<dynamic>(responseString);
				thread_id = obj.id;
			}

			return thread_id;
		}

		public  async Task<string> GetStatus(string thread_id, string run_id)
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
				client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");


				var response = await client.GetAsync($"https://api.openai.com/v1/threads/{thread_id}/runs/{run_id}");
				var responseString = await response.Content.ReadAsStringAsync();
				dynamic obj = JsonConvert.DeserializeObject<dynamic>(responseString);

				return obj.status;
			}
		}

		public  async Task<RootObject> GetMessages(string thread_id, string run_id)
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
				client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

				var response = await client.GetAsync($"https://api.openai.com/v1/threads/{thread_id}/messages?run_id="+run_id);
				var responseString = await response.Content.ReadAsStringAsync();
				RootObject obj = JsonConvert.DeserializeObject<RootObject>(responseString);

				return obj;
			}
		}

		public async Task SendMessage(string thread_id,string role, string message)
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
				client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

				var data = new
				{
					role = role,
					content = message
					//,metadata = new { customer_id = user_id}
				};
				var jsonContent = JsonConvert.SerializeObject(data);
				var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
				var response = await client.PostAsync($"https://api.openai.com/v1/threads/{thread_id}/messages", content);
				var responseString = await response.Content.ReadAsStringAsync();
			}
		}

		public  async Task<string> Run(string thread_id)
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
				client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

				var data = new
				{
					assistant_id = assistantId
				};
				var jsonContent = JsonConvert.SerializeObject(data);
				var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
				var response = await client.PostAsync($"https://api.openai.com/v1/threads/{thread_id}/runs", content);
				var responseString = await response.Content.ReadAsStringAsync();
				dynamic obj = JsonConvert.DeserializeObject<dynamic>(responseString);

				return obj.id;
			}
		}

	}
}
