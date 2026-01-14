using System;
using System.Collections;
using System.Collections.Generic;

namespace GestionCitas.Models;

public partial class Cita
{
    public int citaid { get; set; }

    public int? clienteid { get; set; }

    public decimal precio_total { get; set; }

    public decimal precio_sugerido { get; set; }

    public int? duracion_total { get; set; }

    public string? estado { get; set; }

    public DateTime? fecha_hora_inicio { get; set; }

    public DateTime? fecha_hora_fin { get; set; }

    public int? negocioid { get; set; }

    public int? empleadoid { get; set; }

    public string? observaciones { get; set; }

    public BitArray? recordatorio_enviado { get; set; }

    public virtual ICollection<Cita_Servicio> cita_servicios { get; set; } = new List<Cita_Servicio>();

    public virtual Cliente? cliente { get; set; }

    public virtual Empleado? empleado { get; set; }

    public virtual Negocio? negocio { get; set; }
}
