using Autofac;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using Pustalorc.GlobalBan.Database;
using Pustalorc.PlayerInfoLib.Unturned.Database;

namespace Pustalorc.GlobalBan
{
    public class ContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(ILifetimeScope parentLifetimeScope, IConfiguration configuration,
            ContainerBuilder containerBuilder)
        {
            containerBuilder.AddEntityFrameworkCoreMySql();
            containerBuilder.AddDbContext<GlobalBanDbContext>();
            containerBuilder.AddDbContext<PlayerInfoLibDbContext>();
        }
    }
}