using AutoMapper;

namespace GestionCitas.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Aquí puedes definir tus mapeos entre entidades y DTOs
            // CreateMap<Source, Destination>();
            //Cliente
            CreateMap<Models.Cliente, DTOs.ClienteGetDto>();
            CreateMap<DTOs.ClienteCreateOrUpdateDto, Models.Cliente>();
            //Servicio
            CreateMap<Models.Servicio, DTOs.ServicioGetDto>();
            CreateMap<DTOs.ServicioCreateOrUpdateDto, Models.Servicio>();
            //Cita
            CreateMap<Models.Cita, DTOs.CitaGetDto>();
            CreateMap<DTOs.CitaCreateOrUpdateDto, Models.Cita>();
            //precio_servicio
            CreateMap<Models.precio_servicio, DTOs.PrecioServicioGetDto>();
            CreateMap<DTOs.PrecioServicioCreateOrUpdateDto, Models.precio_servicio>();
            //Cita_Servicio
            CreateMap<Models.Cita_Servicio, DTOs.CitaServicioGetDto>();
            CreateMap<DTOs.CitaServicioCreateOrUpdateDto, Models.Cita_Servicio>();
        }
    }
}
