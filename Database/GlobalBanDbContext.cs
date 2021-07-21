using System;
using Microsoft.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore;
using Pustalorc.GlobalBan.API.Classes;

namespace Pustalorc.GlobalBan.Database
{
    public class GlobalBanDbContext : OpenModDbContext<GlobalBanDbContext>
    {
        public DbSet<PlayerBan> PlayerBans => Set<PlayerBan>();

        public GlobalBanDbContext(DbContextOptions<GlobalBanDbContext> options,
            IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}
