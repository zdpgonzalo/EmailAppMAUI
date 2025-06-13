using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using MailAppMAUI.Repositorios;
using MailKit;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAppMAUI.UseCases.Services.ConcreteServices
{
    public class GenerateResponseService : IService
    {

        //REPOS
        IGenerarRespuestas generarRespuestas;
        IServiceScopeFactory scopeFactory;

        private static readonly object _lock = new object(); // Objeto para sincronización


        //CONSTRUCTORERS
        public GenerateResponseService(IServiceScopeFactory scopeFactory, IGenerarRespuestas generarRespuestas)
        {
            this.scopeFactory = scopeFactory;
            this.generarRespuestas = generarRespuestas;
        }

        public GenerateResponseService() { }

        /// <summary>
        /// Realiza la conexión IMAP
        /// </summary>
        public IService OpenService(string service)
        {
            return this;
        }
        /// <summary>
        /// Genera respuestas de los correos guardados en info[], si el correo pertenece a una conversacion, solo responde al primer correo de cada conversacion 
        /// Útil cuando cargas correos por primera vez y sigue manteniendo la funcionalidad cada vez que te llega uno nuevo (pues será el mas nuevo de esa conver)
        /// </summary>
        /// <param name="action">No sé</param>
        /// <param name="info">Correos para generar respuestas</param>
        /// <returns>Lista de emails con respuesta asociada</returns>
        public async Task<object> Execute(object action, object[] info)
        {
            try
            {
                lock (_lock)
                {
                    var emails = info[0] as List<Correo>;

                    foreach (Correo email in emails)
                    {
                        if (email.RespuestaId == null)
                        {
                            using (var scope = scopeFactory.CreateScope())
                            {
                                var contextoBd = scope.ServiceProvider.GetRequiredService<Context>();

                                var repositoryManager = new RepositoryManager(contextoBd);

                                //Obtengo la conversacion a la que pertenece ese correo
                                //Conversacion conver = repositoryManager.ConversacionRepository.GetById(email.ConversacionId);

                                //Dictionary<string, int> converOrdenada = conver.GetConversacionOrdenada();

                                //Si el primer valor (el mas nuevo) tiene como id el de mi email, significa que ese mail es el mas nuevo y es al que hay que contestar
                                //if (converOrdenada.First().Value == email.CorreoId)
                                //{
                                     GenerarRespuesta(email);
                                //}
                            }
                        }
                    }
                    return emails;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Genera una respuesta con IA
        /// </summary>
        /// <param name="correo">Correo que usará para generar una respuesta</param>
        /// <returns>Respuesta del correo</returns>
        private Task<Respuesta> GenerarRespuesta(Correo correo)
        {
            lock (_lock)
            {
                return generarRespuestas.GenerarRespuestaIA(correo, "");
            }
        }

        /// <summary>
        /// Cierra la conexión IMAP
        /// </summary>
        public bool CloseService()
        {
            return true;
        }
    }
}


