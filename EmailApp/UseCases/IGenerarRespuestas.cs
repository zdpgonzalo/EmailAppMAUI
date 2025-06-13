
using MailAppMAUI.Core;

namespace MailAppMAUI.UseCases
{
    public interface IGenerarRespuestas
    {
        /// <summary>
        /// Genera una respuesta por cada correo
        /// </summary>
        /// <param name="correos">Lista de correos para generar la respuesta</param>
        /// <returns>
        /// Lista de objetos <see cref="Respuesta"/> representando las respuestas a cada correo en una lista.
        /// </returns>
        /// <remarks>
        /// Este método utilizará una IA para generar correos respuesta.
        /// </remarks>
        public List<Task<Respuesta>> GenerarRespuestasIA(List<Correo> correos);

        /// <summary>
        /// Una respuesta a UN correo
        /// </summary>
        /// <param name="correo">Correo para generar la respuesta</param>
        /// <param name="ctx">Contexto para generar la respuesta</param>
        /// <returns>
        /// Una respuesta <see cref="Respuesta"/>
        /// </returns>
        /// <remarks>
        /// Este método utilizará una IA para generar UN correo de respuesta.
        /// </remarks>
        public Task<Respuesta> GenerarRespuestaIA(Correo correo, string ctx);

        public Task<Respuesta> RegenerarRespuestaIA(Correo correo, string ctx);
    }
}
