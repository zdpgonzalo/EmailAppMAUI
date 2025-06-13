using MailAppMAUI.General;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.DTOs
{
    [Table("Planes")]
    [PrimaryKey(nameof(PlanId))]
    public class PlanDTO : BaseDTO
    {
        /// <summary>
        /// Id del plan
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlanId { get; set; }

        /// <summary>
        /// Tipo de plan (Gratuito, Plus, Pro)
        /// </summary>
        public PlanType Tipo { get; set; }

        /// <summary>
        /// Indica si el plan tiene habilitada la opción de traducir
        /// </summary>
        public bool Traducir { get; set; }

        /// <summary>
        /// Número de peticiones permitidas por día
        /// </summary>
        public int PeticionesPorDia { get; set; }

        /// <summary>
        /// Número de correos respondidos automáticamente
        /// </summary>
        public int CorreosRespondidosAutomaticos { get; set; }

        /// <summary>
        /// Indica si el plan tiene reglas inteligentes
        /// </summary>
        public bool Inteligente { get; set; }

        /// <summary>
        /// Precio del plan
        /// </summary>
        public float Precio { get; set; }
        public DateTime UltimoReset { get; set; }

        /// <summary>
        /// Fecha de finalización del plan
        /// </summary>
        public DateTime FechaFinalizacion { get; set; }

        public PlanDTO() : base()
        {
            Tipo = PlanType.Gratuito;  // Valor predeterminado
        }

        public override void CopyFrom(BaseDTO dto)
        {
            base.CopyFrom(dto);
            PlanDTO plan                  = (PlanDTO)dto;
            PlanId                        = plan.PlanId;
            Tipo                          = plan.Tipo;
            Traducir                      = plan.Traducir;
            PeticionesPorDia              = plan.PeticionesPorDia;
            CorreosRespondidosAutomaticos = plan.CorreosRespondidosAutomaticos;
            Inteligente                   = plan.Inteligente;
            Precio                        = plan.Precio;
            FechaFinalizacion             = plan.FechaFinalizacion;
            UltimoReset                   = plan.UltimoReset;
        }

        public override bool GetValue(string propertyName, out string value)
        {
            if (base.GetValue(propertyName, out value)) return true;

            switch (propertyName)
            {
                case nameof(PlanId):
                    value = PlanId.ToString();
                    break;

                case nameof(Tipo):
                    value = Tipo.ToString();
                    break;

                case nameof(Traducir):
                    value = Traducir.ToString();
                    break;

                case nameof(PeticionesPorDia):
                    value = PeticionesPorDia.ToString();
                    break;

                case nameof(CorreosRespondidosAutomaticos):
                    value = CorreosRespondidosAutomaticos.ToString();
                    break;

                case nameof(Inteligente):
                    value = Inteligente.ToString();
                    break;

                case nameof(Precio):
                    value = Precio.ToString();
                    break;
                
                case nameof(UltimoReset):
                    value = UltimoReset.ToString();
                    break;

                case nameof(FechaFinalizacion):
                    value = FechaFinalizacion.ToString();
                    break;

                default:
                    value = string.Empty;
                    return false;
            }

            return true;
        }

        public override string? ToString()
        {
            return $"PlanId: {PlanId}, Tipo: {Tipo}, Precio: {Precio}, Fecha Finalización: {FechaFinalizacion}";
        }

        public static new Type GetType()
        {
            return typeof(PlanDTO);
        }
    }
}
