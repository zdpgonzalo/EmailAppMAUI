using System;
using System.Collections;
using System.Text;

namespace MailAppMAUI.General
{
    /// <summary> Normalizacion de identificadores de la aplicacion
    /// </summary>

    public class AppNorm
    {
        /// <summary> Genera el identificador standard de un campo
        /// 
        /// El identificador esta formado por la tabla y nombre del objeto
        /// Los nombres reales en el origen de datos pueden ser distintos
        /// Cada parte se separa de la otra mediante un guion bajo o un punto
        /// 
        /// </summary>
        /// <param name="cField"> Nombre simple del campo del identificador </param>
        /// <param name="cTable"> Nombre de la tabla del identificador      </param>
        /// <returns> Expresion normalizada del identificador </returns>
        /// 
        /// <remarks>
        /// Se crean las dos partes del identificador aunque alguna este vacia 
        /// Esto permite separar posteriormente cada una de las partes sin error
        /// </remarks>

        public static string GetIden(string cName, string cTable)
        {
            if (cName != null && cTable != null)
            {
                cName = cTable + '_' + cName;
            }
            else
            {
                if (cName != null)
                    cName = '_' + cName;
                else
                    cName = cTable + '_';
            }
            return cName;
        }

        public static string GetIden(object name, object table)
        {
            return GetIden(name.ToString(), table.ToString());
        }

        /// <summary> Normnaliza y completa identificador con tabla por defecto 
        /// Quita la informacion de la fila si existe en el identificador dado
        /// Devuelve el mismo identificador sin modificar si ya esta normalizado
        /// </summary>
        /// <param name="cField"> Nombre o expresion completa del identificador  </param>
        /// <param name="cTable"> Tabla por defecto si el identificador no lleva </param>
        /// <returns> Expresion normalizada del identificador </returns>
        /// <remarks>
        /// Un identificador normalizado lleva tabla y nombre separados por guion bajo
        /// Los identificadores de interfaz pueden llevar otra parte que indica indice
        /// Esta parte se separa tambien por guion bajo y debe ser siempre la ultima
        /// Al generar un mensaje con el identificador se normaliza y quita el indice
        /// El identificador debe buscarse siempre en las tablas en forma normalizada
        /// </remarks>

        public static string NormIden(string cName, string cTable)
        {
            if (cName != null)
            {
                // Quitar informacion adicional del identificador
                int nIndex1 = cName.IndexOf('_');

                if (nIndex1 >= 0)
                {
                    // Eliminar tercera parte del identificador
                    int nIndex2 = cName.IndexOf('_', nIndex1 + 1);

                    if (nIndex2 > 0)
                        cName = cName[..nIndex2];
                }
                else
                {
                    // Completar nombre de la tabla sin definir
                    if (!string.IsNullOrEmpty(cTable))
                        cName = cTable + '_' + cName;
                }

                // Estandarizar inicio del cada parte del identificador
                cName = Normalize(cName);
            }
            return cName;
        }

        /// <summary> Devuelve el nombre base de un identificador
        /// </summary>
        /// <param name="cIden"> Expresion completa del identificador </param>
        /// <returns> Nombre base contenido en el identificador </returns>

        public static string GetName(string cIden)
        {
            string cName = null;

            if (cIden != null)
            {
                int nIndex = cIden.IndexOf('_');

                if (nIndex >= 0)
                    cName = cIden[(nIndex + 1)..];
                else
                    cName = cIden;
            }

            return cName;
        }

        /// <summary> Devuelve el nombre de la tabla de un identificador
        /// </summary>
        /// <param name="cIden"> Expresion completa del identificador  </param>
        /// <returns> Nombre de la tabla contenida en el identificador </returns>

        public static string GetTable(string cIden)
        {
            string cTable = null;

            if (cIden != null)
            {
                int nIndex = cIden.IndexOf('_');

                if (nIndex >= 0)
                    cTable = cIden[..nIndex];
                else
                    cTable = string.Empty;
            }

            return cTable;
        }

        /// <summary> Retorna informacion adiccional del identificador
        /// </summary>
        /// <param name="cIden"> Expresion del identificador extendido </param>
        /// <returns> Informacion adicional del identificador </returns>
        /// <remarks>
        /// Un identificador normalizado lleva tabla y nombre separados por guion bajo
        /// Los identificadores de interfaz pueden llevar otra parte de informacion
        /// Esta parte se separa tambien por guion bajo y debe ser siempre la ultima
        /// Se puede usar para contener el indice o bien otra informacion adicional
        /// Al generar un mensaje con el identificador se normaliza y quita el resto
        /// El identificador debe buscarse siempre en las tablas en forma normalizada
        /// </remarks>


        public static string GetInfo(string cIden)
        {
            return GetInfo(cIden, out _);
        }

        public static string GetInfo(string cIden, out string cBase)
        {
            cBase = null;

            if (cIden != null)
            {
                int nIndex1 = cIden.IndexOf('_');

                if (nIndex1 >= 0)
                {
                    int nIndex2 = cIden.IndexOf('_', nIndex1 + 1);

                    if (nIndex2 > 0)
                    {
                        cBase = cIden[..nIndex2];
                        return cIden[(nIndex2 + 1)..];
                    }
                }
            }

            return null;
        }

        /// <summary> Modifica el nombre de un identificador completo
        /// </summary>
        /// <param name="cIden"> Expresion completa del identificador    </param>
        /// <param name="cName"> Nombre a substituir en el identificador </param>
        /// <returns> Identificador modificado </returns>

        public static string SetName(string cIden, string cName)
        {
            int nIndex = cIden.IndexOf('_');

            if (nIndex >= 0)
                cIden = cIden[..nIndex] + '_' + cName;
            else
                cIden += '_' + cName;

            return cIden;
        }

        /// <summary> Modifica la tabla de un identificador completo
        /// </summary>
        /// <param name="cIden"> Expresion completa del identificador   </param>
        /// <param name="cName"> Tabla a substituir en el identificador </param>
        /// <returns> Identificador modificado </returns>

        public static string SetTable(string cIden, string cTable)
        {
            int nIndex = cIden.IndexOf('_');

            if (nIndex >= 0)
                cIden = cTable + '_' + cIden[..(nIndex + 1)];
            else
                cIden = cTable + '_' + cIden;

            return cIden;
        }

        /// <summary> Normaliza y completa expresion de acceso a un campo 
        /// La expresion esta formada por tabla y nombre separados por un punto
        /// 
        /// </summary>
        /// <param name="cField"> Nombre simple o expresion completa del campo  </param>
        /// <param name="cTable"> Tabla por defecto para completar la expresion </param>
        /// Una cadena vacia elimina la Tabla de la expresion y deja el nombre
        /// <returns> Expresion normalizada de acceso al campo </returns>

        public static string GetField(string cField, string cTable)
        {
            int nIndex;

            if (cField != null)
            {
                // Buscar indices de cada componente 
                if ((nIndex = cField.IndexOf('.')) > 0)
                {
                    // Quitar nombre de tabla si se pasa en blanco
                    if (cTable == "")
                        cField = cField[(nIndex + 1)..];
                }
                else
                {
                    // Añadir tabla a nombre de campo simple
                    if (cTable != null && cTable != "")
                        cField = cTable + '.' + cField;
                }
            }
            return cField;
        }

        /// <summary> Retorna la parte de tabla de un acceso a campo
        /// Si es un nombre simple se asume que es nombre de campo
        /// Por tanto al no llevar tabla retorna una cadena vacia
        /// </summary>
        /// <param name="cField"> Expresion del campo a comprobar </param>
        /// <returns> Nombre de la tabla si existe  </returns>

        public static string FieldTable(string cField)
        {
            int nIndex = cField.IndexOf('.');

            if (nIndex >= 0)
                return cField[..nIndex];

            return "";
        }

        /// <summary> Retorna nombre simple del campo de una expresion
        /// Si la expresion es un nombre simple se asume que es un campo
        /// Por tanto al no llevar tabla retorna la msima cadena pasada
        /// </summary>
        /// <param name="cField"> Expresion del campo a comprobar </param>
        /// <returns> Nombre simple del campo </returns>

        public static string FieldName(string cField)
        {
            int nIndex = cField.IndexOf('.');

            if (nIndex >= 0)
                cField = cField[(nIndex + 1)..];

            return cField;
        }

        /// <summary> Normaliza un identificador generico de un objeto
        /// El identificador tiene dos partes separadas por guion o punto
        /// El proceso de normalizacion deja la primera letra de cada parte
        /// en mayusculas, el resto en minusculas y el guion como separador
        /// </summary>
        /// <param name="cIden"> Identificador a estandarizar </param>
        /// <returns> Identificadro normalizado </returns>
        /// <remarks> 
        /// Solo se comprueban los dos primeros caracteres de cada parte 
        /// El primero debe ser siempre mayusculas y el segundo minusculas
        /// 
        /// Esto permite que cada parte se pueda formar por varias palabras
        /// Cada una empezara por mayusculas y tendra el resto minusculas 
        /// Las palabras anexas sin separador forman parte del mismo nombre
        /// </remarks>

        public static string Normalize(string cIden)
        {
            int nIndex;

            if (!string.IsNullOrEmpty(cIden) && !char.IsDigit(cIden, 0))
            {
                if ((nIndex = cIden.IndexOf('.')) < 0)
                    nIndex = cIden.IndexOf('_');

                if (nIndex >= 0 && nIndex < cIden.Length - 2)
                {
                    if (char.IsLower(cIden[nIndex + 1]) ||
                        char.IsUpper(cIden[nIndex + 2]) || cIden[nIndex] == '.')
                    {
                        cIden = char.ToUpper(cIden[0]) +
                                cIden[1..nIndex].ToLower() + "_" +
                                char.ToUpper(cIden[nIndex + 1]) +
                                cIden[(nIndex + 2)..].ToLower();
                    }
                }

                if (char.IsLower(cIden[0]) ||
                    cIden.Length > 1 && char.IsUpper(cIden[1]))
                {
                    cIden = char.ToUpper(cIden[0]) + cIden[1..].ToLower();
                }
            }

            return cIden;
        }

        /// <summary> Retorna numero de orden de un identificador 
        /// Se usa en identificadores que usan nombre correlativo
        /// </summary>
        /// <param name="cIden"> Identificador a comprobar </param>
        /// <returns> Numero de orden del identificador </returns>

        public static int GetOrder(string cIden)
        {
            int nResul = 0;

            for (int nIndex = 0; nIndex < cIden.Length; nIndex++)
            {
                if (char.IsDigit(cIden, nIndex))
                {
                    nResul = int.Parse(cIden[nIndex..]);
                }
            }
            return nResul;
        }

        /// <summary> Comprueba si la cadena contiene una expresion
        /// Se detecta por el uso de parentesis, igualdad y operadores
        /// </summary>
        /// <param name="cExpres"> Cadena a analizar </param>
        /// <returns> Indica que es una expresion   </returns>

        public static bool IsExpres(string cExpres)
        {
            return cExpres.IndexOfAny(ListOper) >= 0;
        }

        private static readonly char[] ListOper = ['+', '-', '/', '*', '(', ')','[',']',
                                                    '<', '>', '!', '=', '|', '&'];

        /// <summary> Comprueba si la cadena es un identificador valido
        /// La cadena debe contener solo letras, numeros o guion bajo
        /// No se admiten ningun otro simbolo ni caracteres en blanco
        /// </summary>
        /// <param name="cExpres"> Cadena a analizar </param>
        /// <returns> Indica que es un identificador </returns>

        public static bool IsName(string cExpres)
        {
            foreach (char cCar in cExpres)
            {
                if (!char.IsLetterOrDigit(cCar) && cCar != '_')
                    return false;
            }
            return true;
        }

        /// <summary> Comprueba si la cadena es una expresion con operadores
        /// Es un nombre si tiene solo letras, numeros, espacios o guion bajo
        /// Cualquier otro caracter se toma como indicasdor de expresion
        /// </summary>
        /// <param name="cExpres"> Cadena a analizar </param>
        /// <returns> Indica que es una expresion </returns>

        public static bool IsEval(string cExpres)
        {
            int total = cExpres.Length;
            for (int ind = 0; ind < total; ind++)
            {
                char cCar = cExpres[ind];
                if (!char.IsLetterOrDigit(cCar) && cCar != '_')
                {
                    if (!char.IsWhiteSpace(cCar))
                        return true;
                }
            }

            if (cExpres.Contains(" and ", StringComparison.OrdinalIgnoreCase) ||
                cExpres.Contains(" or ", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        /// <summary> Comprueba si la cadena es un identificador valido
        /// La cadena debe tener letras o numeros y el guion separador
        /// </summary>
        /// <param name="iden"> Cadena a analizar </param>
        /// <returns> Indica que es un identificador </returns>

        public static bool IsIden(string iden)
        {
            if (iden != null && iden.Length > 2 &&
                char.IsLetterOrDigit(iden[0]) && iden.IndexOf('_') > 0)
                return true;

            return false;
        }

        /// <summary> Comprueba si la cadena es campo simple o completo
        /// La cadena debe contener letras, numeros, guion bajo o punto
        /// El punto debe aparecer una sola separando la tabla y nombre
        /// </summary>
        /// <param name="cExpres"> Cadena a analizar </param>
        /// <returns> Indica que es un campo simple o completo </returns>

        public static bool IsField(string cExpres)
        {
            bool IsDot = false;

            foreach (char cCar in cExpres)
            {
                if (!char.IsLetterOrDigit(cCar) && cCar != '_')
                {
                    if (cCar == '.' && !IsDot)
                        IsDot = true;
                    else
                        return false;
                }
            }
            return true;
        }

    }
}
