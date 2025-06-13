
using MailAppMAUI.General;
using MailAppMAUI.General;
using Microsoft.Maui.Graphics.Text;
using System.Text;

namespace MailAppMAUI.Config
{
    public class BaseConfig
    {
        // Datos y tablas de configuracion 
        static internal string FileConfig;

        // Valores por defecto de parametros
        const string ConfigDefault = "Config/WebConfig.info";
        const string ConfigPatron = "Config/WebConfig.pat";

        static public bool IsStarted
        {
            get { return Config != null; }
        }

        static Configuration Config { get; set; }
        static BaseConfig BaseApp { get; set; }

        public BaseConfig()
        {
            if ((Config = Configuration.Config) == null)
            {
                Config = new Configuration();
            }

            BaseApp = this;
        }

        /// <summary> Carga o crea la configuracion base de aplicacion
        /// </summary>
        /// <returns> Carga realizada correctamente </returns>

        public static bool LoadConfig()
        {
            bool resul = false;

            if (BaseApp == null)
                BaseApp = new BaseConfig();

            if (Str.Empty(FileConfig))
                FileConfig = ConfigDefault;

            FileConfig = AppBase.GetFullPath(FileConfig);

            string folderPath = Path.GetDirectoryName(FileConfig);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Si no existe el archivo, copiar el patrón
            if (!File.Exists(FileConfig))
            {
                string file = AppBase.GetFullPath(ConfigPatron);
                if (File.Exists(file))
                    File.Copy(file, FileConfig);
            }

            var groups = AppConfig<ConfigKeys>.ReadConfig(FileConfig);
            if (groups == null)
                return false;

            foreach (var groupItem in groups)
            {
                var group = Data.GetEnum(groupItem.Name, GroupKeys.None);
                if (group == GroupKeys.None)
                    group = GroupKeys.EmailConfig;

                if (groupItem?.Items == null)
                    continue;

                foreach (var item in groupItem.Items)
                {
                    switch (group)
                    {
                        case GroupKeys.EmailConfig:
                            switch (item.Key)
                            {
                                case ConfigKeys.UserEmail: Config.User.Email = item.Value; break;
                                case ConfigKeys.UserPassword: Config.User.Password = item.Value; break;
                                case ConfigKeys.UserName: Config.User.Name = item.Value; break;
                                case ConfigKeys.UserToken: Config.User.AccessToken = item.Value; break;
                                case ConfigKeys.UserImapConf: Config.User.ImapConexion = item.Value; break;
                                case ConfigKeys.UserSmtpConf: Config.User.SmtpConexion = item.Value; break;
                                case ConfigKeys.UserSmtpPort: Config.User.SmtpPort = int.TryParse(item.Value, out var smtpPort) ? smtpPort : 0; break;
                                case ConfigKeys.UserImapPort: Config.User.ImapPort = int.TryParse(item.Value, out var imapPort) ? imapPort : 0; break;
                                case ConfigKeys.UserUserId: Config.User.UserId = int.TryParse(item.Value, out var userId) ? userId : 0; break;
                            }
                            break;
                    }
                }
            }

            return true;
        }


        public static void SaveConfig()
        {
            try
            {
                if (Str.Empty(FileConfig))
                    FileConfig = ConfigDefault;

                FileConfig = AppBase.GetFullPath(FileConfig);

                if (BaseApp == null)
                    BaseApp = new BaseConfig();

                string folderPath = Path.GetDirectoryName(FileConfig);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Crea el contenido del archivo
                var grupo = new StringBuilder();
                grupo.AppendLine($"[{GroupKeys.EmailConfig}]");
                grupo.AppendLine($"{ConfigKeys.UserEmail} = {Config.User.Email}");
                grupo.AppendLine($"{ConfigKeys.UserPassword} = {AppCrypt.Encode(Config.User.Password)}");
                grupo.AppendLine($"{ConfigKeys.UserName} = {Config.User.Name}");
                grupo.AppendLine($"{ConfigKeys.UserToken} = {Config.User.AccessToken}");
                grupo.AppendLine($"{ConfigKeys.UserImapConf} = {Config.User.ImapConexion}");
                grupo.AppendLine($"{ConfigKeys.UserSmtpConf} = {Config.User.SmtpConexion}");
                grupo.AppendLine($"{ConfigKeys.UserSmtpPort} = {Config.User.SmtpPort}");
                grupo.AppendLine($"{ConfigKeys.UserImapPort} = {Config.User.ImapPort}");
                grupo.AppendLine($"{ConfigKeys.UserUserId} = {Config.User.UserId}");
                grupo.AppendLine();

                File.WriteAllText(FileConfig, grupo.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error al guardar configuración.");
            }
        }

    }
}

