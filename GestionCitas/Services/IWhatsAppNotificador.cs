using System;
using System.Threading.Tasks;

public interface IWhatsAppNotificador
{    Task EnviarConfirmacionCitaAsync(string telefonoCliente, string nombreCliente, DateTime? fechaCita);
}