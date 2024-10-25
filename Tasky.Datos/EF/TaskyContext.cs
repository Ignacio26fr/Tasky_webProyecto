﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Tasky.Datos.EF;


public partial class TaskyContext : IdentityDbContext<AspNetUsers>
{
    public TaskyContext()
    {
    }

    public TaskyContext(DbContextOptions<TaskyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
    public virtual DbSet<AspNetUsers> AspNetUsers { get; set; } = null!;
    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
    //    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
    // public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-CTSE8NE;Database=Tasky;Trusted_Connection=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.ToTable("AspNetRoles");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.ToTable("AspNetRolesClaims");
            entity.Property(e => e.RoleId).HasMaxLength(450);
            entity.HasOne(d => d.Role)
                .WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__AspNetRol__RoleI__48CFD27E");
        });

        modelBuilder.Entity<AspNetUsers>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ__AspNetUs__A9D10534BB6262D4").IsUnique();
            entity.HasIndex(e => e.UserName, "UQ__AspNetUs__C9F284561E4A71B1").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles)
                .WithMany(p => p.Users)
                .UsingEntity<IdentityUserRole<string>>(
                    j => j
                        .HasOne<AspNetRole>()
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__AspNetUse__RoleI__4316F928"),
                    j => j
                        .HasOne<AspNetUsers>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__AspNetUse__UserI__4222D4EF"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__AspNetUs__AF2760ADD43502DA");
                        j.ToTable("AspNetUserRoles");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.HasOne(d => d.User)
                .WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AspNetUse__UserI__45F365D3");
        });





        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}


