using MailAppMAUI.General;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.DTOs
{
    [Table("Usuarios")]
    [PrimaryKey(nameof(UsuarioId))]
    [Microsoft.EntityFrameworkCore.Index(nameof(Email), IsUnique = true)]
    public class UsuarioDTO : BaseDTO, IComparable<UsuarioDTO>
    {
        /// <summary>
        /// Id del usuario
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [Required]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Lista de correos del usuario
        /// </summary>
        public List<CorreoDTO> Correos { get; set; } = new();

        /// <summary>
        /// Lista de contactos del usuario
        /// </summary>
        public List<ContactoDTO> Contactos { get; set; } = new();

        /// <summary>
        /// Flag para saber si se ha registrado con google
        /// </summary>
        public bool GoogleLogin { get; set; }

        /// <summary>
        /// Token si se ha registrado con google
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Si es Admin o User
        /// </summary>
        public string Role { get; set; }

        public UsuarioDTO() : base()
        {
            Tipo = TipoEntidad.Usuario;
        }

        /// <inheritdoc/>
        public override void CopyFrom(BaseDTO dto)
        {
            base.CopyFrom(dto);
            UsuarioDTO usuario = (UsuarioDTO)dto;
            UsuarioId = usuario.UsuarioId;
            Email = usuario.Email;
            Nombre = usuario.Nombre;
            Password = usuario.Password;
            Correos = usuario.Correos;
            Contactos = usuario.Contactos;
            GoogleLogin = usuario.GoogleLogin;
            Role = usuario.Role;
            Token = usuario.Token;
        }

        public int CompareTo(UsuarioDTO? other)
        {
            return UsuarioId.CompareTo(other?.UsuarioId);
        }

        ///<inheritdoc/>
        public override BaseDTO ImportData(object[] filas, string[] columnas)
        {
            var usuario = new UsuarioDTO();

            for (int i = 0; i < columnas.Length; i++)
            {
                if (columnas[i] == null) break;

                switch (columnas[i])
                {
                    case nameof(UsuarioId):
                        usuario.UsuarioId = Data.ToInt(filas[i]);
                        break;

                    case nameof(Email):
                        usuario.Email = Data.ToString(filas[i]);
                        break;

                    case nameof(Nombre):
                        usuario.Nombre = Data.ToString(filas[i]);
                        usuario.Nombre ??= string.Empty;
                        break;

                    case nameof(Password):
                        usuario.Password = Data.ToString(filas[i]);
                        usuario.Password ??= string.Empty;
                        break;

                    case nameof(GoogleLogin):
                        usuario.GoogleLogin = Data.ToBool(filas[i]);
                        break;

                    case nameof(Role):
                        usuario.Role = Data.ToString(filas[i]);
                        break;

                    case nameof(Token):
                        usuario.Token = Data.ToString(filas[i]);
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
                case nameof(UsuarioId):
                    value = UsuarioId.ToString();
                    break;

                case nameof(Nombre):
                    value = Nombre ?? string.Empty;
                    break;

                case nameof(Email):
                    value = Email.ToString();
                    break;

                case nameof(Password):
                    value = Password.ToString();
                    break;

                case nameof(GoogleLogin):
                    value = Password.ToString();
                    break;

                case nameof(Token):
                    value = Token;
                    break;

                case nameof(Role):
                    value = Role.ToString();
                    break;
                default:
                    value = string.Empty;
                    return false;
            }

            return true;
        }

        public static new Type GetType()
        {
            return typeof(UsuarioDTO);
        }

        public override string? ToString()
        {
            return $"Id: {UsuarioId}, Email: {Email}";
        }
    }
}
