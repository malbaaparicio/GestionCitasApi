using System;
using System.Collections.Generic;
using GestionCitas.Common;
using Microsoft.EntityFrameworkCore;

namespace GestionCitas.Models;

public partial class GestionCitasContext : DbContext
{
    private readonly ICurrentTenantService _currentTenantService;

    public GestionCitasContext(DbContextOptions<GestionCitasContext>options, ICurrentTenantService currentTenantService)
        : base(options)
    {
        _currentTenantService = currentTenantService;
    }
     

    public virtual DbSet<Cita> citas { get; set; }

    public virtual DbSet<Cita_Servicio> cita_servicios { get; set; }

    public virtual DbSet<Cliente> clientes { get; set; }

    public virtual DbSet<Empleado> empleados { get; set; }

    public virtual DbSet<Negocio> negocios { get; set; }

    public virtual DbSet<Servicio> servicios { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
            {
                // Aplicamos el filtro: "Solo trae registros donde negocioid coincida con el usuario actual"
                var method = typeof(GestionCitasContext)
                    .GetMethod(nameof(SetGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(this, new object[] { modelBuilder });
            }
        }

        modelBuilder.Entity<Cita>((Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cita>>)(entity =>
        {
            entity.HasKey(e => e.citaid).HasName("pk_citaid");

            entity.Property(e => e.citaid).UseIdentityAlwaysColumn();
            entity.Property((System.Linq.Expressions.Expression<Func<Cita, string?>>)(e => e.estado)).HasMaxLength(255);
            entity.Property(e => e.fecha_hora_fin).HasColumnType("timestamp without time zone");
            entity.Property(e => e.fecha_hora_inicio).HasColumnType("timestamp without time zone");
            entity.Property((System.Linq.Expressions.Expression<Func<Cita, string?>>)(e => e.observaciones)).HasMaxLength(255);
            entity.Property(e => e.precio_sugerido).HasPrecision(5, 2);
            entity.Property(e => e.precio_total).HasPrecision(5, 2);
            entity.Property((System.Linq.Expressions.Expression<Func<Cita, System.Collections.BitArray?>>)(e => e.recordatorio_enviado)).HasColumnType("bit(1)");

            entity.HasOne(d => d.cliente).WithMany(p => p.cita)
                .HasForeignKey(d => d.clienteid)
                .HasConstraintName("fk_clienteid");

            entity.HasOne(d => d.empleado).WithMany(p => p.cita)
                .HasForeignKey(d => d.empleadoid)
                .HasConstraintName("fk_empleadoid");

            entity.HasOne(d => d.negocio).WithMany(p => p.cita)
                .HasForeignKey((System.Linq.Expressions.Expression<Func<Cita, object?>>)(d => d.negocioid))
                .HasConstraintName("fk_negocioid");
        }));

        modelBuilder.Entity<Cita_Servicio>((Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cita_Servicio>>)(entity =>
        {
            entity.HasKey(e => e.cita_serviciosid).HasName("pk_cita_serviciosid");

            entity.Property(e => e.cita_serviciosid).UseIdentityAlwaysColumn();
            entity.Property(e => e.precio_aplicado).HasPrecision(5, 2);

            entity.HasOne(d => d.cita).WithMany(p => p.cita_servicios)
                .HasForeignKey(d => d.citaid)
                .HasConstraintName("fk_citaid");

            entity.HasOne(d => d.negocio).WithMany(p => p.cita_servicios)
                .HasForeignKey((System.Linq.Expressions.Expression<Func<Cita_Servicio, object?>>)(d => d.negocioid))
                .HasConstraintName("fk_negocioid");

            entity.HasOne(d => d.servicio).WithMany(p => p.cita_servicios)
                .HasForeignKey(d => d.servicioid)
                .HasConstraintName("fk_servicioid");
        }));

        modelBuilder.Entity<Cliente>((Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cliente>>)(entity =>
        {
            entity.HasKey(e => e.clienteid).HasName("pk_clienteid");

            entity.HasIndex(e => e.email, "clientes_email_key").IsUnique();

            entity.HasIndex(e => new {e.negocioid, e.telefono}, "clientes_telefono_key").IsUnique();

            entity.Property(e => e.clienteid).UseIdentityAlwaysColumn();
            entity.Property((System.Linq.Expressions.Expression<Func<Cliente, string?>>)(e => e.apellidos)).HasMaxLength(255);
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Cliente, string?>>)(e => e.nombre)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Cliente, string?>>)(e => e.notas_internas)).HasMaxLength(255);
            entity.Property(e => e.telefono).HasMaxLength(255);

            entity.HasOne(d => d.negocio).WithMany(p => p.clientes)
                .HasForeignKey((System.Linq.Expressions.Expression<Func<Cliente, object?>>)(d => d.negocioid))
                .HasConstraintName("fk_negocioid");
        }));

        modelBuilder.Entity<Empleado>((Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Empleado>>)(entity =>
        {
            entity.HasKey(e => e.empleadoid).HasName("pk_empleadoid");

            entity.Property(e => e.empleadoid).UseIdentityAlwaysColumn();
            entity.Property((System.Linq.Expressions.Expression<Func<Empleado, string?>>)(e => e.color_agenda)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Empleado, string?>>)(e => e.nombre)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Empleado, string?>>)(e => e.apellidos)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Empleado, string?>>)(e => e.telefono)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Empleado, string?>>)(e => e.estado)).HasMaxLength(255);

            entity.HasOne(d => d.negocio).WithMany(p => p.empleados)
                .HasForeignKey((System.Linq.Expressions.Expression<Func<Empleado, object?>>)(d => d.negocioid))
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_negocioid");
        }));

        modelBuilder.Entity<Negocio>((Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Negocio>>)(entity =>
        {
            entity.HasKey((System.Linq.Expressions.Expression<Func<Negocio, object?>>)(e => e.negocioid)).HasName("pk_negocioid");

            entity.Property((System.Linq.Expressions.Expression<Func<Negocio, int>>)(e => (int)e.negocioid)).UseIdentityAlwaysColumn();
            entity.Property((System.Linq.Expressions.Expression<Func<Negocio, string?>>)(e => e.conf_whatsapp)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Negocio, string?>>)(e => e.direccion)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Negocio, string?>>)(e => e.email)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Negocio, string?>>)(e => e.localidad)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Negocio, string?>>)(e => e.nombre)).HasMaxLength(255);
            entity.Property((System.Linq.Expressions.Expression<Func<Negocio, string?>>)(e => e.telefono)).HasMaxLength(255);
        }));

        modelBuilder.Entity<Servicio>((Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Servicio>>)(entity =>
        {
            entity.HasKey(e => e.servicioid).HasName("pk_servicioid");

            entity.Property(e => e.servicioid).UseIdentityAlwaysColumn();
            entity.Property((System.Linq.Expressions.Expression<Func<Servicio, string?>>)(e => e.nombre)).HasMaxLength(255);
            entity.Property(e => e.precio_actual).HasPrecision(5, 2);

            entity.HasOne(d => d.negocio).WithMany(p => p.servicios)
                .HasForeignKey((System.Linq.Expressions.Expression<Func<Servicio, object?>>)(d => d.negocioid))
                .HasConstraintName("fk_negocioid");
        }));

        OnModelCreatingPartial(modelBuilder);
    }
    // Método auxiliar para construir la expresión lambda dinámicamente
    

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    private void SetGlobalQueryFilter<T>(ModelBuilder modelBuilder) where T : class, IMustHaveTenant
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => e.negocioid == _currentTenantService.NegocioId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Buscamos todas las entidades que se van a insertar o modificar
        foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    // Si el servicio nos da un ID válido, lo forzamos en la entidad
                    if (_currentTenantService.NegocioId.HasValue)
                    {
                        entry.Entity.negocioid = _currentTenantService.NegocioId.Value;
                    }
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
