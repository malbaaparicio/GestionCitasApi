using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionCitas.Models;

public partial class GestionCitasContext : DbContext
{
    public GestionCitasContext()
    {
    }

    public GestionCitasContext(DbContextOptions<GestionCitasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cita> citas { get; set; }

    public virtual DbSet<Cita_Servicio> cita_servicios { get; set; }

    public virtual DbSet<Cliente> clientes { get; set; }

    public virtual DbSet<Empleado> empleados { get; set; }

    public virtual DbSet<Negocio> negocios { get; set; }

    public virtual DbSet<Servicio> servicios { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.citaid).HasName("pk_citaid");

            entity.Property(e => e.citaid).UseIdentityAlwaysColumn();
            entity.Property(e => e.estado).HasMaxLength(255);
            entity.Property(e => e.fecha_hora_fin).HasColumnType("timestamp without time zone");
            entity.Property(e => e.fecha_hora_inicio).HasColumnType("timestamp without time zone");
            entity.Property(e => e.observaciones).HasMaxLength(255);
            entity.Property(e => e.precio_sugerido).HasPrecision(5, 2);
            entity.Property(e => e.precio_total).HasPrecision(5, 2);
            entity.Property(e => e.recordatorio_enviado).HasColumnType("bit(1)");

            entity.HasOne(d => d.cliente).WithMany(p => p.cita)
                .HasForeignKey(d => d.clienteid)
                .HasConstraintName("fk_clienteid");

            entity.HasOne(d => d.empleado).WithMany(p => p.cita)
                .HasForeignKey(d => d.empleadoid)
                .HasConstraintName("fk_empleadoid");

            entity.HasOne(d => d.negocio).WithMany(p => p.cita)
                .HasForeignKey(d => d.negocioid)
                .HasConstraintName("fk_negocioid");
        });

        modelBuilder.Entity<Cita_Servicio>(entity =>
        {
            entity.HasKey(e => e.cita_serviciosid).HasName("pk_cita_serviciosid");

            entity.Property(e => e.cita_serviciosid).UseIdentityAlwaysColumn();
            entity.Property(e => e.precio_aplicado).HasPrecision(5, 2);

            entity.HasOne(d => d.cita).WithMany(p => p.cita_servicios)
                .HasForeignKey(d => d.citaid)
                .HasConstraintName("fk_citaid");

            entity.HasOne(d => d.negocio).WithMany(p => p.cita_servicios)
                .HasForeignKey(d => d.negocioid)
                .HasConstraintName("fk_negocioid");

            entity.HasOne(d => d.servicio).WithMany(p => p.cita_servicios)
                .HasForeignKey(d => d.servicioid)
                .HasConstraintName("fk_servicioid");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.clienteid).HasName("pk_clienteid");

            entity.HasIndex(e => e.email, "clientes_email_key").IsUnique();

            entity.HasIndex(e => e.telefono, "clientes_telefono_key").IsUnique();

            entity.Property(e => e.clienteid).UseIdentityAlwaysColumn();
            entity.Property(e => e.apellidos).HasMaxLength(255);
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.nombre).HasMaxLength(255);
            entity.Property(e => e.notas_internas).HasMaxLength(255);
            entity.Property(e => e.telefono).HasMaxLength(255);

            entity.HasOne(d => d.negocio).WithMany(p => p.clientes)
                .HasForeignKey(d => d.negocioid)
                .HasConstraintName("fk_negocioid");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.empleadoid).HasName("pk_empleadoid");

            entity.Property(e => e.empleadoid).UseIdentityAlwaysColumn();
            entity.Property(e => e.color_agenda).HasMaxLength(255);
            entity.Property(e => e.nombre).HasMaxLength(255);

            entity.HasOne(d => d.negocio).WithMany(p => p.empleados)
                .HasForeignKey(d => d.negocioid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_negocioid");
        });

        modelBuilder.Entity<Negocio>(entity =>
        {
            entity.HasKey(e => e.negocioid).HasName("pk_negocioid");

            entity.Property(e => e.negocioid).UseIdentityAlwaysColumn();
            entity.Property(e => e.conf_whatsapp).HasMaxLength(255);
            entity.Property(e => e.direccion).HasMaxLength(255);
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.localidad).HasMaxLength(255);
            entity.Property(e => e.nombre).HasMaxLength(255);
            entity.Property(e => e.telefono).HasMaxLength(255);
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.servicioid).HasName("pk_servicioid");

            entity.Property(e => e.servicioid).UseIdentityAlwaysColumn();
            entity.Property(e => e.nombre).HasMaxLength(255);
            entity.Property(e => e.precio_actual).HasPrecision(5, 2);

            entity.HasOne(d => d.negocio).WithMany(p => p.servicios)
                .HasForeignKey(d => d.negocioid)
                .HasConstraintName("fk_negocioid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
