using System;
using Microsoft.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore.Configurator;
using Pustalorc.GlobalBan.API.Classes;

namespace Pustalorc.GlobalBan.Database
{
    public class GlobalBanDbContext : OpenModDbContext<GlobalBanDbContext>
    {
        public DbSet<PlayerBan> PlayerBans { get; set; }

        public GlobalBanDbContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider) : base(configurator, serviceProvider)
        {
        }
    }
} 
