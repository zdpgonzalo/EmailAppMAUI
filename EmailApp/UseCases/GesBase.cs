using MailAppMAUI.Core;
using MailAppMAUI.General;

namespace MailAppMAUI.UseCases
{
    /// <summary> 
    /// Tipos y metodos de soporte para clases de gestion externas
    /// </summary>
    public class GesBase<TGest, TEvent, TAction, TName, TTable> : DatBase, IAction
                 where TEvent : Enum where TAction : Enum
                 where TName : Enum where TTable : Enum
    {
        protected ICollection<IAction> SupportClasses { get; set; }
        protected IAction Parent { get; set; }

        protected static bool Exists { get; private set; } = false;

        public GesBase(IAction parent)
        {
            Parent = parent;
            SupportClasses = [];
            Exists = true;
        }

        public GesBase()
        {
            SupportClasses = [];
            Exists = true;
        }

        #region Interfaz IAction para enlace de datos

        /// <summary>
        /// Descompone el ident en una instruccion Accion-Tabla y 
        /// realiza la accion indicada
        /// </summary>
        /// <param name="ident">Nombre de la intruccion Accion-Tabla</param>
        /// <param name="info">Informacion extra</param>
        /// <returns>Resultado de la operacion ejecutada</returns>
        public Task<object> Action(string ident, params object[] info)
        {
            ResetChanges();

            if (ident != null)
            {
                string cTable = Data.GetTable(ident);
                string cOper = Data.GetName(ident);

                if (Data.IsDefined<TTable>(cTable) &&
                    Data.IsDefined<TAction>(cOper))
                {
                    var action = Data.GetEnum<TAction>(cOper);
                    var table = Data.GetEnum<TTable>(cTable);

                    return Action(action, table, info);
                }
            }

            return null;
        }

        /// <summary>
        /// Realiza un Set de una propiedad o valor de un item indicado
        /// </summary>
        /// <param name="ident">Instruccion Nombre-Tabla a cambiar</param>
        /// <param name="item">Objeto a modificar el valor</param>
        /// <param name="value">Valor nuevo de la propiedad</param>
        /// <returns>True si realiza el cambio. False en caso contrario</returns>
        public bool SetData(string ident, object item, object value)
        {
            ResetChanges();
            bool result = false;

            if (ident != null)
            {
                string cTable = Data.GetTable(ident);
                string cName = Data.GetName(ident);

                if (Data.IsDefined<TTable>(cTable) &&
                    Data.IsDefined<TName>(cName))
                {
                    var name = Data.GetEnum<TName>(cName);
                    var table = Data.GetEnum<TTable>(cTable);

                    result = SetData(name, table, item, value);
                }
            }

            return result;
        }

        /// <summary>
        /// Realiza una intruccion get de la capa de dominio
        /// </summary>
        /// <param name="ident">Instruccion Nombre-Tabla a realizar</param>
        /// <param name="item">Item a procesar</param>
        /// <returns>Valor de la propiedad o campo</returns>
        public object GetData(string ident, object item)
        {
            SetChanges(OpResul.Cancel);
            object value = null;

            if (ident != null)
            {
                string cTable = Data.GetTable(ident);
                string cName = Data.GetName(ident);

                if (Data.IsDefined<TTable>(cTable) &&
                    Data.IsDefined<TName>(cName))
                {
                    var name = Data.GetEnum<TName>(cName);
                    var table = Data.GetEnum<TTable>(cTable);

                    value = GetData(name, table, item);
                }
            }

            return value;
        }

        /// <summary>
        /// Cambia el nivel de actualizacion de la aplicacion
        /// </summary>
        /// <param name="opResul">Nivel de actualizacion</param>
        /// <param name="windowType">Pantalla de aviso</param>
        public static void SetChanges(OpResul opResul, WindowType windowType = WindowType.None)
        {
            AppChanges.SetChanges(opResul, windowType);
        }

        /// <summary>
        /// Resetea el nivel de actualizacion al nivel mas bajo
        /// </summary>
        public void ResetChanges()
        {
            AppChanges.ResetChanges();
        }

        /// <summary>
        /// Devuelve el nivel de actualizacion
        /// </summary>
        /// <returns></returns>
        public DataResul GetDataResul()
        {
            return AppChanges.GetDataResul();
        }
        #endregion

        #region Metodos sobrecargables para enlace a datos

        protected virtual Task<object> Action(TAction oper, TTable table, params object[] info)
        {
            return Task.FromResult<object>(null);
        }

        protected virtual object GetData(TName name, TTable table, object item)
        {
            return null;
        }

        protected virtual bool SetData(TName name, TTable table, object item, object value)
        {
            return false;
        }

        //protected virtual object OnEvent(TEvent @event, params object[] info)
        //{
        //    return null;
        //}

        protected virtual void OnChildEvent(OpEvent oArg)
        {
            return;
        }

        public new object OnEvent(string @event, params object[] info)
        {
            if (@event != null)
            {
                if (Data.IsDefined<TEvent>(@event))
                {
                    return OnEvent(Data.GetEnum<TEvent>(@event), info);
                }
                return OnParentEvent(@event, info);
            }
            return null;
        }

        public object OnParentEvent(string @event, params object[] info)
        {
            return Parent?.OnEvent(@event, info);
        }

        #endregion

    }


}
