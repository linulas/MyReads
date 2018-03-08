using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyReads.Startup))]
namespace MyReads
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
