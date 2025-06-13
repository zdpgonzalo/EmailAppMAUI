using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.Core
{
    [Table("Adjuntos")]
    [PrimaryKey(nameof(AdjuntoId))]
    public class Adjunto : ModelBaseCore<AdjuntoDTO>
    {
        //ESTE ADJUNTO PUEDE PERTENECER A UN CORREO O A UNA RESPUESTA

        /// <summary>
        /// Id del adjunto
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdjuntoId { get; private set; }

        /// <summary>
        /// Id del correo al que pertenece
        /// </summary>
        public int? CorreoId { get; private set; }

        /// <summary>
        /// Correo al que pertenece el fichero
        /// </summary>
        [ForeignKey(nameof(CorreoId))]
        public Correo? Correo { get; private set; }

        /// <summary>
        /// Id de la respuesta al que pertenece
        /// </summary>
        public int? RespuestaId { get; private set; }

        /// <summary>
        /// Respuesta al que pertenece el fichero
        /// </summary>
        [ForeignKey(nameof(RespuestaId))]
        public Respuesta? Respuesta { get; private set; }

        /// <summary>
        /// Nombre de adjunto
        /// </summary>
        public string Nombre { get; private set; }

        /// <summary>
        /// Ruta de almacenamiento del fichero adjunto
        /// </summary>
        [Required]
        public string Ruta { get; private set; }

        /// <summary>
        /// Extension del fichero
        /// </summary>
        [Required]
        public string Extension { get; private set; }

        /// <summary>
        /// Constructor vacío
        /// </summary>
        private Adjunto() { }

        /// <summary>
        /// Crea una instancia de AdjuntoCore
        /// </summary>
        /// <param name="nombre">Nombre de adjunto</param>
        /// <param name="password">Contraseña de adjunto</param>
        /// <returns>Instancia de AdjuntoCore nueva</returns>
        /// <exception cref="ArgumentException">
        /// Violacion de las reglas de negocio
        /// El adjunto debe definir un nombre y contraseña
        /// </exception>
        public static Adjunto CreateAdjunto(Correo correo, string rutaCompleta)
        {
            if (correo == null)
                throw new ArgumentException("El correo no puede estar vacio", nameof(correo));
            if (string.IsNullOrEmpty(rutaCompleta))
                throw new ArgumentException("La ruta no puede estar vacia", nameof(rutaCompleta));

            var adjunto = new Adjunto()
            {
                Correo = correo,
                CorreoId = correo.CorreoId,
                Ruta = rutaCompleta,
                Nombre = Path.GetFileName(rutaCompleta),
                Extension = Path.GetExtension(rutaCompleta),
            };

            return adjunto;
        }

        public static Adjunto CreateAdjunto(Respuesta respuesta, string rutaCompleta)
        {
            if (respuesta == null)
                throw new ArgumentException("El correo no puede estar vacio", nameof(respuesta));
            if (string.IsNullOrEmpty(rutaCompleta))
                throw new ArgumentException("La ruta no puede estar vacia", nameof(rutaCompleta));

            var adjunto = new Adjunto()
            {
                Respuesta = respuesta,
                RespuestaId = respuesta.RespuestaId,
                Ruta = rutaCompleta,
                Nombre = Path.GetFileName(rutaCompleta),
                Extension = Path.GetExtension(rutaCompleta),
            };

            return adjunto;
        }

        /// <inheritdoc/>
        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

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
                    value = Ruta;
                    break;

                case nameof(Extension):
                    value = Extension;
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Id: {AdjuntoId}, Nombre: {Nombre}";
        }

        /// <summary>
        /// Operador explicito que convierte un AdjuntoCore en AdjuntoDTO
        /// </summary>
        /// <param name="adjunto">Adjunto a convertir</param>
        public static explicit operator AdjuntoDTO(Adjunto adjunto)
        {
            if (adjunto == null)
                throw new ArgumentNullException("AdjuntoCore no puede ser null");

            return new AdjuntoDTO()
            {
                AdjuntoId = adjunto.AdjuntoId,
                Nombre = adjunto.Nombre,
                CorreoId = adjunto.CorreoId,
                Correo = adjunto.Correo != null ? (CorreoDTO)adjunto.Correo : null,
                RespuestaId = adjunto.RespuestaId,
                Respuesta = adjunto.Respuesta != null ? (RespuestaDTO)adjunto.Respuesta : null,
                Ruta = adjunto.Ruta,
                Extension = adjunto.Extension
            };
        }

        /// <summary>
        /// Convierte un AdjuntoDTO en un AdjuntoCore
        /// </summary>
        /// <param name="adjuntoDTO">AdjuntoDTO a convertir</param>
        /// <returns>Instancia AdjuntoCore</returns>
        /// <exception cref="ArgumentNullException">
        /// Error en la logica de negocio. Parametro recibido no puede ser null
        /// </exception>
        public static Adjunto ConvertToCore(AdjuntoDTO adjuntoDTO, Correo? correo = null)
        {
            if (adjuntoDTO == null)
            {
                throw new ArgumentNullException("El registro adjuntoDTO no puede ser null", nameof(adjuntoDTO));
            }
            if (correo == null && adjuntoDTO.Correo == null)
            {
                throw new ArgumentNullException("El correo del adjunto no puuede estar vacio", nameof(correo));
            }

            Adjunto adjunto = new Adjunto()
            {
                DTO_Base = adjuntoDTO,
                Ruta = adjuntoDTO.Ruta,
                Nombre = adjuntoDTO.Nombre,
                Extension = adjuntoDTO.Extension
            };

            // Asigna el correo al que pertenece el fichero adjunto
            if (correo != null)
            {
                adjunto.Correo = correo;
            }
            else if (adjuntoDTO.Correo != null)
            {
                adjunto.Correo = Correo.ConvertToCore(adjuntoDTO.Correo);
            }
            else
            {
                throw new NotImplementedException("Asignar correo por Id no implementado");
            }

            return adjunto;
        }
    }
}
