using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System.Net;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MailKit.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using MailKit.Security;
using MailAppMAUI.Config;
using MailAppMAUI.Core;
using MailAppMAUI.Repositorios;
using MailAppMAUI.Contexto;
using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.Maui.Storage;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Cryptography;
using Logger = Ifs.Comun.Logger;


namespace MailAppMAUI.UseCases.Services.ConcreteServices
{
    public class ReadEmailService : IService
    {
        //RUTA DE GUARDAR ARCHIVOS
        static Configuration Conf { get; set; }

        //PARA SUBIRTLO AL SERVIDOR
        private static readonly HttpClient _httpClient = new HttpClient();

        //IMAP CONNECTION
        private ImapClient client;
        private bool isConnected = false;

        //CONSTRUCTORES
        private readonly IServiceScopeFactory scopeFactory;

        public ReadEmailService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public ReadEmailService() { }


        /// <summary>
        /// --
        /// </summary>
        public IService OpenService(string service)//Meter poca cosa porque si cada vez que se añade un servicio se llama aquí, estoy manteniendo la conexion IMAP desde que lo añado hasta que lo ejecuto.
        {
            if ((Conf = Configuration.Config) == null)
            {
                Conf = new Configuration();
            }

            return this;
        }
        /// <summary>
        /// Lee todos los mensajes nuevos y los procesa
        /// </summary>
        /// <param name="action">No sé</param>
        /// <param name="info">Informacion para realizar la ejecución del servicio (cantidad de mails a mostrar[0])</param>
        public async Task<object> Execute(object action, object[] info)
        {
            try
            {
                string email;
                string password;
                int cantidad = (int)info[0];

                string imapServer = Conf.User.ImapConexion;//EmailConfig.Instance().GetImapServer();//mejor esta --> Conf.EmailConf.ImapConexion;
                int imapPort = Conf.User.ImapPort; //EmailConfig.Instance().GetImapPort();//mejor esta --> Conf.EmailConf.ImapPort;

                client = new ImapClient();
                client.Connect(imapServer, imapPort, true); // Cambia por tu servidor IMAP

                //GoogleLogin
                if (info.Count() > 1)
                {
                    Usuario user = (Usuario)info[1];
                    email = user.Email;
                    password = user.Token; // Password = token aqui

                    var oauth2 = new SaslMechanismOAuth2(email, password);
                    await client.AuthenticateAsync(oauth2);
                }
                else //Normal Login
                {
                    email = Conf.User.Email;
                    password = Conf.User.Password;

                    client.Authenticate(new System.Net.NetworkCredential(email, password)); // Usa credenciales seguras
                }


                if (string.IsNullOrEmpty(imapServer))
                {
                    return "No se encontró un servidor IMAP para este dominio.";
                    return null;
                }

                isConnected = true;

                if (!isConnected)
                    return "Servicio no conectado";

                var emails = new List<Correo>();
                var inbox = client.Inbox;
                // Forzar sincronización completa
                inbox.Open(FolderAccess.ReadWrite);
                client.NoOp();

                // Obtener todos los UIDs (método más confiable)
                var twoWeeksAgo = DateTime.UtcNow.AddMonths(-3);
                var allUids = inbox.Search(SearchQuery.DeliveredAfter(twoWeeksAgo));
                using (var scope = scopeFactory.CreateScope())
                {

                    // Crear un contexto scoped a mano
                    var context = scope.ServiceProvider.GetRequiredService<Context>();

                    // Crear manualmente una instancia de RepositoryManager usando ese contexto
                    var repositoryManager = new RepositoryManager(context);

                    //Que aparezcan de mas nuevos a mas antiguos para asi poder trabajar mientras se carga el resto
                    foreach (var uid in allUids.Reverse()) // Para mantener el orden de nuevo a viejo
                    {
                        if (inbox.IsOpen == false)
                        {
                            inbox.Open(FolderAccess.ReadWrite);
                        }
                        var mensaje = inbox.GetMessage(uid);


                        //Si está en el repositorio
                        if (repositoryManager.CorreoRepository.ExistMensaje(mensaje.MessageId))
                        {
                            continue;
                        }

                        //Si no está en el repositorio
                        else
                        {
                            Correo correoProcesado = await ProcessMessage(mensaje, repositoryManager);

                            emails.Add(correoProcesado);
                        }
                    }
                }
                return emails;
            }
            catch (Exception ex)
            {
                Logger.LogLine("ReadEmailService", "Error al LEER mails " + ex.Message, "");

                return null;
            }
        }

        /// <summary>
        /// Procesa el correo asociandole un usuario y añadiendolo al repositorio de correos.
        /// </summary>
        /// <param name="mensaje">Mensaje para coger el correo que usará para procesar</param>
        private async Task<Correo> ProcessMessage(MimeMessage mensaje, RepositoryManager repositoryManager)
        {

            //Obtiene el correo en base al MimeMessage
            Correo miCorreo = repositoryManager.CorreoRepository.GetCorreoByMimessage(mensaje.MessageId);

            //Obtiene el contacto del email
            Contacto? contacto = GetContactoEmailAsync(mensaje, repositoryManager).Result;

            if (contacto.Nombre == null || contacto.Nombre == "")
            {
                contacto.SetNombre("Desconocido");
            }

            //Convierte el MimeMessage en CorreoCore
            Correo correo = Correo.ConvertToCore(mensaje);

            ////Busca si hay attachment en cuyo caso, los sube al servidor
            //if (mensaje.Attachments.Any())
            //{
            //    foreach (var attachment in mensaje.Attachments)
            //    {
            //        if (attachment is MimePart part)
            //        {
            //            Console.WriteLine($"   - {part.FileName}");
            //            Adjunto miAdjunto = Adjunto.CreateAdjunto(correo, Conf.Paths.DirAdjuntos + $@"\{part.FileName}");

            //            SubirAdjuntoAlServidor(part);

            //            correo.Adjuntos.Add(miAdjunto);
            //        }
            //    }
            //}

            if (mensaje.Attachments.Any())
            {
                foreach (var attachment in mensaje.Attachments)
                {
                    if (attachment is MimePart part)
                    {
                        string nombreArchivo = part.FileName;
                        string rutaLocal = Path.Combine(Conf.Paths.DirAdjuntos, nombreArchivo);

                        // Crear directorio si no existe
                        if (!Directory.Exists(Conf.Paths.DirAdjuntos))
                            Directory.CreateDirectory(Conf.Paths.DirAdjuntos);

                        // Guardar archivo localmente
                        using (var fileStream = File.Create(rutaLocal))
                        {
                            await part.Content.DecodeToAsync(fileStream);
                        }

                        // Subir al servidor (opcional)
                        await SubirAdjuntoAlServidor(part);

                        // Crear el objeto adjunto con la ruta local
                        Adjunto miAdjunto = Adjunto.CreateAdjunto(correo, rutaLocal);

                        // Agregar el adjunto al correo
                        correo.Adjuntos.Add(miAdjunto);
                    }
                }
            }

            string html = ProcesarEmbebidos(mensaje);
            //correo.Adjuntos.AddRange(imagenesEmbebidas);

            correo.SetUsuario(repositoryManager.UsuarioRepository.GetById(Conf.User.UserId));

            correo.SetCuepoHTML(html);

            repositoryManager.CorreoRepository.AddAsync(correo, mensaje.MessageId);

            //emails.Add(correo);
            return correo;

        }


        private Correo? BuscarPorMessageId(string messageId, RepositoryManager repositoryManager)
        {
            //Busco si está ese menssageId en mi BD (ha sido mandando desde mi app) --> Alguien lo tiene
            Correo correo = repositoryManager.CorreoRepository.GetAll().Where(c => c.MensajeId == messageId).FirstOrDefault();
            Respuesta res = repositoryManager.RespuestaRepository.GetAll().Where(c => c.MensajeId == messageId).FirstOrDefault();

            if (correo != null)
            {
                return correo;
            }
            else if (res != null)
            {
                correo = (Correo)res; //Pasamos la respuesta a correo para que se pueda ver desde la bandeja de entrada
                correo.SetUsuario(repositoryManager.UsuarioRepository.GetById(Conf.User.UserId));
                Contacto cont = GetContactoEmailAsync(res.Remitente, repositoryManager);

                return correo;
            }
            else //No se ha enviado desde nuestra app --> O ya estaba cuando se cargan los correos o lo han enviado desde otro lado fuera de la app
            {
                //Esto significa que el correo que estamos intentando buscar que aparece referenciado pero NO está en inbox, estará en enviados del cliente

                var sentFolder = client.GetFolder(SpecialFolder.Sent);
                sentFolder.Open(FolderAccess.ReadOnly);

                var query = SearchQuery.HeaderContains("Message-Id", messageId);
                var uids = sentFolder.Search(query);

                if (uids != null)
                {
                    foreach (var uid in uids)
                    {
                        var mensaje = sentFolder.GetMessage(uid);

                        if (mensaje.MessageId == messageId)
                        {
                            var remitente = mensaje.From.Mailboxes.FirstOrDefault();
                            var contacto = repositoryManager.ContactoRepository.GetByEmail(remitente?.Address ?? "");

                            Correo correoDesdeImap = Correo.ConvertToCore(mensaje, contacto);
                            correoDesdeImap.SetUsuario(repositoryManager.UsuarioRepository.GetById(Conf.User.UserId));

                            if (mensaje.Attachments.Any())
                            {
                                foreach (var attachment in mensaje.Attachments)
                                {
                                    if (attachment is MimePart part)
                                    {
                                        Adjunto adj = Adjunto.CreateAdjunto(correoDesdeImap, Conf.Paths.DirAdjuntos + $@"\{part.FileName}");
                                        SubirAdjuntoAlServidor(part);
                                        correoDesdeImap.Adjuntos.Add(adj);
                                    }
                                }
                            }
                            sentFolder.Close();
                            return correoDesdeImap;
                        }
                    }
                }
                else
                {
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    query = SearchQuery.HeaderContains("Message-Id", messageId);
                    uids = inbox.Search(query);

                    foreach (var uid in uids)
                    {
                        var mensaje = inbox.GetMessage(uid);

                        // Comparar el Message-Id exacto
                        if (mensaje.MessageId == messageId)
                        {
                            var remitente = mensaje.From.Mailboxes.FirstOrDefault();
                            var contacto = repositoryManager.ContactoRepository.GetByEmail(remitente.Address);
                            Correo correoDesdeImap = Correo.ConvertToCore(mensaje, contacto);
                            correoDesdeImap.SetUsuario(repositoryManager.UsuarioRepository.GetById(Conf.User.UserId));

                            if (mensaje.Attachments.Any())
                            {
                                foreach (var attachment in mensaje.Attachments)
                                {
                                    if (attachment is MimePart part)
                                    {
                                        Adjunto adj = Adjunto.CreateAdjunto(correoDesdeImap, Conf.Paths.DirAdjuntos + $@"\{part.FileName}");
                                        SubirAdjuntoAlServidor(part);
                                        correoDesdeImap.Adjuntos.Add(adj);
                                    }
                                }
                            }
                            inbox.Close();
                            return correoDesdeImap;
                        }
                    }
                }
            }
            try
            {

            }
            catch (Exception ex)
            {

            }

            if (res != null)
            {
                Correo correoDesdeImap = (Correo)res;


                return correoDesdeImap;
            }
            else if (correo != null)
            {
                return correo;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene el Contacto del MimeMessage. Si no esta creado crea uno nuevo
        /// </summary>
        /// <param name="correo">Correo a procesar de tipo <see cref="MimeMessage"/></param>
        /// <returns>Contacto del correo <see cref="Contacto"/><</returns>
        private async Task<Contacto?> GetContactoEmailAsync(MimeMessage correo, RepositoryManager repositoryManager)
        {
            var remitente = correo.From.Mailboxes.FirstOrDefault();

            Usuario _usuario = repositoryManager.UsuarioRepository.GetById(Conf.User.UserId);

            if (remitente == null || string.IsNullOrEmpty(remitente.Address))
            {
                return null;
            }
            var contacto = repositoryManager.ContactoRepository.GetByEmail(remitente.Address);


            if (_usuario != null)
            {
                if (contacto?.UsuarioId == _usuario.UsuarioId) //Este Usuario ya tiene este Contacto
                {
                    return contacto;
                }
                else //Si no tiene ese Usuario, lo crea
                {
                    contacto = Contacto.CreateContacto(remitente.Address, _usuario.UsuarioId, remitente.Name);
                    await repositoryManager.ContactoRepository.AddAsync(contacto, false);
                }
            }


            return contacto;
        }

        /// <summary>
        /// Obtiene el Contacto del MimeMessage. Si no esta creado crea uno nuevo
        /// </summary>
        /// <param name="correo">Correo a procesar de tipo <see cref="MimeMessage"/></param>
        /// <returns>Contacto del correo <see cref="Contacto"/><</returns>
        private Contacto GetContactoEmailAsync(string email, RepositoryManager repositoryManager)
        {
            var remitente = email;

            Usuario _usuario = repositoryManager.UsuarioRepository.GetById(Conf.User.UserId);

            if (remitente == null)
            {
                return null;
            }
            var contacto = repositoryManager.ContactoRepository.GetByEmail(remitente);

            //Si no esta en el repositorio, crea un contacto nuevo

            if (_usuario != null)
            {
                if (contacto?.UsuarioId == _usuario.UsuarioId) //Este Usuario ya tiene este Contacto
                {
                    return contacto;
                }
                else
                {
                    contacto = Contacto.CreateContacto(remitente, _usuario.UsuarioId, "Desconocido");

                    repositoryManager.ContactoRepository.AddAsync(contacto);
                }

            }

            return contacto;
        }

        /// <summary>
        /// Cierra la conexión IMAP
        /// </summary>
        public bool CloseService()
        {
            try
            {
                if (client != null && isConnected)
                {
                    client.Disconnect(true);
                    isConnected = false;
                    Console.WriteLine("Desconectado del servidor IMAP.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar la conexión: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sube los adjuntos al servidor cambiandoles el formato
        /// </summary>
        /// <param name="adjunto">Mimepart para subir al servidor</param>
        /// <returns>true if done</returns>
        private async Task SubirAdjuntoAlServidor(MimePart adjunto)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await adjunto.Content.DecodeToAsync(memoryStream);
                    memoryStream.Position = 0; // Reiniciar el stream

                    var fileContent = new StreamContent(memoryStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(adjunto.ContentType.MimeType);

                    using (var form = new MultipartFormDataContent())
                    {
                        form.Add(fileContent, "UploadFiles", adjunto.FileName); //Se transforma al formato para que lo pueda recibir bien el metodo SAVE del controlador

                        string apiUrl = "http://localhost:5002/api/SampleData/Save"; // Cambia esto cuando se haga en servidor
                        HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, form);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($" Archivo '{adjunto.FileName}' subido exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine($" Error al subir '{adjunto.FileName}': {response.ReasonPhrase}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error al procesar adjunto '{adjunto.FileName}': {ex.Message}");
            }
        }


        private string ProcesarEmbebidos(MimeMessage mensaje)
        {
            if (mensaje == null || string.IsNullOrEmpty(mensaje.HtmlBody))
                return mensaje?.HtmlBody;

            string html = mensaje.HtmlBody;
            var embebidos = ObtenerEmbebidos(mensaje.Body);

            foreach (var embed in embebidos)
            {
                string cid = embed.ContentId?.Trim('<', '>');

                if (string.IsNullOrEmpty(cid))
                    continue;

                try
                {
                    using var memoryStream = new MemoryStream();
                    embed.Content.DecodeTo(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    string base64 = Convert.ToBase64String(imageBytes);

                    string mimeType = embed.ContentType?.MimeType ?? "application/octet-stream";
                    string dataUri = $"data:{mimeType};base64,{base64}";

                    html = html.Replace($"cid:{cid}", dataUri);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando embebido CID={cid}: {ex.Message}");
                }
            }

            return html;
        }

        private List<MimePart> ObtenerEmbebidos(MimeEntity entidad)
        {
            var embebidos = new List<MimePart>();

            if (entidad is Multipart multipart)
            {
                foreach (var subParte in multipart)
                {
                    embebidos.AddRange(ObtenerEmbebidos(subParte));
                }
            }
            else if (entidad is MimePart parte)
            {
                string disposition = parte.ContentDisposition?.Disposition;
                if (!string.IsNullOrEmpty(parte.ContentId) &&
                    (disposition == null || disposition.Equals("inline", StringComparison.OrdinalIgnoreCase)))
                {
                    embebidos.Add(parte);
                }
            }

            return embebidos;
        }
    }
}
