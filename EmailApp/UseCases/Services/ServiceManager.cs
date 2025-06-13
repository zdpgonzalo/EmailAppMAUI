using MailAppMAUI.Contexto;
using MailAppMAUI.General;
using MailAppMAUI.Repositorios;
using MailAppMAUI.UseCases.Services;
using MailAppMAUI.UseCases.Services.ConcreteServices;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MailAppMAUI.UseCases.Services
{

    public class ServiceManager : IService
    {
        //REPOS
        private readonly RepositoryManager repositoryManager;

        //GENERAR RESPUESTA
        private readonly IGenerarRespuestas generarRespuestas;
        private readonly ReadEmailService readEmail;
        private readonly ReadPoweGestFileService readPowerGest;
        private readonly SendEmailService sendEmail;

        //Referencia a GesInter()
        GesInter gesInter;

        private readonly Context context;

        private readonly IServiceScopeFactory _scopeFactory;

        public ServiceManager(RepositoryManager repositoryManager, IGenerarRespuestas generarRespuestas, ReadEmailService readEmail, ReadPoweGestFileService readPowerGest, SendEmailService sendEmail,
            Context context, IServiceScopeFactory scopeFactory, GesInter gesInter)
        {
            this.repositoryManager = repositoryManager;
            this.generarRespuestas = generarRespuestas;
            this.readEmail = readEmail;
            this.context = context;
            this._scopeFactory = scopeFactory;
            this.readPowerGest = readPowerGest;
            this.sendEmail = sendEmail;

            this.gesInter = gesInter; // pásalo también aquí
            RegisterServices();
        }

        public void RegisterServices()
        {
            var ServNames = Enum.GetNames(typeof(RegisteredServices));

            foreach (var serv in ServNames)
            {
                gesInter.RegisterService(serv, this);
            }
        }

        //public IService OpenService(string oper)
        //{
        //    IService service = null;

        //    switch (Data.GetEnum(oper, RegisteredServices.None))
        //    {
        //        case RegisteredServices.None:
        //            break;

        //        case RegisteredServices.ReadEmailService:
        //            service = new ReadEmailService(repositoryManager, context);
        //            break;

        //        case RegisteredServices.GenerateResponseService:
        //            service = new GenerateResponseService(repositoryManager, generarRespuestas);
        //            break;
        //        case RegisteredServices.LoadDataService:
        //            service = new LoadDataService();
        //        break;
        //    }

        //    if (service != null)
        //    {
        //        service.OpenService(oper);
        //    }

        //    return service;

        //}

        public IService OpenService(string oper)
        {
            IService service = null;

            var serviceType = Data.GetEnum(oper, RegisteredServices.None);

            using var scope = _scopeFactory.CreateScope();
            var provider = scope.ServiceProvider;

            switch (serviceType)
            {
                case RegisteredServices.None:
                    break;

                case RegisteredServices.ReadEmailService:
                    service = provider.GetRequiredService<ReadEmailService>();
                    break;

                case RegisteredServices.GenerateResponseService:
                    service = provider.GetRequiredService<GenerateResponseService>();
                    break;

                case RegisteredServices.PlanOverService:
                    service = provider.GetRequiredService<PlanOverService>();
                    break;                
                
                case RegisteredServices.ReadPoweGestFileService:
                    service = provider.GetRequiredService<ReadPoweGestFileService>();
                    break;                
                
                case RegisteredServices.SendEmailService:
                    service = provider.GetRequiredService<SendEmailService>();
                    break;
            }

            if (service != null)
            {
                service.OpenService(oper);
            }

            return service;
        }

        public void AddService(string serv, object[] info)
        {
            gesInter.AddService(serv, info);
        }

        public void AddService(string serv, object[] info, int miliseconds, bool isDelayed)
        {
            gesInter.AddService(serv, info, miliseconds, isDelayed);
        }

        public bool CloseService()
        {
            //throw new NotImplementedException();
            return true;
        }

        public Task<object> Execute(object action, object[] info)
        {
            throw new NotImplementedException();

        }
    }
}
