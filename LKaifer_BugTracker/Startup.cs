using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LKaifer_BugTracker.Startup))]
namespace LKaifer_BugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
