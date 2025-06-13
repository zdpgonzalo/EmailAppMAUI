using MailAppMAUI.Config;
using System;

namespace MailAppMAUI.UseCases
{
    public sealed class EmailConfig
    {
        public enum DominioEmail
        {
            Gmail,
            Outlook,
            Yahoo,
            Otro
        }

        private struct EmailProviderConfig
        {
            public string ImapServer { get; }
            public int ImapPort { get; }
            public bool ImapSSL { get; }

            public string SmtpServer { get; }
            public int SmtpPort { get; }
            public bool SmtpSSL { get; }

            public string CarpetaInbox { get; }
            public string CarpetaTrash { get; }

            public EmailProviderConfig(
                string imapServer, int imapPort, bool imapSSL,
                string smtpServer, int smtpPort, bool smtpSSL,
                string carpetaInbox, string carpetaTrash)
            {
                ImapServer = imapServer;
                ImapPort = imapPort;
                ImapSSL = imapSSL;
                SmtpServer = smtpServer;
                SmtpPort = smtpPort;
                SmtpSSL = smtpSSL;
                CarpetaInbox = carpetaInbox;
                CarpetaTrash = carpetaTrash;
            }
        }

        private static EmailConfig _instance;
        private static readonly object _lock = new object();

        private readonly string _email;
        private readonly DominioEmail _dominio;
        private readonly EmailProviderConfig _config;

        static Configuration Conf { get; set; }

        private EmailConfig()
        {
            //Para poder usar un directorio 
            if ((Conf = Configuration.Config) == null)
            {
                Conf = new Configuration();
            }

            _email = Conf.User.Email;
            _dominio = DetectarDominio(_email);
            _config = ObtenerConfiguracion(_dominio);
        }

        public static EmailConfig Instance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new EmailConfig();
                }
                return _instance;
            }
        }

        public string GetEmail() => _email;
        public DominioEmail GetDominio() => _dominio;

        public string GetImapServer() => _config.ImapServer;
        public int GetImapPort() => _config.ImapPort;
        public bool IsImapSslEnabled() => _config.ImapSSL;

        public string GetSmtpServer() => _config.SmtpServer;
        public int GetSmtpPort() => _config.SmtpPort;
        public bool IsSmtpSslEnabled() => _config.SmtpSSL;

        public string GetInboxFolder() => _config.CarpetaInbox;
        public string GetTrashFolder() => _config.CarpetaTrash;

        private DominioEmail DetectarDominio(string email)
        {
            if (email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                return DominioEmail.Gmail;

            if (email.EndsWith("@hotmail.com", StringComparison.OrdinalIgnoreCase) ||
                email.EndsWith("@outlook.com", StringComparison.OrdinalIgnoreCase) ||
                email.EndsWith("@live.com", StringComparison.OrdinalIgnoreCase))
                return DominioEmail.Outlook;

            if (email.EndsWith("@yahoo.com", StringComparison.OrdinalIgnoreCase))
                return DominioEmail.Yahoo;

            return DominioEmail.Otro;
        }

        private EmailProviderConfig ObtenerConfiguracion(DominioEmail dominio)
        {
            return dominio switch
            {
                DominioEmail.Gmail => new EmailProviderConfig(
                    "imap.gmail.com", 993, true,
                    "smtp.gmail.com", 587, true,
                    "INBOX", "[Gmail]/Trash"),

                DominioEmail.Outlook => new EmailProviderConfig(
                    "imap-mail.outlook.com", 993, true,
                    "smtp-mail.outlook.com", 587, true,
                    "Inbox", "Deleted Items"),

                DominioEmail.Yahoo => new EmailProviderConfig(
                    "imap.mail.yahoo.com", 993, true,
                    "smtp.mail.yahoo.com", 465, true,
                    "Inbox", "Trash"),

                _ => throw new NotSupportedException("Dominio de correo no soportado automáticamente.")
            };
        }
    }
}
