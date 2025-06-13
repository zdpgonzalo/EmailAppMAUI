using MailAppMAUI.General;
using MailAppMAUI.UseCases;
using MailAppMAUI.Core;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Threading.Tasks;

namespace MailAppMAUI.Controladores
{
    public partial class VModelBase
    {
        public VModelBase(IAction gesCorreos)
        {
            DataRefer = gesCorreos;
        }

        internal IAction DataRefer { get; set; }

        /// <summary>
        /// Llama al método set de la capa de gestión
        /// </summary>
        public virtual bool SetData(string name, object value, object item = null)
        {
            bool resul = false;
            DataResul dataResul = new();

            try
            {
                if (DataRefer == null) return false;

                resul = DataRefer.SetData(name, item, value);

                dataResul = DataRefer.GetDataResul();

                object[] values = [item, value];

                UpdateModel(dataResul.OpResul);

                //ShowModal(dataResul.WindowType, false, values);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

            return resul;
        }

        /// <summary>
        /// Llama al método get de la capa de gestión
        /// </summary>
        public virtual object GetData(string name, object item = null)
        {
            try
            {
                if (DataRefer == null) return false;

                return DataRefer.GetData(name, item);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        private bool inRefresh = false;

        /// <summary>
        /// Llama al ejecutar una acción en la capa de gestión (versión async)
        /// </summary>
        public virtual async Task<object> Action(string oper, object value, params object[] info)
        {
            if (inRefresh)
            {
                return false;
            }

            inRefresh = true;

            object resul = null;

            try
            {
                object[] values = [.. (new object[] { value }), .. info];

                if (DataRefer != null)
                {
                    resul = await DataRefer.Action(oper, values);

                    if (resul != null)
                    {
                        DataResul dataResul = DataRefer.GetDataResul();
                        UpdateModel(dataResul.OpResul);
                        //ShowModal(dataResul.WindowType, false, values);
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.LogError(exc);
            }
            finally
            {
                inRefresh = false;
            }

            return resul;
        }

        /// <summary>
        /// Actualiza los datos del ViewModel desde la clase de gestión
        /// </summary>
        public virtual void UpdateModel(OpResul dataResul) { }
    }
}
