using MailAppMAUI.Gestion;
using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using MailAppMAUI.DTOs;
using MailAppMAUI.General;
using MailAppMAUI.Config;
using Microsoft.EntityFrameworkCore;

namespace MailAppMAUI.Repositorios
{
    public class CorreoRepository : ICorreoRepository
    {
        private readonly Context contexto;
        private Configuration conf;

        //Lista <message.Id, Correo> de correos MIME Message procesados en el repositorio
        private static Dictionary<string, Correo> MimeMessagesProcesados = new();
        
        //Lista local de correos del repositorio
        private static List<Correo> CorreosUsuario = new();

        //Se dispara cuando se actualiza un correo
        public static event Action<OpResul>? OnUpdateCorreo;

        public CorreoRepository(Context context)
        {
            this.contexto = context;

            conf = Configuration.Config ?? new Configuration();

            CargarCorreosUsuario();
        }

        /// <summary>
        /// Carga la lista de correos si no ha sido cargada
        /// </summary>
        private void CargarCorreosUsuario()
        {
            if (MimeMessagesProcesados.Count > 0)
                return;

            var correos =  contexto.Correos
                .Where(c => c.UsuarioId == conf.User.UserId && !string.IsNullOrEmpty(c.MensajeId))
                .Include(c => c.Adjuntos)
                .ToList();

            foreach (var correo in correos)
            {
                CorreosUsuario.Add(correo);
                MimeMessagesProcesados.TryAdd(correo.MensajeId, correo);
            }
        }

        #region CONVERSIONES CORE-DTO

        /// <summary>
        /// Convierte una entidad DTO a una entidad Core
        /// </summary>
        /// <param name="entityDTO">Entidad DTO a convertir</param>
        /// <returns>Entidad DTO convertida a Core</returns>
        protected Correo? MapToCore(CorreoDTO correoDTO)
        {
            try
            {
                return Correo.ConvertToCore(correoDTO);
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex, "Error al convertir un CorreoCore en CorreoDTO");
                return null;
            }
        }

        /// <summary>
        /// Convierte una entidad Core a una entidad DTO
        /// </summary>
        /// <param name="entityCore">Entidad Core a convertir</param>
        /// <returns>Entidad Core convertida a DTO</returns>
        protected CorreoDTO MapToDTO(Correo correo)
        {
            return (CorreoDTO)correo;
        }

        #endregion

        public async Task<bool> AddAsync(Correo correo, bool save = true)
        {
            try
            {
                bool isValid = IsValid(correo);

                if (!isValid)
                {
                    throw new Exception("Duplicado de correo o propiedad unique no valida");
                }

                // Agregar el correo a la base de datos
                await contexto.Correos.AddAsync(correo);

                CorreosUsuario.Add(correo);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    // Actualizar interfaz
                    OnUpdateCorreo?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Añade un correo procesado de de un MimeMessage y lo guarda en la BD.
        /// Lleva el registro de los Ids de los correos procesados para no repetirlos
        /// </summary>
        /// <param name="correo">Correo a guardar</param>
        /// <param name="mimeMessageId">Id del mimeMessage procesado</param>
        /// <returns>True si almacena el correo, false en caso contrario</returns>
        public async Task<bool> AddAsync(Correo correo, string mimeMessageId)
        {
            try
            {
                if (MimeMessagesProcesados.ContainsKey(mimeMessageId))
                {
                    return false;
                }

                bool isValid = IsValid(correo);

                if (!isValid)
                {
                    throw new Exception("Duplicado de correo o propiedad unique no valida");
                }

                // Agregar el correo a la base de datos
                await contexto.Correos.AddAsync(correo);

                // Guardar los cambios en la base de datos
                contexto.SaveChanges();

                CorreosUsuario.Add(correo);

                // Actualizar interfaz
                OnUpdateCorreo?.Invoke(AppChanges.GetDataResul().OpResul);

                MimeMessagesProcesados.TryAdd(mimeMessageId, correo);

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int? correoId, bool save = true)
        {
            try
            {
                if (correoId == null)
                {
                    return false;
                }

                Correo? correo = await contexto.Correos.FindAsync(correoId);

                if (correo == null)
                {
                    return false;
                }

                // Eliminar el correo de la base de datos
                contexto.Correos.Remove(correo);

                CorreosUsuario.Remove(correo);

                //Sacarlo de la lista
                bool deleted = MimeMessagesProcesados.Remove(correo.MensajeId);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    // Actualizar interfaz
                    OnUpdateCorreo?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public List<Correo> GetAll()
        {
            try
            {
                //List<Correo> correos = contexto.Correos
                //           //.Include(e => e.Adjuntos)
                //           .ToList();

                return CorreosUsuario;
            }
            catch (Exception ex) 
            {
                WebLog.LogError(ex);
                return new List<Correo>();
            }
        }

        public Correo? GetById(int correoId)
        {
            lock (Context._methodLock)
            {
                var correo = contexto.Correos.Find(correoId);

                if (correo == null)
                    return null;

                //Apunta al objeto actualizado del contexto en la lista local
                var index = CorreosUsuario.FindIndex(c => c.CorreoId == correo?.CorreoId);
                if (index >= 0)
                {
                    CorreosUsuario[index] = correo;
                }

                return correo;
            }
        }

        public Correo GetCorreoByMimessage(string mimessage)
        {
            if (MimeMessagesProcesados.ContainsKey(mimessage))
            {
                //MimeMessagesProcesados.TryGetValue(mimessage, out var correo);

                return MimeMessagesProcesados[mimessage]; //correo;
            }
            return null;
        }

        public bool Update(Correo correo, bool updateUI = true)
        {
            try
            {
                contexto.Update(correo);
                contexto.SaveChanges();

                //Actualiza la lista local
                int index = CorreosUsuario.FindIndex(c => c.CorreoId == correo.CorreoId);
                if (index >= 0)
                {
                    CorreosUsuario[index] = correo;
                }
                else
                {
                    CorreosUsuario.Add(correo);
                }

                // Actualizar interfaz
                if (updateUI)
                {
                    OnUpdateCorreo?.Invoke(AppChanges.OpResul);
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
            return CorreosUsuario.Count();
        }

        /// <summary>
        /// Metodo que comprueba si ya existe una correo en la BD con las
        /// mismas propiedades unicas
        /// </summary>
        /// <param name="correo">Correo a comprobar</param>
        /// <returns>True si no hay duplicado y false en caso contrario</returns>
        private bool IsValid(Correo correo)
        {
            bool existeCorreo = contexto.Correos
                .Any(d => d.CorreoId == correo.CorreoId);

            return existeCorreo == false;
        }

        public bool ExistMensaje(string mensajeId)
        {
            return MimeMessagesProcesados.ContainsKey(mensajeId);
        }
    }
}
