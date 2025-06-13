using MailAppMAUI.General;

namespace MailAppMAUI.General
{
    /// <summary> Codigos de accion asociado a un evento
    /// </summary>
    /// <remarks>
    /// 
    /// Los eventos comunican sucesos a las capas superiores 
    /// La capa superior puede ignorarlo o actuar adecuadamente
    /// 
    /// Los mensajes pueden crear un registro y presentarse
    /// La forma de presentacion la realiza la capa de interfaz
    /// Puede variarse segun los requisitos de cada aplicacion
    /// Pero debe respetar el tipo basico de mensaje: modal o no 
    /// 
    /// Cada tipo de mensaje especifica su registro y presentacion
    /// A su vez depende de la configuracion de prueba del sistema
    /// Las operaciones de presentacion y registro standard son:
    /// 
    /// Evento    Registrar     Mostar       Presentacion
    /// --------  ------------  -----------  --------------------
    /// Logger    Si            No
    /// Debug     Modo Debug    No          
    /// Test      Modo Test     No          
    /// Trace     Modo Trace    No       
    /// 
    /// Event     Modo Trace    Si           Informacion no modal
    /// Info      No            Si           Informacion no modal
    /// 
    /// Alert     Si            Si           Mensaje aceptacion
    /// Accept    No            Si           Mensaje aceptacion
    /// Valid     No            Si           Aceptar o cancelar
    /// Select    No            Si           Seleccionar opcion
    /// 
    /// Warning   Si            Trace        Informacion no modal
    /// Error     Si            Si           Mensaje aceptacion
    /// 
    /// </remarks>

    public enum MsgResult { OK, Cancel };

    public enum MsgSelect { Ok, All, Cancel };

    // [Flags]
    public enum OpAction
    {
        None,                 // No se realiza ninguna operacion

        // Eventos de registro unicamente
        Logger = 0x11000, // Mensaje a registrar en cualquier modo 
        Trace = 0x12000, // Mensaje a registrar unicamente en modo Trace
        Test = 0x14000, // Mensaje a registrar unicamente en modo Test
        Debug = 0x18000, // Mensaje a registrar unicamente en modo Debug

        // Eventos de registro y presentacion
        Event = 0x1A000, // Mensaje a registrar y mostrar no confirmado
        Alert = 0x1B000, // Mensaje a registrar y mostrar confirmado

        // Eventos de aviso y error 
        Warning = 0x21000, // Informacion de alerta y registro en modo Trace
        Error = 0x22000, // Informacion de error y registro siempre

        // Eventos de presentacion unicamente
        Info = 0x41000, // Mensaje de estado sin confirmar
        General = 0x42000, // Mensaje de estado general y visible
        Accept = 0x44000, // Mensaje de informacion confirmado
        Valid = 0x48000, // Mensaje de confirmacion o cancelacion
        Select = 0x4A000, // Mensaje de seleccion de opciones
        Retry = 0x4C000, // Mensaje de reintento o cancelacion

        Input = 0x4D000, // Mensaje de lectura de datos
        Password = 0x4E000, // Mensaje de elctura de password

        ActionType = 0xFF000, // Mascara de tipos de eventos
        ActionCode = 0x00FFF, // Mascara de operaciones especificas

        Title = 0x11,    // Informacion de titulo de la operacion

        Enable = 0x14,    // Habilitar uno o varios campos 
        Disable = 0x15,    // Deshabilitar uno o varios campos
        Active = 0x16,    // Activar un campo o un objeto

        Lock = 0x17,    // Bloquear eventos de usuario
        UnLock = 0x18,    // Desbloquear eventos de usuario

        Param = 0x19,    // Modifica propiedades de un objeto
        Stat = 0x1A,    // Notificación de cambio de estado
        Reset = 0x1B,    // Reiniciar control a valores iniciales

        Refresh = 0x1C,    // Refresco sin recarga de datos
        Update = 0x1D,    // Actualizacion de datos obsoletos
        Reload = 0x1E,    // Invalidacion y recarga general de datos

        ProgTypes = 0x20,    // Eventos de notificacion de progreso
        ProgInit = 0x21,    // Notificacion de inicio de un proceso
        ProgStart = 0x22,    // Notificacion de inicio de nueva operacion
        ProgNext = 0x23,    // Notificacion de avance de operacion
        ProgSkip = 0x24,    // Notificacion de avance sin cambio de texto
        ProgClose = 0x25,    // Notificacion de finalización de proceso
        ProgCancel = 0x26,    // Notificacion de cancelación de proceso
        ProgTitle = 0x27,    // Notificacion de cambio de titulo sin avance

        AppTypes = 0x30,    // Eventos de ordenes a nivel aplicacion
        AppExec = 0x31,    // Ejecucion de ordenes a nivel aplicacion
        AppOpen = 0x32,    // Inicia o activa ventanas de aplicacion 
        AppClose = 0x33,    // Cierre de uno o todas las ventanas
        AppRegen = 0x34,    // Inicio de proceso de regeneracion
        AppExit = 0x35,    // Orden de salida de la aplicacion

        Find = 0x41,    // Notificacion de inicio de busqueda
        Data = 0x42,    // Notificacion de entrada de datos 
        Close = 0x43,    // Notificion de cierre del proceso actual

        DataOpen = 0x51,    // Notificacion de apertura de datos
        DataClose = 0x52,    // Notificacion de cierre de datos
        DataSend = 0x53,    // Notificacion de envio de datos
        DataRead = 0x54,    // Notificacion de recepcion de datos
        DataChange = 0x55,    // Notificacion de cambio de datos
        Extern = 0x56,    // Notificacion de evento externo 

        SelectFile = 0x81,  // Busqueda standard de ficheros
        SelectFolder = 0x82,  // Busqueda standard de carpetas

        InterCodes = 0xF0,    // Mensajes de evento a InterData
        InterInfo = 0xF1,    // Mensaje de informacion InterData
        InterValid = 0xF2,    // Mensaje de validacion InterData
        InterResult = 0xF3,    // Mensaje de resultado InterData 
    };



    public class OpEvent : EventArgs
    {
        #region DEFINICION DE LA CLASE

        private int m_Code;     // Codigo numerico unico del evento 
        private OpAction m_Action;   // Tipo de actuacion requerida
        private string m_Title;    // Titulo o descripcion del evento

        private string m_StrCode;  // Codigo unico del evento como texto
                                   // Usado por compatibilidad temporal
                                   // Debe pasarse al codigo numerico 

        private string m_Iden;     // Identificador origen del evento
        private string m_Area;     // Identificador del area origen
        private OpResul m_Resul;    // Estado de la ultima modificacion
        private object[] m_Param;    // Lista de parametros especificos
        private Status StatCode;   // Codigo de estado y proceso del evento

        private const int IndexIden = 0;
        private const int IndexName = 1;
        private const int IndexValue = 2;
        private const int IndexInfo = 3;
        private const int IndexDescrip = 4;
        private const int IndexOper = 5;
        private const int IndexIndex = 6;
        private const int IndexCount = 7;
        private const int IndexTotal = 8;
        private const int IndexArea = 9;
        private const int IndexFile = 10;
        private const int IndexError = 11;
        private const int IndexMessage = 12;

        private const int IndexLast = IndexMessage;

        [Flags]
        private enum Status
        {
            None,
            Logged = 0x1,  // Evento con registro realizado
            Process = 0x2,  // Evento con proceso iniciado
            Delayed = 0x4,  // Evento con proceso retrasado
            Complete = 0x8,  // Evento con proceso completado
        }

        /// <summary> Asignacion de parametros y propiedades
        /// Se debe asignar una propiedad a cada parametro
        /// 
        /// El evento se invoca a traves de una lista de parametros
        /// Esta lista se carga en el campo Info en el constructor 
        /// Sin embargo el evento se consulta con sus propiedades
        /// Por tanto debe existir una correspondencia exacta 
        /// 
        /// El orden por defecto de los parametros es el siguiente:
        /// 
        /// - Parametro 0: Propiedad: Iden 
        /// - Parametro 1: Propiedad: Name 
        /// - Parametro 2: Propiedad: Value
        /// - Parametro 3: Propiedad: Info
        /// 
        /// - Parametro 4: Propiedad: Descrip
        /// - Parametro 5: Propiedad: Oper
        /// - Parametro 6: Propiedad: Index
        /// - Parametro 7: Propiedad: Count
        /// - Parametro 8: Propiedad: File
        /// - Parametro 9: Propiedad: Error 
        /// 
        /// El uso en el codigo de Version 1.02 no es fijo siempre:
        /// 
        /// Param  Propiedades    Significado 
        /// -----  -------------  -----------------------------------
        /// 
        /// Control del interfaz de usuario
        /// 
        ///   0    Iden           Identifica origen o destino del evento
        ///   1    Info           Informacion adicional o titulo
        ///   2    Value          Valor para asignar el parametro
        /// 
        /// Eventos de progreso de una operacion
        /// 
        ///   0    Iden           Identificador origen del mensaje
        ///   1    Value          Valor maximo o valor actual de progreso
        ///   2    Descrip        Texto del estado actual de la operacion
        ///   3    Title          Titulo general de la operacion en curso
        /// 
        /// Eventos de comunicaciones
        /// 
        ///  0     Iden           Identificador del terminal
        ///  1     Name           Nombre del envio o del fichero
        /// 
        /// </summary>

        public OpEvent(int code, Info atrib, params object[] info)
        {
            int[] orders = null;
            Code = code;

            if (atrib == null)
                Action = OpAction.Accept;
            else
            {
                Action = (OpAction)atrib.Action;
                Title = atrib.Title;

                // El area se usa para dirigir mensajes 
                // No se puede hacer esta carga generica
                // Debe asignarlo la clase origen emisora
                // Una vez cargada mantiene el area origen
                // this.Area   = atrib.Area;

                orders = (int[])atrib.Params;

                // if (this.Action <= OpAction.Error)
                if (ToRegister)
                {
                    // Evento a registrar: Añadir nombre del mensaje
                    SetParam(IndexMessage, atrib.Name);
                }
            }

            int lenOrder = orders == null ? 0 : orders.Length;
            int lenInfo = info == null ? 0 : info.Length;
            int total = Math.Max(lenOrder, lenInfo);

            if (total > 0)
            {
                int offset = 0;

                if (lenInfo > 0 && info[0] is Exception exception)
                {
                    // Mensaje de error definido con una excepcion
                    offset = 1;
                    total--;

                    // Cargar datos especificos del error
                    Exception oExc = exception;

                    Error = info[0] as Exception;
                    Info = oExc.GetType().Name;
                    Descrip = oExc.Message;

                    // 2.01-29: Se elimina esto: Dejar los parametros
                    // for( int ind=offset; ind < total+offset; ind++ )
                    // {
                    //     if (info[ind] != null)
                    //         this.Descrip += '\n' + info[ind].ToString();
                    //         // this.Descrip += " - " + info[ind].ToString();
                    // }
                    // total = 0;

                    if (oExc.InnerException != null)
                    {
                        Descrip += '\n';
                        Descrip += oExc.InnerException.Message;
                    }

                }

                // Asignar varios parametros segun ordenes
                for (int numPar = 0; numPar < total; numPar++)
                {
                    int index;
                    // if (orders != null && numPar < orders.Length)
                    if (numPar < lenOrder)
                    {
                        index = orders[numPar];
                        if (index != numPar)
                        {
                            string text = Title;

                            if (text != null && text.Contains('{'))
                            {
                                string str1 = '{' + numPar.ToString() + '}';
                                string str2 = '{' + index.ToString() + '}';
                                Title = text.Replace(str1, str2);
                            }
                        }
                    }
                    else
                        index = numPar;
                    // index = numPar-Offset;

                    if (offset + numPar < lenInfo)
                        SetParam(index, info[offset + numPar]);
                    else
                    {
                        if (atrib.Values != null && numPar < atrib.Values.Length)
                            SetParam(index, atrib.Values[numPar]);
                    }
                }

                if (Action == OpAction.Select)
                {
                    // Seleccion personalizada:Asignar opciones y valores
                    // Admite 4 parametros:
                    // 1) Parametro principal de valor a sustituir en el mensaje
                    //    El texto del mensaje esta en la definicion del evento
                    //
                    // 2) Valor por defecto de las opciones a seleccionar
                    //    Define el tipo de valor retornado del evento
                    //
                    // 3) Parametro Info:  Array con opciones de texto
                    //    Por defecto se obtiene de los nombre de tipo enumerado
                    //
                    // 4) Parametro Value: Array con valores de cada opcion
                    //    Esta lista se puede inferir del valor por defecto
                    //    Asigna valor correlativo de un enum a cada opcion
                    //    Si salta el valor incial del enumerado si es None

                    if (atrib.Values != null)
                    {
                        Info = atrib.Values;

                        object defval = Data.GetParam<object>(info, 1);
                        object[] opcs = Data.GetParam<string[]>(info, 2);
                        object[] vals = null;

                        int[] a = [1, 2, 3];


                        if (info.Length >= 3 && info[3] is Array array)
                            vals = (object[])array;//Arr.GenArray(array);

                        if (!Data.IsEmpty(opcs))
                        {
                            // Se dan textos especificos en la llamada
                            Info = opcs;
                        }

                        if (!Data.IsEmpty(vals))
                        {
                            // Se dan valores especificos en la llamada
                            Value = vals;
                        }
                        else
                        {
                            // Definir los valores correlativos del enumerado
                            if (defval is Enum && !Data.IsEmpty(Info))
                            {
                                int numval = atrib.Values.Length;

                                Type type = defval.GetType();

                                var enumval = Enum.GetValues(type);
                                object[] enumvals = new object[0];
                                if (Enum.GetName(type, 0).Equals("None"))
                                {
                                    enumvals = (object[])enumval;
                                    Arr.Delete<object>(ref enumvals, 0);
                                }

                                object[] keys;

                                Value = keys = new object[numval];

                                if (numval > enumvals.Length)
                                    numval = enumvals.Length;

                                for (int index = 0; index < numval; index++)
                                {
                                    keys[index] = enumvals.GetValue(index);
                                }
                            }
                        }


                    }
                }
            }

            // ToString();
        }

        /// <summary> Constructor de un evento de accion generica
        /// Permite enviar eventos genericos sin registrar en una clase
        /// </summary>
        /// <param name="action"> Codigo de accion del evento </param>
        /// <param name="info"> Paramtros adiccionales </param>

        public OpEvent(OpAction action, params object[] info) :
               this(0, new Info(action), info)
        {
        }

        /// <summary> Asigna indice a cada numero de parametro de los atributos
        /// </summary>
        /// <param name="attrib"> Definicion de atributos a procesar </param>
        /// <remarks>
        /// 
        /// La definicion de atributos debe incluir entre dos o tres parametros
        /// Los dos primeros parametros son el codigo de operacion y el titulo
        /// El tercer parametro opcional es una lista de campos separada por coma
        /// Esta lista se interpreta como nombres de parametros al crear el evento 
        /// 
        /// La lista se carga inicialmente como una cadena en la propiedad Info
        /// Este metodo cambia esta lista por un array con sus indices asociados
        /// Los nombre que corresponden con campos del evento se asigna su indice 
        /// Los nombres que no corresponden a propiedades se añaden al final
        /// 
        /// Por tanto se pueden definir cualquier numero de parametros de usuario
        /// Estos parametros solo podran ser accedidos por su numero de indice
        /// 
        /// </remarks>

        public static void IndexParams(Info attrib)
        {
            if (attrib.Params != null)
            {
                if ((OpAction)attrib.Action == OpAction.Select)
                {
                    // Seleccion: Son el texto de las opciones
                    attrib.Values = Str.Split(attrib.Params as string, ',');
                    attrib.Params = null;
                }
                else
                {
                    object[] values = null;

                    if (attrib.Params is string param)
                    {
                        StrScan scan = new(param, ',');
                        int total = scan.Count();

                        if (total > 0)
                        {
                            // Defincion de parametros del evento
                            // Los parametros son una cadena con comas
                            // Pueden tener valores de inicializacion
                            int[] orders = new int[total];
                            int last = IndexLast + 1;

                            foreach (string name in scan)
                            {
                                int index = -1;
                                int signo = name.IndexOf('=');

                                if (signo > 0)
                                {
                                    string value = name[(signo + 1)..].Trim();

                                    if (values == null)
                                        attrib.Values = values = new object[total];

                                    values[scan.Order] = Data.ToValue(value);

                                    index = MapParam(name[..signo].TrimEnd());
                                }
                                else
                                    index = MapParam(name);

                                if (index < 0)
                                    index = ++last;

                                orders[scan.Order] = index;
                            }
                            attrib.Params = orders;
                        }
                        else
                            attrib.Params = null;
                    }
                }
            }
        }

        /// <summary> Devuelve el indice asociado a un nombre de parametro
        /// </summary>
        /// <param name="name"> Nombre del parametro   </param>
        /// <returns> Indice interno o -1 si no existe </returns>

        private static int MapParam(string name)
        {
            int index;
            switch (name)
            {
                case "Iden": index = IndexIden; break;
                case "Name": index = IndexName; break;
                case "Value": index = IndexValue; break;
                case "Info": index = IndexInfo; break;
                case "Descrip": index = IndexDescrip; break;
                case "Oper": index = IndexOper; break;
                case "File": index = IndexFile; break;
                case "Error": index = IndexError; break;
                case "Count": index = IndexCount; break;
                case "Total": index = IndexTotal; break;
                case "Index": index = IndexIndex; break;
                default: index = -1; break;
            }

            return index;
        }

        #endregion

        #region EVENTOS ANTIGUOS CON CODIGO STRING

        public OpEvent(string Code)
        {
            StrCode = Code;
            LoadInfo();
        }

        public OpEvent(string Code, params object[] Info)
        {
            StrCode = Code;
            LoadInfo(Info);
        }

        public OpEvent(string Code, OpData oArg)
        {
            StrCode = Code;

            if (oArg != null)
            {
                Iden = oArg.Iden;
                Resul = oArg.Resul;
                Index = oArg.Index;
                Count = oArg.Count;
                Info = oArg.Value;
            }

            LoadInfo();
        }

        public OpEvent(string Code, Exception oExc, params object[] Info)
        {
            StrCode = Code;
            LoadInfo(Info);

            Descrip = oExc.Message;
            this.Info = oExc.GetType().Name;
            Error = oExc;
        }
        #endregion

        #region COMPROBACION DE CODIGOS DE OPERACION

        /// <summary> Comprueba si el evento se procesa a alto nivel
        /// Los eventos solo registro no se envian a capas superiores
        /// Los eventos Logger, Debug y Test unicamente se registran
        /// Los evento warning se procesa solo en modo Trace
        /// </summary>

        public bool ToProcess
        {
            get
            {
                bool resul;

                if (resul = m_Action != OpAction.None)
                {
                    if ((m_Action & OpAction.ActionCode) == OpAction.None)
                    {
                        switch (m_Action & OpAction.ActionType)
                        {
                            case OpAction.Logger:
                            case OpAction.Debug:
                            case OpAction.Test:
                            case OpAction.Trace:
                                resul = false;
                                break;

                            case OpAction.Warning:
                                //resul = ConfigCore.Instance.Gestion["IsTrace", false];
                                break;
                        }
                    }
                }
                return resul;
            }
        }

        /// <summary> Centraliza el criterio de registrar eventos
        /// Esta propiedad indica si el evento debe ser registrado
        /// Se registran siempre eventos Error, Warning y Logger
        /// El evento Debug se registra con nivel de prueba Debug
        /// El evento Test  se registra con nivel de prueba Test
        /// El evento Trace se registra con nivel de prueba Trace
        /// El evento Event se registra con nivel de prueba Trace
        /// El resto de codigos de evento no se registran nunca
        /// </summary>

        public bool ToRegister
        {
            get
            {
                bool resul = false;

                OpAction oper = m_Action & OpAction.ActionType;

                if (IsError && oper != OpAction.Debug)
                {
                    // Si es un error autentico se debe registrar
                    // salvo que sea solo registro para modo debug
                    // Estos casos son errores previsibles a ignorar
                    // Solo se deben registrar en caso de depuracion
                    return true;
                }

                switch (oper)
                {
                    case OpAction.Logger:
                    case OpAction.Error:
                    case OpAction.Warning:
                    case OpAction.Alert:
                        resul = true;
                        break;

                    case OpAction.Debug:
                        //resul = ConfigCore.Instance.Gestion["IsDebug", false];
                        break;

                    //case OpAction.Test:
                    //    resul = ConfigCore.Instance.Gestion.IsTest;
                    //    break;

                    case OpAction.Trace:
                    case OpAction.Event:
                        //resul = ConfigCore.Instance.Gestion["IsTrace", false];
                        break;
                }

                return resul;
            }
        }

        /// <summary> Comprueba si el evento genera estado de espera 
        /// Esto incluye cualquier menajes procesado de forma modal
        /// </summary>

        public bool ToWait
        {
            get
            {
                return IsWait(m_Action);

                //bool resul;

                //switch (m_Action & OpAction.ActionType)
                //{
                //    case OpAction.Alert:
                //    case OpAction.Accept:
                //    case OpAction.Valid:
                //    case OpAction.Select:
                //    case OpAction.Warning:
                //    case OpAction.Error:
                //        resul = true;
                //        break;

                //    default:
                //        resul = false;
                //        break;
                //}
                //return resul;
            }
        }

        public static bool IsWait(OpAction oper)
        {
            bool resul;

            switch (oper & OpAction.ActionType)
            {
                case OpAction.Alert:
                case OpAction.Accept:
                case OpAction.Valid:
                case OpAction.Select:
                case OpAction.Warning:
                case OpAction.Error:
                    resul = true;
                    break;

                default:
                    resul = false;
                    break;
            }
            return resul;
        }

        /// <summary> Comprueba si el evento genera una seleccion
        /// Son los mensajes de seleccion de opciones y validacion
        /// Estos mensajes se deben excluir de todos los loggers
        /// </summary>

        public bool ToSelect
        {
            get
            {
                bool resul;

                switch (m_Action & OpAction.ActionType)
                {
                    case OpAction.Valid:
                    case OpAction.Select:
                        resul = true;
                        break;

                    default:
                        resul = false;
                        break;
                }
                return resul;
            }
        }

        /// <summary> Comprueba si el evento va al logger de datos
        /// A este logger van algunos eventos modales y de registro
        /// Permite crear un registro de datos con ciertos mensajes
        /// </summary>
        /// <remarks> El logger de datos cumple dos objetivos:
        /// 
        /// - Permite que un proceso se complete solo hasta el final
        ///   Desactiva todos los mensajes modales incluso de error
        ///   
        /// - Crea registro especifico de datos accesible al usuario
        ///   Por tanto solo se envian eventos con texto de usario 
        ///   
        /// Se envian mensajes modales y los maracdos con Logger
        /// Se quitan expresamente los que requeiren una seleccion 
        /// No tiene sentido registar una pregunta o una selecion
        /// 
        /// El logger de datos tendra molo los mensajes de usario 
        /// Ademas incluye si existen, mensajes de error imprevistos
        /// </remarks>

        public bool ToData
        {
            get
            {
                bool resul;

                switch (m_Action & OpAction.ActionType)
                {
                    case OpAction.Alert:
                    case OpAction.Accept:
                    case OpAction.Error:
                    case OpAction.Logger:
                        resul = true;
                        break;

                    default:
                        resul = false;
                        break;
                }
                return resul;
            }
        }

        #endregion

        #region DEFINICION DE PROPIEDADES GENERALES

        /// <summary> Codigo del evento expresado como cadena
        /// Se usa solo por COMPATIBILIDAD con codigo antiguo
        /// Debe usarse siempre el codigo numerico del evento
        /// </summary>

        public virtual string StrCode
        {
            get { return m_StrCode; }
            set { m_StrCode = AppNorm.Normalize(value); }
        }

        /// <summary> Codigo numerico unico asignado al evento
        /// Corresponde con el enumerado definido en la clase
        /// </summary>

        public virtual int Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        /// <summary> Identificador unico de referencia al objeto
        /// </summary>

        public virtual string Iden
        {
            get
            {
                // El identificador se carga en su campo o en el array
                if (m_Param == null)
                    return m_Iden;
                else
                    return GetParam(IndexIden) as string;
            }

            set
            {
                // El identificador se carga en su campo o en el array
                // if (value != null)
                //     value = AppNorm.Normalize(value);
                // 
                // NOTA: Se quita la normalizacion de identificadores
                // Esta campo puede contener cualquier dato y no siempre se 
                // debe normalizar (Por ejemplo el nombre de un fichero)
                // Debe revisarse el uso del campso en codigo antiguo!!
                // Sobre todo los mensajes de las clases de comunicaciones 
                // Estas clases deben manejar identificadores normalizados

                if (m_Param == null)
                    m_Iden = value;
                else
                    SetParam(IndexIden, value);
            }
        }

        /// <summary> Identificador unico de del area origen del evento
        /// El area se define con el area de operacion de la clase origen
        /// </summary>

        public virtual string Area
        {
            get
            {
                // El area origen se carga en su campo o en el array
                if (m_Param == null)
                    return m_Area;
                else
                    return GetParam(IndexArea) as string;
            }

            set
            {
                // El area origen se carga en su campo o en el array
                if (m_Param == null)
                    m_Area = value;
                else
                    SetParam(IndexArea, value);
            }
        }

        /// <summary> Titulo descriptivo del mensaje o evento
        /// </summary>

        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        /// <summary> Valor por defecto para operacion de seleccion
        /// Se utiliza el segundo parametreo de la llamada al evento
        /// Para operaciones qeu no son de seleccion este valor es nulo
        /// </summary>
        public object Default
        {
            get
            {
                if (Action == OpAction.Select)
                    return GetParam(IndexName);

                return null;
            }
        }

        /// <summary> Codigo de operacion o tipo de operacion del evento
        /// Se devuelve el codigo concreto de operacion si esta definido
        /// En caso contrario de devuelve el tipo de operacion generico
        /// Al modificar la propiedad debe pasarse el codigo completo
        /// </summary>
        /// <remarks>
        /// Un evento define una operacion y un tipo de operacion generico
        /// Se pueden combinar o definir solo la operacion o tipo generico
        /// Debe usarse esta propiedad para definir el proceso de un evento
        /// dado que siempre devuelve el codigo mas concreto de operacion 
        /// </remarks>

        public OpAction Action
        {
            get
            {
                OpAction order = m_Action & OpAction.ActionCode;

                if (order != OpAction.None)
                    return order;
                else
                    return m_Action;

                return m_Action;
            }
            set { m_Action = value; }
        }

        /// <summary> Retorna tipo generico de operacion del evento
        /// Un evento define una operacion generica y una especifica
        /// </summary>
        /// <remarks>
        /// Un evento define una operacion y un tipo de operacion generico
        /// Se pueden combinar o definir solo la operacion o tipo generico
        /// Esta propiedad devuelve o actualiza solo el tipo generico
        /// 
        /// Este tipo se utiliza para tratamientos genericos del evento
        ///    - Indica eventos que deben o no registrase 
        ///    - Indica eventos que deben procesarse a capas superiores
        ///    - Indica forma de presentacion generica del mensaje
        /// 
        /// Estos tratamientos son compatibles con codigos mas especificos
        /// Por ejemplo un evento de progreso puede tambien registrase 
        /// 
        /// </remarks>

        public OpAction ActionType
        {
            get { return m_Action & OpAction.ActionType; }
            set { m_Action = m_Action & OpAction.ActionCode | value; }
        }

        /// <summary> Comprueba codigo de operacion para un tipo dado
        /// </summary>
        /// <param name="check"> Tipo de codigos de operacion </param>
        /// <returns> Indica si la operacion es del tipo dado </returns>

        public bool IsType(OpAction check)
        {
            if (((int)check & 0xF00) != 0)
                return ((int)m_Action & 0xFF0) == (int)check;
            else
                return ((int)m_Action & 0xF0) == (int)check;
        }

        /// <summary> Nivel de cambio producido en la operacion
        /// </summary>

        public OpResul Resul
        {
            get { return m_Resul; }
            set { m_Resul = value; }
        }

        /// <summary> Indica si el evento proviene de un error 
        /// </summary>

        public bool IsError
        {
            get
            {
                /*
                if (this.Error != null &&
                   ((m_Action & OpAction.ActionType) != OpAction.Error))
                {
                    // Antes no se habria tomado como un error 
                    // Ver consecuencias de este cambio
                }
                */

                return Error != null;

                // +++ CUIDADO: Estaba asi hast ala verison 2.01-2
                //     Pero puede no registrar un error real
                //     Comprobar problemas qeu origina
                // 
                // return (this.Error != null && 
                //         ((m_Action & OpAction.ActionType) == OpAction.Error)); 
            }
        }

        /// <summary> Indica si el evento ha sido registrado
        /// Previene registros duplicados de un mimso evento
        /// </summary>

        public bool IsLogged
        {
            get
            {
                return (StatCode & Status.Logged) != 0;
            }
            set
            {
                if (value)
                {
                    if ((StatCode & Status.Logged) == 0)
                        StatCode ^= Status.Logged;
                }
                else
                {
                    if ((StatCode & Status.Logged) != 0)
                        StatCode ^= Status.Logged;
                }
            }
        }

        /// <summary> Indica si el evento ha iniciado el proceso
        /// Esta propiedad normalmente es false al crear un evento
        /// Se debe poner a true para que no continue su proceso 
        /// </summary>

        public bool IsProcess
        {
            get
            {
                return (StatCode & Status.Process) != 0;
            }
            set
            {
                if (value)
                {
                    if ((StatCode & Status.Process) == 0)
                        StatCode ^= Status.Process;
                }
                else
                {
                    if ((StatCode & Status.Process) != 0)
                        StatCode ^= Status.Process;
                }
            }
        }

        /// <summary> Indica si el el evento ha sido retrasado
        /// Esto se hace en estados en los que no puede procesar 
        /// El evento se debe marcar en estos casos como retrasado
        /// Esto garantiza que se va a reenviar posteriormente
        /// </summary>

        public bool IsDelayed
        {
            get
            {
                return (StatCode & Status.Delayed) != 0;
            }
            set
            {
                if (value)
                {
                    if ((StatCode & Status.Delayed) == 0)
                        StatCode ^= Status.Delayed;
                }
                else
                {
                    if ((StatCode & Status.Delayed) != 0)
                        StatCode ^= Status.Delayed;
                }
            }
        }

        /// <summary> Indica si el evento ha sido procesado
        /// Previene el proceso doble de un mismo evento
        /// Se activa al final de un proceso de alto nivel
        /// </summary>

        public bool IsComplete
        {
            get
            {
                return (StatCode & Status.Complete) != 0;
            }
            set
            {
                if (value)
                {
                    if ((StatCode & Status.Complete) == 0)
                        StatCode ^= Status.Complete;
                }
                else
                {
                    if ((StatCode & Status.Complete) != 0)
                        StatCode ^= Status.Complete;
                }
            }
        }

        #endregion

        #region PROCESO DEL TEXTO DEL EVENTO

        /// <summary> Retorna cadena formateada con un valor
        /// El texto puede contener hasta 3 marcas de parametro
        /// Si no existen marcas no se modifica la cadena dada
        /// 
        /// Los parametos marcados se insertan por este orden:
        /// 
        /// - Parametro 0: Campo Iden  del mensaje
        /// - Parametro 1: Campo Name  del mensaje
        /// - Parametro 2: Campo Value del mensaje
        /// - Parametro 3: Campo Info  del mensaje
        /// 
        /// No se pueden pasar cadenas con mas de 4 parametros
        /// </summary>
        /// <param name="Text">  Cadena a formatear </param>
        /// <returns> Cadena formateada </returns>

        public string GetText()
        {
            string text = Descrip;

            if (text == null)
                text = Title;

            return GetText(text);
        }

        public string GetText(string text)
        {
            if (text == null)
                return "";

            if (text.IndexOf('{') >= 0)
            {
                try
                {
                    text = SubNames(text);
                    text = Str.SubParam(text, m_Param);
                }
                catch (Exception exc)
                {
                }
            }

            return text;
        }

        /// <summary> Substituye parametros expresados por su nombre
        /// Los nombres deben ir entre corchetes como otros parametros
        /// </summary>
        /// <param name="text"> Cadena con parametros a substituir </param>
        /// <returns> Cadena con nombres substituidos </returns>

        public static string SubNames(string text)
        {
            int start = 0;
            int count = -1;
            string field;

            while ((field = Str.GetText(text, 0, '{', '}', ref start, ref count)) != null)
            {
                if (!Str.IsNumber(field))
                {
                    // Substituir parametro definido por su nombre
                    int index = MapParam(field);

                    // if (index < 0)
                    //     field = String.Empty;
                    // else
                    //     field = '{' + index.ToString() + '}';

                    // Solo se substituten parametros vaidos encontrados
                    // El texto puede aparecer en mas casos (ejemplo Odbc)
                    if (index >= 0)
                    {
                        field = '{' + index.ToString() + '}';

                        text = text.Substring(0, start - 1) + field +
                               text.Substring(start + count + 1);
                    }

                }
                count = -1;
            }
            return text;
        }

        /// <summary> Retorna cadena descriptiva del evento
        /// </summary>
        /// <returns> Cadena descriptiva del evento </returns>

        public override string ToString()
        {
            return Action.ToString() +
                  (Action != ActionType ? ' ' + ActionType.ToString() : "") + ':' + GetText();
        }

        #endregion

        #region DEFINICION DE PROPIEDADES ESPECIFICAS

        /// <summary> Cuenta de objetos o filas afectados por la operacion
        /// </summary>

        public int Count
        {
            get { return IntParam(IndexCount, 0); }
            set { SetParam(IndexCount, value); }
        }

        /// <summary> Cuenta de filas u objetos totales existentes
        /// </summary>

        public int Total
        {
            get { return IntParam(IndexTotal, 0); }
            set { SetParam(IndexTotal, value); }
        }

        /// <summary> Indice de la fila u objeto actual afectado
        /// </summary>

        public int Index
        {
            get { return IntParam(IndexIndex, -1); }
            set { SetParam(IndexIndex, value); }
        }

        /// <summary> Codigo numerico de operacion afectada
        /// </summary>

        public int Oper
        {
            get { return IntParam(IndexOper, 0); }
            set { SetParam(IndexOper, value); }
        }

        /// <summary> Nombre del campo afectado por la operacion
        /// </summary>

        public string Name
        {
            get
            {
                object Resul = GetParam(IndexName);
                if (Resul != null)
                    return Resul.ToString();
                else
                    return null;
            }
            set
            {
                SetParam(IndexName, value);
            }
        }

        /// <summary> Valor actual de la operacion origen
        /// </summary>

        public object Value
        {
            get { return GetParam(IndexValue); }
            set { SetParam(IndexValue, value); }
        }

        /// <summary> Informacion adiccional asociada al mensaje
        /// </summary>

        public object Info
        {
            get { return GetParam(IndexInfo); }
            set { SetParam(IndexInfo, value); }
        }

        /// <summary> Descripcion extendida de usuario del mensaje
        /// </summary>

        public string Descrip
        {
            get { return (string)GetParam(IndexDescrip); }
            set { SetParam(IndexDescrip, value); }
        }

        /// <summary> Nombre del mensaje origen del evento
        /// </summary>

        public string Message
        {
            get { return (string)GetParam(IndexMessage); }
            set { SetParam(IndexMessage, value); }
        }

        /// <summary> Fichero de registro asociado el evento
        /// </summary>

        public string File
        {
            get { return (string)GetParam(IndexFile); }
            set { SetParam(IndexFile, value); }
        }

        /// <summary> Objeto excepcion de un evento de error
        /// </summary>

        public Exception Error
        {
            get { return (Exception)GetParam(IndexError); }
            set { SetParam(IndexError, value); }
        }

        #endregion 

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

        /// <summary> Consulta una propiedad de la clase tipo entero
        /// </summary>
        /// <param name="nIndex"> Indice de la propiedad    </param>
        /// <param name="defval"> Valor si no esta definido </param>
        /// <returns> Valor de la propiedad </returns>

        private int IntParam(int nIndex, int defval)
        {
            if (m_Param != null && nIndex < m_Param.Length)
            {
                if (m_Param[nIndex] != null)
                    return (int)m_Param[nIndex];
            }
            return defval;
        }

        /// <summary> Modifica una propiedad extendida de la clase
        /// El array se alarga si es necesario hasta el nuevo indice
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
            else
            {
                if (Value is string)
                {
                    // Los valores de texto vacios se tratan como null
                    // Esto permite eliminar los parametros opcionales
                    // Sin embargo se respetan los textos con espacios

                    if (Value == string.Empty)
                    {
                        if (m_Param == null || nIndex >= m_Param.Length)
                            return;
                        else
                            Value = null;
                    }
                }

            }

            if (m_Param == null)
            {
                // Crear el array de parametros extendidos del evento
                // El tamaño minimo sera el de los parametros standard
                int nSize = nIndex >= IndexLast ? nIndex + 1 : IndexLast;
                m_Param = (object[])Array.CreateInstance(typeof(object), nSize);

                // El identificador y el area origen y tiene un campo simple
                // Al crear el array debe copiarse el valor actual de este
                if (m_Iden != null)
                    m_Param[IndexIden] = m_Iden;

                if (m_Area != null)
                    m_Param[IndexArea] = m_Area;

            }

            if (nIndex >= m_Param.Length)
            {
                // Alargar el array de parametros hasta el indice pedido
                object[] ArrMod = (object[])Array.CreateInstance(typeof(object), nIndex + 1);
                m_Param.CopyTo(ArrMod, 0);
                m_Param = ArrMod;
            }

            m_Param[nIndex] = Value;
        }

        #endregion

        #region CARGA DE INFORMACION DEL MENSAJE

        /// <summary> Carga informacion y titulo del evento 
        /// </summary>
        /// <param name="?"></param>
        /// <remarks>
        /// Este metodo debe cargar la informacion de una tabla 
        /// Esta tabla global se carga al incio con ficheros Xml
        /// Debe existir un fichero por Area de la aplicacion
        /// Ademas existe un fichero global con mensajes genericos
        /// 
        /// Para cada mensaje se debe definir al menos:
        ///     - Codigo: Codigo base del mensaje (sin area)
        ///     - Titulo: Titulo o descripcion del mensaje
        ///     - Accion: Tipo de actuacion requerida 
        /// 
        /// El codigo (Code) es obligatorio para generar un evento
        /// El resto de parametros van como parametros variables 
        /// La informacion y orden de esta depende de cada evento 
        /// No obstante existen unas reglas de normalizacion
        /// 
        /// Los criterios para parametros del evento son:
        /// 
        ///     - Identificador origen del evento (Iden)
        ///       Si existe va siempre como primer parametro
        /// 
        ///     - Informacion adiccional del evento (Info)
        ///       Normalmente es el segundo parametro
        /// 
        ///     - Valor asociado al evento (Value)
        ///       Se usa en eventos que requieren un valor
        ///       Puede ser el segundo o tercer parametro
        /// 
        /// Al generar un evento se busca el codigo para el area
        /// Si no existe se busca el mismo evento en el area global
        /// 
        /// Para ello el codigo base se guarda precedido por el area
        /// Los mensajes genericos se guardan asocidos al area Global
        /// La aplicacion emite los mensajes solo con el codigo base
        /// 
        /// Ejemplo: 
        /// 
        ///     - Codigo de mensaje emitido:    Val_Salvar:
        ///     - En el area Ficha de Clientes: FichaClien_Val_Salvar
        ///     - Si no existe se busca global: Global_Val_Salvar
        /// 
        ///     El mensaje global sera generico para todos los casos
        ///     Si se define en las fichas puede ser mas preciso
        /// 
        /// </remarks>

        private void LoadInfo(params object[] Info)
        {
            string sMens = null;
            OpAction nOper = OpAction.None;

            switch (m_StrCode)
            {
                // Actualizacion del interfaz

                case "Dat_Update":
                    sMens = null;
                    nOper = OpAction.Update;
                    break;

                case "Dat_Refresh":
                    sMens = null;
                    nOper = OpAction.Refresh;
                    break;

                case "Dat_Enable":
                    nOper = OpAction.Enable;
                    if (Info.Length > 0)
                        Iden = (string)Info[0];
                    break;

                case "Dat_Disable":
                    nOper = OpAction.Disable;
                    if (Info.Length > 0)
                        Iden = (string)Info[0];
                    break;

                case "Dat_Active":
                    nOper = OpAction.Active;
                    if (Info.Length > 0)
                        Iden = (string)Info[0];
                    break;

                case "Dat_Lock":
                    nOper = OpAction.Lock;
                    break;

                case "Dat_UnLock":
                    nOper = OpAction.UnLock;
                    break;

                case "Dat_Title":
                case "Dat_Info":
                case "Dat_Acept":
                    if (m_StrCode == "Dat_Title")
                        nOper = OpAction.Title;
                    else
                        nOper = OpAction.Info;

                    if (Info.Length > 1)
                    {
                        Iden = (string)Info[0];
                        this.Info = (string)Info[1];
                    }
                    else
                        Iden = "";
                    this.Info = (string)Info[0];
                    break;

                case "Dat_Find":
                    nOper = OpAction.Find;
                    if (Info.Length > 0)
                    {
                        Iden = (string)Info[0];

                        if (Info.Length > 1)
                            this.Info = (string)Info[1];
                    }
                    break;

                case "Dat_Param":
                    nOper = OpAction.Param;
                    Iden = (string)Info[0];
                    Name = (string)Info[1];
                    if (Info.Length > 1)
                        Value = Info[2];
                    break;

                case "Dat_Reset":
                    nOper = OpAction.Reset;
                    Iden = (string)Info[0];
                    break;

                // Inicio de proceso de busqueda

                case "Find_Exec":
                    nOper = OpAction.Find;
                    if (Info.Length > 0)
                        Iden = (string)Info[0];

                    if (Info.Length > 1)
                        this.Info = (string)Info[1];
                    break;

                // Cambio de estado en soporte

                case "Stat_Init":
                    nOper = OpAction.Stat;
                    break;

                case "Stat_Read":
                    nOper = OpAction.Stat;
                    break;

                case "Stat_Edit":
                    nOper = OpAction.Stat;
                    break;

                case "Stat_Append":
                    nOper = OpAction.Stat;
                    break;

                case "Stat_Close":
                    nOper = OpAction.Stat;
                    break;

                // Notificacion de eventos externos
                case "App_Event":
                    nOper = OpAction.Extern;
                    Oper = (int)Info[0];
                    Iden = (string)Info[1];
                    this.Info = (string)Info[2];
                    break;

                // Notificacion de entrada de datos

                case "Scan_Barras":
                    nOper = OpAction.Data;
                    if (Info.Length > 0)
                        Value = (string)Info[0];
                    break;

                // Notificacion de progreso de una operacion
                //     
                // Iden:    Identificador origen del mensaje
                // Value:   Valor maximo o valor actual de progreso
                // Descrip: Texto del estado actual de la operacion
                // Title:   Titulo general de la operacion en curso

                case "Oper_Init":
                case "Impor_Init":
                case "Expor_Init":
                    InfoOper(OpAction.ProgStart, Info);
                    break;

                case "Oper_Next":
                case "Impor_Next":
                case "Expor_Next":
                    InfoOper(OpAction.ProgNext, Info);
                    break;

                case "Oper_End":
                case "Impor_End":
                case "Expor_End":
                    InfoOper(OpAction.ProgClose, Info);
                    break;

                case "Oper_Cancel":
                case "Impor_Cancel":
                case "Expor_Cancel":
                    InfoOper(OpAction.ProgClose, Info);
                    break;

                // Entrada de documentos

                case "Docum_Valid":
                    sMens = "¿Desea editar el documento existente?";
                    nOper = OpAction.Valid;
                    break;

                case "Docum_Cancel":
                    sMens = "¿Desea cancelar las modificaciones del documento?";
                    nOper = OpAction.Valid;
                    break;

                case "Clien_Indef":
                    nOper = OpAction.Accept;
                    sMens = "No existe el codigo de {0}";
                    break;

                case "Clien_Vacio":
                    nOper = OpAction.Accept;
                    sMens = "Debe indicar el codigo y el nombre";
                    break;

                case "Artic_Indef":
                    sMens = "No existe el codigo de artículo";
                    nOper = OpAction.Accept;
                    break;

                case "Artic_Vacio":
                    sMens = "Debe indicar el codigo y nombre del artículo";
                    nOper = OpAction.Accept;
                    break;

                case "Barras_Duplic":
                    sMens = "Ya existe el código de barras";
                    nOper = OpAction.Accept;
                    break;

                case "Barras_Indef":
                    sMens = "Código de barras no encontrado" + Convert.ToChar(13) +
                            "¿Desea darlo de alta?";
                    nOper = OpAction.Valid;
                    break;

                case "Artic_Existen":
                    nOper = OpAction.Accept;
                    sMens = "Existencias insuficientes";
                    break;

                // Mensajes Genericos 

                case "Val_Salvar":
                    sMens = "¿Desea salvar las modificaciones?";
                    nOper = OpAction.Valid;
                    break;

                case "Val_Descartar":
                    sMens = "¿Desea descartar las modificaciones?";
                    nOper = OpAction.Valid;
                    break;

                case "Val_Salir":
                    sMens = "¿Desea salir de la aplicacion?";
                    nOper = OpAction.Valid;
                    break;

                case "Val_Baja":
                    sMens = "¿Desea borrar esta ficha?";
                    nOper = OpAction.Valid;
                    break;

                case "Val_Reset":
                    sMens = "Carga de datos inicial:" +
                            "\nSe perderan los datos pendientes del terminal " +
                            "¿Desea continuar? ";
                    nOper = OpAction.Valid;
                    break;

                case "Inf_Duplic":
                    sMens = "Ya existe este código";
                    nOper = OpAction.Accept;
                    break;

                case "Inf_Action":
                    sMens = "No se puede completar la operacion";
                    nOper = OpAction.Accept;
                    break;

                case "Inf_Envio":
                    sMens = "No existe el envio {0}";
                    nOper = OpAction.Accept;
                    break;

                case "Inf_EnvioPend":
                    sMens = "Envio {0} pendiente";
                    nOper = OpAction.Info;
                    break;

                // Mensajes a nivel de la aplicacion

                case "App_Open":
                    nOper = OpAction.AppOpen;
                    break;

                case "App_Close":
                    nOper = OpAction.AppClose;
                    break;

                case "App_Exit":
                    nOper = OpAction.AppExit;
                    break;

                // Mensajes de bajo nivel de comunicaciones

                case "Com_Connecting":
                    sMens = "Conectando con direccion {0}:{1}";
                    nOper = OpAction.Trace;
                    break;

                case "Com_Connected":
                    sMens = "Conectado con direccion {0}:{1}";
                    nOper = OpAction.Trace;
                    break;

                case "Com_ErrConnect":
                    sMens = "No se puede conectar a {0}:{1}";
                    nOper = OpAction.Trace;
                    break;

                case "Com_ErrComManager":
                    sMens = "Error conectando por GPRS/3G {0}";
                    nOper = OpAction.Trace;
                    break;

                case "Com_FindIp":
                    sMens = "Buscado Ip dinamica para {0}";
                    nOper = OpAction.Trace;
                    break;

                case "Com_FindDns":
                    sMens = "Consultando DNS para {0}";
                    nOper = OpAction.Trace;
                    break;

                case "Com_FindError":
                    sMens = "No se encuentra la ruta del servidor {0}";
                    nOper = OpAction.Event;
                    break;

                case "Com_UpdateIp":
                    sMens = "Actualizando registro de Ip para {0}";
                    nOper = OpAction.Trace;
                    break;

                case "Com_UpdateError":
                    sMens = "No se puede actualizar registro de Ip {0}";
                    nOper = OpAction.Event;
                    break;

                case "Com_Listen":
                    sMens = "Iniciada escucha en el puerto {0}";
                    nOper = OpAction.Trace;
                    break;

                case "Com_ListenError":
                    sMens = "No se puede iniciar escucha en puerto {0}";
                    nOper = OpAction.Event;
                    break;

                case "Com_Error":
                    sMens = "No se puede conectar al servidor {0}";
                    nOper = OpAction.Event;
                    break;

                case "Com_Connect":
                    sMens = "Conectado al servidor {0}";
                    nOper = OpAction.Event;
                    break;

                case "Com_Accept":
                    sMens = "Terminal {0} conectado";
                    nOper = OpAction.Event;
                    break;

                case "Com_Lost":
                    sMens = "Terminal {0} desconectado";
                    nOper = OpAction.Event;
                    break;

                case "Com_Start":
                    sMens = "Servidor iniciado";
                    nOper = OpAction.Event;
                    break;

                case "Com_StartError":
                    sMens = "No se puede iniciar el servidor";
                    nOper = OpAction.Event;
                    break;

                case "Com_Stop":
                    sMens = "Servidor detenido";
                    nOper = OpAction.Event;
                    break;

                case "Com_Close":
                    sMens = "Terminal {0} desconectado";
                    nOper = OpAction.Event;
                    break;

                case "Com_ValidSend":
                    sMens = "Envio correcto {0} ";
                    nOper = OpAction.Event;
                    break;

                case "Com_ValidRead":
                    sMens = "Recepción correcta {0} ";
                    nOper = OpAction.Event;
                    break;

                case "Com_ErrConn":
                    sMens = "No se puede conectar al servidor {0}";
                    nOper = OpAction.Event;
                    break;

                case "Com_ErrSend":
                    sMens = "Fallo de Conexión enviando ";
                    nOper = OpAction.Event;
                    break;

                case "Com_ErrRead":
                    sMens = "Fallo de conexión recibiendo ";
                    nOper = OpAction.Event;
                    break;

                // Proceso de alto nivel de envio y recepcion

                case "Int_Send":
                    sMens = "Transmitido envio {1} del terminal {0}";
                    nOper = OpAction.DataSend;
                    if (Info.Length > 1)
                        Name = (string)Info[1];
                    break;

                case "Int_Read":
                    sMens = "Recibido envio {1} del terminal {0}";
                    nOper = OpAction.DataRead;
                    if (Info.Length > 1)
                        Name = (string)Info[1];
                    break;

                case "Int_Open":
                    sMens = "Terminal {0} conectado";
                    nOper = OpAction.DataOpen;
                    break;

                case "Int_Close":
                    sMens = "Terminal {0} desconectado";
                    nOper = OpAction.DataClose;
                    break;

                case "Int_Connecting":
                    sMens = "Conexion en curso...";
                    nOper = OpAction.Event;
                    break;

                case "Int_Start":
                    sMens = "Servidor iniciado";
                    nOper = OpAction.Event;
                    break;

                case "Int_Stop":
                    sMens = "Servidor detenido";
                    nOper = OpAction.Event;
                    break;

                case "Int_Error":
                    sMens = "No se puede iniciar la conexión";
                    nOper = OpAction.Event;
                    break;

                case "Int_Ready":
                    sMens = "Operación completa";
                    nOper = OpAction.Info;
                    break;

                // Proceso de errores de la aplicacion

                case "Err_RelInfo":
                    sMens = "Error al cargar relacion de tablas ";
                    nOper = OpAction.Error;
                    break;

                case "Err_TabInfo":
                    sMens = "Error al cargar definición de tabla ";
                    nOper = OpAction.Error;
                    break;

                // case "Err_XmlDef":
                //      sMens = "Error al cargar definición de datos ";
                //      nOper = OpAction.Error;
                //      break;

                // case "Err_Area":
                //      sMens = "Error cargando definición de area";
                //      nOper = OpAction.Error;
                //      break;


                case "Err_Baja":
                    sMens = "No se puede borra el registro";
                    nOper = OpAction.Error;
                    break;

                case "Err_Alta":
                    sMens = "No se puede crear el registro";
                    nOper = OpAction.Error;
                    break;

                case "Err_Modif":
                    sMens = "No se pueden modificar datos";
                    nOper = OpAction.Error;
                    break;

                case "Err_Open":
                    sMens = "No se puede acceder a la base de datos";
                    nOper = OpAction.Error;
                    break;

                /*
                case "Err_Seek":
                     sMens = "Error de busqueda de la base de datos";
                     nOper = OpAction.Error;
                     break;

                case "Err_Insert":
                     sMens = "No se puede agregar el registro de datos";
                     nOper = OpAction.Error;
                     break;

                case "Err_Update":
                     sMens = "No se puede actualizar la base de datos";
                     nOper = OpAction.Error;
                     break;

                case "Err_Read":
                     sMens = "Error de lectura de la base de datos";
                     nOper = OpAction.Error;
                     break;

                case "Err_Regen":
                     sMens = "Error al regenerar la base de datos";
                     nOper = OpAction.Error;
                     break;

                case "Err_Field":
                     sMens = "Debe regenerar la estructura de la Base de datos ";
                     nOper = OpAction.Error;
                     break;

                */

                case "Err_Delete":
                    sMens = "No se puede borrar el registro de datos";
                    nOper = OpAction.Error;
                    break;

                case "Err_Exec":
                    sMens = "Error de ejecucion de metodos";
                    nOper = OpAction.Error;
                    break;

                case "Err_Action":
                    sMens = "No se puede completar la operación";
                    nOper = OpAction.Error;
                    break;

                case "Err_Expor":
                    sMens = "No se puede completar exportación {0}";
                    nOper = OpAction.Error;
                    break;

                case "Err_Impor":
                    sMens = "No se puede completar importacion {0}";
                    nOper = OpAction.Error;
                    break;

                case "Err_FileMove":
                    sMens = "No se puede actualizar fichero {0}";
                    nOper = OpAction.Error;
                    break;

                case "Err_FileRead":
                    sMens = "No se puede leer el fichero {0}";
                    nOper = OpAction.Error;
                    break;

                case "Err_FileWrite":
                    sMens = "No se puede modificar el fichero {0}";
                    nOper = OpAction.Error;
                    break;

                case "Err_Value":
                    sMens = "Datos incorrectos";
                    nOper = OpAction.Trace;
                    break;

                case "Err_Connect":
                    sMens = "No se puede conectar al servidor {0}";
                    nOper = OpAction.Error;
                    break;

                case "Err_WebQuery":
                    sMens = "No se puede consultar la web {0}";
                    nOper = OpAction.Trace;
                    break;

                default:
                    sMens = "";
                    nOper = OpAction.Accept;
                    break;
            }

            if (nOper != OpAction.None)
            {
                Action = nOper;
                Title = sMens;
            }

            if (Iden == null)
                Setparams(Info);

            // if (this.Iden == null && Info != null && Info.Length > 0)
            //     this.Iden = (string)Info[0];
        }

        /// <summary> Carga de eventos de notificacion de operaciones
        /// Iden:     Identificador origen del mensaje
        /// Value:    Valor maximo o valor actual de progreso
        /// Descrip:  Texto del estado actual de la operacion
        /// Title:    Titulo general de la operacion en curso
        /// </summary>
        /// <param name="Oper"> Codigo de operacion </param>
        /// <param name="Info"> Array de parametros </param>

        void InfoOper(OpAction Oper, params object[] Info)
        {
            Action = Oper;

            if (Info.Length > 1)
            {
                Iden = Convert.ToString(Info[0]);
                Value = Convert.ToInt32(Info[1]);
                if (Info.Length > 2)
                {
                    if (Info[2] != null)
                        Descrip = Convert.ToString(Info[2]);

                    if (Info.Length > 3 && Info[3] != null)
                        Title = Convert.ToString(Info[3]);
                }
            }
        }

        /// <summary> Define parametros opcionales para mensajes
        /// El primer parametro siempre se carga en el campo Iden
        /// </summary>
        /// <param name="Info"> Lista de parametros pasados </param>
        /// <remarks>
        /// 
        /// Los parametos pasados se cargan por este orden:
        /// 
        /// - Parametro 0: Campo Iden  del mensaje
        /// - Parametro 1: Campo Name  del mensaje
        /// - Parametro 2: Campo Value del mensaje
        /// - Parametro 3: Campo Info  del mensaje
        /// </remarks>

        void Setparams(params object[] Info)
        {
            if (Info != null && Info.Length > 0)
            {
                Iden = Convert.ToString(Info[0]);

                if (Info.Length > 1)
                {
                    Name = Convert.ToString(Info[1]);

                    if (Info.Length > 2)
                    {
                        Value = Info[2];
                        if (Info.Length > 3)
                            this.Info = Info[3];
                    }
                }
            }
        }

        #endregion
    }
}
