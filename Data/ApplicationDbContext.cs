using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BddFindCulpritTool.Models;

namespace BddFindCulpritTool.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BddFindCulpritTool.Models.PrRun> PrRun { get; set; }

        public DbSet<BddFindCulpritTool.Models.Configuration> Configuration { get; set; }

        public DbSet<BddFindCulpritTool.Models.BinarySearchPrRun> BinarySearchPrRun { get; set; }

        public DbSet<BddFindCulpritTool.Models.BinarySearchPoint> BinarySearchPoint { get; set; }
    }
}
