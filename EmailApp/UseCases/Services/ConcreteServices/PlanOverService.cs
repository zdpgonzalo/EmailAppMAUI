using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System.Net;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MailKit.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MailAppMAUI.UseCases.Services;
using MailAppMAUI.Config;
using MailAppMAUI.Repositorios;
using MailAppMAUI.Core;
using MailAppMAUI.Contexto;
using MailAppMAUI.General;
using Logger = Ifs.Comun.Logger;



namespace MailAppMAUI.UseCases.Services.ConcreteServices
{
    public class PlanOverService : IService
    {
        //RUTA DE GUARDAR ARCHIVOS
        static Configuration Conf { get; set; }

        //PARA SUBIRTLO AL SERVIDOR
        private static readonly HttpClient _httpClient = new HttpClient();

        //IMAP CONNECTION
        private ImapClient client;
        private bool isConnected = false;

        //CONSTRUCTORES
        private readonly IServiceScopeFactory scopeFactory;

        public PlanOverService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public PlanOverService() { }


        /// <summary>
        /// --
        /// </summary>
        public IService OpenService(string service)//Meter poca cosa porque si cada vez que se añade un servicio se llama aquí, estoy manteniendo la conexion IMAP desde que lo añado hasta que lo ejecuto.
        {
            if ((Conf = Configuration.Config) == null)
            {
                Conf = new Configuration();
            }

            return this;
        }
        /// <summary>
        /// Comprueba si el plan del cliente ha acabado o si han pasado 24h para resetear
        /// </summary>
        /// <param name="action">No sé</param>
        /// <param name="info"></param>
        public async Task<object> Execute(object action, object[] info)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    // Crear un contexto scoped a mano
                    var context = scope.ServiceProvider.GetRequiredService<Context>();

                    // Crear manualmente una instancia de RepositoryManager usando ese contexto
                    var repositoryManager = new RepositoryManager(context);

                    Usuario user = repositoryManager.UsuarioRepository.GetById(Conf.User.UserId);

                    if (user.Plan == null)
                        return false;

                    DateTime ahora = DateTime.Now;
                    DateTime inicioDelDia = DateTime.Today;

                    if (user.Plan.UltimoReset < inicioDelDia)
                    {
                        user.Plan.ResetPlan();
                        context.Update(user.Plan);
                        await context.SaveChangesAsync();
                    }


                    // Cancelar el plan si ha finalizado y no haga nada si es gratuito
                    if (user.Plan.Tipo != PlanType.Gratuito && ahora > user.Plan.FechaFinalizacion)
                    {
                        CancelarPlan(user);
                        context.Update(user.Plan);
                        context.Update(user);
                        await context.SaveChangesAsync();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogLine("PlanOverService", "Error al ejecutar servicio: " + ex, DateTime.Now.ToString());
                return false;
            }
        }

        /// <summary>
        /// Volver al plan gratuito
        /// </summary>
        private void CancelarPlan(Usuario user)
        {
            user.Plan.ChangePlan(PlanType.Gratuito);
        }

        /// <summary>
        /// Cierra el servicio
        /// </summary>
        public bool CloseService()
        {
            return true;
        }
    }
}
