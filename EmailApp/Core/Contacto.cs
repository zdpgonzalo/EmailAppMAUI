using MailAppMAUI.DTOs;
using MailAppMAUI.General;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.Core
{
    [Table("Contactos")]
    [PrimaryKey(nameof(ContactoId))]
    public class Contacto :ModelBaseCore<ContactoDTO>
    {
        /// <summary>
        /// Id del contacto
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactoId { get; private set; }

        /// <summary>
        /// Id del usuario al que esta asociado
        /// </summary>
        public int UsuarioId { get; private set; }

        /// <summary>
        /// Email del contacto
        /// </summary>
        [Required]
        public string Email { get; private set; }

        /// <summary>
        /// Nombre de contacto
        /// </summary>
        public string? Nombre { get; private set; }

        /// <summary>
        /// Descripcion del contacto
        /// </summary>
        public string? Descripcion { get; private set; }

        /// <summary>
        /// Tipo de contacto
        /// </summary>
        public TipoContacto Tipo { get; private set; } = TipoContacto.Formal;

        /// <summary>
        /// Telefono del contacto
        /// </summary>
        public string Telefono { get; private set; } = string.Empty;

        /// <summary>
        /// Constructor vacío
        /// </summary>
        private Contacto() { }

        /// <summary>
        /// Crea una instancia de ContactoCore
        /// </summary>
        /// <param name="nombre">Nombre de contacto</param>
        /// <param name="email">Email del contacto</param>
        /// <returns>Instancia de ContactoCore nueva</returns>
        /// <exception cref="ArgumentException">
        /// Violacion de las reglas de negocio
        /// El contacto debe definir un nombre y contraseña
        /// </exception>
        public static Contacto CreateContacto(string email, int usuarioId, string? nombre)
        {
            if (usuarioId == null) 
                throw new ArgumentException("El usuario del contacto no puede estar vacio", nameof(usuarioId));
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("El email de contacto no puede estar vacio", nameof(email));

            var contacto = new Contacto()
            {
                UsuarioId = usuarioId,
                Email = email,
                Nombre = nombre,
            };

            return contacto;
        }

        /// <inheritdoc/>
        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

            switch (propertyName)
            {
                case nameof(ContactoId):
                    value = ContactoId.ToString();
                    break;

                case nameof(Email):
                    value = Email;
                    break;

                case nameof(Nombre):
                    value = Nombre ?? string.Empty;
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Id: {ContactoId}, Nombre: {Nombre}";
        }

        /// <summary>
        /// Operador explicito que convierte un ContactoCore en ContactoDTO
        /// </summary>
        /// <param name="contacto">Contacto a convertir</param>
        public static explicit operator ContactoDTO(Contacto contacto)
        {
            if(contacto == null) 
                throw new ArgumentNullException("ContactoCore no puede ser null", nameof(contacto));

            return new ContactoDTO()
            {
                ContactoId      = contacto.ContactoId,
                UsuarioId       = contacto.UsuarioId,
                Nombre          = contacto.Nombre,
                Email           = contacto.Email,
            };
        }

        /// <summary>
        /// Convierte un ContactoDTO en un ContactoCore
        /// </summary>
        /// <param name="contactoDTO">ContactoDTO a convertir</param>
        /// <param name="usuario">Usuario al que esta asociado. Puede ser null</param>
        /// <returns>Instancia ContactoCore</returns>
        /// <exception cref="ArgumentNullException">
        /// Error en la logica de negocio. Parametro recibido no puede ser null
        /// </exception>
        public static Contacto ConvertToCore(ContactoDTO contactoDTO, int usuarioId)
        {
            if (contactoDTO == null)
            {
                throw new ArgumentNullException("El registro contactoDTO no puede ser null", nameof(contactoDTO));
            }

            if(usuarioId == null && contactoDTO.UsuarioId == null)
            {
                throw new ArgumentNullException("El usuario no puede ser null");
            }

            Contacto contacto = new Contacto()
            {
                DTO_Base    = contactoDTO,
                Nombre      = contactoDTO.Nombre,
                Email       = contactoDTO.Email,
            };

            //Asigna el empleado del contacto.Si es no tiene se queda en null
            //if (usuarioId != null)
            //{
            //    contacto.UsuarioId = usuarioId;
            //}
            //else if (contactoDTO.Usuario != null)
            //{
            //    contacto.UsuarioId = Usuario.ConvertToCore(contactoDTO.Usuario);
            //}
            //else
            //{
            //    throw new NotImplementedException("Asignar usuario por Id no implementado");
            //}

            return contacto;
        }

        /// <summary>
        /// Cambia el tipo del contacto
        /// </summary>
        /// <param name="tipo">Tipo de contacto</param>
        public void SetTipo(string tipo)
        {
            if (Enum.TryParse<TipoContacto>(tipo, true, out var resultado) && resultado != TipoContacto.None)
            {
                this.Tipo = resultado;
            }
            else
            {
                // Si el valor no corresponde a Formal o Informal, asignamos Desconocido
                this.Tipo = TipoContacto.Desconocido;
            }
        }

        /// <summary>
        /// Cambia el nombre del contacto
        /// </summary>
        /// <param name="name">Nuevo nombre</param>
        public bool SetNombre(string name)
        {
            if(name == Nombre)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            Nombre = name;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Cambia el Telefono del Contacto
        /// </summary>
        /// <param name="telefono"></param>
        public bool SetTelefono(string telefono)
        {
            if(telefono == Telefono)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            Telefono = telefono;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Cambia el Correo del Conatcto
        /// </summary>
        /// <param name="correo"></param>
        public bool SetCorreo(string correo)
        {
            if(correo == Email)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            Email = correo;
            SetChanges(OpResul.Line);
            return true;
        }

        /// <summary>
        /// Metodo para Actualizar la descripcion
        /// </summary>
        /// <param name="descripcion"></param>
        public bool UpdateDescripcion(string descripcion)
        {
            if(descripcion == Descripcion)
            {
                SetChanges(OpResul.Cancel);
                return false;
            }

            Descripcion = descripcion;
            SetChanges(OpResul.Line);
            return true;
        }

    }
}
