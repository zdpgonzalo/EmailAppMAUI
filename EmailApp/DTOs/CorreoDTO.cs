using MailAppMAUI.General;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.DTOs
{
    [Table("Correos")]
    [PrimaryKey(nameof(CorreoId))]
    public class CorreoDTO : BaseDTO, IComparable<CorreoDTO>
    {
        /// <summary>
        /// Id del correo
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CorreoId { get; set; }

        /// <summary>
        /// Id de la conver
        /// </summary>
        public int ConversacionId { get; set; }

        /// <summary>
        /// Id original obtenido del MimeMessage
        /// </summary>
        public string? MensajeId { get; set; }

        /// <summary>
        /// Id del usuario al que pertenece
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Usuario al que pertenece
        /// </summary>
        public UsuarioDTO Usuario { get; set; }

        /// <summary>
        /// Id de la respuesta generada
        /// </summary>
        public int? RespuestaId { get; set; }


        /// <summary>
        /// Email de la persona que envia el correo
        /// </summary>
        [Required]
        public string Remitente { get; set; } = string.Empty;

        /// <summary>
        /// Lista de emails de destinatarios
        /// </summary>
        [Required]
        public List<string> Destinatarios { get; set; } = new List<string>();

        /// <summary>
        /// Asunto del correo
        /// </summary>
        public string? Asunto { get; set; }

        /// <summary>
        /// Contenido del cuerpo del correo
        /// </summary>
        public string? Cuerpo { get; set; }

        /// <summary>
        /// ContenidoHTML del cuerpo del correo
        /// </summary>
        public string? CuerpoHTML { get; set; }

        /// <summary>
        /// Fecha de cuando se ha recibido el correo
        /// </summary>
        public DateTime FechaRecibido { get; set; }

        /// <summary>
        /// Indica si se ha mandado a procesar a la IA
        /// </summary>
        public bool Procesado { get; set; }

        /// <summary>
        /// Indica si se ha leido el correo
        /// </summary>
        public bool Leido { get; set; }

        /// <summary>
        /// Indica si se ha eliminado el correo
        /// </summary>
        public bool Eliminado { get; set; }

        public bool Destacado { get; set; }

        /// <summary>
        /// Lista de ficheros adjuntos en el correo
        /// </summary>
        public List<AdjuntoDTO> Adjuntos { get; set; } = new();

        public CorreoDTO() : base()
        {
            Tipo = TipoEntidad.Correo;
        }

        public override void CopyFrom(BaseDTO dto)
        {
            base.CopyFrom(dto);
            CorreoDTO correo = (CorreoDTO)dto;
            CorreoId = correo.CorreoId;
            MensajeId = correo.MensajeId;
            UsuarioId = correo.UsuarioId;
            Usuario = correo.Usuario;
            RespuestaId = correo.RespuestaId;
            Remitente = correo.Remitente;
            Destinatarios = correo.Destinatarios; // Asignación de la lista
            Asunto = correo.Asunto;
            Cuerpo = correo.Cuerpo;
            FechaRecibido = correo.FechaRecibido;
            Procesado = correo.Procesado;
            Adjuntos = correo.Adjuntos;
            Leido = correo.Leido;
            Destacado = correo.Destacado;
            CuerpoHTML = correo.CuerpoHTML;
            ConversacionId = correo.ConversacionId;
            Eliminado = correo.Eliminado;
        }

        public int CompareTo(CorreoDTO? other)
        {
            return CorreoId.CompareTo(other?.CorreoId);
        }

        public override BaseDTO ImportData(object[] filas, string[] columnas)
        {
            var correo = new CorreoDTO();

            for (int i = 0; i < columnas.Length; i++)
            {
                if (columnas[i] == null) break;

                switch (columnas[i])
                {
                    case nameof(CorreoId):
                        correo.CorreoId = Data.ToInt(filas[i]);
                        break;

                    case nameof(MensajeId):
                        correo.MensajeId = Data.ToString(filas[i]);
                        break;
                
                    case nameof(UsuarioId):
                        correo.UsuarioId = Data.ToInt(filas[i]);
                        break;
                
                    case nameof(ConversacionId):
                        correo.ConversacionId = Data.ToInt(filas[i]);
                        break;

                    case nameof(Eliminado):
                        correo.Eliminado = Data.ToBool(filas[i]);
                        break;

                    case nameof(RespuestaId):
                        correo.RespuestaId = Data.ToInt(filas[i]);
                        break;
                
                    case nameof(Remitente):
                        correo.Remitente = Data.ToString(filas[i]);
                        break;

                    case nameof(Destinatarios):
                        {
                            string destinatariosStr = Data.ToString(filas[i]);
                            correo.Destinatarios = string.IsNullOrWhiteSpace(destinatariosStr)
                                ? new List<string>()
                                : destinatariosStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(s => s.Trim())
                                                  .ToList();
                            break;
                        }

                    case nameof(Asunto):
                        correo.Asunto = Data.ToString(filas[i]);
                        break;
                
                    case nameof(Cuerpo):
                        correo.Cuerpo = Data.ToString(filas[i]);
                        break;

                    case nameof(CuerpoHTML):
                        correo.CuerpoHTML = Data.ToString(filas[i]);
                        break;

                    case nameof(FechaRecibido):
                        correo.FechaRecibido = Convert.ToDateTime(Data.ToString(filas[i]));
                        break;
                
                    case nameof(Procesado):
                        correo.Procesado = Data.ToBool(filas[i]);
                        break;

                    case nameof(Leido):
                        correo.Leido = Data.ToBool(filas[i]);
                        break;

                    case nameof(Destacado):
                        correo.Destacado = Data.ToBool(filas[i]);
                        break;
                    default:
                        continue;
                }
            }
            return correo;
        }

        public override bool GetValue(string propertyName, out string value)
        {
            if (base.GetValue(propertyName, out value)) return true;

            switch (propertyName)
            {
                case nameof(CorreoId):
                    value = CorreoId.ToString();
                    break;

                case nameof(MensajeId):
                    value = MensajeId.ToString();
                    break;

                case nameof(UsuarioId):
                    value = UsuarioId.ToString();
                    break;

                case nameof(ConversacionId):
                    value = ConversacionId.ToString();
                    break;

                case nameof(RespuestaId):
                    value = RespuestaId?.ToString() ?? string.Empty;
                    break;

                case nameof(Remitente):
                    value = Remitente;
                    break;

                case nameof(Destinatarios):
                    value = string.Join(", ", Destinatarios);
                    break;

                case nameof(Asunto):
                    value = Asunto ?? string.Empty;
                    break;
                    
                case nameof(Cuerpo):
                    value = Cuerpo ?? string.Empty;
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

                case nameof(Eliminado):
                    value = Eliminado.ToString();
                    break;

                case nameof(Destacado):
                    value = Destacado.ToString();
                    break;

                case nameof(CuerpoHTML):
                    value = CuerpoHTML.ToString();
                    break;

                default:
                    value = string.Empty;
                    return false;
            }

            return true;
        }

        public static new Type GetType()
        {
            return typeof(CorreoDTO);
        }

        public override string? ToString()
        {
            return $"Id: {CorreoId}, Asunto {Asunto}";
        }
    }
}
