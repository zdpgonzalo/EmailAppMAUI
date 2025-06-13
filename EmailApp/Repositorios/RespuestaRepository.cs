using MailAppMAUI.Gestion;
using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using MailAppMAUI.General;
using MailAppMAUI.Config;

namespace MailAppMAUI.Repositorios
{
    public class RespuestaRepository : IRepository<Respuesta>
    {
        private readonly Context contexto;

        //Lista local de respuestas del repositorio
        private static List<Respuesta> RespuestasUsuario = new List<Respuesta>();

        //Se dispara cuando se actualiza la respuesta
        public static event Action<OpResul>? OnUpdateRespuesta;

        private static readonly object _lock = new object(); // Objeto de bloqueo

        public RespuestaRepository(Context context)
        {
            this.contexto = context;

            // Carga contactos del usuario actual desde la base de datos.
            RespuestasUsuario = contexto.Respuestas
                .Include(r => r.Adjuntos)
                .ToList();
        }

        #region CONVERSIONES CORE-DTO

        /// <summary>
        /// Convierte una entidad DTO a una entidad Core
        /// </summary>
        /// <param name="entityDTO">Entidad DTO a convertir</param>
        /// <returns>Entidad DTO convertida a Core</returns>
        protected Respuesta? MapToCore(RespuestaDTO respuestaDTO)
        {
            try
            {
                return Respuesta.ConvertToCore(respuestaDTO);
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex, "Error al convertir un RespuestaCore en RespuestaDTO");
                return null;
            }
        }

        /// <summary>
        /// Convierte una entidad Core a una entidad DTO
        /// </summary>
        /// <param name="entityCore">Entidad Core a convertir</param>
        /// <returns>Entidad Core convertida a DTO</returns>
        protected RespuestaDTO MapToDTO(Respuesta respuesta)
        {
            return (RespuestaDTO)respuesta;
        }

        #endregion

        public async Task<bool> AddAsync(Respuesta respuesta, bool save = true)
        {
            try
            {
                bool isValid = IsValid(respuesta);

                if (!isValid)
                {
                    throw new Exception("Duplicado de respuesta o propiedad unique no valida");
                }

                // Agregar el respuesta a la base de datos
                await contexto.Respuestas.AddAsync(respuesta);
                RespuestasUsuario.Add(respuesta);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    // Actualizar interfaz
                    OnUpdateRespuesta?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int? respuestaId, bool save = true)
        {
            try
            {
                if (respuestaId == null)
                {
                    return false;
                }

                Respuesta? respuesta = await contexto.Respuestas.FindAsync(respuestaId);

                if (respuesta == null)
                {
                    return false;
                }

                // Eliminar el respuesta de la base de datos
                contexto.Respuestas.Remove(respuesta);
                RespuestasUsuario.Remove(respuesta);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    // Actualizar interfaz
                    OnUpdateRespuesta?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public List<Respuesta> GetAll()
        {
            try
            {
                //List<Respuesta> respuestas = contexto.Respuestas
                //    .Include(e => e.Adjuntos)
                //    .ToList();

                return RespuestasUsuario;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return new List<Respuesta>();
            }
        }

        public Respuesta? GetById(int respuestaId)
        {
            var respuesta = contexto.Respuestas.Find(respuestaId);

            if (respuesta == null)
            {
                return null;
            }

            //Apunta al objeto actualizado del contexto en la lista local
            var index = RespuestasUsuario.FindIndex(r => r.RespuestaId == respuesta?.RespuestaId);
            if (index >= 0)
            {
                RespuestasUsuario[index] = respuesta;
            }

            return respuesta;
        }

        public bool Update(Respuesta respuesta, bool updateUI = true)
        {
            try
            {
                contexto.Update(respuesta);
                contexto.SaveChanges();

                //Actualiza la lista local
                int index = RespuestasUsuario.FindIndex(r => r.RespuestaId == respuesta?.RespuestaId);
                if (index >= 0)
                {
                    RespuestasUsuario[index] = respuesta;
                }
                else
                {
                    RespuestasUsuario.Add(respuesta);
                }

                // Actualizar interfaz
                if (updateUI)
                {
                    OnUpdateRespuesta?.Invoke(AppChanges.OpResul);
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
            return RespuestasUsuario.Count();
        }

        /// <summary>
        /// Metodo que comprueba si ya existe una respuesta en la BD con las
        /// mismas propiedades unicas
        /// </summary>
        /// <param name="respuesta">Respuesta a comprobar</param>
        /// <returns>True si no hay duplicado y false en caso contrario</returns>
        private bool IsValid(Respuesta respuesta)
        {
            lock (Context._methodLock)
            {
                if(respuesta.RespuestaId == 0)
                {
                    return true;
                }

                return !contexto.Respuestas.Any(d => d.RespuestaId == respuesta.RespuestaId);
            }
            //return existeRespuesta == false;
        }
    }
}
