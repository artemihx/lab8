using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cafeapp1.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<Foodonorder> Foodonorders { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderstatus> Orderstatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Waiterontable> Waiterontables { get; set; }

    public virtual DbSet<Workersonshift> Workersonshifts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("foods_pkey");

            entity.ToTable("foods");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<Foodonorder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("foodonorders_pkey");

            entity.ToTable("foodonorders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Idfood).HasColumnName("idfood");
            entity.Property(e => e.Idorder).HasColumnName("idorder");

            entity.HasOne(d => d.IdfoodNavigation).WithMany(p => p.Foodonorders)
                .HasForeignKey(d => d.Idfood)
                .HasConstraintName("foodonorders_idfood_fkey");

            entity.HasOne(d => d.IdorderNavigation).WithMany(p => p.Foodonorders)
                .HasForeignKey(d => d.Idorder)
                .HasConstraintName("foodonorders_idorder_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.Shiftid).HasColumnName("shiftid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Tableid).HasColumnName("tableid");

            entity.HasOne(d => d.Shift).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Shiftid)
                .HasConstraintName("orders_shiftid_fkey");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("orders_status_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Tableid)
                .HasConstraintName("orders_tableid_fkey");
        });

        modelBuilder.Entity<Orderstatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orderstatus_pkey");

            entity.ToTable("orderstatus");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shift_pkey");

            entity.ToTable("shift");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Doc)
                .HasColumnType("character varying")
                .HasColumnName("doc");
            entity.Property(e => e.Fname)
                .HasColumnType("character varying")
                .HasColumnName("fname");
            entity.Property(e => e.Lname)
                .HasColumnType("character varying")
                .HasColumnName("lname");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Mname)
                .HasColumnType("character varying")
                .HasColumnName("mname");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Photo)
                .HasColumnType("character varying")
                .HasColumnName("photo");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("users_roleid_fkey");
        });

        modelBuilder.Entity<Waiterontable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("waiterontables_pkey");

            entity.ToTable("waiterontables");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Idtable).HasColumnName("idtable");
            entity.Property(e => e.Idwaiter).HasColumnName("idwaiter");

            entity.HasOne(d => d.IdtableNavigation).WithMany(p => p.Waiterontables)
                .HasForeignKey(d => d.Idtable)
                .HasConstraintName("waiterontables_idtable_fkey");

            entity.HasOne(d => d.IdwaiterNavigation).WithMany(p => p.Waiterontables)
                .HasForeignKey(d => d.Idwaiter)
                .HasConstraintName("waiterontables_idwaiter_fkey");
        });

        modelBuilder.Entity<Workersonshift>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("workersonshift_pkey");

            entity.ToTable("workersonshift");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Shiftid).HasColumnName("shiftid");
            entity.Property(e => e.Workerid).HasColumnName("workerid");

            entity.HasOne(d => d.Shift).WithMany(p => p.Workersonshifts)
                .HasForeignKey(d => d.Shiftid)
                .HasConstraintName("workersonshift_shiftid_fkey");

            entity.HasOne(d => d.Worker).WithMany(p => p.Workersonshifts)
                .HasForeignKey(d => d.Workerid)
                .HasConstraintName("workersonshift_workerid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
