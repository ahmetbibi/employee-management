using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser> // DbContext
    {
        // we are getting the DbContextOptions with this constructor and then passing it to the derived class (DbContext) by using base() constructor
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        // We have to create all the database sets related to our models
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            // To avoid the cascade on delete operation
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}