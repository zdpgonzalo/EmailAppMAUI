
#if PocketPC
#define WindowsCE
#endif

using System.Collections;
using System.Text;
using System.Reflection;

#if !WindowsCE
using System.IO.IsolatedStorage;
using System.Diagnostics;
using MailAppMAUI.General;
#endif

//[assembly: CLSCompliant(false)]

namespace MailAppMAUI.General
{
    /// <summary> Clases para independizar version del sistema 
    /// </summary>

    public static class AppSystem
    {
#pragma warning disable 618

        #region DEFINICION DE LA CLASE

        static private bool m_Version2;              // Version 2 o mayor de FrameWork
        static private bool m_Isolated;              // Utilizar almacenamiento aislado
#if !WindowsCE
        static private IsolatedStorageFile AppStore; // Almacenamiento aislado aplicacion 
#endif


        /// <summary> Caracteristicas del sistema y la plataforma
        /// 
        /// Clase estatica Environment
        /// 
        /// Version.Mayor: Version mayor del sistema (2 para CF20)
        /// Version.Minor: Version menor del sistema (0 pata CF20)
        /// 
        /// OsVersion.Platform: Plataforma WIndows   (WinCE para Windows CE)
        /// OsVersion.Mayor: Version mayor del OS    (2 para Windows CE 5.0)
        /// OsVersion.Minor: Version menor del OS    (0 para Windows CE 5.0)
        /// 
        /// Resultados para los sistemas Windows CE
        /// 
        /// Sistema                Mayor Minor Platform Alias 
        /// ---------------------- ----- ----- -------- -----------------
        /// Windows Mobile 2003 SE   4    21   WinCE    Pocket PC 2003 SE
        /// Windows CE 5.0           5     0   WinCE    
        /// Windows Mobile 5.0       5     1   WinCE
        /// Windows Mobile 6.0       5     2   WinCE
        /// 
        /// </summary>

        static AppSystem()
        {
            m_Isolated = false;
            m_Version2 = Environment.Version.Major >= 2;
#if !WindowsCE
            AppStore = null;
#endif
            StackBuilder = new StringBuilder();


        }

        /// <summary> Define si se utiliza almacenamient aislado
        /// El almacenamiento aislado evita problemas de seguridad
        /// ya que el espacio de cada fichero lo asigna framework
        /// </summary>

        public static bool Isolated
        {
            get { return m_Isolated; }
            set
            {
#if WindowsCE
                m_Isolated = false; 
#else
                m_Isolated = value;
#endif
            }
        }

        /// <summary> Formularios con barra de herrameintas abajo
        /// Esto ocurre a partir de la version 5.1 (Windows Mobile)
        /// En versiones 5.0 (Win CE 50) esta encima del formulario
        /// Si la barra de herramientas esta en la parte superior
        /// tapa las pestañas del TabControl y hay que bajar todo
        /// </summary>

        public static bool IsToolDown
        {
            get
            {
#if WindowsCE
                return Environment.OSVersion.Version.Major > 5 ||
                       Environment.OSVersion.Version.Minor > 0;
#else
                return false;
#endif
            }
        }

        #endregion

        #region INSTANCIA DE ESTRUCTURAS DE DATOS

        /// <summary> Crea Hashtable que distingue mayusculas y minusculas
        /// Esta opcion es mas rapida si las claves estan ya normalizadas
        /// </summary>
        /// <returns> Clase Hashtable creada </returns>

        public static Hashtable GetHashTable()
        {
            return new Hashtable();
        }

        /// <summary> Crea HashTable independiente de mayusculas/minusculas
        /// Segun version del FrameWork utiliza el constructor recomendado
        /// </summary>
        /// <returns> Nueva clase HashTable creada </returns>

        public static Hashtable GetHashTable(bool Ignore)
        {
            Hashtable oList;

            if (m_Version2)
            {
                oList = new Hashtable(
                        StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                oList = new Hashtable(
                        CaseInsensitiveHashCodeProvider.DefaultInvariant,
                        CaseInsensitiveComparer.DefaultInvariant);
            }
            return oList;
        }
        #endregion

        #region METODOS DE SOPORTE Y MANTENIMIENTO

        public static bool IsExecuting(string name)
        {
            Process[] pname = Process.GetProcessesByName(name);

            return pname != null && pname.Length > 0;
        }

        public static bool KillProc(string name)
        {
            bool resul = false;

            Process[] pname = Process.GetProcessesByName(name);

            if (pname != null)
            {
                foreach (var proc in pname)
                {
                    proc.Kill();
                    resul = true;
                }
            }

            return resul;
        }


        public static void Execute(string file, string pars = null)
        {
            // Preparar proceso del ejecutable
            Process print = new Process();

            print.StartInfo.FileName = file;
            print.StartInfo.UseShellExecute = true;
            print.StartInfo.CreateNoWindow = true;
            print.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // Arrancar el proceso con los parametros opcionales dados
            // Cuidado: No poner el objeto Process en una inspeccion
            // Solo por la inspeccion se cargan las variables de entorno
            // Con variables de entorno da error con ShellExecute = true
            // ---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(pars))
                print.StartInfo.Arguments = pars;

            print.Start();
        }


        private static StringBuilder StackBuilder;

        public enum StackInfo { Short, ShortLines, ShortError, Methods, Params, Lines, Files, ErrorTrace, FullTrace };

        public static string GetStack()
        {
            return GetStack(StackInfo.Lines);
        }

        public static string GetStack(StackInfo mode)
        {
            StackTrace st = new StackTrace(0,
                      mode >= StackInfo.Lines || mode == StackInfo.ShortLines);

            StackBuilder.Length = 0;
            int offset = mode == StackInfo.ShortLines ? 4 : 3;
            bool isEvent = false;
            bool isError = false;

            if (mode == StackInfo.ErrorTrace || mode == StackInfo.ShortError)
            {
                isError = true;

                if (mode == StackInfo.ErrorTrace)
                    mode = StackInfo.Lines;
                else
                    mode = StackInfo.ShortLines;
            }

            for (int ind = offset; ind < st.FrameCount; ind++)
            {
                StackFrame sf = st.GetFrame(ind);
                string name = sf.GetMethod().DeclaringType.FullName;
                string method = sf.GetMethod().Name;
                bool isAppIfs = Str.Starts(name, "Ifs.");

                if (mode < StackInfo.FullTrace)
                {
                    // if (name.StartsWith("System.", StringComparison.InvariantCultureIgnoreCase))
                    if (Str.Starts(name, "System."))
                        continue;

                    // if (ind < 4 && name.EndsWith(".Logger", StringComparison.InvariantCulture))
                    if (ind < 4 && name.EndsWith(".Logger"))
                        continue;

                    if (mode <= StackInfo.ShortLines && Str.Equals(method, "Main"))
                        continue;

                    if (ind < 7 && isAppIfs && isError)
                    {
                        if (Str.Starts(method, "OnEvent"))
                        {
                            // Llamada a la gestion de eventos 
                            isEvent = true;
                            continue;
                        }
                        else
                        {
                            if (isEvent)
                            {
                                // Llamada siguiente a gestion de eventos
                                isEvent = false;
                                continue;
                            }
                        }
                    }
                }

                if (StackBuilder.Length > 0)
                {
                    if (mode <= StackInfo.ShortLines)
                        StackBuilder.Append(' ');
                    else
                        StackBuilder.AppendLine();
                }

                if (mode == StackInfo.Files)
                    StackBuilder.AppendFormat("{0, -12} ",
                    Path.GetFileNameWithoutExtension(sf.GetFileName()));

                int line = sf.GetFileLineNumber();

                if (line == 0 && !isAppIfs && mode <= StackInfo.ShortLines)
                    continue;

                if (mode >= StackInfo.Lines)
                {
                    if (line > 0)
                    {
                        string cline = (line.ToString() + ':').PadRight(7);
                        StackBuilder.Append(cline);
                        // StackBuilder.AppendFormat("{0, 5}: ", line);
                    }
                    else
                    {
                        StackBuilder.Append(' ', 7);
                    }
                }

                StackBuilder.Append(sf.GetMethod().DeclaringType.Name);
                StackBuilder.Append('.');
                StackBuilder.Append(sf.GetMethod().Name);

                if (mode == StackInfo.ShortLines)
                {
                    line = sf.GetFileLineNumber();

                    if (line > 0)
                        StackBuilder.AppendFormat("({0})", line);
                }

                if (mode >= StackInfo.Params)
                {
                    ParameterInfo[] list = sf.GetMethod().GetParameters();

                    StackBuilder.Append('(');

                    for (int par = 0; par < list.Length; par++)
                    {
                        if (par > 0)
                            StackBuilder.Append(',');

                        StackBuilder.Append(' ');

                        StackBuilder.Append(list[par].ParameterType.Name);
                        StackBuilder.Append(' ');
                        StackBuilder.Append(list[par].Name);
                    }

                    StackBuilder.Append(')');
                }
            }

            return StackBuilder.ToString();
        }


        #endregion

        #region SOPORTE DE METODOS DE EXTENSION 

        /// <summary> Retorna array de nombres de un enumerado
        /// Este metodo existe en Windows pero no en WindowsCE
        /// Por tanto en este sistema se obtiene por reflexion
        /// </summary>
        /// <param name="typ"> Tipo enumerado a comprobar </param>
        /// <returns> Array con nombres de enumerados </returns>

        public static string[] EnumGetNames(Type typ)
        {
#if WindowsCE

            FieldInfo[] fields = typ.GetFields();
            string[] names = null;
            int count = 0;

            foreach(FieldInfo field in fields)
            {
                if (!field.IsSpecialName)
                    count++;
            }

            if (count > 0)
            {
                names = new string[count];
                count = 0;
                foreach(FieldInfo field in fields)
                {
                    if (!field.IsSpecialName)
                        names[count++] = field.Name;
                }
            }
            return names;

#else
            return Enum.GetNames(typ);
#endif
        }

        /// <summary> Retorna array de valores de un enumerado
        /// Este metodo existe en Windows pero no en WindowsCE
        /// Por tanto en este sistema se obtiene por reflexion
        /// </summary>
        /// <param name="typ"> Tipo enumerado a comprobar </param>
        /// <returns> Array con valores de los enumerados </returns>

        public static Array EnumGetValues(Type typ)
        {
#if WindowsCE

            FieldInfo[] fields = typ.GetFields();
            int count = 0;
            Array values = null;

            foreach(FieldInfo field in fields)
            {
                if (!field.IsSpecialName)
                    count++;
            }

            if (count > 0)
            {
                values = Array.CreateInstance(typ.UnderlyingSystemType, count);
                count = 0;
                foreach(FieldInfo field in fields)
                {
                    if (!field.IsSpecialName)
                        values.SetValue(field.GetValue(null), count++ );
                }
            }
            return values;

#else
            return Enum.GetValues(typ);
#endif
        }

        #endregion

        #region ACCESO A FICHEROS DEL SISTEMA

        /// <summary> Abre y retorna Stream generico para escritura
        /// Se utiliza almacenamiento aislado si esta configurado
        /// </summary>
        /// <param name="cName"> Nombre del fichero a abrir </param>
        /// <returns> Stream abierto segun plataforma </returns>

        public static Stream WriteStream(string cName)
        {
            return GetStream(cName, FileMode.OpenOrCreate, FileAccess.Write);
        }

        /// <summary> Abre y retorna Stream generico para escritura
        /// Se utiliza almacenamiento aislado si esta configurado
        /// </summary>
        /// <param name="cName"> Nombre del fichero a abrir </param>
        /// <returns> Stream abierto segun plataforma </returns>

        public static Stream AppendStream(string cName)
        {
            return GetStream(cName, FileMode.Append, FileAccess.Write);
        }

        /// <summary> Abre y retorna Stream generico para lectura
        /// Se utiliza almacenamiento aislado si esta configurado
        /// </summary>
        /// <param name="cName"> Nombre del fichero a abrir </param>
        /// <returns> Stream abierto segun plataforma </returns>

        public static Stream ReadStream(string cName)
        {
            return GetStream(cName, FileMode.Open, FileAccess.Read);
        }

        /// <summary> Abre un Stream generico para lectura y escritura
        /// El fichero se crea si no existe y se abre para lectura/escritura
        /// </summary>
        /// <param name="cName"> Nombre del fichero a abrir </param>
        /// <returns> Stream abierto segun plataforma </returns>

        public static Stream GetStream(string cName)
        {
            return GetStream(cName, FileMode.OpenOrCreate);
        }

        /// <summary> Abre y retorna Stream indicando modo de apertura
        /// Si este modo es solo Open abre el ficehro para lectura 
        /// En cualquier otro caso se abre para lectura y escritura
        /// Se utiliza almacenamiento aislado si esta configurado
        /// </summary>
        /// <param name="cName">  Nombre del fichero a abrir   </param>
        /// <param name="Mode">   Modo de apertura del fichero </param>
        /// <returns> Stream abierto segun plataforma </returns>

        public static Stream GetStream(string cName, FileMode Mode)
        {
            FileAccess Access;
            if (Mode == FileMode.Open)
                Access = FileAccess.Read;
            else
                Access = FileAccess.ReadWrite;

            return GetStream(cName, Mode, Access);
        }

        /// <summary> Abre y retorna Stream indicando modo y tipo de acceso
        /// Se usa almacenamiento aislado en rutas relativas si esta configurado
        /// Las rutas absolutas (referidas al raiz o unidad) se respetan siempre 
        /// </summary>
        /// <param name="cName">  Nombre del fichero a abrir   </param>
        /// <param name="Mode">   Modo de apertura del fichero </param>
        /// <param name="Access"> Tipo de acceso al fichero    </param>
        /// <returns> Stream abierto segun plataforma </returns>

        public static Stream GetStream(string cName, FileMode Mode, FileAccess Access)
        {
            if (Isolated && !Path.IsPathRooted(cName))
            {
#if !WindowsCE
                OpenAppStore();
                return new IsolatedStorageFileStream(cName, Mode, Access, AppStore);
#endif
            }

            FileStream file = null;

            try
            {
                file = new FileStream(cName, Mode, Access);
            }
            catch (Exception exc)
            {
            }

            if (file == null)
            {
                // Thread.Sleep(100);
                try
                {
                    file = new FileStream(cName, Mode, Access);
                }
                catch (Exception exc)
                {
                }
            }

            return file;

        }

        /// <summary> Crea directorio aislado o del sistema de ficheros
        /// Si se utiliza almacenamiento aislado todos los datos deben
        /// crearse y salvarse en el almacen asignado por el sistema
        /// En caso contrario se cre un directorio normal del sistema
        /// </summary>
        /// <param name="cName"> Nombre del directorio a crear </param>

        public static void CreateDirectory(string cName)
        {
            if (!string.IsNullOrEmpty(cName))
            {
                if (Isolated && !Path.IsPathRooted(cName))
                {
#if !WindowsCE
                    OpenAppStore();
                    AppStore.CreateDirectory(cName);
#endif
                }
                else
                {
                    Directory.CreateDirectory(cName);
                }
            }
        }

        /// <summary> Abre el almacen aislado usado en la aplicacion
        /// El almacen depende de la aplicacion y del usuario windows
        /// </summary>

        private static void OpenAppStore()
        {
#if !WindowsCE
            if (AppStore == null)
                AppStore = IsolatedStorageFile.GetStore(
                           IsolatedStorageScope.User |
                           IsolatedStorageScope.Domain |
                           IsolatedStorageScope.Assembly,
                           null, null);
#endif
        }

        #endregion
    }
}
