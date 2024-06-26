﻿using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Configurations;

namespace Persistence.Context;

//Our class now inherits from the IdentityDbContext class and not DbContext because we want to integrate our context with Identity
public class AppDBContext : IdentityDbContext<UserModel>
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }

    public DbSet<CustomerModel> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //We call the OnModelCreating method from the base class because we use Asp.NET Core Identity and it required to let Microsoft Identity do its stuff.
        //Now, we have to move on to the configuration part. Please see the configuration class for more details in Api > Service Installers > IdentityInstaller.cs.
        base.OnModelCreating(modelBuilder);

        //It will take all configurations from IEntityTypeConfiguration interface and run it.
        //Please refer to this link : https://learn.microsoft.com/en-us/ef/core/modeling/
        //For more details of EF Core code-first data model, please refer to this link : https://www.entityframeworktutorial.net/code-first
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerConfiguration).Assembly);

        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }
}