using MailAppMAUI.General;

namespace MailAppMAUI.General
{
    /// <summary> Resultado de una actualizacion
    /// </summary>
    /// <remarks>
    /// Los niveles de cambio indican el nivel de refresco necesario
    /// Son comunes incluso para comunicacion de aplicaciones
    /// Deben mantenerse simpre con los mismos codigos definidos
    /// 
    /// - El nivel Range se reserva a cambios qeu invalidan todo
    ///   Refresca controles que no suelebn cambiar (Paneles, tools)
    ///   Por tanto debe asiugnarse solo en casos muy concretos
    ///   Se aplica de forma automatioca al inciarse una ventana
    ///   
    /// - El nivel Docum debe usarse cuando se descartan rangos
    ///   Por tanto debe usarse al añadir y crear lineas de grid
    ///   Tambien debne referscar todas las paginas de un folder
    ///   
    /// - El nivel Page refresca el contenido de una sola pagina
    ///   No se recargan los rangos limites de acceso a tablas
    ///   Por tanto los Select de datos siguen siendo validos
    ///   
    /// - El nivel Line se aplica a nivel de una linea o grupo
    ///   Por tanto solo se asumen cmabios en datos relacionados
    /// 
    /// - El nivel Data solo afecta al campo usado directamente
    ///   Indica operacion completa y el refrresco es automatico
    ///   
    /// - El nivel Valid indica validacion de una operacion
    ///   La operaicon esta sin grabar en procesos de usuario 
    ///   
    /// - El nivel Cancel es anulacion completa de la operacion
    ///   No se ha realizadi nigun cambio en lso datos 
    /// </remarks>

    //public enum OpResul
    //{
    //    Cancel,    // No se ha realizado la operacion
    //    Valid,     // Operacion valida pendiente de salvar
    //    Data,      // Operacion con cambio simple del dato
    //    Line,      // Operacion con cambio de una linea 
    //    Page,      // Operacion con cambio de una pagina
    //    Docum,     // Operacion con cambio del documento
    //    Range      // Operacion con cambio de los rangos
    //};

    /// <summary> Estado de modificaicon de datos de una tabla
    /// </summary>

    public enum OpStat
    {
        Init,      // No se han cargado datos
        Load,      // Se ha cargado datos sin modificar
        Data,      // Se han modificado datos
        Key,       // Se han modificado datos clave
        Range      // El rango en memoria es obsoleto 
    };


    /// <summary> Clase de envio de mensajes y operaciones
    /// Trasporta parametros y retorna resulados de la operacion
    /// </summary>

    public class OpData
    {
        public const int IndexDef = -1; // Indicacion de fila por defecto
        public const int IndexEmpty = -2; // Indicacion de fila vacia
        public const int IndexHeader = -3; // Indicacion de fila header
        public const int IndexData = -4; // Indicacion de fila de datos

        private string m_Iden;     // Identificador unico del objeto
        private int m_Index;    // Indice dentro de una tabla

        private object[] m_Param;  // Lista de parametros especiales

        private string m_Oper;   // Codigo de operacion especifico
        private object m_Value;  // Valor actual del objeto
        private int m_Count;  // Cuenta de objetos afectados
        private OpResul m_Resul;  // Estado de la ultima modificacion
        private int m_Attrib; // Flags de atributos del mensaje

        private const int ParamField = 0;
        private const int ParamFormat = 1;
        private const int ParamFormats = 2;
        private const int ParamLast = 3;
        private const int ParamTitle = 4;
        private const int ParamEvent = 5;

        private const int SetReadOnly = 0x1;  // Campo de solo lectura
        private const int SetTextConv = 0x2;  // Convertir resultado a texto

        /// <summary> Constructor de mensaje a partir de un identificador
		/// </summary>
		/// <param name="Iden"> Identificador origen del mensaje </param>
        /// <remarks>
        /// 
        /// Un Identificador normalizado consta de dos partes
        /// 
        ///    - Identificador normalizado de la tabla 
        ///    - Nombre simple del campo refererenciado
        /// 
        /// Los identificadores de interfaz pueden tener mas informacion
        /// 
        ///    - Informacion o propiedades del campo (Label, Title)
        ///      Esta informacion queda como propiedes del mensaje
        /// 
        ///    - Indice del mismo si existen varias instancias
        ///      Esta informacion se deja como el indice del mensaje
        /// 
        /// La informacion extendida se elimina al crear el mensaje
        /// El identificador se deja siempre en forma normalizada
        /// El indice y propiedades se dejan como parte del mensaje
        /// 
        /// Los identificadores deben cumplir varias reglas:
        /// 
        ///    - Cada parte se separa de otras mediante un guion bajo
        ///    - Cada parte comiena por letra mayuscula y minuscula
        ///    - Cadad parte solo puede tener letras o digitos
        /// 
        /// Ejemplo de identificadores:
        /// 
        ///    - Docum_Vencim          Identificador normalizado
        ///    - Docum_Vencim_2        Extendido (Campo de indice 2)
        ///    - Docum_Vencim_Label_1  Extendido (Etiqueta de indice 2)
        /// 
        /// </remarks>

        public OpData(string Iden)
        {
            Index = IndexDef;
            string cInfo = AppNorm.GetInfo(Iden);

            if (cInfo != null)
            {
                switch (cInfo)
                {
                    case "Label":
                        m_Attrib |= SetReadOnly;
                        break;
                }

                Index = AppNorm.GetOrder(Iden);
            }

            if (Index > 0)
                Index--;

            m_Iden = AppNorm.NormIden(Iden, null);
            Resul = OpResul.Cancel;
        }

        /// <summary> Constructor de mensaje con identificador y operacion 
        /// El identificador hace referencia al objeto origen del evento 
        /// La operacion puede ser cualquier codigo reconocido de comando 
        /// </summary>
        /// <param bane="Oper"> Identificador de la operacion    </param>
        /// <param name="Iden"> Identificador origen del mensaje </param>

        public OpData(string Oper, string Iden)
             : this(Iden)
        {
            if (Oper != null)
                this.Oper = Oper;
        }

        /// <summary> Constructor con identificador, operacion y valores
        /// El identificador hace referencia al objeto origen del evento 
        /// La operacion puede ser cualquier codigo reconocido de comando 
        /// Los valores pueden ser uno o varios objetos de cualquier tipo
        /// </summary>
        /// <param name="Iden">   Identificador origen del mensaje </param>
        /// <param bane="Oper">   Identificador de la operacion    </param>
        /// <param name="Values"> Lista de valores para el mensaje </param>

        public OpData(string Oper, string Iden, params object[] Values)
             : this(Oper, Iden)
        {
            if (Values.Length > 0)
            {
                if (Values.Length == 1)
                    Value = Values[0];
                else
                    this.Values = Values;
            }
        }

        /// <summary> Constructor de mensajes por defecto 
        /// </summary>

        public OpData()
        {
            m_Iden = "";
            Index = -1;
            Resul = OpResul.Cancel;
        }

        /// <summary> Indice o fila absoluta dentro de la tabla
        /// </summary>

        public int Index
        {
            get { return m_Index; }
            set
            {
                if (m_Index != value)
                {
                    m_Index = value;
                    m_Param = null;
                }
            }
        }

        /// <summary> Devuelve atributo de campo solo lectura
        /// </summary>

        public bool ReadOnly
        {
            get { return (m_Attrib & SetReadOnly) != 0; }
        }

        /// <summary> Valor leido o modificado en la ultima operacion
        /// </summary>

        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        /// <summary> Valores para operaciones con campos multiples
        /// </summary>

        public object[] Values
        {
            get { return (object[])m_Value; }
            set { m_Value = value; }
        }

        /// <summary> Cuenta de objetos afectados por la operacion
        /// </summary>

        public int Count
        {
            get { return m_Count; }
            set { m_Count = value; }
        }

        /// <summary> Nivel de cambio producido en la ultima operacion
        /// </summary>

        public OpResul Resul
        {
            get { return m_Resul; }
            set { m_Resul = value; }
        }

        public virtual string Oper
        {
            get { return m_Oper == null ? Name : m_Oper; }
            set { m_Oper = value; }
        }

        /// <summary> Identificador unico de referencia al objeto
        /// </summary>

        public virtual string Iden
        {
            get { return m_Iden; }
            set { m_Iden = AppNorm.Normalize(value); }
        }

        /// <summary> Nombre unico de referencia al objeto
        /// </summary>

        public virtual string Name
        {
            get { return AppNorm.GetName(m_Iden); }
            set
            {
                m_Iden = AppNorm.GetIden(AppNorm.Normalize(value),
                           AppNorm.GetTable(m_Iden));
            }
        }

        /// <summary> Identificador unico de Tabla asociada
        /// </summary>

        public virtual string Table
        {
            get { return AppNorm.GetTable(m_Iden); }
            set
            {
                m_Iden = AppNorm.GetIden(AppNorm.GetName(m_Iden),
                           AppNorm.Normalize(value));
            }
        }

        /// <summary> Titulo para presentacion al usuario
        /// Por defecto se utiliza el nombre del campo
        /// </summary>

        public string Title
        {
            get
            {
                object cTitle = GetParam(ParamTitle);
                return cTitle == null ? Name : cTitle.ToString();
            }

            set
            {
                if (Name == value)
                    Value = null;

                SetParam(ParamTitle, value);
            }
        }

        /// <summary> Formato de conversion especifica del valor
        /// </summary>

        public string Format
        {
            get
            {
                object value = GetParam(ParamFormat);
                if (value != null)
                    return (string)value;
                else
                    return null;
            }
            set { SetParam(ParamFormat, value); }
        }

        /// <summary> Formatos de conversion para varios campos
        /// </summary>

        public string[] Formats
        {
            get
            {
                object aValues = GetParam(ParamFormats);
                if (aValues != null)
                    return (string[])aValues;
                else
                    return null;
            }
            set { SetParam(ParamFormats, value); }
        }

        /// <summary> Seleccion de campos dentro de un registro 
        /// Permite operaciones con nombres de campos multiples 
        /// </summary>

        public string[] Fields
        {
            get
            {
                object aValues = GetParam(ParamField);
                if (aValues != null)
                    return (string[])aValues;
                else
                    return null;
            }
            set { SetParam(ParamField, value); }
        }

        /// <summary> Evento pendiente de ser enviado
        /// </summary>

        public OpEvent Event
        {
            get
            {
                object resul = GetParam(ParamEvent);
                if (resul != null)
                    return (OpEvent)resul;
                else
                    return null;
            }
            set { SetParam(ParamEvent, value); }
        }

        public bool IsText
        {
            get
            {
                return (m_Attrib & SetTextConv) == SetTextConv;
            }
            set
            {
                m_Attrib |= SetTextConv;
            }
        }

        #region SOPORTE DE DATOS ESPECIFICOS

        /// <summary> Consulta una propiedad extendida de la clase
        /// </summary>
        /// <param name="nIndex"> Indice de la propiedad </param>
        /// <returns> Valor de la propiedad </returns>

        private object GetParam(int nIndex)
        {
            if (m_Param != null && nIndex < m_Param.Length)
                return m_Param[nIndex];

            return null;
        }

        /// <summary> Modifica una propiedad extendida de la clase
        /// </summary>
        /// <param name="nIndex"> Indice de la propiedad      </param>
        /// <param name="Value">  Nuevo valor de la propiedad </param>

        private void SetParam(int nIndex, object Value)
        {
            if (Value == null)
            {
                if (m_Param == null || nIndex >= m_Param.Length)
                    return;
            }

            if (m_Param == null)
                m_Param = (object[])Array.CreateInstance(typeof(object), nIndex + 1);

            if (nIndex >= m_Param.Length)
            {
                object[] ArrMod = (object[])Array.CreateInstance(typeof(object), nIndex + 1);
                m_Param.CopyTo(ArrMod, 0);
                m_Param = ArrMod;
            }

            m_Param[nIndex] = Value;
        }

        #endregion


    }

}
