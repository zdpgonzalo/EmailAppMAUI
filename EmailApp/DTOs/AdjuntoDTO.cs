using MailAppMAUI.General;
using MailAppMAUI.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.DTOs
{
    [Table("Adjuntos")]
    [PrimaryKey(nameof(AdjuntoId))]
    public class AdjuntoDTO : BaseDTO, IComparable<AdjuntoDTO>
    {
        /// <summary>
        /// Id del ficheroAdjunto
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdjuntoId { get; set; }

        /// <summary>
        /// Id del correo al que pertenece
        /// </summary>
        public int? CorreoId { get; set; }

        /// <summary>
        /// Correo al que pertenece el fichero
        /// </summary>
        [ForeignKey(nameof(CorreoId))]
        public CorreoDTO? Correo { get; set; }

        /// <summary>
        /// Id de la respuesta al que pertenece
        /// </summary>
        public int? RespuestaId { get; set; }

        /// <summary>
        /// Respuesta al que pertenece el fichero
        /// </summary>
        [ForeignKey(nameof(RespuestaId))]
        public RespuestaDTO? Respuesta { get; set; }

        /// <summary>
        /// Nombre del Fichero
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Ruta del fichero adjunto
        /// </summary>
        [Required]
        public string Ruta { get; set; } = string.Empty;

        /// <summary>
        /// Extension del fichero adjunto
        /// </summary>
        [Required]
        public string Extension { get; set; } = string.Empty;

        public AdjuntoDTO() : base()
        {
            Tipo = TipoEntidad.Adjunto;
        }

        /// <inheritdoc/>
        public override void CopyFrom(BaseDTO dto)
        {
            base.CopyFrom(dto);
            AdjuntoDTO usuario = (AdjuntoDTO)dto;
            AdjuntoId = usuario.AdjuntoId;
            CorreoId = usuario.CorreoId;
            Correo = usuario.Correo;
            RespuestaId = usuario.RespuestaId;
            Respuesta = usuario.Respuesta;
            Nombre = usuario.Nombre;
            Ruta = usuario.Ruta;
            Extension = usuario.Extension;
        }

        public int CompareTo(AdjuntoDTO? other)
        {
            return AdjuntoId.CompareTo(other?.AdjuntoId);
        }

        ///<inheritdoc/>
        public override BaseDTO ImportData(object[] filas, string[] columnas)
        {
            var usuario = new AdjuntoDTO();

            for (int i = 0; i < columnas.Length; i++)
            {
                if (columnas[i] == null) break;

                switch (columnas[i])
                {
                    case nameof(AdjuntoId):
                        usuario.AdjuntoId = Data.ToInt(filas[i]);
                        break;

                    case nameof(CorreoId):
                        usuario.CorreoId = Data.ToInt(filas[i]);
                        break;

                    case nameof(RespuestaId):
                        usuario.RespuestaId = Data.ToInt(filas[i]);
                        break;

                    case nameof(Nombre):
                        usuario.Nombre = Data.ToString(filas[i]);
                        break;

                    case nameof(Ruta):
                        usuario.Ruta = Data.ToString(filas[i]) ?? string.Empty;
                        break;

                    case nameof(Extension):
                        usuario.Extension = Data.ToString(filas[i]);
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
                case nameof(AdjuntoId):
                    value = AdjuntoId.ToString();
                    break;

                case nameof(CorreoId):
                    value = CorreoId.ToString() ?? string.Empty;
                    break;

                case nameof(RespuestaId):
                    value = RespuestaId.ToString() ?? string.Empty;
                    break;

                case nameof(Nombre):
                    value = Nombre ?? string.Empty;
                    break;

                case nameof(Ruta):
                    value = Ruta.ToString();
                    break;

                case nameof(Extension):
                    value = Extension.ToString();
                    break;

                default:
                    value = string.Empty;
                    return false;
            }

            return true;
        }

        public static new Type GetType()
        {
            return typeof(AdjuntoDTO);
        }

        public override string? ToString()
        {
            return $"Id: {AdjuntoId}, CorreoId: {CorreoId}";
        }
    }
}
