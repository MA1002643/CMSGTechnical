using CMSGTechnical.Domain;
using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Repository
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            builder.FindMissingEntitiesOfType<IEntity>(typeof(AssemblyHook).Assembly);

            builder.SeedData();
        }
       
       
    }
}
