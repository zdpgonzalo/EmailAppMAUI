using MailAppMAUI.Gestion;
using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using MailAppMAUI.General;
using MailAppMAUI.Config;

namespace MailAppMAUI.Repositorios
{
    public class EliminadoRepository : IRepository<Eliminado>
    {
        private readonly Context contexto;
        private Configuration conf;

        //Lista local de eliminados del repositorio
        private static List<Eliminado> EliminadosUsuario;

        //Se dispara cuando se actualiza el eliminado
        public static event Action<OpResul>? OnUpdateEliminado;

        private static readonly object _lock = new object();

        public EliminadoRepository(Context context)
        {
            this.contexto = context;

            conf = Configuration.Config ?? new Configuration();

            // Carga eliminados del usuario actual desde la base de datos.
            EliminadosUsuario = contexto.Eliminados
                .Where(e => e.UsuarioId == conf.User.UserId)
                .Include(e => e.Adjuntos)
                .ToList();
        }

        #region CONVERSIONES CORE-DTO

        /// <summary>
        /// Convierte una entidad DTO a una entidad Core
        /// </summary>
        /// <param name="entityDTO">Entidad DTO a convertir</param>
        /// <returns>Entidad DTO convertida a Core</returns>
        protected Eliminado? MapToCore(EliminadoDTO EliminadoDTO)
        {
            try
            {
                return Eliminado.ConvertToCore(EliminadoDTO);
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex, "Error al convertir un EliminadoCore en EliminadoDTO");
                return null;
            }
        }

        /// <summary>
        /// Convierte una entidad Core a una entidad DTO
        /// </summary>
        /// <param name="entityCore">Entidad Core a convertir</param>
        /// <returns>Entidad Core convertida a DTO</returns>
        protected EliminadoDTO MapToDTO(Eliminado Eliminado)
        {
            return (EliminadoDTO)Eliminado;
        }

        #endregion

        public async Task<bool> AddAsync(Eliminado Eliminado, bool save = true)
        {
            try
            {
                bool isValid = IsValid(Eliminado);

                if (!isValid)
                {
                    throw new Exception("Duplicado de Eliminado o propiedad unique no valida");
                }

                // Agregar el Eliminado a la base de datos
                await contexto.Eliminados.AddAsync(Eliminado);
                EliminadosUsuario.Add(Eliminado);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    // Actualizar interfaz
                    OnUpdateEliminado?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int? EliminadoId, bool save = true)
        {
            try
            {
                if (EliminadoId == null)
                {
                    return false;
                }

                Eliminado? Eliminado = await contexto.Eliminados.FindAsync(EliminadoId);

                if (Eliminado == null)
                {
                    return false;
                }

                // Eliminar el Eliminado de la base de datos
                contexto.Eliminados.Remove(Eliminado);
                EliminadosUsuario.Remove(Eliminado);

                if (save)
                {
                    // Guardar los cambios en la base de datos
                    contexto.SaveChanges();

                    // Actualizar interfaz
                    OnUpdateEliminado?.Invoke(AppChanges.OpResul);
                }

                return true;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return false;
            }
        }

        public List<Eliminado> GetAll()
        {
            try
            {
            //    List<Eliminado> Eliminados = contexto.Eliminados
            //        .Include(e => e.Adjuntos)
            //        .ToList();

                return EliminadosUsuario;
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex);
                return new List<Eliminado>();
            }
        }

        public Eliminado? GetById(int eliminadoId)
        {
            var eliminado = contexto.Eliminados.Find(eliminadoId);

            if (eliminado == null)
            {
                return null;
            }

            //Apunta al objeto actualizado del contexto en la lista local
            var index = EliminadosUsuario.FindIndex(e => e.EliminadoId == eliminado?.EliminadoId);
            if (index >= 0)
            {
                EliminadosUsuario[index] = eliminado;
            }

            return eliminado;
        }

        public bool Update(Eliminado eliminado, bool updateUI = true)
        {
            try
            {
                contexto.Update(eliminado);
                contexto.SaveChanges();

                //Actualiza la lista local
                int index = EliminadosUsuario.FindIndex(e => e.EliminadoId == eliminado.EliminadoId);
                if (index >= 0)
                {
                    EliminadosUsuario[index] = eliminado;
                }
                else
                {
                    EliminadosUsuario.Add(eliminado);
                }

                // Actualizar interfaz
                if (updateUI) 
                { 
                    OnUpdateEliminado?.Invoke(AppChanges.OpResul);
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
            return EliminadosUsuario.Count();
        }

        /// <summary>
        /// Metodo que comprueba si ya existe una Eliminado en la BD con las
        /// mismas propiedades unicas
        /// </summary>
        /// <param name="Eliminado">Eliminado a comprobar</param>
        /// <returns>True si no hay duplicado y false en caso contrario</returns>
        private bool IsValid(Eliminado Eliminado)
        {
            lock (Context._methodLock)
            {
                if(Eliminado.EliminadoId == 0)
                {
                    return true;
                }

                return !contexto.Eliminados.Any(d => d.EliminadoId == Eliminado.EliminadoId);
            }
            //return existeEliminado == false;
        }
    }
}
