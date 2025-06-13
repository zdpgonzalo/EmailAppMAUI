
#define UseMailKit 

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using System.Net.Mail;
using System.Net.Mime;

using MimeKit;
using MimeKit.Utils;
using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;

using Ifs.Comun;
using Ifs.ComInter;
using System.Net;
using System.Net.Security;

/// <summary> COMPATIBILIDAD DE PLATAFORMAS
/// 
/// Este proyecto utiliza el envio SMTP con MailKit 2.0
/// El proyecto con MailKit necesita minimo el framework 4.5
/// 
/// El Framework se limita segun el sistemas operativo:
/// 
/// - Windows 10:    Framework 4.6 minimo preinstaldo
/// - Windows 8.1:   Framework 4.5.1 preinstaldo
/// - Windows 8:     Framework 4.5   preinstaldo
/// - Windows 7:     El framework necesita SP1 
/// - Windows 7 SP1: Framework 4 - 4.7 instalable
/// - Windows Vista: Framework 4 - 4.6 instalable
/// - Windows XP:    Framework 4 maxima version soportada
/// 
/// Por tanto el correo con MailKit no puede funcionar en XP
/// Para clientes con XP usar IfsMail 102 (es compatible)
/// Esto limita a SMTP normal (25) y STARTTLS (puerto 587)
/// No se puede usar conexion SSL directa al puerto 465
/// Sin embargo es la unica solucion para Windows XP
/// </summary>


namespace Ifs.Mail
{

    public enum TestMail
    {
        None,     // No crear logger
        Trace,    // Crear registro del mensaje
        Debug     // Crear registro detallado
    }

    public class IfsMail
    {
        // Indice de datos del array de intercambio
        public enum MailProps
        {
            Server    =  0,  // Direccion o ip del servidor
            Port      =  1,  // Puerto para el servidor
            User      =  2,  // Nombre completo de usuario
            Password  =  3,  // Clave del usario en el host
            FromField =  4,  // Lista de origen entre punto y coma
            ToField   =  5,  // Lista de destinos entre punto y coma
            Subject   =  6,  // Asunto del mensaje
            Company   =  7,  // Nombre de la compañia
            TextField =  8,  // Texto o codigo html del cuerpo
            FileNames =  9,  // Lista de adjuntos entre punto y coma
            Timeout   = 10,  // Tiempo de espera maximo
            Confirm   = 11,  // Solicitar confirmacion
            EnableSsl = 12,  // El server requiere SSL/TTL
            KeyGest   = 13,  // CLave de gestion
            KeyIdent  = 14,  // Identificador del documento
            MailIdent = 15,  // Identificador del correo
            MailLogo  = 16,  // Path a un logo del correo
            MailFooter= 17,  // Texto o html para el pie
            DirBase   = 18,  // Carpeta raiz o base de imagenes
            CopyBcc   = 19,  // Copia oculta del email (bcc)
            TestLevel = 20,  // Nivel de pruebas requerido
            IsUtf8    = 21,  // Codificacion en UTF8 (defecto)
        }

        public const int MinTimeout = 200; // Timeout minimo en mseg

        public bool CreateLogger = true;   // Crear logger de envios
        public bool ResetLogger  = false;  // Reiniciar logger

        public bool RemoveScript = true;   // Quitar tags de script
        public bool RemoveHidden = false;  // Quitar visibility hidden

        public int NumSend;        // Numero de correos enviados
        public int NumError;       // Numero de correos erroneos
        public string MailResult;  // Ultimo mensaje de resultado

        public string BaseDir;     // Ruta base del html e imagenes

        List<ImageDef> Images;     // Lista de imagenes del mensaje

        /// <summary> Envia lista de email segun fichero de datos
        /// </summary>
        /// <param name="fileName"> Fichero con datos a enviar </param>
        /// <returns> Numero de mensjes enviados </returns>

        public int SendFile(string fileName)
        {
            try
            {
                object[] emails = LoadFile(fileName);

                if (ResetLogger)
                    ClearData();

                NumSend = 0;
                NumError = 0;
                MailResult = "";

                if (emails == null)
                {
                    MailResult = "No se han encontrado emails para enviar";
                    NumError++;
                }
                else
                {
                    foreach (object email in emails)
                    {
                        #if UseMailKit
                        var mail = LoadParams((object[])email);

                        SendMail(mail);

                        #else
                                                
                            SendNetMail((object[])email);

                        #endif
                    }

                    if (NumError == 0)
                        File.Delete(fileName);
                }
            }
            catch (Exception exc)
            {
                Logger.LogError(exc);
                MailResult = "Se ha producido un error al enviar el correo";
            }

            return NumError;
        }


        #region CLASE DE DEFINICION DE PARAMETROS DE MAIL

        public class MailDef
        {
            public string Host;        // Direccion del servidor
            public string User;        // Usuario de la cuenta
            public string Pass;        // Password de la cuenta
            public int    Port;        // Numero de puerto usado
            public bool   IsSsl;       // Utiliza protocolo SSL

            public string From;        // Direccion de correo origen
            public string To;          // Direcciones de correo destino
            public string Subject;     // Titulo del correo
            public string CopyBcc;     // Copia oculta Bcc

            public string Company;     // Nombre de la empresa
            public string Text;        // Texto principal del correo
            public string Files;       // Lista de ficheros adjuntos
            public int    Timeout;     // Temporizacion maxima de envio
            public bool   Confirm;     // Enviar confirmacion de lectura

            public string Logo;        // Ruta con imagen de Logo
            public string Footer;      // Texto html para el pie
            public string DirBase;     // Directorio base de imagenes
                                       // Contiene las imagenes o Image

            public string IdMail;      // Identificador de gestion
            public string KeyGest;     // Codigo de gestion del documento
            public string KeyIdent;    // Identificador unico del documento

            public string MailError;   // Error en proceso del mail 
            public TestMail TestLevel; // Nivel de registro de errpres

            public bool IsUtf8;        // Codificacion en UTF8 (por defecto)
        }

        public MailDef LoadParams(object[] email)
        {
            var mail = new MailDef();

            mail.Host = GetValue<string>(email, MailProps.Server);
            mail.IsSsl = GetValue<bool>(email, MailProps.EnableSsl);

            mail.Port = GetValue<int>(email, MailProps.Port);

            mail.User = GetValue<string>(email, MailProps.User);
            mail.Pass = GetValue<string>(email, MailProps.Password);
            mail.From = GetValue<string>(email, MailProps.FromField);
            mail.To = GetValue<string>(email, MailProps.ToField);
            mail.Subject = GetValue<string>(email, MailProps.Subject);
            mail.Company = GetValue<string>(email, MailProps.Company);
            mail.Text = GetValue<string>(email, MailProps.TextField);
            mail.Files = GetValue<string>(email, MailProps.FileNames);
            mail.Timeout = GetValue<int>(email, MailProps.Timeout);
            mail.Confirm = GetValue<bool>(email, MailProps.Confirm);

            mail.Logo = GetValue<string>(email, MailProps.MailLogo);
            mail.Footer = GetValue<string>(email, MailProps.MailFooter);

            mail.IdMail = GetValue<string>(email, MailProps.MailIdent);
            mail.KeyGest = GetValue<string>(email, MailProps.KeyGest);
            mail.KeyIdent = GetValue<string>(email, MailProps.KeyIdent);

            mail.DirBase = GetValue<string>(email, MailProps.DirBase);
            mail.CopyBcc = GetValue<string>(email, MailProps.CopyBcc);

            mail.TestLevel = TestMail.Trace;

            // Requerido para encontrar las imagenes
            // mail.DirBase = @"D:\DOC\MAIL\Navidad2";
            
            if (mail.TestLevel == TestMail.Trace)
            {
                LogMailParams(email);
            }


            if (mail.Port == 465)   // Normalmente no se usa este puerto
                mail.IsSsl = true;  // Pero si se usa debe ser TLS

            mail.To = mail.To.TrimEnd();
            mail.From = mail.From.TrimEnd();

            if (email.Length > (int)MailProps.IsUtf8+1)
                mail.IsUtf8 = GetValue<bool>(email, MailProps.IsUtf8);
            else
                mail.IsUtf8 = true;  // Por defecto tratar como UTF 8

            if (mail.IsUtf8)
            {
                mail.IsUtf8 = true;

                byte[] bytes = Encoding.Default.GetBytes(mail.Text);
                var text = Encoding.UTF8.GetString(bytes);

                mail.Text = text;
                
            }

            return mail;
        }

        private void LogMailParams(object[] email)
        {
            string lines = "";

            for(int index = 0; index < email.Length; index++)
            {
                var key = (MailProps)index;
                var name = Enum.GetName(typeof(MailProps), key);

                if (key != MailProps.Password)
                {
                    object value = email[index];
                    if (value == null)
                        value = "null";
                    else
                    {
                        if (value is string)
                            value = "'" + value + "'";
                        else
                            value = value.ToString();
                    }

                    lines += name + "=" + value + "\n";
                }
            }

            Logger.LogWrite("mpars", lines);
        }

        #endregion

        #region CREACION Y ENVIO DE CORREOS

        /// <summary> Metodo principal de envio de correos
        /// </summary>
        /// <param name="mail"> Parametros del correo </param>
        /// <returns> Resultado de la operacion </returns>

        public bool SendMail( MailDef mail)
        {
            bool resul = true;


            // var txt = mail.Text;

            // var str = txt.Substring(23, 3);


            // byte[] bytes = Encoding.Unicode.GetBytes(txt);
            // byte[] bytes = Encoding.Default.GetBytes(txt);
            //var txt_utf8 = Encoding.UTF8.GetString(bytes);

            // bytes = Encoding.Default.GetBytes(txt);
            // var txt_unicode = Encoding.ASCII.GetString(bytes);

            resul = true;


            // Codigo de pruebas

            // mail.To = "admin@infoser.net";
            // mail.From = "departamentofacturacion@lidermadrid.com";
            // mail.Host = "authsmtp.securemail.pro";
            // mail.User = "departamentofacturacion@lidermadrid.com";
            // mail.Pass = "P%=u;yuXB2rj3U4Pa83Q";
            // mail.Port = 465;
            // mail.IsSsl= true;

            // Crear mensaje Mime con los parametros dados
            var message = CreateMail(mail);

            if (message == null)
                return false;

            string dirbase = mail.DirBase;
            if (!String.IsNullOrWhiteSpace(dirbase))
                Logger.SetLogger(dirbase);

            string logfile = Logger.GetFilePath("Log\\smtp.log");

            using (var client = new MailKit.Net.Smtp.SmtpClient(new ProtocolLogger(logfile)))
            {
                try
                {
                    // Timeout: Va en milisegundos
                    if (mail.Timeout > MinTimeout)
                        client.Timeout = mail.Timeout;

                    var options = SecureSocketOptions.Auto;

                    if (mail.Port == 465)
                        options = SecureSocketOptions.SslOnConnect;

                    if (!mail.IsSsl)
                        options = SecureSocketOptions.None;

                    try
                    {
                        // mail.Host = "authsmtp.securemail.pro";
                        
                        // client.Connect(mail.Host, mail.Port );
                        client.Connect( mail.Host, mail.Port, options);
                    }
                    catch (SmtpCommandException ex)
                    {
                        mail.MailError = String.Format("Error tratando de conectar: {0} \n"+
                                                       "Codigo de estado {1}",
                                                        ex.Message, ex.StatusCode);
                        resul = false;
                    }
                    catch (SmtpProtocolException ex)
                    {
                        mail.MailError = String.Format( "Error estableciendo protocolo: {0}",
                                                        ex.Message );
                        resul = false;
                    }

                    if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                    {
                        try
                        {
                            client.Authenticate(mail.User, mail.Pass );
                        }
                        catch (AuthenticationException ex)
                        {
                            mail.MailError = "Usuario o clave invalidas ";
                            resul = false;
                        }
                        catch (SmtpCommandException ex)
                        {
                            mail.MailError = String.Format("Error tratando de autentificar: {0} \n" +
                                                           "Codigo de estado {1}",
                                                            ex.Message, ex.StatusCode);
                            resul = false;
                        }
                        catch (SmtpProtocolException ex)
                        {
                            mail.MailError = String.Format("Error de protocolo al autentificar: {0}",
                                                            ex.Message);
                            resul = false;
                        }
                    }

                    try
                    {
                        if (resul)
                            client.Send(message);
                    }
                    catch (SmtpCommandException ex)
                    {
                        string error = String.Format("Error enviando correo: {0} \n" +
                                                       "Codigo de estado {1}",
                                                        ex.Message, ex.StatusCode);

                        switch (ex.ErrorCode)
                        {
                           case SmtpErrorCode.RecipientNotAccepted:
                                error +=  String.Format( "\nReceptor del correo no acceptado: {0}", ex.Mailbox);
                                break;

                           case SmtpErrorCode.SenderNotAccepted:
                                error += String.Format("\nEmisor del correo no acceptado: {0}", ex.Mailbox);
                                break;

                           case SmtpErrorCode.MessageNotAccepted:
                                error += "\nMensaje no acceptado";
                                break;
                        }

                        mail.MailError = error;
                        resul = false;

                    }
                    catch (SmtpProtocolException ex)
                    {
                        mail.MailError = String.Format("Error general de protocolo enviando correo: {0}",
                                                        ex.Message);
                        resul = false;
                    }

                    client.Disconnect(true);

                }
                catch (Exception exc)
                {
                    mail.MailError = String.Format("Error general enviando mensaje: {0}",
                                     exc.Message);
                    Logger.LogError(exc);
                    resul = false;
                }
            }

            if (resul)
            {
                NumSend++;
                mail.MailError = "Enviado";
                MailResult = "Destinatario: " + mail.To + "\n Correo enviado";
            }
            else
            {
                NumError++;
                MailResult = "Destinatario: " + mail.To + ". No se puede enviar el correo: " +
                              mail.MailError;
            }

            if (CreateLogger)
                LogData(mail.To, mail.Subject, mail.From, mail.MailError, 
                        mail.IdMail, mail.KeyGest, mail.KeyIdent);

            return resul;
        }

        /// <summary> Crea mensaje de email en base a parametros dados
        /// </summary>
        /// <param name="mail"> Parametros del email </param>
        /// <returns> Mensaje de email creado </returns>
        /// <remarks>
        /// Parametros para definicion de email
        /// 
        /// From: Direcciones origen del correo
        ///       Admite varias direcciones separados por punto y coma
        ///       Cada direcion tiene un correo y nombre entre comas
        ///       Si no se da el nombre sera igual al correo
        ///       
        /// To:   Direcciones destino del correo
        ///       Admite varias direcciones separados por punto y coma
        ///       Cada direcion tiene un correo y nombre entre comas
        ///       Si no se da el nombre sera igual al correo
        /// 
        /// Files Lista de ficheros adjuntos al correo
        ///       Admite varias direcciones separados por punto y coma
        /// 
        /// </remarks>

        public MimeMessage CreateMail(MailDef mail)
        {
            var message = new MimeMessage();
            bool resul = true;

            // Carpeta base: Necesaria para imagenes
            if (!String.IsNullOrWhiteSpace(mail.DirBase))
                this.BaseDir = mail.DirBase;
            
            // Direcciones origen: Puede haber varias
            string[] addr = mail.From.Split(';');

            for (int index = 0; index < addr.Length; index++)
            {
                string from = GetItem(addr[index], 0);  // Direccion email
                string name = GetItem(addr[index], 1);  // Nombre opcional

                MailboxAddress dir;

                if (String.IsNullOrWhiteSpace(name))
                    dir = MailboxAddress.Parse(from);
                else
                    dir = new MailboxAddress(name, from);

                message.From.Add(dir);
            }

            // Direcciones destino: Puede haber varias
            addr = mail.To.Split(';');

            for (int index = 0; index < addr.Length; index++)
            {
                string to = GetItem(addr[index], 0);  // Direccion email
                string name = GetItem(addr[index], 1);  // Nombre opcional

                MailboxAddress dir;

                if (String.IsNullOrWhiteSpace(name))
                    dir = MailboxAddress.Parse(to);
                else
                    dir = new MailboxAddress(name, to);

                message.To.Add(dir);
            }

            if (!String.IsNullOrWhiteSpace(mail.CopyBcc))
            {
                var bccList = mail.CopyBcc.Split(';');

                foreach(var bccdir in bccList)
                {
                    var bcc = MailboxAddress.Parse(bccdir);
                    message.Bcc.Add(bcc);
                }
            }


            /*
            InternetAddressList bbcList = message.Bcc;
            MailboxAddress bbc = new MailboxAddress("backup@dominio.es");
            bbcList.Add(bbc);
            */

            // Añadir titulo y propiedades del correo
            message.Subject   = mail.Subject;
            if (!String.IsNullOrWhiteSpace(mail.IdMail))
                 message.MessageId = mail.IdMail;

            // Crear cuerpo del correo multiparte
            var builder = new BodyBuilder();

            // Agregar lista de ficheros adjuntos
            if (!String.IsNullOrEmpty(mail.Files))
            {
                String[] names = mail.Files.Split(';');

                for (int index = 0; index < names.Length; index++)
                {
                    string file = names[index];

                    int separ = file.IndexOf('=');

                    if (separ > 0)
                        file = file.Substring(0, separ);

                    if (StrEmpty(BaseDir))
                    {
                        BaseDir = Path.GetDirectoryName(file);
                    }

                    builder.Attachments.Add(file);
                }
            }

            // Procesar texto a html para el cerpo
            string html = null;

            if (StrIsHtml(mail.Text))
            {
                // Texto sumistrado como Html: Añadir el pie
                html = TextToHtml(mail.Text);

                // if (StrIsUtf(mail.Text))
                //     html = mail.Text;
                // else
                // html = TextToHtml(mail.Text);

                html = AddFooter(html, mail.Footer, mail.Logo, mail.Company);

                // Crear texto plano desde el codigo html 
                // El cuerpo debe ir con la version texto
                // Los correos lo muestran en los avisos

                builder.TextBody = HtmlToText(mail.Text);
            }
            else
            {
                if (mail.Text == null)
                {
                    // No esta cargado el texto: No enviar nada
                    mail.MailError = "No se ha definido el texto del correo";
                }
                else
                {
                    // El cuerpo es directamente texto plano
                    builder.TextBody = mail.Text;

                    // Puede haber una version html con el pie
                    html = AddFooter(mail.Text, mail.Footer, mail.Logo, mail.Company);

                    if (mail.Text != null && mail.Text.Equals(html))
                        html = null;
                }
            }

            if (!StrEmpty(html))
            {
                // Comprobar carpeta base para imagenes y contenido
                // Si no se da se ha debido cargar de los adjuntos
                // Si sigue vacia cargarla de la ruta del logo
                if (StrEmpty(BaseDir))
                {
                    if (!String.IsNullOrEmpty(mail.Logo))
                        BaseDir = Path.GetDirectoryName(mail.Logo);
                }

                // Definir las imagenes existentes en el cuerpo
                html = AjustImages(html);

                // Quitar codigo javascript si esta configurado
                if (RemoveScript)
                    html = RemoveScripts(html);

                // Ajustar elemtos de estilo Css requeridos
                if (RemoveHidden || RemoveScript)
                    html = AjustStyle(html);

                // Crear recursos de imagenes para el html
                if (Images != null)
                {
                    foreach (ImageDef img in Images)
                    {
                        try
                        {
                            // MediaTypeNames media = MediaTypeNames.Image.Gif;
                            var link = builder.LinkedResources.Add(img.Path);
                            link.ContentId = img.Name;
                            // link.ContentId = MimeUtils.GenerateMessageId();
                        }
                        catch (Exception exc)
                        {
                            Logger.LogError(exc);
                        }
                    }
                }

                // Set the html version of the message text
                builder.HtmlBody = html;
            }

            // Cargar el cuerpo del mensaje calculado
            message.Body = builder.ToMessageBody();

            return message;
        }

        #endregion


#if !UseMailKit
        
        public bool SendNetMail( object[] email )
        {
            bool resul = true; 

            // Extraer parametros del array de datos
            // Puertos: 25  Smtp Normal
            //         465: Conexion TLS directa;
            //         587: Plain text elevado a TLS con STARTTLS
            //         En la practica se usa siempre 587 para TTLS
            // ---------------------------------------------------
            string host  = GetValue<string>(email, MailProps.Server);
            bool   isSsl = GetValue<bool>(email, MailProps.EnableSsl);

            int    port = GetValue<int>(email, MailProps.Port);

            if (port == 465)   // Normalmente no se usa este puerto
                isSsl = true;  // Pero si se usa debe ser TLS

            string user  = GetValue<string>(email, MailProps.User);
            string pass  = GetValue<string>(email, MailProps.Password);
            string from  = GetValue<string>(email, MailProps.FromField);
            string to    = GetValue<string>(email, MailProps.ToField);
            string subject = GetValue<string>(email, MailProps.Subject);
            string company = GetValue<string>(email, MailProps.Company);
            string text  = GetValue<string>(email, MailProps.TextField);
            string files = GetValue<string>(email, MailProps.FileNames);
            int    timeout = GetValue<int>(email, MailProps.Timeout);
            bool   confirm = GetValue<bool>(email, MailProps.Confirm);


            to = to.TrimEnd();
            from = from.TrimEnd();

            string logo = GetValue<string>(email, MailProps.MailLogo); 
            string footer = GetValue<string>(email, MailProps.MailFooter); 

            string idMail  = GetValue<string>(email, MailProps.MailIdent); 
            string keyGest = GetValue<string>(email, MailProps.KeyGest); 
            string keyIdent= GetValue<string>(email, MailProps.KeyIdent); 

            // Cuenta de pruebas de gmail
            // host  = "smtp.gmail.com";
            // user  = "infosersistemas@gmail.com"; 
            // pass  = "infoser459300";
            // port  = 587;
            // isSsl = true;
            // from  = "infosersistemas@gmail.com";
            // company = "Infoser Sistemas";

            // Cadena de resultado y de logger de envios
            string mailError  = "";

            try
            {
                // Crear y configurar instancia de SmtpClient 
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(user, pass);

                SmtpServer.Port = port;
                SmtpServer.Host = host;
                SmtpServer.EnableSsl = isSsl;

                // Crear y configurar instancia del mensaje
                MailMessage mail = new MailMessage();

                MailAddress adr = new MailAddress(from, company, System.Text.Encoding.UTF8);
               
                mail.Sender = adr;  // Dirección que envia realmente el correo
                mail.From   = adr;  // Cabecera del menaje from

                mail.Subject = subject.TrimEnd();

                // El campo To puede tener varias direcciones 
                String[] addr = to.Split(';');

                for (int index = 0; index < addr.Length; index++)
                {
                    mail.To.Add(addr[index]);
                }

                // El campo Files puede tener varias archivos
                if (!String.IsNullOrEmpty(files))
                {
                    String[] names = files.Split(';');
                        
                    for (int index = 0; index < names.Length; index++)
                    {
                        string file = names[index];

                        int separ = file.IndexOf('=');

                        if (separ > 0)
                            file = file.Substring(0, separ);

                        if (StrEmpty(BaseDir))
                        {
                            BaseDir = Path.GetDirectoryName(file);
                        }

                        mail.Attachments.Add(new Attachment(file));
                    }
                }

                // Direcciones de retorno y confirmacion
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                mail.ReplyTo = new MailAddress(from);

                if (confirm)
                {
                    mail.Headers.Add("Disposition-Notification-To", from.ToString());
                }

                // Componer el cuerpo como texto plano o html
                string html = null;

                if (StrIsHtml( text ))
                {
                    // Texto sumistrado como Html: Añadir el pie
                    mail.IsBodyHtml = true;

                    html = TextToHtml( text );
                    html = AddFooter(html, footer, logo, company);

                    // Crear texto plano desde el codigo html 
                    // El cuerpo debe ir con la version texto
                    // Los correos lo muestran en los avisos

                    text = HtmlToText( text );
                    mail.Body = text;
                }
                else
                {
                    if (text == null)
                    {
                        // No esta cargado el texto: No enviar nada
                        mailError = "No se ha definido el texto del correo";
                        resul = false;
                    }
                    else
                    {
                        // El cuerpo es directamente texto plano
                        mail.Body = text;

                        // Puede haber una version html con el pie
                        html = AddFooter(text, footer, logo, company);

                        if (text != null && text.Equals(html))
                            html = null;
                    }
                }

                if (!StrEmpty(html) )
                {
                    // Comprobar carpeta base para imagenes y contenido
                    // Si no se da se ha debido cargar de los adjuntos
                    // Si sigue vacia cargarla de la rita del logo
                    if (StrEmpty(BaseDir))
                    {
                        if (logo != null)
                            BaseDir = Path.GetDirectoryName(logo);
                    }

                    // Definir las imagenes existentes en el cuerpo
                    html = AjustImages( html );

                    // Crear recursos de imagenes para el html
                    AlternateView view = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);

                    if (Images != null)
                    {
                        foreach (ImageDef img in Images)
                        {
                            try
                            {
                                // MediaTypeNames media = MediaTypeNames.Image.Gif;

                                LinkedResource link;

                                link = new LinkedResource(img.Path, img.Mime);
                                link.ContentId = img.Name;

                                view.LinkedResources.Add(link);
                            }
                            catch (Exception exc)
                            {
                                Logger.LogError(exc);
                            }
                        }
                    }

                    mail.AlternateViews.Add(view);
                    mail.IsBodyHtml = true;
                    // mail.Body = html;
                }

                // Enviar  y registrar correo enviado
                if (resul)
                {
                    SmtpServer.Send(mail);
                }
            }
            catch (Exception exc)
            {
                mailError = exc.Message;
                if (exc.InnerException != null)
                {
                    mailError += '\n' + exc.InnerException.Message;
                }

                Logger.LogError(exc);
                resul = false;
            }

            if (resul)
            {
                NumSend++;
                mailError = "Enviado";
                MailResult ="Destinatario: "+ to + "\n Correo enviado";
            }
            else
            {
                NumError++;
                MailResult = "Destinatario: "+to + ". No se puede enviar el correo: " +
                                  mailError;
            }

            if (CreateLogger)
                LogData(to, subject, from, mailError, idMail, keyGest, keyIdent);

            return resul;
        }

#endif

        /// <summary> Añade el pie y logo del cuerpo del correo
        /// </summary>
        /// <param name="text"> Texto actual del correo </param>
        /// <param name="footer"> Texto o Html del pie  </param>
        /// <param name="logo"> Ruta de imagen de logo  </param>
        /// <returns></returns>

        private string AddFooter( string text, string footer, string logo, string company )
        {
            if (!StrEmpty(logo))
                logo = FindImage( logo );

            if (!StrEmpty(logo))
            {
                if (StrEmpty(footer) && !StrEmpty(company))
                {
                    String[] data = company.Split(';');

                    string name = null;
                    string url  = null;
                    
                   if (data.Length > 0)
                       name = data[0];

                   if (data.Length > 1)
                       url = data[1];

                   footer = "<br /><br />" +
                            "<hr style='border:none; color:#909090; background-color:#B0B0B0; height: 1px; width: 99%;'/>" +
                            "<table style='border-collapse:collapse;border:none;'>" +
                            "<tr><td style='border:none;padding:0px 15px 0px 8px'>";

                    if (url != null)
                        footer += "<a href=\"" + url + "\">";

                    footer += "<img src=\"cid:Logo\" border=0 /></a></td>";

                    if (name != null)
                        footer += "<td><p style=\"color:#ffffff; font:14px Arial\" "+
                                  name+"</p></td>";
                                  
                    footer += "</tr></table>";
                }
            }

            if (!StrEmpty(footer))
                text = AppendHtml(text, footer);

            if (!StrEmpty(logo))
                AddImage(logo, null);

            return text;
        }

        /// <summary> Busca y completa al ruta de una imagen
        /// </summary>
        /// <param name="image"> Nombre o path de la Imagen </param>
        /// <returns> Ruta completa de la imagen </returns>
        /// <remarks>
        /// Las imagenes pueden ser localizadas de varias formas:
        /// - Ruta absoluta en el propio enlace
        /// - Url para cargar la imagen
        /// - Ruta base del html o imagenes: BaseDir
        /// - Carpeta Images sobre esta ruta base
        /// 
        /// </remarks>

        private string FindImage( string image )
        {
            if (image == null)
                return null;

            string path = null;

            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            if (image.StartsWith("http:", comp)  || 
                image.StartsWith("https:", comp) ||
                image.StartsWith("/", comp))
            {
                try
                {
                    path = DownLoadFile(image);
                }
                catch(Exception exc)
                {
                }

                if (path == null)
                    return image;
                else
                    image = path;
            }

            if (Path.IsPathRooted(image))
                return image;

            path = BaseDir;

            if (!String.IsNullOrEmpty(path))
            {
                if (!path.EndsWith("\\"))
                    path += '\\';

                if (File.Exists(path + image))
                {
                    path += image;
                }
                else
                {
                    path += "Image\\"+image;

                    if (!File.Exists(path + image))
                        return null;
                }
            }

            return path;
        }

        class ImageDef
        {
            public string Path;   // Path completo de la imagen
            public string Name;   // Identificador de la imagen
            public string Mime;   // Cadena con el tipo mime

            public ImageDef( string path, string name, string type )
            {
                Path = path;
                Name = name;
                Mime = type;
            }
        }

        private string AddImage(string image, string name)
        {
            if (name == null)
                name = Path.GetFileNameWithoutExtension(image).ToLower();

            StringComparison comp = StringComparison.OrdinalIgnoreCase;
            bool found = false;
            int count = 0;

            if (Images == null)
                Images = new List<ImageDef>();

            for (int index = 0; index < Images.Count; index++)
            {
                ImageDef img = Images[index];

                if (img.Path.Equals(image, comp))
                {
                    // Es la misma ruta de imagen
                    found = true;
                    break;
                }

                if (img.Name.StartsWith(name, comp))
                {
                    if (img.Name.Equals(name, comp) ||
                        img.Name.Equals(name + '_' + count.ToString(), comp))
                    {
                        count++;
                    }
                }
            }

            if (!found)
            {
                if (count > 0)
                    name += '_' + count.ToString();

                string type = Path.GetExtension(image);

                if (!StrEmpty(type) && type.Length > 2)
                {
                    type = "Image/" + type.Substring(1, 1).ToUpper() +
                                      type.Substring(2).ToLower(); ;
                }
                else
                {
                    type = null;
                }

                Images.Add(new ImageDef(image, name, type));
            }

            return name;
        }

        string DownLoadFile(string url, string folder = null)
        {
            if (folder == null)
                folder = "images";

            string path = BaseDir + folder;

            var image = Path.GetFileName(url);
            var file = GetFilePath(image, path);

            if (!File.Exists(file))
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    using (WebClient downloader = new WebClient())
                    {

                        downloader.DownloadFile(url, file);

                    }

                    bool ValidateServerCertificate(Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                                                  System.Security.Cryptography.X509Certificates.X509Chain chain,
                                                                  SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    }
                }
                catch (ArgumentException exc)
                {
                    Logger.LogError(exc);
                    MailResult = "Error enviando correo: " + exc.Message;

                    file = null;
                }
            }
            return file;
        }

        /// <summary> Carga el fichero de deficion de emails
        /// </summary>
        /// <param name="fileName"> Path al fichero a cargar </param>
        /// <returns> Array de emails decodificado </returns>

        private object[] LoadFile( string fileName )
        {
            object[] items = null;

            if (File.Exists(fileName))
            {
                // var lines = File.ReadAllLines(fileName);

                byte[] data = File.ReadAllBytes(fileName);

                MsgEncode msg = new MsgEncode();
                items = msg.GetValues(data);
            }

            return items;
        }

        private T GetValue<T>( object[] email, MailProps data)
        {
            int index = (int)data;

            T resul = GetParam<T>( email, index);

            return resul;
        }

#region METODOS GENERALES DE SOPORTE (COMUN)

        /// <summary> Retorna un parametro de tipo dado como un elemento 
        /// Si se da un array del tipo pedido retorna el primer elemento
        /// Si no existe el parametro o es de otro tipo retorna nulo
        /// </summary>
        /// <typeparam name="T"> Tipo del parametro esperado   </typeparam>
        /// <param name="par">   Array de parametros pasados   </param>
        /// <param name="index"> Numero de orden del parametro </param>
        /// <returns> Elemento del tipo dado </returns>

        public static T GetParam<T>(Array par, int index) 
        {
            if (par != null && par.Length > index)
            {
                object item = par.GetValue(index);

                if (item is T)
                    return (T)item;
                else
                {
                    if (item is T[])
                    {
                        if (((T[])item).Length > 0)
                            return ((T[])item)[0];
                    }
                    else
                    {
                        try
                        {
                            if (typeof(T).IsEnum)
                                return (T)(object)item;
                            else
                                return (T)Convert.ChangeType(item, typeof(T));
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return default(T);
        }

        /// <summary> Comprueba si un string esta vacio
        /// Acepta como vacio un string sin inicializar
        /// </summary>
        /// <param name="Cadena"> String a comprobar </param>
        /// <returns> true si el string esta vacio o es nulo </returns>

        public bool StrEmpty(string Cadena)
        {
            if (Cadena == null || Cadena.Length == 0)
                return true;

			int nTotal = Cadena.Length;

			for (int nIndex = 0; nIndex < nTotal; nIndex++)
			{
				if (!Char.IsWhiteSpace(Cadena[nIndex]))
					return false;
			}
			return true;
        }

        public bool StrIsHtml( string text )
        {
            if (StrEmpty(text))
                return false;

            bool resul = ((text.IndexOf('<') >= 0) && (text.IndexOf(">") >= 0));

            return resul;

            // return ((text.IndexOf('<') >= 0) && (text.IndexOf(">") >= 0));
        }


        public bool StrIsUtf( string text )
        {
            int index1 = text.IndexOf("charset=utf-8");
            int index2 = text.IndexOf("<body");

            if (index1 > 0 && index1 < index2)
                return true;

            return false;
        }


        /// <summary> Retorna valor de un elemento de una lista
        /// </summary>
        /// <param name="Lista"> Lista de elementos </param>
        /// <param name="Orden"> Indice del elemnto </param>
        /// <param name="Separ"> Separador de lista </param>
        /// <returns> Valor del elemento </returns>

        public string GetItem(string lista, int orden, char Separ = ',')
        {
            if (lista != null && orden >= 0)
            {
                // Buscar el numeor de elemento pedido
                int nIndex, nNext = -1;
                do
                {
                    nIndex = nNext;
                    nNext = lista.IndexOf(Separ, ++nIndex);

                    if (nNext == -1)
                    {
                        if (orden > 0)
                            return null;

                        nNext = lista.Length;
                    }
                }
                while (orden-- > 0);

                // Quitar blancos al inicio del token
                while (nIndex < nNext)
                {
                    if (Char.IsWhiteSpace(lista, nIndex))
                        nIndex++;
                    else
                        break;
                }

                // Quitar blancos al final del token
                while (nIndex < nNext)
                {
                    if (Char.IsWhiteSpace(lista, nNext - 1))
                        nNext--;
                    else
                        break;
                }

                return lista.Substring(nIndex, nNext - nIndex);
            }

            // Se cambia retorno si no existe el campo
            // Debe ser coherente si falta antes o detras
            // Comprobar efecto en coidigo existente +++
            // return null;
            return String.Empty;
        }


        /// <summary> Retorna directori completo de un fichero
        /// No toca los ficheros dados con un path absoluto
        /// </summary>
        /// <param name="file">  Nombre o path del Fichero </param>
        /// <param name="folder">Directorio base </param>
        /// <returns> Directorio completo del fichero </returns>

        public string GetFilePath(string file, string folder)
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

        public string GetFile(string DirBase, string DirPath)
        {
            int nPos;
            if (!string.IsNullOrEmpty(DirBase))
            {
                nPos = DirBase.Length - 1;

                // Si el path base no tiene barra final se la añadimos
                char cSepar = Path.DirectorySeparatorChar;

                if (DirBase[nPos] != cSepar)
                    DirBase = DirBase + cSepar;

                // retroceder directorios sobre el base si se pide
                // while (DirPath.Contains( ".." + cSepar ))
                if (DirPath == null)
                    DirPath = DirBase;
                else
                {
                    while (DirPath.IndexOf(".." + cSepar) >= 0)
                    {
                        if (nPos > 0)
                        {
                            if (DirBase[nPos] == cSepar)
                                nPos--;

                            nPos = DirBase.LastIndexOf(cSepar, nPos);
                            if (nPos > 0)
                                DirBase = DirBase.Substring(0, nPos + 1);
                        }

                        DirPath = DirPath.Substring(3, DirPath.Length - 3);
                    }
                    DirPath = DirBase + DirPath;
                }
            }

            // retornar directorios combinados
            return DirPath;
        }




        #endregion

        #region METODOS DE SOPORTE DE TEXTOS HTML

        /// <summary> Combina una cadena de texto o html con otra dada
        /// </summary>
        /// <param name="text"> Cadena principal </param>
        /// <param name="append"> Cadena a concatenar </param>
        /// <returns> Resultado completo </returns>

        private string AppendHtml(string text, string append)
        {
            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            if (!StrEmpty(append))
            {

                if (StrEmpty(append))
                {
                    text = append;
                }
                else
                {
                    int index1 = text.IndexOf("</body>", 0, comp);

                    if (index1 > 0)
                    {
                        text = text.Substring(0, index1) +
                               ExtractBody(append) +
                               text.Substring(index1);
                    }
                    else
                    {
                        index1 = append.IndexOf("<body>", 0, comp);

                        if (index1 > 0)
                        {
                            text = append.Substring(0, index1) +
                                   text + append.Substring(index1);
                        }
                        else
                        {
                            if (!StrEmpty(text))
                                text += "\n";

                            text += append;
                        }
                    }
                }
            }

            if (StrIsHtml(text))
            {
                if (text.IndexOf("<html>", comp) < 0)
                {
                    if (text.IndexOf("<body>", comp) < 0)
                        text = "<html><body>" + text + "</body></html>";
                    else
                        text = "<html>" + text + "</html>";
                }
            }

            return text;
        }

        /// <summary> Devuelve la parte interna del html
        /// </summary>
        /// <param name="text"> Cadena a analizar </param>
        /// <returns> Texto interno html </returns>

        private string ExtractBody(string text)
        {
            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            int index1 = text.IndexOf("<body>", 0, comp);

            if (index1 >= 0)
            {
                int index2 = text.IndexOf("</body>", index1, comp);

                if (index2 > 0)
                    text = text.Substring(index1 + 6, index2 - index1 - 6);
            }
            return text;
        }

        /// <summary> Elimina codigo Javascrips del html dado
        /// </summary>
        /// <param name="html"> Texto html a procesar </param>
        /// <returns> Texto Html procesado </returns>

        private string RemoveScripts(string text)
        {
            if (StrEmpty(text))
                return text;

            int index1 = 0;
            int index2 = 0;
            
            while (index1 >= 0)
            {
                int index = FindToken(text, null, ref index1, ref index2,
                                            "<script", "</script>");
                if (index >= 0)
                {
                    text = RemoveString( text, index1-7, index2+9);
                    index1 = index2 = 0;
                }
                else
                    break;
            }

            return text;
        }

        /// <summary> Ajusta elementos de estilo problematicos
        /// Actualmente elimina solo el estibo bidy hidden
        /// </summary>
        /// <param name="html"> Texto html a procesar </param>
        /// <returns> Texto Html procesado </returns>

        private string AjustStyle(string text)
        {
            if (StrEmpty(text))
                return text;

            int style1 = 0;
            int style2 = 0;

            int body = FindToken(text, "body", ref style1, ref style2, "<style", "</style>");

            if (body >= 0)
            {
                var tokens = new string[] { "visibility:"  };

                foreach (string token in tokens)
                {
                    int key1 = body;
                    int key2 = style2;

                    int index = FindToken(text, token, ref key1, ref key2, "{", "}");

                    if (index >= 0)
                    {
                        int line1 = index;
                        int line2 = key2;

                        string value = GetLine(text, ref line1, ref line2, index + token.Length);

                        if (value.Trim().Equals("hidden", StringComparison.OrdinalIgnoreCase))
                        {
                            // text = text.Substring(0, line1+1) + text.Substring(line2+1);
                            int offset;
                            text = RemoveString(text, line1, line2, out offset);
                            key2   -= offset;
                            style2 -= offset;

                            if (IsEmptyLine(text, key1, key2))
                            {
                                text = RemoveString( text, body, key2+1, out offset);
                                style2 -= offset;

                                if (FindToken(text, "{", ref style1, ref style2) == -1)
                                { 
                                    text = RemoveString( text, style1-7, style2 + 8);
                                }
                            }
                        }
                    }
                }
            }

            return text;
        }

        /// <summary> Quita un substring de string dado y retorna diferencia
        /// </summary>
        /// <param name="line">  Texto donde quitar string </param>
        /// <param name="start"> Indice inicial a suprimir </param>
        /// <param name="stop">  Indice final a suprimir   </param>
        /// <param name="offset">Reduccion neta de longitud</param>
        /// <returns> Cadena modificiada </returns>

        private string RemoveString(string line, int start, int stop, out int offset)
        {
            int nlines = 0;
            int nchar = 1;

            for (int ind = stop + 1; ind < line.Length - 1; ind++)
            {
                if (line[ind] == '\r')
                {
                    nchar = 2;
                    ind++;
                }

                if (line[ind] == '\n')
                {
                    if (nlines == 0)
                        nlines++;
                    else
                    {
                        stop += nchar;
                        break;
                    }
                }
                else
                    break;
                
            }

            line = line.Substring(0, start) + line.Substring(stop + 1);
            offset = stop - start + 1;

            return line;           
        }

        private string RemoveString(string line, int start, int stop)
        {
            int offset;

            return RemoveString(line, start, stop, out offset);
        }




        /// <summary> Comprueba  contenido vacio o saltos linea enter indices
        /// </summary>
        /// <param name="lines"> Cadena con lineas de texto </param>
        /// <param name="start"> Inicio de linea o busqueda </param>
        /// <param name="stop">  Final de linea o busqueda  </param>
        /// <returns> Contenido vacio en el rango dado </returns>

        private bool IsEmptyLine(string line, int start, int stop)
        {
            return String.IsNullOrWhiteSpace(line.Substring(start, stop - start + 1));
        }

        /// <summary> retorna texto hasta final de linea desde un indice
        /// </summary>
        /// <param name="lines"> Cadena con lineas de texto </param>
        /// <param name="start"> Inicio de linea o busqueda </param>
        /// <param name="stop">  Final de linea o busqueda  </param>
        /// <param name="index"> Indice inicial a retornar  </param>
        /// <returns> Texto hasta final de la linea </returns>
        /// <remarks>
        /// El parametro start debe estar dentro de la linea buscada
        /// Se actualizan Los indices inicial y final de linea actual
        /// El parametro indice puede ser nulo o dentro de esta linea
        /// El texto se retorna desde el indice dado hasta fin de linea
        /// Si se da indice nulo retorna linea completa desde el inicio
        /// </remarks>

        private string GetLine( string lines, ref int start, ref int stop, int index = 0)
        {
            int endlin = lines.IndexOf('\n', start, stop-start+1);

            if (endlin == -1)
                endlin = lines.Length;

            string line = "";

            if (endlin >= 0)
            {
                while( start > 0 && lines[start - 1] != '\n')
                {
                    start--;
                }

                if (index < start)
                    index = start;

                stop = endlin;

                if (lines[endlin] == '\n')
                    endlin--;

                if (lines[endlin] == '\r')
                    endlin--;

                line = lines.Substring(index, endlin - index + 1);
            }

            return line;
        }

        /// <summary> Ajusta las imagenes del texto html como recursos
        /// </summary>
        /// <param name="html">   Texto html a procesar </param>
        /// <param name="images"> Coleccion de imagenes </param>
        /// <returns> Lista de imagenes encontrada </returns>
        /// <remarks>
        /// Para cada imagen se crea un identificador en base a su nombre 
        /// Las imagenes de la lista deben añadirse al email como recursos
        /// El identificador debe ser el propio nombre 
        /// 
        /// </remarks>

        private string AjustImages( string text )
        {
            if (StrEmpty(text))
                return text;

            foreach (string tag in new string[] { "img", "amp-img" })
            {
                int index1 = 0;
                int index2 = 0;

                while (index1 >= 0)
                {
                    StringComparison comp = StringComparison.OrdinalIgnoreCase;

                    index1 = text.IndexOf("<"+tag, index1, comp);

                    if (index1 >= 0)
                    {
                        int index3 = text.IndexOf("src=", index1, comp);

                        if (index3 > 0)
                        {
                            index3 += 5;  // Apunta al inicio de la ruta de imagen
                            index2 = text.IndexOf("\"", index3, comp);

                            if (index2 > 0)
                            {
                                string image = text.Substring(index3, index2 - index3);
                                string path = FindImage(image);

                                if (path != null)
                                {
                                    if (!path.StartsWith("https:"))
                                    {
                                        string name = AddImage(path, null);

                                        text = text.Substring(0, index3) + "cid:" + name + text.Substring(index2);
                                    }
                                }

                                if (tag != "img")
                                {
                                    text = text.Substring(0, index1) + "<img" + text.Substring(index1 + tag.Length+1);

                                    int index4 = text.IndexOf("</" + tag, index1, comp);

                                    if (index4 >= 0)
                                        text = text.Substring(0, index4) + "</img>" + text.Substring(index4 + tag.Length+3);
                                }

                                index1 = index2;
                            }
                            else
                                break;
                        }

                    }
                }
            }

            text = text.Replace("layout=\"intrinsic\"", "");


            return text;
        }

        /// <summary> Busca un tokens entreo unos indices y tokens dados
        /// </summary>
        /// <param name="text">  Texto completo donde buscar</param>
        /// <param name="token"> Token a buscar en el rango </param>
        /// <param name="index1">Indice inicial de busqueda </param>
        /// <param name="index2">Indice final de busqueda   </param>
        /// <param name="token1">Token inicial de busqueda  </param>
        /// <param name="token2">Token final de busqueda    </param>
        /// <returns> Indice del token o incio del rango </returns>
        /// <remarks>
        /// Este metodo busca token en un rango entre indices y tokens
        /// Si no se dan el indice inicial utilizar inicio del texto
        /// Si no se dan el indice final utiliza la longitud del texto
        /// Si se da el token inicial busca y ajusta el indice inicial
        /// Si se da el token final busca y ajusta el indice final
        /// Si no se da el token retirna indoce inicial del rango
        /// 
        /// Los indices pasado se actualizan como siguen:
        /// index1: apunta al principio del token inicial dado
        /// index2: apunta al principio del token final dado          
        /// index:  apunta al principio del token buscado
        ///         Si no se da token apunta detras del token inicial
        /// </remarks>

        private int FindToken(string text, string token, ref int index1, ref int index2,
                              string token1 = null, string token2 = null)
        {
            int ind1, ind2;

            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            if (String.IsNullOrWhiteSpace(text))
                return -1;

            if (index1 < 0)
                index1 = 0;

            if (index2 <= 0)
                index2 = text.Length - 1;

            int count = index2 - index1 + 1;

            if (token1 != null)
            {
                ind1 = text.IndexOf(token1, index1, count, comp);

                if (ind1 > index1)
                {
                    index1 = ind1+token1.Length;
                    count  = index2 - index1 + 1;
                }
                else
                    return -1;
            }

            if (token2 != null)
            {
                ind2 = text.IndexOf(token2, index1, count, comp);

                if (ind2 >= index1)
                {
                    index2 = ind2-1;
                    count  = index2 - index1 + 1;
                }
                else
                    return -1;
            }

            int index = index1;

            if (token != null)
            {
                index = text.IndexOf(token, index1, count, comp);
            }
            else
            {
                if (token1 != null)
                    index = index1;
            }

            return index;
        }

        #endregion

        #region METODOS DE CONVERSION HTML

        /// <summary> Convierte los caracteres no ASCII a entidades html
        /// El texto origen puede contener otras marcas que se mantienen
        /// </summary>
        /// <param name="text"> Cadena con el texto a convertir </param>
        /// <returns> Texto convertido a codificación HTML </returns>
        /// <remarks>
        /// Este método cambia únicamente los caracteres no ASCII
        /// Estos codigos se convierten a entidades standrd html 
        /// 
        /// Se comprueban solo los primeros 256 codigos de ISO-8859-1
        /// Esto es suficiente para la gran mayoria de aplicaciones 
        /// 
        /// El juego de caracteres por defecto del navegador es ISO-8859-1
        /// Este juego se usa en toda Europa occidental, Estados unidos,
        /// canada, africa, caribe y latinoamerica
        /// Por tanto se puede considerar con validez y amplituz suficiente
        /// Quedan fuera caracteres cirilicos, arabes, nordicos, etc
        /// 
        /// El UNICODE representa un standard para cualquier lenguaje
        /// Los primeros 256 caracteres coindicen con la ISO-8859-1
        /// Por tanto es compatible con el rango de paises citado
        /// 
        /// El standard Unicode se implementa con varios sistemas:
        /// 
        ///   - UTF-8: Usa codigos de 8 bits codificado de 0-4 bytes
        ///     Es el sistema recomendado para email y paginas web
        ///     Los primeros 127 caracteres coinciden con el ASCII
        ///     
        ///   - UTF-16: Usa codigos de 16 bits de longitud variable
        ///     Es el sistema usado en plataformas windows y .NET  
        ///    
        /// </remarks>

        public string TextToHtml(string text)
        {
            if (text != null)
            {
                int limite = -1;

                if (HtmlTable == null)
                    CreateHtmlTable();

                for(int index=0; index < text.Length; index++)
                {
                    int chr = (int)text[index];

                    if (index == limite)
                    {

                    }

                    if (chr >= 160 && chr <= 255)
                    {
                        // Convertir el caracter a entidad html si est definida
                        // No se deben convertir signos menor/mayor ni ampersand
                        // Estos simbolos componen las marcas y entidades html

                        string token = HtmlTable[chr-1];

                        if (token != null)
                        {
                            text = text.Substring(0, index) + '&' + token + ';' +
                                   text.Substring(index + 1);
                            
                            index += (token.Length + 2);
                        }
                    }
                }
            }
            return text;
        }

        public string HtmlToText(string html )
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            string text = reg.Replace(html, "");

            if (text.IndexOf("&") >= 0)
            {
                text = text.Replace("&#9;", "\t");
                text = text.Replace("&#10;", "\n");
                text = text.Replace("&#13;", "\r");
                text = text.Replace("&#32;", " ");
                text = text.Replace("&#32;", " ");
                text = text.Replace("&nbsp;", " ");
                text = text.Replace("&#38;", "&");
            }

            return text;
        }

        static private string[] HtmlTable;

        static private void CreateHtmlTable()
        {
            HtmlTable = new string[255];

            // Caracteres Html reservados 
            HtmlTable[33] = "quot";
            HtmlTable[38] = "apos";  // No funciona en IE
            HtmlTable[37] = "amp";
            HtmlTable[59] = "lt";
            HtmlTable[61] = "gt";

            // Caracteres de simbolos html

            HtmlTable[159] = "nbsp";    // non-breaking space
            HtmlTable[160] = "iexcl";   // inverted exclamation mark
            HtmlTable[161] = "cent";    // cent
            HtmlTable[162] = "pound";   // pound
            HtmlTable[163] = "curren";  // currency
            HtmlTable[164] = "yen";     // yen
            HtmlTable[165] = "brvbar";  // broken vertical bar
            HtmlTable[166] = "sect";    // section
            HtmlTable[167] = "uml";     // spacing diaeresis
            HtmlTable[168] = "copy";    // copyright
            HtmlTable[169] = "ordf";    // feminine ordinal indicator
            HtmlTable[170] = "laquo";   // angle quotation mark (left)
            HtmlTable[171] = "not";     // negation
            HtmlTable[172] = "shy";     // soft hyphen
            HtmlTable[173] = "reg";     // registered trademark
            HtmlTable[174] = "macr";    // spacing macron
            HtmlTable[175] = "deg";     // degree
            HtmlTable[176] = "plusmn";  // plus-or-minus
            HtmlTable[177] = "sup2";    // superscript 2
            HtmlTable[178] = "sup3";    // superscript 3
            HtmlTable[179] = "acute";   // spacing acute

            HtmlTable[180] = "micro";   // micro
            HtmlTable[181] = "para";    // paragraph
            HtmlTable[182] = "middot";  // middle dot
            HtmlTable[183] = "cedil";   // spacing cedilla
            HtmlTable[184] = "sup1";    // superscript 1
            HtmlTable[185] = "ordm";    // masculine ordinal indicator
            HtmlTable[186] = "raquo";   // angle quotation mark (right)
            HtmlTable[187] = "frac14";  // fraction 1/4
            HtmlTable[188] = "frac12";  // fraction 1/2
            HtmlTable[189] = "frac34";  // fraction 3/4
            HtmlTable[190] = "iquest";  // inverted question mark

            HtmlTable[214] = "times";   // multiplication
            HtmlTable[246] = "divide";  // division

            // Caracteres especificos de idiomas
            HtmlTable[191] = "Agrave";  // capital a, grave accent
            HtmlTable[192] = "Aacute";  // capital a, acute accent
            HtmlTable[193] = "Acirc";   // capital a, circumflex accent
            HtmlTable[194] = "Atilde";  // capital a, tilde
            HtmlTable[195] = "Auml";    // capital a, umlaut mark
            HtmlTable[196] = "Aring";   // capital a, ring
            HtmlTable[197] = "AElig";   // capital ae
            HtmlTable[198] = "Ccedil";  // capital c, cedilla
            HtmlTable[199] = "Egrave";  // capital e, grave accent
            HtmlTable[200] = "Eacute";  // capital e, acute accent
            HtmlTable[201] = "Ecirc";   // capital e, circumflex accent
            HtmlTable[202] = "Euml";    // capital e, umlaut mark
            HtmlTable[203] = "Igrave";  // capital i, grave accent
            HtmlTable[204] = "Iacute";  // capital i, acute accent
            HtmlTable[205] = "Icirc";   // capital i, circumflex accent
            HtmlTable[206] = "Iuml";    // capital i, umlaut mark
            HtmlTable[207] = "ETH";     // capital eth, Icelandic
            HtmlTable[208] = "Ntilde";  // capital n, tilde
            HtmlTable[209] = "Ograve";  // capital o, grave accent
            HtmlTable[210] = "Oacute";  // capital o, acute accent
            HtmlTable[211] = "Ocirc";   // capital o, circumflex accent
            HtmlTable[212] = "Otilde";  // capital o, tilde
            HtmlTable[213] = "Ouml";    // capital o, umlaut mark

            HtmlTable[215] = "Oslash";  // capital o, slash
            HtmlTable[216] = "Ugrave";  // capital u, grave accent
            HtmlTable[217] = "Uacute";  // capital u, acute accent
            HtmlTable[218] = "Ucirc";   // capital u, circumflex accent
            HtmlTable[219] = "Uuml";    // capital u, umlaut mark
            HtmlTable[220] = "Yacute";  // 
            HtmlTable[221] = "THORN";   // capital THORN, Icelandic
            HtmlTable[222] = "szlig";   // small sharp s, German
            HtmlTable[223] = "agrave";  // small a, grave accent
            HtmlTable[224] = "aacute";  // small a, acute accent
            HtmlTable[225] = "acirc";   // small a, circumflex accent
            HtmlTable[226] = "atilde";  // small a, tilde
            HtmlTable[227] = "auml";    // small a, umlaut mark 
            HtmlTable[228] = "aring";   // small a, ring
            HtmlTable[229] = "aelig";   // small ae
            HtmlTable[230] = "ccedil";  // small c, cedilla
            HtmlTable[231] = "egrave";  // small e, grave accent
            HtmlTable[232] = "eacute";  // small e, acute accent
            HtmlTable[233] = "ecirc";   // small e, circumflex accent
            HtmlTable[234] = "euml";    // small e, umlaut mark
            HtmlTable[235] = "igrave";  // small i, grave accent
            HtmlTable[236] = "iacute";  // small i, acute accent
            HtmlTable[237] = "icirc";   // small i, circumflex accent
            HtmlTable[238] = "iuml";    // small i, umlaut mark
            HtmlTable[239] = "eth";     // small eth, Icelandic
            HtmlTable[240] = "ntilde";  // small n, tilde
            HtmlTable[241] = "ograve";  // small o, grave accent
            HtmlTable[242] = "oacute";  // small o, acute accent
            HtmlTable[243] = "ocirc";   // small o, circumflex accent
            HtmlTable[244] = "otilde";  // small o, tilde
            HtmlTable[245] = "ouml";    // small o, umlaut mark
            
            HtmlTable[247] = "oslash";  // small o, slash
            HtmlTable[248] = "ugrave";  // small u, grave accent
            HtmlTable[249] = "uacute";  // small u, acute accent
            HtmlTable[250] = "ucirc";   // small u, circumflex accent
            HtmlTable[251] = "uuml";    // small u, umlaut mark
            HtmlTable[252] = "yacute";  // small y, acute accent
            HtmlTable[253] = "thorn";   // small thorn, Icelandic
            HtmlTable[254] = "yuml";    // small y, umlaut mark
        }

#endregion

#region METODOS DE CREACION DE LOGGER

        private const int Mail_Lenght = 50;

        static public bool LogData(string to, string subject, string from,
                           string result, string idMail, string keyGest, string keyIdent )
        {

            string fields = DateTime.Now.Date.ToShortDateString() + '\t' +
                            DateTime.Now.ToShortTimeString() + '\t' +
                            to    + '\t' + subject + '\t' + from + '\t' + result + '\t' +
                            idMail+ '\t' + keyGest + '\t' + keyIdent;

            return Logger.LogLine("MailData", fields, "");
        }

        public bool ClearData()
        {
            return Logger.LogClear("MailData");
        }

#endregion

    }
    
}
