using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

using Ifs.Comun;

namespace Ifs.ComInter
{
    #region Defincion de tipos y longitudes de datos

    internal enum InterTypes: byte
    {
        Byte     = 0x001,
        Short    = 0x002,
        Integer  = 0x003,
        Long     = 0x004,
        Double   = 0x005,
        Logical  = 0x006,
        String   = 0x007,
        Date     = 0x009,
        Datetime = 0x00A,
        Pointer  = 0x00B,
        Null     = 0x00F,
        Array    = 0x010,
        Object   = 0x020,
        Hash     = 0x030,
    }

    internal enum InterSizes
    {
        Byte     =  1,
        Short    =  2,
        Integer  =  4,
        Long     =  4,
        Double   =  8,
        Logical  =  1,
        String   =  4,
        Date     =  8,
        Datetime =  8,
        Pointer  =  8,
        Null     =  0,
        Array    =  4,
        Object   =  8,
        Hash     =  12,

        // Header   =  4, // Longitud de la cabecera
        Header   =  8,    // Longitud de la cabecera
                          // Integer indice 0: Tamaño 
                          // Integer indice 4: Chequeo 
        IndCheck = 4,     // Indice de la palabra de chequeo

        Type     =  1,    // Longitud del campo tipo
        Count    =  4,    // Longitud de campos contador
        Value    =  9,    // Longitud maxima de campos valor
        MinSize  = 2000,  // Longitud inicial del buffer
        Realloc  = 2000,  // Multiplo para reajustar tamaño
    }
    #endregion

    /// <summary> Codificacion de mensajes interprocesos
    /// 
    /// El mensaje tiene la estructura siguiente:
    ///  - Cabecera (8 bytes) con dos elementos:
    ///    . Tamaño total del buffer  (4 bytes)
    ///    . Identificador de mensaje (4 bytes)
    ///  - Numero de elementos al primer nivel
    ///  - Elementos de datos del mensaje
    /// 
    /// Cada elemento simple tiene:
     /// - Tipo de datos (1 byte)
    ///  - Valor en binario del elemento
    /// 
    /// Cada elemento string tiene:
    ///  - Tipo de datos (1 byte)
    ///  - Longitud de datos (4 bytes)
    ///  - Valor del texto del string
    /// 
    /// Cada elemento array tiene:
    /// - Tipo de datos (1 byte)
    ///  - Longitud del array (4 bytes)
    ///  - Valores de los elementos
    ///    Cada elemento puede ser de cualquier tipo
    ///     
    /// Cada elemento hash tiene:
    ///  - Tipo de datos (1 byte)
    ///  - Longitud del array (4 bytes)
    ///  - Array de claves de la tabla hash
    ///  - Array de valores de la tabla hash
    /// 
    /// Cada elemento objeto tiene:
    ///  - Tipo de datos (1 byte)
    ///  - Numero de campos (4 bytes)
    ///  - Nombre de la clase
    ///  - Array de nombres de cada campo
    ///  - Array de valores de cada campo
    /// 
    /// La estructura del mensaje es igual en 32/64 bytes
    /// Puede variar el orden de bytes segun plataforma
    /// Dentro de la misma maquina no hay problemas
    /// Habria que ajustar entre plataformas distintas
    /// 
    /// </summary>

    public class MsgEncode
    {
        #region Defincion de la clase

        public MsgEncode()
        {
            CodePage = Encoding.Default.CodePage; // 1252 ESWIN
        }

        // Constante identificadora de mensaje
        // Tiene reservado sitio para numeracion
        private const int MsgIdent = 0x0000735F;

        // La pagina por defecto de la aplicacion es ESWIN (1252)
        // Se puede forzar para que no cambie en ningun equipo
        // Si la pagina Windows es otra se deben traducir textos

        public const int  DefaultCodePage = 1252; // ESWIN
        public int CodePage = DefaultCodePage;

        // Limites para conversion de fechas en formato Ole
        // Fuera de este rango se genera error de argumento

        private const double MinDate = -657435.0; 
        private const double MaxDate = 2958465.99999999; 

        #endregion

        #region Decodificaion de valores del mensaje

        public object[] GetValues(byte[] data)
        {
            object[] values = null;

            try
            {
                int ident = BitConverter.ToInt32(data, (int)InterSizes.IndCheck);

                if (ident == MsgIdent)
                {
                    int index = (int)InterSizes.Header;
                    values = MsgValues(data, ref index);
                }
            }
            catch( Exception exc)
            {
                Logger.LogError(exc);
            }

            return values;
        }

        /// <summary> Decodifica recursivamente un mensaje de datos
        /// </summary>
        /// <param name="data">  Array binario con el mensaje </param>
        /// <param name="index"> Indiece actual en el array   </param>
        /// <returns> Array de valores obtenidos </returns>

        private object[] MsgValues( byte[] data, ref int index )
        {
            int size;
            int count = BitConverter.ToInt32( data, index );
            index += (int)InterSizes.Count;

            object[] values = new object[count];

            for (int item = 0; item < count && index >= 0; item++)
            {
                InterTypes type = (InterTypes)data[index];
                index += (int)InterSizes.Type;

                try
                {
                    switch (type)
                    {
                        case InterTypes.Logical:
                            values[item] = BitConverter.ToBoolean(data, index);
                            index += (int)InterSizes.Logical;
                            break;

                        case InterTypes.Integer:
                            values[item] = BitConverter.ToInt32(data, index);
                            index += (int)InterSizes.Integer;
                            break;

                        case InterTypes.Double:
                            values[item] = BitConverter.ToDouble(data, index);
                            index += (int)InterSizes.Double;
                            break;

                        case InterTypes.Date:
                            double date = BitConverter.ToDouble(data, index);
                            if (date >= MinDate && date <= MaxDate)
                                values[item] = DateTime.FromOADate(date);
                            else
                                values[item] = DateTime.MinValue;
                            index += (int)InterSizes.Date;
                            break;

                        case InterTypes.Pointer:
                            values[item] = BitConverter.ToInt32(data, index);
                            index += (int)InterSizes.Pointer;
                            break;

                        case InterTypes.String:
                            size = BitConverter.ToInt32(data, index);
                            index += (int)InterSizes.String;

                            // Crear string comprobando la pagina ANSI
                            string text;

                            if (CodePage != DefaultCodePage)
                            {
                                // El istema tiene otra pagina de codigos
                                Encoding enc = Encoding.GetEncoding(CodePage);
                                text = enc.GetString(data, index, size);
                            }
                            else
                            {
                                // Pagina por defecto 1252 (ESWIN) de la aplicacion
                                text = Encoding.Default.GetString(data, index, size);
                            }
                            values[item] = text;
                            index += size;
                            break;

                        case InterTypes.Array:
                            values[item] = MsgValues(data, ref index );
                            break;

                        case InterTypes.Hash:
                            object[] keys = MsgValues(data, ref index);
                            object[] vals = MsgValues(data, ref index);
                            size = keys.Length;
                            Hashtable table = new Hashtable(size);
                            for (int nPos = 0; nPos < size; nPos++)
                            {
                                table.Add(keys[nPos], vals[nPos]);
                            }
                            values[item] = table;
                            break;

                        case InterTypes.Null:
                            break;

                        default:
                            // Tipo no reconocido: Error de buffer
                            values[item] = "** Error **";
                            index = -1;
                            break;
                    }
                }
                catch(Exception exc)
                {
                    Logger.LogError(exc);
                }
            }

            return values;
        }
        #endregion

        #region Codificacion de valores del mensaje

        /// <summary> Devuelve el mensaje de un array de valrores
        /// </summary>
        /// <param name="values"> Array de valores a codificar </param>
        /// <returns> Array binario con el mensaje </returns>
        
        public byte[] GetBytes(object[] values, out int size)
        {
            int index = (int)InterSizes.Header;

            byte[] bytes = new byte[(int)InterSizes.MinSize];
            MsgBytes(values, ref bytes, ref index);

            int header = 0;
            SetDataInt(bytes, ref header, index);
            SetDataInt(bytes, ref header, MsgIdent);

            size = index+1;

            return bytes;
        }

        /// <summary> Codifica recursivamente un array de valores
        /// </summary>
        /// <param name="values"> Array de valores a codificar </param>
        /// <param name="data">   Array binario con el mensaje </param>
        /// <param name="index">  Indice actual en el mensaje  </param>
        /// <returns> Array binario con el mensaje </returns>

        private void MsgBytes( object[] values, ref byte[] data, ref int index )
        {
            int count = values.Length;
            int size  = count * (int)InterSizes.Value + (int)InterSizes.Count * 2;

            MsgResize(ref data, index + size);

            SetDataInt(data, ref index, count);

            foreach (object value in values)
            {
                if (value == null)
                {
                    data[index++] = (int)InterTypes.Null;
                }
                else
                {
                    // Comprobar el tipo base de valor
                    Type type = value.GetType();

                    TypeCode code = Type.GetTypeCode(type);

                    switch (code)
                    {
                        case TypeCode.Boolean:
                            data[index++] = (int)InterTypes.Logical;
                            data[index++] = (byte)((bool)value ? 1 : 0);
                            break;

                        case TypeCode.Int32:
                            data[index++] = (int)InterTypes.Integer;
                            SetDataInt(data, ref index, (int)value);
                            break;

                        case TypeCode.Int16:
                            data[index++] = (int)InterTypes.Integer;
                            SetDataInt(data, ref index, (int)(short)value);
                            break;

                        case TypeCode.Double:
                            data[index++] = (int)InterTypes.Double;
                            SetDataDouble(data, ref index, (double)value);
                            break;

			            case TypeCode.DateTime:

                            // El valor vacio 1/1/0001 llega como 1/12/1999 
                            // Es erroneo pero dejar un valor null tambien
                            // if ((DateTime)value == DateTime.MinValue)
                            if (false)
                            {
                                data[index++] = (int)InterTypes.Null;
                            }
                            else
                            {
                                data[index++] = (int)InterTypes.Date;
                                SetDataDouble(data, ref index, ((DateTime)value).ToOADate());
                            }
                            break;

			           case TypeCode.Char:
                            // Caracter: Se envia como cadena de 1 byte
                            data[index++] = (int)InterTypes.String;
                            SetDataText(data, ref index, value.ToString());
                            MsgResize(ref data, index + size);
                            // data[index++] = (int)InterTypes.Integer;
                            // SetDataInt(data, ref index, (int)(char)value);
                            break;

                       case TypeCode.String:
                            data[index++] = (int)InterTypes.String;
                            SetDataText(data, ref index, (string)value);
                            MsgResize(ref data, index + size);
                            break;

                        case TypeCode.Byte:
                            data[index++] = (int)InterTypes.Integer;
                            SetDataInt(data, ref index, (int)(byte)value);
                            break;

			            case TypeCode.SByte:
                            data[index++] = (int)InterTypes.Integer;
                            SetDataInt(data, ref index, (int)(SByte)value);
                            break;

                        case TypeCode.UInt32:
                            data[index++] = (int)InterTypes.Integer;
                            SetDataInt(data, ref index, (int)(UInt32)value);
                            break;

                        case TypeCode.Int64:
                            data[index++] = (int)InterTypes.Integer;
                            SetDataInt(data, ref index, (int)(Int64)value);
                            break;

                        case TypeCode.UInt64:
                            data[index++] = (int)InterTypes.Integer;
                            SetDataInt(data, ref index, (int)(UInt64)value);
                            break;

                        case TypeCode.Single:
                            data[index++] = (int)InterTypes.Double;
                            SetDataDouble(data, ref index, (double)(float)value);
                            break;

                        case TypeCode.Decimal:
                            data[index++] = (int)InterTypes.Double;
                            SetDataDouble(data, ref index, (double)(decimal)value);
                            break;

                        default:
                            if (type.IsArray)
                            {
                                data[index++] = (int)InterTypes.Array;
                                MsgBytes((object[])value, ref data, ref index);
                            }
                            else
                            {
                                if (value is Hashtable)
                                {
                                    // Tabla Hash: Lista de claves y valores
                                    Hashtable hash = value as Hashtable;
                                    int items = hash.Count;
                                    object[] keys = new object[items];
                                    object[] vals = new object[items];
                                    items = 0;

                                    foreach (DictionaryEntry entry in hash)
                                    {
                                        keys[items] = entry.Key;
                                        vals[items] = entry.Value;
                                    }

                                    data[index++] = (int)InterTypes.Hash;
                                    MsgBytes(keys, ref data, ref index);
                                    MsgBytes(vals, ref data, ref index);
                                }
                                else
                                {
                                    // Otras clases: No soportadas todavia
                                    data[index++] = (int)InterTypes.Null;
                                }
                            }
                            break;
                        }
                    }
                }
        }
        
        #endregion

        #region Soporte de asignacion y ajuste del mensaje

        /// <summary> Asigna un valor entero al buffer de mensaje
        /// </summary>
        /// <param name="data">  Array binario del mensaje </param>
        /// <param name="index"> Indice actual del mensaje </param>
        /// <param name="value"> Valor entero para asignar </param>

        private void SetDataInt(byte[] data, ref int index, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            Buffer.BlockCopy(bytes, 0, data, index, (int)InterSizes.Integer);
            index += (int)InterSizes.Integer;
        }

        /// <summary> Asigna un valor double al buffer de mensaje
        /// </summary>
        /// <param name="data">  Array binario del mensaje </param>
        /// <param name="index"> Indice actual del mensaje </param>
        /// <param name="value"> Valor double para asignar </param>

        private void SetDataDouble(byte[] data, ref int index, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            Buffer.BlockCopy(bytes, 0, data, index, (int)InterSizes.Double);
            index += (int)InterSizes.Double;
        }

        /// <summary> Asigna un valor de cadena al buffer de mensaje
        /// El texto se convierte a la pagina de codigos ANSI actual
        /// </summary>
        /// <param name="data">  Array binario del mensaje </param>
        /// <param name="index"> Indice actual del mensaje </param>
        /// <param name="value"> Valor texto para asignar  </param>

        private void SetDataText(byte[] data, ref int index, string value)
        {
            byte[] bytes;

            if (CodePage != DefaultCodePage)
            {
                // El sistema tiene otra pagina de codigos
                Encoding enc = Encoding.GetEncoding(CodePage);
                bytes = enc.GetBytes(value);
            }
            else
            {
                // Pagina por defecto 1252 (ESWIN) de la aplicacion
                bytes = Encoding.Default.GetBytes(value);
            }

            int size   = bytes.Length;
            int maxind = index + size + (int)InterSizes.String;

            if (maxind > data.Length)
                MsgResize(ref data, maxind );

            // data[index] = (byte)size;

            byte[] lenStr = BitConverter.GetBytes(size);

            Buffer.BlockCopy(lenStr, 0, data, index, (int)InterSizes.Integer);
            // index += (int)InterSizes.Integer;
            
            index += (int)InterSizes.String;
            Buffer.BlockCopy(bytes, 0, data, index, size);
            index += size;
        }

        /// <summary> Comprueba y ajusta tamaño del buffer de mensaje
        /// </summary>
        /// <param name="data"> Array binario del mensaje </param>
        /// <param name="size"> Nuevo tamaño requerido    </param>
        /// <returns> Buffer de mensaje con tamaño pedido </returns>

        private void MsgResize(ref byte[] data, int size)
        {
            if (size >= data.Length)
            {
                size = ((size / (int)InterSizes.Realloc) + 1) *
                                (int)InterSizes.Realloc;

                if (data != null && size != data.Length)
                {
                    Array.Resize(ref data, size);
                }
                // Arr.Resize(ref data, size);
            }
        }
        #endregion
    }
}
