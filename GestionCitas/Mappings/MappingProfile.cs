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
            CreateMap<Models.Cita, DTOs.CitaGetDto>()
                .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src => src.cliente.nombre)) // Mapeo manual
                .ForMember(dest => dest.NombreEmpleado, opt => opt.MapFrom(src => src.empleado.nombre)) // Mapeo manual;
                .ForMember(dest => dest.Servicios, opt => opt.MapFrom(src => src.cita_servicios)) // Mapeo manual para servicios
                .ForMember(dest => dest.clienteid, opt => opt.MapFrom(src => src.clienteid)) // Mapeo manual para idCliente
                .ForMember(dest => dest.empleadoid, opt => opt.MapFrom(src => src.empleadoid)); // Mapeo manual para idEmpleado

            CreateMap<DTOs.CitaUpdateDto, Models.Cita>();
            CreateMap<DTOs.CitaCreateDto, Models.Cita>();
               
            //Cita_Servicio
            CreateMap<Models.Cita_Servicio, DTOs.CitaServicioGetDto>();
            CreateMap<DTOs.CitaServicioCreateOrUpdateDto, Models.Cita_Servicio>();
            //Empleado
            CreateMap<Models.Empleado, DTOs.EmpleadoGetDto>();
            CreateMap<DTOs.EmpleadoCreateOrUpdateDto, Models.Empleado>();
        }
    }
}
