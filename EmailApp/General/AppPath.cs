using System.Reflection;

namespace MailAppMAUI.General
{
    /// <summary> Gestiona la creacion de directorios usados por la aplicacion
    /// </summary>

    public static class AppPath
    {
        private static readonly char cSepar;            // Separador de directorios del sistema
        private static string dirBase = null;  // Directorio de inicio de la aplicacion
        private static string fileCon = null;  // Nombre del fichero de configuracion 
        private static string dirConfig = null;  // Directorio general de configuracion
        private static string dirApplic = null;  // Directorio especifico de configuracion
        private static string dirData = null;  // Directorio especifico de datos
        private static string AppName = null;  // Nombre especifico de la aplicacion

        static AppPath()
        {
            cSepar = Path.DirectorySeparatorChar;
        }

        /// <summary> Consulta o modifica el directorio Base de la aplicacion
        /// 
        /// Este directorio se usa unicamente para cargar valores de configuracion
        /// Una vez cargada se tienen en cuenta los valores y carpetas configurados
        /// 
        /// Suele usar normalmente el directorio de arranque de la aplicacion
        /// Se puede indicar otro distinto si la aplicacion trabaja enlazada
        /// a otra carpeta o si se invoca desde codigo no administrado o ActiveX
        /// Este cambio debe hacerse antes de usar ningun valor de configuracion
        /// </summary>

        public static string DirBase
        {
            get
            {
                if (dirBase == null)
                    DefineBaseDir();

                return dirBase;
            }

            set
            {
                if (!Str.Empty(value))
                    dirBase = NormDir(value);
            }
        }

        public static string DirLogger
        {
            get
            {
                return dirBase + @"Log\";
            }
        }

        /// <summary> Consulta o modifica directorio Base de la aplicacion
        /// 
        /// Este valor se utiliza para comprobar ficheros de configuracion
        /// Se puede definir antes de cargar el fichero de configuracion
        /// </summary>

        public static string DataFolder
        {
            get { return dirData; }
            set { dirData = NormDir(value); }
        }

        /// <summary> Retorna directorio global de configuracion
        /// </summary>

        public static string DirConfig
        {
            get
            {
#if WindowsCE
                    return DirBase;
#else
                if (dirConfig == null)
                {
                    DefineBaseDir();
                    return dirConfig;
                }
                return string.Empty;
#endif
            }
            private set
            {
                dirConfig = value;
            }
        }

        /// <summary> Consulta el nombre de un fichero de configuracion
        /// Si se da un nombre devuelve el camino completo del fichero
        /// Si es una cadena vacia devuelve directorio de configuracion
        /// </summary>
        /// <remarks>
        /// Existen dos carpetas para buscar ficheros de configuracion:
        /// 
        /// - Carpeta de configuracion general
        ///   Normalmente sera la carpeta Config sobre el directorio base
        ///   
        /// - Carpeta de configuracion de una aplicacion
        ///   Es una subcarpeta sobre Config con el nombre de la aplicacion
        ///   Puede contener configuraciones especificas de una aplicacion
        ///   
        ///   Para leer un fichero se usa primero el de la aplicacion
        ///   Pero para crearlo se ubica sobre la carpeta Config general
        ///   Por tanto cada aplicacion puede definir variables especificas
        ///   Pero al salvar debe uusarse un fichero de configuracion comun
        /// 
        /// </remarks>

        public static string FileConfig(string name)
        {
            if (name == null)
            {
                // Nombre por defecto del fichero de configuracion
                name = fileCon;
            }
            else
            {
                // Nombre vacio: Devolver carpeta de configuracion
                if (name == string.Empty)
                    return dirConfig;
            }

            string resul;
            // Buscar carpeta mas especifica con el fichero
            if (DataFolder != null)
            {
                if (AppName != null)
                {
                    // Directorio por empresa y aplicacion
                    resul = GetDir(DataFolder, AppName);
                    resul = GetFile(resul, name);

                    if (File.Exists(resul))
                        return resul;
                }

                // Directorio especifico por empresa
                resul = GetFile(DataFolder, name);

                if (File.Exists(resul))
                    return resul;
            }

            // Directorio especifico por aplicacion
            resul = dirApplic + name;

            if (File.Exists(resul))
                return resul;

            // Si no existe crear fichero en la carpeta Config
            // La configuracion se comparte entre aplicaciones
            resul = dirConfig + name;

            return resul;


            /*
            if (name != null)
            {
                // Nombre vacio: Devolver carpeta de configuracion
                if (name == String.Empty)
                    resul = dirConfig;
                    // resul = dirApplic;
                else
                {
                    // Devolver nombre completo de un fichero
                    resul = dirApplic + name;

                    if (!File.Exists(resul))
                    {
                        // Si no existe crear fichero en la carpeta Config
                        // La configuracion se comparte entre aplicaciones
                        resul = dirConfig + name;
                    }
                }
            }
            else
            {
                // Nombre completo del fichero de configuracion
                resul = fileCon;

                if (resul == null)
                    resul = GetNameExec() + ".config";

                resul = dirApplic + fileCon;

                if (!File.Exists(resul))
                {
                    // Si no existe crear fichero en la carpeta Config
                    // La configuracion se comparte entre aplicaciones
                    resul = dirConfig + fileCon;
                }
            }

            return resul;
            */
        }

        /// <summary> Define el nombre base de la aplicacion actual
        /// 
        /// Puede ser simple o con varias partes separadas por punto
        /// No puede incluir caracteres separadores de directorio
        /// 
        /// Se utiliza como  clave para definir datos especificos:
        /// 
        ///  - Fichero de configuracion: Nombre con extension config
        ///  - Carpeta de configuracion: Subcarpeta con este nombre
        ///  
        /// </summary>
        /// <param name="app"> Nombre de la aplicacion </param>

        public static void SetAppName(string app)
        {
            app ??= Path.GetFileNameWithoutExtension(GetFileExec());

            int index = app.IndexOf('.');

            // PowerGest.Tienda --> PowerGest.config

            if (index > 0)
            {
                fileCon = string.Concat(app.AsSpan(0, index), ".Config");
                app = app[(index + 1)..];
            }
            else
            {
                fileCon = app + ".Config";
            }
            AppName = app;

            string config = GetFile(DirConfig, app) + '\\';

            if (Directory.Exists(config))
                dirApplic = config;

            // Calcualr directorios: Nuevo +++
            // DefineBaseDir();
        }

        /// <summary> Define directorio base y directorio de configuracion
        /// 
        /// El criterio de calculo de estos directorios es el siguiente:
        /// 
        /// - Primero se busca la carpeta Config en el path de arranque
        /// - Si es un proyecto Web se busca Config dos niveles abajo
        /// - Si es otro proyecto Web se busca Config tres niveles abajo
        /// - En cualquier otro caso se usa el el path de arranque 
        /// </summary>
        /// <remarks>
        /// 
        /// La carpeta Config indica siempre el directorio de configuracion
        /// En esta caso el path base es siempre el directorio anterior
        /// 
        /// Con estos criterios se logra:
        /// 
        /// - Si no hay carpeta Config todo esta en el path de arranque
        /// - Si hay carpeta config el directorio base es el anterior
        /// - En el entorno el directorio base es siempre el de la solucion
        /// 
        /// </remarks>

        public static void DefineBaseDir()
        {
            if (dirConfig == null)
            {
                // Buscar directorio especifico de configuracion
                string direct = GetDirExec();
                string config = GetFile(direct, @"Config\");

                if (!Directory.Exists(config))
                {
                    // Directorio para entorno Vs de proyectos Web
                    if (direct.EndsWith(@"\AppX\"))
                    {
                        // La carpeta base en la Web es el contenedor de bin
                        // 
                        // Si la dll ejecutable esta en: host/app/bin
                        // La carpeta base (aplicacion): host/app 
                        // La carpeta de estructuras es: host/app/est
                        // La carpeta de configuracion:  host/app/config
                        // 
                        // En el desarrollo se puede poner un nivel abajo
                        // Asi se usarian las mismas carpetas que WinForm
                        // Esto es util para proyectos Web/Winforms
                        // Las carpetas estructuras/config se comparten
                        // ---------------------------------------------
                        config = GetFile(direct, @"..\..\..\..\..\..\Config\");
                        if (!Directory.Exists(config))
                        {
                            // Comprobar la configuracion de desarrollo
                            config = GetFile(direct, @"..\..\Config\");

                            if (!Directory.Exists(config))
                                config = null;
                        }
                    }
                    else
                    {
                        // Directorio para entorno Vs de otros proyectos
                        if (GetFile(direct, @"..\").Contains(@"\bin\"))
                        {
                            // El proyecto puede estar o no en una subcarpeta
                            // Se comprueba la capeta config en ambos niveles
                            config = GetFile(direct, @"..\Config\");
                            if (!Directory.Exists(config))
                            {
                                config = GetFile(direct, @"..\..\..\Config\");

                                if (!Directory.Exists(config))
                                    config = null;
                            }
                        }
                    }
                }

                // Usar directorio Config como referencia si existe
                if (config != null)
                {
                    dirBase = GetFile(config, @"..\");
                    dirConfig = config;
                }
                else
                {
                    // No hay directorio base: Usar path de arranque
                    dirBase = direct;
                    dirConfig = direct;
                }
            }

            dirApplic ??= dirConfig;
        }

        /// <summary> Comprueba fichero de configuracion de usuario .user
        /// Esta extension tiene siempre prioridad sobre la extension .config
        /// El fichero se busca en la carpeta de especifica o de configuracion
        /// </summary>

        public static string CheckUser(string file)
        {
            string name = Path.GetFileName(file);
            name = Path.ChangeExtension(name, ".user");

            string direc = FileConfig(name);

            if (File.Exists(direc))
                return direc;

            return file;
        }

        /// <summary> Devuelve el nombre base del fichero de arranque 
        /// </summary>
        /// <returns> Nombre base del fichero de arranque </returns>

        public static string GetNameExec()
        {
            return Path.GetFileNameWithoutExtension(GetModExec());
        }

        /// <summary> Devuelve el nombre del fichero de arranque 
        /// Se retorna el nombre simple con extension del fichero 
        /// </summary>
        /// <returns> Nombre del fichero de arranque </returns>

        public static string GetFileExec()
        {
            return Path.GetFileName(GetModExec());
        }

        /// <summary> Devuelve directorio del modulo de arranque 
        /// El resultado esta normalizado y con separadores del sistema
        /// La cadena se acaba con el caracter separador de directorios
        /// </summary>
        /// <returns> Directorio de arranque </returns>

        public static string GetDirExec()
        {
            string DirExec = Path.GetDirectoryName(GetModExec());

            if (DirExec[^1] != cSepar)
                DirExec += cSepar;

            return DirExec;
        }


        /// <summary> Devuelve modulo de inicio de la aplicacion
        /// 
        /// El modulo de inicio es el primer ejecutable de la aplicacion
        /// Si es codigo no gestionado devuelve el camino la DLL actual
        /// 
        /// </summary>

        public static string GetModExec()
        {
            // Obtener el nombre del fichero de ensamblado inicial
            // Si es codigo no administrado se usa el nombre de DLL actual
            // El resultado es el Path absoluto devuelto por el sistema
            // No se normaliza ni se cambian separadores de directorios

            Assembly AsEntry;
            string DirExec;

#if WindowsCE

            AsEntry = Assembly.GetExecutingAssembly();
            DirExec = AsEntry.ManifestModule.FullyQualifiedName;

#else

            AsEntry = Assembly.GetEntryAssembly();
            if (AsEntry == null)
                AsEntry = Assembly.GetExecutingAssembly();

            AssemblyName AsName = AsEntry.GetName();
            Uri AsUri = new(AsName.CodeBase);
            DirExec = AsUri.AbsolutePath;

#endif

            return DirExec;
        }

        /// <summary> Busca un fichero en los directorios superiores a uno dado
        /// Retorna path absoluto del fichero en el primer directorio encontrado
        /// </summary>
        /// <param name="cName"> Nombre del fichero a buscar
        /// Si contiene un directorio se extrae el nombre de fichero
        /// </param>
        /// <param name="cBase"> Directorio base de inicio de la busqueda
        /// Si esta vacio utiliza el directorio del nombre del fichero 
        /// </param>
        /// 
        /// <returns> Directorio completo del fichro encontrado
        /// Si no existe retorna null
        /// </returns>

        public static string FindUpperDir(string cName, string cBase)
        {
            string DirFile = null;
            bool lFound = false;

            if (!Str.Empty(cName))
            {
                if (Str.Empty(cBase))
                    cBase = Path.GetDirectoryName(cName);

                cName = Path.GetFileName(cName);
                cBase = NormDir(cBase);

                while (!lFound && !Str.Empty(cBase))
                {
                    lFound = File.Exists(cBase + cName);

                    if (!lFound)
                        cBase = GetFile(cBase, "..\\");
                }

                if (lFound)
                    DirFile = cBase + cName;
            }

            return DirFile;
        }

        public static string GetRelPath(string path, string dirBase)
        {
            if (!Str.Empty(path) && !Str.Empty(dirBase))
            {
                // D:\dir1\dir2\base\dir3\dir4\file.txt

                int start = path.Length - 1;
                int index = path.LastIndexOf('\\', start);

                if (index >= 0)
                {
                    dirBase = dirBase.Trim();
                    if (!dirBase.EndsWith('\\'))
                        dirBase += "\\";

                    int nLen = dirBase.Length;
                    int nFile = -1;

                    while (index >= 0)
                    {
                        if (nFile != -1 && index >= nLen)
                        {
                            if (string.Compare(dirBase, 0, path, index - nLen + 1, nLen, true) == 0)
                            {
                                return path[(index + 1)..];
                            }
                        }

                        start = index - 1;

                        if (nFile == -1)
                            nFile = index + 1;

                        if (start >= 0)
                            index = path.LastIndexOf('\\', start);
                        else
                            break;
                    }

                    if (nFile != -1)
                        path = path[nFile..];
                }
            }

            return path;
        }



        /// <summary> Obtiene el directorio base de un fichero
        /// El resultado se da con el separador de directorio
        /// </summary>
        /// <param name="file"> Path completo del fichero </param>
        /// <returns> Directorio base del fichero </returns>

        public static string GetUpper(string file)
        {
            if (!Str.Empty(file))
            {
                int nlen = file.Length;

                if (nlen > 0 && file[nlen - 1] == cSepar)
                    file = file[..(nlen - 1)];

                file = Path.GetDirectoryName(file);

                if (file.Length > 0)
                {
                    if (file[^1] != cSepar)
                        file += cSepar;
                }
            }

            return file;
        }

        /// <summary> Retorna ultimo componente de un path
        /// </summary>
        /// <param name="path"> patah a analizar </param>
        /// <returns> Ultimo fichero o carpeta </returns>

        public static string GetLastItem(string path)
        {
            int nlen = path.Length;

            if (nlen > 0 && path[nlen - 1] == cSepar)
                path = path[..(nlen - 1)];

            path = Path.GetFileName(path);

            return path;
        }

        /// <summary> Retorna directori completo de un fichero
        /// No toca los ficheros dados con un path absoluto
        /// </summary>
        /// <param name="file">  Nombre o path del Fichero </param>
        /// <param name="folder">Directorio base </param>
        /// <returns> Directorio completo del fichero </returns>

        public static string GetFilePath(string file, string folder)
        {
            if (!Path.IsPathRooted(file))
            {
                file = GetFile(folder, file);
            }

            return file;
        }

        /// <summary> Combina directorio base con el directorio relativo
        /// Se utiliza para crear paths absolutos de la aplicacion
        /// Los directorios deben referirse a un directorio base
        /// Admite uso de .. como referencia a directorios superiores
        /// </summary>
        /// 
        /// <param name="sDataPath"> Cadena con el directorio base  </param>
        /// <param name="sDirPath">  Directorio relativo a combinar </param>
        /// 
        /// <returns> Cadena combinada de directorios </returns>

        public static string GetFile(string DirBase, string DirPath)
        {
            int nPos;
            if (!string.IsNullOrEmpty(DirBase))
            {
                nPos = DirBase.Length - 1;

                // Si el path base no tiene barra final se la añadimos
                if (DirBase[nPos] != cSepar)
                    DirBase += cSepar;

                // retroceder directorios sobre el base si se pide
                // while (DirPath.Contains( ".." + cSepar ))
                if (DirPath == null)
                    DirPath = DirBase;
                else
                {
                    while (DirPath.Contains(".." + cSepar, StringComparison.CurrentCulture))
                    {
                        if (nPos > 0)
                        {
                            if (DirBase[nPos] == cSepar)
                                nPos--;

                            nPos = DirBase.LastIndexOf(cSepar, nPos);
                            if (nPos > 0)
                                DirBase = DirBase[..(nPos + 1)];
                        }

                        DirPath = DirPath[3..];
                    }
                    DirPath = DirBase + DirPath;
                }
            }

            // retornar directorios combinados
            return DirPath;
        }

        /// <summary> Combina un directorio base con un subdirectorio
        /// Añade siempre separador de directorio al final si no existe
        /// No puede usarse para combinar un fichero sobre una carpeta
        /// </summary>
        /// <param name="dirBase"> Cadena con el directorio base  </param>
        /// <param name="dirRel">  Directorio relativo a combinar </param>
        /// <returns> Directorio combinado </returns>

        public static string GetDir(string dirBase, string dirRel)
        {
            if (dirRel != null)
            {
                dirBase = GetFile(dirBase, dirRel);

                int length = dirBase.Length;
                if (length > 0)
                {
                    if (dirBase[length - 1] != cSepar)
                        dirBase += cSepar;
                }
                return dirBase;
            }
            return null;
        }

        /// <summary> Combina directorio base con el nombre de un fichero
        /// Este metodo comprueba si el fichero tiene ya un directorio
        /// Solo se modifica el fichero dado si no tiene ya un directorio
        /// Admite uso de .. como referencia a directorios superiores
        /// </summary>
        /// <param name="cBase"> Cadena con el directorio base   </param>
        /// <param name="cFile"> Cadena con el nombre de fichero </param>
        /// <returns> Cadena combinada del directorio y fichero  </returns>

        public static string DefaultDir(string cBase, string cFile)
        {
            if (Path.GetFileName(cFile) == cFile)
                cFile = GetFile(cBase, cFile);

            return cFile;
        }

        /// <summary> Normaliza y combina dos directorios
        /// </summary>
        /// <param name="dir1"> Directiro base inicial </param>
        /// <param name="dir2"> Foder o path relativo  </param>
        /// <returns> Directorio compuesto </returns>

        public static string Combine(string dir1, string dir2, bool isFile = false)
        {
            string resul;
            if (!Str.Empty(dir1))
            {
                dir1 = NormDir(dir1);

                if (!Str.Empty(dir2))
                {
                    if (!isFile)
                        isFile = dir2.Contains('.');

                    dir2 = NormDir(dir2, isFile);
                    resul = Path.Combine(dir1, dir2);
                }
                else
                    resul = dir1;
            }
            else
            {
                resul = NormDir(dir1);
            }

            return resul;
        }


        /// <summary> Normaliza una cadena de directorio
        /// 
        /// Substituye el separador por el apropiado del sistema
        /// Añade la barra final de directorio si no la tiene
        /// 
        /// </summary>
        /// 
        /// <param name="DirPath"> Directorio a estandarizar </param>
        /// 
        /// <returns> Cadena con el directorio normalizado </returns>

        public static string NormDir(string DirPath, bool isFile = false)
        {
            if (!string.IsNullOrEmpty(DirPath))
            {
                // Obtenemos los separados para el reemplazo
                char SepIni = '\\';
                char SepFin = '/';

                if (cSepar == '\\')
                {
                    SepIni = '/';
                    SepFin = '\\';
                }

                // Reeemplazamos los separadores de directorios
                DirPath = DirPath.Replace(SepIni, SepFin);

                // Eliminar la barra inicial del directorio
                // Esto no debe hacerse para estandarizar directorio
                // Comprobar efecto de suprimir este codigo 
                // Verificar con paths relativos del tipo ..\..\Est
                // 
                // if (DirPath[ 0 ] == cSepar)
                //     DirPath = DirPath.Substring( 1, DirPath.Length - 1 );

                // Añadimos la barra final a los directorios
                if (!isFile)
                {
                    if (DirPath[^1] != cSepar)
                        DirPath += cSepar;
                }
            }
            return DirPath;
        }

        /// <summary> Compara lista de mascaras separada por punto y coma
        /// </summary>
        /// <param name="name"> Nombre del fichero a comparar </param>
        /// <param name="mask"> Mascaras de ficheros validos  </param>
        /// <returns> Resultado de la comparacion </returns>

        public static bool CompareMasks(string name, string masks)
        {
            foreach (string mask in Str.Scan(masks, ';'))
            {
                if (!Str.Empty(mask) && CompareMask(name, mask))
                    return true;
            }

            return false;
        }

        /// <summary> Compara un nombre de fichero con una mascara
        /// Las mascara admite los comodines standard del S.O (* y ?)
        /// Ademas se admite un comodin especial (#) para digitos
        /// Si la mascara es nula retorna siempre comparacion valida
        /// </summary>
        /// <param name="name"> Nombre del fichero a comparar </param>
        /// <param name="mask"> Mascara de ficheros validos  </param>
        /// <returns> Resultado de la comparacion </returns>

        public static bool CompareMask(string name, string mask)
        {
            if (mask == null || mask == string.Empty)
                return true;

            if (name == null)
                return false;

            int nMask = mask.Length;
            int nName = name.Length;
            bool IsAny = false;
            bool IsExt = false;
            bool resul = true;
            int offset = 0;

            for (int ind = 0; ind < nMask; ind++)
            {
                if (!resul)
                    break;

                char cMask = char.ToUpper(mask[ind]);

                if (ind + offset >= nName)
                {
                    if (!IsAny && cMask != '?' && cMask != '*' && cMask != '.' ||
                        ind < nMask - 1)
                        resul = false;

                    break;
                }
                char cName = char.ToUpper(name[ind + offset]);

                switch (cMask)
                {
                    case '.':
                        // Extension: Obtener la ultima del nombre
                        IsAny = false;
                        resul = cName == '.';
                        int exten = name.LastIndexOf('.');
                        IsExt = nName - exten == nMask - ind;

                        if (exten != ind + offset)
                            offset = exten - ind;

                        break;

                    case '*':
                        IsAny = true;

                        if (cName == '.')
                            offset--;
                        else
                        {
                            while (ind + offset < nName - 1)
                            {
                                if (name[offset + ind] == '.')
                                {
                                    if (offset > 0)
                                        offset--;

                                    break;
                                }

                                offset++;
                            }
                        }
                        continue;

                    case '?':
                        continue;

                    case '#':
                        resul = char.IsDigit(cName);
                        break;

                    default:
                        resul = IsAny || cName == cMask;
                        break;
                }
            }

            if (resul && nName > nMask && !IsAny && !IsExt)
                resul = false;

            return resul;
        }
    }
}
