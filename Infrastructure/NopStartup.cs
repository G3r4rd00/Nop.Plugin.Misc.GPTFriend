
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.GPTFriend.Factories;
using Nop.Plugin.Misc.GPTFriend.Services;


namespace Nop.Plugin.Misc.GPTFriend.Infrastructure
{

    public class NopStartup : INopStartup
    {
        public int Order => 100;

        public void Configure(IApplicationBuilder application)
        {
            application.UseAuthorization();
            application.UseRouting();
            //application.UseCustomRedirect();
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MessageHub>("/notify");
            });
        }

        
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();

			
			services.AddScoped<GptFriendService>();
			services.AddScoped<QuestionFactory>();
			services.AddScoped<QuestionParametersFactory>();
		}
    }
}