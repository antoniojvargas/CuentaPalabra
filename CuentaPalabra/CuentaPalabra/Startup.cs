using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CuentaPalabra.Startup))]
namespace CuentaPalabra
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
