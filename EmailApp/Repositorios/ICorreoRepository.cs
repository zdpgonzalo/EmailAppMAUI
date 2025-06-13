using MailAppMAUI.Core;
using MailAppMAUI.Repositorios;

namespace MailAppMAUI.Repositorios
{
    public interface ICorreoRepository : IRepository<Correo>
    {
        /// <summary>
        /// Agrega una nueva entidad al repositorio.
        /// </summary>
        /// <param name="entity">Entidad a agregar.</param>
        /// <param name="mimeMessageId">Entidad a agregar.</param>
        public Task<bool> AddAsync(Correo entity, string mimeMessageId);

        /// <summary>
        /// Devuelve true si el mensaje esta almacenado en el repositorio
        /// </summary>
        /// <param name="mensajeId"></param>
        /// <returns></returns>
        public bool ExistMensaje(string mensajeId);

        /// <summary>
        /// Devuelve el correo si existe la llave (mimessage)
        /// </summary>
        /// <param name="mimessage"></param>
        /// <returns></returns>
        public Correo GetCorreoByMimessage(string mimessage);
    }
}
