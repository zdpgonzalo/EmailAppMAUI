using MailAppMAUI.General;
using System.Collections;
using System.Reflection;

//[assembly:CLSCompliant(true)]

namespace MailAppMAUI.General
{
    /// <summary> Clase soporte de aplicacion a nivel de datos 
    /// Contiene las tablas globales de informacion y de tipos
    /// Genera evento de mensaje global a nivel de aplicacion
    /// </summary>

    //[CLSCompliant(true)]
    public class AppData : DatBase
    {
        #region DEFINICION DE LA CLASE

        const bool IsLoadAll = true;     // Carga completa de areas de aplicacion

        const string Tables_Name = "Tables"; // Nombre base del fichero de tablas
        const string Areas_Name = "Areas";  // Nombre base del fichero de areas
        const string Types_Name = "Types";  // Nombre base del fichero de tipos

        const string Tables_Base = "Tables.xml"; // Fichero salvado de tablas
        const string Areas_Base = "Areas.xml";  // Fichero salvado de areas

        private Hashtable GlobalSesion;  // Tablas de informacion de sesiones
        private Info[,][] GlobalProp;    // Tablas de definicion de tipos  

        private static Hashtable GlobalDat; // Lista de objetos globales genericos

        public bool IsInit
        {
            get { return isInit; }
            set { isInit = value; }
        }
        private bool isInit;            // Tablas globales inicializadas
        private bool LoadAll;           // Cargar todas las versiones de datos
                                        // por defecto se carga la version activa

        public AppData()
        {
            GlobalDat = [];
            GlobalSesion = AppSystem.GetHashTable();
            GlobalProp = new Info[MinSize, Data.ToInt(SubType.Count)][];
            // GlobalProp   = new ClsDef[MinSize];
        }

        static AppData()
        {
            LoadEvents(typeof(Events));
        }

        #endregion

        #region GESTION DE LAS TABLAS DE INFORMACION GLOBAL

        ///// <summary> Inicializa tabla de areas y tabla global de informacion
        ///// </summary>
        ///// <returns> Resultado de la operacion </returns>

        //public bool InitTables()
        //{
        //    AppInfo oInfo = new AppInfo();
        //    XmlTables oXml = new XmlTables();

        //    // Entrada para pruebas de desarrollo
        //    // TestHarbour.DoTest();

        //    // Cargar primero la tabla de definicion de tipos
        //    oXml.TryLoad(oInfo, Types_Name);

        //    // Comprobar cambio de fechas de algun modulo 
        //    bool resul = oXml.CheckFiles(Tables_Base) &&
        //                 oXml.CheckFiles(Areas_Base);

        //    if (!resul)
        //    {
        //        // Crear ficheros debido a  cambio de fechas 
        //        File.Delete(Paths.StructsDirectory + Tables_Base);
        //        File.Delete(Paths.StructsDirectory + Areas_Base);
        //    }

        //    // Cargar tabla de definicion del area global
        //    if (resul)
        //        resul = oXml.Load(oInfo, Tables_Base);

        //    if (resul)
        //        IsInit = true;
        //    else
        //    {
        //        // Cargar tablas por modulos y crear archivo
        //        oXml.LoadAll = this.LoadAll;
        //        oXml.TablePrefix = ConfigCore.Instance.Gestion.TablePrefix;

        //        resul = oXml.Load(oInfo, Tables_Name);
        //        if (resul)
        //        {
        //            oInfo.InitGlobal();
        //            IsInit = true;
        //        }
        //        oXml.Save(oInfo, Tables_Base);
        //    }

        //    // Cargar tablas de definicion de areas de aplicacion
        //    if (IsLoadAll)
        //    {
        //        // Cargar todas las areas del fichero de definicion
        //        bool valid = true;       // Resultado de la carga
        //        bool save = false;      // Carga inicial de areas
        //        bool found = false;      // Area ya cargada
        //        string file = Areas_Base; // Fichero o mascara a cargar
        //        string direc = null;       // Ultimo directorio cargado
        //        DatInfo oBase = null;
        //        resul = false;

        //        while (valid)
        //        {
        //            // Cargar areas sucesivas del fichero
        //            DatInfo oArea = new DatInfo();

        //            valid = oXml.LoadNext(oArea, file);

        //            if (!valid && !save && !resul)
        //            {
        //                // se debe crear el fichero de areas base
        //                save = true;
        //                file = Areas_Name;

        //                // Cargar siguiente area en la lista de archivos
        //                valid = oXml.LoadNext(oArea, file);
        //            }

        //            if (valid)
        //            {
        //                // Añadir la sesion cargada a la tabla
        //                found = (GlobalSesion[oArea.Area] != null);

        //                if (found && !LoadAll)
        //                {
        //                    OnEvent(Events.DupArea, oArea.Area, oXml.FileName);
        //                }
        //                else
        //                {
        //                    if (!found)
        //                        GlobalSesion.Add(oArea.Area, oArea);
        //                    resul = true;

        //                    // Inicializar las areas principales
        //                    if (oArea.Base == null)
        //                    {
        //                        if (save && direc != oXml.DirectoryName)
        //                        {
        //                            // Cargar tablas a nivel de la carpeta del modulo
        //                            direc = oXml.DirectoryName;
        //                            XmlTables oLoad = new XmlTables();
        //                            oBase = new DatInfo();

        //                            if (oLoad.Load(oBase, direc + Tables_Name))
        //                                oBase.InitGlobal();
        //                            else
        //                                oBase = null;
        //                        }

        //                        oArea.InitArea(oBase);
        //                    }

        //                    if (save)
        //                    {
        //                        if (found)
        //                        {
        //                            DatInfo oPrev = (DatInfo)GlobalSesion[oArea.Area];
        //                            oPrev.Append(oArea);
        //                            oArea = oPrev;
        //                        }

        //                        oXml.Save(oArea, Areas_Base);
        //                    }
        //                }
        //            }

        //        }
        //        oXml.Close();

        //        // Inicializar y completar ahora las area heredadas
        //        foreach (DictionaryEntry entry in GlobalSesion)
        //        {
        //            DatInfo area = (DatInfo)entry.Value;

        //            if (area.Base != null)
        //            {
        //                // Incializar un area heredada de otra 
        //                DatInfo super = (DatInfo)GlobalSesion[area.Base];
        //                if (super != null)
        //                    area.Append(super);
        //            }
        //        }
        //    }

        //    return resul;
        //}

        ///// <summary> Recarga las tablas globales y de areas
        ///// </summary>
        ///// <returns> Resultado de la operación </returns>

        //public bool Reload()
        //{
        //    GlobalSesion.Clear();
        //    AppInfo.ClearGlobal();
        //    return InitTables();
        //}

        ///// <summary> Regenera y recarga la definicion de tablas
        ///// Se puede indicar la carga de todas las versiones de datos
        ///// Se usa para generar campos de tablas para cualquier version 
        ///// </summary>
        ///// <param name="loadAll"> Carga todas las versioens de datos </param>
        ///// <returns> Resultado de la operación </returns>

        //public bool Regenerate(bool loadAll)
        //{
        //    string tables = Paths.StructsDirectory + Tables_Base;
        //    string areas = Paths.StructsDirectory + Areas_Base;

        //    if (File.Exists(tables))
        //        File.Delete(tables);

        //    if (File.Exists(areas))
        //        File.Delete(areas);

        //    this.LoadAll = loadAll;

        //    bool resul = Reload();

        //    Session.ReloadInfo = true;

        //    //if (resul)
        //    //    DbAccess.Reload();


        //    return resul;
        //}

        ///// <summary> Devuelve clase de informacion del area dada
        ///// Carga la informacion del area si no lo esta previamente
        ///// </summary>
        ///// <param name="cArea"> Nombre del area a devolver </param>
        ///// <returns> objeto de informacion de la sesion </returns>

        //public DatInfo GetArea(string cArea)
        //{
        //    // Cargar tablas globales y araes si no estan
        //    if (!isInit)
        //        InitTables();

        //    DatInfo oInfo = (DatInfo)GlobalSesion[cArea];

        //    if (oInfo == null && !(IsLoadAll && IsInit))
        //    {
        //        // Cargar e inicializar informacion del area
        //        oInfo = new DatInfo(cArea);
        //        XmlTables oXml = new XmlTables();

        //        if (oXml.Load(oInfo, null))
        //        {
        //            if (oInfo.Base != null)
        //            {
        //                // Se hereda de otro area: Copiar datos
        //                DatInfo oBase = GetArea(oInfo.Base);

        //                if (oBase != null)
        //                    oInfo.Append(oBase);

        //                oInfo.InitArea(null);
        //            }
        //            else
        //            {
        //                // Es un area de definicion directa
        //                oInfo.InitArea(null);
        //            }

        //            // Añadir definicion si se ha inicializado las tablas
        //            if (IsInit)
        //                GlobalSesion.Add(cArea, oInfo);
        //        }
        //        else
        //            oInfo = null;
        //    }

        //    // Devolver siempre un duplicado del area
        //    DatInfo oArea = null;

        //    if (oInfo != null)
        //        oArea = oInfo.Clone();

        //    return oArea;
        //}
        #endregion

        #region GESTION DE LAS TABLAS DE DEFINICION DE TIPOS

        /// <summary> Estructura para definir cada lista de propiedades
        /// Se crea un array con una estructura para cada lista definida
        /// </summary>

        struct ClsDef
        {
            public Info[][] Mens;
        }
        // const int MinSize = 50;   // Multiplo para aumentar la tabla global
        const int MinSize = 5;   // Multiplo para aumentar la tabla global

        /// <summary> Devuelve objeto de informacion asociado a un mensaje 
        /// La informacion se define mediante atributos o fichero de tipos
        /// Si no existe una definicion para el menaje pedido devuelve nulo
        /// </summary>
        /// <param name="code"> Codigo de mensaje a cargar </param>
        /// <returns> Objeto de informacion asociado al mensaje </returns>

        public Info GetInfo(int code)
        {
            int group = code >> Data.ToInt(OpMask.Size);

            // if (group > 0 && group < GlobalProp.Length) 
            if (group > 0 && group <= GlobalProp.GetUpperBound(0))
            {
                // int index = code & Data.ToInt(OpMask.Index);

                int subType = (code & Data.ToInt(OpMask.Base)) >> Data.ToInt(OpMask.Enum);
                int index = code & Data.ToInt(OpMask.Index);

                // OpInfo[] mens = GlobalProp[group].Mens[subType];

                Info[] mens = GlobalProp[group, subType];

                if (mens != null && index < mens.Length)
                    return mens[index];
            }
            return null;
        }

        /// <summary> Carga definicion de mensajes del tipo dado
        /// Los mensajes se definen con atributos en cada elemento
        /// Por otra parte el fichero de tipos puede redefinirlos
        /// </summary>
        /// <param name="enumTyp"> Enumerado de mensajes a definir </param>
        /// <remarks>
        /// 
        /// El nombre de un enumerado se define como sigue:
        /// 
        ///  - Si esta fuera de una clase:  Ifs.Gestion.Events
        ///  - Si esta dentro de una clase: Ifs.Gestion.TClien+Events
        ///    Este nombre se genera con tipos anidados (Enums)
        ///
        /// Esta diferencia de nombre se detecta y se normaliza
        /// Cada enumerado se salva con una clave homogenea de dos niveles:
        ///   - A nivel de un esapcion de nombres: Datos.OpLoad
        ///   - Si se define centro de una clase:  Tregen.Events
        ///      
        /// La definicion en Xlm se debe hacer siempre igual
        /// 
        ///    <Base Name="Datos">
        ///       <Info Name="OpLoad">
        ///           <Key Id="NoAction" Title="Mensaje no definido en {0}"/>
        ///       </Info>
        ///    </Base>
        ///    
        ///    <Base Name="TRegen">
        ///       <Info Name="Events">
        ///           <Key Id="NoAction" Title="Mensaje no definido en {0}"/>
        ///       </Info>
        ///    </Base>
        /// 
        /// CAMBIO IMPORTANTE PARA NUMERACION DE EVENTOS DINAMICA
        ///
        /// LoadInfo debe asignar el numero de grupo de cada tipo
        ///  - Debe ser llamado desde el constructor de tipo (static)
        ///  - Para cada tipo se asigna el grupo correlativo de un cotador
        ///  - Como cada tipo se llama una sola vez se asiga un grupo unico
        ///  - El numero de Grupo queda en una VARIABLE STATICA en cada tipo
        /// 
        /// Los eventos se definen siempre igual
        ///   - Comienzan en 0 (None) y numeran correlatimente (sin grupo)
        ///   - Por tanto su numero es relativo al tipo en que son definidos
        ///   
        /// Al invocar eventos se asigan su codigo definitivo
        ///   - La numeracion de eventos sirve solo para cargar su definicion
        ///   - Esto se puede hacer con la numeracion relativa de eventos
        ///   - Al invocar un evento (OnEvent) se le suma su grupo estatico  
        ///   - La carga de datos del evento sigue funcionando como hasta ahora
        ///   - El codigo que se carga en el evento debe ser el original relativo
        ///     De esta forma coincide si la clase cliente comprueba el codigo  
        ///   
        /// Carga del codigo de evento para enviarlo externamente
        ///   - El evento se carga con el codigo de evento completo (grupo e indice)
        ///   - La propiedad OpEvent.Code quita el grupo y da el evento original
        ///   - Por tanto la comprobacion de este codigo con un case funcionara
        ///     (El compilador maneja siempre las constantes enumeradas relativas)
        ///     
        /// Registro del codigo completo
        ///   - Hasta aqui todo funciona igual pero asignando eventos dinamicamente
        ///   - Se puede definir una propiedad en el evento que de el codigo completo
        ///   - Esto puede servir a efectos de registro de eventos y mantenimiento
        /// 
        /// </remarks>

        public void LoadInfo(Type enumTyp)
        {
            try
            {
                FieldInfo[] fields = enumTyp.GetFields(BindingFlags.Public |
                                                       BindingFlags.Static);
                int total = fields.Length;
                int offset = 0;

                if (total > 1)
                {
                    // Comprobar y descartar elemento None si existe
                    int value = Data.ToInt(fields[0].GetValue(null));

                    if (value == 0)
                        offset = 1;

                    value = Data.ToInt(fields[1].GetValue(null));

                    // int index = value & Data.ToInt(OpMask.Index);
                    int group = value >> Data.ToInt(OpMask.Size);
                    int subType = (value & Data.ToInt(OpMask.Base)) >> Data.ToInt(OpMask.Enum);
                    int index = value & Data.ToInt(OpMask.Index);

                    if (group >= GlobalProp.GetLength(0))
                    {
                        int size = (1 + group / MinSize) * MinSize;
                        // ClsDef[] newtab = new ClsDef[size];
                        // GlobalProp   = new OpInfo[MinSize,(int)SubType.Last][];

                        Info[,][] newtab = new Info[size, Data.ToInt(SubType.Count)][];

                        // GlobalProp[1, 1] = new OpInfo[4];
                        // GlobalProp[4, 0] = new OpInfo[5];

                        Array.Copy(GlobalProp, newtab, GlobalProp.Length);

                        // GlobalProp.CopyTo(newtab, 0);
                        GlobalProp = newtab;
                    }

                    // if (GlobalProp[group, subType] != null)
                    // {
                    //     OnEvent(Events.DupEvent, enumTyp.ToString());
                    //     return;
                    // }
                    // GlobalProp[group,subType] = new Info[total-offset];

                    // Crear clave nornalizada de dos niveles del tipo
                    string name = null;
                    int last = enumTyp.FullName.LastIndexOf('+');
                    if (last < 0)
                    {
                        // Enumerado definido en el espacio de nombres
                        last = enumTyp.FullName.LastIndexOf('.');
                        name = enumTyp.Name;
                    }

                    int start = enumTyp.FullName.LastIndexOf('.', last - 1);

                    string super = enumTyp.FullName.Substring(start + 1, last - start - 1);
                    string cbase = super + '.' + enumTyp.Name;

                    if (name == null)
                        name = super;

                    bool hasData = GlobalDat[cbase] != null;

                    if (GlobalProp[group, subType] != null)
                    {
                        // Tipo ya definido: Puede ser un error
                        // Si es el mismo tipo no debe hacer nada

                        if (fields.Length >= offset)
                        {
                            FieldInfo field = fields[offset];
                            string fname = name + '.' + field.Name;

                            if (!Str.Equals(fname, GlobalProp[group, subType][0].Name))
                            {
                                OnEvent(Events.DupEvent, enumTyp.ToString());
                                return;
                            }
                        }
                    }
                    GlobalProp[group, subType] = new Info[total - offset];

                    for (int nPos = 0; nPos < total; nPos++)
                    {
                        if (nPos >= offset)
                        {
                            FieldInfo field = fields[nPos];

                            Info[] list = (Info[])field.GetCustomAttributes(typeof(Info), false);
                            Info attrib = null;

                            if (list != null && list.Length > 0)
                            {
                                attrib = list[0];
                                attrib.Name = name + '.' + field.Name;
                            }

                            // if (Data.Equals(attrib.Name, "WebExpor.ExporStart"))
                            // {
                            // }

                            if (hasData)
                            {
                                Info redef = (Info)GlobalDat[AppNorm.GetIden(field.Name, cbase)];

                                if (redef != null)
                                {
                                    if (attrib == null)
                                        attrib = redef;
                                    else
                                    {
                                        // if (redef.Action != OpAction.None)
                                        // if (redef.Action != default(Enum))
                                        if (redef.Action != 0)
                                            attrib.Action = redef.Action;

                                        if (redef.Title != null)
                                            attrib.Title = redef.Title;

                                        if (redef.Params != null)
                                            attrib.Params = redef.Params;
                                    }
                                }
                            }

                            if (attrib != null)
                            {
                                GlobalProp[group, subType][nPos - offset] = attrib;
                                // GlobalProp[group].Mens[nPos-offset] = attrib;
                                OpEvent.IndexParams(attrib);
                            }
                        }
                    }
                }
            }
            catch (Exception oExc)
            {
            }
        }

        /// <summary> Comprueba si un tipo esta redefinido en el fichero
        /// Solo se comprueba la re-definicion mediante fichero de tipos
        /// </summary>
        /// <param name="enumTyp"> Enumerado demensjes a definir </param>
        /// <returns> Resultado de la comprobacion </returns>

        public static bool IsDefined(Type enumTyp)
        {
            int start = enumTyp.FullName.IndexOf('.');
            string cbase = enumTyp.FullName.Substring(start + 1);
            return GlobalDat[cbase] != null;
        }

        /// <summary> Comprueba si un codigo local de evento crea espera
        /// La espera es cualquier mensaje que origina entrada de usuaario
        /// </summary>
        /// <typeparam name="T">Tipo Action locla de una clase TBase  </typeparam>
        /// <param name="oper"> Codigo de operacion local en la clase </param>
        /// <returns> Indica si el evento crea una espera </returns>

        public static bool IsWaitEvent(object oper)
        {
            int code = Data.ToInt(oper);

            Info info = Tables.GetInfo(code);

            if (info != null)
            {
                OpAction action = (OpAction)info.Action;

                return OpEvent.IsWait(action);
            }

            return false;
        }

        #endregion

        #region EVENTO GLOBAL A NIVEL DE LA APLICACION

        // Declaracion del evento a nivel de aplicacion
        public event AppEventHandler AppEvent;

        // Declaracion del manejador para el evento
        public delegate void AppEventHandler(OpEvent oArg);

        /// <summary> Emision del evento a nivel de aplicacion
        /// Se utiliza por defecto si falta el evento de instancia
        /// </summary>
        /// <param name="oArg"> Mensaje completo del evento </param>

        protected override bool OnEvent(OpEvent oArg)
        {
            bool resul = true;

            if (!oArg.IsLogged)
                Logger.LogError(oArg.Error);

            if (AppEvent != null)
            {
                AppEvent(oArg);
                resul = oArg.Resul > OpResul.Cancel;
            }
            return resul;
        }

        #endregion

        #region REFERENCIA ESTATICA Y ACCESO GLOBAL A LA CLASE

        /// <summary> Retorna referencia a la clase soporte de aplicacion
        /// </summary>
        /// <returns> Referencia a la clase soprte de aplicacion </returns>

        public static AppData Tables
        {
            get
            {
                if (GlobalAppData == null)
                    GlobalAppData = new AppData();

                return GlobalAppData;
            }
        }
        static AppData GlobalAppData; // Referencia global a la clase

        #endregion 


        #region DEFINICION DE MENSAJES

        public enum Events
        {
            None,

            [Info(OpAction.Error, "Definción del area {0} duplicada [en {1}]")]
            DupArea = EnumCore.AppData,

            [Info(OpAction.Error, "Definición de eventos {0} duplicada [en {1}]")]
            DupEvent,
        }
        #endregion 

    }



}
