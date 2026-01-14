using System;
using System.Collections.Generic;

namespace GestionCitas.Models;

public partial class Cliente
{
    public int clienteid { get; set; }

    public string telefono { get; set; } = null!;

    public string email { get; set; } = null!;

    public string? nombre { get; set; }

    public string? apellidos { get; set; }

    public int negocioid { get; set; }

    public string? notas_internas { get; set; }

    public virtual ICollection<Cita> cita { get; set; } = new List<Cita>();

    public virtual Negocio? negocio { get; set; }
}
