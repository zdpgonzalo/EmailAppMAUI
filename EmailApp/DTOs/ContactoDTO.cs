using MailAppMAUI.General;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.DTOs
{
    [Table("Contactos")]
    [PrimaryKey(nameof(ContactoId))]
    [Microsoft.EntityFrameworkCore.Index(nameof(Email), IsUnique = true)]
    public class ContactoDTO : BaseDTO, IComparable<ContactoDTO>
    {
        /// <summary>
        /// Id del usuario
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactoId { get; set; }

        /// <summary>
        /// Id del Usuario acoplado
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Telefono del Usuario
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [Required]
        public string Email { get; set; } = string.Empty;

        public ContactoDTO() : base()
        {
            Tipo = TipoEntidad.Contacto;
        }

        /// <inheritdoc/>
        public override void CopyFrom(BaseDTO dto)
        {
            base.CopyFrom(dto);
            ContactoDTO usuario     = (ContactoDTO)dto;
            ContactoId              = usuario.ContactoId;
            UsuarioId               = usuario.UsuarioId;
            Email                   = usuario.Email;
            Nombre                  = usuario.Nombre;
            Telefono                = usuario.Telefono;
        }

        public int CompareTo(ContactoDTO? other)
        {
            return ContactoId.CompareTo(other?.ContactoId);
        }

        ///<inheritdoc/>
        public override BaseDTO ImportData(object[] filas, string[] columnas)
        {
            var usuario = new ContactoDTO();

            for (int i = 0; i < columnas.Length; i++)
            {
                if (columnas[i] == null) break;

                switch (columnas[i])
                {
                    case nameof(ContactoId):
                        usuario.ContactoId = Data.ToInt(filas[i]);
                        break;

                    case nameof(UsuarioId):
                        usuario.UsuarioId = Data.ToInt(filas[i]);
                        break;

                    case nameof(Email):
                        usuario.Email = Data.ToString(filas[i]);
                        break;

                    case nameof(Nombre):
                        usuario.Nombre = Data.ToString(filas[i]) ?? string.Empty;
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
                case nameof(ContactoId):
                    value = ContactoId.ToString();
                    break;

                case nameof(Nombre):
                    value = Nombre ?? string.Empty;
                    break;

                case nameof(Email):
                    value = Email.ToString();
                    break;

                case nameof(UsuarioId):
                    value = UsuarioId.ToString();
                    break;

                default:
                    value = string.Empty;
                    return false;
            }

            return true;
        }

        public static new Type GetType()
        {
            return typeof(ContactoDTO);
        }

        public override string? ToString()
        {
            return $"Id: {ContactoId}, Email: {Email}";
        }
    }
}
