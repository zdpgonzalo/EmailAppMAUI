using MailAppMAUI.General;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using MimeKit.Utils;

namespace MailAppMAUI.Core
{
    [Table("Respuestas")]
    [PrimaryKey(nameof(RespuestaId))]
    public class Respuesta : ModelBaseCore<RespuestaDTO>
    {
        /// <summary>
        /// Id del respuesta
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RespuestaId { get; private set; }

        [Required]
        public Guid Guid { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Id para ordenarlos bien en el correo del usuario para que funcione bien en la bandeja de entrada (si están bien colocados, estarán bien leidos)
        /// </summary>
        public string? MensajeId { get; private set; }

        /// <summary>
        /// Id del correo al que esta asociado
        /// </summary>
        /// <remarks>Puede recibir null, porque así puedo crear un mensaje de cero</remarks>
        public int? CorreoId { get; private set; }

        /// <summary>
        /// Id de la conversacion generada
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
        /// Email del destinatario
        /// </summary>
        [Required]
        public List<string> Destinatarios { get; private set; } = new List<string>();

        /// <summary>
        /// Asunto del respuesta
        /// </summary>
        public string? Asunto { get; private set; }

        /// <summary>
        /// Cuerpo del respuesta
        /// </summary>
        public string? Cuerpo { get; private set; }

        /// <summary>
        /// Cuerpo del respuesta HTML
        /// </summary>
        public string? CuerpoHTML { get; private set; }

        /// <summary>
        /// Fecha y hora de cuando se ha enviado la respuesta
        /// </summary>
        public DateTime FechaEnviado { get; private set; }

        /// <summary>
        /// Fecha y hora de cuando se ha procesado la respuesta
        /// </summary>
        public DateTime FechaProcesado { get; private set; }

        /// <summary>
        /// True si el usuario ha aprobado la respuesta
        /// </summary>
        public bool Aprobado { get; private set; }

        /// <summary>
        /// Nombre del destinatario de la respuesta
        /// </summary>
        public string? NombreDestinatario { get; private set; }

        /// <summary>
        /// True si se ha enviado la respuesta al destinatario
        /// </summary>
        public bool Enviado
        {
            get => FechaEnviado != default;
        }

        /// <summary>
        /// True si el usuario ha generado por IA
        /// </summary>
        public bool EsIA { get; private set; } = true;
        public bool EsEliminado { get; private set; } = false;

        /// <summary>
        /// Lista de Adjuntos
        /// </summary>
        public List<Adjunto> Adjuntos { get; set; } = new();

        /// <summary>
        /// True si la respuesta es un Borrador
        /// </summary>
        public bool Borrador { get; private set; } = false;

        /// <summary>
        /// Constructor vacío
        /// </summary>
        private Respuesta() { }

        /// <summary>
        /// Crea una instancia de RespuestaCore
        /// </summary>
        /// <param name="nombre">Nombre de respuesta</param>
        /// <param name="email">Email del respuesta</param>
        /// <returns>Instancia de RespuestaCore nueva</returns>
        /// <exception cref="ArgumentException">
        /// Violacion de las reglas de negocio
        /// El respuesta debe definir un nombre y contraseña
        /// </exception>
        public static Respuesta CreateRespuesta(Correo? correo, string remitente, List<string> destinatarios,
            string? asunto, string? cuerpo, string? cuerpoHTML, DateTime fechaProcesado)
        {
            if (string.IsNullOrEmpty(remitente))
                throw new ArgumentNullException("El remitente no puede estar vacio", nameof(remitente));
            if (destinatarios == null || destinatarios.Count == 0)
                throw new ArgumentNullException("Debe proporcionar al menos un destinatario", nameof(destinatarios));

            var respuesta = new Respuesta()
            {
                CorreoId = correo?.CorreoId,
                Remitente = remitente,
                Destinatarios = destinatarios,
                Asunto = asunto,
                Cuerpo = cuerpo,
                CuerpoHTML = cuerpoHTML,
                FechaProcesado = DateTime.Now,
                MensajeId = GenerateMessageID()
            };

            return respuesta;
        }

        public static Respuesta CreateBorrador(string remitente, List<string>? destinatarios, string? asunto, string? cuerpo, string? cuerpoHTML, DateTime fechaProcesado)
        {
            if (string.IsNullOrEmpty(remitente))
                throw new ArgumentNullException("El remitente no puede estar vacío", nameof(remitente));

            var respuesta = new Respuesta
            {
                CorreoId = null,
                Remitente = remitente,
                Destinatarios = destinatarios ?? new List<string>(),
                Asunto = asunto,
                Cuerpo = cuerpo,
                CuerpoHTML = cuerpoHTML,
                FechaProcesado = fechaProcesado,
                MensajeId = GenerateMessageID()
            };

            return respuesta;
        }

        /// <summary>
        /// Sobrecargado para poder recibir 1, aunque luego lo mete en una lista
        /// </summary>
        /// <param name="correo"></param>
        /// <param name="remitente"></param>
        /// <param name="destinatario"></param>
        /// <param name="asunto"></param>
        /// <param name="cuerpo"></param>
        /// <param name="fechaProcesado"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Respuesta CreateRespuesta(Correo? correo, string remitente, string destinatario,
    string? asunto, string? cuerpo, string? cuerpoHTML, DateTime fechaProcesado)
        {
            if (string.IsNullOrEmpty(remitente))
                throw new ArgumentNullException("El remitente no puede estar vacio", nameof(remitente));
            if (destinatario == null)
                throw new ArgumentNullException("Debe proporcionar al menos un destinatario", nameof(destinatario));

            List<string> destinatarios = new List<string> { destinatario };

            var respuesta = new Respuesta()
            {
                CorreoId = correo?.CorreoId,
                Remitente = remitente,
                Destinatarios = destinatarios,
                Asunto = asunto,
                Cuerpo = cuerpo,
                CuerpoHTML = cuerpoHTML,
                FechaProcesado = DateTime.Now,
                MensajeId = GenerateMessageID(),
            };

            return respuesta;
        }

        public static string GenerateMessageID()
        {
            string randomPart = Guid.NewGuid().ToString("N"); 

            // Usar un dominio válido
            string domain = "infoser.net"; // Reemplaza con tu dominio real

            // Retornar el Message-ID en el formato correcto
            return $"{randomPart}@{domain}";
        }

        /// <inheritdoc/>
        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

            switch (propertyName)
            {
                case nameof(RespuestaId):
                    value = RespuestaId.ToString();
                    break;

                case nameof(CorreoId):

                    if (CorreoId == null)
                    {
                        value = null;
                    }
                    else
                    {
                        value = CorreoId.ToString();
                    }

                    break;

                case nameof(Remitente):
                    value = Remitente;
                    break;

                case nameof(MensajeId):
                    value = MensajeId;
                    break;

                case nameof(Destinatarios):
                    value = string.Join(", ", Destinatarios);
                    break;

                case nameof(ConversacionId):
                    value = ConversacionId.ToString();
                    break;

                case nameof(Asunto):
                    value = Asunto ?? string.Empty;
                    break;

                case nameof(Cuerpo):
                    value = Cuerpo ?? string.Empty;
                    break;

                case nameof(CuerpoHTML):
                    value = CuerpoHTML ?? string.Empty;
                    break;

                case nameof(NombreDestinatario):
                    value = NombreDestinatario ?? string.Empty;
                    break;

                case nameof(FechaEnviado):
                    value = FechaEnviado.ToString();
                    break;

                case nameof(Enviado):
                    value = Enviado.ToString();
                    break;

                case nameof(EsIA):
                    value = EsIA.ToString();
                    break;

                case nameof(Borrador):
                    value = Borrador.ToString();
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Id: {RespuestaId}, Asunto: {Asunto}";
        }

        /// <summary>
        /// Operador explicito que convierte un RespuestaCore en RespuestaDTO
        /// </summary>
        /// <param name="respuesta">Respuesta a convertir</param>
        public static explicit operator RespuestaDTO(Respuesta respuesta)
        {
            if (respuesta == null)
                throw new ArgumentNullException("RespuestaCore no puede ser null", nameof(respuesta));

            return new RespuestaDTO()
            {
                RespuestaId = respuesta.RespuestaId,
                CorreoId = respuesta.CorreoId ?? -1,
                Remitente = respuesta.Remitente,
                Destinatarios = respuesta.Destinatarios,
                Asunto = respuesta.Asunto,
                Cuerpo = respuesta.Cuerpo,
                CuerpoHTML = respuesta.CuerpoHTML,
                FechaEnviado = respuesta.FechaEnviado,
                Enviado = respuesta.Enviado,
                EsIA = respuesta.EsIA,
                Borrador = respuesta.Borrador,
                NombreDestinatario = respuesta.NombreDestinatario ?? string.Empty,
                //ConversacionId = respuesta.ConversacionId,
                MensajeId = respuesta.MensajeId,
                EsEliminado = respuesta.EsEliminado,
            };
        }

        /// <summary>
        /// Convierte un RespuestaDTO en un RespuestaCore
        /// </summary>
        /// <param name="respuestaDTO">RespuestaDTO a convertir</param>
        /// <param name="correo">Correo original. Puede ser null</param>
        /// <returns>Instancia RespuestaCore</returns>
        /// <exception cref="ArgumentNullException">
        /// Error en la logica de negocio. Parametro recibido no puede ser null
        /// </exception>
        public static Respuesta ConvertToCore(RespuestaDTO respuestaDTO, Correo? correo = null)
        {
            if (respuestaDTO == null)
                throw new ArgumentNullException("El registro respuestaDTO no puede ser null", nameof(respuestaDTO));


            Respuesta respuesta = new Respuesta()
            {
                DTO_Base = respuestaDTO,
                Remitente = respuestaDTO.Remitente,
                Destinatarios = respuestaDTO.Destinatarios,
                Asunto = respuestaDTO.Asunto,
                Cuerpo = respuestaDTO.Cuerpo,
                CuerpoHTML = respuestaDTO.CuerpoHTML,
                FechaEnviado = respuestaDTO.FechaEnviado,
                ConversacionId = respuestaDTO.ConversacionId,
                MensajeId = respuestaDTO.MensajeId,
                EsEliminado = respuestaDTO.EsEliminado,
            };

            //Asigna el empleado del respuesta.
            if (correo != null)
            {
                //respuesta.Correo = correo;
            }
            else if (correo == null && respuestaDTO.Correo != null)
            {
                //respuesta.Correo = Correo.ConvertToCore(respuestaDTO.Correo);
            }
            else if (correo == null && respuestaDTO.Correo == null) //Si ambos son null --> la respuesta no tiene correo asociado
            {
                //Pa que no de error porque si ambos son null significa que es un correo enviado desde Nuevo Correo
            }
            else
            {
                throw new NotImplementedException("Asignar correo por Id no implementado");
            }

            respuesta.SetAdjuntos(respuestaDTO.Adjuntos);

            return respuesta;
        }


        /// <summary>
        /// Operador explicito que convierte un Eliminado en Respuesta
        /// </summary>
        /// <param name="respuesta">Respuesta a convertir</param>
        //public static explicit operator Respuesta(Eliminado eliminado)
        //{
        //    if (eliminado == null)
        //        throw new ArgumentNullException("Correo no puede ser null", nameof(eliminado));
        //    if (eliminado.UsuarioId == null)
        //        throw new ArgumentNullException("Usuario no puede ser null", nameof(eliminado));

        //    Respuesta miEliminado = new Respuesta()
        //    {
        //        Remitente = eliminado.Remitente,
        //        Destinatarios = eliminado.Destinatarios,
        //        NombreDestinatario = eliminado?.NombreDestinatario,
        //        Asunto = eliminado.Asunto,
        //        Cuerpo = eliminado.Cuerpo,
        //        CuerpoHTML = eliminado.CuerpoRespuestaHTML,

        //        //----------DATOS RESPUESTAS----------
        //        FechaEnviado = eliminado.FechaEnviado,
        //        FechaProcesado = eliminado.FechaProcesado,
        //        EsIA = eliminado.EsIA,
        //        Borrador = eliminado.Borrador,

        //        Adjuntos = eliminado.Adjuntos,
        //    };

        //    return miEliminado;
        //}
        public bool SetAdjuntos(List<AdjuntoDTO> adjuntosDTOList)
        {
            Adjuntos.Clear();

            foreach (var adjunto in adjuntosDTOList)
            {
                Adjuntos.Add(Adjunto.ConvertToCore(adjunto));
            }

            SetChanges(OpResul.Page);
            return true;
        }

        public bool SetAdjuntos(List<Adjunto> adjuntosList)
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
        /// Añade adjuntos
        /// </summary>
        /// <param name="adjunto">Adjunto a añadir</param>
        /// <returns>True si realiza el cambio, false en caso contrario</returns>
        public bool ChangeAdjunto(Adjunto adjunto)
        {
            Adjuntos.Add(adjunto);

            SetChanges(OpResul.Line);

            return true;
        }

        /// <summary>
        /// Cambia el asunto de la respuesta del email
        /// </summary>
        /// <param name="asunto">Nuevo asunto de la respuesta</param>
        /// <returns>True si realiza el cambio, false en caso contrario</returns>
        public bool ChangeAsunto(string asunto)
        {
            if(asunto == Asunto)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            Asunto = asunto;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Cambia el cuerpo de la respuesta del email
        /// </summary>
        /// <param name="nuevoCuerpo">Nuevo cuerpo de la respuesta</param>
        /// <returns>True si realiza el cambio, false en caso contrario</returns>
        public bool ChangeCuerpo(string nuevoCuerpo)
        {
            if(nuevoCuerpo == Cuerpo && nuevoCuerpo == CuerpoHTML)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            Cuerpo = nuevoCuerpo;
            CuerpoHTML = nuevoCuerpo;

            SetChanges(OpResul.Line);

            return true;
        }

        /// <summary>
        /// Cambia el nombre del destinatario
        /// </summary>
        /// <param name="nombreDest">Nuevo dest</param>
        /// <returns>True si realiza el cambio, false en caso contrario</returns>
        public bool ChangeNombreDestinatario(string nombreDest)
        {
            if(nombreDest == NombreDestinatario)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            NombreDestinatario = nombreDest;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Cambia el Destinatario de la Respuesta
        /// </summary>
        /// <param name="nuevoDestinatrio">Nuevo Destinatario de la Respuesta</param>
        /// <returns></returns>
        public bool ChangeDestinatarios(List<string> nuevosDestinatarios)
        {
            Destinatarios = nuevosDestinatarios;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Cambia los parametros de Asunto, Cuerpo y FechaProcesado de 
        /// la respuesta
        /// </summary>
        /// <param name="respuesta">Respuesta de la que copiar los parametros</param>
        /// <returns>True si realiza el cambio, false en caso contrario</returns>
        public bool ChangeRespuesta(Respuesta respuesta)
        {
            if (respuesta == null)
            {
                SetChanges(OpResul.Cancel, WindowType.None);
                return false;
            }

            this.Destinatarios = respuesta.Destinatarios;
            this.Asunto = respuesta.Asunto;
            this.Cuerpo = respuesta.Cuerpo;
            this.CuerpoHTML = respuesta.CuerpoHTML;
            this.FechaProcesado = respuesta.FechaProcesado;

            SetChanges(OpResul.Docum);

            return true;
        }

        /// <summary>
        /// Cambia la fecha de envio 
        /// la respuesta
        /// </summary>
        /// <param name="fecha" nueva fecha</param>
        /// <returns>True si realiza el cambio, false en caso contrario</returns>
        public bool ChangeFechaEnviado(DateTime fecha)
        {
            if(fecha == FechaEnviado)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            FechaEnviado = fecha;
            SetChanges(OpResul.Docum);
            return true;
        }

        /// <summary>
        /// Marca una Respuesta como Borrador
        /// </summary>
        /// <returns></returns>
        public bool ChangeBorrador(bool borrador)
        {
            Borrador = borrador;
            EsIA = false;
            SetChanges(OpResul.Line);
            return true;
        }

        public bool ChangeEsIA(bool newValue)
        {
            EsIA = newValue;
            SetChanges(OpResul.Line);
            return true;
        }

        public bool ChangeEsEliminado(bool newValue)
        {
            EsEliminado = newValue;
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
    }
}
