namespace GestionCitas.Common
{
    public class CurrentTenantService : ICurrentTenantService
    {       
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Inyectamos el contexto HTTP para acceder a la petición actual
        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? NegocioId
        {
            get
            {
                // Buscamos dentro de los "Claims" (atributos) del token el que se llama "negocioid"
                var tenantClaim = _httpContextAccessor.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == "negocioid")?.Value;

                // Si lo encontramos y es un número, lo devolvemos
                if (!string.IsNullOrEmpty(tenantClaim) && int.TryParse(tenantClaim, out int tenantId))
                {
                    return tenantId;
                }

                // Si por lo que sea el token no lo trae (ej. súper admin), devolvemos null
                return null;
            }
        }
    }
}
