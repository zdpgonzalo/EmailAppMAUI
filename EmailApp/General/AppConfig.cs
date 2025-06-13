using System;
using System.Text;
using System.IO;
using System.Threading;
using MailAppMAUI.General;

namespace MailAppMAUI.General
{
    public class ConfigItem<T> where T : struct
    {
        public T Key;
        public string Value;

        public string ToString()
        {
            string resul = Key.ToString();

            if (Value != null)
                resul += " = " + Value.ToString();

            return resul;
        }
    }

    public class ConfigGroup<T> where T : struct
    {
        public string Name;
        public ConfigItem<T>[] Items;
        public string ToString()
        {
            var resul = Name.ToString();

            if (Items != null)
                resul += " [" + Items.Length.ToString() + "]";

            return resul;
        }
    }

    public class AppConfig<T> where T : struct
    {
        const string Group_Global = "Global";
        const string Key_Password = "Password";



        /// <summary> Lectura de ficheros de configuarcion Grupo/valores
        /// </summary>
        /// <param name="file"> Nombre del fichero a procesar </param>
        /// <param name="pass"> Campo a tratar como password  </param>
        /// <param name="lines">Lineas ya cargadas a procesar </param>
        /// <param name="separate">Separar grupos con el mismo nombre </param>
        /// <returns> Liat de grupor de configuracion leidos</returns>
        /// <remarks>
        /// Este metodo lee fichero de configuracion tipo INI con grupos / items
        /// Puede leer el fichero o procesar las lineas ya cargadas que se pasan
        /// Esto permite procesar las mismas lineas con varias configuraciones
        /// Si se indica un campo de password se deencripta al cargarlo
        /// Por defecto en los grupso de igual nombre se suman todos los items
        /// Si se pasa el parametro separate se crea siempre un grupo distinto
        /// </remarks>

        public static ConfigGroup<T>[] ReadConfig(string file, T pass, ref string[] lines, bool separate = false)
        {
            ConfigGroup<T>[] groups = null;

            try
            {
                bool changed = false;
                bool error = false;

                try
                {
                    if (lines == null)
                    {
                        if (File.Exists(file))
                        {
                            for (int loop = 1; loop <= 2; loop++)
                            {
                                try
                                {
                                    lines = File.ReadAllLines(file);
                                }
                                catch
                                {
                                    error = true;
                                }

                                if (error)
                                {
                                    Thread.Sleep(100);
                                    error = false;
                                }
                                else
                                {
                                    if (loop != 1)
                                        error = false; // Test

                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Logger.LogError(exc, "Error leyendo configuracion: " + file);
                }

                if (lines == null)
                    return null;

                var groupName = Group_Global;
                ConfigGroup<T> group = null;
                T lastKey = default(T);

                for (int index = 0; index < lines.Length; index++)
                {
                    string line = lines[index];
                    if (Str.Empty(line))
                        continue;

                    if (line.TrimStart().StartsWith("#") ||
                        line.TrimStart().StartsWith("//"))
                        continue;

                    // Buscar inicio de grupos de variables
                    bool isGroup = line.TrimStart().StartsWith("[") &&
                                   line.TrimEnd().EndsWith("]");

                    if (isGroup)
                    {
                        groupName = line.Trim();
                        groupName = groupName.Replace('.', '_');
                        groupName = groupName.Replace('-', '_');
                        groupName = groupName.Substring(1, groupName.Length - 2);
                        groupName = Data.NormVariable(groupName);
                        lastKey = default(T);
                    }

                    if (group == null || isGroup)
                    {
                        if (groups != null && !separate)
                            group = Array.Find(groups, g => g.Name == groupName);
                        else
                            group = null;

                        if (group == null)
                        {
                            group = new ConfigGroup<T> { Name = groupName };
                            Arr.Append<ConfigGroup<T>>(ref groups, group);
                        }

                        if (isGroup)
                            continue;
                    }

                    // Buscar par clave-valor con el enumerado dado
                    var (key, value) = Data.GetKeyValue(line);

                    T code;

                    if (Str.Empty(key))
                    {
                        value = line.Trim();
                        code = lastKey;
                    }
                    else
                    {
                        // Substituir guiones medios por guiones bajos
                        // Es necesario para convertirlos a enumerados
                        if (key.IndexOf('-') >= 0)
                            key = key.Replace('-', '_');

                        Enum.TryParse(key, true, out code);
                    }

                    if (!code.Equals(default(T)))
                    {
                        lastKey = code;
                        if (code.Equals(pass) ||
                            !code.Equals(default(T)) && key.EndsWith(Key_Password))
                        {
                            // Comprobar y encripotar claves de passwords
                            // Si se da clave de password debe coincidir
                            // Si no la clave debe acabar con "Password"

                            string decoded = AppCrypt.TryDecode(value);

                            if (decoded != null)
                            {
                                // Obtenido valor real del valor codificado
                                value = decoded;
                            }
                            else
                            {
                                // Valor sin codificar: debe actualizarse
                                string encoded = AppCrypt.Encode(value);

                                lines[index] = key + ": " + encoded;
                                changed = true;
                            }
                        }

                        var item = new ConfigItem<T> { Key = code, Value = value };
                        group.Items = Arr.Append(group.Items, item);
                    }
                }

                if (changed)
                    File.WriteAllLines(file, lines);

            }
            catch (Exception exc)
            {
                Logger.LogError(exc, "Error leyendo configuracion " + file);
                groups = null;
            }

            return groups;
        }

        public static ConfigGroup<T>[] ReadConfig(string file, bool separate = false)
        {
            return ReadConfig(file, default(T), separate);
        }

        public static ConfigGroup<T>[] ReadConfig(string file, T pass, bool separate = false)
        {
            string[] lines = null;

            return ReadConfig(file, pass, ref lines, separate);
        }
    }
}
