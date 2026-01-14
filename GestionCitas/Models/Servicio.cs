using System;
using System.Collections.Generic;

namespace GestionCitas.Models;

public partial class Servicio
{
    public int servicioid { get; set; }

    public string? nombre { get; set; }

    public int? negocioid { get; set; }

    public int? duracion { get; set; }

    public decimal? precio_actual { get; set; }

    public virtual ICollection<Cita_Servicio> cita_servicios { get; set; } = new List<Cita_Servicio>();

    public virtual Negocio? negocio { get; set; }
}
