using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProtoCI.Startup))]
namespace ProtoCI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            ConfigureAuth(app);
        }
    }
}
