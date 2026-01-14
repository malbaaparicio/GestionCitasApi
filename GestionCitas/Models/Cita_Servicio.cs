using System;
using System.Collections.Generic;

namespace GestionCitas.Models;

public partial class Cita_Servicio
{
    public int cita_serviciosid { get; set; }

    public int? citaid { get; set; }

    public int? servicioid { get; set; }

    public decimal? precio_aplicado { get; set; }

    public int? duracion { get; set; }

    public int? negocioid { get; set; }

    public virtual Cita? cita { get; set; }

    public virtual Negocio? negocio { get; set; }

    public virtual Servicio? servicio { get; set; }
}
