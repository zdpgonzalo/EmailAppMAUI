using MailAppMAUI.General;

namespace MailAppMAUI.Core
{
    /// <summary>
    /// Estructurado que define el tipo de nivel de cambio que ha habido,
    /// y el si hay una accion de abrir una ventana de aviso, busqueda, etc
    /// </summary>
    /// <param name="opResul">Nivel de cambio a actualizar</param>
    /// <param name="windowType">Ventana resultado a mostrar si existe</param>
    public struct DataResul(OpResul opResul, WindowType windowType)
    {
        public OpResul OpResul { get; private set; } = opResul;
        public WindowType WindowType { get; private set; } = windowType;
    }

    public class AppChanges
    {
        private static DataResul DataResul = new DataResul(OpResul.Cancel, WindowType.None);

        /// <summary>
        /// Devuelve el OpResul del OpResul
        /// </summary>
        public static OpResul OpResul
        {
            get => DataResul.OpResul;
        }

        /// <summary>
        /// Devuelve el DataResul actual
        /// </summary>
        /// <returns></returns>
        public static DataResul GetDataResul()
        {
            return DataResul;
        }

        /// <summary>
        /// Actualiza el nivel de actualizacion de la UI y establece una ventana resultado 
        /// si interesa, en caso contrario la establece a None
        /// </summary>
        /// <param name="opResul">Nivel de actualizacion de la UI</param>
        /// <param name="windowType">Pantalla resultado a mostrar</param>
        public static void SetChanges(OpResul opResul, WindowType windowType = WindowType.None)
        {
            // Valores iguales
            if (opResul == DataResul.OpResul && windowType == DataResul.WindowType)
            {
                return;
            }

            if (opResul == OpResul.Cancel || opResul > DataResul.OpResul)
            {
                if (DataResul.WindowType != WindowType.None)
                {
                    windowType = DataResul.WindowType;
                }
                DataResul = new(opResul, windowType);
            }
        }

        /// <summary>
        /// Resetea el nivel de actualizacion al nivel mas bajo
        /// </summary>
        public static void ResetChanges()
        {
            DataResul = new(OpResul.Cancel, WindowType.None);
        }
    }
}
