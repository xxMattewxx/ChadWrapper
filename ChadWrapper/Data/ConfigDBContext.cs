using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapperMake.Data
{
    class ConfigDBContext : DbContext
    {
        public DbSet<ConfigEntry> Config { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder = optionsBuilder.UseNpgsql(Global.ChadDBConnectionBuilder.ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
