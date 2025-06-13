using MailAppMAUI.General;
using MailAppMAUI.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.DTOs
{
    [Table("Eliminados")]
    [PrimaryKey(nameof(EliminadoId))]
    public class EliminadoDTO : BaseDTO, IComparable<EliminadoDTO>
    {
        /// <summary>
        /// Id del eliminado
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EliminadoId { get; set; }

        /// <summary>
        /// Email del remitente
        /// </summary>
        [Required]
        public string Remitente { get; set; }

        public bool EsCorreo { get; set; } = false;

        /// <summary>
        /// Email del destinatario
        /// </summary>
        [Required]
        public List<string> Destinatarios { get; set; } = new List<string>();

        /// <summary>
        /// Nombre del destinatario
        /// </summary>
        public string NombreDestinatario { get; set; }

        /// <summary>
        /// Asunto del eliminado
        /// </summary>
        public string? Asunto { get; set; }

        /// <summary>
        /// Cuerpo del eliminado
        /// </summary>
        public string? Cuerpo { get; set; }

        /// <summary>
        /// Cuerpo HTML del eliminado
        /// </summary>
        public string? CuerpoCorreoHTML { get; set; }
        /// <summary>
        /// Cuerpo HTML del eliminado
        /// </summary>
        public string? CuerpoRespuestaHTML { get; set; }

        /// <summary>
        /// Id original obtenido del MimeMessage
        /// </summary>
        public string? MensajeId { get; set; }

        /// <summary>
        /// Id del usuario al que esta asociado
        /// </summary>
        public int? UsuarioId { get; set; }

        /// <summary>
        /// Usuario al que pertenece
        /// </summary>
        public UsuarioDTO? Usuario { get; set; }

        /// <summary>
        /// True si se ha leido
        /// </summary>
        public bool? Leido { get; set; }

        /// <summary>
        /// Fecha y hora de cuando se ha recibido el correo
        /// </summary>
        public DateTime? FechaRecibido { get; set; }

        //----------DATOS RESPUESTAS----------

        /// <summary>
        /// Fecha y hora de cuando se ha enviado la respuesta
        /// </summary>
        public DateTime? FechaEnviado { get; set; }

        /// <summary>
        /// Fecha y hora de cuando se ha procesado la respuesta
        /// </summary>
        public DateTime? FechaProcesado { get; set; }

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
        public bool? EsIA { get; set; } = true;

        /// <summary>
        /// True si la respuesta es un Borrador
        /// </summary>
        public bool? Borrador { get; set; } = false;

        /// <summary>
        /// Constructor vacío
        /// </summary>

        //----------OTRO ELIMINADO----------
        //Si eliminado 1 tiene a eliminado 2, significa que es un correo (E1) con una respuesta (E2). Si esa respuesta tiene otro eliminado (E3) significa que es la respuesta de la respuesta. --> Si es par es Respuesta si es Impar es correo

        /// <summary>
        /// Id del otro eliminado
        /// </summary>
        /// <remarks>Puede recibir null</remarks>
        public int? RespuestaEliminadaId { get;  set; }

        //Adjuntos del eliminado PRINCIPAL
        public List<Adjunto> Adjuntos { get; set; } = new();

        public EliminadoDTO() : base()
        {
            Tipo = TipoEntidad.Eliminado;
        }

        /// <inheritdoc/>
        public override void CopyFrom(BaseDTO dto)
        {
            base.CopyFrom(dto);
            EliminadoDTO eliminado = (EliminadoDTO)dto;
            EsCorreo = eliminado.EsCorreo;
            Remitente = eliminado.Remitente;
            Destinatarios = eliminado.Destinatarios;
            Asunto = eliminado?.Asunto;
            Cuerpo = eliminado?.Cuerpo;
            CuerpoCorreoHTML = eliminado?.CuerpoCorreoHTML;
            CuerpoRespuestaHTML = eliminado?.CuerpoRespuestaHTML;
            MensajeId = eliminado?.MensajeId;
            UsuarioId = eliminado.UsuarioId;
            Usuario = eliminado.Usuario;
            Leido = eliminado.Leido;
            FechaRecibido = eliminado.FechaRecibido;
            Adjuntos = eliminado.Adjuntos;

            NombreDestinatario = eliminado?.NombreDestinatario;
            RespuestaEliminadaId = eliminado.RespuestaEliminadaId;
        }

        public int CompareTo(EliminadoDTO? other)
        {
            return EliminadoId.CompareTo(other?.EliminadoId);
        }

        ///<inheritdoc/>
        public override BaseDTO ImportData(object[] filas, string[] columnas)
        {
            var eliminado = new EliminadoDTO();

            for (int i = 0; i < columnas.Length; i++)
            {
                if (columnas[i] == null) break;

                switch (columnas[i])
                {
                    case nameof(Remitente):
                        eliminado.Remitente = Data.ToString(filas[i]);
                        break;

                    case nameof(Destinatarios):
                        {
                            string destinatariosStr = Data.ToString(filas[i]);
                            eliminado.Destinatarios = string.IsNullOrWhiteSpace(destinatariosStr)
                                ? new List<string>()
                                : destinatariosStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(s => s.Trim())
                                                  .ToList();
                            break;
                        }

                    case nameof(Asunto):
                        eliminado.Asunto = Data.ToString(filas[i]);
                        break;

                    case nameof(Cuerpo):
                        eliminado.Cuerpo = Data.ToString(filas[i]);
                        break;

                    case nameof(CuerpoRespuestaHTML):
                        eliminado.CuerpoRespuestaHTML = Data.ToString(filas[i]);
                        break;

                    case nameof(CuerpoCorreoHTML):
                        eliminado.CuerpoCorreoHTML = Data.ToString(filas[i]);
                        break;

                    case nameof(UsuarioId):
                        eliminado.UsuarioId = Data.ToInt(filas[i]);
                        break;

                    case nameof(RespuestaEliminadaId):
                        eliminado.RespuestaEliminadaId = Data.ToInt(filas[i]);
                        break;

                    case nameof(FechaRecibido):
                        eliminado.FechaRecibido = Convert.ToDateTime(Data.ToString(filas[i]));
                        break;

                    case nameof(Leido):
                        eliminado.Leido = Data.ToBool(filas[i]);
                        break;

                    case nameof(FechaEnviado):
                        eliminado.FechaEnviado = Convert.ToDateTime(Data.ToString(filas[i]));
                        break;

                    case nameof(EsIA):
                        eliminado.EsIA = Data.ToBool(filas[i]);
                        break;

                    case nameof(EsCorreo):
                        eliminado.EsCorreo = Data.ToBool(filas[i]);
                        break;

                    case nameof(Borrador):
                        eliminado.Borrador = Data.ToBool(filas[i]);
                        break;

                    case nameof(NombreDestinatario):
                        eliminado.NombreDestinatario = Data.ToString(filas[i]);
                        break;

                    default:
                        continue;
                }
            }
            return eliminado;
        }

        ///<inheritdoc/>
        public override bool GetValue(string propertyName, out string value)
        {
            if (base.GetValue(propertyName, out value)) return true;

            switch (propertyName)
            {
                case nameof(RespuestaEliminadaId):

                    if (RespuestaEliminadaId == null)
                    {
                        value = null;
                    }
                    else
                    {
                        value = RespuestaEliminadaId.ToString();
                    }
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

                case nameof(CuerpoCorreoHTML):
                    value = CuerpoCorreoHTML ?? string.Empty;
                    break;

                case nameof(CuerpoRespuestaHTML):
                    value = CuerpoRespuestaHTML ?? string.Empty;
                    break;

                case nameof(NombreDestinatario):
                    value = NombreDestinatario.ToString();
                    break;

                case nameof(UsuarioId):
                    value = UsuarioId.ToString();
                    break;

                case nameof(FechaRecibido):
                    value = FechaRecibido.ToString();
                    break;

                case nameof(Leido):
                    value = Leido.ToString();
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

        public static new Type GetType()
        {
            return typeof(RespuestaDTO);
        }

        public override string? ToString()
        {
            return $"";
        }
    }
}
