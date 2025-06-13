using MailAppMAUI.Gestion;
using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using MailAppMAUI.DTOs;
using MailAppMAUI.General;
using MailAppMAUI.Config;

namespace MailAppMAUI.Repositorios
{
    public class ContactoRepository : IContactoRepository
    {
        private readonly Context contexto;
        private Configuration conf;

        //Lista local de correos del repositorio
        private static List<Contacto> ContactosUsuario = new();

        //Se dispara cuando se actualiza el contacto
        public static event Action<OpResul>? OnUpdateContactos;

        public ContactoRepository(Context context)
        {
            this.contexto = context;

            conf = Configuration.Config ?? new Configuration();

            // Carga contactos del usuario actual desde la base de datos.
            ContactosUsuario = contexto.Contactos
                .Where(c => c.UsuarioId == conf.User.UserId)
                .ToList();
        }

        #region Conversiones CORE-DTO

        /// <summary>
        /// Convierte una entidad DTO a una entidad Core
        /// </summary>
        /// <param name="entityDTO">Entidad DTO a convertir</param>
        /// <returns>Entidad DTO convertida a Core</returns>
        protected Contacto? MapToCore(ContactoDTO contactoDTO)
        {
            try
            {
                return null; 
                //return Contacto.ConvertToCore(contactoDTO);
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex, "Error al convertir un ContactoCore en ContactoDTO");
                return null;
            }
        }

        /// <summary>
        /// Convierte una entidad Core a una entidad DTO
        /// </summary>
        /// <param name="entityCore">Entidad Core a convertir</param>
        /// <returns>Entidad Core convertida a DTO</returns>
        protected ContactoDTO MapToDTO(Contacto contacto)
        {
            return (ContactoDTO)contacto;
        }

        #endregion

        public async Task<bool> AddAsync(Contacto contacto, bool save = true)
        {
            try
            {
                bool isValid = IsValid(contacto);

                if (!isValid)
                {
                    throw new Exception("Duplicado de contacto o propiedad unique no valida");
                }

                // Agregar el contacto a la base de datos
                await contexto.Contactos.AddAsync(contacto);

                ContactosUsuario.Add(contacto);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    OnUpdateContactos?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int? contactoId, bool save = true)
        {
            try
            {
                if(contactoId == null)
                {
                    return false;
                }

                Contacto? contacto = await contexto.Contactos.FindAsync(contactoId);

                if (contacto == null)
                {
                    return false;
                }

                // Eliminar el contacto de la base de datos
                contexto.Contactos.Remove(contacto);

                ContactosUsuario.Remove(contacto);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    OnUpdateContactos?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public List<Contacto> GetAll()
        {
            try
            {
                //List<Contacto> contactos = contexto.Contactos
                //    .ToList();

                return ContactosUsuario;
            }
            catch (Exception ex) 
            {
                WebLog.LogError(ex);
                return new List<Contacto>();
            }
        }

        public Contacto? GetById(int contactoId)
        {
            var contacto = contexto.Contactos.Find(contactoId);

            if (contacto == null)
                return null;

            //Apunta al objeto actualizado del contexto en la lista local
            var index = ContactosUsuario.FindIndex(c => c.ContactoId == contacto?.ContactoId);
            if (index >= 0)
            {
                ContactosUsuario[index] = contacto;
            }

            return contacto;
        }

        public Contacto? GetByEmail(string email)
        {
            var contactos = contexto.Contactos.ToList();
            Contacto? contacto = contactos.FirstOrDefault(c => c.Email == email);

            if(contacto == null)
            {
                return null;
            }

            return contacto;
        }

        public bool Update(Contacto contacto, bool updateUI = true)
        {
            try
            {
                contexto.Update(contacto);
                contexto.SaveChanges();

                //Actualiza la lista local
                int index = ContactosUsuario.FindIndex(c => c.ContactoId == contacto.ContactoId);
                if (index >= 0)
                {
                    ContactosUsuario[index] = contacto;
                }
                else
                {
                    ContactosUsuario.Add(contacto);
                }

                //Actualiza la interfaz
                if (updateUI)
                {
                    OnUpdateContactos?.Invoke(AppChanges.OpResul);
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
            return ContactosUsuario.Count();
        }

        /// <summary>
        /// Metodo que comprueba si ya existe una contacto en la BD con las
        /// mismas propiedades unicas
        /// </summary>
        /// <param name="contacto">Contacto a comprobar</param>
        /// <returns>True si no hay duplicado y false en caso contrario</returns>
        private bool IsValid(Contacto contacto)
        {
            if(contacto.ContactoId == 0)
            {
                return true;
            }

            bool existeContacto = contexto.Contactos
                .Any(c => c.ContactoId == contacto.ContactoId);

            return existeContacto == false;
        }
    }
}
