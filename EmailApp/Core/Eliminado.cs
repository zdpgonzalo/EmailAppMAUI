using MailAppMAUI.General;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace MailAppMAUI.Core
{
    /* ACLARACIÓN SOBRE ESTA CLASE
        Aquí se van a almacenar los elementos eliminados que pueden ser de dos tipos: Correo o Respuesta. En caso de ser Correo, tendrán una Respuesta asociada (la creada por la IA). 
    Esta se guardará en Eliminado.

        Por otro lado, en caso de ser Respuesta (RespuestaEnviada, RespuestaNueva, RespuestaBorrador), NO debería tener un correo asociado es decir, Eliminado deberá ser NULL, puesto que
    aún no se ha implementado lo de "Conversaciones", CorreoOG que tiene una Respuesta, que tiene una Respuesta, que tiene una Respuesta...     
     */


    [Table("Eliminados")]
    [PrimaryKey(nameof(EliminadoId))]
    public class Eliminado : ModelBaseCore<EliminadoDTO>
    {
        /// <summary>
        /// Id del eliminado
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EliminadoId { get; private set; }

        public Guid Guid { get; private set; }

        /// <summary>
        /// Email del remitente
        /// </summary>
        [Required]
        public string Remitente { get; private set; }

        public bool EsCorreo { get; private set; } = false;

        /// <summary>
        /// Email del destinatario
        /// </summary>
        [Required]
        public List<string> Destinatarios { get; private set; } = new List<string>();

        /// <summary>
        /// Nombre del destinatario
        /// </summary>
        public string? NombreDestinatario { get; private set; }

        /// <summary>
        /// Asunto del eliminado
        /// </summary>
        public string? Asunto { get; private set; }

        /// <summary>
        /// Cuerpo del eliminado
        /// </summary>
        public string? Cuerpo { get; private set; }

        /// <summary>
        /// Cuerpo HTML del eliminado
        /// </summary>
        public string? CuerpoCorreoHTML { get; private set; }
        public string? CuerpoRespuestaHTML { get; private set; }

        /// <summary>
        /// Id original obtenido del MimeMessage
        /// </summary>
        public string? MensajeId { get; private set; }

        /// <summary>
        /// Id del usuario al que esta asociado
        /// </summary>
        public int? UsuarioId { get; private set; }

        /// <summary>
        /// Usuario al que pertenece
        /// </summary>
        public Usuario? Usuario { get; private set; }

        /// <summary>
        /// True si se ha leido
        /// </summary>
        public bool Leido { get; private set; }

        /// <summary>
        /// Fecha y hora de cuando se ha recibido el correo
        /// </summary>
        public DateTime FechaRecibido { get; private set; }

        //----------DATOS RESPUESTAS----------

        /// <summary>
        /// Fecha y hora de cuando se ha enviado la respuesta
        /// </summary>
        public DateTime FechaEnviado { get; private set; }

        /// <summary>
        /// Fecha y hora de cuando se ha procesado la respuesta
        /// </summary>
        public DateTime FechaProcesado { get; private set; }

        /// <summary>
        /// True si se ha enviado la respuesta al destinatario
        /// </summary>
        public bool Enviado { get; private set; }

        /// <summary>
        /// True si el usuario ha generado por IA
        /// </summary>
        public bool EsIA { get; private set; } = true;

        /// <summary>
        /// True si la respuesta es un Borrador
        /// </summary>
        public bool Borrador { get; private set; } = false;

        /// <summary>
        /// Constructor vacío
        /// </summary>

        //----------OTRO ELIMINADO----------
        //Si eliminado 1 tiene a eliminado 2, significa que es un correo (E1) con una respuesta (E2). Si esa respuesta tiene otro eliminado (E3) significa que es la respuesta de la respuesta. --> Si es par es Respuesta si es Impar es correo

        /// <summary>
        /// Id del otro eliminado
        /// </summary>
        /// <remarks>Puede recibir null</remarks>
        public int? RespuestaEliminadaId { get; private set; }

        /// <summary>
        /// El otro eliminado
        /// </summary>
        //[ForeignKey(nameof(RespuestaEliminadaId))]
        //public Eliminado? RespuestaEliminada { get; private set; } = null;

        //Adjuntos del eliminado PRINCIPAL
        public List<Adjunto> Adjuntos { get; set; } = new();

        /// <summary>
        /// Constructor vacío
        /// </summary>
        private Eliminado() { }

        /// <summary>
        /// Creación de un eliminado con UN CORREO
        /// </summary>
        /// <param name="correo">Correo para hacer el eliminado</param>
        /// <param name="respuestaDelCorreo">Respuesta del correo (otro eliminado)</param>
        /// <returns></returns>
        public static Eliminado CreateEliminado(Correo? correo)
        {
           // Eliminado respuesta = (Eliminado)correo.Respuesta;

            var eliminado = new Eliminado()
            {
                Guid = correo.Guid,

                EsCorreo = true,
                Remitente = correo.Remitente,
                Destinatarios = correo.Destinatarios,
                Asunto = correo?.Asunto,
                Cuerpo = correo?.Cuerpo,
                CuerpoCorreoHTML = correo?.CuerpoHTML,
                MensajeId = correo?.MensajeId,

                UsuarioId = correo?.UsuarioId,
                Usuario = correo?.Usuario,

                Leido = correo.Leido,
                FechaRecibido = correo.FechaRecibido,
                Adjuntos = correo.Adjuntos,
            };

            //eliminado.RespuestaEliminada = respuesta;
            return eliminado;
        }

        /// <summary>
        /// Creación de un eliminado con UNA RESPUESTA
        /// </summary>
        /// <param name="respuesta">Respuesta para hacer el eliminado</param>
        /// <param name="eliminado">Respuesta de la respuesta (otro eliminado)</param>
        /// <returns></returns>
        public static Eliminado CreateEliminado(Respuesta? respuesta)
        {
            var newEliminado = new Eliminado()
            {
                Guid = respuesta.Guid,

                Remitente = respuesta.Remitente,
                Destinatarios = respuesta.Destinatarios,
                Asunto = respuesta.Asunto,
                Cuerpo = respuesta.Cuerpo,
                CuerpoRespuestaHTML = respuesta.CuerpoHTML,
                NombreDestinatario = respuesta?.NombreDestinatario,

                FechaEnviado = respuesta.FechaEnviado,
                FechaProcesado = respuesta.FechaProcesado,
                EsIA = respuesta.EsIA,
                Borrador = respuesta.Borrador,
                Adjuntos = respuesta.Adjuntos,
            };

            return newEliminado;
        }

        /// <inheritdoc/>
        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

            switch (propertyName)
            {
                case nameof(Remitente):
                    value = Remitente;
                    break;

                case nameof(Destinatarios):
                    value = string.Join(", ", Destinatarios);
                    break;

                case nameof(CuerpoCorreoHTML):
                    value = CuerpoCorreoHTML ?? string.Empty;
                    break;

                case nameof(CuerpoRespuestaHTML):
                    value = CuerpoRespuestaHTML ?? string.Empty;
                    break;

                case nameof(UsuarioId):
                    value = UsuarioId?.ToString();
                    break;

                case nameof(Cuerpo):
                    value = Cuerpo ?? string.Empty;
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

                case nameof(NombreDestinatario):
                    value = NombreDestinatario.ToString();
                    break;


                default:
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"";
        }

        /// <summary>
        /// Operador explicito que convierte un EliminadoCore en EliminadoDTO
        /// </summary>
        /// <param name="respuesta">Respuesta a convertir</param>
        public static explicit operator EliminadoDTO(Eliminado eliminado)
        {
            if (eliminado == null)
                throw new ArgumentNullException("EliminadoCore no puede ser null", nameof(eliminado));

            return new EliminadoDTO()
            {
                Remitente = eliminado.Remitente,
                EsCorreo = eliminado.EsCorreo,
                Destinatarios = eliminado.Destinatarios,
                NombreDestinatario = eliminado.NombreDestinatario,
                Asunto = eliminado.Asunto,
                Cuerpo = eliminado.Cuerpo,
                CuerpoCorreoHTML = eliminado.CuerpoCorreoHTML,
                CuerpoRespuestaHTML = eliminado.CuerpoRespuestaHTML,

                MensajeId = eliminado.MensajeId,
                UsuarioId = eliminado?.UsuarioId,
                Usuario =  eliminado?.Usuario != null ? (UsuarioDTO)eliminado.Usuario : null,
                Leido = eliminado?.Leido,
                FechaRecibido = eliminado?.FechaRecibido,

                //----------DATOS RESPUESTAS----------
                FechaEnviado = eliminado?.FechaEnviado,
                FechaProcesado = eliminado?.FechaProcesado,
                EsIA = eliminado?.EsIA,
                Borrador = eliminado?.Borrador,
                RespuestaEliminadaId = eliminado?.RespuestaEliminadaId,
                //RespuestaEliminada = eliminado?.RespuestaEliminada,

                Adjuntos = eliminado?.Adjuntos,
            };
        }

        /// <summary>
        /// Operador explicito que convierte un Correo en Eliminado
        /// </summary>
        /// <param name="respuesta">Respuesta a convertir</param>
        public static explicit operator Eliminado(Correo correo)
        {
            if (correo == null)
                throw new ArgumentNullException("Correo no puede ser null", nameof(correo));

            Eliminado miEliminado =  new Eliminado()
            {
                Guid                = correo.Guid,

                Remitente           = correo.Remitente,
                EsCorreo            = true,
                Destinatarios       = correo.Destinatarios,
                Asunto              = correo.Asunto,
                Cuerpo              = correo.Cuerpo,
                CuerpoCorreoHTML    = correo.CuerpoHTML,

                MensajeId           = correo.MensajeId,
                UsuarioId           = correo.UsuarioId,
                Usuario             = correo.Usuario,
                Leido               = correo.Leido,
                FechaRecibido       = correo.FechaRecibido,

                //----------DATOS RESPUESTAS----------
                FechaEnviado        = DateTime.Now,
                FechaProcesado      = DateTime.Now,
                EsIA                = false,
                Borrador            = false,

                //Se crea la respuestaEliminada con la Respuesta del Correo
               //RespuestaEliminada = eliminado?.Respuesta != null ? (Eliminado)eliminado.Respuesta : null, // Manejar null

                Adjuntos            = correo.Adjuntos,
            };

            return miEliminado;
        }

        /// <summary>
        /// Operador explicito que convierte una Respuesta en Eliminado
        /// </summary>
        /// <param name="respuesta">Respuesta a convertir</param>
        public static explicit operator Eliminado(Respuesta respuesta)
        {
            if (respuesta == null)
                throw new ArgumentNullException("EliminadoCore no puede ser null", nameof(respuesta));

            return new Eliminado()
            {
                Guid                = respuesta.Guid,
                Remitente           = respuesta.Remitente,
                EsCorreo            = false,
                Destinatarios       = respuesta.Destinatarios,
                NombreDestinatario  = respuesta?.NombreDestinatario,
                Asunto              = respuesta.Asunto,
                Cuerpo              = respuesta.Cuerpo,
                CuerpoRespuestaHTML = respuesta.CuerpoHTML,
                //CuerpoHTML = null,

                //MensajeId = null,
                //UsuarioId = -1,
                //Usuario = null,
                //ContactoId = eliminado.Contacto?.ContactoId,
                //Contacto = eliminado.Contacto,
                Leido               = false,
                FechaRecibido       = DateTime.Now,

                //----------DATOS RESPUESTAS----------
                FechaEnviado        = respuesta.FechaEnviado,
                FechaProcesado      = respuesta.FechaProcesado,
                EsIA                = respuesta.EsIA,
                Borrador            = respuesta.Borrador,

                ////No se crea un RespuestaEliminada
                //RespuestaEliminadaId = -1,
                //RespuestaEliminada = null,

                Adjuntos            = respuesta.Adjuntos,
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
        public static Eliminado ConvertToCore(EliminadoDTO eliminadoDTO, Correo? correo = null, Respuesta? respuesta = null) //** No se usa y esto está mal
        {
            if (eliminadoDTO == null)
                throw new ArgumentNullException("El registro respuestaDTO no puede ser null", nameof(eliminadoDTO));


            Eliminado eliminado = new Eliminado()
            {
                DTO_Base = eliminadoDTO,
                //Correo      = (Correo)eliminadoDTO.Correo,
                //Respuesta   = eliminadoDTO.Respuesta
            };

            if (correo == null && respuesta == null)
            {
                throw new Exception("Respuesta o Correo deben tener un valor");
            }

            //Si existen los dos, es un correo con su respuesta autogenerada
            if (correo != null)
            {
                // eliminado.Adjuntos = eliminado.Correo.Adjuntos;
            }

            if (respuesta != null)
            {
                // eliminado.Adjuntos = eliminado.Respuesta.Adjuntos;
            }

            return eliminado;
        }

        /// <summary>
        /// Establece la referencia del Id de la respuesta del eliminado
        /// </summary>
        /// <param name="id">Id de la respuesta del correo</param>
        /// <returns>True si realiza el cambio, false en caso contrario</returns>
        public bool SetRespuestaEliminadaId(int id)
        {
            if(id == RespuestaEliminadaId)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            RespuestaEliminadaId = id;
            SetChanges(OpResul.Line);

            return true;
        }

        /// <summary>
        /// Establece la propiedad esCorreo
        /// </summary>
        /// <param name="esCorreo">Valor nuevo de la propiedad</param>
        /// <returns>True</returns>
        public bool SetEsCorreo(bool esCorreo)
        {
            EsCorreo = esCorreo;
            Enviado = false;
            Borrador = false;
            EsIA = false;

            SetChanges(OpResul.Page);
            return true;
        }

        /// <summary>
        /// Establece la propiedad Borrador
        /// </summary>
        /// <param name="esBorrador">Valor de la propiedad</param>
        /// <returns>True</returns>
        public bool SetEsBorrador(bool esBorrador)
        {
            EsCorreo = false;
            Enviado = false;
            Borrador = esBorrador;
            EsIA = false;

            SetChanges(OpResul.Page);
            return true;
        }

        /// <summary>
        /// Establece el usuario del correo
        /// </summary>
        /// <param name="id">Id del usuario del correo</param>
        /// <returns>True si realiza el cambio, false en caso contrario</returns>
        public bool SetUsuario(int id)
        {
            if(id == UsuarioId)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            UsuarioId = id; 
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Establece la propiedad Enviado del correo
        /// </summary>
        /// <param name="esEnviado"></param>
        /// <returns>True</returns>
        public bool SetEsEnviado(bool esEnviado)
        {
            EsCorreo = false;
            Enviado = esEnviado;
            Borrador = false;
            EsIA = false;

            SetChanges(OpResul.Page);
            return true;
        }

        /// <summary>
        /// Establece la propiedad EsIA del correo
        /// </summary>
        /// <param name="esIa"></param>
        /// <returns>True</returns>
        public bool SetEsIA(bool esIa)
        {
            EsCorreo = false;
            Enviado = false;
            Borrador = false;
            EsIA = esIa;

            SetChanges(OpResul.Page);
            return true;
        }
    }
}
