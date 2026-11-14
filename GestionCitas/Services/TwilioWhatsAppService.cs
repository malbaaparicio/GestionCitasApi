using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioWhatsAppService : IWhatsAppNotificador
{
    private readonly IConfiguration _config;
    private readonly ILogger<TwilioWhatsAppService> _logger;

    public TwilioWhatsAppService(IConfiguration config, ILogger<TwilioWhatsAppService> logger)
    {
        _config = config;
        _logger = logger;

        // Inicializamos el cliente global de Twilio
        TwilioClient.Init(_config["Twilio:AccountSid"], _config["Twilio:AuthToken"]);
    }

    public async Task EnviarConfirmacionCitaAsync(int negocioid, string telefonoCliente, string nombreCliente, DateTime? fechaCita)
    {
        try
        {
            // Twilio exige el prefijo "whatsapp:" antes del número
            // Asumimos que telefonoCliente viene con el +34 (ej: "+34600123456")
            var to = new PhoneNumber($"whatsapp:{telefonoCliente}");
            var from = new PhoneNumber(_config["Twilio:WhatsAppNumber"]);

            var mensaje = $"Hola {nombreCliente}, te confirmamos tu cita para el {fechaCita:dd/MM/yyyy} a las {fechaCita:HH:mm}. ¡Te esperamos!";

            var message = await MessageResource.CreateAsync(
                contentSid: mensaje,
                body: mensaje,
                from: from,
                to: to
            );

            _logger.LogInformation($"WhatsApp enviado con éxito a {telefonoCliente}. SID: {message.Sid}");
        }
        catch (Exception ex)
        {
            // Esto imprimirá el error en rojo en la consola de Visual Studio
            Console.WriteLine($"\n ❌ ERROR DE TWILIO: {ex.Message} \n");
            _logger.LogError(ex, $"Fallo crítico al enviar WhatsApp a {telefonoCliente}");
        }
    }

   
}