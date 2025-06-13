using MailAppMAUI.UseCases;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using MailAppMAUI.Core;
using MailAppMAUI.Repositorios;
using MailAppMAUI.Contexto;
using MailAppMAUI.Gestion;
using MailAppMAUI.General;
using Syncfusion.Blazor.PivotView;
using Logger = Ifs.Comun.Logger;


namespace MailAppMAUI.UseCases
{
    public class GenerarRespuestas : IGenerarRespuestas
    {
        private static readonly string apiKey = "sk-a32083fe444b45e48992fb9491a9df8c";
        private static readonly string organizationId = "infoser_app";
        private static readonly string apiUrl = "https://api.deepseek.com/v1/chat/completions";

        //private DesglosarFacturasIA _desglosarFacturas;
        private readonly IServiceScopeFactory scopeFactory;

        // Cambiar para recibir la interfaz en lugar de la implementación concreta
        public GenerarRespuestas(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            //_desglosarFacturas = new DesglosarFacturasIA();
        }

        public List<Task<Respuesta>> GenerarRespuestasIA(List<Correo> correos)
        {
            //Crea una lista de respuestas
            List<Task<Respuesta>> misRespuestas = new List<Task<Respuesta>>();

            //Itera sobre todos los correos y genera una respuesta por cada uno
            foreach (Correo correo in correos)
            {
                misRespuestas.Add(GenerarRespuestaIA(correo, null));
            }

            return misRespuestas;
        }
        public async Task<Respuesta> GenerarRespuestaIA(Correo correo, string ctx)
        {
            Contacto contact = null;
            using (var scope = scopeFactory.CreateScope())
            {
                // Crear un contexto scoped a mano
                var contextoBd = scope.ServiceProvider.GetRequiredService<Context>();

                // Crear manualmente una instancia de RepositoryManager usando ese contexto
                var repositoryManager = new RepositoryManager(contextoBd);

                contact = ObtenerContacto(correo.Destinatarios.FirstOrDefault(), repositoryManager);


                if (correo.Procesado)
                {
                    if (correo.RespuestaId.HasValue)
                    {
                        return repositoryManager.RespuestaRepository.GetById(correo.RespuestaId.Value);
                    }
                }
            }
            var formalidad = GetFormalidad(contact);


            if (correo.Adjuntos.Count > 0)
            {
                //await CheckIfPDF(correo);
            }

            using (var client = new HttpClient())
            {
                // Configura la clave de la API en los encabezados
                // Añadir cabeceras de autenticación
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("DeepSeek-Organization", organizationId); // nombre de cabecera según plataforma

                var promptBase = $"Contesta a este correo de {correo.Remitente}: {correo.Cuerpo}";

                var prompt = promptBase + ctx + contact?.Descripcion;

                var context = $"Eres un asistente útil de emails. {formalidad}. Intenta ser siempre apaciguador." +
                    $"Debes cumplir con todas las instruciones del promp con fidelidad";


                // Crea el cuerpo de la petición (JSON)
                var requestBody = new
                {
                    model = "deepseek-chat",
                    messages = new[]
                    {
                         new { role = "system", content = context },
                         new { role = "user", content = prompt }
                     },
                    max_tokens = 150, // **Ajustar, de momento prueba
                    temperature = 0.7
                };

                // Convierte el objeto a JSON
                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);

                // Realiza la solicitud POST
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content); //QUITAR CONF AWAIT SI NO FUFA

                try
                {
                    // Si la solicitud es exitosa, procesa la respuesta
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Solicitud incorrecta. Respuesta no proporcionada");
                    }
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Extrae la respuesta del modelo desde el JSON de respuesta y la deserializa en mi clase OpenAIResponse (por ahora está escrita abajo)
                    var jsonResponse = JsonConvert.DeserializeObject<OpenAiResponseCopia>(responseBody);
                    string respuestaTexto = jsonResponse?.choices?.FirstOrDefault()?.message?.content?.Trim() ?? "Error al generar respuesta";


                    //var asunto = ObtenerAsuntoDeCorreo(respuestaTexto);
                    var cuerpoSinAsunto = ObtenerCuerpoSinAsunto(respuestaTexto);

                    using (var scope = scopeFactory.CreateScope())
                    {
                        // Crear un contexto scoped a mano
                        var contextoBd = scope.ServiceProvider.GetRequiredService<Context>();

                        // Crear manualmente una instancia de RepositoryManager usando ese contexto
                        var repositoryManager = new RepositoryManager(contextoBd);
                        // Guardamos respuesta en repositorio
                        Respuesta miRespuesta = Respuesta.CreateRespuesta(correo, correo.Destinatarios.FirstOrDefault() ?? string.Empty, new List<string> { correo.Remitente }, $"{correo.Asunto}", cuerpoSinAsunto, cuerpoSinAsunto, correo.FechaRecibido);

                        //miRespuesta.SetConver(correo.ConversacionId);
                        //Asocio el nombre del destinatario
                        miRespuesta.ChangeNombreDestinatario(correo.Remitente);

                        miRespuesta.ChangeEsEliminado(false);

                        await repositoryManager.RespuestaRepository.AddAsync(miRespuesta);

                        //hacer un set de respuesta publico
                        correo.SetRespuesta(miRespuesta);

                        // Devolvemos la respuesta
                        return miRespuesta;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogLine("GenerarRespuestas", "Error al GENERAR resuesta", "");
                    return Respuesta.CreateRespuesta(correo, correo.Destinatarios.FirstOrDefault() ?? string.Empty, new List<string> { correo.Remitente }, "Respuesta NO Generada", "Respuesta NO Generada", "null", DateTime.Now);
                }
            }
        }

        /// <summary>
        /// Método para regenerar la respuesta de la IA
        /// </summary>
        /// <param name="correo">Correo original a renegerar</param>
        /// <param name="ctx">Más información sobre cómo regenerarlo</param>
        /// <returns></returns>
        public async Task<Respuesta> RegenerarRespuestaIA(Correo correo, string ctx)
        {
            Contacto contact = null;
            using (var scope = scopeFactory.CreateScope())
            {
                // Crear un contexto scoped a mano
                var contextoBd = scope.ServiceProvider.GetRequiredService<Context>();

                // Crear manualmente una instancia de RepositoryManager usando ese contexto
                var repositoryManager = new RepositoryManager(contextoBd);

                contact = ObtenerContacto(correo.Destinatarios.FirstOrDefault(), repositoryManager);
            }

            var formalidad = GetFormalidad(contact);

            using (var client = new HttpClient())
            {
                // Configura la clave de la API en los encabezados
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Add("DeepSeek-Organization", organizationId); // nombre de cabecera según plataforma

                var promptBase = $"Contesta a este correo de {correo.Remitente}: {correo.Cuerpo}";

                var prompt = promptBase + ctx + contact?.Descripcion;

                var context = $"Eres un asistente útil de emails. {formalidad}. Intenta ser siempre apaciguador." +
                    $"Debes cumplir con todas las instruciones del promp con fidelidad";

                // Crea el cuerpo de la petición (JSON)
                var requestBody = new
                {
                    model = "deepseek-chat",
                    messages = new[]
                    {
                         new { role = "system", content = context },
                         new { role = "user", content = prompt }
                     },
                    max_tokens = 150,
                    temperature = 0.7
                };

                // Convierte el objeto a JSON
                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);

                // Realiza la solicitud POST
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content); //QUITAR CONF AWAIT SI NO FUFA

                try
                {
                    // Si la solicitud es exitosa, procesa la respuesta
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Solicitud incorrecta. Respuesta no proporcionada");
                    }
                    var responseBody = await response.Content.ReadAsStringAsync();

                    // Extrae la respuesta del modelo desde el JSON de respuesta y la deserializa en mi clase OpenAIResponse (por ahora está escrita abajo)
                    var jsonResponse = JsonConvert.DeserializeObject<OpenAiResponseCopia>(responseBody);
                    string respuestaTexto = jsonResponse?.choices?.FirstOrDefault()?.message?.content?.Trim() ?? "Error al generar respuesta";


                    //var asunto = ObtenerAsuntoDeCorreo(respuestaTexto);
                    var cuerpoSinAsunto = ObtenerCuerpoSinAsunto(respuestaTexto);

                    Respuesta miRespuesta;

                    using (var scope = scopeFactory.CreateScope())
                    {
                        // Crear un contexto scoped a mano
                        var contextoBd = scope.ServiceProvider.GetRequiredService<Context>();

                        // Crear manualmente una instancia de RepositoryManager usando ese contexto
                        var repositoryManager = new RepositoryManager(contextoBd);
                        // Guardamos respuesta en repositorio
                        miRespuesta = Respuesta.CreateRespuesta(correo, correo.Remitente, new List<string>(correo.Destinatarios), $"{correo.Asunto}", cuerpoSinAsunto, cuerpoSinAsunto, correo.FechaRecibido);

                    }
                    //hacer un set de respuesta publico
                    //correo.SetRespuesta(miRespuesta);

                    // Devolvemos la respuesta
                    return miRespuesta;

                }
                catch (Exception ex)
                {
                    Logger.LogLine("GenerarRespuestas", "Error al REGENERAR resuesta", "");
                    return Respuesta.CreateRespuesta(correo, correo.Remitente, new List<string>(correo.Destinatarios), "Respuesta NO Generada", "Respuesta NO Generada", "null", DateTime.Now);
                }
            }
        }

        /// <summary>
        /// Comprueba si es un PDF
        /// </summary>
        /// <param name="correoAVerAdjuntos"></param>
        /// <returns></returns>
        private async Task CheckIfPDF(Correo correoAVerAdjuntos)
        {
            List<Adjunto> adjuntos = correoAVerAdjuntos.Adjuntos;

            foreach (Adjunto adjuntoPDF in adjuntos)
            {
                if (Path.GetExtension(adjuntoPDF.Ruta).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    // await _desglosarFacturas.DesglosarPDF(adjuntoPDF, correoAVerAdjuntos);
                }
            }
        }

        private Contacto ObtenerContacto(string mail, RepositoryManager repositoryManager)
        {
            return repositoryManager.ContactoRepository.GetByEmail(mail);
        }
        private string GetFormalidad(Contacto contact)
        {
            switch (contact.Tipo)
            {
                case TipoContacto.Formal:
                    return "Debes contestar formalmente";

                case TipoContacto.Informal:
                    return "Debes contestar informalmente";

                case TipoContacto.Desconocido:
                    return "Debes contestar formalmente";
            }

            return "Debes contestar formalmente";
        }

        /// <summary>
        /// Obtiene el asunto del mensaje
        /// </summary>
        /// <param name="cuerpoCorreo"></param>
        /// <returns></returns>
        private string ObtenerAsuntoDeCorreo(string cuerpoCorreo)
        {
            // Dividir el cuerpo del correo en líneas y obtener la primera línea
            var lineas = cuerpoCorreo.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            return lineas.Length > 0 ? lineas[0].Trim() : "Respuesta Generada";  // Si no hay líneas, asigna un asunto predeterminado
        }

        /// <summary>
        /// Obtiene el cuerpo del mensaje
        /// </summary>
        /// <param name="cuerpoCorreo"></param>
        /// <returns></returns>
        private string ObtenerCuerpoSinAsunto(string cuerpoCorreo)
        {
            var lineas = cuerpoCorreo.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            // Si hay más de una línea, unimos todas las líneas a partir de la segunda
            return string.Join("\n", lineas.Skip(1));
        }
    }

    public class OpenAiResponseCopia
    {
        public List<ChoiceCopia> choices { get; set; }
    }

    public class ChoiceCopia
    {
        public MessageCopia message { get; set; }
    }

    public class MessageCopia
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
