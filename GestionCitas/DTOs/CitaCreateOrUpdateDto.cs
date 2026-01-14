namespace GestionCitas.DTOs
{
    public class CitaCreateOrUpdateDto
    {
        public DateTime fecha_hora { get; set; }
        public int clienteid { get; set; }
        public decimal precio_total { get; set; }       
        public int duracion_total { get; set; }
        public string? estado { get; set; }

    }
}
