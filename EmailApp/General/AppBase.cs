using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MailAppMAUI.General
{
    public enum TestType
    {
        None,    // Desactivar el registro de eventos 
        Normal,  // Modo normal sin mensajes ni registros de prueba
        Trace,   // Modo soporte con mensajes y traza detallados
        Test,    // Modo prueba con mensajes y traza completa
        Debug,   // Modo desarrollo para traza y pruebas especiales 
    }

    /// <summary> Clase base con metodos generales de soporte
    /// </summary>

    public class AppBase
    {
        private AppBase()
        {
            //Instance ??= this;
        }

        public static AppBase GetAppBase()
        {
            return Instance;
        }
        private static AppBase _instance;
        private static AppBase Instance
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }

        #region GESTION DE PARAMETROS

        public enum AppParams
        {
            None,
            Result,      // Fichero resultado
        }

        public enum AppResult
        {
            None,
            Valid,
            Warning,
            Error
        }

        public string Params;      // Parametros del proceso
        public string FileResult;  // Fichero de resultado

        const string ResultDefault = "AppResult.ctr";

        /// <summary> Aplicacion de parametros por defecto
        /// Este metodo reconoce y aplica parametros genrales
        /// Se puede llamar ademas a metodos mas especificos 
        /// </summary>
        public void ApplyParams()
        {
            var options = GetParams<AppParams, string>(Params, true);

            Console.WriteLine(Params.Length);

            foreach (var (key, val) in options)
            {
                switch (key)
                {
                    case AppParams.Result:
                        FileResult = val;
                        break;
                }
            }
        }

        public (TKey key, TVal val)[] GetParams<TKey, TVal>(string pars = null, bool norm = false)
        where TKey : struct
        {
            pars ??= Params;

            var values = new List<(TKey, TVal)>();

            if (pars != null)
            {
                string[] options = pars.Split('-');

                if (options.Length > 0)
                {
                    foreach (string item in options)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            try
                            {
                                var (key, value) = Data.GetKeyValue(item);
                                key = Data.NormText(key);

                                if (norm && typeof(TVal) == typeof(string))
                                {
                                    value = Data.NormText(value);
                                }

                                if (Enum.TryParse(key, true, out TKey code))
                                {
                                    var val = (TVal)(object)value;
                                    values.Add((code, val));
                                }
                            }
                            catch (Exception exc)
                            {
                                ProcError("No se pueden leer los parametros", exc);
                            }
                        }
                    }
                }
            }

            return [.. values];
        }

        #endregion

        #region GESTION DE DIRECTORIOS

        /// <summary> Normaliza path añadiendo el separador adecuado
        /// </summary>
        /// <param name="path"> Patha a estandarizar </param>
        /// <returns> Path con separador final </returns>

        static public string GetFullPath(string path)
        {
            path = path.TrimEnd();

            if (!Path.IsPathRooted(path))
            {
                string dirbas = GetDirBase();

                char cAlter = Path.AltDirectorySeparatorChar;

                if (path.Contains(cAlter))
                    dirbas = dirbas.Replace(Path.DirectorySeparatorChar, cAlter);

                path = GetFile(dirbas, path);
            }
            return Path.GetFullPath(path);
            //return path;
        }

        /// <summary> Devuelve directorio base para paths relativos
        /// </summary>
        /// <returns> Directorio base </returns>
        /// <remarks>
        /// Si el path actual acaba en bin\release o bin\debug esta en VStudio
        /// En este caso el directorio base sera el de proyecto (fuentes)
        /// En geral el directori base sera el mismo del ejecutable
        /// </remarks>

        static public string GetDirBase()
        {
            //string dirbas = Directory.GetCurrentDirectory();
            string dirbas = AppContext.BaseDirectory; // Funciona tanto en desarrollo como en instalación

            if (!AjustDirDev("Release"))
                AjustDirDev("Debug");

            return dirbas;

            //ANTIGUO
            bool AjustDirDev(string rel)
            {
                //string dev = @"\bin\" + rel;
                string dev = Path.Combine("bin", rel);

                if (dirbas.Contains(dev, StringComparison.OrdinalIgnoreCase))
                {
                    if (dirbas.EndsWith(dev))
                        dirbas = GetFile(dirbas, @"..\..\..\");     // Framework
                    else
                    {
                        if (dirbas.EndsWith("publish"))
                            dirbas = GetFile(dirbas, @"..\..\..\..\..\");  // Publish Net Core
                        else
                            dirbas = GetFile(dirbas, @"..\..\..\..\");     // Net Core
                    }

                    return true;
                }

                return false;
            }

        }

        /// <summary> Combina directorio base con el directorio relativo
        /// Se utiliza para crear paths absolutos de la aplicacion
        /// Los directorios deben referirse a un directorio base
        /// Admite uso de .. como Id a directorios superiores
        /// </summary>
        /// 
        /// <param name="sDataPath"> Cadena con el directorio base  </param>
        /// <param name="sDirPath">  Directorio relativo a combinar </param>
        /// 
        /// <returns> Cadena combinada de directorios </returns>

        public static string GetFile(string dirBase, string dirPath)
        {
            int nPos;
            if (!string.IsNullOrEmpty(dirBase))
            {
                char cSepar = Path.DirectorySeparatorChar;
                char cAlter = Path.AltDirectorySeparatorChar;

                nPos = dirBase.Length - 1;

                if (dirBase.IndexOf(cSepar) < 0)
                {
                    // Separador alternativo si existe en los paths
                    if (dirBase.Contains(cAlter) ||
                        dirPath != null && dirPath.Contains(cAlter))
                    {
                        (cAlter, cSepar) = (cSepar, cAlter);
                    }
                }

                // Si el path base no tiene barra final se la añadimos
                // if (dirBase[nPos] != cSepar && !Data.Equals(dirPath, cSepar))
                // CAMBIO ULTIMA HORA: Chequear!!!! *******

                if (dirBase[nPos] != cSepar)
                    dirBase += cSepar;

                // retroceder directorios sobre el base si se pide
                // while (DirPath.Contains( ".." + cSepar ))
                if (dirPath == null)
                    dirPath = dirBase;
                else
                {
                    if (dirPath.Contains(cAlter))
                        dirPath = dirPath.Replace(cAlter, cSepar);

                    while (dirPath.Contains(".." + cSepar, StringComparison.CurrentCulture))
                    {
                        if (nPos > 0)
                        {
                            if (dirBase[nPos] == cSepar)
                                nPos--;

                            nPos = dirBase.LastIndexOf(cSepar, nPos);
                            if (nPos > 0)
                                dirBase = dirBase[..(nPos + 1)];
                        }

                        dirPath = dirPath[3..];
                    }

                    if (dirPath.Length > 1 && dirPath.StartsWith(cSepar.ToString()))
                        dirPath = dirPath[1..];

                    if (Data.Equals(dirPath, cSepar))
                        dirPath = dirBase;
                    else
                        dirPath = Path.Combine(dirBase, dirPath);

                        //dirPath = dirBase + dirPath;
                }
            }

            // retornar directorios combinados
            return dirPath;
        }

        /// <summary> Normaliza folder con directorio base y separador
        /// </summary>
        /// <param name="path"> Path a estandarizar </param>
        /// <returns> Path absoluto y normalizado </returns>
        public static string NormFolder(string path)
        {
            string separator1 = Path.DirectorySeparatorChar.ToString();
            string separator2 = Path.AltDirectorySeparatorChar.ToString();

            path = GetFullPath(path);

            if (path.EndsWith(separator1) || path.EndsWith(separator2))
                return path;

            if (path.Contains(separator2))
                return path + separator2;

            return path + separator1;
        }

        #endregion

        #region GESTION DE RESULTADOS

        /// <summary> Genera un fichro resultado con codgo y texto
        /// </summary>
        /// <param name="message"> Texto para resultado correcto /param>
        /// <returns> Indica fuchero generado </returns>
        /// <remarks>
        /// El fichero generado se especifica con -Result=FileName
        /// 
        /// Contiene:
        /// 
        /// - Linea 1: Id de resultado: Valid/Warning/Error
        /// - Linea 2: Mensaje de la aplicacion con el error
        /// - Lineas siguientes: Detalle del error y traza 
        /// 
        /// </remarks>
        public bool CreateResult(string message = null)
        {
            string name = FileResult;
            bool resul = true;

            try
            {
                if (string.IsNullOrEmpty(name))
                    name = ResultDefault;

                string errors = GetErrorText();

                AppResult code = AppResult.Valid;

                if (AppErrors != null && AppErrors.Length > 0)
                    code = AppResult.Error;
                else
                {
                    if (AppWarnings != null && AppWarnings.Length > 0)
                        code = AppResult.Warning;
                }

                string text = code.ToString();

                if (code != AppResult.Error)
                {
                    if (!IsEmpty(message))
                        text += "\n" + message;
                }

                if (!IsEmpty(errors))
                    text += '\n' + errors;

                string file = GetFullPath(name);
                File.WriteAllText(file, text);
            }
            catch (Exception exc)
            {
                resul = false;
                Logger.LogError(exc);
            }
            return resul;
        }

        #endregion

        #region GESTION DE ERRORES

        /// <summary> Procesa mensaje de error 
        /// </summary>
        /// <param name="exc"> Excepcion generada </param>

        public bool ProcError(string info, Exception exc = null)
        {
            if (exc != null)
            {
                info += '\n' + exc.Message;

                if (exc.StackTrace != null)
                    info += '\n' + exc.StackTrace;
            }

            AddError(info);

            return false;
        }

        /// <summary> Lista de errores detectados
        /// </summary>
        public string[] AppErrors
        {
            get { return m_AppErrors; }
        }
        private string[] m_AppErrors;       // Mensajes de errores

        /// <summary> Lista de warnings detectados
        /// </summary>
        public string[] AppWarnings
        {
            get { return m_AppWarnings; }
        }
        private string[] m_AppWarnings;     // Mensajes de avisos

        /// <summary> Retorna lista de errores como lineas de texto
        /// </summary>
        /// <returns> Lineas de errores y warnings </returns>

        public string GetErrorText()
        {
            string errors = null;

            if (AppErrors != null)
            {
                foreach (string text in AppErrors)
                {
                    if (errors != null)
                        errors += "\n\n";

                    errors += text;
                }
            }

            if (AppWarnings != null)
            {
                foreach (string text in AppWarnings)
                {
                    if (errors != null)
                        errors += '\n';

                    errors += text;
                }
            }

            return errors;
        }

        private static void AddItem(ref string[] info, string text)
        {
            if (info == null)
            {
                info = [text];
            }
            else
            {
                int total = info.Length + 1;
                Array.Resize(ref info, total);
                info[total - 1] = text;
            }
        }

        public void AddError(string error, bool isWarning = false)
        {
            if (isWarning)
            {
                AddItem(ref m_AppWarnings, error);
            }
            else
            {
                AddItem(ref m_AppErrors, error);
            }

            if (logMesages)
            {
                Logger.LogText(error);
            }
        }


        public static void LogMessages(bool log)
        {
            logMesages = log;
        }

        private static bool logMesages = true;

        public void AddWarning(string error)
        {
            AddError(error, true);
        }

        public static TestType SetTest(TestType test)
        {
            TestType actual = TestType;

            if (test != TestType.None)
                TestType = test;

            return actual;
        }

        public static TestType GetTest()
        {
            return TestType;
        }

        public static bool IsTest(TestType test = TestType.Test)
        {
            return TestType >= test;
        }

        private static TestType TestType;

        #endregion

        #region METODOS DE SOPORTE


        /// <summary> Comprueba si el objeto dado esta vacio
        /// </summary>
        /// <param name="value"> Objecto a comprobar </param>
        /// <returns> Indicacion de objeto vacio </returns>
        public bool IsEmpty<T>(T value)
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
        #endregion
    }

    #region ENTRADA GENERAL DE ERRORES

    public class Logger
    {
        static public void LogError(Exception exc, string text = null)
        {
            //string dir = AppBase.GetFullPath("Log");
            string dir = Paths.LogsDirectory;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            AppBase.GetAppBase().ProcError(text, exc);

            // if (!AppSystem.HasConsole())
            //      AppSystem.ShowConsole();
        }

        static public void LogError(string text)
        {
            LogError(null, text);
        }

        static public void LogText(string text)
        {
            string file = AppBase.GetFullPath($"{Paths.LogsDirectory}{DateTime.Now.Date.ToString().Split(' ')[0].Replace('/', '_')}.log");
            string data = "\n" + DateTime.Now.ToString("G") + " " + text;

            File.AppendAllText(file, data);
        }

    }
    #endregion
}

