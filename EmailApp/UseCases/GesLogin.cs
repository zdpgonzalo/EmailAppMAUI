
using MailAppMAUI.UseCases;
using MailAppMAUI.Config;
using MailAppMAUI.Contexto;
using MailAppMAUI.General;
using MailAppMAUI.Gestion;
using MailAppMAUI.Core;
using MailAppMAUI.Repositorios;
using MailAppMAUI.UseCases.Services;
using MailKit.Net.Imap;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Logger = Ifs.Comun.Logger;


namespace MailAppMAUI.UseCases
{
    public class GesLogin : GesBase<GesLogin, GesLogin.Events, GesLogin.Actions,
                                                GesLogin.Names, GesLogin.Tables>
    {
        private RepositoryManager repositoryManager;
        private readonly IGenerarRespuestas generarRespuesta;

        private ServiceManager serviceManager;

        static event Action<Usuario>? OnLogged;

        private readonly IServiceScopeFactory scopeFactory;
        static Configuration Conf { get; set; }

        private readonly Context context;
        public GesLogin(RepositoryManager repositoryManager,
            IGenerarRespuestas generarRespuesta, ServiceManager serviceManager,
            Context context,
            IServiceScopeFactory scopeFactory) : base()
        {
            this.serviceManager = serviceManager;
            this.context = context;
            this.repositoryManager = repositoryManager;
            this.generarRespuesta = generarRespuesta;

            this.scopeFactory = scopeFactory;

            //Para poder meter las credenciales 
            if ((Conf = Configuration.Config) == null)
            {
                Conf = new Configuration();
            }

            //OnLogged += LogUser();
        }

        public enum Tables
        {
            None,
            Usuario
        }

        public enum Names
        {
            None,
        }


        public enum Actions
        {
            None,
            Entrar,
            Registrar,
            ReadEmails,
            CheckIfReal
        }

        public enum Events
        {
            None,
        }

        #region ACTION
        /// <summary>
        /// Realiza una accion de la aplicacion
        /// </summary>
        /// <param name="oper">Operacion a realizar</param>
        /// <param name="table">Tabla sobre la que trabajar</param>
        /// <param name="info">Informacion extra</param>
        /// <returns>Resultado de la operacion ejecutada</returns>
        protected override async Task<object> Action(Actions oper, Tables table, object[] info)
        {
            try
            {
                ResetChanges();
                switch (table)
                {
                    case Tables.None:
                        switch (oper)
                        {
                            default:
                                return -1; // Acción no válida en Tables.None
                        }
                    case Tables.Usuario:
                        switch (oper)
                        {
                            case Actions.Entrar:
                                return await EntrarUsuario(info);

                            case Actions.Registrar:
                                return await RegistrarUsuario(info);

                            case Actions.ReadEmails:
                                return await ReadEmails(); //Para MAUI

                            case Actions.CheckIfReal:
                                return CheckIfReal(); //Para MAUI

                            default:
                                return -1; // Acción no válida en Tables.Correo
                        }

                    default:
                        return -1; // Tabla no válida
                }
            }
            catch (Exception e)
            {
                WebLog.LogError(e, "No se pueden actualizar datos");
                return -1;
            }

        }
        #endregion

        #region SETDATA
        /// <summary>
        /// Realiza un Set de una propiedad o valor de un item indicado
        /// </summary>
        /// <param name="name">Campo o propiedad a modificar</param>
        /// <param name="table">Tabla de la que cambiar el valor</param>
        /// <param name="item">Objeto a modificar el valor</param>
        /// <param name="value">Valor nuevo de la propiedad</param>
        /// <returns>True si realiza el cambio. False en caso contrario</returns>
        protected override bool SetData(Names name, Tables table, object item, object value)
        {
            try
            {
                ResetChanges();
                switch (table)
                {
                    case Tables.None:
                        switch (name)
                        {
                            default:
                                return false;
                        }

                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                WebLog.LogError(e);
                return false;
            }
        }

        #endregion

        #region GETDATA
        /// <summary>
        /// Devuelve datos de las capas inferiores
        /// </summary>
        /// <param name="name">Nombre de la accion</param>
        /// <param name="table">Tabla a procesar</param>
        /// <param name="item">Informacion extra</param>
        /// <returns>Informacion devuelta</returns>
        protected override object GetData(Names name, Tables table, object item)
        {
            switch (table)
            {
                case Tables.None:
                    switch (name)
                    {
                        case Names.None:
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

            return null;
        }
        #endregion


        public bool CheckIfReal()
        {
            if (!UserIsReal())
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        public async Task<bool> EntrarUsuario(object[] info)
        {
            //AUTO LOGIN QUITAR LUEGO
            string email = "hugo.infoser@gmail.com";
            string password = "bbsm ubpw rbjv prkn";

            Usuario usuarioBase = context.Usuarios.FirstOrDefault();
            if (usuarioBase == null)
            {
                string role = "User";
                if (repositoryManager.UsuarioRepository.GetAll().ToList().Count() == 0)
                {
                    role = "Admin";
                }

                Plan plan = new Plan(PlanType.Gratuito);

                await context.AddAsync(plan);
                await context.SaveChangesAsync();

                Usuario user = Usuario.CreateUsuario(email, password, role, plan);
                await repositoryManager.UsuarioRepository.AddAsync(user);

                Conf.User.UserId = user.UsuarioId;
                Conf.User.Email = email;
                Conf.User.Password = password;
                Conf.User.AccessToken = "noAccessTok";
            }


            //Compruebo si hay algo escrito en los campos
            //if (info[0] != null && info[1] != null)
            //{
            //    email = info[0] as string;
            //    password = info[1] as string;
            //}
            //else
            //{
            //    return false;
            //}

            //Obtengo el userID cuyo mail sea el que ha escrito en el campo Username
            int userId = context.Usuarios
                .Where(u => u.Email == email)
                .Select(u => u.UsuarioId) // Seleccionamos solo el UserID
                .FirstOrDefault();

            //Si no es null
            if (userId != null)
            {
                Usuario usuarioAEntrar = repositoryManager.UsuarioRepository.GetById(userId);
                //Compruebo que la contraseña sea la guardad en la dataBase

                if (usuarioAEntrar != null && usuarioAEntrar.Password == password)
                {
                    Conf.User.Email = email;
                    Conf.User.Password = password;
                    Conf.User.AccessToken = "noAccessTok";
                    Conf.User.UserId = usuarioAEntrar.UsuarioId;

                    LoadData();

                    return true;
                }
            }
            return false;
        }

        //Si el plan a acabado, no le deja entrar
        private bool IsActualPlanOver()
        {
            //if (Conf.User.Usuario.Plan.FechaFinalizacion > DateTime.Now)
            //{
            return true;
            //}

            //return false;
        }
        public bool LoadData()
        {
            //Lee todo los correos
            serviceManager.AddService("ReadEmailService", new object[] { 10 }); //Instantaneamente la primera vez
            serviceManager.AddService("ReadEmailService", new object[] { 10 }, 60000, false); //cada 5 minutos

            //GesInter.ServiceCompleted += (serviceName, result) => GenerarRespuesta(serviceName, result);

            return true;
        }

        /// <summary>
        /// Lee el fichero de configuracion, comprueba si el usuario existe y ejecuta el servicio de leer mails
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ReadEmails()
        {
            try
            {
                BaseConfig.LoadConfig();

                //Si el usuario no es real/no es correcto/no hay fichero de confg
                if (!UserIsReal())
                {
                    return false;
                }

                //Busca si está en la BD
                Usuario? user = context.Usuarios
                    .Include(u => u.Plan)
                    .Where(u => u.Email == Conf.User.Email)
                    .FirstOrDefault();

                //Si es null, lo crea
                if (user == null)
                {
                    Plan plan = new Plan(PlanType.Gratuito);

                    await context.AddAsync(plan);
                    await context.SaveChangesAsync();

                    user = Usuario.CreateUsuario(Conf.User.Email, Conf.User.Password, "User", plan);
                    await repositoryManager.UsuarioRepository.AddAsync(user);
                }

                //Lo guarda en el fichero de Configuración
                Conf.User.UserId = user.UsuarioId;


                // **PROBLEMA** Cuando cambia de usuario se meten las ordences circulares otra vez (cada vez que se resetea la app, se cierran, pero si se cambia mil veces de user, se añaden muchas (auqnue no hay un problema real porque se ejecuta nde una en una))
                serviceManager.AddService("ReadEmailService", new object[] { 10 }); //Instantaneamente la primera vez
                serviceManager.AddService("ReadPoweGestFileService", new object[] { 10 }); //Instantaneamente la primera vez
                serviceManager.AddService("PlanOverService", new object[] { 10 }); //Instantaneamente la primera vez

                serviceManager.AddService("PlanOverService", new object[] { 10 }, 60000, false); //cada 5 minutos
                serviceManager.AddService("ReadEmailService", new object[] { 10 }, 120000, false); //cada 5 minutos


                return true;
            }
            catch (Exception ex)
            {
                Logger.LogLine("GesLogin", "Error en método ReadMails " + ex, "");
                return false;
            }
        }

        public void GenerarRespuesta(string serviceName, object info)
        {
            try
            {
                if (serviceName == "ReadEmailService")
                {
                    //GesInter.ServiceCompleted -= (serviceName, result) => GenerarRespuesta(serviceName, result);
                    using (var scope = scopeFactory.CreateScope())
                    {
                        // Crear un contexto scoped a mano
                        var context = scope.ServiceProvider.GetRequiredService<Context>();
                        var repositoryManager = new RepositoryManager(context);

                        serviceManager.AddService("GenerateResponseService", new object[] { repositoryManager.CorreoRepository.GetAll().ToList() }); //TODOS
                    }
                }
            }
            catch (Exception ex) { }

        }

        /// <summary>
        /// Registra al usuario en la base de datos
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <remarks>PUEDE NO FUNCIONAR POR LO DE CONF.USER.MAIL y lo de EMAILCONFIG.INSTACE....</remarks>
        public async Task<bool> RegistrarUsuario(object[] info)
        {
            string email;
            string password;
            string passwordConfirmation;

            //Compruebo si hay algo escrito en los campos
            if (info[0] != null && info[1] != null && info[2] != null)
            {
                email = info[0] as string;

                password = info[1] as string;

                passwordConfirmation = info[2] as string;
            }
            else
            {
                return false;
            }

            if (password != passwordConfirmation)
            {
                return false;
            }

            //Hago esto momentaneamente para que funcione lo del UserIsReal
            Conf.User.Email = email;
            Conf.User.Password = password;

            if (UserIsReal())
            {
                //Obtengo el userID cuyo mail sea el que ha escrito en el campo Username
                bool existingUser = context.Usuarios
                    .Where(u => u.Email == email)
                    .Select(u => u.Email == email) // Seleccionamos solo el UserID
                    .FirstOrDefault();

                if (existingUser)
                {
                    Conf.User.Email = "";
                    Conf.User.Password = "";
                    return false; //Usuario existente
                }
                else
                {
                    Plan plan = new Plan(PlanType.Gratuito);

                    await context.AddAsync(plan);
                    await context.SaveChangesAsync();

                    Usuario _usuario = Usuario.CreateUsuario(email, password, "User", plan);
                    repositoryManager.UsuarioRepository.AddAsync(_usuario);
                }
                Logger.LogLine("StartUp", "Usuario registrado: " + Conf.User.Email + Conf.User.UserId, "");
                return true;
            }
            return false;
        }


        private bool UserIsReal()
        {
            try
            {
                string imapServerString = Conf.User.ImapConexion;//EmailConfig.Instance().GetImapServer();
                int port = Conf.User.ImapPort; //EmailConfig.Instance().GetImapPort();

                using (var client = new ImapClient())
                {
                    // Intentamos conectar al servidor IMAP sin autenticación para verificar que el servidor es accesible
                    client.Connect(imapServerString, port, true);
                    client.Authenticate(new System.Net.NetworkCredential(Conf.User.Email, Conf.User.Password)); // Usa credenciales seguras
                    client.Disconnect(true);
                    return true;
                }
            }
            catch { return false; }
        }
    }
}
