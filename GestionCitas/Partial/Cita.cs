using GestionCitas.Common;
using GestionCitas.Models;

namespace GestionCitas.Models
{
    public partial class Cita : IMustHaveTenant
    {
        public int NegocioId 
        {           
            get => negocioid; // Mapea a la propiedad generada por scaffold
            set => negocioid = value;
        }
    }
}
