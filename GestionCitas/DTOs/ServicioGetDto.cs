namespace GestionCitas.DTOs
{
    public class ServicioGetDto
    {
        public int servicioid { get; set; }
        public string? nombre { get; set; }      
        public int? duracion { get; set; }
        public decimal? precio_actual { get; set; }
        public string? estado { get; set; }

    }
}
