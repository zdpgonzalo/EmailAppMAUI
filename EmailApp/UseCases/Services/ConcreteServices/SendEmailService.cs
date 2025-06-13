using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System.Net;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MailKit.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using MailKit.Security;
using MailAppMAUI.Config;
using MailAppMAUI.Core;
using MailAppMAUI.Repositorios;
using MailAppMAUI.Contexto;
using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.Maui.Storage;
using Org.BouncyCastle.Asn1.X509;
using System.Security.Cryptography;
using MailKit.Net.Smtp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;
using System.Net.Security;
using Ifs.Comun;
using MailAppMAUI.General;
using Logger = Ifs.Comun.Logger;


namespace MailAppMAUI.UseCases.Services.ConcreteServices
{
    public class SendEmailService : IService
    {
        //RUTA DE GUARDAR ARCHIVOS
        static Configuration Conf { get; set; }

        //IMAP CONNECTION
        private ImapClient client;
        private bool isConnected = false;

        //CONSTRUCTORES
        private readonly IServiceScopeFactory scopeFactory;

        public SendEmailService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public SendEmailService() { }


        /// <summary>
        /// --
        /// </summary>
        public IService OpenService(string service)//Meter poca cosa porque si cada vez que se añade un servicio se llama aquí, estoy manteniendo la conexion IMAP desde que lo añado hasta que lo ejecuto.
        {
            if ((Conf = Configuration.Config) == null)
            {
                Conf = new Configuration();
            }

            return this;
        }
        /// <summary>
        /// Lee todos los mensajes nuevos y los procesa
        /// </summary>
        /// <param name="action">No sé</param>
        /// <param name="info">Informacion para realizar la ejecución del servicio (cantidad de mails a mostrar[0])</param>
        public async Task<object> Execute(object action, object[] info)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                // Crear un contexto scoped a mano
                var context = scope.ServiceProvider.GetRequiredService<Context>();

                // Crear manualmente una instancia de RepositoryManager usando ese contexto
                var repositoryManager = new RepositoryManager(context);

                var message = info[0] as MimeMessage; //Mensaje creado desde ReadPowerGestFileService

                //if (info.Length > 1) //Si llegan dos parámetros, es un correo enviado desde la app y requiere de emebeber imagenes, si no, es que viene desde powegest
                //{
                //    var builder = new BodyBuilder();

                //    string html;

                //    if (StrIsHtml(message.HtmlBody) || StrIsHtml(message.TextBody))
                //    {
                //        // Crear texto plano desde el codigo html 
                //        // El cuerpo debe ir con la version texto
                //        // Los correos lo muestran en los avisos

                //        if (message.HtmlBody != null)
                //        {
                //            builder.TextBody = HtmlToText(message.HtmlBody); //Convierto el htmlbody en textbody
                //            html = message.HtmlBody;

                //        }

                //        else if (message.TextBody != null)
                //        {
                //            builder.TextBody = HtmlToText(message.TextBody); //Convierto el htmlbody en textbody
                //            html = message.TextBody;
                //        }

                //        else
                //        {
                //            html = null;
                //        }

                //    }
                //    else
                //    {
                //        // El cuerpo es directamente texto plano
                //        builder.TextBody = HtmlToText(message.HtmlBody);

                //        html = null;

                //    }

                //    if (!StrEmpty(html))
                //    {
                //        // Comprobar carpeta base para imagenes y contenido
                //        // Si no se da se ha debido cargar de los adjuntos
                //        // Si sigue vacia cargarla de la ruta del logo
                //        //if (StrEmpty(BaseDir))
                //        //{
                //        //    if (!String.IsNullOrEmpty(mail.Logo))
                //        //        BaseDir = Path.GetDirectoryName(mail.Logo);
                //        //}

                //        // Definir las imagenes existentes en el cuerpo
                //        html = AjustImages(html);

                //        // Quitar codigo javascript si esta configurado
                //        html = RemoveScripts(html);

                //        // Ajustar elemtos de estilo Css requeridos
                //        html = AjustStyle(html);

                //        // Crear recursos de imagenes para el html
                //        if (Images != null)
                //        {
                //            foreach (ImageDef img in Images)
                //            {
                //                try
                //                {
                //                    // MediaTypeNames media = MediaTypeNames.Image.Gif;
                //                    var link = builder.LinkedResources.Add(img.Path);
                //                    link.ContentId = img.Name;
                //                    // link.ContentId = MimeUtils.GenerateMessageId();
                //                }
                //                catch (Exception exc)
                //                {
                //                    //Logger.LogError(exc);
                //                }
                //            }
                //        }

                //        // Set the html version of the message text
                //        builder.HtmlBody = html;
                //    }

                //    // Cargar el cuerpo del mensaje calculado
                //    message.Body = builder.ToMessageBody();
                //}
               
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    try
                    {
                        await client.ConnectAsync(Conf.User.SmtpConexion, Conf.User.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    }
                    catch (MailKit.Net.Smtp.SmtpCommandException ex)
                    {
                        Logger.LogLine("SendService", "Error al conectar", "");

                        throw new Exception(String.Format("Error tratando de conectar: {0} \n" +
                                                       "Codigo de estado {1}",
                                                        ex.Message, ex.StatusCode));
                    }
                    catch (MailKit.Net.Smtp.SmtpProtocolException ex)
                    {

                        Logger.LogLine("SendService", "Error en el protocolo", "");

                        throw new Exception(String.Format("Error estableciendo protocolo: {0}",
                                                        ex.Message));

                    }

                    if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.Authentication))
                    {
                        try
                        {
                            client.Authenticate(Conf.User.Email, Conf.User.Password);
                        }
                        catch (AuthenticationException ex)
                        {
                            Logger.LogLine("SendService", "Error al autentificar: claves no concuerda", "");

                            throw new Exception("Usuario o clave invalidas ");

                        }
                        catch (MailKit.Net.Smtp.SmtpCommandException ex)
                        {
                            Logger.LogLine("SendService", "Error al autentificar", "");

                            throw new Exception(String.Format("Error tratando de autentificar: {0} \n" +
                                                           "Codigo de estado {1}",
                                                            ex.Message, ex.StatusCode));
                        }
                        catch (MailKit.Net.Smtp.SmtpProtocolException ex)
                        {
                            Logger.LogLine("SendService", "Error del protocolo al autentificar", "");

                            throw new Exception(String.Format("Error de protocolo al autentificar: {0}",
                                                            ex.Message));
                        }
                    }

                    try
                    {
                        string s = "";
                        await client.SendAsync(message);
                    }
                    catch (MailKit.Net.Smtp.SmtpCommandException ex)
                    {
                        throw new Exception(String.Format("Error enviando correo: {0} \n" +
                                                       "Codigo de estado {1}",
                                                        ex.Message, ex.StatusCode));


                    }
                    catch (SmtpProtocolException ex)
                    {
                        throw new Exception(String.Format("Error general de protocolo enviando correo: {0}",
                                                        ex.Message));

                    }

                    await client.DisconnectAsync(true);
                }

                return true;
            }
        }

        #region EMBEBIDOS
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
                    text = RemoveString(text, index1 - 7, index2 + 9);
                    index1 = index2 = 0;
                }
                else
                    break;
            }

            return text;
        }
        public string HtmlToText(string html)
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

        /// <summary>
        /// Para saber si es un HTMl
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool StrIsHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            bool resul = ((text.IndexOf('<') >= 0) && (text.IndexOf(">") >= 0));

            return resul;

            // return ((text.IndexOf('<') >= 0) && (text.IndexOf(">") >= 0));
        }

        private string AjustImages(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            foreach (string tag in new string[] { "img", "amp-img" })
            {
                int index1 = 0;
                int index2 = 0;

                while (index1 >= 0)
                {
                    StringComparison comp = StringComparison.OrdinalIgnoreCase;

                    index1 = text.IndexOf("<" + tag, index1, comp);

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
                                    text = text.Substring(0, index1) + "<img" + text.Substring(index1 + tag.Length + 1);

                                    int index4 = text.IndexOf("</" + tag, index1, comp);

                                    if (index4 >= 0)
                                        text = text.Substring(0, index4) + "</img>" + text.Substring(index4 + tag.Length + 3);
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
        class ImageDef
        {
            public string Path;   // Path completo de la imagen
            public string Name;   // Identificador de la imagen
            public string Mime;   // Cadena con el tipo mime

            public ImageDef(string path, string name, string type)
            {
                Path = path;
                Name = name;
                Mime = type;
            }
        }

        List<ImageDef> Images;
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

                if (!string.IsNullOrEmpty(type) && type.Length > 2)
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

        private string FindImage(string image)
        {
            if (string.IsNullOrWhiteSpace(image))
                return null;

            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            // Si es URL, intenta descargar (No es necesario puesto que la app gestiona bien las imagenes con URL directamente)
            if (image.StartsWith("http:", comp) ||
                image.StartsWith("https:", comp) ||
                image.StartsWith("/", comp))
            {
                //try
                //{
                //    string downloadedPath = DownLoadFile(image);
                //    if (!string.IsNullOrEmpty(downloadedPath))
                //        return downloadedPath;
                //}
                //catch { }

                return image; // Devuelve la URL original si no se pudo descargar
            }

            // Si ya es ruta absoluta
            if (Path.IsPathRooted(image))
                return image;

            // Si es ruta relativa, combínala con BaseDir
            //if (!string.IsNullOrEmpty(BaseDir))
            //{
            string directPath = Path.Combine("C:\\Users\\programacion3\\Desktop\\", image); //Funciona bien si encuentra la imagen
            if (File.Exists(directPath))
                return Path.GetFullPath(directPath);

            //string fallback = Path.Combine(BaseDir, "Image", image);
            //if (File.Exists(fallback))
            //    return Path.GetFullPath(fallback);
            //}

            return null;
        }


        private string AjustStyle(string text)
        {
            if (StrEmpty(text))
                return text;

            int style1 = 0;
            int style2 = 0;

            int body = FindToken(text, "body", ref style1, ref style2, "<style", "</style>");

            if (body >= 0)
            {
                var tokens = new string[] { "visibility:" };

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
                            key2 -= offset;
                            style2 -= offset;

                            if (IsEmptyLine(text, key1, key2))
                            {
                                text = RemoveString(text, body, key2 + 1, out offset);
                                style2 -= offset;

                                if (FindToken(text, "{", ref style1, ref style2) == -1)
                                {
                                    text = RemoveString(text, style1 - 7, style2 + 8);
                                }
                            }
                        }
                    }
                }
            }

            return text;
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

        private string GetLine(string lines, ref int start, ref int stop, int index = 0)
        {
            int endlin = lines.IndexOf('\n', start, stop - start + 1);

            if (endlin == -1)
                endlin = lines.Length;

            string line = "";

            if (endlin >= 0)
            {
                while (start > 0 && lines[start - 1] != '\n')
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
                    index1 = ind1 + token1.Length;
                    count = index2 - index1 + 1;
                }
                else
                    return -1;
            }

            if (token2 != null)
            {
                ind2 = text.IndexOf(token2, index1, count, comp);

                if (ind2 >= index1)
                {
                    index2 = ind2 - 1;
                    count = index2 - index1 + 1;
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

        /// <summary>
        /// Seguramente llegue una ruta absoluta ()
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public string GetBaseDir(string imagePath)
        {
            return Path.GetDirectoryName(imagePath);
        }

        #endregion

        /// <summary>
        /// Cierra la conexión IMAP
        /// </summary>
        public bool CloseService()
        {
            return true;
        }
    }
}

