using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class MetaWhatsAppService : IWhatsAppNotificador
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<MetaWhatsAppService> _logger;

    public MetaWhatsAppService(HttpClient httpClient, IConfiguration config, ILogger<MetaWhatsAppService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task EnviarConfirmacionCitaAsync(string telefonoCliente, string nombreCliente, DateTime? fechaCita)
    {
        try
        {
            var token = _config["MetaWhatsApp:AccessToken"];
            var phoneNumberId = _config["MetaWhatsApp:PhoneNumberId"];

            // Meta exige el número con prefijo de país pero sin el símbolo '+' (ej: 34670403589)
            var numeroLimpio = telefonoCliente.Replace(" ", "").TrimStart('+');

            var url = $"https://graph.facebook.com/v25.0/{phoneNumberId}/messages";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                messaging_product = "whatsapp",
                to = numeroLimpio,
                type = "template",
                template = new
                {
                    name = "hello_world",
                    language = new { code = "en_US" }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"\n ✅ WHATSAPP ENVIADO CON ÉXITO A {numeroLimpio} \n");
                _logger.LogInformation($"WhatsApp enviado con éxito vía Meta a {numeroLimpio}");
            }
            else
            {
                var detalleError = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"\n ❌ ERROR DE META: {detalleError} \n");
                _logger.LogError($"Error de Meta al enviar WhatsApp: {detalleError}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n ❌ EXCEPCIÓN HTTP: {ex.Message} \n");
            _logger.LogError(ex, $"Fallo crítico al enviar WhatsApp vía Meta a {telefonoCliente}");
        }
    }
}