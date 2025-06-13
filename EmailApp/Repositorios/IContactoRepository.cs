using MailAppMAUI.Core;

namespace MailAppMAUI.Repositorios
{
    public interface IContactoRepository : IRepository<Contacto>
    {
        /// <summary>
        /// Devuelve un Contacto del repositorio a traves de su email
        /// Si no existe devuelve null
        /// </summary>
        /// <param name="email">Email del contacto a buscar.</param>
        public Contacto? GetByEmail(string email);
    }
}
