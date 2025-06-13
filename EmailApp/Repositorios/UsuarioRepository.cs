using MailAppMAUI.Gestion;
using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MailAppMAUI.Repositorios
{
    public class UsuarioRepository : IRepository<Usuario>
    {
        private readonly Context contexto;

        //Lista local de usuarios del repositorio
        private static List<Usuario> UsuariosLocal = new List<Usuario>();

        public UsuarioRepository(Context context)
        {
            this.contexto = context;

            UsuariosLocal = contexto.Usuarios
                .Include(e => e.Contactos)
                .Include(e => e.Correos)
                .Include(e => e.Plan)
                .ToList();
        }

        #region CONVERSIONES CORE-DTO

        /// <summary>
        /// Convierte una entidad DTO a una entidad Core
        /// </summary>
        /// <param name="entityDTO">Entidad DTO a convertir</param>
        /// <returns>Entidad DTO convertida a Core</returns>
        protected Usuario? MapToCore(UsuarioDTO usuarioDTO)
        {
            try
            {
                return Usuario.ConvertToCore(usuarioDTO);
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex, "Error al convertir un UsuarioCore en UsuarioDTO");
                return null;
            }
        }

        /// <summary>
        /// Convierte una entidad Core a una entidad DTO
        /// </summary>
        /// <param name="entityCore">Entidad Core a convertir</param>
        /// <returns>Entidad Core convertida a DTO</returns>
        protected UsuarioDTO MapToDTO(Usuario usuario)
        {
            return (UsuarioDTO)usuario;
        }

        #endregion 

        public async Task<bool> AddAsync(Usuario usuario, bool save = true)
        {
            try
            {
                bool isValid = IsValid(usuario);

                if (!isValid)
                {
                    throw new Exception("Duplicado de usuario o propiedad unique no valida");
                }

                // Agregar el usuario a la base de datos
                await contexto.Usuarios.AddAsync(usuario);
                UsuariosLocal.Add(usuario);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int? usuarioId, bool save = true)
        {
            try
            {
                if (usuarioId == null)
                {
                    return false;
                }

                Usuario? usuario = await contexto.Usuarios.FindAsync(usuarioId);

                if (usuario == null)
                {
                    return false;
                }

                // Eliminar el usuario de la base de datos
                contexto.Usuarios.Remove(usuario);
                UsuariosLocal.Remove(usuario);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public List<Usuario> GetAll()
        {
            try
            {
                //List<Usuario> usuarios = contexto.Usuarios
                //    .Include(e => e.Contactos)
                //    .Include(e => e.Correos)
                //    .Include(e => e.Plan)
                //    .ToList();

                return UsuariosLocal;
            }
            catch (Exception ex) 
            {
                WebLog.LogError(ex);
                return new List<Usuario>();
            }
        }

        public Usuario? GetById(int usuarioId)
        {
            var usuario = contexto.Usuarios
                    .Find(usuarioId);

            if (usuario == null)
            {
                return null;
            }

            //Apunta al objeto actualizado del contexto en la lista local
            var index = UsuariosLocal.FindIndex(u => u.UsuarioId == usuario?.UsuarioId);
            if (index >= 0)
            {
                UsuariosLocal[index] = usuario;
            }

            return usuario;
        }

        public bool Update(Usuario usuario, bool updateUI = true)
        {
            try
            {
                contexto.Update(usuario);
                contexto.SaveChanges();

                //Actualiza la lista local
                int index = UsuariosLocal.FindIndex(u => u.UsuarioId == usuario?.UsuarioId);
                if (index >= 0)
                {
                    UsuariosLocal[index] = usuario;
                }
                else
                {
                    UsuariosLocal.Add(usuario);
                }

                if (updateUI)
                {
                    
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public void Save()
        {
            contexto.SaveChangesAsync();
        }

        public int Count()
        {
            return UsuariosLocal.Count();
        }

        /// <summary>
        /// Metodo que comprueba si ya existe una usuario en la BD con las
        /// mismas propiedades unicas
        /// </summary>
        /// <param name="usuario">Usuario a comprobar</param>
        /// <returns>True si no hay duplicado y false en caso contrario</returns>
        private bool IsValid(Usuario usuario)
        {
            if(usuario.UsuarioId == 0)
            {
                return contexto.Usuarios.Any(u => u.Email == usuario.Email) == false;
            }

            bool existeUsuario = contexto.Usuarios
                .Any(d => d.UsuarioId == usuario.UsuarioId && d.Email == usuario.Email);

            return existeUsuario == false;
        }
    }
}
