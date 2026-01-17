namespace GestionCitas.DTOs
{
    public class CitaGetDto
    {
        public int citaid { get; set; }
        public DateTime fecha_hora_inicio { get; set; }
        public DateTime fecha_hora_fin { get; set; }
        public string? estado { get; set; }       
        public decimal precio_total { get; set; }
        public decimal precio_sugerido { get; set; }
        public string NombreCliente { get; set; }
        public string NombreEmpleado { get; set; }

    }
}
