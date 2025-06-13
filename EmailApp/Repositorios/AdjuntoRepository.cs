using MailAppMAUI.Gestion;
using MailAppMAUI.Core;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using MailAppMAUI.Contexto;

namespace MailAppMAUI.Repositorios
{
    public class AdjuntoRepository : IRepository<Adjunto>
    {
        private readonly Context contexto;

        //Lista local de adjuntos del repositorio
        private static List<Adjunto> AdjuntosUsuario = new();

        public AdjuntoRepository(Context context)
        {
            this.contexto = context;

            AdjuntosUsuario = contexto.Adjuntos
                .Include(a => a.Correo)
                .ToList();
        }

        #region Conversiones CORE-DTO

        /// <summary>
        /// Convierte una entidad DTO a una entidad Core
        /// </summary>
        /// <param name="entityDTO">Entidad DTO a convertir</param>
        /// <returns>Entidad DTO convertida a Core</returns>
        protected Adjunto? MapToCore(AdjuntoDTO adjuntoDTO)
        {
            try
            {
                return Adjunto.ConvertToCore(adjuntoDTO);
            }
            catch (Exception ex)
            {
                WebLog.LogError(ex, "Error al convertir un AdjuntoCore en AdjuntoDTO");
                return null;
            }
        }

        /// <summary>
        /// Convierte una entidad Core a una entidad DTO
        /// </summary>
        /// <param name="entityCore">Entidad Core a convertir</param>
        /// <returns>Entidad Core convertida a DTO</returns>
        protected AdjuntoDTO MapToDTO(Adjunto adjunto)
        {
            return (AdjuntoDTO)adjunto;
        }

        #endregion

        public async Task<bool> AddAsync(Adjunto adjunto, bool save = true)
        {
            try
            {
                bool isValid = IsValid(adjunto);

                if (!isValid)
                {
                    throw new Exception("Duplicado de adjunto o propiedad unique no valida");
                }

                // Agregar el adjunto a la base de datos
                await contexto.Adjuntos.AddAsync(adjunto);
                
                AdjuntosUsuario.Add(adjunto);

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

        public async Task<bool> DeleteAsync(int? adjuntoId, bool save = true)
        {
            try
            {
                if (adjuntoId == null)
                {
                    return false;
                }


                Adjunto? adjunto = await contexto.Adjuntos.FindAsync(adjuntoId);

                if (adjunto == null)
                {
                    return false;
                }

                // Eliminar el adjunto de la base de datos
                contexto.Adjuntos.Remove(adjunto);

                AdjuntosUsuario.Remove(adjunto);

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

        public List<Adjunto> GetAll()
        {
            try
            {
                return AdjuntosUsuario;
            }
            catch (Exception ex) 
            {
                WebLog.LogError(ex);
                return new List<Adjunto>();
            }
        }

        public Adjunto? GetById(int adjuntoId)
        {
            var adjunto = contexto.Adjuntos.Find(adjuntoId);

            if (adjunto == null)
                return null;

            //Apunta al objeto actualizado del contexto en la lista local
            var index = AdjuntosUsuario.FindIndex(a => a.AdjuntoId == adjunto?.AdjuntoId);
            if (index >= 0)
            {
                AdjuntosUsuario[index] = adjunto;
            }

            return adjunto;
        }

        public bool Update(Adjunto adjunto, bool updateUI = true)
        {
            try
            {
                contexto.Update(adjunto);
                contexto.SaveChanges();

                //Actualiza la lista local
                int index = AdjuntosUsuario.FindIndex(a => a.AdjuntoId == adjunto.AdjuntoId);
                if (index >= 0)
                {
                    AdjuntosUsuario[index] = adjunto;
                }
                else
                {
                    AdjuntosUsuario.Add(adjunto);
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
            return AdjuntosUsuario.Count();
        }

        /// <summary>
        /// Metodo que comprueba si ya existe una adjunto en la BD con las
        /// mismas propiedades unicas
        /// </summary>
        /// <param name="adjunto">Adjunto a comprobar</param>
        /// <returns>True si no hay duplicado y false en caso contrario</returns>
        private bool IsValid(Adjunto adjunto)
        {
            if(adjunto.AdjuntoId == 0)
            {
                return contexto.Adjuntos.Any(a => a.Ruta == adjunto.Ruta) == false;
            }

            bool existeAdjunto = contexto.Adjuntos
                .Any(a => a.AdjuntoId == adjunto.AdjuntoId || a.Ruta == adjunto.Ruta);

            return existeAdjunto == false;
        }
    }
}
