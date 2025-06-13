using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using MailAppMAUI.General;

namespace Ifs.Comun
{
    /// <summary> Gestiona la creación de registros de eventos
    /// Esta es una verison reducida de Ifs.Comun para IfsMail
    /// Cuando se integre utilizar la libreria Ifs.Comun normal
    /// </summary>

    public static class Logger
    {
        static public bool LogClear(string logger)
        {
            if (logger.IndexOf('.') < 0)
                logger += ".log";

            string fileLog = GetFilePath(logger);

            if (File.Exists(fileLog))
            {
                File.Delete(fileLog);
            }

            return true;
        }


        static public bool LogLine(string logger, string text1, string text2)
        {
            bool resul = true;

            try
            {
                if (logger.IndexOf('.') < 0)
                    logger += ".log";

                string fileLog = GetFilePath(logger);

                StreamWriter fLog = new StreamWriter(fileLog, true);

                fLog.WriteLine(text1 + '\t' + text2);

                fLog.Close();
            }
            catch
            {
                resul = false;
            }

            return resul;
        }

        static public bool LogError(Exception exc)
        {
            string time = DateTime.Now.Date.ToShortDateString() + "  " +
                          DateTime.Now.TimeOfDay.Hours.ToString() + ":"+
                          DateTime.Now.TimeOfDay.Minutes.ToString() + ":"+
                          DateTime.Now.TimeOfDay.Seconds.ToString() + "  ";

            return LogLine("MailError", time + exc.Message, "\n"+exc.StackTrace);
        }

        /// <summary> Retorna todo el texto contenido en un logger 
        /// </summary>
        /// <param name="name"> Nombre del logger a leer </param>
        /// <returns> Texto contenido en el logger </returns>

        public static string LogRead(string name)
        {
            string text = null;

            try
            {
                if (name.IndexOf('.') < 0)
                    name += ".log";

                string fileLog = GetFilePath(name);

                StreamReader log = new StreamReader(fileLog);

                text = log.ReadToEnd();
                log.Close();
            }
            catch
            {
            }

            return text;
        }

        public static string LogWrite(string name, string lines)
        {
            string text = null;

            try
            {
                if (name.IndexOf('.') < 0)
                    name += ".log";

                if (name.IndexOf('\\') < 0)
                    name = @"Log\"+name;

                string fileLog = GetFilePath(name);

                var log = new StreamWriter(fileLog);

                var separators = new[] { '\r', '\n' };
                var linlog = lines.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in linlog)
                {
                    log.WriteLine(line);
                }
                log.Close();
            }
            catch
            {
            }

            return text;
        }


        /// <summary> Devuelve contenido del logger como array de lineas
        /// </summary>

        public static string[] LogLines( string name )
        {
            string[] lines = null;
            string text = LogRead( name );

            if (text != null)
            {
                lines = text.Split(new string[] {"\r\n", "\n"}, 
                             StringSplitOptions.RemoveEmptyEntries);
            }

            return lines;
        }
        
        /// <summary> Retorna fichero sobre directorio de la aplicacion
        /// </summary>
        /// <param name="file"> Nombre del fichero (o cadena vacia) </param>
        /// <returns> Directorio completo del fichero dado </returns>

        static public string GetFilePath(string file)
        {
            string dirBase = DirBase;

            if (dirBase != null && dirBase.Length < 5)
                dirBase = null;

            if (String.IsNullOrWhiteSpace(dirBase))
                //dirBase = GetDirExec();
                dirBase = AppBase.GetDirBase();

            // dirBase = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(dirBase);

            string dirFile = dirBase + file;

            return dirFile;
        }

        static public void SetLogger(string logger)
        {
            if (Directory.Exists(logger))
            {
                if (Directory.Exists(logger + @"log\"))
                    logger += @"log\";

                DirBase = logger;
            }
        }

        static string DirBase;


        /// <summary> Devuelve directorio del modulo de arranque 
        /// El resultado esta normalizado y con separadores del sistema
        /// La cadena se acaba con el caracter separador de directorios
        /// </summary>
        /// <returns> Directorio de arranque </returns>

        public static string GetDirExec()
        {
            string DirExec = Path.GetDirectoryName(GetModExec());

            var cSepar = Path.DirectorySeparatorChar;

            if (DirExec[DirExec.Length - 1] != cSepar)
                DirExec = DirExec + cSepar;

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

            AsEntry = Assembly.GetEntryAssembly();
            if (AsEntry == null)
                AsEntry = Assembly.GetExecutingAssembly();

            AssemblyName AsName = AsEntry.GetName();
            Uri AsUri = new Uri(AsName.CodeBase);
            DirExec = AsUri.AbsolutePath;

            return DirExec;
        }
    }
}