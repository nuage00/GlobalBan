// ReSharper disable AnnotateCanBeNullParameter
// ReSharper disable AnnotateNotNullParameter
// ReSharper disable AnnotateCanBeNullTypeMember
// ReSharper disable AnnotateNotNullTypeMember
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Global

using System;
using Microsoft.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore;
using Pustalorc.GlobalBan.API.Classes;

namespace Pustalorc.GlobalBan.Database
{
    public class GlobalBanDbContext : OpenModDbContext<GlobalBanDbContext>
    {
        public DbSet<PlayerBan> PlayerBans { get; set; }

        public GlobalBanDbContext(DbContextOptions<GlobalBanDbContext> options,
            IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }
    }
}