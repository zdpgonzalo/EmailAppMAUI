using MailAppMAUI.DTOs;
using MailAppMAUI.DTOs;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MailAppMAUI.General
{
    /// <summary> Conversiones de datos genericas de la aplicacion
    /// </summary>

    public enum TypeBase
    {
        Indef = 0,
        Bool = 0x1,
        String = 0x2,
        Date = 0x4,
        Object = 0x8,

        Integer = 0x10,
        Decimal = 0x100,
        Numeric = 0x110,
    }

    public static class Data
    {
        #region DEFINICION DE LA CLASE

        static readonly char DecimalSepar;   // Separador decimal del sistema
        static readonly char DataSepar;      // Separador decimal de datos
        static readonly char GroupSepar;     // Separador de grupos de millar
        static readonly bool IsEurDate;      // Indicacion de formato europeo

        static Data()
        {
            // Inicializa separadores numericos segun sistema
            if (Convert.ToDecimal("1.1") == 11)
            {
                // Sistema Europeo: Decimales con coma
                DecimalSepar = ',';
                GroupSepar = '.';
            }
            else
            {
                // Sistema Americano: Decimales con punto
                DecimalSepar = '.';
                GroupSepar = ',';
            }

            // Para datos se usa siempre el separador punto
            DataSepar = '.';

            // Comprobar tipo de formato de hora del sistema
            try
            {
                DateTime.Parse("31/01/01");

                // No hay error: Formato europeo (dd/MM/yy)
                IsEurDate = true;
            }
            catch
            {
                // Error: Formato amnericano (MM/dd/yy)
                IsEurDate = false;
            }
        }

        #endregion

        #region COMPARACION Y RANGO DE VALORES

        /// <summary> Comparacion de valor de dos objetos
        /// Se devuelve siempre una comparacion del valor
        /// </summary>
        /// <param name="Value1"> Valor a comparar </param>
        /// <param name="Value2"> Valor a comparar </param>
        /// <returns> Resulado logico de la comparación </returns>
        /// <remarks>
        /// Este metodo tiene como ventaja que gestiona valores nulos
        /// El metodo Equals permite tambien una comparacion correcta
        /// Para tipos valor da siempre una comparacion de valores
        /// Para cadenas retorna true si las cademas son iguales
        /// </remarks>

        public static new bool Equals(object Value1, object Value2)
        {
            if (Value1 == null || Value2 == null)
            {
                if (Value1 == null)
                {
                    if (Value2 == null)
                        return true;
                    else
                        return IsEmpty(Value2);
                }
                else
                    return IsEmpty(Value1);
            }

            if (Value1 is string)
            {
                if (Value2 is not string)
                    Value2 = Value2.ToString();

                return string.Compare(Value1 as string,
                                     Value2 as string, true) == 0;
            }

            return Value1.Equals(Value2);
        }

        /// <summary> Comparacion de comienzo de una cadena por otra
        /// Si los objetos son cadenas comprueba comienzo de estas
        /// Si los objetos no son cadenas hace la comparacion normal
        /// </summary>
        /// <param name="Value1"> Valor a comparar </param>
        /// <param name="Value2"> Valor a comparar </param>
        /// <returns> Resultado logico de la comparación </returns>

        public static bool Starts(object Value1, object Value2)
        {
            if (Value1 == null || Value2 == null)
            {
                if (Value1 == null)
                {
                    if (Value2 == null)
                        return true;
                    else
                        return IsEmpty(Value2);
                }
                else
                    return IsEmpty(Value1);
            }

            if (Value1 is string)
            {
                string Text1 = Value1 as string;
                string Text2 = Value2 as string;

                int index1 = Str.IndexTrimEnd(Text1);
                int index2 = Str.IndexTrimEnd(Text2);

                if (index1 == index2)
                {
                    if (index1 <= -1)
                        return true;
                    else
                        return string.Compare(Text1, 0, Text2, 0, index1 + 1) == 0;
                }
                return false;
                // return Str.Starts(Value1 as string, Value2 as string);
            }
            else
                return Value1.Equals(Value2);
        }

        /// <summary> Comparacion entre dos objetos cualquiera
        /// Los elementos deben tener el interfaz IComparable
        /// Si el primer objeto es es mayor retorna 1.
        /// Si el primer objeto es menor retorna -1.
        /// Si los dos objetos son iguales retorna 0.
        /// Este metodo admite nulos en cualquiera de los objetos
        /// Un valor nulo se considera menor que cualquier otro valor
        /// </summary>
        /// <param name="dato1"> Primer objeto a comparar  </param>
        /// <param name="dato2"> Segundo objeto a comparar </param>
        /// <returns></returns>

        public static int Compare(object value1, object value2)
        {
            if (value1 != value2)
            {
                if (value1 == null || value2 == null)
                {
                    if (value1 == null)
                        return -1;

                    if (value2 == null)
                        return 1;
                }
                else
                {
                    int resul;

                    if (value1 is IComparable comp)
                    {
                        resul = comp.CompareTo(value2 as IComparable);
                        if (resul != 0)
                            return resul > 0 ? 1 : -1;
                    }
                    else
                    {
                        // Objetos no comparables (No implementan IComparable)
                        // Deteminar solo si son iguales con el operador igual
                        // Si no se sobrecarga el operador compara la instancia
                        if (value1 == value2)
                            resul = 0;
                        else
                            resul = -1;

                        return resul;
                    }
                }
            }
            return 0;
        }

        /// <summary> Comprueba si un valor esat dentro de un rango dado
        /// Si un rango es nulo o vacio se considera menor a otro valor
        /// Este metodo admite nulos en cualquiera de los objetos
        /// Un valor nulo se considera menor que cualquier otro valor
        /// </summary>
        /// <param name="value">  Valor a comprobar en el rango  </param>
        /// <param name="range1"> Valor limite inicial del rango </param>
        /// <param name="range2"> Valor limite final del rango   </param>
        /// <returns> Resulado logico de la comparacion </returns>

        public static bool Range(object value, object range1, object range2)
        {
            return Compare(value, range1) >= 0 &&
                    Compare(value, range2) <= 0;
        }

        /// <summary> Comprueba si un valor esta dentro de un filtro dado
        /// Este metodo considera que cumplen los limites vacios o nulos
        /// Soporta el criterio de filtro donde el limite vacio se ignora
        /// Este metodo admite nulos en cualquiera de los objetos
        /// Un valor nulo se considera menor que cualquier otro valor
        /// </summary>
        /// <param name="value">  Valor a comprobar en el filtro  </param>
        /// <param name="range1"> Valor limite inicial del filtro </param>
        /// <param name="range2"> Valor limite final del filtro   </param>
        /// <returns> Resulado logico de la comparacion </returns>

        public static bool Filter(object value, object range1, object range2)
        {
            return Compare(value, range1) >= 0 &&
                   (Compare(value, range2) <= 0 || IsEmpty(range2));
        }

        #endregion

        #region CONVERSION Y FORMATEO DE DE VALORES

        /// <summary> Conversion por defecto de un valor como cadena
        /// Retorna cadena de texto equivalente segun tipo de valor
        /// </summary>
        /// <param name="Value"> Valor a convertir a cadena </param>
        /// <returns> Cadena con el valor convertido </returns>

        public static string ToString(object Value)
        {
            return ToString(Value, null, TypeCode.Empty);
        }

        /// <summary> Formateo por defecto de un valor como cadena
        /// Retorna cadena de texto convertida y con formateo aplicado
        /// </summary>
        /// <param name="Value"> Valor a convertir a cadena </param>
        /// <returns> Cadena con el valor convertido </returns>

        public static string ToString(object Value, string format)
        {
            return ToString(Value, format, TypeCode.Empty);
        }

        /// <summary> Convierte un valor a cadena formateada
        /// Por defecto utiliza la definicion de la variable
        /// Si se da cadena de formateo formateo segun esta
        /// Acepta un array de objeto para formateo multiple
        /// </summary>
        /// <param name="Value">  Valor a convertir  </param>
        /// <param name="Format"> Formato especifico </param>
        /// <param name="nType">   Id del tipo   </param>
        /// <returns> Cadena de texto formateada </returns>

        public static string ToString(object Value, string Format, TypeCode nType)
        {
            string Resul;

            if (Value == null)
                return null;
            // return String.Empty;

            if (Format != null && Format.LastIndexOf('{') > 0)
            {
                // Se pasa cadena con formato multiple standard
                Resul = string.Format(Format, Value);
            }
            else
            {
                // Formato por defecto segun tipo de dato
                TypeBase nBase;
                if (nType == TypeCode.Empty)
                    nBase = GetTypeBase(Value);
                else
                    nBase = GetTypeBase(nType);

                // int nTotal = DatSize;

                switch (nBase)
                {
                    case TypeBase.Bool:
                        Resul = (bool)Value ? "true" : "false";
                        break;

                    case TypeBase.Decimal:
                        double nVal = Convert.ToDouble(Value);
                        Resul = string.Format(NumFormat(Format), nVal);
                        break;

                    case TypeBase.Integer:
                        if (string.IsNullOrEmpty(Format))
                            Format = "D";

                        long nLon = Convert.ToInt64(Value);
                        Resul = string.Format(NumFormat(Format), nLon);
                        break;

                    case TypeBase.String:
                        Resul = Value.ToString();
                        break;

                    case TypeBase.Date:
                        DateTime Date = (DateTime)Value;
                        if (Date == DateTime.MinValue)
                            Resul = "";
                        else
                        {
                            if (!Str.Empty(Format))
                                Resul = Date.ToString(Format);
                            else
                                Resul = Date.ToString("dd/MM/yy");
                        }
                        break;

                    default:
                        Resul = Value.ToString();
                        break;
                }
            }

            return Resul;
        }

        /// <summary> Convierte un valor en una cadena de datos
        /// La conversion se hace sin influir la configuracion
        /// Se genera un formato fijo segun cada tipo de datos
        /// </summary>
        /// <param name="Value"> Valor a convertir </param>
        /// <returns> Cadena convertida </returns>
        /// <remarks>
        /// Se deben centralizar las converisones en una case Conv
        /// Esta clase tendra una enumeracion con tipos de formato
        /// Esta enumeracion debe ser una propiedad de la clase
        /// Segun el enlace de datos se usan criterios distintos
        ///
        /// Para datos numericos
        ///     - Conversion con punto decimal fijo (Siempre punto)
        ///     - Conversion sin separador decimal (Bancarios)
        ///
        /// Para datos de fecha:
        ///     - Conversion con separador DD/MM/YYYY
        ///     - Conversion con separador YYYY/MM/DD
        ///     - Conversion sin separador YYYYMMDD
        /// </remarks>

        public static string ToData(object Value)
        {
            TypeBase nBase = GetTypeBase(Value);
            string Resul;
            if (Value == null)
                Resul = "";
            else
            {
                switch (nBase)
                {
                    case TypeBase.Decimal:
                        double nVal = Convert.ToDouble(Value);
                        Resul = nVal.ToString("F");
                        if (Resul.IndexOf(',') > 0)
                            Resul = Resul.Replace(',', '.');
                        break;

                    case TypeBase.Date:
                        DateTime Date = (DateTime)Value;
                        if (Date == DateTime.MinValue)
                            Resul = "";
                        else
                            Resul = Date.ToString("yyyyMMdd");
                        break;

                    default:
                        Resul = Value.ToString();
                        break;
                }
            }
            return Resul;
        }

        /// <summary> Genera cadena de formato numerico de la variable
        /// Utiliza la longitud y decimale definidos en la variable
        /// Si se pasa un acadena de formato standard la utiliza
        /// Admite formato longitud y decimales separado por un punto
        /// </summary>
        /// <param name="Format"> Cadena de formato opcional </param>
        /// <returns> Cadena completa de formato </returns>

        private static string NumFormat(string Format)
        {
            int nTotal = 1;
            int nDecs;

            if (!string.IsNullOrEmpty(Format))
            {
                if (char.IsDigit(Format, 0))
                {
                    // Comprobar el formato tipo: Lon.Dec
                    int nDot = Format.IndexOf('.');
                    int nSep = Format.IndexOf(';');

                    if (nDot >= 0)
                    {
                        nTotal = Convert.ToInt32(Format[..nDot]);

                        if (nSep > 0)
                            nDecs = Convert.ToInt32(Format.Substring(nDot + 1,
                                                    nSep - nDot - 1));
                        else
                            nDecs = Convert.ToInt32(Format[(nDot + 1)..]);
                    }
                    else
                    {
                        if (nSep > 0)
                            Format = Format[..nSep];

                        nTotal = Convert.ToInt32(Format);
                        nDecs = 0;
                    }

                    Format = "#,###";
                    if (nDecs > 0)
                        Format += ".".PadRight(nDecs + 1, '#');

                    if (nSep > 0)
                        Format += ";";

                }
                Format = "{0," + nTotal.ToString() + ":" + Format + "}";
                // Format = "{0,1:" + Format+"}";
            }
            else
                Format = "{0,1:n}";

            return Format;
        }

        /// <summary> Convierte un texto segun tipo de otro objeto
        /// </summary>
        /// <param name="Text">  Cadena con el valor a convertir </param>
        /// <param name="value"> Objeto con el tipo deseado      </param>
        /// <returns> Objecto resultado de la conversion </returns>

        public static object ToValue(string text, object value)
        {
            return ToValue(text, GetTypeCode(value));
        }

        /// <summary> Convierte un texto en un valor equivalente
        /// </summary>
        /// <param name="Text">  Cadena a convertir      </param>
        /// <param name="nType"> Tipo de valor resultado </param>
        /// <returns> Objecto resultado de la conversion </returns>

        public static object ToValue(string Text, TypeCode nType)
        {
            object Resul = null;

            if (!Str.Empty(Text))
            {
                try
                {
                    switch (nType)
                    {
                        case TypeCode.Boolean:
                            if (string.IsNullOrEmpty(Text))
                                Resul = false;
                            else
                            {
                                if (string.Compare(Text, "si", true) == 0 ||
                                    string.Compare(Text, "1", false) == 0)
                                    Resul = true;
                                else
                                {
                                    if (string.Compare(Text, "no", true) == 0 ||
                                        string.Compare(Text, "0", false) == 0)
                                        Resul = false;
                                    else
                                        Resul = Convert.ToBoolean(Text);
                                }
                            }
                            break;

                        case TypeCode.Byte:
                            Resul = Convert.ToByte(Text);
                            break;

                        case TypeCode.Char:
                            Resul = Convert.ToChar(Text);
                            break;

                        case TypeCode.SByte:
                            Resul = Convert.ToSByte(Text);
                            break;

                        case TypeCode.Int16:
                            Resul = Convert.ToInt16(Text);
                            break;

                        case TypeCode.Int32:
                            Resul = Convert.ToInt32(Text);
                            break;

                        case TypeCode.Int64:
                            Resul = Convert.ToInt64(Text);
                            break;

                        case TypeCode.UInt16:
                            Resul = Convert.ToUInt16(Text);
                            break;

                        case TypeCode.UInt32:
                            Resul = Convert.ToUInt32(Text);
                            break;

                        case TypeCode.UInt64:
                            Resul = Convert.ToUInt64(Text);
                            break;

                        case TypeCode.Decimal:
                            Resul = Convert.ToDecimal(Text);
                            break;

                        case TypeCode.Double:
                            Resul = Convert.ToDouble(Text);
                            break;

                        case TypeCode.Single:
                            Resul = Convert.ToSingle(Text);
                            break;

                        case TypeCode.DateTime:
                            if (IsEurDate)
                                Resul = Convert.ToDateTime(Text);
                            else
                            {
                                int index1 = Text.IndexOf('/');
                                if (index1 < 0)
                                    index1 = Text.IndexOf('-');
                                int index2 = Text.IndexOf(Text[index1], index1 + 1);

                                string date = string.Concat(Text.AsSpan(index1 + 1, index2 - index1), Text.AsSpan()[..index1], Text.AsSpan()[index2..]);
                                Resul = Convert.ToDateTime(date);
                            }
                            break;

                        default:
                            Resul = Text;
                            break;
                    }
                }
                catch (Exception oExc)
                {
                    Logger.LogError(oExc);
                }
            }

            Resul ??= VarDefault(nType);

            return Resul;
        }

        public static DateTime ToDateTime(object value, string format = null)
        {
            if (value == null)
                return DateTime.MinValue;

            string text = ((string)value).Trim();
            DateTime date = DateTime.MinValue;

            char cSepar = '/';
            int index1 = text.IndexOf(cSepar);

            if (index1 < 0)
            {
                cSepar = '-';
                index1 = text.IndexOf(cSepar);
            }

            if (!string.IsNullOrEmpty(text))
            {
                int index2 = text.IndexOf(cSepar, index1 + 1);
                int index3 = text.IndexOf(' ', index2 + 1);
                int index4 = text.IndexOf(':', index3 + 1);
                int index5 = text.IndexOf(':', index4 + 1);

                if (index2 <= 0)
                    index2 = index3;

                // Coger componentes de la fecha en orden DD-MM-YY
                string month = string.Empty;
                string year = string.Empty;

                try
                {
                    int nDay = ToInt(text[..index1]);
                    int nMonth = DateTime.Now.Month;
                    int nYear = DateTime.Now.Year;
                    int nHour = 0;
                    int nMin = 0;
                    int nSec = 0;

                    if (index2 > 0)
                    {
                        string cMonth = text.Substring(index1 + 1, index2 - index1 - 1);
                        nMonth = ToInt(cMonth);
                    }

                    if (index3 > 0)
                    {
                        if (index3 > index2)
                        {
                            year = text.Substring(index2 + 1, index3 - index2 - 1);
                        }

                        if (index4 > 0)
                        {
                            string min = string.Empty;
                            string hour = text.Substring(index3 + 1, index4 - index3 - 1).Trim();
                            nHour = ToInt(hour);

                            if (index5 > 0)
                            {
                                min = text.Substring(index4 + 1, index5 - index4 - 1).Trim();

                                if (text.Length > index5)
                                {
                                    string sec = text[(index5 + 1)..];
                                    nSec = ToInt(sec);
                                }
                            }
                            else
                            {
                                if (text.Length > index4)
                                    min = text[(index4 + 1)..];
                            }
                            nMin = ToInt(min);

                            if (min.Length >= 4 && min[2] == 'P' && min[3] == 'M')
                                nHour += 12;
                        }
                    }
                    else
                    {
                        year = text[(index2 + 1)..];
                    }
                    nYear = ToInt(year);
                    if (nYear < 2000)
                        nYear += 2000;

                    if (format != null)
                    {
                        if (format[2] == 'D' || nMonth > 12)
                        {
                            if (format[0] == 'M' || nMonth > 12)
                                SwapVar(ref nDay, ref nMonth);  // Fomato MM-DD-YY
                            else
                                SwapVar(ref nDay, ref nYear);   // Fomato YY-DD-MM
                        }
                        else
                        {
                            if (format[4] == 'D')
                            {
                                if (format[0] == 'Y')
                                    SwapVar(ref nDay, ref nYear); // Fomato YY-MM-DD
                                else
                                {
                                    SwapVar(ref nDay, ref nMonth);  // Fomato MM-YY-DD
                                    SwapVar(ref nMonth, ref nYear);
                                }
                            }
                        }
                    }

                    if (!IsEmpty(month) && !char.IsDigit(month[0]))
                    {
                        switch (month.ToLower()[..3])
                        {
                            case "ene":
                            case "jan": nMonth = 1; break;
                            case "feb": nMonth = 2; break;
                            case "mar": nMonth = 3; break;
                            case "abr":
                            case "apr": nMonth = 4; break;
                            case "may": nMonth = 5; break;
                            case "jun": nMonth = 6; break;
                            case "jul": nMonth = 7; break;
                            case "ago":
                            case "aug": nMonth = 8; break;
                            case "sep": nMonth = 9; break;
                            case "oct": nMonth = 10; break;
                            case "nov": nMonth = 11; break;
                            case "dic":
                            case "dec": nMonth = 12; break;
                        }
                    }

                    date = new DateTime(nYear, nMonth, nDay, nHour, nMin, nSec);
                }
                catch (Exception exc)
                {
                    Logger.LogError(exc);
                }
            }

            return date;
        }

        private static void SwapVar(ref int var1, ref int var2)
        {
            (var2, var1) = (var1, var2);
        }

        public static float ToFloat(object value)
        {
            float result = 0;

            if (value != null)
            {
                if (value is string)
                {
                    string text = value as string;

                    if (Str.IsNumber(text))
                    {
                        int nDecimal = text.IndexOf(DecimalSepar);
                        int nGroup = text.IndexOf(GroupSepar);

                        if (nGroup >= 0 && nDecimal < 0)
                        {
                            // Se usa el separador al reves: Cambiarlo
                            text = text.Replace(GroupSepar, DecimalSepar);
                        }
                        else
                        {
                            // Dejar solo el separador decimal para convertir
                            if (nGroup > 0 && nDecimal > 0)
                                text = text.Replace(GroupSepar.ToString(), string.Empty);
                        }
                        result = Convert.ToSingle(text);
                    }
                }
                else
                {
                    Type type = value.GetType();
                    if (type.IsPrimitive)
                        result = Convert.ToSingle(value);
                }
            }
            return result;
        }


        public static double ToDouble(object value)
        {
            double result = 0;

            if (value != null)
            {
                try
                {
                    if (value is string)
                    {
                        string text = value as string;

                        //Elimina todos los caracteres no numericos y mantiene los (.), (,) y (-)
                        text = Regex.Replace(text, @"[^\d,.-]", "");
                        text.Trim();

                        if (Str.IsNumber(text))
                        {
                            int nDecimal = text.IndexOf(DecimalSepar);
                            int nGroup = text.IndexOf(GroupSepar);

                            if (nGroup >= 0 && nDecimal < 0)
                            {
                                // Se usa el separador al reves: Cambiarlo
                                text = text.Replace(GroupSepar, DecimalSepar);
                            }
                            else
                            {
                                // Dejar solo el separador decimal para convertir
                                if (nGroup > 0 && nDecimal > 0)
                                    text = text.Replace(GroupSepar.ToString(), string.Empty);
                            }
                            result = Convert.ToDouble(text);
                        }
                    }
                    else
                    {
                        Type type = value.GetType();
                        if (type.IsPrimitive)
                            result = Convert.ToDouble(value);
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        public static decimal ToDecimal(object value)
        {
            decimal result = 0;

            if (value != null)
            {
                try
                {
                    if (value is string)
                    {
                        string text = value as string;

                        if (Str.IsNumber(text))
                        {
                            int nDecimal = text.IndexOf(DecimalSepar);
                            int nGroup = text.IndexOf(GroupSepar);

                            if (nGroup >= 0 && nDecimal < 0)
                            {
                                // Se usa el separador al reves: Cambiarlo
                                text = text.Replace(GroupSepar, DecimalSepar);
                            }
                            else
                            {
                                // Dejar solo el separador decimal para convertir
                                if (nGroup > 0 && nDecimal > 0)
                                    text = text.Replace(GroupSepar.ToString(), string.Empty);
                            }
                            result = Convert.ToDecimal(text);
                        }
                    }
                    else
                    {
                        Type type = value.GetType();
                        if (type.IsPrimitive)
                            result = Convert.ToDecimal(value);
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        public static int ToInt(object value)
        {
            int result = 0;

            if (value != null)
            {
                try
                {
                    if (value is string)
                    {
                        string text = Str.GetNumber(value as string);

                        text = Regex.Replace(text, @"[^\d,.-]", "");
                        text.Trim();

                        if (text != string.Empty)
                            result = Convert.ToInt32(text);
                    }
                    if (value is Enum)
                    {
                        result = (int)value;
                    }
                    else
                    {
                        Type type = value.GetType();
                        if (type.IsPrimitive)
                            result = Convert.ToInt32(value);
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        public static bool ToBool(object value)
        {
            bool resul = false;

            if (value != null)
            {
                if (value is string)
                {
                    string Text = value as string;

                    if (string.IsNullOrEmpty(Text))
                        resul = false;
                    else
                    {
                        if (string.Compare(Text, "true", true) == 0 ||
                            string.Compare(Text, "yes", true) == 0 ||
                            string.Compare(Text, "si", true) == 0 ||
                            string.Compare(Text, "1", false) == 0)
                            resul = true;
                        else
                            resul = false;
                    }
                }
                else
                {
                    resul = value is bool ? (bool)value : !IsEmpty(value);
                }
            }
            return resul;
        }

        /// <summary> Convierte un texto en un valor equivalente
        /// </summary>
        /// <param name="Text">  Cadena a convertir      </param>
        /// <param name="nType"> Tipo de valor resultado </param>
        /// <returns> Objecto resultado de la conversion </returns>
        /// <remarks>
        /// Para determinar el tipo de valor se siguen estos criterios
        ///
        /// - Si comienza por comilla simple se convierte a caracter
        ///
        /// - Si comienza por comilla doble se convierte a cadena
        ///
        /// - Si se usa un separador de fechas se convierte como fecha
        ///   Se admiten la barra inclinada y el guion como separador
        ///   No pueden estar en el primer caracter de la cadena
        ///
        /// - Si comienza por digito o por signo se convierte como numero
        ///
        /// - Si no se usa separador Decimal o de Grupo se asume entero
        ///   Por tanto solo se puede formzar a dounle usando separador
        ///
        /// - Si lleva solo separador de grupo se asume qeu es decimal
        ///   Se convierte cambiando el separador de grupo por el decimal
        ///
        /// - Si lleva ambos separadores se quita el separador de grupo
        ///   El separador decimal se deja indicando cifras decimales
        ///
        /// Con este criterio no puede dar error nunca por separadores
        /// Pero se puede hacer una conversion erronea si se usa al reves
        /// Es preferible usar simpre el separador decimal del sistema
        ///
        /// </remarks>

        public static object ToValue(string text)
        {
            if (text == null)
                return null;

            object resul = null;
            int len = text.Length;

            if (len > 0 && char.IsWhiteSpace(text, 0))
            {
                text = text.TrimStart();
                len = text.Length;
            }

            if (len > 0)
            {
                if (text.IndexOf('/') > 0 || text.IndexOf('-') > 0)
                    resul = Convert.ToDateTime(text);
                else
                {
                    char init = text[0];

                    if (char.IsDigit(init) || init == '.' || init == ',' ||
                                              init == '+' || init == '-')
                    {
                        int nDecimal = text.IndexOf(DecimalSepar);
                        int nGroup = text.IndexOf(GroupSepar);

                        if (nGroup < 0 && nDecimal < 0)
                        {
                            // No hay separadores: Se asume un valor entero
                            resul = Convert.ToInt32(text);
                        }
                        else
                        {
                            if (nGroup >= 0 && nDecimal < 0)
                            {
                                // Se usa el separador al reves: Cambiarlo
                                text = text.Replace(GroupSepar, DecimalSepar);
                            }
                            else
                            {
                                // Dejar solo el separador decimnal para convertir
                                if (nGroup > 0)
                                    text = text.Replace(GroupSepar.ToString(), string.Empty);
                            }

                            resul = Convert.ToDouble(text);
                        }
                    }
                    else
                    {
                        if (len >= 2)
                        {
                            if (init == '\'')
                                resul = Convert.ToChar(text.Substring(1, len - 2));
                            else
                            {
                                if (init == '\"')
                                    resul = Convert.ToString(text.Substring(1, len - 2));
                            }
                        }
                    }
                }
            }

            resul ??= text;

            return resul;
        }

        /// <summary> Convierte un objeto con una cadena en su valor
        // </summary>
        /// <param name="value"> Objeto con el valor a convertir </param>
        /// <returns> Objeto con el valor convertido </returns>

        public static object ToValue(object value)
        {
            if (value != null)
            {
                if (value is string)
                    value = ToValue(value as string);
            }
            return value;
        }

        /// <summary> Normaliza una cadena de datos para conversion
        /// </summary>
        /// <param name="text"> Cadena a estandarizar </param>
        /// <returns> Cadena normalizada </returns>
        /// <remarks>
        /// El fomato de cadenas de datos de la aplicacion es fijo
        ///
        ///    - Numero sin separador de millar y con punto decimal
        ///    - Fechas en formato YYYYMMDD sin separador de fecha
        ///    - Fechas en formato DD/MM/YY con separador
        ///
        /// La conversion numerica depende de la configuracion de windows
        /// Este metodo deja el separador del sistema en una cadena de datos
        /// Esta cadena puede ser ya convertida con los metodos starndard
        /// No se debe llamar a este metodo si la cadena no es de datos
        /// Las cadenas usadas en el interfaz siguen la norma del sistema
        /// </remarks>

        public static string DataNorm(string text, TypeBase nBase)
        {
            switch (nBase)
            {
                case TypeBase.Integer:
                    int nIndex = text.IndexOf(DataSepar);

                    if (nIndex > 0)
                    {
                        // Separador decimal en un entero
                        text = text[..nIndex];
                    }
                    break;

                case TypeBase.Decimal:
                    if (text.Contains(DataSepar))
                    {
                        // Separadopr distinto al de datos
                        text = text.Replace(DataSepar, DecimalSepar);
                    }
                    break;

                case TypeBase.Date:
                    if (text.IndexOf('/') < 0)
                    {
                        // Formato sin separador de fechas
                        text = text.Substring(6, 2) + '/' +
                               text.Substring(4, 2) + '/' +
                               text[..4];
                    }
                    break;
            }
            return text;
        }

        /// <summary> Comprueba si el objeto dado esta vacio
        /// </summary>
        /// <param name="value"> Objecto a comprobar </param>
        /// <returns> Indicacion de objeto vacio </returns>

        public static bool IsEmpty<T>(T value)
        {
            if (value == null)
                return true;

            if (value is Array)
            {
                Array values = value as Array;

                int total = values.GetLength(0);

                if (total > 0)
                {
                    for (int ind = 0; ind < total; ind++)
                    {
                        object data = values.GetValue(ind);

                        if (data != null && !IsEmpty(data))
                            return false;
                    }
                }
                return true;
            }
            else
            {
                if (value is string)
                {
                    string str = value as string;
                    return str == string.Empty;
                }

                else
                {
                    if (value is DateTime)
                    {
                        DateTime time = (DateTime)(object)value;
                        return time == DateTime.MinValue;
                    }
                }
                return false;
            }
        }

        /// <summary> Devueleve el valor por defecto del dato dado
        /// Existe una sobrecarga generica y otra para tipos object
        /// </summary>
        /// <typeparam name="T"> Tipo de dato a convertir </typeparam>
        /// <param name="value"> Dato para obtener su valor por defecto </param>
        /// <returns> Valor por defecto del dato </returns>

        public static T VarDefault<T>(T value)
        {
            if (value != null)
            {
                switch (GetTypeBase(value))
                {
                    case TypeBase.Bool:
                        return (T)(object)false;

                    case TypeBase.Integer:
                    case TypeBase.Decimal:
                        return (T)(object)0;

                    case TypeBase.Date:
                        return (T)(object)DateTime.MinValue;
                }
            }

            return default;
        }

        public static object VarDefault(object value)
        {
            if (value != null)
                return VarDefault(value.GetType());

            return null;
        }

        /// <summary> Devuelve valor por defecto del codigo de tipo
        /// Para todos los tipos numericos este valor es cero
        /// Para tipo string el valor por defecto es cadena vacia
        /// Para el tipo DateTime es la hora inicial (año 1 DC)
        /// Para el tipo object el valor por defecto es null
        /// </summary>
        /// <returns> objeto del tipo indicado con su valor por defecto </returns>

        public static object VarDefault(TypeCode nType)
        {
            return nType switch
            {
                TypeCode.String => string.Empty,
                // value = (object)string.Empty;
                // break;
                TypeCode.DateTime => DateTime.MinValue,
                // value = DateTime.MinValue;
                // break;
                TypeCode.Object => null,
                TypeCode.Byte => (byte)0,
                TypeCode.Char => (char)0,
                TypeCode.Int16 => (short)0,
                TypeCode.Int32 => 0,
                TypeCode.Int64 => (long)0,
                TypeCode.Boolean => false,
                _ => Convert.ChangeType(0, nType, null),
            };
            // return Convert.ChangeType(value, nType, null);
        }

        /// <summary> Devuelve el valor por defecto del tipo dado
        /// Para todos los tipos numericos este valor es cero
        /// Para tipo string el valor por defecto es cadena vacia
        /// Para el tipo DateTime es la hora inicial (año 1 DC)
        /// Para el tipo object el valor por defecto es null
        /// </summary>
        /// <returns> objeto del tipo indicado con su valor por defecto </returns>

        public static object VarDefault(Type oType)
        {
            return VarDefault(Type.GetTypeCode(oType));
        }

        #endregion

        #region CONVERSION DE ARRAYS DE BYTES

        /// <summary> Retorna un array de bytes desde un string
        /// No se efectua ninguna conversion ni codificacion
        /// </summary>
        /// <param name="str"> String a convertir </param>
        /// <returns> Array de bytes equivalente </returns>

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary> Retorna un string convertido a array de bytes
        /// No se efectua ninguna conversion ni codificacion
        /// </summary>
        /// <param name="bytes"> Array de bytes a convertir </param>
        /// <param name="start"> Indice inicial a convertir </param>
        /// <param name="count"> Nunero de bytes a procesar </param>
        /// <returns> String equievalente al array </returns>

        public static string GetString(byte[] bytes, int count)
        {
            char[] chars = new char[count / sizeof(char)];

            /*
            string s2 = System.Text.Encoding.Default.GetString(bytes, start, count);
            var x = System.Text.Encoding.Default.CodePage;
            var x2 = System.Text.Encoding.GetEncodings();
            var c850 = System.Text.Encoding.GetEncoding(850);
            var c1252 = System.Text.Encoding.GetEncoding(1252);
            var x4 = c1252.GetString(bytes, start, count);
            var x5 = c850.GetString(bytes, start, count);
            */

            count = chars.Length;
            Buffer.BlockCopy(bytes, 0, chars, 0, count);
            return new string(chars);
        }

        /// <summary> Retorna un string convertido a array de bytes
        /// No se efectua ninguna conversion ni codificacion
        /// </summary>
        /// <param name="bytes"> Array de bytes a convertir </param>
        /// <returns> String equievalente al array </returns>

        public static string GetString(byte[] bytes)
        {
            if (bytes == null)
                return null;

            return GetString(bytes, bytes.Length);
        }

        #endregion

        #region OPERACIONES MATEMATICAS

        /// <summary> Redondeo de numeros con el criterio del euro
        /// Los valores intermedios deben redondearse hacia arriba
        /// </summary>
        /// <param name="value">  Valor a redndear </param>
        /// <param name="numdec"> Nuemro de decimales (2) </param>
        /// <returns> Valor redidneado </returns>
        /// <remarks>
        /// El metodo Round standard tiene el criterio del valor par
        /// Ademas el redondeo con double puede dar valores erroneos
        /// Este metodo redondea internamente en decimal hacia arriba
        /// </remarks>

        public static double Round(double value, int numdec = 2)
        {
            double impdes = (double)Math.Round((decimal)value, numdec, MidpointRounding.AwayFromZero);

            return impdes;
        }

        #endregion

        #region INCREMENTO Y DECREMENTO DE VALORES


        /// <summary> Incrementa un valor segun el tipo de datos
        /// </summary>
        /// <param name="value"> Valor a incrementar </param>
        /// <returns> Resultado incrementado </returns>

        public static object Next(object value, int step)
        {
            if (value == null)
                return null;

            try
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.DateTime:
                        DateTime time = Convert.ToDateTime(value);
                        value = time.AddDays(step);
                        break;

                    case TypeCode.String:
                        if (value is string text)
                        {
                            int total = text.Length;
                            int index = total - 1;

                            while (index >= 0)
                            {
                                if (text[index] < 255)
                                {
                                    text = (text[..index] +
                                           (char)(text[index] + 1)).PadRight(total);
                                    break;
                                }
                                else
                                {
                                    if (index > 0)
                                        index--;
                                    else
                                        break;
                                }
                            }
                            value = text;
                        }
                        break;

                    case TypeCode.Byte:
                        value = Convert.ToByte(value) + step;
                        break;

                    case TypeCode.Char:
                        value = Convert.ToChar(value) + step;
                        break;

                    case TypeCode.Int16:

                        value = Convert.ToInt16(value) + step;
                        break;

                    case TypeCode.Int32:
                        value = Convert.ToInt32(value) + step;
                        break;

                    case TypeCode.Int64:
                        value = Convert.ToInt64(value) + step;
                        break;

                    case TypeCode.UInt16:
                        value = Convert.ToUInt16(value) + step;
                        break;

                    case TypeCode.UInt32:
                        value = Convert.ToUInt32(value) + step;
                        break;

                    case TypeCode.Decimal:
                        value = Convert.ToDecimal(value) + step;
                        break;

                    case TypeCode.Double:
                        value = Convert.ToDouble(value) + step;
                        break;

                    case TypeCode.Single:
                        value = Convert.ToSingle(value) + step;
                        break;
                }
            }
            catch (Exception oExc)
            {
                Logger.LogError(oExc);
            }

            return value;
        }

        public static object Next(object value)
        {
            return Next(value, 1);
        }

        public static object Prev(object value)
        {
            return Next(value, -1);
        }

        #endregion

        #region GESTION DE TIPOS DE VARIABLES

        /// <summary> Devuelve tipo del sistema de un nombre de tipo
        /// </summary>
        /// <returns> Objeto type correspondiente al nombre de tipo </returns>

        public static Type VarType(string cType)
        {
            Type oType = null;

            if (cType != null)
            {
                if (cType.IndexOf('.') < 0)
                    cType = "System." + cType;

                oType = Type.GetType(cType);
            }
            return oType;
        }

        /// <summary> Retorna el codigo de tipo de un valor dado
        /// </summary>
        /// <param name="Value"> Valor a comprobar </param>
        /// <returns> Id de tipo del valor </returns>

        public static TypeCode GetTypeCode(object Value)
        {
            TypeCode nCode;

            if (Value == null)
                nCode = TypeCode.DBNull;
            else
                nCode = Type.GetTypeCode(Value.GetType());

            return nCode;
        }

        /// <summary> Comprueba tipo base de un valor
        /// </summary>
        /// <param name="Value"> Valor a comprobar   </param>
        /// <param name="nBase"> Id de tipo base </param>
        /// <returns> Indica si coincide el tipo base </returns>

        public static bool CheckType(object Value, TypeBase nBase)
        {
            // return (GetTypeBase(Value) == nBase);
            return (GetTypeBase(Value) & nBase) != 0;
        }

        /// <summary> Retorna el tipo base de un valor dado
		/// Esto permite tratamiento por tipos de variable
        /// </summary>
		/// <returns> Tipo base del valor </returns>

        public static TypeBase GetTypeBase(object value)
        {
            if (value == null)
                return TypeBase.Indef;

            return GetTypeBase(Type.GetTypeCode(value.GetType()));
        }

        public static TypeBase GetTypeBase<T>(T value)
        {
            if (value == null)
                return TypeBase.Indef;

            return GetTypeBase(Type.GetTypeCode(value.GetType()));
        }

        /// <summary> Retorna el tipo base de un codigo de tipo
		/// Esto permite tratamiento por tipos de variable
        /// </summary>
		/// <returns> Tipo base de la variable </returns>

        public static TypeBase GetTypeBase(TypeCode nType)
        {
            var nBase = nType switch
            {
                TypeCode.Boolean => TypeBase.Bool,
                TypeCode.Byte or TypeCode.Char or TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 => TypeBase.Integer,
                TypeCode.Decimal or TypeCode.Double or TypeCode.Single => TypeBase.Decimal,
                TypeCode.String => TypeBase.String,
                TypeCode.DateTime => TypeBase.Date,
                TypeCode.Object => TypeBase.Object,
                _ => TypeBase.Indef,
            };
            return nBase;
        }

        public static TypeBase ConvertType(string ctype, object size)
        {
            TypeCode type = GetEnum(ctype, TypeCode.Empty);
            TypeBase tbase = TypeBase.Indef;

            if (type != TypeCode.Empty)
                tbase = GetTypeBase(type);

            if (tbase == TypeBase.Indef && ctype.Length == 1)
            {
                switch (ctype)
                {
                    case "C":
                        tbase = TypeBase.String;
                        break;

                    case "D":
                        tbase = TypeBase.Date;
                        break;

                    case "L":
                        tbase = TypeBase.Bool;
                        break;

                    case "N":
                        tbase = TypeBase.Integer;

                        if (size is string)
                        {
                            if (size.ToString().Contains('.'))
                                tbase = TypeBase.Decimal;
                        }
                        else
                        {
                            if (size is not int)
                                tbase = TypeBase.Decimal;
                        }
                        break;
                }
            }

            return tbase;
        }

        #endregion

        #region METODOS GENERALES DE SOPORTE PUBLICO

        /// <summary> Retorna un parametro de tipo dado como un elemento
        /// Si se da un array del tipo pedido retorna el primer elemento
        /// Si no existe el parametro o es de otro tipo retorna nulo
        /// </summary>
        /// <typeparam name="T"> Tipo del parametro esperado   </typeparam>
        /// <param name="par">   Array de parametros pasados   </param>
        /// <param name="index"> Numero de orden del parametro </param>
        /// <returns> Elemento del tipo dado </returns>

        public static T GetParam<T>(Array par, int index)
        {
            if (par != null && par.Length > index)
            {
                object item = par.GetValue(index);

                if (item is T t)
                    return t;
                else
                {
                    if (item is T[] v)
                    {
                        if (v.Length > 0)
                            return v[0];
                    }
                    else
                    {
                        try
                        {
                            if (typeof(T).IsEnum)
                            {
                                if (item is string)
                                {
                                    string name = item as string;
                                    if (Enum.IsDefined(typeof(T), name))
                                    {
                                        return (T)Enum.Parse(typeof(T), name);
                                    }
                                }
                                else
                                {
                                    return (T)item;
                                }
                            }
                            else
                                return (T)Convert.ChangeType(item, typeof(T));
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return default;
        }

        /// <summary> Retorna un parametro de tipo dado como un elemento
        /// Si se da un array del tipo pedido retorna el primer elemento
        /// Si no existe el parametro o es de otro tipo retorna nulo
        /// </summary>
        /// <typeparam name="T"> Tipo del parametro esperado   </typeparam>
        /// <param name="par">   Array de parametros pasados   </param>
        /// <param name="index"> Numero de orden del parametro </param>
        /// <returns> Elemento del tipo dado </returns>

        public static T GetParam<T>(Array par, Enum option)
        {
            int index = (int)(object)option;

            if (par != null && par.Length > index)
            {
                object item = par.GetValue(index);

                if (item is T t)
                    return t;
                else
                {
                    if (item is T[] v)
                    {
                        if (v.Length > 0)
                            return v[0];
                    }
                    else
                    {
                        try
                        {
                            if (typeof(T).IsEnum)
                                return (T)item;
                            else
                            {
                                if (item != null)
                                    return (T)Convert.ChangeType(item, typeof(T));
                            }
                        }
                        catch (Exception exc)
                        {
                            Logger.LogError(exc);
                        }
                    }
                }
            }

            return default;
        }

        /// <summary> Retorna un array de parametros de un tipo dado
        /// Si se da un elemento simple del tipo pedido crea un array
        /// Si no existe el parametro o es de otro tipo retorna nulo
        /// </summary>
        /// <typeparam name="T"> Tipo del elemento del array   </typeparam>
        /// <param name="par">   Array de parametros pasados   </param>
        /// <param name="index"> Numero de orden del parametro </param>
        /// <returns> Elemento del tipo dado </returns>

        public static T[] GetParams<T>(Array par, int index)
        {
            if (par != null && par.Length > index)
            {
                object item = par.GetValue(index);

                if (item is T[] v)
                    return v;
                else
                {
                    if (item is T t)
                    {
                        return [t];
                    }
                }
            }

            return default;
        }

        /// <summary> Convierte un valor de enumerado en el codigo asociado
        /// Encuentra en valores correctos ignorando matusculas y minusculas
        /// Sin embargo es mas rapido si se da el nombre exacto de enumerado
        /// Si se da una cadena con digitos asume que es un valor pedido
        /// </summary>
        /// <typeparam name="T"> Tipo enumerado a convertir </typeparam>
        /// <param name="value"> Valor a buscar y convertir </param>
        /// <param name="defval">Valor asignado por defecto </param>
        /// <returns> Id de enumerado encontrado o nulo </returns>

        public static T GetEnum<T>(string value, T defval) where T : Enum
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (char.IsDigit(value[0]))
                    return (T)(object)Convert.ToInt32(value);
                else
                {
                    if (Enum.IsDefined(typeof(T), value))
                        return (T)Enum.Parse(typeof(T), value);
                    else
                    {
                        foreach (string name in Enum.GetNames(typeof(T)))
                        {
                            if (name.Equals(value, StringComparison.OrdinalIgnoreCase))
                                return (T)Enum.Parse(typeof(T), value, true);
                        }
                    }
                }
            }

            return defval;
        }

        public static T GetEnum<T>(string value) where T : Enum
        {
            return GetEnum<T>(value, default);
        }

        public static T GetEnums<T>(string enumValues) where T : Enum
        {
            char separ = ',';

            if (enumValues.IndexOf(separ) < 0)
                separ = ' ';

            if (enumValues.IndexOf(separ) > 0)
            {
                int value = 0;

                foreach (string key in Str.Scan(enumValues, separ))
                {
                    if (Enum.IsDefined(typeof(T), key))
                        value += (int)Enum.Parse(typeof(T), key);
                }

                return (T)(object)value;
            }
            else
                return GetEnum<T>(enumValues);
        }

        public static IEnumerable<T> GetEnums<T>() where T : struct
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }

        public static T GetEnum<T>(object enumValue) where T : Enum
        {
            if (enumValue is string)
                return GetEnum<T>(enumValue as string, default);
            else
            {
                if (enumValue != null && enumValue.GetType().IsEnum)
                    return (T)(object)Convert.ToInt32(enumValue);
            }
            return default;
        }

        public static string GetName<T>(T value) where T : struct
        {
            return Enum.GetName(typeof(T), value);
        }

        public static bool IsDefined<T>(string key) where T : Enum
        {
            if (string.IsNullOrEmpty(key))
                return false;

            return Enum.IsDefined(typeof(T), key);
        }


        /// <summary> Retorna los nombre publicos del tipo dado
        /// Para el caso de enum se puede indicar el primer campo
        /// Esto permite saltar el valor por defecto del enum
        /// </summary>
        /// <param name="enumTyp"> Tipo a consultar </param>
        /// <param name="offset">  Numero de campo inicial </param>
        /// <returns> Array de nombres </returns>

        public static string[] GetFields(Type enumTyp, int offset)
        {
            string[] names = null;

            try
            {
                FieldInfo[] fields = enumTyp.GetFields(BindingFlags.Public |
                                                       BindingFlags.Static);

                int total = fields.Length;
                if (total > offset)
                {
                    names = new string[total - offset];

                    for (int index = offset; index < total; index++)
                    {
                        names[index - offset] = fields[index].Name;
                    }
                }
            }
            catch
            {
            }

            return names;
        }

        /// <summary> Comparacion de enumerado con valor mascara
        /// Permiete mezclar enuemrados y enteros libremente
        /// </summary>
        /// <param name="value"> Valor a comprobar </param>
        /// <param name="mask">  Maascar para and  </param>
        /// <returns> El valor contiene la mascara </returns>

        static public bool Check(object value, object mask)
        {
            int val = ToInt(value);
            int msk = ToInt(mask);

            return (val & msk) == msk;
        }

        static public bool Check(Enum value, Enum mask)
        {
            int val = (int)(object)value;
            int msk = (int)(object)mask;

            return (val & msk) == msk;
        }


        /// <summary> Retorna par clave valor del string dado
        /// Permite analizar los parametros de un programa
        /// </summary>
        /// <param name="option"> Cadena a analizar </param>
        /// <returns> Pareja clave valor </returns>

        static public (string key, string value) GetKeyValue(string option)
        {
            string key = null;
            string value = null;

            if (!string.IsNullOrWhiteSpace(option))
            {
                int index1 = option.IndexOf('=');
                int index2 = option.IndexOf(':');

                if (index1 < 0 || index2 > 0 && index2 < index1)
                    index1 = index2;

                if (index1 < 0 && index2 < 0)   // No hay separadores 
                    index1 = option.IndexOf(' '); // Usar espacio por defecto

                if (index1 > 0)
                {
                    key = option[..index1].Trim();
                    if (index1 < option.Length - 1)
                        value = option[(index1 + 1)..].Trim();
                }
            }

            return (key, value);
        }

        #endregion

        #region NORMALIZACION DE VALORES

        /// <summary> Normaliza un texto con la primera en mayusculas
        /// </summary>
        /// <param name="text"> Texto a estandarizar </param>
        /// <returns> texto normalizado  </returns>

        public static string NormText(string text)
        {
            if (text != null)
            {
                text = text.Trim();

                if (text.Length > 1)
                {
                    text = char.ToUpper(text[0]) + text[1..].ToLower();
                }
                else
                {
                    if (text.Length > 0)
                        text = text.ToUpper();
                }
            }

            return text;
        }

        /// <summary> Normaliza un nombre de variable
        /// </summary>
        /// <param name="nomvar"> Nombre de variable </param>
        /// <returns> Variable normalizada </returns>
        public static string NormVariable(string nomvar)
        {
            int index = nomvar.IndexOf('.');
            string resul;

            if (index <= 0)
                index = nomvar.IndexOf('_');

            if (index > 0)
            {
                resul = char.ToUpper(nomvar[0]) +
                        nomvar[1..index].ToLower() + '_' +
                        char.ToUpper(nomvar[index + 1]) +
                        nomvar[(index + 2)..].ToLower();
            }
            else
            {
                resul = char.ToUpper(nomvar[0]) +
                        nomvar[1..].ToLower();
            }

            return resul;
        }
        #endregion

        #region NORMALIZACION DE IDENTIFICADORES

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
        #endregion

    }
}
