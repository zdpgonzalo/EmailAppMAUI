using System.Collections;
using System.Text;

namespace MailAppMAUI.General
{
    #region METODOS DE SOPORTE PARA CADENAS

    /// <summary> metodos generales de soporte de cadenas
    /// </summary>

    public static class Str
    {
        enum TipSepar { nulo, punto, coma };

        static TipSepar SepDecimal = TipSepar.nulo;  // Separador que interpreta el sistema
        static readonly TipSepar SepDefecto = TipSepar.punto; // Separador que usa por defecto la aplicacion

        #region METODOS DE SOPORTE GENERAL DE CADENAS

        /// <summary> Parte una cadena en subcadenas separadas por un separador.
        /// Devuelve un array de string con las suibcadenas encontradas.
        /// Admite cualquier caracter como separador incluyendo separados blancos.
        /// Varios separadores blancos seguidos se toman como un unico separador.
        /// </summary>
        /// <param name="Line"> Cadena con el texto a analizar para buscar subcadenas.</param>
        /// <param name="Separator"> Caracter seoparador </param>

        static public string[] Split(string Line, char Separator)
        {
            char[] aSep = [Separator];
            return Split(Line, aSep, 0);
        }

        /// <summary> Parte una cadena en subcadenas separadas por uno o varios separadores.
        /// Devuelve un array de string con las suibcadenas encontradas.
        /// Admite cualquier caracter como separador incluyendo separados blancos.
        /// Varios separadores blancos seguidos se toman como un unico separador.
        /// </summary>
        /// <param name="Line"> Cadena con el texto a analizar para buscar subcadenas. </param>
        /// <param name="Separator"> Array con los caracteres separadores. </param>

        static public string[] Split(string Line, char[] Separator, int nBlank)
        {
            string[] aResul = null;
            char[] SepBlank = null;
            string cSepar = new(Separator);

            bool lBlank = cSepar.IndexOfAny([' ', '\t']) >= 0;
            int nItem;

            if (nBlank > 0)
            {
                SepBlank = new char[Separator.Length + 2];
                SepBlank[0] = ' ';
                SepBlank[1] = '\t';
                Separator.CopyTo(SepBlank, 2);
                lBlank = true;
            }
            if (Line != null)
            {
                Line = Line.Trim();

                for (int nPaso = 0; nPaso <= 1; nPaso++)
                {
                    int nPos, nInd, nFinal;

                    nItem = 0;   // Contador de elementos encontrados.
                    nPos = 0;    // Posicion de inicio de la busqueda.

                    do
                    {
                        // Buscar siguiente caracter separador.
                        if (lBlank && nItem < nBlank)
                            nInd = nFinal = Line.IndexOfAny(SepBlank, nPos);
                        else
                            nInd = nFinal = Line.IndexOfAny(Separator, nPos);

                        if (nInd >= 0)
                        {
                            // Comprobar si en los separadores se incluyen espacios.
                            if (lBlank)
                            {
                                // Descartar varios espacios separadores seguidos.
                                bool lSepar = false; // Encontrado separador no blanco.

                                while (nInd < Line.Length)
                                {
                                    char cSep = Line[nInd];

                                    if (cSep == ' ' || cSep == '\t')
                                        nInd++;
                                    else
                                    {
                                        // Si es un caracter separador descartar tambien.
                                        if (cSepar.Contains(cSep))
                                        {
                                            if (lSepar)
                                            {
                                                // Segundo separador no blanco: Otro elemento.
                                                nInd--;
                                                break;
                                            }
                                            else
                                            {
                                                nInd++;
                                                lSepar = true;
                                            }
                                        }
                                        else
                                        {
                                            // Caracter no separador: Es otro elemento.
                                            nInd--;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            // No hay mas separadores: Incluir resto del string.
                            nFinal = Line.Length;
                        }

                        if (nPaso == 1)
                        {
                            // Segundo paso: Cargar elementos en el array.
                            aResul[nItem] = Line[nPos..nFinal];
                            aResul[nItem] = aResul[nItem].Trim();
                        }
                        // Dejar indice de busqueda detras del separador.
                        nPos = nInd + 1;
                        nItem++;

                    } while (nInd >= 0);

                    if (nPaso == 0)
                    {
                        // Primer paso: Crear array de resultado.
                        if (nItem > 0)
                            aResul = new string[nItem];
                        else
                            break;
                    }
                }
            }
            return aResul;
        }

        /// <summary> Comprueba si un string esta vacio
        /// Acepta como vacio un string sin inicializar
        /// </summary>
        /// <param name="Cadena"> String a comprobar </param>
        /// <returns> true si el string esta vacio o es nulo </returns>

        static public bool Empty(string Cadena)
        {
            if (Cadena == null || Cadena.Length == 0)
                return true;

            int nTotal = Cadena.Length;

            for (int nIndex = 0; nIndex < nTotal; nIndex++)
            {
                if (!char.IsWhiteSpace(Cadena[nIndex]))
                    return false;
            }
            return true;
        }

        /// <summary> Devuelve indice del primer caracter no blanco
        /// </summary>
        /// <param name="Cadena"> Cadena a comprobar </param>
        /// <param name="index">  Indoce de inicio   </param>
        /// <returns> Indice del primer caracter </returns>

        static public int IndexTrim(string Cadena, int index)
        {
            int nTotal = Cadena.Length;

            for (int nIndex = index; nIndex < nTotal; nIndex++)
            {
                if (!char.IsWhiteSpace(Cadena[nIndex]))
                    return nIndex;
            }
            return 0;
        }

        static public int IndexTrim(string Cadena)
        {
            int nTotal = Cadena.Length;

            for (int nIndex = 0; nIndex < nTotal; nIndex++)
            {
                if (!char.IsWhiteSpace(Cadena[nIndex]))
                    return nIndex;
            }
            return 0;
        }

        /// <summary> Devuelve indice del primer caracter no blanco
        /// </summary>
        /// <param name="Cadena"> Cadena a comprobar </param>
        /// <returns> Indice del primer caracter </returns>

        static public int IndexTrimEnd(string Cadena)
        {
            int nTotal = Cadena.Length - 1;

            for (int nIndex = nTotal; nIndex >= 0; nIndex--)
            {
                if (!char.IsWhiteSpace(Cadena[nIndex]))
                    return nIndex;
            }
            return nTotal;
        }

        /// <summary> Trasforma una cadena a valor logico
        /// Admite los siguientes tipos de valores 
        /// - Numerico: Se considera false solo el cero
        /// - Cadena Si/No: Se trasforma al valor logico
        /// - Cadena true/false: Se trasforma al valor logico
        /// - Cadena Vacia: Se considera como resilatdo false
        /// </summary>
        /// <param name="cValor"> Cadena a con</param>
        /// <returns></returns>

        static public bool Logic(string cValor)
        {
            bool Resul = false;

            if (!Empty(cValor))
            {
                if (IsNumber(cValor))
                    Resul = Convert.ToInt32(cValor) != 0;
                else
                {
                    if (string.Compare(cValor, "si", true) == 0 ||
                         string.Compare(cValor, "true", true) == 0)
                        Resul = true;
                }
            }

            return Resul;
        }

        /// <summary> Comprueba si una cadena comienza por la segunda
        /// </summary>
        /// <param name="Text1"> Cadena contenedora a comparar </param>
        /// <param name="Text2"> Cadena buscada en la primera  </param>
        /// <returns> Indicacion de comienzo coincidente </returns>

        static public bool Starts(string Text1, string Text2)
        {
            if (Text1 == null || Text2 == null)
            {
                if (Text1 == null && Text2 == null)
                    return true;
                else
                    return false;
            }

            int index1 = IndexTrimEnd(Text1); // Cadena contenedora
            int index2 = IndexTrimEnd(Text2); // Cadena buscada

            if (index1 >= index2)
            {
                return string.Compare(Text1, 0, Text2, 0, index2 + 1, true) == 0;
            }

            // if (index1 == index2)
            // {
            //     if (index1 <= -1)
            //         return true;
            //     else
            //         return String.Compare(Text1, 0, Text2, 0, index1+1)==0;
            // }

            return false;
        }

        /// <summary> Compara dos cadenas ignorando cultura y mayusculas
        /// </summary>
        /// <param name="Text1"> Cadena primera a comparar </param>
        /// <param name="Text2"> Cadena segunda a comparar </param>
        /// <returns> Indicacion de cadenas iguales </returns>

        static public bool Equals(string Text1, string Text2)
        {
            if (Text1 == null)
                return Text2 == null;
            else
            {
                if (Text2 == null)
                    return Text1 == null;
            }

            return string.Compare(Text1, Text2,
                   StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary> Compara dos cadenas ignorando cultura y mayusculas
        /// </summary>
        /// <param name="Text1"> Cadena primera a comparar </param>
        /// <param name="Text2"> Cadena segunda a comparar </param>
        /// <param name="ignoreSpace"> Descartar espacios  </param>
        /// <returns> Indicacion de cadenas iguales </returns>

        public static bool EqualsNoSpaces(string Text1, string Text2)
        {
            if (Text1 == null)
                return Text2 == null;
            else
            {
                if (Text2 == null)
                    return Text1 == null;
            }

            return string.Compare(Text1.Trim(), Text2.Trim(),
                   StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary> Compara dos cadenas ignorando cultura y mayusculas
        /// </summary>
        /// <param name="Text1"> Cadena primera a comparar </param>
        /// <param name="Text2"> Cadena segunda a comparar </param>
        /// <param name="index1">Indice de la cadena 1     </param>
        /// <param name="index2">Indice de la cadena 2     </param>
        /// <param name="leng">  Longintud a comparar      </param>
        /// <returns> Indicacion de cadenas iguales </returns>

        static public bool Equals(string Text1, string Text2, int leng = 0, int index1 = 0, int index2 = 0)
        {
            if (Text1 == null)
                return Text2 == null;
            else
            {
                if (Text2 == null)
                    return Text1 == null;
            }

            bool resul;

            if (leng > 0 || index1 != 0 || index2 != 0)
            {
                if (index1 > Text1.Length || index2 > Text2.Length)
                    return false;

                resul = string.Compare(Text1, index1, Text2, index2, leng,
                         StringComparison.OrdinalIgnoreCase) == 0;
            }
            else
            {
                resul = string.Compare(Text1, Text2,
                         StringComparison.OrdinalIgnoreCase) == 0;
            }

            return resul;
        }

        /// <summary> Devuelve un substring a partir del indice indicado
        /// Si el indice pedido esta fuera de la longitud del
        /// string de entrada devuelve una cadena nula
        /// </summary>
        /// <param name="Line">   Cadena con el texto origen </param>
        /// <param name="nIndex"> Indice incial a retornar   </param>

        static public string Substring(string Line, int nIndex)
        {
            int nLen = Line.Length;

            if (nIndex < 0 || nIndex >= nLen)
                return "";

            return Line[nIndex..nLen];
        }

        /// <summary> Devuelve un substring a partir del indice indicado
        /// Si el indice pedido esta fuera de la longitud del
        /// string de entrada devuelve una cadena nula
        /// </summary>
        /// <param name="Line">   Cadena con el texto origen </param>
        /// <param name="nIndex"> Indice incial a retornar   </param>
        /// <param name="nCount"> Longitud maxima a retornar </param>

        static public string Substring(string Line, int nIndex, int nCount)
        {
            int nLen = Line.Length;

            if (nIndex < 0 || nIndex >= nLen || nCount < 0)
                return "";

            if (nIndex + nCount >= nLen)
                nCount = nLen - nIndex;

            return Line.Substring(nIndex, nCount);
        }

        /// <summary> Normaliza una clave dejando solo letras y digitos
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>

        static public string NormKey(string code, bool lower = true)
        {
            int len = code.Length;

            while (--len >= 0)
            {
                if (!char.IsLetterOrDigit(code[len]))
                {
                    code = Substring(code, 0, len) +
                           Substring(code, len + 1);
                }
            }

            if (lower)
                code = code.ToLower();

            return code;
        }


        /// <summary> Devuelve array con todas la lineas de un texto
        /// </summary>
        /// <param name="text"> Texto a analizar </param>
        /// <returns> Array con las lineas encontradas </returns>

        static public string[] GetLines(string text)
        {
            if (text == null)
                return null;

            int index = 0;
            string line;
            List<string> lines = [];

            try
            {
                while ((line = GetLine(text, ref index)) != null)
                {
                    lines.Add(line);
                }
            }
            catch
            {
            }

            return [.. lines];
        }

        /// <summary> Devuelve lineas sucesivas de una cadena indicada
        /// Se debe pasar el indice inicial que se devuelve actualizado 
        /// </summary>
        /// <param name="lines"> Cadena para explorar lineas </param>
        /// <param name="index"> Indice inicial de busqueda  </param>
        /// <returns> Linea encontrada o null si no hay mas  </returns>

        static public string GetLine(string lines, ref int index)
        {
            if (index >= 0 && index < lines.Length)
            {
                string line;

                int last = lines.IndexOf('\n', index);

                if (last < 0)
                {
                    line = lines[index..];
                    index = -1;
                }
                else
                {
                    int len = last - index;

                    if (last > 0 && lines[last - 1] == '\r')
                        len--;

                    line = lines.Substring(index, len);

                    // if (last > index && last == length-1)
                    //     index = last;
                    // else
                    //      index = last + 1;

                    index = last + 1;
                }
                return line;
            }

            return null;
        }


        /// <summary> Busca un elemento dado en una lista con separadores
        /// Si lo encuentra devuelve numero de elemento dentro de la lista
        /// Si el elemento pedido no esta en la lista devuelve -1
        /// </summary>
        /// <param name="Lista"> Lista de elementos con separadores </param>
        /// <param name="Campo"> Elemento a buscar en la lista      </param>
        /// <param name="Separ"> Caracter separador en la lista     </param>
        /// <returns> Numero de elemento en contrado o -1 si no existe  </returns>

        static public int Find(string Lista, string Campo, char Separ)
        {
            if (Lista != null)
            {
                int nItem = -1;
                StrScan oScan = Scan(Lista, Separ);
                foreach (string cToken in oScan)
                {
                    nItem++;
                    if (string.Compare(cToken, Campo,
                        StringComparison.InvariantCultureIgnoreCase) == 0)
                        return nItem;
                }
            }
            return -1;
        }

        /// <summary> Busca un elemento dado en una lista entre comas
		/// Si lo encuentra devuelve numero de elemento dentro de la lista
		/// Si el elemnto pedido no esta en la lista devuelve -1
        /// </summary>
        /// <param name="Lista"> Lista de elementos con separadores </param>
        /// <param name="Campo"> Elemento a buscar en la lista      </param>
        /// <returns> Numero de elemento en contrado o -1 si no existe  </returns>

        static public int Find(string Lista, string Campo)
        {
            return Find(Lista, Campo, ',');
        }

        static public int ItemCount(string lista, char separ = ',')
        {
            int count = 0;

            if (!Empty(lista))
            {
                // Recorrer y contar todos los elmentos
                int nIndex, nNext = -1;
                do
                {
                    nIndex = nNext;
                    nNext = lista.IndexOf(separ, ++nIndex);

                    if (nNext == -1)
                    {
                        if (count == 0)
                        {
                            if (nIndex == -1)
                                nIndex = 0;

                            if (!Empty(lista[nIndex..]))
                                count++;
                        }
                        else
                            count++;

                        break;
                    }
                    count++;
                }
                while (true);
            }
            return count;
        }

        /// <summary> Retorna valor de una clave en una lista doble
        /// </summary>
        /// <param name="lista"> Lista de elementos </param>
        /// <param name="key">   Clave del elemento a buscar </param>
        /// <param name="separ"> Separador de la lista </param>
        /// <returns> Valor del elemento </returns>

        static public string Item(string lista, string key, char separ = ',')
        {
            int orden = Find(lista, key);

            if (orden >= 0)
                return Item(lista, orden + 1, separ);

            return string.Empty;
        }

        /// <summary> Retorna valor de un elemento de una lista
        /// </summary>
        /// <param name="Lista"> Lista de elementos </param>
        /// <param name="Orden"> Indice del elemnto </param>
        /// <param name="Separ"> Separador de lista </param>
        /// <returns> Valor del elemento </returns>

        static public string Item(string lista, int orden, char Separ)
        {
            if (lista != null && orden >= 0)
            {
                // Buscar el numero de elemento pedido
                int nIndex, nNext = -1;
                do
                {
                    nIndex = nNext;
                    nNext = lista.IndexOf(Separ, ++nIndex);

                    if (nNext == -1)
                    {
                        if (orden > 0)
                            return null;

                        nNext = lista.Length;
                    }
                }
                while (orden-- > 0);

                // Quitar blancos al inicio del token
                while (nIndex < nNext)
                {
                    if (char.IsWhiteSpace(lista, nIndex))
                        nIndex++;
                    else
                        break;
                }

                // Quitar blancos al final del token
                while (nIndex < nNext)
                {
                    if (char.IsWhiteSpace(lista, nNext - 1))
                        nNext--;
                    else
                        break;
                }

                return lista[nIndex..nNext];
            }

            // Se cambia retorno si no existe el campo
            // Debe ser coherente si falta antes o detras
            // Comprobar efecto en coidigo existente +++
            // return null;
            return string.Empty;
        }

        /// <summary> Retorna elemento de una lista separada por comas
        /// </summary>
        /// <param name="List">  Lista de elementos </param>
        /// <param name="Index"> Indice del elemnto </param>
        /// <returns> Valor del elemento </returns>

        static public string Item(string List, int Index)
        {
            return Item(List, Index, ',');
        }

        /// <summary> Actualiza un valor en una lista con separadores
        /// </summary>
        /// <param name="Lista"> Lista de elementos  </param>
        /// <param name="Orden"> Indice del elemento </param>
        /// <param name="value"> Valor a modificar   </param>
        /// <param name="Separ"> Separador de lista  </param>
        /// <returns> Cadena modificada </returns>

        static public string SetItem(string lista, int orden, string value, char separ)
        {
            if (lista != null && orden >= 0)
            {
                // Buscar el numero de elemento pedido
                int nIndex, nNext = -1;
                do
                {
                    nIndex = nNext;
                    nNext = lista.IndexOf(separ, ++nIndex);

                    if (nNext == -1)
                        break;
                }
                while (orden-- > 0);

                // Añadir elementos intermedios si faltan
                if (orden > 0)
                {
                    lista = lista.PadRight(lista.Length + orden, separ);
                    nIndex = nNext = lista.Length;
                }
                else
                    nNext = lista.Length;

                // Insertar el nuevo elemento en su posicion
                lista = string.Concat(lista.AsSpan(0, nIndex), value, lista.AsSpan(nNext));
            }

            return lista;
        }

        /// <summary> Actualiza un valor en una lista separada por comas
        /// </summary>
        /// <param name="Lista"> Lista de elementos  </param>
        /// <param name="Orden"> Indice del elemento </param>
        /// <param name="value"> Valor a modificar   </param>
        /// <returns> Cadena modificada </returns>

        static public string SetItem(string list, int index, string value)
        {
            return SetItem(list, index, value, ',');
        }

        /// <summary> Comprueba si una cadena representa un numero.
        /// Comprueba si el primer caracter es un digito o signo.
        /// Retorna un booleano para conocer el resultado
        /// </summary>
        /// <param name="text">Cadena a comprobar</param>

        static public bool IsNumber(string text)
        {
            bool lResul = false;

            if (!string.IsNullOrEmpty(text))
            {
                if (char.IsWhiteSpace(text, 0))
                    text = text.Trim();

                char cInit = text[0];
                if (char.IsDigit(cInit) || cInit == '.' || cInit == ',' ||
                                           cInit == '+' || cInit == '-')
                {
                    int length = text.Length;

                    if (length == 1 || char.IsDigit(text[length - 1]))
                        lResul = true;
                }
            }
            return lResul;
        }

        static public bool IsDigit(string text)
        {
            if (text == null || text.Length < 1)
                return false;

            return char.IsDigit(text[0]);
        }

        static public string GetNumber(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (char.IsWhiteSpace(text, 0))
                    text = text.Trim();

                char cInit = text[0];
                if (char.IsDigit(cInit) || cInit == '.' || cInit == ',' ||
                                           cInit == '+' || cInit == '-')
                {
                    int length = text.Length;

                    if (length > 1 && !char.IsDigit(text[length - 1]))
                    {
                        for (int index = 1; index < length; index++)
                        {
                            char c = text[index];

                            if (!char.IsDigit(c) && c != '.' && c != ',')
                            {
                                text = text[..index];
                                break;
                            }
                        }
                    }
                }
            }
            return text;
        }

        /// <summary> COmprueba si uan cadena esta en un rango dado
        /// </summary>
        /// <param name="val1"> Valor inicial del rango </param>
        /// <param name="val2"> Valor final del rango </param>
        /// <returns> Indica que esta en el rango </returns>

        static public bool InRange(string value, string val1, string val2)
        {
            bool resul;

            if (val1 == null && val2 == null)
                return true;

            if (val1 == null)
            {
                resul = Equals(value, val2);
            }
            else
            {
                if (val2 == null)
                {
                    resul = Equals(value, val1);
                }
                else
                {
                    resul = string.Compare(value, val1, true) >= 0 &&
                            string.Compare(value, val2, true) <= 0;
                }
            }

            return resul;
        }


        /// <summary> Comprueba si un objeto representa un string
        /// </summary>
        /// <param name="value"> Valor a comprobar </param>
        /// <returns> Resultado de la comprobacion </returns>

        static public bool IsString(object value)
        {
            if (value != null)
            {
                if (Type.GetTypeCode(value.GetType()) == TypeCode.String)
                    return true;
            }
            return false;
        }

        /// <summary> Normaliza una cadena numerica decimal pasada por parametro.
        /// Para ello comprueba cual es el separador del sistema y si es diferente 
        /// al de la aplicacion recorre la cadena pasada por parametro y va cambiando
        /// los puntos por comas y viceversa.
        /// </summary>
        /// <param name="sNumber">Cadena numerica decimal a estandarizar</param>

        static public void NormalDec(ref string sNumber)
        {
            if (!string.IsNullOrEmpty(sNumber))
            {
                if (SepDecimal == TipSepar.nulo)
                {
                    // Averiguamos cual es el separador decimal por defecto del sistema.
                    if (Convert.ToDecimal("1.1") == 11)
                        SepDecimal = TipSepar.coma;
                    else
                        SepDecimal = TipSepar.punto;
                }

                // Si el del sistema y el de la aplicacion son diferentes
                if (SepDecimal != SepDefecto)
                {
                    StringBuilder sBuild = new(sNumber);

                    // Recorremos la cadena caracter a caracter
                    for (int nInd = 0; nInd < sBuild.Length; nInd++)
                    {
                        char cCar = sBuild[nInd];

                        // Si hay espacios los elimina.
                        if (char.IsWhiteSpace(cCar))
                            sBuild.Remove(nInd, 1);
                        else
                        {
                            // Intercambia los puntos por comas y viceversa
                            if (cCar == '.')
                                sBuild[nInd] = ',';
                            else
                                if (cCar == ',')
                                sBuild[nInd] = '.';
                        }
                    }

                    if (sBuild[0] == (SepDecimal == TipSepar.coma ? ',' : '.'))
                        sBuild.Insert(0, "0");

                    sNumber = sBuild.ToString();
                }
            }
        }

        #endregion

        #region GESTION DE LISTAS Y MARCAS CON SEPARADORES

        /// <summary> Busca en una lista simple separada por un delimitador
        /// Si se da un numero de orden no nulo retorna este numero de elemento
        /// </summary>
        /// <param name="text">   Texto donde buscar la cadena pedida    </param>
        /// <param name="order">  Numero de elemento de la lista buscado </param>
        /// <param name="separ1"> Caracter separador de cada elemento    </param>
        /// <returns> Cadena encontrada o nulo si no existe </returns>
        /// <remarks>
        /// Si se quiere especificar comienzo, la cuenta de caracteres o retornar 
        /// la posicion y longitud del elemento encontrado utilizar la version de
        /// este metodo con dos separadores dejando el segundo separador a cero.
        /// Para otros casos esta version con un solo separador es mas rapida
        /// </remarks>

        public static string GetText(string text, int order, int separ)
        {
            string resul = null;

            if (text != null)
            {
                int total = text.Length;
                int start = 0;
                int index = 0;

                if (total > 0)
                {
                    do
                    {
                        if (text[index] == separ)
                        {
                            if (order > 0)
                            {
                                start = index + 1;
                                order--;
                            }
                            else
                                break;
                        }
                    }
                    while (++index < total);
                }

                if (order == 0)
                    resul = text[start..index];
            }

            return resul;
        }

        /// <summary> Retorna orden de una clave es una lista con separador
        /// </summary>
        /// <param name="text"> Texto donde buscar la cadena </param>
        /// <param name="key">  Clave a buscar en la cadena  </param>
        /// <param name="separ">Caracter separador de lista  </param>
        /// <returns> Orden de la clave en la lista </returns>

        public static int GetOrder(string text, string key, int separ)
        {
            if (text != null)
            {
                int total = text.Length;
                int start = 0;
                int order = 0;
                int lonKey = key.Length;

                for (int index = 0; index <= total; index++)
                {
                    if (index == total || text[index] == separ)
                    {
                        if (start + lonKey <= total)
                        {
                            if (string.Compare(text, start, key, 0, lonKey,
                                StringComparison.OrdinalIgnoreCase) == 0)
                                return order;
                        }

                        start = index + 1;
                        order++;
                    }
                    else
                    {
                        if (char.IsWhiteSpace(text[index]))
                            start++;
                    }
                }
            }
            return -1;
        }

        /// <summary> Busca una clave comprendida entre uno o dos delimitadores
        /// Si se dan ambos delimitadores busca el texto comprendido entre ambos
        /// Si el segundo delimitador es nulo sera una busqueda en lista simple
        /// Si se da un numero de orden no nulo retorna este numero de aparicion
        /// </summary>
        /// <param name="text">   Texto donde buscar la cadena pedida    </param>
        /// <param name="order">  Numero de aparicion del texto buscado  </param>
        /// <param name="separ1"> Caracter separador inicial de busqueda </param>
        /// <param name="separ2"> Caracter separador final de busqueda  
        ///                       Si es nulo busca como una lista simple </param>
        /// <returns> Cadena encontrada o nulo si no existe </returns>

        public static string GetText(string text, int order, int separ1, int separ2)
        {
            if (text == null)
                return null;

            int start = 0;
            int count = text.Length;

            return GetText(text, order, separ1, separ2, ref start, ref count);
        }

        /// <summary> Busca una clave comprendida entre uno o dos delimitadores
        /// Si se dan ambos delimitadores busca el texto comprendido entre ambos
        /// Si el segundo delimitador es nulo sera una busqueda en lista simple
        /// Si se da un numero de orden no nulo retorna este numero de aparicion
        /// Esta sobrecarga indica y devuelve el indice de comienzo de  busqueda
        /// </summary>
        /// <param name="text">   Texto donde buscar la cadena pedida    </param>
        /// <param name="order">  Numero de aparicion del texto buscado  </param>
        /// <param name="separ1"> Caracter separador inicial de busqueda </param>
        /// <param name="separ2"> Caracter separador final de busqueda  
        ///                       Si es nulo busca como una lista simple </param>
        /// <param name="start">  Indice donde comenzar la busqueda      
        ///                       Retorna indice de la clave encontrada  </param>
        /// <returns> Cadena encontrada o nulo si no existe </returns>
        /// <remarks>
        /// Al retornar el indice queda avanzado un byte tras el primer separador
        /// Esto permite recorrer rapidamente en bucle las claves de una cadena
        /// El parametro start se inicializa antes del bucle y se actualiza solo
        /// El numero de orden debe ser siempre 0 para buscar la clave siguiente
        /// </remarks>

        public static string GetText(string text, int order, int separ1, int separ2, ref int start)
        {
            int count = -1;
            return GetText(text, order, separ1, separ2, ref start, ref count);
        }

        /// <summary> Busca una clave comprendida entre uno o dos delimitadores
        /// Si se dan ambos delimitadores busca el texto comprendido entre ambos
        /// Si el segundo delimitador es nulo sera una busqueda en lista simple
        /// Si se da un numero de orden no nulo retorna este numero de aparicion
        /// </summary>
        /// <param name="text">   Texto donde buscar la cadena pedida    </param>
        /// <param name="order">  Numero de aparicion del texto buscado  </param>
        /// <param name="separ1"> Caracter separador inicial de busqueda </param>
        /// <param name="separ2"> Caracter separador final de busqueda  
        ///                       Si es nulo busca como una lista simple </param>
        /// <param name="start">  Indice donde comenzar la busqueda      
        ///                       Retorna indice de la clave encontrada  </param>
        /// <param name="count">  Numero de caracteres donde buscar    
        ///                       Retorna longitud de clave encontrada   </param>
        /// <returns> Cadena encontrada o nulo si no existe </returns>

        public static string GetText(string text, int order, int separ1, int separ2, ref int start, ref int count)
        {
            string resul = null;

            if (text != null)
            {
                int index = start;
                int nivel = 0;

                if (count == -1)
                    count = text.Length;
                else
                {
                    count += start;
                    if (count > text.Length)
                        count = text.Length;
                }

                if (count > 0)
                {
                    do
                    {
                        char chr = text[index];

                        if (chr == separ2 && nivel > 0)
                        {
                            // Separador final al mismo nivel del inicial
                            if (--nivel == 0)
                            {
                                order--;

                                if (order < 0)
                                    break;
                            }
                        }

                        if (chr == separ1)
                        {
                            if (separ2 == 0)
                            {
                                // Separador final nulo: Lista con un separador
                                if (order > 0)
                                {
                                    start = index + 1;
                                    order--;
                                }
                                else
                                {
                                    order = -1;
                                    break;
                                }
                            }
                            else
                            {
                                // Claves entre dos separadores: Marcar inicio
                                if (nivel == 0 && order == 0)
                                    start = index + 1;

                                nivel++;
                            }
                        }
                    }
                    while (++index < count);
                }

                // Retornar clave si encuentra el numero de separador
                if (order <= 0)
                {
                    if (order == -1 || separ2 == 0)
                    {
                        count = index - start;
                        resul = text.Substring(start, count);
                    }
                }
            }
            return resul;
        }

        /// <summary> Busca una clave comprendida entre uno o dos delimitadores
        /// Si se dan ambos delimitadores busca el texto comprendido entre ambos
        /// Si el segundo delimitador es nulo sera una busqueda en lista simple
        /// Si se da un numero de orden no nulo retorna este numero de aparicion
        /// </summary>
        /// <param name="text">   Texto donde buscar la cadena pedida    </param>
        /// <param name="order">  Numero de aparicion del texto buscado  </param>
        /// <param name="separ1"> Caracter separador inicial de busqueda </param>
        /// <param name="separ2"> Caracter separador final de busqueda  
        ///                       Si es nulo busca como una lista simple </param>
        /// <param name="start">  Indice donde comenzar la busqueda      </param>
        /// <param name="count">  Numero de caracteres donde buscar      </param>
        /// <returns> Cadena encontrada o nulo si no existe </returns>
        /// <remarks>
        /// Esta sobrecarga se diferencia de la anterior en omitir Ref en parametros
        /// Unicamente sirve para facilitar la llamada usando constantes en linea
        /// Puede eliminarse ya que se usa poco si se requiere compatibilidad con CLS
        /// </remarks>

        public static string GetText(string text, int order, int separ1, int separ2, int start, int count)
        {
            return GetText(text, order, separ1, separ2, ref start, ref count);
        }

        /// <summary> Substituye valor comprendido entre uno o dos delimitadores
        /// Si se dan ambos delimitadores busca el texto comprendido entre ambos
        /// Si el segundo delimitador es nulo sera una busqueda en lista simple
        /// Si se da un numero de orden no nulo retorna este numero de aparicion
        /// </summary>
        /// <param name="text">   Texto donde buscar la cadena pedida    </param>
        /// <param name="order">  Numero de aparicion del texto buscado  </param>
        /// <param name="separ1"> Caracter separador inicial de busqueda </param>
        /// <param name="separ2"> Caracter separador final de busqueda  
        ///                       Si es nulo busca como una lista simple </param>
        /// <param name="start">  Indice donde comenzar la busqueda      
        ///                       Retorna indice de la clave encontrada  </param>
        /// <param name="count">  Numero de caracteres donde buscar    
        ///                       Retorna longitud de la clave encontrada</param>
        /// <returns> Cadena modificada con el valor indicado </returns>


        public static string SetText(string text, int order, int separ1, int separ2, string field)
        {
            string actual = null;
            int start = 0;


            text ??= "";

            int index = start;
            int nivel = 0;

            int count = text.Length;

            if (count > 0)
            {
                do
                {
                    char chr = text[index];

                    if (chr == separ2 && nivel > 0)
                    {
                        // Separador final al mismo nivel del inicial
                        if (--nivel == 0)
                        {
                            order--;

                            if (order < 0)
                                break;
                        }
                    }

                    if (chr == separ1)
                    {
                        if (separ2 == 0)
                        {
                            // Separador final nulo: Lista con un separador
                            if (order > 0)
                            {
                                start = index + 1;
                                order--;
                            }
                            else
                            {
                                order = -1;
                                break;
                            }
                        }
                        else
                        {
                            // Claves entre dos separadores: Marcar inicio
                            if (nivel == 0 && order == 0)
                                start = index + 1;

                            nivel++;
                        }
                    }
                }
                while (++index < count);
            }

            if (order > 0)
            {
                // Añadir separadores que faltan si no hay bastantes
                text += "".PadRight(order, (char)separ1);
                start = text.Length;
                count = 0;
                actual = "";
            }
            else
            {
                // Obtener valor del campo encontrado
                if (order == -1 || separ2 == 0)
                {
                    count = index - start;
                    actual = text.Substring(start, count);
                }
            }

            // Substituir campo si existe y es distinto
            if (actual != null && actual != field)
            {
                if (separ2 == 0)
                    text = string.Concat(text.AsSpan(0, start), field, text.AsSpan(start + count));
                else
                    text = string.Concat(text.AsSpan(0, start - 1), field, text.AsSpan(start + count + 1));
            }

            return text;
        }


        // Funcion antigua: SetText: FALLA !!!
        // 
        // Comprobar si hay algun aincompatibilidad con la nueva
        // 
        // public static string Set_Text(string text, int order, int separ1, int separ2, string field) 
        // {
        //     int start = 0;
        //     int count = text.Length;
        //     string actual = GetText(text, order, separ1, separ2, ref start, ref count);
        // 
        //     if (actual == null && separ2 == 0)
        //     {
        //         // Crear elementos que faltan hasta el pedido
        //         StrScan scan = new StrScan(text, (char)separ1);
        //         int total = scan.Count();
        // 
        //         if (total <= order)
        //         {
        //             text += "".PadRight(order + 1 - total, (char)separ1);
        //             actual = "";
        //             start  = text.Length;
        //             count = 0;
        //         }
        //     }
        // 
        //     if (actual != null && actual != field)
        //     {
        //         if (separ2 == 0)
        //             text = text.Substring(0, start) + field + 
        //                   text.Substring(start+count);
        //        else
        //            text = text.Substring(0, start-1) + field + 
        //                   text.Substring(start+count+1);
        //    }
        //    return text;
        // }

        #endregion 

        #region SUBSTITUCION DE PARAMETROS EN CADENAS

        /// <summary> Substituye parametros y zonas opcionales en una cadena
        /// Los parametros se subtituyen segun el formato de cadena standard
        /// Ademas reconoce zonas opcionales que se eliminan si no hay valores
        /// </summary>
        /// <param name="text">   Cadena de texto origen a substituir  </param>
        /// <param name="values"> Array de valores para cada parametro </param>
        /// <returns> Cadena con parametros y opciones substituidas </returns>
        /// <remarks>
        /// Los parametros se marcan entre corchetes con sintaxis standard
        /// Las zonas opcionales se marcan entre parentesis cuadradados 
        /// Debe haber al menos un parametro para tratarla como zona opcional
        /// 
        /// Si todos los parametros de la zona opcional son nulos se elimina
        /// En la cadena resultante se subtituyen valores de los parametros
        /// Puede haber zonas opcionales anidadas que se tratan por separado
        /// </remarks>

        public static string SubParam(string text, object[] values)
        {
            // text = SubOption(text, values);

            if (values == null)
            {
                text = SubOption(text, null);
                text = DelParam(text);
            }
            else
            {
                string cSub = text;
                text = cSub;

                text = SubOption(text, values);

                // if (Str.Equals(cSub, text))
                // {
                //     text = String.Format(text, values);
                // }

                // text = SubOption(text, values);
            }

            // if (values != null)
            //     text = String.Format(text, values);
            // else
            //     text = DelParam(text);

            return text;
        }

        /// <summary> Elimina todos los parametros de la cadena dada
        /// </summary>
        /// <param name="text"> cadena a eliminar parametros </param>
        /// <returns> Cadena modificada </returns>

        public static string DelParam(string text)
        {
            while (text.Contains('{'))
                text = SetText(text, 0, '{', '}', null);

            return text;
        }

        /// <summary> Substituye solo las zonas opcionales en una cadena
        /// Los parametros se subtituyen segun el formato de cadena standard
        /// Si todos los parametros son nulos se elimina la zona opcional
        /// Los paramatros y texto fuera de una zona opcional no se toca
        /// </summary>
        /// <param name="text">   Cadena de texto origen a substituir  </param>
        /// <param name="values"> Array de valores para cada parametro </param>
        /// <returns> Cadena con parametros y opciones substituidas </returns>

        public static string SubOption(string text, object[] values)
        {
            string field;

            int start = 0;
            int count = text.Length;

            // Substituir parametros en areas opcionales 
            while ((field = GetText(text, 0, '[', ']', ref start, ref count)) != null)
            {
                string prev = field;

                if (field.Contains('['))
                    field = SubOption(field, values);

                // Comprobar si los valores de todos los parametros son nulos
                // Si existe alguno no nulo debe mantenerse y substituir
                field = SubValues(field, values, out bool lEmpty);

                if (lEmpty)
                    field = "";  // Sin valores: Borrar zona opcional
                // else
                //     field = String.Format(field, values);

                if (prev != field)
                {
                    text = string.Concat(text.AsSpan(0, start - 1), field, text.AsSpan(start + count + 1));
                }
                else
                {
                    if (field.Length < 1)
                        start++;
                }

                // Avanzar el indice para no procesar el mismo valor
                start += field.Length - 1;
                count = text.Length - start;

                if (count <= 0)
                    break;
            }

            // Substituir parametros fuera de areas opcionales
            text = SubValues(text, values, out _);

            return text;
        }

        /// <summary> Substituye paramteros standard en la cadena dada
        /// </summary>
        /// <param name="text">   Cadena con paraetros a substiuir </param>
        /// <param name="values"> Array de valores de parametros   </param>
        /// <param name="lEmpty"> Todas las substituciones vacias  </param>
        /// <returns> Cadena modificada </returns>

        private static string SubValues(string text, object[] values, out bool lEmpty)
        {
            string cPar;
            int nInd = 0;
            lEmpty = false;

            int start = 0;
            int count = text.Length;

            // while ((cPar = Str.GetText(text, nInd++, '{', '}', ref start, ref count)) != null)
            while ((cPar = GetText(text, nInd, '{', '}', ref start, ref count)) != null)
            {
                if (IsNumber(cPar))
                {
                    // Existen parametros numerico: Asumir vacio
                    // if (nInd == 1)
                    if (nInd == 0)
                        lEmpty = true;

                    if (values != null)
                    {
                        int nPar = int.Parse(cPar);

                        if (nPar >= 0 && nPar < values.Length)
                        {
                            // Se toma parametro vacio solo con valor nulo
                            // Esto es correcto para los valores numericos
                            // Sin embargo los textos vacios deben quitarse
                            // Los textos deben ponerse a null al cargarlos
                            // En este punto ya no es conveniente hacerlo

                            string value = Data.ToString(values[nPar]);

                            if (Empty(value))
                            {
                                // Si esta entre parentesis se quitan cadenas vacias
                                if (!Empty(GetText(text, 0, '(', ')')))
                                    value = null;
                            }

                            if (value != null)   // Existe algun valor no nulo
                                lEmpty = false;  // Mantener el campo si es opcional

                            value ??= "";

                            text = string.Concat(text.AsSpan(0, start - 1), value, text.AsSpan(start + count + 1));

                            start += value.Length - 1;
                        }
                        else
                        {
                            // Eliminar indice inexistente para format
                            nInd--;
                            text = SetText(text, nInd, '{', '}', null);
                        }

                        count = text.Length - start;

                        if (count == 0)
                            break;
                    }
                }
                else
                {
                    // Proteccion frente a bucle infinito
                    break;
                }
            }

            return text;
        }

        #endregion 

        #region SOPORTE PARA CADENAS DE LISTAS CLAVE VALOR 

        /// <summary> Retorna el valor asociado a una clave en una lista
        /// La lista son pares clave-valor separados por un punto y coma
        /// Cada par clave-valor se separa a su vez por un signo igual
        /// </summary>
        /// <param name="list"> Cadena con pares clave valor </param>
        /// <param name="key">  Clave a buscar en la lsita   </param>
        /// <returns> Valor asociado a la clave </returns>

        public static string GetValue(string list, string key)
        {
            string value = null;

            if (list != null)
            {
                StrScan scan = Scan(list, ';');

                foreach (string item in scan)
                {
                    if (item.StartsWith(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        value = Item(item, 1, '=');
                        value ??= string.Empty;
                        break;
                    }
                }
            }

            return value;
        }

        /// <summary> Retorna el numero de elementos en la ista dada
        /// Los elementos se consideran separados por punto y coma
        /// Pueden llevar o no valor asociado separado por signo igual
        /// </summary>
        /// <param name="list"> Lista a procesar </param>
        /// <returns> Cuenta de elementos </returns>

        public static int CountItems(string list)
        {
            StrScan scan = Scan(list, ';');
            return scan.Count();
        }

        public static string SetValue(string list, string key, string value)
        {
            if (list == null)
            {
                list = key + '=' + value;
            }
            else
            {
                StrScan scan = Scan(list, ';');
                bool found = false;

                foreach (string item in scan)
                {
                    if (item.StartsWith(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        scan.Current = key + '=' + value;
                        list = scan.Text;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    list += ';' + key + '=' + value;
            }
            return list;
        }

        #endregion

        #region ENUMERACION DE CADENAS CON SEPARADORES

        /// <summary> Iteracion de una cadena de claves con separador
        /// Retorna un iterador que devuelve cada elemento de la cadena
        /// Se utiliza para ello el separador indicado en el parametro
        /// Se puede usar en bucle foreach para recorrer la lista dada
        /// </summary>
        /// <param name="cStr"> Cadena con los elementos a iterar  </param>
        /// <param name="cSep"> Caracter separador entre elementos </param>
        /// <returns> Iterador para recorrer la cadena </returns>

        public static StrScan Scan(string cStr, char cSep)
        {
            return new StrScan(cStr, cSep);
        }

        /// <summary> Iteracion de una cadena con varios separadores
        /// Retorna un iterador que devuelve cada elemento de la cadena
        /// Se utiliza el array de caracteres separadores indicado
        /// Se puede usar en bucle foreach para recorrer la lista dada
        /// </summary>
        /// <param name="cStr"> Cadena con los elementos a iterar </param>
        /// <param name="aSep"> Array de caracteres separadores   </param>
        /// <returns> Iterador para recorrer la cadena </returns>

        public static StrScan Scan(string cStr, char[] aSep)
        {
            return new StrScan(cStr, aSep);
        }

        /// <summary> Iteracion de una cadena separada por punto y coma
        /// Retorna un iterador que devuelve cada elemento de la cadena
        /// Se puede usar en bucle foreach para recorrer la lista dada
        /// </summary>
        /// <param name="cStr"> Cadena con los elementos a iterar  </param>
        /// <param name="cSep"> Caracter separador entre elementos </param>
        /// <returns> Iterador para recorrer la cadena </returns>

        public static StrScan Scan(string cStr)
        {
            return Scan(cStr, ',');
        }

        #endregion

        #region SOPORTE PARA CODIFICACION DE CARACCTERES 

        /// <summary> Conversion a la codificación por defecto de una cadena oem
        /// </summary>
        /// <param name="oemText"> Cadena de texto Oem a convertir </param>
        /// <returns> Texto convertido a la codificación por defecto </returns>
        /// <remarks>
        /// La pagina de codigos MSDOS por defecto para Europa-Oeste es 858 
        /// Se devuelve al acceder a una base de datos con el driver Dbase 
        /// Una vez en memoria se debe traducir a la codificacion por defecto
        /// </remarks>

        static public string OemToText(string oemText)
        {
            OemEncoding ??= Encoding.GetEncoding(858);

            byte[] bytes = OemEncoding.GetBytes(oemText);
            string text = Encoding.Default.GetString(bytes);

            return text;
        }

        /// <summary> Conversion a Oem de texto codificado por defecto 
        /// </summary>
        /// <param name="text"> Cadena de texto Oem a convertir </param>
        /// <returns> Texto convertido a codificación OEM pagina 858 </returns>
        /// <remarks>
        /// La pagina de codigos MSDOS por defecto para Europa-Oeste es 858 
        /// Se devuelve al acceder a una base de datos con el driver Dbase 
        /// Una vez en memoria se debe traducir a la codificacion por defecto
        /// </remarks>

        static public string TextToOem(string text)
        {
            OemEncoding ??= Encoding.GetEncoding(858);

            byte[] bytes = Encoding.Default.GetBytes(text);
            string oemText = OemEncoding.GetString(bytes);

            return oemText;
        }
        static private Encoding OemEncoding;

        /// <summary> Convierte los caracteres no ASCII a entidades html
        /// El texto origen puede contener otras marcas que se mantienen
        /// </summary>
        /// <param name="text"> Cadena con el texto a convertir </param>
        /// <returns> Texto convertido a codificación HTML </returns>
        /// <remarks>
        /// Este método cambia únicamente los caracteres no ASCII
        /// Estos codigos se convierten a entidades standrd html 
        /// 
        /// Se comprueban solo los primeros 256 codigos de ISO-8859-1
        /// Esto es suficiente para la gran mayoria de aplicaciones 
        /// 
        /// El juego de caracteres por defecto del navegador es ISO-8859-1
        /// Este juego se usa en toda Europa occidental, Estados unidos,
        /// canada, africa, caribe y latinoamerica
        /// Por tanto se puede considerar con validez y amplituz suficiente
        /// Quedan fuera caracteres cirilicos, arabes, nordicos, etc
        /// 
        /// El UNICODE representa un standard para cualquier lenguaje
        /// Los primeros 256 caracteres coindicen con la ISO-8859-1
        /// Por tanto es compatible con el rango de paises citado
        /// 
        /// El standard Unicode se implementa con varios sistemas:
        /// 
        ///   - UTF-8: Usa codigos de 8 bits codificado de 0-4 bytes
        ///     Es el sistema recomendado para email y paginas web
        ///     Los primeros 127 caracteres coinciden con el ASCII
        ///     
        ///   - UTF-16: Usa codigos de 16 bits de longitud variable
        ///     Es el sistema usado en plataformas windows y .NET  
        ///    
        /// </remarks>

        static public string TextToHtml(string text)
        {
            if (text != null)
            {
                if (HtmlTable == null)
                    CreateHtmlTable();

                for (int index = 0; index < text.Length; index++)
                {
                    int chr = text[index];

                    if (chr >= 160 && chr <= 255)
                    {
                        // Convertir el caracter a entidad html si est definida
                        // No se deben convertir signos menor/mayor ni ampersand
                        // Estos simbolos componen las marcas y entidades html

                        string token = HtmlTable[chr - 1];

                        if (token != null)
                        {
                            text = text[..index] + '&' + token + ';' +
                                   text[(index + 1)..];

                            index += token.Length + 2;
                        }
                    }
                }
            }
            return text;
        }

        static private string[] HtmlTable;

        static private void CreateHtmlTable()
        {
            HtmlTable = new string[255];

            // Caracteres Html reservados 
            HtmlTable[33] = "quot";
            HtmlTable[38] = "apos";  // No funciona en IE
            HtmlTable[37] = "amp";
            HtmlTable[59] = "lt";
            HtmlTable[61] = "gt";

            // Caracteres de simbolos html

            HtmlTable[159] = "nbsp";    // non-breaking space
            HtmlTable[160] = "iexcl";   // inverted exclamation mark
            HtmlTable[161] = "cent";    // cent
            HtmlTable[162] = "pound";   // pound
            HtmlTable[163] = "curren";  // currency
            HtmlTable[164] = "yen";     // yen
            HtmlTable[165] = "brvbar";  // broken vertical bar
            HtmlTable[166] = "sect";    // section
            HtmlTable[167] = "uml";     // spacing diaeresis
            HtmlTable[168] = "copy";    // copyright
            HtmlTable[169] = "ordf";    // feminine ordinal indicator
            HtmlTable[170] = "laquo";   // angle quotation mark (left)
            HtmlTable[171] = "not";     // negation
            HtmlTable[172] = "shy";     // soft hyphen
            HtmlTable[173] = "reg";     // registered trademark
            HtmlTable[174] = "macr";    // spacing macron
            HtmlTable[175] = "deg";     // degree
            HtmlTable[176] = "plusmn";  // plus-or-minus
            HtmlTable[177] = "sup2";    // superscript 2
            HtmlTable[178] = "sup3";    // superscript 3
            HtmlTable[179] = "acute";   // spacing acute

            HtmlTable[180] = "micro";   // micro
            HtmlTable[181] = "para";    // paragraph
            HtmlTable[182] = "middot";  // middle dot
            HtmlTable[183] = "cedil";   // spacing cedilla
            HtmlTable[184] = "sup1";    // superscript 1
            HtmlTable[185] = "ordm";    // masculine ordinal indicator
            HtmlTable[186] = "raquo";   // angle quotation mark (right)
            HtmlTable[187] = "frac14";  // fraction 1/4
            HtmlTable[188] = "frac12";  // fraction 1/2
            HtmlTable[189] = "frac34";  // fraction 3/4
            HtmlTable[190] = "iquest";  // inverted question mark

            HtmlTable[214] = "times";   // multiplication
            HtmlTable[246] = "divide";  // division

            // Caracteres especificos de idiomas
            HtmlTable[191] = "Agrave";  // capital a, grave accent
            HtmlTable[192] = "Aacute";  // capital a, acute accent
            HtmlTable[193] = "Acirc";   // capital a, circumflex accent
            HtmlTable[194] = "Atilde";  // capital a, tilde
            HtmlTable[195] = "Auml";    // capital a, umlaut mark
            HtmlTable[196] = "Aring";   // capital a, ring
            HtmlTable[197] = "AElig";   // capital ae
            HtmlTable[198] = "Ccedil";  // capital c, cedilla
            HtmlTable[199] = "Egrave";  // capital e, grave accent
            HtmlTable[200] = "Eacute";  // capital e, acute accent
            HtmlTable[201] = "Ecirc";   // capital e, circumflex accent
            HtmlTable[202] = "Euml";    // capital e, umlaut mark
            HtmlTable[203] = "Igrave";  // capital i, grave accent
            HtmlTable[204] = "Iacute";  // capital i, acute accent
            HtmlTable[205] = "Icirc";   // capital i, circumflex accent
            HtmlTable[206] = "Iuml";    // capital i, umlaut mark
            HtmlTable[207] = "ETH";     // capital eth, Icelandic
            HtmlTable[208] = "Ntilde";  // capital n, tilde
            HtmlTable[209] = "Ograve";  // capital o, grave accent
            HtmlTable[210] = "Oacute";  // capital o, acute accent
            HtmlTable[211] = "Ocirc";   // capital o, circumflex accent
            HtmlTable[212] = "Otilde";  // capital o, tilde
            HtmlTable[213] = "Ouml";    // capital o, umlaut mark

            HtmlTable[215] = "Oslash";  // capital o, slash
            HtmlTable[216] = "Ugrave";  // capital u, grave accent
            HtmlTable[217] = "Uacute";  // capital u, acute accent
            HtmlTable[218] = "Ucirc";   // capital u, circumflex accent
            HtmlTable[219] = "Uuml";    // capital u, umlaut mark
            HtmlTable[220] = "Yacute";  // 
            HtmlTable[221] = "THORN";   // capital THORN, Icelandic
            HtmlTable[222] = "szlig";   // small sharp s, German
            HtmlTable[223] = "agrave";  // small a, grave accent
            HtmlTable[224] = "aacute";  // small a, acute accent
            HtmlTable[225] = "acirc";   // small a, circumflex accent
            HtmlTable[226] = "atilde";  // small a, tilde
            HtmlTable[227] = "auml";    // small a, umlaut mark 
            HtmlTable[228] = "aring";   // small a, ring
            HtmlTable[229] = "aelig";   // small ae
            HtmlTable[230] = "ccedil";  // small c, cedilla
            HtmlTable[231] = "egrave";  // small e, grave accent
            HtmlTable[232] = "eacute";  // small e, acute accent
            HtmlTable[233] = "ecirc";   // small e, circumflex accent
            HtmlTable[234] = "euml";    // small e, umlaut mark
            HtmlTable[235] = "igrave";  // small i, grave accent
            HtmlTable[236] = "iacute";  // small i, acute accent
            HtmlTable[237] = "icirc";   // small i, circumflex accent
            HtmlTable[238] = "iuml";    // small i, umlaut mark
            HtmlTable[239] = "eth";     // small eth, Icelandic
            HtmlTable[240] = "ntilde";  // small n, tilde
            HtmlTable[241] = "ograve";  // small o, grave accent
            HtmlTable[242] = "oacute";  // small o, acute accent
            HtmlTable[243] = "ocirc";   // small o, circumflex accent
            HtmlTable[244] = "otilde";  // small o, tilde
            HtmlTable[245] = "ouml";    // small o, umlaut mark

            HtmlTable[247] = "oslash";  // small o, slash
            HtmlTable[248] = "ugrave";  // small u, grave accent
            HtmlTable[249] = "uacute";  // small u, acute accent
            HtmlTable[250] = "ucirc";   // small u, circumflex accent
            HtmlTable[251] = "uuml";    // small u, umlaut mark
            HtmlTable[252] = "yacute";  // small y, acute accent
            HtmlTable[253] = "thorn";   // small thorn, Icelandic
            HtmlTable[254] = "yuml";    // small y, umlaut mark
        }

        #endregion

    }
    #endregion

    #region CLASE SOPORTE DE COMPARACION DE CADENAS

    public enum CompareType
    {
        None,        // Comparacion standerd de cadenas
        Ordinal,     // Comparacion binaria de cadenas
        OrdinalCase, // Comparacion binaria con mayusculas/minusculas
        Current,     // Comparacion de cadenas con la cultura actual
        CurrentCase, // Comparacion con la cultura y mayusculas/minusculas
    }

    /// <summary> Compara dos cadenas y devuelve la relacion entre ellas
    /// 
    /// -1: La primera cadena es menor a la segunda
    ///  0: Ambas cadens son iguales
    ///  1: La primera cadena es mayor a la segunda
    ///  
    /// </summary>

    public class StrComp : IComparer<string>
    {
        public CompareType CompareType
        {
            get
            {
                return compType switch
                {
                    StringComparison.OrdinalIgnoreCase => CompareType.Ordinal,
                    StringComparison.Ordinal => CompareType.OrdinalCase,
                    StringComparison.CurrentCulture => CompareType.CurrentCase,
                    StringComparison.CurrentCultureIgnoreCase => CompareType.Current,
                    _ => CompareType.None,
                };
            }

            set
            {
                compType = value switch
                {
                    CompareType.Ordinal => StringComparison.OrdinalIgnoreCase,
                    CompareType.OrdinalCase => StringComparison.Ordinal,
                    CompareType.Current => StringComparison.CurrentCultureIgnoreCase,
                    CompareType.CurrentCase => StringComparison.CurrentCulture,
                    _ => StringComparison.OrdinalIgnoreCase,
                };
            }
        }
        StringComparison compType;
        readonly int Start;
        readonly int Length;

        public StrComp(CompareType type, int start, int length)
        {
            CompareType = type;
            Start = start;
            Length = length;
        }

        public StrComp(int start, int length)
        {
            Start = start;
            Length = length;
            compType = StringComparison.OrdinalIgnoreCase;
        }

        public StrComp(CompareType type)
        {
            CompareType = type;
        }

        public StrComp()
        {
            compType = StringComparison.OrdinalIgnoreCase;
        }

        public int Compare(string str1, string str2)
        {
            int resul;

            if (str1.StartsWith("1-"))
            {
            }

            if (Length <= 0)
            {
                resul = string.Compare(str1, str2, compType);
            }
            else
            {
                if (str1 == null)
                {
                    if (str2 == null)
                        return 0;
                    else
                        return -1;
                }
                else if (str2 == null)
                {
                    if (str1 == null)
                        return 0;
                    else
                        return 1;
                }
                else
                {
                    if (Start > 0)
                    {
                        if (str1.Length < Start)
                        {
                            if (str2.Length < Start)
                                return 0;
                            else
                                return -1;
                        }

                        if (str2.Length < Start)
                        {
                            if (str1.Length < Start)
                                return 0;
                            else
                                return 1;
                        }
                    }
                    resul = string.Compare(str1, Start, str2, Start, Length, compType);
                }
            }

            return resul;
        }
    }

    #endregion

    #region CLASE SOPORTE DE ENUMERACION DE CADENAS

    /// <summary> Enumeracion de tokens contenidos en una cadena 
    /// </summary>

    public class StrScan : IEnumerable, IEnumerator
    {
        private string cScan;
        private readonly char cSepar;
        private readonly char[] aSepar;
        private int nIndex;
        private int nNext;
        private int nStart;
        private int nOrder;
        private readonly bool IsTrim;

        const int IndexReset = -2; // Indice Next al principio
        const int IndexEnd = -1; // Indice Next al final

        /// <summary> Devuelve un enumerador de una cadena
        /// </summary>
        /// <param name="cStr"> Cadena a enumerar  </param>
        /// <param name="cSep"> Caracter separador </param>

        public StrScan(string cStr, char cSep)
        {
            cScan = cStr;
            cSepar = cSep;
            nIndex = nOrder = 0;
            nNext = IndexReset;
            IsTrim = true;
        }

        /// <summary> Devuelve un enumerador de una cadena
        /// </summary>
        /// <param name="cStr"> Cadena a enumerar    </param>
        /// <param name="aSep"> Array de separadores </param>

        public StrScan(string cStr, char[] aSep)
        {
            cScan = cStr;
            aSepar = aSep;
            nIndex = nOrder = 0;
            nNext = IndexReset;
            IsTrim = true;
        }

        /// <summary> Metodo para obtener el enumerador
        /// La misma clase implementa los metodos del interfaz
        /// </summary>
        /// <returns> Referncia usada como enumerador </returns>

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        /// <summary> Reinicia la enumeracion
        /// </summary>

        public void Reset()
        {
            nIndex = nStart;
            nOrder = 0;
            nNext = IndexReset;
        }

        /// <summary> Posiciona el siguiente elemento de la cadena
        /// Puede utilizar uno o varios caracteres separadores
        /// </summary>
        /// <returns> Indica si existen mas elementos </returns>

        public bool MoveNext()
        {
            if (cScan == null)
                return false;

            if ((nIndex = nNext) < 0)
            {
                if (nIndex == IndexEnd)
                    return false;
                else
                    nIndex = nStart;
            }

            if (nNext >= 0)
            {
                nIndex++;
                nOrder++;
            }

            if (aSepar != null)
                nNext = cScan.IndexOfAny(aSepar, nIndex);
            else
            {
                nNext = cScan.IndexOf(cSepar, nIndex);

                if (cSepar == ' ' && nNext >= 0)
                {
                    int length = cScan.Length - 1;
                    while (nNext < length && cScan[nNext + 1] == ' ')
                        nNext++;
                }

            }

            return true;
        }

        /// <summary> Obtiene Id al elemento actual
        /// Aunque no es parte del interfaz standard este 
        /// metodo permite tambien la modificacion del mismo
        /// </summary>

        public object Current
        {
            get { return GetItem(); }
            set { SetItem((string)value); }
        }

        /// <summary> Retorna el elemento actual como texto
        /// </summary>
        /// <returns> Cadena con el elemnto actual </returns>

        public string GetItem()
        {
            // Obtener longitud del token resultado
            int nLength;

            if (nNext < 0)
                nLength = cScan.Length - nIndex;
            else
                nLength = nNext - nIndex;

            return GetResul(nIndex, nLength);
        }

        private string GetResul(int index, int length)
        {
            if (IsTrim)
            {
                // Quitar blancos al inicio del token
                while (length > 0)
                {
                    if (char.IsWhiteSpace(cScan, index))
                    {
                        index++;
                        length--;
                    }
                    else
                        break;
                }

                // Quitar blancos al final del token
                while (length > 0)
                {
                    if (char.IsWhiteSpace(cScan, index + length - 1))
                    {
                        length--;
                    }
                    else
                        break;
                }
            }

            return cScan.Substring(index, length);
        }

        public string GetKey()
        {
            // Obtener longitud del token resultado
            int nLength;

            if (nNext < 0)
                nLength = cScan.Length - nIndex;
            else
                nLength = nNext - nIndex;

            // Buscar separador de clave y valor
            int npos = cScan.IndexOf('=', nIndex, nLength);

            if (npos >= 0)
                nLength = npos - nIndex;

            return GetResul(nIndex, nLength);
        }

        public string GetValue()
        {
            // Obtener longitud del token resultado
            int nLength;

            if (nNext < 0)
                nLength = cScan.Length - nIndex;
            else
                nLength = nNext - nIndex;

            // Buscar separador de clave y valor
            if (nLength > 0)
            {
                int npos = cScan.IndexOf('=', nIndex, nLength - 1);

                if (npos > 0)
                    return GetResul(npos + 1, nLength - (npos - nIndex) - 1);
            }
            return null;
        }

        public void SetValue(string value)
        {
            value = GetKey() + '=' + value;
            SetItem(value);
        }

        /// <summary> Modifica el valor del elemento actual
        /// </summary>
        /// <param name="value"> Nuevo valor pra el elemento </param>

        public void SetItem(string value)
        {
            if (nNext < 0)
                cScan = cScan[..nIndex] + value;
            else
            {
                int nTotal = cScan.Length;
                cScan = cScan[..nIndex] + value +
                        cScan[nNext..];
                nNext += cScan.Length - nTotal;
            }
        }

        /// <summary> Borra el elemento actual y ajusta indices
        /// </summary>
        /// <remarks>
        /// Despues de borrar quedan los indices incoherentes
        /// La siguiente llamada a MoveNext() los regulariza 
        /// Por tanto lo mejor es continuar el bucle de proceso
        /// Si se debe hacer algo mas (cargar item, borrar otro)
        /// debe ejcutarse un MoveNext despues del borrado
        /// </remarks>

        public void DelItem()
        {
            if (nNext < 0)
            {
                if (nIndex > 0)
                    cScan = cScan[..(nIndex - 1)];
                else
                    cScan = string.Empty;
            }
            else
            {
                if (nIndex == 0)
                {
                    cScan = cScan[(nNext + 1)..];
                    nNext = IndexReset;
                }
                else
                {
                    cScan = string.Concat(cScan.AsSpan(0, nIndex), cScan.AsSpan(nNext + 1));

                    nNext = nIndex - 1;
                }
            }
        }

        /// <summary> Asigna cadena completa y reinicia exploracion
        /// 
        /// </summary>
        /// <param name="value"> Nuevo texto de la cadena </param>

        public void SetText(string value)
        {
            cScan = value;
            Reset();
        }

        /// <summary> Retorna cadena de texto modificada
        /// </summary>

        public string Text
        {
            get { return cScan; }
            set { SetText(value); }
        }

        /// <summary> Retorna indice en la cadena del elemento actual
        /// </summary>

        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; }
        }

        /// <summary> Retorna indice incial de busqueda en la cadena
        /// </summary>

        public int Start
        {
            get { return nStart; }
            set { nStart = value; }
        }

        /// <summary> Retorna numero de orden del elemento actual
        /// </summary>

        public int Order
        {
            get { return nOrder; }
        }

        /// <summary> Retorna elemento siguiente manteniendo el actual
        /// </summary>
        /// <returns> Cadena con el elemento siguiente </returns>

        public string GetNext()
        {
            int SaveIndex = nIndex;
            int SaveNext = nNext;
            int SaveOrder = nOrder;

            string resul = null;

            if (MoveNext())
                resul = GetItem();

            nIndex = SaveIndex;
            nNext = SaveNext;
            nOrder = SaveOrder;

            return resul;
        }

        public bool SetOrder(int nOrder)
        {
            if (Order > nOrder)
                Reset();

            while (Order < nOrder)
            {
                if (!MoveNext())
                    return false;
            }
            return true;
        }

        /// <summary> Retorna resto de la cadena como token unico
        /// </summary>
        /// <returns> Resto de la cadena </returns>

        public string GetRest()
        {
            nNext = IndexEnd;
            return GetItem();
        }


        /// <summary> Retorna delimitador final del token actual
        /// Si el elemento no tiene separador detras retorna nulo
        /// </summary>
        /// <returns> Siguiente delimitador </returns>
        /// <remarks>
        /// Para analisis sintactico los separadores son operadores
        /// Por tanto este metodo retorna el siguiente operador
        /// </remarks>

        public char GetSeparator()
        {
            if (nNext >= 0 && cScan.Length > nNext)
                return cScan[nNext];

            return '\0';
        }

        /// <summary> Cuenta el numero de elemento restantes
        /// Devuelve los elementos que quedan por iterar 
        /// </summary>
        /// <returns> Numero de elemntos </returns>

        public int Count()
        {
            int SaveIndex = nIndex;
            int SaveNext = nNext;
            int SaveOrder = nOrder;
            int nCount = 0;

            while (MoveNext())
            {
                if (!Str.Empty(Current as string))
                    nCount++;
            }

            nIndex = SaveIndex;
            nNext = SaveNext;
            nOrder = SaveOrder;

            return nCount;
        }
    }

    #endregion
}
