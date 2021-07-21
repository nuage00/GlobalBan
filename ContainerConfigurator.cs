using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.MySql.Extensions;
using Pustalorc.GlobalBan.Database;

namespace Pustalorc.GlobalBan
{
    public class ContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddMySqlDbContext<GlobalBanDbContext>();
        }
    }
}
