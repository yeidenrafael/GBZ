using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GrinGlobal.Zone.Startup))]
namespace GrinGlobal.Zone
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
