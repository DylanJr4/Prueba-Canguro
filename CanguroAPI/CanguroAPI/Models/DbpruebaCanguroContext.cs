using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CanguroAPI.Server.Models;

public partial class DbpruebaCanguroContext : DbContext
{
    public DbpruebaCanguroContext()
    {
    }

    public DbpruebaCanguroContext(DbContextOptions<DbpruebaCanguroContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Moneda> Moneda { get; set; }

    public virtual DbSet<Sucursal> Sucursals { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Log> Logs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Moneda>(entity =>
        {
            entity.HasKey(e => e.IdMoneda).HasName("PK__Moneda__AA690671D821DD0F");

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal).HasName("PK__Sucursal__BFB6CD9979F7D842");

            entity.ToTable("Sucursal");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.Identificacion)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdMonedaNavigation).WithMany(p => p.Sucursals)
                .HasForeignKey(d => d.IdMoneda)
                .HasConstraintName("FK_Sucursal_Moneda");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF976B358B32");

            entity.ToTable("Usuario");

            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PK__Logs__0C54DBC65050A00D");

            entity.Property(e => e.ActionName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ControllerName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.LogLevel)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.RequestUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
