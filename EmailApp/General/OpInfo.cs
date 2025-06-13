namespace MailAppMAUI.General
{
    /// <summary> Definicion del codigo de mensajes
    /// Cada proyecto y clase numera sobre una base
    /// Cada enumerado tiene un codigo correlativo
    /// </summary>

    public enum OpMask
    {
        Type = 0xFFF0000,  // Codigo base para numerar cada clase
        Size = 16,         // Tamaño en bits del area de indices
        Next = 0x10000,    // Incremento de codigo entre cada tipo

        Base = 0xFFFF,     // Mascara base del codigo  en cada clase

        Enum = 12,         // Tamaño en bits indice de cada enumerado
        Index = 0xFFF,      // Mascara del indice de cada enumerado
    }

    public enum SubType
    {
        Event = 0x0000,     // Subtipo para mensajes de eventos
        Action = 0x1000,     // Subtipo para mensajes de operacion
        Count = 2,          // Numero de Subtipos definidos 
    }


    /// <summary> Clase soporte para definicion de atributos
    /// Permite asociar informacion extra a clases y enumeraciones
    /// </summary>
    public class Info : Attribute
    {
        #region DEFINICION DE LA CLASE

        public int Action;  // Codigo de operacion
        public string Title;   // Titulo asociado al elemento
        public object Params;  // Informacion adiccional 
        public string Area;    // Area origen del mensaje

        public object Name;    // Nombre original del mensaje
        public object[] Values;  // Valores por defecto

        /// <summary> Constructor especifico para definir codigos de evento
        /// Asigna codigo de accion y titulo a una enumeracion de eventos
        /// Puede añadirse una lista de parametros extra de cualquier tipo
        /// </summary>
        /// <param name="code">  Codigo de accion del evento </param>
        /// <param name="title"> Titulo principal del evento </param>
        /// <remarks>
        /// El area de un mensaje se puede indicar de dos formas:
        /// 
        /// - Con el prefijo: [nombre_area] al inicio del titulo
        ///   Este nombre se queita del titulo y se define como area
        ///   
        /// - Pasando por parametro el nombre del area del mensaje
        ///   Esto se hace pasando Area como 'named' en el atributo
        ///   Pero asi se detiene la depuracion ante cualquier cambio
        ///   Por tanto no es aconsejable pasar el area de esta forma
        /// </remarks>

        public Info(object code, string title, string info, string area)
        {
            Action = (int)code;

            if (!string.IsNullOrEmpty(info))
                Params = info;

            if (title != null && title.Length > 4)
            {
                if (title[0] == '[')
                {
                    int index = title.IndexOf(']');

                    if (index > 3)
                    {
                        string key = title[1..index];

                        if (key.IndexOf('{') < 0)
                        {
                            if (string.IsNullOrEmpty(area))
                                area = key.Trim();

                            title = title[(index + 1)..].TrimStart();
                        }
                    }
                }
            }

            Title = title;

            if (!string.IsNullOrEmpty(area))
                Area = area.ToString();
        }

        public Info(object code)
             : this(code, null, null, null)
        { }

        public Info(object code, string title)
             : this(code, title, null, null)
        { }

        // NOTA: Este metodo tenia el area como parametro 3
        // Por tanto los eventos con parametro con nombre estaban mal
        // Verificar si no hay usos pasando el area en paramatro 3

        public Info(object code, string title, string param)
            : this(code, title, param, null)
        { }

        /// <summary> Constructor general para definir cualquier elemento
        /// Asigna como minimo un titulo o descripcion del elemento
        /// Puede añadirse una lista de parametros extra de cualquier tipo
        /// </summary>
        /// <param name="code">  Codigo de accion del evento </param>
        /// <param name="title"> Titulo principal del evento </param>

        public Info(string title, params object[] info)
        {
            Title = title;
            if (info != null && info.Length > 0)
                Params = info;
        }

        public string ToString()
        {
            string text = string.Empty;

            // if (Action != OpAction.None)
            if (Action != 0)
                text = Action.ToString();

            if (Title != null)
            {
                if (text == null)
                    text = Title;
                else
                    text += ": " + Title;
            }

            return text;
        }


        #endregion

    }
}
