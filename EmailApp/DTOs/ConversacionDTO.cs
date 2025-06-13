using MailAppMAUI.General;
using MailAppMAUI.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailAppMAUI.Core;

namespace MailAppMAUI.DTOs
{
    [Table("Conversacion")]
    [PrimaryKey(nameof(ConversacionId))]
    public class ConversacionDTO : BaseDTO, IComparable<ConversacionDTO>
    {
        /// <summary>
        /// Id de la Conversacion
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConversacionId { get; set; }

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


        public ConversacionDTO() : base()
        {
            Tipo = TipoEntidad.Conversacion;
        }

        public override void CopyFrom(BaseDTO dto)
        {
            base.CopyFrom(dto);
            ConversacionDTO conver = (ConversacionDTO)dto;
            ConversacionId = conver.ConversacionId;
            Correos = conver.Correos;
            Respuestas = conver.Respuestas;
        }

        public int CompareTo(ConversacionDTO? other)
        {
            return ConversacionId.CompareTo(other?.ConversacionId);
        }



        public override BaseDTO ImportData(object[] filas, string[] columnas)
        {
            var conver = new ConversacionDTO();

            for (int i = 0; i < columnas.Length; i++)
            {
                if (columnas[i] == null) break;

                switch (columnas[i])
                {
                    case nameof(ConversacionId):
                        conver.ConversacionId = Data.ToInt(filas[i]);
                        break;
                    //Las listas por lo general no se devuelve clueless why

                    //case nameof(Correos):
                    //    conver.Correos = Data.ToString(filas[i]);
                    //    break;

                    //case nameof(Respuestas):
                    //    conver.Respuestas = Data.ToInt(filas[i]);
                    //    break;
                    default:
                        continue;
                }
            }
            return conver;
        }

        public override bool GetValue(string propertyName, out string value)
        {
            if (base.GetValue(propertyName, out value)) return true;

            switch (propertyName)
            {
                case nameof(ConversacionId):
                    value = ConversacionId.ToString();
                    break;

                case nameof(Correos):
                    value = Correos.ToString();
                    break;

                case nameof(Respuestas):
                    value = Respuestas.ToString();
                    break;
                default:
                    value = string.Empty;
                    return false;
            }
            return true;
        }

        public static new Type GetType()
        {
            return typeof(ConversacionDTO);
        }

    }
}
