using MailAppMAUI.DTOs;
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

namespace MailAppMAUI.Core
{
    [Table("Plan")]
    [PrimaryKey(nameof(PlanId))]
    public class Plan : ModelBaseCore<PlanDTO>
    {
        /// <summary>
        /// Id del plan
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlanId { get; private set; }
        public PlanType Tipo { get; set; }
        public bool Traducir { get; set; }
        public int PeticionesPorDia { get; set; }
        public int CorreosRespondidosAutomaticos { get; set; }
        public bool Inteligente { get; set; } //Reglas limitadas para gestionar el plan 
        public float Precio { get; set; }
        public DateTime FechaFinalizacion { get; set; } //Fin del plan. 
        public DateTime UltimoReset { get; private set; } //Reset diario

        public Plan() { }
        public Plan(PlanType tipo)
        {
            Tipo = tipo;

            switch (tipo)
            {
                case PlanType.Gratuito:
                    Traducir = false;
                    PeticionesPorDia = 25;
                    CorreosRespondidosAutomaticos = -1;
                    FechaFinalizacion = DateTime.Now.AddMinutes(5); //Da igual porque se comprueba si es plan gratuito en cuyo caso, omite esto
                    Inteligente = false;
                    Precio = 0;
                    break;

                case PlanType.Plus:
                    Traducir = true;
                    PeticionesPorDia = 75;
                    CorreosRespondidosAutomaticos = 10;
                    Inteligente = true;
                    FechaFinalizacion = DateTime.Now.AddDays(30);
                    Precio = 10;
                    break;

                case PlanType.Pro:
                    Traducir = true;
                    PeticionesPorDia = 300;
                    CorreosRespondidosAutomaticos = 99;
                    FechaFinalizacion = DateTime.Now.AddDays(30);
                    Inteligente = true;
                    Precio = 30;
                    break;
            }
            UltimoReset = DateTime.Today;
        }


        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

            switch (propertyName)
            {
                case nameof(PlanId):
                    value = PlanId.ToString();
                    break;

                case nameof(Tipo):
                    value = Tipo.ToString();
                    break;

                case nameof(Traducir):
                    value = Traducir.ToString() ?? string.Empty;
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

                case nameof(FechaFinalizacion):
                    value = FechaFinalizacion.ToString() ?? string.Empty;
                    break;     
                    
                case nameof(UltimoReset):
                    value = UltimoReset.ToString() ?? string.Empty;
                    break;

                default:
                    return false;
            }

            return true;
        }

        public static explicit operator PlanDTO(Plan plan)
        {
            return new PlanDTO()
            {
                PlanId = plan.PlanId,
                Tipo = plan.Tipo,
                Traducir = plan.Traducir,
                PeticionesPorDia = plan.PeticionesPorDia,
                CorreosRespondidosAutomaticos = plan.CorreosRespondidosAutomaticos,
                Inteligente = plan.Inteligente,
                Precio = plan.Precio,
                FechaFinalizacion = plan.FechaFinalizacion,
                UltimoReset = plan.UltimoReset,
            };
        }

        /// <summary>
        /// Convierte un planDTO en un planCore
        /// </summary>
        public static Plan ConvertToCore(PlanDTO planDTO)
        {
            Plan plan = new Plan()
            {
                DTO_Base = planDTO,
                PlanId = planDTO.PlanId,
                Tipo = planDTO.Tipo,
                Traducir = planDTO.Traducir,
                PeticionesPorDia = planDTO.PeticionesPorDia,
                CorreosRespondidosAutomaticos = planDTO.CorreosRespondidosAutomaticos,
                Inteligente = planDTO.Inteligente,
                Precio = planDTO.Precio,
                FechaFinalizacion = planDTO.FechaFinalizacion,
                UltimoReset = planDTO.UltimoReset,
            };
            return plan;
        }


        public void ResetPlan()
        {
            switch (Tipo)
            {
                case PlanType.Gratuito:
                    PeticionesPorDia = 25;
                    CorreosRespondidosAutomaticos = -1;
                    break;

                case PlanType.Plus:
                    PeticionesPorDia = 75;
                    CorreosRespondidosAutomaticos = 10;
                    break;

                case PlanType.Pro:
                    PeticionesPorDia = 300;
                    CorreosRespondidosAutomaticos = 99;
                    break;
            }

            UltimoReset = DateTime.Today; // Marca el día en que se reseteó
        }

        /// <summary>
        /// Comprueba si puede hacer la peticion y resta uno del maximo de peticiones
        /// </summary>
        /// <returns></returns>
        public bool MakeAPeticion()
        {
            bool canDoIt = false;
            if (PeticionesPorDia > 0)
            {
                PeticionesPorDia--;
                canDoIt = true;
            }

            return canDoIt;
        }

        /// <summary>
        /// Comprueba si puede hacer la peticion y resta uno del maximo de peticiones
        /// </summary>
        /// <returns></returns>
        public bool CanResponderAutomaticamente()
        {
            bool canDoIt = false;
            if (CorreosRespondidosAutomaticos > 0)
            {
                CorreosRespondidosAutomaticos--;
                canDoIt = true;
            }
            return canDoIt;
        }

        public void ChangePlan(PlanType tipo)
        {

            Tipo = tipo;

            switch (tipo)
            {
                case PlanType.Gratuito:
                    Traducir = false;
                    PeticionesPorDia = 25; //Cuando cambie al plan grauito tendrá 25 peticiones (si ha gastado 75 del plus y justo lo pierde, pasa a tener 25)
                    CorreosRespondidosAutomaticos = -1;
                    FechaFinalizacion = DateTime.Now.AddDays(5);
                    Inteligente = false;
                    Precio = 0;
                    break;

                case PlanType.Plus:
                    Traducir = true;
                    PeticionesPorDia = 75;
                    CorreosRespondidosAutomaticos = 10;
                    Inteligente = true;
                    FechaFinalizacion = DateTime.Now.AddDays(30);
                    Precio = 10;
                    break;

                case PlanType.Pro:
                    Traducir = true;
                    PeticionesPorDia = 300;
                    CorreosRespondidosAutomaticos = 99;
                    FechaFinalizacion = DateTime.Now.AddDays(30);
                    Inteligente = true;
                    Precio = 30;
                    break;
            }
        }
    }
}
