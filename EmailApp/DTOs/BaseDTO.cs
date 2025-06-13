using MailAppMAUI.General;
using System.ComponentModel.DataAnnotations;

namespace MailAppMAUI.DTOs
{
    /// <summary>
    /// Clase base para todas las clases entidades de la base de datos.
    /// </summary>
    public class BaseDTO
    {
        /// <summary>
        /// Enumerado del tipo de la entidad
        /// </summary>
        [Required]
        public TipoEntidad Tipo { get; set; }

        public BaseDTO() { }

        /// <summary>
        /// Convierte los datos recibidos en una entidad BaseDTO
        /// </summary>
        /// <param name="filas">Datos de las propiedades</param>
        /// <param name="columnas">Propiedades del objeto</param>
        /// <returns>Objeto BaseDTO convertido</returns>
        public virtual BaseDTO ImportData(object[] filas, string[] columnas)
        {
            return null;
        }

        /// <summary>
        /// Comprueba si el campo a comprobar esta definido en la entidad y devuelve el valor
        /// </summary>
        /// <param name="propertyName">Nombre del campo</param>
        /// <param name="value">Valor de la propiedad convertido a string</param>
        /// <returns>True si existe el campo y false en campo contrario</returns>
        public virtual bool GetValue(string propertyName, out string value)
        {
            value = null;
            switch (propertyName)
            {
                case nameof(Tipo):
                    value = ((int)Tipo).ToString();
                    break;

                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Realiza una copia de los campos de una entidad a otra
        /// </summary>
        /// <param name="dto">Entidad de la que copiar los datos.</param>
        public virtual void CopyFrom(BaseDTO dto)
        {
            Tipo = dto.Tipo;
        }


        public static implicit operator TipoEntidad(BaseDTO bDTO)
        {
            return bDTO.Tipo;
        }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
