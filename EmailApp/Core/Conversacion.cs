using MailAppMAUI.Core;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAppMAUI.Core
{
    [Table("Conversacion")]
    [PrimaryKey(nameof(ConversacionId))]
    public class Conversacion : ModelBaseCore<ConversacionDTO>
    {
        /// <summary>
        /// Id de la conversacion
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConversacionId { get; private set; }

        /// <summary>
        /// Lista de correos en la conversacion
        /// </summary>
        [Required]
        public List<Correo> Correos { get; set; }

        /// <summary>
        /// Lista de respuestas en la conversacion
        /// </summary>
        [Required]
        public List<Respuesta> Respuestas { get; set; }


        /// <summary>
        /// Constructor vacío
        /// </summary>
        private Conversacion() { }

        /// <summary>
        /// Crea una instancia de ConversacionCore con un Correo (alguien ha iniciado la conversacion)
        /// </summary>
        public static Conversacion CreateConversacion()
        {
            List<Correo> misCorreos = new List<Correo>();
            List<Respuesta> misRespuestas = new List<Respuesta>();
            Dictionary<string, int> miConversacionOrdenada = new Dictionary<string, int>();

            var conversacion = new Conversacion()
            {
                Correos = misCorreos,
                Respuestas = misRespuestas,
            };

            return conversacion;
        }


        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

            switch (propertyName)
            {
                case nameof(ConversacionId):
                    value = ConversacionId.ToString();
                    break;

                case nameof(Correos):
                    value = Correos.ToString();
                    break;

                case nameof(Respuestas):
                    value = Respuestas?.ToString() ?? string.Empty;
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Operador explícito que convierte una ConversacionCore en ConversacionoDTO
        /// </summary>
        /// <param name="correo">Correo a convertir</param>
        public static explicit operator ConversacionDTO(Conversacion conver)
        {
            return new ConversacionDTO()
            {
                ConversacionId = conver.ConversacionId,
                Correos = conver.Correos,
                Respuestas = conver.Respuestas,
            };
        }

        /// <summary>
        /// Convierte un ConversacionDTO en un ConversacionCore
        /// </summary>
        public static Conversacion ConvertToCore(ConversacionDTO converDTO)
        {
            if (converDTO == null)
                throw new ArgumentNullException("El registro correoDTO no puede ser null", nameof(converDTO));

            Conversacion conversacion = new Conversacion()
            {
                DTO_Base = converDTO,
                Correos = converDTO.Correos,
                Respuestas = converDTO.Respuestas,
            };


            return conversacion;
        }


        public void AddToConversacion(Correo correo)
        {
            string mensajeId = correo.MensajeId;

            bool yaExiste = Correos.Any(c => c.MensajeId == mensajeId) ||
                            Respuestas.Any(r => r.MensajeId == mensajeId);

            if (!yaExiste)
            {
                Correos.Add(correo);
                SetChanges(General.OpResul.Page);
            }
        }

        public void AddToConversacion(Respuesta res)
        {
            string mensajeId = res.MensajeId;

            bool yaExiste = Correos.Any(c => c.MensajeId == mensajeId) ||
                            Respuestas.Any(r => r.MensajeId == mensajeId);

            if (!yaExiste)
            {
                Respuestas.Add(res);
                SetChanges(General.OpResul.Page);
            }
        }

        /// <summary>
        /// Ordena por fecha de mas nuevo a mas antiguo toda la conversacion
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetConversacionOrdenada()
        {
            var merged = new List<(DateTime fecha, string key, int id)>();

            // Agregar correos
            foreach (var correo in Correos)
            {
                merged.Add((correo.FechaRecibido, $"Correo{correo.CorreoId}", correo.CorreoId));
            }

            // Agregar respuestas
            foreach (var respuesta in Respuestas)
            {
                merged.Add((respuesta.FechaEnviado, $"Respuesta{respuesta.CorreoId}", respuesta.RespuestaId));
            }

            // Ordenar por fecha y construir el diccionario
            var ConversacionOrdenada = merged
                   .OrderByDescending(x => x.fecha)
                   .ToDictionary(x => x.key, x => x.id);

            return ConversacionOrdenada;
        }

        public List<string> ObtenerCadenaDeReferenciasOrdenada()
        {
            var mensajes = new List<(DateTime fecha, string mensajeId)>();

            foreach (var correo in Correos)
            {
                if (!string.IsNullOrEmpty(correo.MensajeId))
                {
                    mensajes.Add((correo.FechaRecibido, correo.MensajeId));
                }
            }

            foreach (var respuesta in Respuestas)
            {
                if (!string.IsNullOrEmpty(respuesta.MensajeId))
                {
                    mensajes.Add((respuesta.FechaEnviado, respuesta.MensajeId));
                }
            }

            return mensajes.OrderBy(m => m.fecha)
                           .Select(m => m.mensajeId)
                           .ToList();
        }

        /// <summary>
        /// Todos los cuerpos de una conversacion ordenados 
        /// </summary>
        /// <returns></returns>
        public List<string> ObtenerCuerposHTMLOrdenados()
        {
            // Lista para almacenar los cuerpos de los correos y respuestas
            var mensajes = new List<(DateTime fecha, string cuerpoHTML)>();

            // Agregar los cuerpos HTML de los correos
            if (Correos != null)
            {
                foreach (var correo in Correos)
                {
                    if (!string.IsNullOrEmpty(correo.CuerpoHTML))
                    {
                        mensajes.Add((correo.FechaRecibido, correo.CuerpoHTML));
                    }
                }
            }

            if (Respuestas != null)
            {
                // Agregar los cuerpos HTML de las respuestas
                foreach (var respuesta in Respuestas)
                {
                    if (!string.IsNullOrEmpty(respuesta.CuerpoHTML))
                    {
                        mensajes.Add((respuesta.FechaEnviado, respuesta.CuerpoHTML));
                    }
                }
            }
            
            // Ordenar los mensajes por fecha y eliminar duplicados
            var mensajesOrdenados = mensajes
                .OrderBy(m => m.fecha) // Ordenar por la fecha de envío/recepción
                .Select(m => m.cuerpoHTML) // Solo quedarnos con el cuerpo HTML
                .Distinct() // Eliminar cuerpos repetidos
                .ToList(); // Convertirlo a una lista

            return mensajesOrdenados;
        }
    }
}
