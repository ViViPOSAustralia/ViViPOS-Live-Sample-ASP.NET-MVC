using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ViViPOSLive_Sample_AspNet.Startup))]
namespace ViViPOSLive_Sample_AspNet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
