using System;
using System.Collections.Generic;

namespace GestionCitas.Models;

public partial class Negocio
{
    public int negocioid { get; set; }

    public string? nombre { get; set; }

    public string? telefono { get; set; }

    public string? direccion { get; set; }

    public string? localidad { get; set; }

    public string? email { get; set; }

    public string? conf_whatsapp { get; set; }

    public virtual ICollection<Cita> cita { get; set; } = new List<Cita>();

    public virtual ICollection<Cita_Servicio> cita_servicios { get; set; } = new List<Cita_Servicio>();

    public virtual ICollection<Cliente> clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Empleado> empleados { get; set; } = new List<Empleado>();

    public virtual ICollection<Servicio> servicios { get; set; } = new List<Servicio>();
}
