using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using MailAppMAUI.General;
using MailAppMAUI.Gestion;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MailAppMAUI.Repositorios
{
    public class ConversacionRepository : IRepository<Conversacion>
    {
        private readonly Context contexto;

        private static List<Conversacion> ConversacionesUsuario = new();

        //Se dispara cuando se actualiza la conversacion
        public static event Action<OpResul>? OnUpdateConversacion;

        private static readonly object _lock = new object();

        public ConversacionRepository(Context context)
        {
            this.contexto = context;

            //Carga las conversaciones desde la base de datos
            ConversacionesUsuario = contexto.Conversacion
                .Include(c => c.Correos)
                .Include(c => c.Respuestas)
                .ToList();
        }

        #region CONVERSION CORE-DTO

        /// <summary>
        /// Convierte una entidad DTO a una entidad Core
        /// </summary>
        /// <param name="entityDTO">Entidad DTO a convertir</param>
        /// <returns>Entidad DTO convertida a Core</returns>
        protected Conversacion? MapToCore(ConversacionDTO ConversacionDTO)
        {
            try
            {
                return Conversacion.ConvertToCore(ConversacionDTO);
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex, "Error al convertir un ConversacionCore en ConversacionDTO");
                return null;
            }
        }

        /// <summary>
        /// Convierte una entidad Core a una entidad DTO
        /// </summary>
        /// <param name="entityCore">Entidad Core a convertir</param>
        /// <returns>Entidad Core convertida a DTO</returns>
        protected ConversacionDTO MapToDTO(Conversacion Conversacion)
        {
            return (ConversacionDTO)Conversacion;
        }

        #endregion

        public async Task<bool> AddAsync(Conversacion conversacion, bool save = true)
        {
            try
            {
                bool isValid = IsValid(conversacion);

                if (!isValid)
                {
                    throw new Exception("Duplicado de Conversacion o propiedad unique no valida");
                }

                // Agregar el Conversacion a la base de datos
                await contexto.Conversacion.AddAsync(conversacion);
                ConversacionesUsuario.Add(conversacion);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    // Actualizar interfaz
                    OnUpdateConversacion?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int? ConversacionId, bool save = true)
        {
            try
            {
                if (ConversacionId == null)
                {
                    return false;
                }

                Conversacion? Conversacion = await contexto.Conversacion.FindAsync(ConversacionId);

                if (Conversacion == null)
                {
                    return false;
                }

                // Eliminar el Conversacion de la base de datos
                contexto.Conversacion.Remove(Conversacion);
                ConversacionesUsuario.Remove(Conversacion);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    // Actualizar interfaz
                    OnUpdateConversacion?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public List<Conversacion> GetAll()
        {
            try
            {
                return ConversacionesUsuario;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return new List<Conversacion>();
            }
        }

        public Conversacion? GetById(int conversacionId)
        {
            var conver = contexto.Conversacion.Find(conversacionId);

            if (conver == null)
            {
                return null;
            }

            //Apunta al objeto actualizado del contexto en la lista local
            var index = ConversacionesUsuario.FindIndex(c => c.ConversacionId == conver?.ConversacionId);
            if (index >= 0)
            {
                ConversacionesUsuario[index] = conver;
            }

            return conver;
        }

        public bool Update(Conversacion conversacion, bool updateUI = true)
        {
            try
            {
                contexto.Update(conversacion);
                contexto.SaveChanges();

                //Actualiza la lista local
                int index = ConversacionesUsuario.FindIndex(c => c.ConversacionId == conversacion.ConversacionId);
                if (index >= 0)
                {
                    ConversacionesUsuario[index] = conversacion;
                }
                else
                {
                    ConversacionesUsuario.Add(conversacion);
                }

                // Actualizar interfaz
                if (updateUI)
                {
                    OnUpdateConversacion?.Invoke(AppChanges.OpResul);
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
            return ConversacionesUsuario.Count();
        }

        /// <summary>
        /// Metodo que comprueba si ya existe una Conversacion en la BD con las
        /// mismas propiedades unicas
        /// </summary>
        /// <param name="Conversacion">Conversacion a comprobar</param>
        /// <returns>True si no hay duplicado y false en caso contrario</returns>
        private bool IsValid(Conversacion Conversacion)
        {
            lock (Context._methodLock)
            {
                if(Conversacion.ConversacionId == 0)
                {
                    return true;
                }

                return !contexto.Conversacion.Any(d => d.ConversacionId == Conversacion.ConversacionId);
            }
            //return existeConversacion == false;
        }
    }
}
