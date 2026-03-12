using System;
using System.Collections.Generic;

namespace GestionCitas.Models;

public partial class Empleado
{
    public int empleadoid { get; set; }

    public string? nombre { get; set; }
    public string? apellidos { get; set; }

    public string? telefono { get; set; }

    public int negocioid { get; set; }

    public string? color_agenda { get; set; }
    public string? estado { get; set; }

    public virtual ICollection<Cita> cita { get; set; } = new List<Cita>();

    public virtual Negocio negocio { get; set; } = null!;
}
