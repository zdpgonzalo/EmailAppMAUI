using System;
using System.Collections;
using System.Collections.Generic;
using MailAppMAUI.UseCases.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MailAppMAUI.UseCases
{
    //WORKHERE

    public class GesInter : GesBase<GesInter, GesInter.Events, GesInter.Actions,
                            GesInter.Names, GesInter.Tables>
    {
       #region ENUMS GESINTER
        public enum Tables
        {
            None, Docum
        }

        public enum Names
        {
            None,
        }
        public enum Actions
        {
            None,
        }
        public enum Events
        {
            None,
        }
        #endregion

        private readonly IServiceScopeFactory _scopeFactory;

        public GesInter(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        #region GESTION DE SERVICIOS EXTERNOS

        private class ServiceInfo
        {
            public string Name;
            public IService Service;
            public bool IsRunnig;

            public object[] Info;

            public ServiceInfo(IService service, string name = null)
            {
                Name = name;
                Service = service;
                IsRunnig = false;
            }

            public ServiceInfo Clone()
            {
                return MemberwiseClone() as ServiceInfo;
            }
        }

        public delegate void ServiceCompletedHandler(string serviceName, object result);
        public static event ServiceCompletedHandler ServiceCompleted;

        static SortedList<string, ServiceInfo> Services;

        /// <summary> Registrar un servicio de la aplicacion
        /// </summary>
        /// <param name="order">   Nombre del servicio   </param>
        /// <param name="service"> Interfaz del servicio </param>
        /// <remarks>
        /// Los servicios deben registrase antes de añadir ordenes
        /// El registro no implica la creacion de la clase asociada
        /// Trabaja como inversison de dependencias para servicios
        /// 
        /// Los servicios se ejecutan siempre de forma secuencial
        /// El servicio puede tener acceso al soporte de aplicaicon
        /// Para ello se pasa referenca a InterData en el arranque
        /// </remarks>

        public void RegisterService(string order, IService service)
        {
            if (Services == null)
                Services = new SortedList<string, ServiceInfo>();

            if (!Services.ContainsKey(order))
                Services[order] = new ServiceInfo(service, order);
        }

        /// <summary> Añadir orden para un servicio registrado
        /// </summary>
        /// <param name="order"> Nombre de la orden a ejecutar </param>
        /// <param name="info">  Parametros para la ejecucion  </param>

        public bool AddService(string order, object[] info, int miliseconds = 0, bool isDelayed = false)
        {
            bool resul = false;

            if (Services != null)
            {
                ServiceInfo serInfo;

                if (Services.TryGetValue(order, out serInfo))
                {
                    if (!serInfo.IsRunnig)
                    {
                        // Arrancar el servicio la primera vez
                        var oper = serInfo.Name;
                        var serProc = serInfo.Service.OpenService(oper);

                        if (serProc != null)
                        {
                            serInfo.Service = serProc; // Instancia real del servicio
                            serInfo.IsRunnig = true;   // Indica servicio en ejecucion
                        }
                        else
                            serInfo = null;  // No se puede arrancar el servicio
                    }

                    if (resul = (serInfo != null))
                    {
                        // Servicio arrancado: crear duplicado
                        var service = serInfo.Clone();

                        // Añadir a la thread de procesos
                        service.Info = info;

                        if (miliseconds > 0) //Tarea con tiempo
                        {
                            if (isDelayed) //Puede ser que sea tarea con retraso
                            {
                                AddOrder(service, miliseconds, isDelayed);
                            }
                            else //Puede ser que sea tarea repetitiva
                            {
                                AddOrder(service, miliseconds);
                            }
                        }
                        else //Tarea de ejecutarse y se va
                        {
                            AddOrder(service);
                        }
                    }
                }
            }

            return resul;
        }

        /// <summary> Ejecutar orden de un servicio de aplicacion
        /// </summary>
        /// <param name="service"> Descriptor del servicio </param>
        /// <returns> Resultado de la operacion </returns>

        //object StartService(ServiceInfo service)
        //{
        //    var order = service.Name;
        //    var info = service.Info;

        //    var resul = service.Service.Execute(order, info);

        //    ServiceCompleted?.Invoke(order, resul);

        //    return resul;
        //}

        object StartService(ServiceInfo serviceInfo)
        {
            using var scope = _scopeFactory.CreateScope();

            // Obtener instancia nueva del servicio desde el scope
            var serviceType = serviceInfo.Service.GetType();
            var scopedService = (IService)scope.ServiceProvider.GetRequiredService(serviceType);

            // Ejecutar dentro del scope válido
            var order = serviceInfo.Name;
            var info = serviceInfo.Info;

            var result = scopedService.Execute(order, info);

            ServiceCompleted?.Invoke(order, result);

            return result;
        }

        /// <summary> Parada de servicios activos
        /// </summary>

        void StopServices()
        {
            if (Services != null)
            {
                foreach (var entry in Services)
                {
                    var service = entry.Value.Service;
                    service.CloseService();
                }
            }
            Services = null;
        }
        #endregion

       #region THREAD DE EJECUCION DE OPERACIONES

        internal enum OrderType
        {
            None,
            Service  // Orden de servicion generica
        }

        /// <summary> Clase de definicion de ordenes y servicios
        /// </summary>

        private class OrderInfo
        {
            public OrderType Type;     // Tipo de operacion
            public ServiceInfo Service;  // Servicio externo

            // Servicios que se hacen cada PeriodMs
            public bool IsPeriodic { get; set; } = false;
            public bool IsDelayed { get; set; } = false;
            public int PeriodMs { get; set; } = 0;
            public DateTime LastExecution { get; set; } = DateTime.MinValue;

            public OrderInfo(ServiceInfo info)
            {
                Type = OrderType.Service;
                Service = info;
            }

            public OrderInfo()
            {
                Type = OrderType.Service;
            }
        }
        #endregion

       #region Parametros de control de la thread de procesos

        const int DefTimeSend = 4000;        // Tiempo entre envios por defecto        (1 seg)  
        const int DefTimeInit = 500;         // Retardo para enviar documentos nuevos  (0.5 sg)
        const int DefTimeAction = 500;         // Retardo despues de un action           (0.5 sg)
        const int DefTimeCreated = 800;         // Retardo para envio recien creado       (0.8 sg)
        const int DefTimeStart = 4000;        // Retardo para arranque de la aplicacion (1 seg)
        const int DefTimeError = 60000;       // Retardo despues de error de recepcion  (1 min)
        const int DefTimeExit = 2000;        // Retardo despues de error de recepcion  (1 min)

        private OrderTable<OrderInfo> AppList;  // Tabla de ordenes

        private bool IsActive = false;          // Proceso de ordenes activo 
        private bool IsSync = false;          // Proceso de ordenes sincrono
        private bool IsDelay = false;          // Orden para crear un retraso 
        private bool IsRunApp = false;          // Proceso de servicios arrancado

        private Thread AppThread;               // Tread de procesos
        private AutoResetEvent AppSync;         // Sincronizacion de procesos

        private int TimeSend;                   // Tiempo de espera entre envios
        private int TimeInit;                   // Retardo para enviar documentos nuevos
        private int TimeStart;                  // Retardo para arranque de la aplicacion

        private int TimeAction;                 // Tiempo de espera a un action en curso
                                                // Debe volver a la aplicacion antes

        private int TimeCreated;                // Tiempo de espera para alta en memoria
                                                // Debe procesar el retorno en powergest
                                                // Se aplica para envio inmeidato de altas

        private int TimeExit;                   // Tiempo de espera de salida aplicacion
        private int TimeError;                  // Retardo cuando se produce un error
        private bool AutoStart;                 // Arranque automatico de envios
        private bool AutoCreate;                // Creacion automatica de envios
        private string LastIden;                // Ultimo identificador procesado

        #endregion

       #region GESTION DE THREADS Y PROCESOS

        public void AppStart()
        {
            if (AppList == null)
            {
                if (TimeSend == 0)
                {
                    TimeStart = DefTimeStart;
                    TimeInit = DefTimeInit;
                    TimeAction = DefTimeAction;
                    TimeCreated = DefTimeCreated;
                    TimeSend = DefTimeSend;
                    TimeError = DefTimeError;
                    TimeExit = DefTimeExit;
                }

                // IsActive = true;  // ** Cuidado
                AppList = new OrderTable<OrderInfo>();
                AppSync = new AutoResetEvent(false);

                // TimeStart = DateTime.Now.Ticks;

                // Arrancar thread en espera de inicializacion
                IsActive = false;
                IsRunApp = true;//IsRunApp = false;
                AppThread = new Thread(new ThreadStart(AppProc));
                // AppThread.Start();  // ** Cuidado
            }

            if (!IsActive && IsRunApp)
            {
                // Servicios de aplicacion incializados
                // Arrancar thread de proceso de ordenes

                IsActive = true;
                AppThread.Start();
                AppSync.Set();
            }
        }

        public void AppStop()
        {
            // Borrar ordenes pendientes de iniciarse
            if (AppList != null)
            {
                StopServices();

                AppList.Reset();

                // Salida incondicional de thread de procesos
                IsActive = false;

                if (AppSync != null)
                    AppSync.Set();

                // Esperar salida de la thread de procesos
                if (AppThread != null)
                    AppThread.Join(DefTimeExit);

                AppSync = null;
                AppThread = null;
            }
        }

        /// <summary>
        /// Añadir orden normal
        /// </summary>
        /// <param name="service"></param>
        private void AddOrder(ServiceInfo service)
        {
            var order = new OrderInfo(service);

            AppendOrder(order);
        }

        /// <summary>
        /// Añadir orden periodica
        /// </summary>
        /// <param name="service"></param>
        /// <param name="periodMs"></param>
        private void AddOrder(ServiceInfo service, int periodMs)
        {
            var order = new OrderInfo(service)
            {
                IsPeriodic = true,
                IsDelayed = false,
                PeriodMs = periodMs,
                LastExecution = DateTime.Now
            };

            //Evitar meter la misma orden CIRCULAR si ya hay una
            AppendOrder(order);
        }        
        /// <summary>
        /// Añadir orden con retraso (que se ejecute una vez a los X segundos)
        /// </summary>
        /// <param name="service"></param>
        /// <param name="periodMs"></param>
        /// <param name="isDelayed"></param>
        private void AddOrder(ServiceInfo service, int periodMs, bool isDelayed)
        {
            var order = new OrderInfo(service)
            {
                IsPeriodic = false,
                IsDelayed = true,
                PeriodMs = periodMs,
                LastExecution = DateTime.Now
            };

            AppendOrder(order);
        }


        private void AppendOrder(OrderInfo order)
        {
            if (AppList == null)
            {
                // Iniciar proceso de ordenes de aplicacion
                AppStart();
            }

            if (AppList != null && order != null)
            {
                // Añadir orden al proceso de ejecucion
                AppList.Add(order);

                // Se da orden de reinicio de la ejecucion
                if (IsActive && IsRunApp)
                {
                    AppSync.Set();
                }
            }
        }

        #endregion

       #region EJECUCIÓN DE SERVICIO

        private DateTime LastSend;

        /// <summary> Ejecucion de la thread de procesos
        /// </summary>

        private void AppProc()
        {
            int wait = 0;

            while (IsActive)
            {
                AppSync.WaitOne(TimeSend);

                if (IsActive)
                {
                    if (LastSend == DateTime.MinValue)
                    {
                        LastSend = DateTime.Now;
                        Thread.Sleep(TimeStart); // Espera inicial de la aplicacion

                        continue;
                    }

                    try
                    {
                        bool lValid = true;  // Continuar proceso 

                        while (AppList.Count > 0)
                        {
                            lock (AppList)
                            {
                                if (IsSync || IsDelay)
                                {
                                    wait = TimeAction; // Espera tras el action
                                    IsDelay = false;
                                    lValid = !IsSync;
                                }
                                else
                                {
                                    if (wait != 0)
                                    {
                                        Thread.Sleep(wait); // Espera al final de action
                                        wait = 0;
                                    }

                                    OrderInfo order = AppList.CircularPeek();

                                    if (order.Type == OrderType.Service)
                                    {
                                        if (order.IsPeriodic) //Compruebo si ha pasado el tiempo determinado desde la ultima ejecucion, pero nunca la saco
                                        {
                                            var now = DateTime.Now;
                                            var elapsed = (now - order.LastExecution).TotalMilliseconds;

                                            if (elapsed >= order.PeriodMs)
                                            {
                                                StartService(order.Service);
                                                order.LastExecution = now;
                                            }

                                            //AppendOrder(order);
                                        }
                                        else if (order.IsDelayed) //Compruebo si ha pasado el tiempo para ejecutarse, y la saco
                                        {
                                            var now = DateTime.Now;
                                            var elapsed = (now - order.LastExecution).TotalMilliseconds;

                                            if (elapsed >= order.PeriodMs)
                                            {
                                                AppList.CircularRemove();
                                                StartService(order.Service);
                                            }
                                        }
                                        else //Es una normal, la ejecuto y la saco
                                        {
                                            AppList.CircularRemove();
                                            // Orden normal (no periódica)
                                            StartService(order.Service);
                                        }
                                    }
                                }
                            }
                        }

                        // Envio del siguiente en la tabla
                        if (lValid && !IsSync)
                        {
                            if (wait != 0)
                            {
                                Thread.Sleep(TimeInit); // Espera al final de action
                                wait = 0;
                            }

                            // Buscar siguiente envio pendiente 
                            try
                            {
                                //SendNext();
                            }
                            catch (Exception exc)
                            {

                            }

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            AppThread = null;
        }
        #endregion 
    }
}