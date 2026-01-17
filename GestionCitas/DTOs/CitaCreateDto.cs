namespace GestionCitas.DTOs
{
    public class CitaCreateDto
    {
        public int? clienteid { get; set; }
        public int? empleadoid { get; set; }
        public DateTime fecha_hora_inicio { get; set; }
        public string observaciones { get; set; }
        public List<int> serviciosids { get; set; } = new List<int>();
    }
}
