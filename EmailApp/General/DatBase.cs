using MailAppMAUI.General;

namespace MailAppMAUI.General
{
    public class DatBase
    {
        #region EVENTOS Y HANDLERS

        // Declaracion del evento a nivel de datos
        public event DatEventHandler DatEvent;

        // Declaracion del manejador para el evento
        public delegate void DatEventHandler(OpEvent oArg);

        /// <summary> Comprueba si un delegado esta ya subscrito al evento
        /// Esto permite evitar invocacion multiple de un mismo delegado
        /// </summary>
        /// <param name="deleg"> Delegado a comprobar </param>
        /// <returns> Indica si el delegado esta ya subscrito </returns>

        public bool ExistDelegate(DatEventHandler deleg)
        {
            foreach (DatEventHandler del in DatEvent.GetInvocationList())
            {
                if (del.Equals(deleg))
                    return true;
            }
            return false;
        }

        /// <summary> Comprueba si una clase esta ya subscrita al evento
        /// Esto permite evitar invocacion multiple de un mismo delegado
        /// </summary>
        /// <param name="target"> Instancia de la clase a comprobar </param>
        /// <returns> Indica si el delegado esta ya subscrito </returns>

        public bool ExistDelegate(object target)
        {
            foreach (DatEventHandler del in DatEvent.GetInvocationList())
            {
                if (del.Target == target)
                    return true;
            }
            return false;
        }

        /// <summary> Emision de evento con codigo e identificador
        /// </summary>
        /// <param name="Code"> Codigo del mensaje a generar </param>

        protected virtual bool OnEvent(Enum Code, params object[] Info)
        {
            // int nCode = Convert.ToInt32(Code);
            int nCode = Data.ToInt((object)Code);

            OpEvent oArg = new OpEvent(nCode, AppData.Tables.GetInfo(nCode), Info);

            return OnEvent(oArg);

        }

        protected virtual bool OnEvent(string Code, params object[] Info)
        {
            OpEvent oArg = new OpEvent(Code, Info);
            return OnEvent(oArg);
        }

        /// <summary> Emision de evento global a nivel aplicacion 
        /// Se puede utiliza para eventos desde metodos estaticos
        /// </summary>
        /// <typeparam name="T"> Tipo de eventos      </typeparam>
        /// <param name="code">  Codigo del evento    </param>
        /// <param name="info">  Parametros asociados </param>
        /// <returns> Resultado del evento </returns>

        public static bool AppEvent<T>(T code, params object[] info) where T : struct
        {
            LoadEvents(typeof(T));

            // int nCode = Convert.ToInt32(code);
            int nCode = Data.ToInt((object)code);

            OpEvent oArg = new OpEvent(nCode, AppData.Tables.GetInfo(nCode), info);
            return AppData.Tables.OnEvent(oArg);
        }

        /// <summary> Emision de evento a partir de un mensaje
        /// </summary>
        /// <param name="oArg"> Mensaje completo del evento </param>

        protected virtual bool OnEvent(OpEvent oArg)
        {
            bool resul = true;

            // Registrar evento salvo que provenga de un error
            if (!oArg.IsLogged)
                Logger.LogError(oArg.Error);

            //// Salvar logger en memoria en caso de error
            //if (oArg.IsError)
            //    Logger.LogSave();

            // Emitir el evento a la capa cliente
            if (!oArg.IsProcess && oArg.ToProcess)
            {
                if (DatEvent != null)
                {
                    DatEvent(oArg);
                    resul = oArg.Resul > OpResul.Cancel;
                }
                else
                {
                    AppData.Tables.OnEvent(oArg);
                }
            }
            return resul;
        }

        /// <summary> Metodo de proceso inicial de excepciones 
        /// </summary>
        /// <param name="oExc">  Parametros de la excepcion    </param>
        /// <param name="Code">  Codigo de error de aplicacion </param>
        /// <param name="info"> Cadena con informacion extra  </param>

        protected virtual bool OnError(Exception oExc, string Code, string info)
        {
            // Registrar error y evento asociado
            Logger.LogError(oExc, info);

            // Generar un evento con informacion de error
            OpEvent oArg = new OpEvent(Code, oExc, info);
            return OnEvent(oArg);
        }

        // Controlador de los eventos de otras clases conectadas

        public void DatHandler(OpEvent oArg)
        {
            OnEvent(oArg);
        }

        public static void LoadEvents(Type typ)
        {
            // AppProp.LoadInfo(typ);
            AppData.Tables.LoadInfo(typ);
        }

        #endregion
    }
}
