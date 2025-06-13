using MailAppMAUI.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MailAppMAUI.General
{
    /// <summary> metodos generales de soporte de arrays
    /// </summary>

    public static class Arr
    {
        /// <summary> Añade un nuevo objeto a un array generico
        /// Se crea un nuevo array con la longitud incrementada
        /// Se copian los elementos previos y se añade el nuevo
        /// Si el array es nulo crea nuevo array con este valor
        /// </summary>
        /// <param name="Origen"> Array a modificar    </param>
        /// <param name="value">  Nuevo valor a añadir </param>
        /// <returns> Array modificado </returns>

        public static void Append<T>(ref T[] origen, T value)
        {
            if (origen == null)
                origen = [value];
            else
            {
                int size = origen.Length + 1;
                Array.Resize(ref origen, size);
                origen[size - 1] = value;
            }
        }

        public static T[] Append<T>(T[] origen, T value)
        {
            Append(ref origen, value);
            return origen;
        }

        public static void Append<T>(ref T[] destino, T[] origen)
        {
            if (destino == null)
            {
                origen = [];
            }
            else
            {
                if (origen != null && origen.Length > 0)
                {
                    int index = destino.Length;

                    Resize(ref destino, destino.Length + origen.Length);

                    Array.Copy(origen, 0, destino, index, origen.Length);
                }
            }
        }

        public static T[] Append<T>(T[] destino, T[] origen)
        {
            Append(ref destino, origen);

            return destino;
        }

        public static Array Create(object item, int size)
        {
            Array array = null;

            if (item != null)
            {
                Type type = item.GetType();
                array = Array.CreateInstance(type, size);
            }
            return array;
        }

        /*
		public static Array Append(Array Origen, object value)
		{
			Array Destino;

			if (Origen == null)
			{
				Type oType = value.GetType();
				Destino = Array.CreateInstance(oType, 1);
				Destino.SetValue(value, 0);
			}
			else
			{
				int nIndex = Origen.Length + 1;

				Type oType = Origen.GetType().GetElementType();
				Destino = Array.CreateInstance(oType, nIndex);

				Origen.CopyTo(Destino, 0);
				Destino.SetValue(value, nIndex-1);
			}
			return Destino;
		}
        */

        /*
		public static object[] Append(object[] Origen, object value)
		{
			object[] Destino;

			if (Origen == null)
			{
				Type oType = value.GetType();
				Destino = (object[])Array.CreateInstance(oType, 1);
				Destino[0] = value;
			}
			else
			{
				int nIndex = Origen.Length + 1;

				Type oType = Origen.GetType().GetElementType();
				Destino = (object[])Array.CreateInstance(oType, nIndex);

				Origen.CopyTo(Destino, 0);
				Destino[nIndex-1] = value;
			}
			return Destino;
		}
        */

        /// <summary> Inserta un nuevo elemento en la posicion dada
        /// Se crea un nuevo array con la longitud incrementada
        /// Se copian los elementos previos y se inserta el nuevo
        /// Si el indice esta fuera del array se ignora la operacion
        /// </summary>
        /// <param name="origen"> Array a modificar     </param>
        /// <param name="index">  Indice donde insertar </param>
        /// <param name="value">  Nuevo valor a añadir  </param>
        /// <returns> Array modificado </returns>

        public static void Insert<T>(ref T[] origen, int index, T value)
        {
            if (origen == null)
                origen = [value];
            else
            {
                int nTotal = origen.Length;

                if (index >= 0 && index <= nTotal)
                {
                    Type oType = origen.GetType().GetElementType();
                    T[] destino = (T[])Array.CreateInstance(oType, nTotal + 1);

                    if (index > 0)
                        Array.Copy(origen, 0, destino, 0, index);

                    if (index < nTotal)
                        Array.Copy(origen, index, destino, index + 1, nTotal - index);

                    destino.SetValue(value, index);
                    origen = destino;
                }
            }
        }

        public static T[] Insert<T>(T[] origen, int index, T value)
        {
            Insert(ref origen, index, value);
            return origen;
        }

        /*
		public static Array Insert(Array origen, int index, object value)
		{
            if (origen == null)
            {
                return Append((object[])origen, value);
            }
            
            int nTotal = origen.Length;

			if (index >= 0 && index <= nTotal)
			{
				Type oType = origen.GetType().GetElementType();
				Array Destino = Array.CreateInstance(oType, nTotal+1);

				if (index > 0)
					Array.Copy(origen, 0, Destino, 0, index);

				if (index < nTotal)
					Array.Copy(origen, index, Destino, index + 1, nTotal - index);

				Destino.SetValue(value, index);

				return Destino;
			}
			return origen;
		}
        */


        /*
		public static object[] Insert(object[] origen, int index, object value)
		{
            if (origen == null)
                return Append(null, value);
            
            int nTotal = origen.Length;

			if (index >= 0 && index <= nTotal)
			{
				Type oType = origen.GetType().GetElementType();
				object[] Destino = (object[])Array.CreateInstance(oType, nTotal+1);

				if (index > 0)
					Array.Copy(origen, 0, Destino, 0, index);

				if (index < nTotal)
					Array.Copy(origen, index, Destino, index + 1, nTotal - index);

				Destino[index] = value;

				return Destino;
			}
			return origen;
		}
         */

        /// <summary> Consulta un elemento de array comprobando acceso
        /// Si el array es nulo o es de tamaño menor devuelve valor nulo
        /// </summary>
        /// <param name="origen"> Array origen a consultar  </param>
        /// <param name="index">  Indice pedido a consultar </param>
        /// <returns> Valor contenido en el indice o nulo   </returns>

        public static object Get(object[] origen, int index)
        {
            if (origen != null && index < origen.Length)
                return origen[index];

            return null;
        }

        /// <summary> Asigna elemento a un array expandiendo si es preciso
        /// Si el array es nulo se crea un nuevo array con el tamaño pedido
        /// Si el tamaño es insuficiente se duplica y copia a un nuevo array
        /// </summary>
        /// <param name="origen"> Array a modificar o expandir </param>
        /// <param name="index">  Numero de indice a modificar </param>
        /// <param name="value">  Valor a incluir en el indice </param>
        /// <returns> Array modificado </returns>

        public static void Put<T>(ref T[] origen, int index, T value)
        {
            if (origen == null)
                origen = new T[index + 1];
            else
            {
                if (index < origen.Length)
                    Resize(ref origen, index + 1);
            }
            origen[index] = value;
        }

        public static T[] Put<T>(T[] origen, int index, T value)
        {
            Put(ref origen, index, value);
            return origen;
        }

        /*
        public static object[] Put(object[] origen, int index, object value)
        {
            if (origen == null || index < origen.Length)
            {
                if (origen == null)
                {
				    Type oType = value.GetType();
				    origen  = (object[])Array.CreateInstance(oType, index);
                }
                else
                    origen = (object[])Resize(origen, index);
            }
            origen[index] = value;
            return origen;
        }
        */

        /// <summary> Comprueba y aumenta al tamaño de un array 
        /// Se crea un duplicado del array si el tamaño es mayor
        /// </summary>
		/// <param name="origen"> Array a expandir       </param>
		/// <param name="size">   Nuevo tamaño del array </param>
        /// <returns> Array original o array modificado  </returns>

        public static void Resize<T>(ref T[] origen, int size)
        {
            if (origen != null && size != origen.Length)
            {
                Array.Resize(ref origen, size);

                /*
				Type oType = origen.GetType().GetElementType();
				Array destino = Array.CreateInstance(oType, size);

                if (size > 0)
                    Array.Copy(origen, destino, size);
                */
            }
        }

        public static T[] Resize<T>(T[] origen, int size)
        {
            Resize(ref origen, size);
            return origen;
        }

        /*
        public static Array Resize(Array origen, int size)
        { 
            if (origen != null && size > origen.Length)
            {
				Type oType = origen.GetType().GetElementType();
				Array destino = Array.CreateInstance(oType, size);
				origen.CopyTo(destino, 0);

                return destino;
            }
            return origen;
        }
        */

        /// <summary> Borra un elemento de un array en el indice dado
        /// Se crea un nuevo array con los elementos restantes
        /// Si el indice esta fuera del array se ignora la operacion
        /// </summary>
        /// <param name="origen"> Array a modificar   </param>
        /// <param name="index"> Indice donde borrar </param>
        /// <returns> Array modificado </returns>

        public static void Delete<T>(ref T[] origen, int index)
        {
            int nTotal = origen.Length;

            if (index >= 0 && index <= nTotal)
            {
                T[] destino = new T[nTotal - 1];

                if (index > 0)
                    Array.Copy(origen, 0, destino, 0, index);

                if (index < nTotal)
                    Array.Copy(origen, index + 1, destino, index, nTotal - index - 1);

                origen = destino;
            }

        }

        public static void Delete<T>(ref T[] origen, int index, int count)
        {
            int nTotal = origen.Length;

            if (index >= 0 && index < nTotal)
            {
                if (index + count > nTotal)
                    count = nTotal - index;

                T[] destino = new T[nTotal - count];

                if (index > 0)       // Copia elementos previos al indice de borrado
                    Array.Copy(origen, 0, destino, 0, index);

                if (index + count < nTotal)  // Copia elementos posteriores a los borrados
                    Array.Copy(origen, index + count, destino, index, nTotal - index - count);

                origen = destino;
            }
        }


        public static T[] Delete<T>(T[] origen, int index)
        {
            Delete(ref origen, index);
            return origen;
        }

        /*
		public static Array Delete(Array Origen, int nIndex)
		{
			int nTotal = Origen.Length;

			if (nIndex >= 0 && nIndex <= nTotal)
			{
				Type oType = Origen.GetType().GetElementType();
				Array destino = Array.CreateInstance(oType, nTotal-1);

				if (nIndex > 0)
					Array.Copy(Origen, 0, destino, 0, nIndex);

				if (nIndex < nTotal)
					Array.Copy(Origen, nIndex+1, destino, nIndex, nTotal-nIndex-1);

                return destino;
			}

			return Origen;
		}
         */

        /// <summary> Borra un elemento especificado de un array
        /// </summary>
		/// <param name="origen"> Array a modificar </param>
        /// <param name="item">   Elemento a borrar </param>
        /// <returns> Array modificado </returns>

        public static void Delete<T>(ref T[] origen, object item)
        {
            if (origen != null)
            {
                int index = Array.IndexOf(origen, item);

                if (index >= 0)
                    Delete(ref origen, index);
            }
        }

        public static T[] Delete<T>(T[] origen, object item)
        {
            Delete(ref origen, item);
            return origen;
        }

        /*
        public static Array Delete(Array origen, object item)
        {
            if (origen != null)
            {
                int index = Array.IndexOf(origen, item);

                if (index >= 0)
                    origen = Delete(origen, index);
            }
            return origen;
        }
        */

        /// <summary> Comprueba si un array contiene el objeto indicado
        /// Admite que el array dado sea nulo y cuyo caso retorna false
        /// </summary>
        /// <param name="ArraySearch">Array donde se quiere localizar un dato</param>
        /// <param name="KeySearch">Dato que quiere buscarse en el array</param>
        /// <returns> Resultado logico de la busqueda </returns>

        public static bool Contains(Array Origen, object value)
        {
            if (Origen == null)
                return false;
            else
                return Array.IndexOf(Origen, value) >= 0;
        }

        /// <summary> Comprueba la igualdad de dos arrays de objetos
        /// Son iguales si ambos son nulos o tienen los mismos valores
        /// </summary>
        /// <param name="Array1"> Primer array a comparar  </param>
        /// <param name="Array2"> Segundo array a comparar </param>
        /// <returns> Resultado logico de la comparacion </returns>

        static public bool Equals(Array Array1, Array Array2)
        {
            if (Array1 != Array2)
            {
                if (Array1 == null || Array2 == null)
                    return false;

                int count = Array1.Length;

                if (count != Array2.Length)
                    return false;

                for (int index = 0; index < count; index++)
                {
                    object dato1 = Array1.GetValue(index);
                    object dato2 = Array2.GetValue(index);

                    if (dato1 != dato2)
                    {
                        if (dato1 != null)
                        {
                            if (dato1 is not IComparable comp || comp.CompareTo(dato2) != 0)
                                return false;
                        }
                        else
                        {
                            if (dato2 != null)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary> Compara dos arrays de objetos comparables (IComparable)
        /// Si el primer array es mayor retorna 1.
        /// Si el primer array es menor retorna -1.
        /// Si los arrays son iguales retorna 0.
        /// Este metodo admite nulos en los arrays o en los elementos
        /// Un valor nulo se considera menor que cualquier otro valor
        /// </summary>
        /// <param name="array1">Primer array a comparar</param>
        /// <param name="array2">Segundo array a comparar</param>
        /// <returns> Resultado de la comparacion </returns>

        static public int Compare(Array array1, Array array2)
        {
            int count = array1 == null ? 0 : array1.Length;

            return Compare(array1, array2, 0, count, StringComparison.Ordinal);
        }

        static public int Compare(Array array1, Array array2, StringComparison type)
        {
            int count = array1 == null ? 0 : array1.Length;

            return Compare(array1, array2, 0, count, type);
        }

        /// <summary> Compara un rango de elementos de dos arrays
        /// Los elementos deben tener el interfaz IComparable
        /// Si el primer array es mayor retorna 1.
        /// Si el primer array es menor retorna -1.
        /// Si los arrays son iguales retorna 0.
        /// Este metodo admite nulos en los arrays o en los elementos
        /// Un valor nulo se considera menor que cualquier otro valor
        /// </summary>
        /// <param name="array1"> Primer array a comparar   </param>
        /// <param name="array2"> Segundo array a comparar  </param>
        /// <param name="index">  Indice inicial a comparar </param>
        /// <param name="count">  Numero de elementos a comparar </param>
        /// <returns> Resultado de la comparacion </returns>

        static public int Compare(Array array1, Array array2, int index, int count, StringComparison type)
        {
            // Comprobar primero si se dan ambos arrays
            if (array1 == null)
            {
                if (array2 == null)
                    return 0;
                else
                    return -1;
            }

            if (array2 == null)
                return 1;

            // Comprobar limites de longitud de cada arrays
            int last = index + count - 1;
            int resul = 0;

            if (last >= array1.Length)
            {
                last = array1.Length - 1;
                resul = -1;
            }

            if (last >= array2.Length)
            {
                last = array2.Length - 1;
                resul = 1;
            }

            // Comparar cada elemento chequeando nulos
            while (index <= last)
            {
                object dato1 = array1.GetValue(index);
                object dato2 = array2.GetValue(index++);

                if (dato1 != dato2)
                {
                    if (dato1 == null || dato2 == null)
                    {
                        if (dato1 == null)
                            return -1;

                        if (dato2 == null)
                            return 1;
                    }
                    else
                    {
                        int comp;

                        if (dato1 is string)
                            comp = string.Compare((string)dato1, (string)dato2, type);
                        else
                        {
                            try
                            {
                                comp = ((IComparable)dato1).CompareTo(dato2);
                            }
                            catch
                            {
                                comp = -1;
                            }
                        }

                        if (comp != 0)
                            return comp > 0 ? 1 : -1;
                    }
                }
            }

            return resul;
        }

        /// <summary> Comprueba si un array esta dentro del rango dado
        /// Si un rango es nulo o vacio se considera menor a otro valor 
        /// </summary>
        /// <param name="Value">  Valor para comprobar rango </param>
        /// <param name="Range1"> Limite inferior del rango  </param>
        /// <param name="Range2"> Limite superior del rango  </param>
        /// <returns> Resultado logico de la comprobacion    </returns>

        static public bool Range(Array value, Array range1, Array range2)
        {
            return Compare(value, range1) >= 0 &&
                    Compare(value, range2) <= 0;
        }

        /// <summary> Comprueba si un array esta dentro del rango dado
        /// Si un rango es nulo o vacio se considera menor a otro valor 
        /// Este metodo especifica el criterio de comparacion a utilizar
        /// </summary>
        /// <param name="Value">  Valor para comprobar rango   </param>
        /// <param name="Range1"> Limite inferior del rango    </param>
        /// <param name="Range2"> Limite superior del rango    </param>
        /// <param name="type">   Tipo de comparacion de texto </param>
        /// <returns> Resultado logico de la comprobacion    </returns>

        static public bool Range(Array value, Array range1, Array range2, StringComparison type)
        {
            return Compare(value, range1, type) >= 0 &&
                    Compare(value, range2, type) <= 0;
        }

        /// <summary> Comprueba si un valor esta dentro del rango dado
        /// El valor a comprobar puede ser un array o un valor simple 
        /// El valor simple se compara con el primer elemento del array
        /// Si un rango es nulo o vacio se considera menor a otro valor 
        /// </summary>
        /// <param name="Value">  Valor para comprobar rango </param>
        /// <param name="Range1"> Limite inferior del rango  </param>
        /// <param name="Range2"> Limite superior del rango  </param>
        /// <returns> Resultado logico de la comprobacion    </returns>

        static public bool Range(object value, Array range1, Array range2)
        {
            if (value is Array)
            {
                return Compare(value as Array, range1) >= 0 &&
                        Compare(value as Array, range2) <= 0;
            }
            else
            {
                // Comparar objeto con el primer elemento del rango
                object dato1 = null;
                object dato2 = null;

                if (range1 != null && range1.Length > 0)
                    dato1 = range1.GetValue(0);

                if (range2 != null && range2.Length > 0)
                    dato2 = range2.GetValue(0);

                return Data.Range(value, dato1, dato2);
            }
        }

        /// <summary> Comprueba si un array esta dentro del rango dado
        /// Si uno de los limites es nulo se da por valido este limite
        /// Soporta criterio de filtro donde el limite vacio se ignora
        /// </summary>
        /// <param name="Value">  Valor para comprobar rango </param>
        /// <param name="Range1"> Limite inferior del rango  </param>
        /// <param name="Range2"> Limite superior del rango  </param>
        /// <returns> Resultado logico de la comprobacion    </returns>

        static public bool Filter(Array value, Array range1, Array range2)
        {
            bool Resul = true;

            if (Compare(value, range1) < 0)
                Resul = false;
            else
            {
                if (range2 != null && Compare(value, range2) > 0)
                    Resul = false;
            }
            return Resul;
        }

        /// <summary> Comprueba si un objeto esta dentro del rango dado
        /// Si uno de los limites es nulo se da por valido este limite
        /// Soporta criterio de filtro donde el limite vacio se ignora
        /// </summary>
        /// <param name="Value">  Valor para comprobar rango </param>
        /// <param name="Range1"> Limite inferior del rango  </param>
        /// <param name="Range2"> Limite superior del rango  </param>
        /// <returns> Resultado logico de la comprobacion    </returns>

        static public bool Filter(object value, Array range1, Array range2)
        {
            if (value is Array)
            {
                return Filter(value as Array, range1, range2);
            }
            else
            {
                // Comparar objeto con el primer elemento del rango
                object dato1 = null;
                object dato2 = null;

                if (range1 != null && range1.Length > 0)
                    dato1 = range1.GetValue(0);

                if (range2 != null && range2.Length > 0)
                    dato2 = range2.GetValue(0);

                return Data.Filter(value, dato1, dato2);
            }
        }

        /// <summary> Copia o crea un array a partir de otro array
        /// Si el destino no esta definido duplica el array origen
        /// Si tienen distinta longitud tambien se duplica el array
        /// Si el destino tiene igual longitud se copian los elementos
        /// </summary>
        /// <param name="Origen">  Array origen  </param>
        /// <param name="Destino"> Array destino </param>
        /// <returns> Array creado o copiado </returns>

        public static void Copy<T>(T[] Origen, ref T[] Destino)
        {
            if (Origen != null)
            {
                if (Destino == null)
                    Destino = (T[])Origen.Clone();
                else
                {
                    if (Destino.Length < Origen.Length)
                        Resize(ref Destino, Origen.Length);
                    else
                        Array.Copy(Origen, Destino, Origen.Length);
                }
            }
        }

        public static T[] Copy<T>(T[] Origen, T[] Destino)
        {
            Copy(Origen, ref Destino);

            return Destino;
        }

        public static T[] Copy<T>(Array Origen, T[] Destino)
        {
            if (Origen != null)
            {
                if (Destino == null)
                    Destino = new T[Origen.Length];
                else
                {
                    if (Destino.Length < Origen.Length)
                        Resize(ref Destino, Origen.Length);
                }

                Array.Copy(Origen, Destino, Origen.Length);
            }
            return Destino;
        }

        /// <summary> Comprueba si un array es nulo o con valores nulos
        /// </summary>
        /// <param name="origen"> Array a comprobar </param>
        /// <returns> Indica que el array es nulo   </returns>

        public static bool IsNull<T>(T[] origen)
        {
            if (origen != null)
            {
                int total = origen.Length;

                for (int ind = 0; ind < total; ind++)
                {
                    if (origen[ind] != null)
                        return false;
                }
            }
            return true;
        }

        /// <summary> Comprueba si un array es nulo o con valores vacios
        /// Los valores vacios incluyen tambien cadenas y fechas vacias
        /// No se consideran vacios los tipos numericos con valor cero
        /// </summary>
        /// <param name="origen"> Array a comprobar  </param>
        /// <returns> Indica que el array esta vacio </returns>

        public static bool IsEmpty<T>(T[] origen)
        {
            if (origen != null)
            {
                int total = origen.Length;

                for (int ind = 0; ind < total; ind++)
                {
                    T value = origen[ind];

                    if (value != null)
                    {
                        if (value is string)
                        {
                            string str = value as string;
                            if (str != string.Empty)
                                return false;
                        }
                        else
                        {
                            if (value is DateTime)
                            {
                                DateTime time = (DateTime)(object)value;
                                if (time != DateTime.MinValue)
                                    return false;
                            }
                            else
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        public static ArrScan Scan(Array List)
        {
            return new ArrScan(List);
        }
    }

    #region ENUMERACION DE ARRAYS MULTIDIMENSIONALES

    public class ArrScan : IEnumerable, IEnumerator
    {
        readonly Array List;
        readonly int NumDims;
        readonly int Length2;
        readonly int Length3;
        int Index;
        readonly Type ItemType;

        public ArrScan(Array list)
        {
            List = list;
            NumDims = List.Rank;
            ItemType = List.GetType().GetElementType();

            if (NumDims == 2)
                Length2 = List.GetLength(1);
            else
            {
                if (NumDims == 3)
                    Length3 = List.GetLength(2);
            }

            Reset();
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        public void Reset()
        {
            Index = -1;
        }

        public bool MoveNext()
        {
            bool resul = Index < List.GetLength(0) - 1;

            if (resul)
                Index++;

            return resul;
        }

        public object Current
        {
            get
            {
                Array resul = null;

                switch (NumDims)
                {
                    case 2:
                        resul = Array.CreateInstance(ItemType, Length2);
                        break;

                    case 3:
                        resul = Array.CreateInstance(ItemType, Length2, Length3);
                        break;
                }

                for (int ind2 = 0; ind2 < Length2; ind2++)
                {
                    object value;

                    switch (NumDims)
                    {
                        case 2:
                            value = List.GetValue(Index, ind2);
                            resul.SetValue(value, ind2);
                            break;

                        case 3:
                            for (int ind3 = 0; ind3 < Length3; ind3++)
                            {
                                value = List.GetValue(Index, ind2, ind3);
                                resul.SetValue(value, ind2, ind3);
                            }
                            break;
                    }
                }

                return resul;
            }
        }
    }



    #endregion

}
