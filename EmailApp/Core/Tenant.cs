using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MailAppMAUI.Core
{
    [Table("Tenant")]
    [PrimaryKey(nameof(TenantId))]
    public class Tenant : ModelBaseCore<TenantDTO>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string Carpeta { get; set; } // ejemplo: infoser.net/MiTenant

        public static Tenant CreateTenant(string name, string carpeta)
        {
            if (name == null)
                throw new ArgumentNullException("El nombre no puede estar vacio");

            if (carpeta == null)
                throw new ArgumentNullException("La carpta no puede ser vacia", nameof(carpeta));

            var correo = new Tenant()
            {
                Name = name,
                Carpeta = carpeta,
            };

            return correo;
        }

        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

            switch (propertyName)
            {
                case nameof(TenantId):
                    value = TenantId.ToString();
                    break;

                case nameof(Name):
                    value = Name.ToString();
                    break;

                case nameof(Carpeta):
                    value = Carpeta.ToString();
                    break;

                default:
                    return false;
            }

            return true;
        }

        //No meto conversiones de DTO a Core ni nada
    }
}
        
