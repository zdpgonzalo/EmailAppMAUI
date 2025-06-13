
using MailAppMAUI.Core;

namespace MailAppMAUI.UseCases
{
    public interface IEmailService
    {
        /// <summary>
        /// Recupera los últimos correos electrónicos de la bandeja de entrada.
        /// </summary>
        /// <param name="cantidad">Cantidad máxima de correos a obtener.</param>
        /// <returns>
        /// Lista de objetos <see cref="MimeMessage"/> representando los correos electrónicos.
        /// </returns>
        /// <remarks>
        /// Este método establece una conexión IMAP, recupera los mensajes más recientes y cierra la conexión.
        /// Se recomienda manejar excepciones al llamar este método para evitar fallos en caso de problemas de conexión.
        /// </remarks>
        public List<Correo> ReceiveEmails(int cantidad);
    }
}
