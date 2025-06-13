using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAppMAUI.UseCases.Services
{
    public interface IService
    {
        /// <summary> Arranque de un servicio de la aplicacion
        /// Devuelve la instancia creada del servicio 
        /// </summary>
        /// <param name="service"> Servicio para abrir </param>
        /// <returns> Arranque correcto </returns>
        IService OpenService(string service);

        /// <summary> Parada de un servicio de la aplicacion
        /// </summary>
        /// <returns> Parada correcta </returns>

        bool CloseService();

        /// <summary> Ejecucion de un servicio generico de aplicacion
        /// <param name="action"> Nombre del servicio a ejecutar </param>
        /// <param name="info"> Datos para ejecutar el servicio </param>
        /// </summary>

        Task<object> Execute(object action, object[] info);
    }
}
