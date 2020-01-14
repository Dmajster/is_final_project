using Microsoft.AspNetCore.Hosting;
using MusicShareWeb.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace MusicShareWeb.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}