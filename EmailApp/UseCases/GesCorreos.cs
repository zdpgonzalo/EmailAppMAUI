using MailAppMAUI.Config;
using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using MailAppMAUI.General;
using MailAppMAUI.Gestion;
using MailAppMAUI.Core;
using MailAppMAUI.Repositorios;
using MailAppMAUI.UseCases.Services;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Logger = Ifs.Comun.Logger;

namespace MailAppMAUI.UseCases
{
    public class GesCorreos : GesBase<GesCorreos, GesCorreos.Events, GesCorreos.Actions,
                                                GesCorreos.Names, GesCorreos.Tables>
    {
        private RepositoryManager repositoryManager;
        private readonly IGenerarRespuestas generarRespuesta;

        private ServiceManager serviceManager;
        private readonly Context context;
        private DataResul dataResul;

        public static event Action<OpResul>? OnCorreoEnviado;

        static Configuration Conf { get; set; }
        public GesCorreos(RepositoryManager repositoryManager, IGenerarRespuestas generarRespuesta,
            ServiceManager serviceManager, Context context) : base()
        {
            this.serviceManager = serviceManager;
            this.context = context;
            this.repositoryManager = repositoryManager;
            this.generarRespuesta = generarRespuesta;

            if ((Conf = Configuration.Config) == null)
            {
                Conf = new Configuration();
            }

        }

        public enum Tables
        {
            None,
            Usuario,
            Contacto,
            Correo,
            Respuesta,
            Adjunto,
        }

        public enum Names
        {
            None,

            GetCorreos,

            GetConversaciones,

            GetRespuestas,

            GetContactos,

            GetUsuarios,

            GetRespuestaById,

            GetEliminados,

            RespuestaDestinatarios,

            RespuestaAsunto,

            RespuestaNombreDest,

            RespuestaCuerpo,

            RespuestaBorrador,

            DestacarCorreo,

            NoDestacarCorreo,

            UpdateDescripcion,

            UpdateCorreo,

            UpdateTelefono,

            UpdateNombre,

            UpdateTipo,

            GetCorreoById

        }


        public enum Actions
        {
            None,

            RegenerarRespuesta,

            ActualizarRespuesta,

            EnviarRespuesta,

            EliminarCorreo,

            MarcarComoLeido,

            EliminarRespuesta,

            AdjuntarArchivo,

            RespuestaBorrador,

            GuardarRespuestaBaseDatos,

            EliminarContacto,

            EliminarCorreoDefinitivamente,

            GuardarContacto,

            RestaurarEliminado,

            ResponderCorreo,

            DestacarListaCorreos,
            EliminarListaCorreos,
            RestaurarListaEliminados,
            SuprimirListaCorreos,
            EliminarListaContactos,
        }

        public enum Events
        {
            None,

            DocInit,
        }

        /// <summary>
        /// Realiza una accion de la aplicacion
        /// </summary>
        /// <param name="oper">Operacion a realizar</param>
        /// <param name="table">Tabla sobre la que trabajar</param>
        /// <param name="info">Informacion extra</param>
        /// <returns>Resultado de la operacion ejecutada</returns>
        protected override async Task<object> Action(Actions oper, Tables table, object[] info)
        {
            try
            {
                ResetChanges();
                switch (table)
                {
                    case Tables.None:
                        switch (oper)
                        {
                            default:
                                return -1; // Acción no válida en Tables.None
                        }

                    case Tables.Correo:
                        switch (oper)
                        {
                            case Actions.EliminarCorreo:
                                return await EnviarCorreoAPapeleraAction(Data.ToInt(info[0]));

                            case Actions.MarcarComoLeido:
                                return MarcarComoLeidoAction(Data.ToInt(info[0]), Data.ToBool(info[1]));

                            case Actions.EliminarCorreoDefinitivamente:
                                return await EliminarCorreoDefinitivamenteAction(Data.ToInt(info[0]));

                            case Actions.RestaurarEliminado:
                                return await RestaurarElimiando(Data.ToInt(info[0]));

                            case Actions.DestacarListaCorreos:
                                return DestacarListaCorreos((List<int>)info[0], Data.ToBool(info[1]));

                            case Actions.EliminarListaCorreos:
                                return MandarAPapeleraCorreos((List<int>)info[0]);

                            case Actions.RestaurarListaEliminados:
                                return await RestaurarListaEliminados((List<int>)info[0]);

                            case Actions.SuprimirListaCorreos:
                                return await SuprimirListaCorreos((List<int>)info[0]);

                            default:
                                return -1; // Acción no válida en Tables.Correo
                        }

                    case Tables.Respuesta:
                        switch (oper)
                        {
                            case Actions.AdjuntarArchivo:
                                // info[0]: correoId, info[1]: filePath
                                return AdjuntarArchivoAction(info[0], info[1] as List<string>);

                            case Actions.RegenerarRespuesta:
                                return await RegenerarRespuestaAction(Data.ToInt(info[0]), Data.ToString(info[1]));

                            case Actions.EnviarRespuesta:
                                return await EnviarRespuestaAction(info[0]);

                            case Actions.EliminarRespuesta:
                                return await EnviarRespuestaAPapeleraAction(Data.ToInt(info[0]));

                            case Actions.GuardarRespuestaBaseDatos:
                                return await GuardarRespuestaBaseDatos(info[0]);

                            case Actions.ResponderCorreo:
                                return await ResponderCorreoAction(info[0]);

                            default:
                                return -1; // Acción no válida en Tables.Correo
                        }
                    case Tables.Contacto:
                        switch (oper)
                        {
                            case Actions.EliminarContacto:
                                return await EliminarContactoAction(Data.ToInt(info[0]));

                            case Actions.GuardarContacto:
                                return await GuardarContacto(info[0]);

                            case Actions.EliminarListaContactos:
                                return await EliminarListaContactos((List<int>)info[0]);

                            default:
                                return false;
                        }

                    default:
                        return -1; // Tabla no válida
                }
            }
            catch (Exception e)
            {
                WebLog.LogError(e, "No se pueden actualizar datos");
                return -1;
            }

        }

        /// <summary>
        /// Realiza un Set de una propiedad o valor de un item indicado
        /// </summary>
        /// <param name="name">Campo o propiedad a modificar</param>
        /// <param name="table">Tabla de la que cambiar el valor</param>
        /// <param name="item">Objeto a modificar el valor</param>
        /// <param name="value">Valor nuevo de la propiedad</param>
        /// <returns>True si realiza el cambio. False en caso contrario</returns>
        protected override bool SetData(Names name, Tables table, object item, object value)
        {
            try
            {
                ResetChanges();
                switch (table)
                {
                    case Tables.Usuario:
                        switch (name)
                        {
                            default:
                                return false;
                        }

                    case Tables.Correo:
                        Correo correo = repositoryManager.CorreoRepository.GetById(Data.ToInt(item));
                        if (correo == null) return false;
                        switch (name)
                        {
                            case Names.DestacarCorreo:
                                correo.SetDestacado();
                                repositoryManager.CorreoRepository.Update(correo, false);
                                return true;

                            case Names.NoDestacarCorreo:
                                correo.QuitarDestacado();
                                repositoryManager.CorreoRepository.Update(correo, false);
                                return true;

                            default:
                                return false;
                        }

                    case Tables.Respuesta:
                        Respuesta respuesta = repositoryManager.RespuestaRepository.GetById(Data.ToInt(item));
                        if (respuesta == null) return false;
                        switch (name)
                        {
                            case Names.RespuestaCuerpo:
                                respuesta.ChangeCuerpo(Data.ToString(value));
                                repositoryManager.RespuestaRepository.Update(respuesta, false);
                                return true;

                            case Names.RespuestaDestinatarios:
                                respuesta.ChangeDestinatarios((List<string>)value);
                                repositoryManager.RespuestaRepository.Update(respuesta, false);
                                return true;

                            case Names.RespuestaAsunto:
                                respuesta.ChangeAsunto(Data.ToString(value));
                                repositoryManager.RespuestaRepository.Update(respuesta, false);
                                return true;

                            case Names.RespuestaBorrador:
                                respuesta.ChangeBorrador(Data.ToBool(value));
                                repositoryManager.RespuestaRepository.Update(respuesta, false);
                                return true;

                            case Names.RespuestaNombreDest:
                                respuesta.ChangeNombreDestinatario(Data.ToString(value));
                                repositoryManager.RespuestaRepository.Update(respuesta, false);
                                return true;

                            default:
                                return false;
                        }

                    case Tables.Contacto:
                        var contacto = repositoryManager.ContactoRepository.GetById(Data.ToInt(item));
                        if (contacto == null) return false;
                        switch (name)
                        {
                            case Names.UpdateDescripcion:
                                contacto.UpdateDescripcion(Data.ToString(value));
                                repositoryManager.ContactoRepository.Update(contacto, false);
                                return true;

                            case Names.UpdateCorreo:
                                contacto.SetCorreo(Data.ToString(value));
                                repositoryManager.ContactoRepository.Update(contacto, false);
                                return true;

                            case Names.UpdateTelefono:
                                contacto.SetTelefono(Data.ToString(value));
                                repositoryManager.ContactoRepository.Update(contacto, false);
                                return true;

                            case Names.UpdateTipo:
                                contacto.SetTipo(Data.ToString(value));
                                repositoryManager.ContactoRepository.Update(contacto, false);
                                return true;

                            case Names.UpdateNombre:
                                contacto.SetNombre(Data.ToString(value));
                                repositoryManager.ContactoRepository.Update(contacto, false);
                                return true;

                            default:
                                return false;
                        }

                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                WebLog.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// Devuelve datos de las capas inferiores
        /// Devuelve datos de las capas inferiores
        /// </summary>
        /// <param name="name">Nombre de la accion</param>
        /// <param name="table">Tabla a procesar</param>
        /// <param name="item">Informacion extra</param>
        /// <returns>Informacion devuelta</returns>
        protected override object GetData(Names name, Tables table, object item)
        {
            switch (table)
            {
                case Tables.Correo:
                    switch (name)
                    {
                        case Names.None:
                            break;

                        case Names.GetCorreos:
                            return GetCorreos(Data.ToInt(item));

                        case Names.GetConversaciones:
                            return GetConversaciones(Data.ToInt(item));

                        case Names.GetRespuestas:
                            return GetRespuestas(Data.ToInt(item));

                        case Names.GetRespuestaById:
                            return GetRespuestaById(Data.ToInt(item));

                        case Names.GetContactos:
                            return GetContactos(Data.ToInt(item));

                        case Names.GetUsuarios:
                            return GetUsuarios(Data.ToInt(item));

                        case Names.GetEliminados:
                            return GetEliminados(Data.ToInt(item));

                        case Names.GetCorreoById:
                            return GetCorreoById(Data.ToInt(item));

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

            return null;
        }

        #region GETTERS

        /// <summary>
        /// Obtiene la lista de correos del repositorio
        /// </summary>
        /// <param name="cantidad">Cantidad de correos a devolver</param>
        /// <returns>Lista de correos</returns>
        public List<Correo> GetCorreos(int cantidad = 10)
        {
            List<Correo> correos = new();

            correos = repositoryManager.CorreoRepository.GetAll();

            if (Conf.User.UserId != 0 && Conf.User.UserId != null)
            {
                correos = correos.Where(c => c.UsuarioId == Conf.User.UserId).ToList();
            }
            else
            {
                correos = new List<Correo>();
            }

            return correos;
        }
        public List<Conversacion> GetConversaciones(int cantidad = 10)
        {
            List<Conversacion> conversaciones = new();

            conversaciones = repositoryManager.ConversacionRepository.GetAll();

            return conversaciones;
        }

        /// <summary>
        /// Obtiene la lista de respuestas del repositorio
        /// </summary>
        /// <param name="cantidad"></param>
        /// <returns>Lista de respuestas</returns>
        public List<Respuesta> GetRespuestas(int cantidad = 10)
        {
            lock (Context._methodLock) //Para que los hilos no accedan a la vez
            {
                List<Respuesta> respuestas = new();

                respuestas = repositoryManager.RespuestaRepository.GetAll();

                if (Conf.User.UserId != 0 && Conf.User.UserId != null)
                {
                    respuestas = respuestas.Where(c => c.Remitente == Conf.User.Email)
                        .ToList();
                }
                else
                {
                    respuestas = new List<Respuesta>();
                }


                return respuestas;
            }
        }

        /// <summary>
        /// Dado un ID, devuelve la respuesta
        /// </summary>
        /// <param name="id">de la respuesta a devolver</param>
        /// <returns>La respuesta o Null en caso de que no exista</returns>
        /// <remarks>Método creado para hacer compatible los métidos enviar y adjuntar archivos cuando creas una respuesta desde NuevoCorreo</remarks>
        public Respuesta GetRespuestaById(int id)
        {
            Respuesta res = null;

            try
            {
                res = repositoryManager.RespuestaRepository.GetById(id);

            }
            catch (Exception ex) { }


            return res;
        }


        /// <summary>
        /// Obtiene la lista de contactos del repositorio
        /// </summary>
        /// <param name="cantidad"></param>
        /// <returns>Lista de contactos</returns>
        public List<Contacto> GetContactos(int cantidad = 10)
        {
            List<Contacto> contactos = new();

            contactos = repositoryManager.ContactoRepository.GetAll();

            if (Conf.User.UserId != 0 && Conf.User.UserId != null)
            {
                contactos = contactos.Where(c => c.UsuarioId == Conf.User.UserId).ToList();
            }
            else
            {
                contactos = new List<Contacto>();
            }


            return contactos;
        }

        public List<Usuario> GetUsuarios(int cantidad = 10)
        {
            List<Usuario> usuarios = new();

            usuarios = repositoryManager.UsuarioRepository.GetAll();

            if (Conf.User.UserId != 0 && Conf.User.UserId != null)
            {
                usuarios = usuarios.Where(c => c.UsuarioId == Conf.User.UserId).ToList();
            }
            else
            {
                usuarios = new List<Usuario>();
            }
            return usuarios;
        }
        public List<Eliminado> GetEliminados(int cantidad = 10)
        {
            List<Eliminado> eliminados = new();

            eliminados = repositoryManager.EliminadoRepository.GetAll();

            if (Conf.User.UserId != 0 && Conf.User.UserId != null)
            {
                eliminados = eliminados.Where(c => c.UsuarioId == Conf.User.UserId).ToList();
            }
            else
            {
                eliminados = new List<Eliminado>();
            }

            return eliminados;
        }
        #endregion

        #region [1] GESTION ELEMENTOS INDIVIDUALES
        // Métodos que operan sobre una sola entidad:
        // - Regenerar una respuesta con IA
        // - Restaurar un correo o respuesta eliminado
        // - Envio de correos
        // - Eliminacion de correos y contactos
        // - Marcar como leido y guardar la respuesta en BD

        #region [1.1] REGENERAR RESPUESTA IA

        /// <summary>
        /// Genera una respuesta si el correo no la tiene o Regenera la respuesta si el correo ya tiene una
        /// </summary>
        /// <returns>Si la acción se ha realizado o no</returns>
        public async Task<bool> RegenerarRespuestaAction(int id, string prompt)
        {
            //Hacer la logica de regenerarRespuesta //Asignar una nueva respuesta al correo

            Usuario myUser = repositoryManager.UsuarioRepository.GetById(Conf.User.UserId);
            try
            {
                if (id != null && myUser.Plan.MakeAPeticion())
                {
                    Correo correoARegenerar = repositoryManager.CorreoRepository.GetById(id);

                    Respuesta respuestaOriginal = null;

                    if (correoARegenerar.RespuestaId.HasValue)
                    {
                        respuestaOriginal = repositoryManager.RespuestaRepository.GetById(correoARegenerar.RespuestaId.Value);

                    }

                    if (correoARegenerar.RespuestaId == null) //Si no tiene respuesta
                    {
                        Respuesta res = await generarRespuesta.GenerarRespuestaIA(correoARegenerar, prompt); //Genera respuesta y la añade al repo como si fuera una nueva y la asocia al correo
                        return true;
                    }

                    if (correoARegenerar.RespuestaId != null) //si tiene respuesta
                    {
                        try
                        {
                            Respuesta res = await generarRespuesta.RegenerarRespuestaIA(correoARegenerar, prompt); //Sustituye al respuesta que habia por una nueva

                            res.ChangeDestinatarios(respuestaOriginal.Destinatarios);

                            respuestaOriginal.ChangeRespuesta(res);

                            repositoryManager.RespuestaRepository.Update(respuestaOriginal);

                            SetChanges(OpResul.Range, WindowType.None);

                            return true;
                        }
                        catch
                        {

                            return false;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al regenerar respuesta " + ex, DateTime.Now.ToString());

                return false;
            }
        }
        #endregion

        #region [1.2] RESTAURAR ELIMINADO

        /// <summary>
        /// Elimina el CorreoEliminado y la respuesta generada por IA asociada (si la tiene)
        /// </summary>
        /// <param name="idEliminado"></param>
        /// <returns></returns>
        private async Task<bool> RestaurarElimiando(int idEliminado)
        {
            List<Correo> listCorreo = repositoryManager.CorreoRepository.GetAll().ToList();
            List<Respuesta> listRespuesta = repositoryManager.RespuestaRepository.GetAll().ToList();

            Eliminado elim = repositoryManager.EliminadoRepository.GetById(idEliminado);
            try
            {
                if (elim != null)
                {
                    Correo? correoRestaurado = listCorreo.FirstOrDefault(c => c?.Guid == elim?.Guid);
                    Respuesta? respuestaRestaurada = listRespuesta.FirstOrDefault(c => c?.Guid == elim?.Guid);

                    if (elim.RespuestaEliminadaId != null)
                    {
                        respuestaRestaurada = listRespuesta.Where(c => c.CorreoId == correoRestaurado?.CorreoId).FirstOrDefault();
                    }

                    //Compruebo si era correo o respuesta y la borro de eliminados y la borro del repositorio de respuestas o correos
                    if (correoRestaurado != null)
                    {
                        correoRestaurado.SetEliminado(false);
                        await repositoryManager.EliminadoRepository.DeleteAsync(elim.EliminadoId);
                    }

                    if (respuestaRestaurada != null)
                    {
                        respuestaRestaurada.ChangeEsEliminado(false);
                        await repositoryManager.EliminadoRepository.DeleteAsync(elim.EliminadoId);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al restaurar eliminado " + ex, DateTime.Now.ToString());
                return false;
            }

        }

        #endregion

        #region [1.3] ENVIO DE CORREOS

        /// <summary>
        /// Crea una nueva respuesta asociada a un correo existente
        /// </summary>
        /// <param name="datos">Diccionario con Para, Asunto, CuerpoHTML y CorreoOriginalId</param>
        /// <returns></returns>
        private async Task<bool> ResponderCorreoAction(object datos)
        {
            try
            {
                var dict = datos as Dictionary<string, object>;
                if (dict == null) return false;

                string para = dict["Para"]?.ToString();
                string asunto = dict["Asunto"]?.ToString();
                string cuerpo = dict["CuerpoHTML"]?.ToString();
                int correoOriginalId = Data.ToInt(dict["CorreoOriginalId"]);

                if (string.IsNullOrWhiteSpace(para) || string.IsNullOrWhiteSpace(cuerpo) || correoOriginalId <= 0)
                    return false;

                var destinatarios = para.Split(';', ',').Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)).ToList();

                var correoOriginal = repositoryManager.CorreoRepository.GetById(correoOriginalId);

                var nuevaRespuesta = Respuesta.CreateRespuesta(
                    correoOriginal,
                    remitente: Conf.User.Email,
                    destinatarios,
                    asunto,
                    cuerpo,
                    cuerpo,
                    DateTime.Now
                );

                nuevaRespuesta.ChangeEsIA(false);
                nuevaRespuesta.ChangeBorrador(false);

                // Guardar en base de datos
                await repositoryManager.RespuestaRepository.AddAsync(nuevaRespuesta);

                // Enviar
                var envioOk = await EnviarRespuestaAction(nuevaRespuesta);

                return envioOk;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex, "Error al enviar respuesta con ResponderCorreoAction");
                return false;
            }
        }

        /// <summary>
        /// Metodo para Enviar Respuestas
        /// </summary>
        /// <param name="respuestaId">Respuesta a enviar</param>
        /// <returns></returns>
        /// <remarks> </remarks>
        public async Task<bool> EnviarRespuestaAction(object respuestaArg)
        {
            //Obtenemos la respuesta
            Respuesta respuesta = respuestaArg as Respuesta;

            if (respuesta == null)
            {
                return false;
            }

            try
            {
                var message = new MimeMessage
                {
                    MessageId = respuesta.MensajeId
                };

                // Si está asociada a un correo original
                if (respuesta.CorreoId.HasValue)
                {
                    var correoOriginal = repositoryManager.CorreoRepository.GetById(respuesta.CorreoId.Value);
                    if (correoOriginal != null && !string.IsNullOrWhiteSpace(correoOriginal.MensajeId))
                    {
                        message.InReplyTo = correoOriginal.MensajeId;
                        message.References.Add(correoOriginal.MensajeId);
                    }

                    respuesta.ChangeFechaEnviado(DateTime.Now);
                    repositoryManager.CorreoRepository.Update(correoOriginal);
                }
                else
                {
                    // Respuesta nueva (no asociada)
                    respuesta.ChangeFechaEnviado(DateTime.Now);
                    respuesta.ChangeEsIA(false);
                }

                //Accedo a los datos de sesion del usuario
                //byte[] data;
                //_httpContextAccessor.HttpContext.Session.TryGetValue("SessionUser", out data);
                //var json = System.Text.Encoding.UTF8.GetString(data);
                //var userInfo = JsonConvert.DeserializeObject<Usuario>(json);


                var contraseña = Conf.User.Password;
                var email = Conf.User.Email;
                var accessToken = Conf.User.AccessToken;

                message.From.Add(MailboxAddress.Parse(email)); //El correo que se envia tiene como remitente yo (el destinatario de la respuesta)

                // Verifica que la lista de destinatarios no esté vacía
                if (respuesta.Destinatarios == null || !respuesta.Destinatarios.Any())
                {
                    Logger.LogLine("GesCorreos", "Error al enviar correo: lista de destinatarios vacia", DateTime.Now.ToString());
                    return false;
                }

                foreach (var rec in respuesta.Destinatarios)
                {
                    if (!string.IsNullOrWhiteSpace(rec))
                    {
                        message.To.Add(MailboxAddress.Parse(rec.Trim()));
                    }
                }

                message.Subject = string.IsNullOrEmpty(respuesta.Asunto) ?
                                      (respuesta.Asunto ?? "Sin Asunto") :
                                      respuesta.Asunto;

                // Se arma el cuerpo del mensaje: se utiliza multipart/mixed si hay adjuntos
                if (respuesta.Adjuntos.Any())
                {
                    var multipart = new Multipart("mixed");
                    // Parte HTML del mensaje
                    multipart.Add(new TextPart("html")
                    {
                        Text = respuesta.Cuerpo ?? string.Empty
                    });
                    // Se añaden cada uno de los adjuntos
                    foreach (var adjunto in respuesta.Adjuntos)
                    {
                        // Se abre el archivo utilizando la ruta almacenada en la propiedad Ruta
                        var mimeAttachment = new MimePart()
                        {
                            Content = new MimeContent(File.OpenRead(adjunto.Ruta), ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = adjunto.Nombre
                        };
                        multipart.Add(mimeAttachment);
                    }
                    message.Body = multipart;
                }
                else
                {
                    message.Body = new TextPart("html")
                    {
                        Text = respuesta.Cuerpo ?? string.Empty
                    };
                }

                //// Enviar el mensaje a través del servidor SMTP
                //using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
                //{

                //    //Obtengo el dominio del destinatario y creo la conexion con el servidor stmp con ese domino
                //    string smtpServerString = "smtp.gmail.com"; //Conf.EmailConf.SmtpConexion;
                //    int smtpPort = 587;//Conf.EmailConf.SmtpPort;

                //    await smtpClient.ConnectAsync(smtpServerString, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                //    //Si no se ha registrado con gmail
                //    if (accessToken == "NoAccessToken")
                //    {
                //        await smtpClient.AuthenticateAsync(email, Conf.User.Password);
                //        await smtpClient.SendAsync(message);
                //        await smtpClient.DisconnectAsync(true);
                //    }
                //    else //Si se ha registrado con el GOOGLE
                //    {
                //        var oauth2 = new SaslMechanismOAuth2(email, accessToken);
                //        await smtpClient.AuthenticateAsync(oauth2);
                //        await smtpClient.SendAsync(message);
                //        await smtpClient.DisconnectAsync(true);
                //    }
                //}

                //Usar el servicio para enviar el mensaje
                serviceManager.AddService("SendEmailService", new object[] { message, "ourapp" }, 10, true);

                OnCorreoEnviado?.Invoke(OpResul.Range);

                // Se puede actualizar el correo en la BD para marcar el envío, si fuera necesario
                repositoryManager.RespuestaRepository.Update(respuesta);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al enviar correo" + ex, DateTime.Now.ToString());
                return false;
            }
        }

        /// <summary>
        /// Añade archivos adjuntos a una respuesta
        /// </summary>
        /// <param name="respuestaArg">Respuesta a la que añadir adjuntos</param>
        /// <param name="fileNames">Lista de nombres de los archivos a añadir</param>
        /// <returns>true si se ha añadido</returns>
        public bool AdjuntarArchivoAction(object respuestaArg, List<string> fileNames)
        {
            Respuesta respuesta = respuestaArg as Respuesta;

            try
            {
                if (respuesta.CorreoId <= 0 || respuesta.CorreoId == null) //Significa que la respuesta NO tiene un correo asociado (es creada de 0 por el user)
                {
                    respuesta.ChangeEsIA(false);
                }

                // Se obtiene la respuesta a la que adjuntar el archivo con repositoryManager.RespuestaRespository.GetById(eespuestaId); (se ha cambiado porque ahora recibe una respuesta y no un int)

                List<string> filepathsString = new List<string>();

                foreach (string name in fileNames)
                {
                    filepathsString.Add(Conf.Paths.DirAdjuntos + $@"\{name}");
                }

                if (respuesta != null)
                {

                    foreach (string filepath in filepathsString)
                    {
                        // Se crea el adjunto usando el método existente en la clase Adjunto
                        Adjunto adjunto = Adjunto.CreateAdjunto(respuesta, filepath);

                        respuesta.Adjuntos.Add(adjunto);
                    }

                    repositoryManager.RespuestaRepository.Update(respuesta);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al adjuntar correo" + ex, DateTime.Now.ToString());
                return false;

            }
        }

        #endregion

        #region [1.4] ELIMINACION DE CORREOS Y CONTACTOS

        /// <summary>
        /// Elimina un correo de la papelera definitivamente
        /// </summary>
        /// <param name="idEliminado">id del Eliminado</param>
        /// <param name="esCorreo">true si es correo (tiene un RespuestaEliminada!=null)</param>
        /// <returns></returns>
        public async Task<bool> EliminarCorreoDefinitivamenteAction(int idEliminado)
        {
            List<Correo> listCorreo = repositoryManager.CorreoRepository.GetAll().ToList();
            List<Respuesta> listRespuesta = repositoryManager.RespuestaRepository.GetAll().ToList();

            Eliminado elim = repositoryManager.EliminadoRepository.GetById(idEliminado);

            try
            {
                if (elim != null)
                {
                    Correo? correoRestaurado = listCorreo.Where(c => c.Asunto == elim.Asunto).FirstOrDefault();
                    Respuesta? respuestaRestaurada = listRespuesta.Where(c => c.Asunto == elim.Asunto).FirstOrDefault();

                    if (elim.RespuestaEliminadaId != null)
                    {
                        List<Respuesta> respuestas = repositoryManager.RespuestaRepository.GetAll().ToList();

                    }

                    //Compruebo si era correo o respuesta y la borro de eliminados y la borro del repositorio de respuestas o correos
                    if (correoRestaurado != null)
                    {
                        correoRestaurado.SetEliminado(false);
                        await repositoryManager.EliminadoRepository.DeleteAsync(elim.EliminadoId);
                        await repositoryManager.CorreoRepository.DeleteAsync(correoRestaurado.CorreoId);

                    }

                    if (respuestaRestaurada != null)
                    {
                        respuestaRestaurada.ChangeEsEliminado(false);
                        await repositoryManager.EliminadoRepository.DeleteAsync(elim.EliminadoId);
                        await repositoryManager.RespuestaRepository.DeleteAsync(respuestaRestaurada.RespuestaId);
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al eliminar correo COMPLETAMENTE " + ex, DateTime.Now.ToString());

                return false;

            }





            //if (idEliminado < 1)
            //{
            //    return false;
            //}

            //Eliminado eliminado = repositoryManager.EliminadoRepository.GetById(idEliminado);

            //if (eliminado == null)
            //{
            //    return false;
            //}

            //await EliminarCorreoDefinitivamenteGmail(eliminado); //En caso de ser un borrador, como NO se mete como borrador en el gmail, cuando se mande a la papelera no estará y por ende, no lo borrará

            //if (eliminado.RespuestaEliminada != null)
            //{
            //    repositoryManager.EliminadoRepository.DeleteAsync(eliminado.RespuestaEliminada.EliminadoId); //Borro la respuesta
            //}

            //repositoryManager.EliminadoRepository.DeleteAsync(idEliminado); //Borro el eliminaado

            //return true;
        }

        private async Task<bool> EliminarCorreoDefinitivamenteGmail(Eliminado eliminado)
        {
            bool deleted = false;

            using (var imapClient = new ImapClient())
            {
                try
                {
                    var contraseña = Conf.User.Password;
                    var email = Conf.User.Email;

                    // Gmail siempre usa este servidor IMAP
                    string imapServerString = EmailConfig.Instance().GetImapServer();
                    int imapPort = EmailConfig.Instance().GetImapPort();

                    await imapClient.ConnectAsync(imapServerString, imapPort, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    await imapClient.AuthenticateAsync(email, contraseña);

                    // Accedemos a la papelera
                    var trash = imapClient.GetFolder(EmailConfig.Instance().GetTrashFolder());
                    await trash.OpenAsync(FolderAccess.ReadWrite);

                    // Buscar el mensaje por asunto (puedes usar otro criterio si lo prefieres)
                    var uids = await trash.SearchAsync(SearchQuery.SubjectContains(eliminado.Asunto));


                    if (uids.Count > 0)
                    {
                        // Marcar como Deleted
                        foreach (var uid in uids)
                        {
                            await trash.AddFlagsAsync(uid, MessageFlags.Deleted, true);
                        }

                        // Expunge = eliminación definitiva
                        await trash.ExpungeAsync();

                        deleted = true;
                    }

                    await imapClient.DisconnectAsync(true);
                    return deleted;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Elimina una respuesta (Desde Enviados o Borradores)
        /// </summary>
        /// <param name="idRespuesta">Respuesta a eliminar</param>
        /// <returns></returns>
        public async Task<bool> EnviarRespuestaAPapeleraAction(int idRespuesta)
        {
            bool isDeleted = false;

            if (idRespuesta == null)
            {
                return isDeleted;
            }

            try
            {
                Respuesta respuestaAEliminar = repositoryManager.RespuestaRepository.GetById(idRespuesta);

                if (respuestaAEliminar != null)//si es una respuesta valida y si está enviado
                {
                    Eliminado respuestaElim = (Eliminado)respuestaAEliminar;

                    respuestaElim.SetUsuario(Conf.User.UserId);

                    if (respuestaAEliminar.Borrador)
                    {
                        respuestaElim.SetEsBorrador(true);
                    }

                    if (respuestaAEliminar.Enviado)
                    {
                        respuestaElim.SetEsEnviado(true);
                    }

                    if (respuestaAEliminar.EsIA)
                    {
                        respuestaElim.SetEsIA(true);
                    }

                    respuestaAEliminar.ChangeEsEliminado(true); //La marcamos como eliminada

                    await repositoryManager.EliminadoRepository.AddAsync(respuestaElim);

                    repositoryManager.RespuestaRepository.Update(respuestaAEliminar);//La updateamos, NO LA SACAMOS DEL REPOSITORIO

                    //await repositoryManager.RespuestaRepository.DeleteAsync(idRespuesta);

                    isDeleted = true;
                }
                return isDeleted;

            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al enviar respuesta ala papelera " + ex, DateTime.Now.ToString());
                return isDeleted;
            }
        }

        /// <summary>
        /// Elimina un correo y su respuesta asociada de la base de datos y del propio Gmail, Hotmail... del usuario. 
        /// </summary>
        /// <param name="id">id del correo a eliminar</param>
        /// <returns>Si se ha eliminado correctamente, devuelve true</returns>
        public async Task<bool> EnviarCorreoAPapeleraAction(int id)
        {
            if (id == null)
            {
                return false;
            }
            try
            {
                //Correo para borrar
                Correo correoAEliminar = repositoryManager.CorreoRepository.GetById(id);

                //Elimino la respuesta cuyo ID es el correoAEliminar.Respuesta.RespuestaId (el asociado al correo que quiero eliminar)

                if (correoAEliminar != null) //Por si quiere borrar si el correo aun no se ha procesado/la respuesta no se ha generado
                {
                    //await EnviarCorreoALaPapeleraGmail(correoAEliminar); //Solo funciona con gmail

                    correoAEliminar.SetEliminado(true); //Lo marcamos como borrado

                    repositoryManager.CorreoRepository?.Update(correoAEliminar);

                    Eliminado correoElim = (Eliminado)correoAEliminar;

                    correoElim.SetEsCorreo(true);

                    //Añado ambas al repositorio de eliminaod
                    repositoryManager.EliminadoRepository?.AddAsync(correoElim);

                    if (correoAEliminar.RespuestaId != null) //Si tiene respuesta
                    {
                        if (correoAEliminar.RespuestaId.HasValue)
                        {
                            Respuesta respuesta = repositoryManager.RespuestaRepository.GetById(correoAEliminar.RespuestaId.Value);
                            repositoryManager.EliminadoRepository?.AddAsync((Eliminado)respuesta);
                        }

                        if (correoElim.RespuestaEliminadaId != null)
                        {
                            Eliminado respuestaElimianda = repositoryManager.EliminadoRepository.GetById(correoElim.RespuestaEliminadaId.Value);

                            correoElim.SetRespuestaEliminadaId(respuestaElimianda.EliminadoId); //Es probable que la ID de la respuesta != de la respuesta eliminada, por eso lo asocio
                            repositoryManager.EliminadoRepository?.Update(correoElim);
                        }
                    }

                    //NO HACE FALTA BORRAR DEL CORREO REPO PORQUE:
                    //- Si correo sigue en el repositorio, no se vuelve a leer con ReadEmailService
                    //- En salida, Solo muestro las respuestas cuyo correo NO sea elimiando, aunque siguen en el repositorio
                    //- en entrada, Solo muestro los correos que tengan el flag eliminado == false

                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al enviar correo a la papelera " + ex, DateTime.Now.ToString());

                return false;
            }
        }

        /// <summary>
        /// Elimina un contacto
        /// </summary>
        /// <param name="id">id del contacto a eliminar</param>
        /// <returns>Si se ha eliminado correctamente, devuelve true</returns>
        private async Task<bool> EliminarContactoAction(int id)
        {
            bool isDeleted = false;
            if (id <= 0)
            {
                return isDeleted;
            }
            try
            {
                // Obtener el contacto desde el repositorio
                Contacto contactoAEliminar = repositoryManager.ContactoRepository.GetById(id);
                Contacto contactoDummy = null;

                if (contactoAEliminar != null)
                {
                    List<Correo> correosConEseContacto = repositoryManager.CorreoRepository.GetAll().Where(c => c.Destinatarios.Contains(contactoAEliminar.Email)).ToList();

                    //Si no tiene correos asociados
                    if (correosConEseContacto.Count == 0)
                    {
                        // Eliminar el contacto de la base de datos de forma asíncrona
                        await repositoryManager.ContactoRepository.DeleteAsync(id);
                        isDeleted = true;
                    }
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al eliminar contacto" + ex, DateTime.Now.ToString());
                return isDeleted;
            }
        }

        /// <summary>
        /// Añade un contacto a la base de datos si no existe
        /// </summary>
        /// <param name="contacto">Contacto a añadir</param>
        /// <returns></returns>
        private async Task<bool> GuardarContacto(object contacto)
        {
            bool isAdded = false;
            try
            {
                Contacto newCont = contacto as Contacto;

                //Si no existe
                //if (newCont != null && !Context.Instance.Contactos.Any(c => c.Email == newCont.Email))
                if (newCont != null && !repositoryManager.ContactoRepository.GetAll().Any(c => c.Email == newCont.Email))
                {
                    if (string.IsNullOrEmpty(newCont.Nombre))
                    {
                        string nombreTruncado = newCont.Email.Split('@')[0];
                        newCont.SetNombre(nombreTruncado);
                    }

                    await repositoryManager.ContactoRepository.AddAsync(newCont);

                    isAdded = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al guardar contacto" + ex, DateTime.Now.ToString());

            }

            return isAdded;
        }

        /// <summary>
        /// Manda el correo a la papelera en el GMAIL
        /// </summary>
        /// <param name="correoAEliminar"></param>
        /// <returns></returns>
        private async Task<bool> EnviarCorreoALaPapeleraGmail(Correo correoAEliminar)
        {
            bool movedToTrash = false;

            using (var imapClient = new ImapClient())
            {
                try
                {
                    var email = Conf.User.Email;
                    var contraseña = Conf.User.Password;

                    string imapServerString = EmailConfig.Instance().GetImapServer();
                    int port = EmailConfig.Instance().GetImapPort();

                    await imapClient.ConnectAsync(imapServerString, port, SecureSocketOptions.SslOnConnect);
                    await imapClient.AuthenticateAsync(email, contraseña);

                    // Abrir carpeta INBOX
                    var inbox = imapClient.Inbox; //Suele funcionar con todos los dominios
                    await inbox.OpenAsync(FolderAccess.ReadWrite);

                    // Buscar mensajes con ese asunto
                    var resultados = await inbox.SearchAsync(SearchQuery.SubjectContains(correoAEliminar.Asunto));

                    if (resultados.Count > 0)
                    {
                        // Obtener la carpeta TRASH
                        var trashFolder = imapClient.GetFolder(EmailConfig.Instance().GetTrashFolder());

                        // Mover todos los mensajes encontrados a la papelera
                        foreach (var id in resultados)
                        {
                            await inbox.CopyToAsync(id, trashFolder); //Lo copia a la papelera
                            await inbox.AddFlagsAsync(id, MessageFlags.Deleted, true); //Lo marca pa borrar
                        }

                        imapClient.Inbox.Expunge(); //Borra todos los marcados
                    }

                    await imapClient.DisconnectAsync(true);
                    return movedToTrash;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    return false;
                }
            }
        }

        #endregion

        #region [1.5] MARCAR COMO LEIDO Y GUARDAR RESPUESTA EN BD

        /// <summary>
        /// Marca un Correo como Leido
        /// </summary>
        /// <param name="correoId"></param>
        /// <param name="leido"></param>
        /// <returns></returns>
        public bool MarcarComoLeidoAction(int correoId, bool leido)
        {
            // Obtener el correo desde el repositorio.
            Correo correo = repositoryManager.CorreoRepository.GetById(correoId);
            if (correo == null)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] No se encontró el correo con ID {correoId}.");
                return false;

            }
            // Si se indica que debe marcarse como leído.
            if (leido)
            {
                correo.SetLeido();
                System.Diagnostics.Debug.WriteLine($"[DEBUG] El correo con ID {correoId} se ha marcado como leído.");
            }
            // Actualiza el correo en el repositorio.
            repositoryManager.CorreoRepository.Update(correo);
            return true;
        }

        /// <summary>
        /// Guarda una respuesta en la Database
        /// </summary>
        /// <param name="resAug">Respuesta a guardar</param>
        /// <returns>True if added</returns>
        public async Task<bool> GuardarRespuestaBaseDatos(object resAug)
        {

            bool added = false;
            try
            {
                Respuesta res = resAug as Respuesta;
                if (res.RespuestaId == 0)
                {
                    if (res.CorreoId == 0 || res.CorreoId == null) //Respuesta sin correo --> creo nueva coversacion
                    {
                        Conversacion conver = Conversacion.CreateConversacion();
                        await repositoryManager.ConversacionRepository.AddAsync(conver);

                        res.SetConver(conver.ConversacionId);

                        await repositoryManager.RespuestaRepository.AddAsync(res);

                        conver.AddToConversacion(res);

                        repositoryManager.ConversacionRepository.Update(conver);
                    }
                    else
                    {
                        await repositoryManager.RespuestaRepository.AddAsync(res);
                    }

                    added = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesCorreos", "Error al GuardarRespuesta en la BD" + ex, DateTime.Now.ToString());

            }
            return added;
        }


        public Correo GetCorreoById(int correoId)
        {

            Correo miCorreo = repositoryManager.CorreoRepository.GetById(correoId);


            return miCorreo;
        }

        #endregion

        #endregion

        #region [2] GESTION DE LISTAS DE DATOS
        // Métodos que operan sobre una una lista de entidades:
        // - Destacado de correos
        // - Mandar correos y respuestas a la papelera
        // - Restaura una lista de correos y respuestas
        // - Suprime una lista de correos y respuestas
        // - Eliminar contactos

        #region [2.1] DESTACADO DE CORREOS

        /// <summary>
        /// Destaca una lista de correos
        /// </summary>
        /// <param name="correos"></param>
        /// <returns></returns>
        public bool DestacarListaCorreos(List<int> correosId, bool destacar)
        {
            foreach(var idCorreo in correosId)
            {
                Correo correo = repositoryManager.CorreoRepository.GetById(Data.ToInt(idCorreo));

                if (destacar)
                {
                    correo.SetDestacado();
                }
                else
                {
                    correo.QuitarDestacado();
                }
            }

            SetChanges(OpResul.Page);
            context.SaveChanges();
            return true;
        }

        #endregion

        #region [2.2] MANDAR A PAPELERA

        /// <summary>
        /// Recibe una lista de IDs de correos y los procesa para mandarlos
        /// a la papelera. Tambien elimina las respuestas asociadas a los correos
        /// </summary>
        /// <param name="correosId">Lista de IDs de los correos</param>
        /// <returns>True</returns>
        private bool MandarAPapeleraCorreos(List<int> correosId)
        {
            foreach (var idCorreo in correosId)
            {
                Correo correo = repositoryManager.CorreoRepository.GetById(Data.ToInt(idCorreo));

                if (correo == null)
                    continue; //Si es null, pasa al siguiente

                correo.SetEliminado(true);

                //Crear objeto eliminado
                Eliminado correoElim = CreateCorreoEliminado(correo);

                //Eliminar respuesta correo
                EliminarRespuestaCorreo(correo, correoElim);
            }

            SetChanges(OpResul.Page);
            context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Devuelve un Eliminado a partir de un Correo y
        /// lo agrega al repositorio de eliminados
        /// </summary>
        /// <param name="correo">Correo a eliminar</param>
        /// <returns>Eliminado del correo</returns>
        private Eliminado CreateCorreoEliminado(Correo correo)
        {
            Eliminado eliminado = (Eliminado)correo;
            eliminado.SetEsCorreo(true);
            repositoryManager.EliminadoRepository?.AddAsync(eliminado, false);

            return eliminado;
        }

        /// <summary>
        /// Si el correo original tiene respuesta, genera un eliminado de la 
        /// respuesta y la añade al repositorio de eliminados.
        /// </summary>
        /// <param name="correo">Correo original</param>
        /// <param name="correoElim">Referencia correo original eliminado</param>
        /// <returns>True si elimina la respuesta, false en caso contrario</returns>
        private bool EliminarRespuestaCorreo(Correo correo, Eliminado correoElim)
        {
            if(correo.RespuestaId == null)
            {
                return false;
            }

            try
            {
                if (correo.RespuestaId.HasValue)
                {
                    Respuesta respuesta = repositoryManager.RespuestaRepository.GetById(correo.RespuestaId.Value);
                    repositoryManager.EliminadoRepository?.AddAsync((Eliminado)respuesta, false);
                }

                if (correoElim.RespuestaEliminadaId != null)
                {
                    Eliminado respuestaElimianda = repositoryManager.EliminadoRepository.GetById(correoElim.RespuestaEliminadaId.Value);

                    correoElim.SetRespuestaEliminadaId(respuestaElimianda.EliminadoId); //Es probable que la ID de la respuesta != de la respuesta eliminada, por eso lo asocio
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex);
            }
            
            return true;
        }

        #endregion

        #region [2.3] RESTAURAR CORREOS

        /// <summary>
        /// Metodo para restaurar una lista de correos desde la papelera
        /// </summary>
        /// <param name="listaEliminadosId">Lista de IDs de los correos a restaurar</param>
        /// <returns>True</returns>
        private async Task<bool> RestaurarListaEliminados(List<int> listaEliminadosId)
        {
            List<Correo> listCorreo = repositoryManager.CorreoRepository.GetAll().ToList();
            List<Respuesta> listRespuesta = repositoryManager.RespuestaRepository.GetAll().ToList();

            foreach(var eliminadoId in listaEliminadosId)
            {
                try
                {
                    Eliminado? eliminado = repositoryManager.EliminadoRepository.GetById(eliminadoId);

                    if (eliminado == null)
                        continue;

                    Correo? correoRestaurar = listCorreo.FirstOrDefault(c => c?.Guid == eliminado?.Guid);
                    Respuesta? respuestaRestaurar = listRespuesta.FirstOrDefault(c => c?.Guid == eliminado?.Guid);

                    if (eliminado.RespuestaEliminadaId != null)
                    {
                        respuestaRestaurar = listRespuesta.FirstOrDefault(c => c.CorreoId == correoRestaurar?.CorreoId);
                    }

                    //Compruebo si era correo o respuesta y la borro de eliminados y la borro del repositorio de respuestas o correos
                    if (correoRestaurar != null)
                    {
                        correoRestaurar.SetEliminado(false);
                        await repositoryManager.EliminadoRepository.DeleteAsync(eliminado.EliminadoId, false);
                    }

                    if (respuestaRestaurar != null)
                    {
                        respuestaRestaurar.ChangeEsEliminado(false);
                        await repositoryManager.EliminadoRepository.DeleteAsync(eliminado.EliminadoId, false);
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogLine("GesCorreos", "Error al restaurar eliminado " + ex, DateTime.Now.ToString());
                    return false;
                }
            }

            SetChanges(OpResul.Range);
            context.SaveChanges();
            return true;
        }
        #endregion

        #region [2.4] SUPRIMIR CORREOS

        /// <summary>
        /// Metodo para suprimir una lista correos en la papelera definitivamente
        /// </summary>
        /// <param name="idsEliminados">Lista de IDs de correos a eliminar</param>
        /// <returns>True</returns>
        public async Task<bool> SuprimirListaCorreos(List<int> idsEliminados)
        {
            List<Correo> listCorreo = repositoryManager.CorreoRepository.GetAll().ToList();
            List<Respuesta> listRespuesta = repositoryManager.RespuestaRepository.GetAll().ToList();

            foreach(var idEliminado in idsEliminados)
            {
                Eliminado eliminado = repositoryManager.EliminadoRepository.GetById(idEliminado);

                try
                {
                    if (eliminado == null)
                        continue;

                    Correo? correoRestaurado = listCorreo.FirstOrDefault(c => c.MensajeId == eliminado.MensajeId);

                    //----ATENCION TO DO-----
                    //GESTIONAR ELIMINAR RESPUESTAS,
                    //es posible que asunto sea igual para 2 respuestas distintas,
                    //agregar identificador clave que permita diferenciar
                    Respuesta? respuestaRestaurada = listRespuesta.FirstOrDefault(c =>
                                                                        c.Asunto == eliminado.Asunto &&
                                                                        c.FechaProcesado == eliminado.FechaProcesado);

                    if (eliminado.RespuestaEliminadaId != null)
                    {
                        List<Respuesta> respuestas = repositoryManager.RespuestaRepository.GetAll().ToList();

                    }

                    //Compruebo si era correo o respuesta y la borro de eliminados y la borro del repositorio de respuestas o correos
                    if (correoRestaurado != null)
                    {
                        correoRestaurado.SetEliminado(false);
                        await repositoryManager.EliminadoRepository.DeleteAsync(eliminado.EliminadoId, false);
                        await repositoryManager.CorreoRepository.DeleteAsync(correoRestaurado.CorreoId, false);

                    }

                    if (respuestaRestaurada != null)
                    {
                        respuestaRestaurada.ChangeEsEliminado(false);
                        await repositoryManager.EliminadoRepository.DeleteAsync(eliminado.EliminadoId, false);
                        await repositoryManager.RespuestaRepository.DeleteAsync(respuestaRestaurada.RespuestaId, false);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogLine("GesCorreos", "Error al eliminar correo COMPLETAMENTE " + ex, DateTime.Now.ToString());

                    return false;

                }
            }

            SetChanges(OpResul.Docum);
            context.SaveChanges();
            return true;
        }

        #endregion

        #region [2.5] ELIMINAR CONTACTOS

        /// <summary>
        /// Elimina una lista de contactos
        /// </summary>
        /// <param name="contactosIds">Lista de los ID de los contactos a eliminar</param>
        /// <returns>True si elimina todos los contactos, false en caso contrario</returns>
        public async Task<bool> EliminarListaContactos(List<int> contactosIds)
        {
            bool resul = true;

            foreach(int contactoId in contactosIds)
            {
                if (contactoId <= 0)
                    resul = false;

                try
                {
                    Contacto contactoAEliminar = repositoryManager.ContactoRepository.GetById(contactoId);

                    if (contactoAEliminar != null)
                    {
                        continue;
                    }
                    
                    List<Correo> correosContacto = repositoryManager.CorreoRepository
                        .GetAll()
                        .Where(c => c.Destinatarios.Contains(contactoAEliminar.Email))
                        .ToList();

                    //Si no tiene correos asociados
                    if (correosContacto.Count == 0)
                    {
                        // Eliminar el contacto de la base de datos de forma asíncrona
                        await repositoryManager.ContactoRepository.DeleteAsync(contactoId, false);
                    }
                    else
                    {
                        resul = false;
                    }
                    
                }
                catch (Exception ex)
                {
                    Logger.LogLine("GesCorreos", "Error al eliminar contacto" + ex, DateTime.Now.ToString());
                    resul = false;
                    continue;
                }
            }

            SetChanges(OpResul.Range);
            context.SaveChanges();
            return true;
        }

        #endregion

        #endregion

    }
}
