using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GestionCitas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class MetaWhatsAppService : IWhatsAppNotificador
{
    private readonly HttpClient _httpClient;
    private readonly GestionCitasContext _context;
    private readonly ILogger<MetaWhatsAppService> _logger;

    public MetaWhatsAppService(HttpClient httpClient, GestionCitasContext context, ILogger<MetaWhatsAppService> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
    }

    public async Task EnviarConfirmacionCitaAsync(int negocioid, string telefonoCliente, string nombreCliente, DateTime? fechaCita)
    {
        try
        {            
            var negocio = await _context.negocios.FindAsync(negocioid);
            // Si el negocio no existe o no tiene WhatsApp configurado, salimos sin lanzar error
            if (negocio == null ||
                string.IsNullOrWhiteSpace(negocio.conf_whatsapp_number_id) ||
                string.IsNullOrWhiteSpace(negocio.conf_whatsapp_token))
            {
                _logger.LogInformation($"El negocio {negocioid} no tiene credenciales de WhatsApp configuradas. Se omite el envío.");
                return;
            }

            var token = negocio.conf_whatsapp_token;
            var phoneNumberId = negocio.conf_whatsapp_number_id;

            // Meta exige el número con prefijo de país pero sin el símbolo '+' (ej: 34670403589)
            var numeroLimpio = telefonoCliente.Replace(" ", "").TrimStart('+');

            var url = $"https://graph.facebook.com/v25.0/{phoneNumberId}/messages";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", negocio.conf_whatsapp_token);                      

            var payload = new
            {
                messaging_product = "whatsapp",
                to = numeroLimpio,
                type = "template",
                template = new
                {
                    name = "confirmacion_cita",
                    language = new { code = "es" },
                    components = new[]
                    {
                        new
                        {
                            type = "body",
                            parameters = new[]
                            {
                                new { type = "text", text = nombreCliente },
                                new { type = "text", text = fechaCita?.ToString("dd/MM/yyyy") ?? "" },
                                new { type = "text", text = fechaCita?.ToString("HH:mm") ?? "" }
                            }
                        }
                    }
                }
            };

            request.Content = JsonContent.Create(payload);
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"WhatsApp enviado con éxito vía Meta a {numeroLimpio} (Negocio ID: {negocioid})");
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