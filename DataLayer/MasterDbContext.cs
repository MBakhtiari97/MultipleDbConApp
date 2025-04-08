﻿using Core;
using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options)
        : base(options)
    {
    }
    public DbSet<AppUser> AppUser { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<SystemLog>();
    }
}