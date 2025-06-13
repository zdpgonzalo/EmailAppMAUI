using MailAppMAUI.Core;
using MailAppMAUI.General;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Charts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailAppMAUI.Core
{
    [Table("Usuarios")]
    [PrimaryKey(nameof(UsuarioId))]
    [Microsoft.EntityFrameworkCore.Index(nameof(Email), IsUnique = true)]
    public class Usuario : ModelBaseCore<UsuarioDTO>
    {
        /// <summary>
        /// Id del usuario
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; private set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required]
        public string Email { get; private set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string? Nombre { get; private set; }

        /// <summary>
        /// Si es Admin o User
        /// </summary>
        public string Role { get; private set; }

        /// <summary>
        /// Contraseña de la cuenta del usuario -> Not required porque puede iniciar sesion con google
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Lo que permitirá acceder a los correos si inicia sesion con google
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Id del plan
        /// </summary>
        public int PlanId { get; private set; }

        /// <summary>
        /// Si es premium, no tiene prueba gratuita
        /// </summary>

        [ForeignKey(nameof(PlanId))]
        public Plan Plan { get; private set; }

        /// <summary>
        /// Lista de correos del usuario
        /// </summary>
        public List<Correo> Correos { get; set; } = new();

        /// <summary>
        /// Lista de contactos del usuario
        /// </summary>
        public List<Contacto> Contactos { get; set; } = new();

        /// <summary>
        /// Flag para saber si se ha registrado con google o no
        /// </summary>
        public bool GoogleLogin { get; private set; }

        /// <summary>
        /// Constructor vacío
        /// </summary>
        private Usuario() { }

        /// <summary>
        /// Crea una instancia de UsuarioCore cuando se REGISTRA CON CONTRASEÑA
        /// </summary>
        /// <param name="nombre">Nombre de usuario</param>
        /// <param name="password">Contraseña de usuario</param>
        /// <returns>Instancia de UsuarioCore nueva</returns>
        /// <exception cref="ArgumentException">
        /// Violacion de las reglas de negocio
        /// El usuario debe definir un nombre y contraseña
        /// </exception>
        public static Usuario CreateUsuario(string email, string password, string role, Plan plan, string? nombre = "")
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("El email de usuario no puede estar vacio", nameof(email));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacia", nameof(password));

            var usuario = new Usuario()
            {
                Email = email,
                Nombre = nombre,
                Password = password,
                Token = "noAccessTok",
                Role = role,
                GoogleLogin = false,
                Plan = plan
            };

            return usuario;
        }

        /// <summary>
        /// Crea una instancia de UsuarioCore cuando se REGISTRA CON GOOGLE
        /// </summary>
        /// <param name="nombre">Nombre de usuario</param>
        /// <param name="password">Contraseña de usuario</param>
        /// <returns>Instancia de UsuarioCore nueva</returns>
        /// <exception cref="ArgumentException">
        /// Violacion de las reglas de negocio
        /// El usuario debe definir un nombre y contraseña
        /// </exception>
        public static Usuario CreateUsuarioGoogle(string email, string role, string initialToken, string? nombre = "")
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("El email de usuario no puede estar vacio", nameof(email));

            var usuario = new Usuario()
            {
                Email = email,
                Nombre = nombre,
                Password = "noPassword",
                Token = initialToken,
                Role = role,
                GoogleLogin = true
                //Plan = new Plan(planType)
            };

            return usuario;
        }

        /// <inheritdoc/>
        public override bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;

            switch (propertyName)
            {
                case nameof(UsuarioId):
                    value = UsuarioId.ToString();
                    break;

                case nameof(Email):
                    value = Email;
                    break;

                case nameof(Nombre):
                    value = Nombre ?? string.Empty;
                    break;

                case nameof(Password):
                    value = Password;
                    break;

                case nameof(Token):
                    value = Token;
                    break;

                case nameof(Role):
                    value = Role;
                    break;

                case nameof(GoogleLogin):
                    value = GoogleLogin.ToString();
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Id: {UsuarioId}, Nombre: {Nombre}";
        }

        /// <summary>
        /// Operador explicito que convierte un UsuarioCore en UsuarioDTO
        /// </summary>
        /// <param name="usuario">Usuario a convertir</param>
        public static explicit operator UsuarioDTO(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException("UsuarioCore no puede ser null", nameof(usuario));

            return new UsuarioDTO()
            {
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Password = usuario.Password,
                Email = usuario.Email,
                Token = usuario.Token,
                GoogleLogin = usuario.GoogleLogin,
                Role = usuario.Role
            };
        }

        /// <summary>
        /// Convierte un UsuarioDTO en un UsuarioCore
        /// </summary>
        /// <param name="usuarioDTO">UsuarioDTO a convertir</param>
        /// <returns>Instancia UsuarioCore</returns>
        /// <exception cref="ArgumentNullException">
        /// Error en la logica de negocio. Parametro recibido no puede ser null
        /// </exception>
        public static Usuario ConvertToCore(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO == null)
            {
                throw new ArgumentNullException("El registro usuarioDTO no puede ser null", nameof(usuarioDTO));
            }

            Usuario usuario = new Usuario()
            {
                DTO_Base = usuarioDTO,
                Nombre = usuarioDTO.Nombre,
                Password = usuarioDTO.Password,
                Email = usuarioDTO.Email,
                Token = usuarioDTO.Token,
                GoogleLogin = usuarioDTO.GoogleLogin,
                Role = usuarioDTO.Role
            };

            // Asigna las listas de las clases
            usuario.SetCorreosList(usuarioDTO.Correos);
            usuario.SetContactosList(usuarioDTO.Contactos);

            return usuario;
        }

        /// <summary>
        /// Convierte una lista de CorreosDTO en una lista de CorreosCore y 
        /// lo asigna a la lista de correos del usuario
        /// </summary>
        /// <param name="correosDTOList">Lista de correos a convertir y asignar</param>
        private bool SetCorreosList(List<CorreoDTO> correosDTOList)
        {
            Correos.Clear();

            foreach (var correo in correosDTOList)
            {
                Correos.Add(Correo.ConvertToCore(correo));
            }

            SetChanges(OpResul.Page);
            return true;
        }

        /// <summary>
        /// Convierte una lista de ContactosDTO en una lista de ContactosCore y 
        /// lo asigna a la lista de contactos del usuario
        /// </summary>
        /// <param name="correosDTOList">Lista de correos a convertir y asignar</param>
        private bool SetContactosList(List<ContactoDTO> contactoDTOList)
        {
            Contactos.Clear();

            foreach (var contacto in contactoDTOList)
            {
                //Contactos.Add(Contacto.ConvertToCore(contacto));
            }

            SetChanges(OpResul.Page);
            return true;
        }

        /// <summary>
        /// El token del usuario se debe refresear cada X tiempo para que sea seguro
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ChangeToken(string token)
        {
            Token = token;
            SetChanges(OpResul.Line);
            return true;
        }
    }
}
