using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(S.A___BookRental_LTD.Startup))]
namespace S.A___BookRental_LTD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
