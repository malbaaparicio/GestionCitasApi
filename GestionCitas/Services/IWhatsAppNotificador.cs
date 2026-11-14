using System;
using System.Threading.Tasks;

public interface IWhatsAppNotificador
{    Task EnviarConfirmacionCitaAsync(int negocioid, string telefonoCliente, string nombreCliente, DateTime? fechaCita);
}