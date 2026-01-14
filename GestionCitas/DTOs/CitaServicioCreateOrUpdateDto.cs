namespace GestionCitas.DTOs
{
    public class CitaServicioCreateOrUpdateDto
    {
        public int citaid { get; set; }
        public int servicioid { get; set; }
        public decimal precio_aplicado { get; set; }
        public int duracion { get; set; }
    }
}
