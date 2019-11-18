using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NTG.Test.Startup))]
namespace NTG.Test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
