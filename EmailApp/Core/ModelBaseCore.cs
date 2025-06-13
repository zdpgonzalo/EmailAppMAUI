using MailAppMAUI.Contexto;
using MailAppMAUI.General;
using MailAppMAUI.DTOs;
using Microsoft.EntityFrameworkCore;
using MailAppMAUI.ContextProvider;

namespace MailAppMAUI.Core
{
    /// <summary> Clase base para todas las clases de soporte
    /// - Propiedades comunes: Acceso a datos (DbContext), servicios, ..
    /// - Eventos generados a niveles superiores
    /// </summary>
    public class ModelBaseCore<T> : IComparable<ModelBaseCore<T>> where T : BaseDTO, new()
    {
        /// <summary>
        /// Proveedor de contexto actual (inyectado desde fuera)
        /// </summary>
        public static IDbContextProvider? DbProvider { get; set; }

        /// <summary>
        /// Objeto DTO asociado al Core
        /// </summary>
        protected T DTO_Base { get; set; }

        public ModelBaseCore()
        {
            //DTO_Base = new T();
        }

        /// <summary>
        /// Comprueba si el campo a comprobar está definido en la entidad y devuelve el valor
        /// </summary>
        public virtual bool GetValue(string propertyName, out string value)
        {
            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        public virtual bool Save()
        {
            var context = DbProvider?.GetContext();
            if (context != null)
            {
                context.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Establece el nivel de actualización de los cambios aplicados
        /// </summary>
        public void SetChanges(OpResul opResul)
        {
            SetChanges(opResul, WindowType.None);
        }

        /// <summary>
        /// Establece el nivel de actualización de los cambios aplicados y pantalla opcional
        /// </summary>
        public void SetChanges(OpResul opResul, WindowType windowType)
        {
            AppChanges.SetChanges(opResul, windowType);

            if (opResul != OpResul.Cancel)
            {
                var context = DbProvider?.GetContext();
                if (context != null)
                {
                    context.Entry(this).State = EntityState.Modified;
                }
            }
        }

        /// <summary>
        /// Método ToString del objeto Core
        /// </summary>
        public virtual string ToString()
        {
            return string.Empty;
        }

        public virtual int CompareTo(ModelBaseCore<T>? other)
        {
            return 0;
        }
    }
}
