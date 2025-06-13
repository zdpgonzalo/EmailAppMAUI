using MailAppMAUI.General;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.DTOs
{
    [Table("Respuestas")]
    [PrimaryKey(nameof(RespuestaId))]
    public class RespuestaDTO : BaseDTO, IComparable<RespuestaDTO>
    {
        /// <summary>
        /// Id del respuesta
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RespuestaId { get; set; }

        /// <summary>
        /// Id original obtenido del MimeMessage
        /// </summary>
        public string? MensajeId { get; set; }

        /// <summary>
        /// Id de la conversacion
        /// </summary>
        public int ConversacionId { get; set; }

        /// <summary>
        /// Id de la respuesta generada
        /// </summary>
        public int CorreoId { get; set; }

        /// <summary>
        /// Respuesta generada
        /// </summary>
        [ForeignKey(nameof(RespuestaId))]
        public CorreoDTO? Correo { get; set; }

        /// <summary>
        /// Email de la persona que envia el respuesta
        /// </summary>
        [Required]
        public string Remitente { get; set; } = string.Empty;

        /// <summary>
        /// Email de la persona que recibe el respuesta
        /// </summary>
        [Required]
        public List<string> Destinatarios { get; set; } = new List<string>();

        /// <summary>
        /// Asunto del respuesta
        /// </summary>
        public string? Asunto { get; set; }

        /// <summary>
        /// Contenido del cuerpo del respuesta
        /// </summary>
        public string? Cuerpo { get; set; }
        public string? CuerpoHTML { get; set; }

        /// <summary>
        /// Nombre del destinatario de la respuesta
        /// </summary>
        public string? NombreDestinatario { get; set; }

        /// <summary>
        /// Fecha de cuando se ha procesado la respuesta
        /// </summary>
        public DateTime FechaProcesado { get; set; }

        /// <summary>
        /// Fecha de cuando se ha enviado la respuesta
        /// </summary>
        public DateTime FechaEnviado { get; set; }

        /// <summary>
        /// Indica si se ha mandado a procesar a la IA
        /// </summary>
        public bool Aprobado { get; set; }

        /// <summary>
        /// Indica si se ha enviado la respuesta
        /// </summary>
        public bool Enviado { get; set; }

        /// <summary>
        /// Indica si la Respuesta es generada por IA
        /// </summary>
        public bool EsIA { get; set; }
        public bool EsEliminado { get; set; }

        /// <summary>
        /// Indica si la Respuesta es Borrador
        /// </summary>
        public bool Borrador { get; set; }

        /// <summary>
        /// Lista de ficheros adjuntos en el correo
        /// </summary>
        public List<AdjuntoDTO> Adjuntos { get; set; } = new();

        public RespuestaDTO() : base()
        {
            Tipo = TipoEntidad.Respuesta;
        }

        /// <inheritdoc/>
        public override void CopyFrom(BaseDTO dto)
        {
            base.CopyFrom(dto);
            RespuestaDTO respuesta = (RespuestaDTO)dto;
            RespuestaId = respuesta.RespuestaId;
            CorreoId = respuesta.RespuestaId;
            Correo = respuesta.Correo;
            Remitente = respuesta.Remitente;
            Destinatarios = respuesta.Destinatarios;
            Asunto = respuesta.Asunto;
            Cuerpo = respuesta.Cuerpo;
            CuerpoHTML = respuesta.CuerpoHTML;
            FechaEnviado = respuesta.FechaEnviado;
            FechaProcesado = respuesta.FechaProcesado;
            Aprobado = respuesta.Aprobado;
            Adjuntos = respuesta.Adjuntos;
            Enviado = respuesta.Enviado;
            Borrador = respuesta.Borrador;
            EsIA = respuesta.EsIA;
            EsEliminado = respuesta.EsEliminado;
            ConversacionId = respuesta.ConversacionId;
            NombreDestinatario = respuesta.NombreDestinatario ?? string.Empty;
            MensajeId = respuesta.MensajeId;
        }

        public int CompareTo(RespuestaDTO? other)
        {
            return RespuestaId.CompareTo(other?.RespuestaId);
        }

        ///<inheritdoc/>
        public override BaseDTO ImportData(object[] filas, string[] columnas)
        {
            var usuario = new RespuestaDTO();

            for (int i = 0; i < columnas.Length; i++)
            {
                if (columnas[i] == null) break;

                switch (columnas[i])
                {
                    case nameof(RespuestaId):
                        usuario.RespuestaId = Data.ToInt(filas[i]);
                        break;

                    case nameof(CorreoId):
                        usuario.CorreoId = Data.ToInt(filas[i]);
                        break;

                    case nameof(Remitente):
                        usuario.Remitente = Data.ToString(filas[i]);
                        break;                    
                    
                    case nameof(MensajeId):
                        usuario.MensajeId = Data.ToString(filas[i]);
                        break;

                    case nameof(ConversacionId):
                        usuario.ConversacionId = Data.ToInt(filas[i]);
                        break;

                    case nameof(Destinatarios):
                        {
                            string destinatariosStr = Data.ToString(filas[i]);
                            // Si el string es vacío, asigna una lista vacía.
                            usuario.Destinatarios = string.IsNullOrWhiteSpace(destinatariosStr)
                                ? new List<string>()
                                : destinatariosStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(s => s.Trim())
                                                  .ToList();
                            break;
                        }


                    case nameof(NombreDestinatario):
                        usuario.NombreDestinatario = Data.ToString(filas[i]);
                        break;

                    case nameof(Asunto):
                        usuario.Asunto = Data.ToString(filas[i]);
                        break;

                    case nameof(Cuerpo):
                        usuario.Cuerpo = Data.ToString(filas[i]);
                        break;

                    case nameof(CuerpoHTML):
                        usuario.CuerpoHTML = Data.ToString(filas[i]);
                        break;

                    case nameof(FechaProcesado):
                        usuario.FechaProcesado = Convert.ToDateTime(Data.ToString(filas[i]));
                        break;

                    case nameof(FechaEnviado):
                        usuario.FechaEnviado = Convert.ToDateTime(Data.ToString(filas[i]));
                        break;

                    case nameof(Enviado):
                        usuario.Enviado = Data.ToBool(filas[i]);
                        break;

                    case nameof(Aprobado):
                        usuario.Aprobado = Data.ToBool(filas[i]);
                        break;

                    case nameof(Borrador):
                        usuario.Borrador = Data.ToBool(filas[i]);
                        break;

                    case nameof(EsIA):
                        usuario.EsIA = Data.ToBool(filas[i]);
                        break;

                    case nameof(EsEliminado):
                        usuario.EsEliminado = Data.ToBool(filas[i]);
                        break;

                    default:
                        continue;
                }
            }
            return usuario;
        }

        ///<inheritdoc/>
        public override bool GetValue(string propertyName, out string value)
        {
            if (base.GetValue(propertyName, out value)) return true;

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
                    value = Remitente.ToString();
                    break;

                case nameof(MensajeId):
                    value = MensajeId = ToString();
                    break;

                case nameof(ConversacionId):
                    value = ConversacionId.ToString();
                    break;

                case nameof(Destinatarios):
                    value = string.Join(", ", Destinatarios);
                    break;

                case nameof(NombreDestinatario):
                    value = NombreDestinatario;
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

                case nameof(FechaProcesado):
                    value = FechaProcesado.ToString();
                    break;

                case nameof(FechaEnviado):
                    value = FechaEnviado.ToString();
                    break;

                case nameof(Aprobado):
                    value = Aprobado.ToString();
                    break;

                case nameof(Enviado):
                    value = Enviado.ToString();
                    break;

                case nameof(EsIA):
                    value = EsIA.ToString();
                    break;

                case nameof(EsEliminado):
                    value = EsEliminado.ToString();
                    break;

                case nameof(Borrador):
                    value = Borrador.ToString();
                    break;


                default:
                    value = string.Empty;
                    return false;
            }

            return true;
        }

        public static new Type GetType()
        {
            return typeof(RespuestaDTO);
        }

        public override string? ToString()
        {
            return $"Id: {RespuestaId}, CorreoId: {Correo},  Asunto {Asunto}";
        }
    }
}
