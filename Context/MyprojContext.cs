using System;
using System.Collections.Generic;
using Agents.Models;
using Microsoft.EntityFrameworkCore;

namespace Agents.Context;

public partial class MyprojContext : DbContext
{
    public MyprojContext()
    {
    }

    public MyprojContext(DbContextOptions<MyprojContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AgPriorChange> AgPriorChanges { get; set; }

    public virtual DbSet<Agent> Agents { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<TochkaProd> TochkaProds { get; set; }

    public virtual DbSet<Models.Type> Types { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=myproj;Username=postgres;Password=18b22M02a");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgPriorChange>(entity =>
        {
            entity.HasKey(e => e.IdPriorChange).HasName("ag_prior_change_pkey");

            entity.ToTable("ag_prior_change", "agents");

            entity.Property(e => e.IdPriorChange)
                .ValueGeneratedNever()
                .HasColumnName("id_prior_change");
            entity.Property(e => e.IdAgent).HasColumnName("id_agent");
            entity.Property(e => e.IdNewPrior).HasColumnName("id_new_prior");

            entity.HasOne(d => d.IdNewPriorNavigation).WithMany(p => p.AgPriorChanges)
                .HasForeignKey(d => d.IdNewPrior)
                .HasConstraintName("pr_ch");
        });

        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasKey(e => e.IdAgent).HasName("agent_pkey");

            entity.ToTable("agent", "agents");

            entity.Property(e => e.IdAgent)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_agent");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.Director)
                .HasColumnType("character varying")
                .HasColumnName("director");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.IdDiscount).HasColumnName("id_discount");
            entity.Property(e => e.IdPriority).HasColumnName("id_priority");
            entity.Property(e => e.IdType).HasColumnName("id_type");
            entity.Property(e => e.Inn).HasColumnName("inn");
            entity.Property(e => e.Kpp).HasColumnName("kpp");
            entity.Property(e => e.Logo)
                .HasColumnType("character varying")
                .HasColumnName("logo");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasColumnType("character varying")
                .HasColumnName("phone");

            entity.HasOne(d => d.IdDiscountNavigation).WithMany(p => p.Agents)
                .HasForeignKey(d => d.IdDiscount)
                .HasConstraintName("disc_fk");

            entity.HasOne(d => d.IdPriorityNavigation).WithMany(p => p.Agents)
                .HasForeignKey(d => d.IdPriority)
                .HasConstraintName("prior_fk");

            entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.Agents)
                .HasForeignKey(d => d.IdType)
                .HasConstraintName("type_fk");

            entity.HasMany(d => d.IdTochkaPrs).WithMany(p => p.IdAgents)
                .UsingEntity<Dictionary<string, object>>(
                    "AgentTochkaPr",
                    r => r.HasOne<TochkaProd>().WithMany()
                        .HasForeignKey("IdTochkaPr")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tochka_pr_fk"),
                    l => l.HasOne<Agent>().WithMany()
                        .HasForeignKey("IdAgent")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("ag_fk"),
                    j =>
                    {
                        j.HasKey("IdAgent", "IdTochkaPr").HasName("agent_tochka_pr_pkey");
                        j.ToTable("agent_tochka_pr", "agents");
                        j.IndexerProperty<int>("IdAgent").HasColumnName("id_agent");
                        j.IndexerProperty<int>("IdTochkaPr").HasColumnName("id_tochka_pr");
                    });
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.IdDiscount).HasName("discount_pkey");

            entity.ToTable("discount", "agents");

            entity.Property(e => e.IdDiscount)
                .ValueGeneratedNever()
                .HasColumnName("id_discount");
            entity.Property(e => e.NameDisc)
                .HasColumnType("character varying")
                .HasColumnName("name_disc");
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.IdPriority).HasName("id_prior");

            entity.ToTable("priority", "agents");

            entity.Property(e => e.IdPriority)
                .ValueGeneratedNever()
                .HasColumnName("id_priority");
            entity.Property(e => e.NamePr)
                .HasColumnType("character varying")
                .HasColumnName("name_pr");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("product_pkey");

            entity.ToTable("product", "agents");

            entity.Property(e => e.IdProduct)
                .ValueGeneratedNever()
                .HasColumnName("id_product");
            entity.Property(e => e.NameProduct)
                .HasColumnType("character varying")
                .HasColumnName("name_product");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.IdSale).HasName("sale_pkey");

            entity.ToTable("sale", "agents");

            entity.Property(e => e.IdSale).HasColumnName("id_sale");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.IdAgent).HasColumnName("id_agent");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");

            entity.HasOne(d => d.IdAgentNavigation).WithMany(p => p.Sales)
                .HasForeignKey(d => d.IdAgent)
                .HasConstraintName("agent_fk");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Sales)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("prod_fkey");
        });

        modelBuilder.Entity<TochkaProd>(entity =>
        {
            entity.HasKey(e => e.IdTochka).HasName("tochka_prod_pkey");

            entity.ToTable("tochka_prod", "agents");

            entity.Property(e => e.IdTochka)
                .ValueGeneratedNever()
                .HasColumnName("id_tochka");
            entity.Property(e => e.NameTochka)
                .HasColumnType("character varying")
                .HasColumnName("name_tochka");
        });

        modelBuilder.Entity<Models.Type>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("type_pkey");

            entity.ToTable("type", "agents");

            entity.Property(e => e.IdType)
                .ValueGeneratedNever()
                .HasColumnName("id_type");
            entity.Property(e => e.NameType)
                .HasColumnType("character varying")
                .HasColumnName("name_type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
