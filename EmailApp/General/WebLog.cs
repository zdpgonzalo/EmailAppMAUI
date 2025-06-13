using MailAppMAUI.General;
using System.Text;

namespace MailAppMAUI.Gestion
{
    public static class WebLog
    {
        private static string BaseUrl = "https://infoser.net/AppService/AppService.php";

        /// <summary>
        /// Loggea el error en el dominio
        /// </summary>
        /// <param name="ex">Excepcion a loggear</param>
        /// <param name="info">Informacion extra a loggear</param>
        public static void LogError(Exception ex, string info = "")
        {
            GetAsync(ExceptionToString(ex), info);
        }

        /// <summary>
        /// Añade el Log al dominio PHP
        /// </summary>
        /// <param name="eventParam">Evento recibido</param>
        /// <param name="info">Informacion extra</param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string eventParam, string info)
        {
            using (HttpClient client = new HttpClient())
            {
                //url = $"{BaseUrl}?event={eventParam}&group={group}&device={device}&info={info}";
                string url = GetRequest(BaseUrl, [$"event={eventParam}", $"info={info}"]);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode(); // Lanza una excepción si el código de estado no es exitoso

                    string responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                catch (Exception ex)
                {
                    return "No se puede establecer conexión con el servicio web";
                }
            }
        }

        /// <summary>
        /// Concatena los parametros recibidos a la URL base para hacer una
        /// peticion php
        /// </summary>
        /// <param name="url">Url base</param>
        /// <param name="pars">Parametros a añadir, el formato debe ser "[clave]=[valor]"</param>
        /// <returns></returns>
        private static string GetRequest(string url, params string[] pars )
        {
            // if (!url.ToLower().StartsWith("http://"))
            if (url.IndexOf("//") < 0)
            {
                if (url.ToLower().StartsWith("ftp."))
                    url = "www." + url.Substring(4);

                url = "https://" + url;
            }

            string strpar = null;

            if (pars != null && pars.Length > 0)
            {
                foreach (object par in pars)
                {
                    if (strpar == null)
                        strpar = "?" + Data.ToString(par);
                    else
                        strpar += "&" + Data.ToString(par);
                }
            }

            if (!Str.Empty(strpar))
                url += strpar;

            return url;
        }

        /// <summary>
        /// Descompone la excepcion recibida y devuelve un string
        /// </summary>
        /// <param name="ex">Excepcion a descomponer</param>
        /// <returns>String del error de la excepcion</returns>
        public static string ExceptionToString(Exception ex)
        {
            string result = string.Empty;

            if (ex == null)
            {
                return result;
            }

            result = $"{DateTime.Now}: {ex?.GetType()?.FullName}\n" +
                        $"{ex?.Message}\n" +
                        $"{ex?.StackTrace}\n";

            return result;
        }

        // Método POST
        public static async Task<string> PostAsync(string eventParam, string group, int device, string info)
        {
            using (HttpClient client = new HttpClient())
            {
                // Crear el contenido con los parámetros
                string contentData = $"event={eventParam}&group={group}&device={device}&info={info}";
                StringContent content = new StringContent(contentData, Encoding.UTF8, "application/x-www-form-urlencoded");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(BaseUrl, content);
                    response.EnsureSuccessStatusCode(); // Lanza una excepción si el código de estado no es exitoso

                    string responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                catch (Exception ex)
                {
                    return "No se puede establecer conexión con el servicio web";
                }
            }
        }
    }
}
