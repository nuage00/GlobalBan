using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using Pustalorc.GlobalBan.Database;

namespace Pustalorc.GlobalBan
{
    public class ContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddEntityFrameworkCoreMySql();
            context.ContainerBuilder.AddDbContext<GlobalBanDbContext>();
        }
    }
}