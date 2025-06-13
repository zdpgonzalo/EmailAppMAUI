using MailAppMAUI.General;
using MailAppMAUI.DTOs;
using MailAppMAUI.Core;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.Core
{
    [Table("Correos")]
    [PrimaryKey(nameof(CorreoId))]
    public class Correo : ModelBaseCore<CorreoDTO>
    {
        /// <summary>
        /// Id del correo
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CorreoId { get; private set; }

        [Required]
        public Guid Guid { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Id original obtenido del MimeMessage
        /// </summary>
        public string? MensajeId { get; private set; }

        /// <summary>
        /// Id del usuario al que esta asociado
        /// </summary>
        public int UsuarioId { get; private set; }

        /// <summary>
        /// Usuario al que pertenece
        /// </summary>
        public Usuario Usuario { get; private set; }

        /// <summary>
        /// Id de la respuesta generada
        /// </summary>
        public int? RespuestaId { get; private set; }

        /// <summary>
        /// Id de la respuesta generada
        /// </summary>
        public int? ConversacionId { get; private set; } 

        //[ForeignKey(nameof(ConversacionId))]
        //public Conversacion Conversacion { get; private set; }
        /// <summary>
        /// Email del remitente
        /// </summary>
        [Required]
        public string Remitente { get; private set; }

        /// <summary>
        /// Lista de emails de destinatarios
        /// </summary>
        [Required]
        public List<string> Destinatarios { get; private set; } = new List<string>();

        /// <summary>
        /// Asunto del correo
        /// </summary>
        public string? Asunto { get; private set; }

        /// <summary>
        /// Cuerpo del correo
        /// </summary>
        public string? Cuerpo { get; private set; }

        /// <summary>
        /// Cuerpo HTML del correo
        /// </summary>
        public string? CuerpoHTML { get; private set; }

        /// <summary>
        /// True si se ha leído
        /// </summary>
        public bool Leido { get; private set; }

        /// <summary>
        /// True si se ha eliminado
        /// </summary>
        public bool Eliminado { get; private set; }

        /// <summary>
        /// True si se ha destacado
        /// </summary>
        public bool Destacado { get; private set; }

        /// <summary>
        /// Fecha y hora de cuando se ha recibido el correo
        /// </summary>
        public DateTime FechaRecibido { get; private set; }

        /// <summary>
        /// True si se ha generado una respuesta.
        /// False si no se ha generado una respuesta
        /// </summary>
        public bool Procesado
        {
            get => RespuestaId != null;
        }

        public List<Adjunto> Adjuntos { get; set; } = new();

        /// <summary>
        /// Constructor vacío
        /// </summary>
        private Correo() { }

        /// <summary>
        /// Crea una instancia de CorreoCore
        /// </summary>
        public static Correo CreateCorreo(Usuario usuario, string remitente, List<string> destinatarios,
            string? asunto, string? cuerpo)
        {
            if (usuario == null)
                throw new ArgumentNullException("El usuario no puede ser null", nameof(usuario));
            if (string.IsNullOrEmpty(remitente))
                throw new ArgumentNullException("El remitente no puede estar vacio", nameof(remitente));
            if (destinatarios == null || destinatarios.Count == 0)
                throw new ArgumentNullException("Se debe proporcionar al menos un destinatario", nameof(destinatarios));

            var correo = new Correo()
            {
                Usuario = usuario,
                Remitente = remitente,
                Destinatarios = destinatarios,
                Asunto = asunto,
                Cuerpo = cuerpo,
                FechaRecibido = DateTime.Now,
                Eliminado = false,
            };

            return correo;
        }

        /// <inheritdoc/>
        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

            switch (propertyName)
            {
                case nameof(CorreoId):
                    value = CorreoId.ToString();
                    break;

                case nameof(UsuarioId):
                    value = UsuarioId.ToString();
                    break;

                case nameof(RespuestaId):
                    value = RespuestaId?.ToString() ?? string.Empty;
                    break;

                case nameof(Remitente):
                    value = Remitente;
                    break;

                case nameof(Eliminado):
                    value = Eliminado.ToString();
                    break;

                case nameof(Destinatarios):
                    value = string.Join(", ", Destinatarios);
                    break;

                case nameof(Asunto):
                    value = Asunto ?? string.Empty;
                    break;

                case nameof(ConversacionId):
                    value = ConversacionId.ToString();
                    break;

                case nameof(Cuerpo):
                    value = Cuerpo ?? string.Empty;
                    break;

                case nameof(CuerpoHTML):
                    value = CuerpoHTML ?? string.Empty;
                    break;

                case nameof(FechaRecibido):
                    value = FechaRecibido.ToString();
                    break;

                case nameof(Procesado):
                    value = Procesado.ToString();
                    break;

                case nameof(Leido):
                    value = Leido.ToString();
                    break;

                case nameof(Destacado):
                    value = Destacado.ToString();
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Id: {CorreoId}, Asunto: {Asunto}";
        }

        /// <summary>
        /// Operador explícito que convierte un CorreoCore en CorreoDTO
        /// </summary>
        /// <param name="correo">Correo a convertir</param>
        public static explicit operator CorreoDTO(Correo correo)
        {
            return new CorreoDTO()
            {
                CorreoId = correo.CorreoId,
                UsuarioId = correo.UsuarioId,
                Usuario = (UsuarioDTO)correo.Usuario,
                RespuestaId = correo.RespuestaId,
                Remitente = correo.Remitente,
                Destinatarios = correo.Destinatarios,
                Asunto = correo.Asunto,
                Cuerpo = correo.Cuerpo,
                FechaRecibido = correo.FechaRecibido,
                Procesado = correo.Procesado,
                CuerpoHTML = correo.CuerpoHTML,
                Leido = correo.Leido,
                Destacado = correo.Destacado,
                //ConversacionId = correo.ConversacionId,
                Eliminado = correo.Eliminado,
            };
        }

        /// <summary>
        /// Convierte un CorreoDTO en un CorreoCore
        /// </summary>
        public static Correo ConvertToCore(CorreoDTO correoDTO, Usuario? usuario = null, Respuesta? respuesta = null)
        {
            if (correoDTO == null)
                throw new ArgumentNullException("El registro correoDTO no puede ser null", nameof(correoDTO));

            if (usuario == null && correoDTO.Usuario == null)
                throw new ArgumentNullException("El usuario no puede ser null");

            Correo correo = new Correo()
            {
                DTO_Base = correoDTO,
                Remitente = correoDTO.Remitente,
                // Convertir el string importado (si es que no se hizo el ValueConverter) a lista;
                // pero si ya se tiene la lista en el DTO, se asigna directamente.
                Destinatarios = correoDTO.Destinatarios,
                Asunto = correoDTO.Asunto,
                Cuerpo = correoDTO.Cuerpo,
                FechaRecibido = correoDTO.FechaRecibido,
                CuerpoHTML = correoDTO.CuerpoHTML,
                Leido = false,
                Destacado = false,
                Eliminado = correoDTO.Eliminado,
                //ConversacionId = correoDTO.ConversacionId,
            };

            // Asigna el usuario
            if (usuario != null)
            {
                correo.Usuario = usuario;
            }
            else if (correoDTO.Usuario != null)
            {
                correo.Usuario = Usuario.ConvertToCore(correoDTO.Usuario);
            }
            else
            {
                throw new NotImplementedException("Asignar usuario por Id no implementado");
            }

            // Asigna la respuesta si existe
            //if (respuesta == null && correoDTO.Respuesta != null)
            //{
            //    correo.Respuesta = MailAppMAUI.Core.Respuesta.ConvertToCore(correoDTO.Respuesta); //No distingue bien si es la Respuesta de este correo o el Respuesta.Create
            //}
            //else
            //{
            //    correo.Respuesta = respuesta;
            //}

            // Convierte la lista de adjuntosDTO en una lista de AdjuntosCore
            correo.SetAdjuntos(correoDTO.Adjuntos);

            return correo;
        }
        /// <summary>
        /// Convierte una respuesta en un correo
        /// </summary>
        /// <param name="eliminado"></param>
        public static explicit operator Correo(Respuesta eliminado)
        {
            if (eliminado == null)
                throw new ArgumentNullException("Correo no puede ser null", nameof(eliminado));

            Correo miEliminado = new Correo()
            {
                Remitente = eliminado.Remitente,
                Destinatarios = eliminado.Destinatarios,
                Asunto = eliminado.Asunto,
                Cuerpo = eliminado.Cuerpo,
                CuerpoHTML = eliminado.CuerpoHTML,
                MensajeId = eliminado.MensajeId,


                //----------DATOS RESPUESTAS----------
                FechaRecibido = eliminado.FechaEnviado,

                Adjuntos = eliminado.Adjuntos,
            };

            return miEliminado;
        }


        ///// <summary>
        ///// Operador explicito que convierte un Eliminado en Correo
        ///// </summary>
        ///// <param name="respuesta">Respuesta a convertir</param>
        //public static explicit operator Correo(Eliminado eliminado)
        //{
        //    if (eliminado == null)
        //        throw new ArgumentNullException("Correo no puede ser null", nameof(eliminado));
        //    if (eliminado.UsuarioId == null)
        //        throw new ArgumentNullException("Usuario no puede ser null", nameof(eliminado));

        //    Correo miEliminado = new Correo()
        //    {
        //        Remitente = eliminado.Remitente,
        //        Destinatarios = eliminado.Destinatarios,
        //        Asunto = eliminado.Asunto,
        //        Cuerpo = eliminado.Cuerpo,
        //        CuerpoHTML = eliminado.CuerpoCorreoHTML,

        //        MensajeId = eliminado.MensajeId,
        //        UsuarioId = eliminado?.UsuarioId ?? 0,
        //        Usuario = eliminado.Usuario,
        //        ContactoId = eliminado.ContactoId,
        //        Contacto = eliminado.Contacto,
        //        Leido = eliminado.Leido,
        //        FechaRecibido = eliminado.FechaRecibido,

        //        //Se crea la respuestaEliminada con la Respuesta del Correo
        //        Respuesta = eliminado?.RespuestaEliminada != null ? (Respuesta)eliminado.RespuestaEliminada : null, // Manejar null
        //        RespuestaId = eliminado?.RespuestaEliminada != null ? eliminado.RespuestaEliminadaId : null, // Manejar null

        //        Adjuntos = eliminado.Adjuntos,
        //    };

        //    return miEliminado;
        //}
        /// <summary>
        /// Convierte una lista de AdjuntoDTO en una lista de AdjuntosCore y la asigna
        /// </summary>
        private bool SetAdjuntos(List<AdjuntoDTO> adjuntosDTOList)
        {
            Adjuntos.Clear();

            foreach (var adjunto in adjuntosDTOList)
            {
                Adjuntos.Add(Adjunto.ConvertToCore(adjunto));
            }

            SetChanges(OpResul.Page);
            return true;
        }


        private bool SetAdjuntos(List<Adjunto> adjuntosList)
        {
            Adjuntos.Clear();

            foreach (var adjunto in adjuntosList)
            {
                Adjuntos.Add(adjunto);
            }

            SetChanges(OpResul.Page);
            return true;
        }

        /// <summary>
        /// Convierte una lista de AdjuntoDTO en una lista de AdjuntosCore y la asigna
        /// </summary>
        public bool SetEliminado(bool eliminado)
        {
            if(eliminado == Eliminado)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            Eliminado = eliminado;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Asigna el adjunto del usuario
        /// </summary>
        /// <param name="usuario">usuario para asociar al correo</param>
        public bool SetUsuario(Usuario usuario)
        {
            if(usuario == Usuario)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            Usuario = usuario;
            UsuarioId = usuario.UsuarioId;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Asigna la respuesta al correo
        /// </summary>
        /// <param name="respuesta">Respuesta a settear</param>
        public bool SetRespuesta(Respuesta respuesta)
        {
            if (respuesta?.RespuestaId == RespuestaId)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            //Respuesta = respuesta;
            RespuestaId = respuesta.RespuestaId;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Marca el correo como leido
        /// </summary>
        /// <param name="contacto">Contacto para asociar al correo</param>
        public bool SetLeido()
        {
            Leido = true;
            SetChanges(OpResul.Line);
            return true;
        }

        public bool SetDestacado()
        {
            Destacado = true;
            SetChanges(OpResul.Line);
            return true;
        }

        public bool QuitarDestacado()
        {
            Destacado = false;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Añade el cuerpo HTML
        /// </summary>
        /// <param name="cuerpoHTML">Cuerpo a añadir al correo</param>
        public bool SetCuepoHTML(string cuerpoHTML)
        {
            if(cuerpoHTML == CuerpoHTML)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            CuerpoHTML = cuerpoHTML;
            SetChanges(OpResul.Line);
            return true;
        }

        public bool SetConver(int converId)
        {
            if (ConversacionId == 0)
            {
                ConversacionId = converId;
                SetChanges(OpResul.Line);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Convierte un MimeMessage en un CorreoCore
        /// </summary>
        /// <param name="mensaje">MimeMesage recibido</param>
        /// <returns>Instancia de correo transformada</returns>
        public static Correo ConvertToCore(MimeMessage mensaje, Contacto? contacto = null)
        {
            if (mensaje == null)
                throw new ArgumentNullException("El correo recibido no puede ser null", nameof(mensaje));

            var correo = new Correo
            {
                MensajeId = mensaje.MessageId,
                Asunto = mensaje.Subject,
                Remitente = mensaje.From.Mailboxes.FirstOrDefault()?.Address ?? string.Empty,
                Destinatarios = new List<string> { mensaje.To.Mailboxes.FirstOrDefault()?.Address ?? string.Empty },
                Cuerpo = mensaje.TextBody ?? mensaje.HtmlBody ?? string.Empty,
                CuerpoHTML = mensaje.HtmlBody,
                FechaRecibido = mensaje.Date.UtcDateTime,
            };

            // Procesar adjuntos (comentado)
            foreach (var fichero in mensaje.Attachments)
            {
                //correo.Adjuntos.Add(Adjunto.CreateAdjunto(correo, fichero?.ContentLocation.ToString()));
            }

            return correo;
        }
    }
}
