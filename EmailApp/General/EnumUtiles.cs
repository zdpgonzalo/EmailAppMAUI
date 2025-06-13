namespace MailAppMAUI.General
{
    public enum ActionType
    {
        None,
        Estandar,
        Data,
        Catalogo,
        Revision,
        Initial,
    }
    public enum Estacion
    {
        None,
        Central,
        Catalogo,
        Cliente,
        Tienda,
        Movil,
        Web,
    }
    /// <summary>
    /// Tipos de contactos que existen
    /// </summary>
    public enum TipoContacto
    {
        None,
        Formal,
        Informal,
        Desconocido
    }

    /// <summary>
    /// Servicios hábiles
    /// </summary>
    public enum RegisteredServices
    {
        None,
        ReadEmailService,
        GenerateResponseService,
        PlanOverService,
        ReadPoweGestFileService,
        SendEmailService
    }

    public enum ComType
    {
        None,
        Tcp,
        Folder,
        Manual,
        Ftp,
    }

    public enum PlanType
    {
        Gratuito,
        Plus,
        Pro
    }

    /// <summary>
    /// Roles de los usuarios en la app de fichaje
    /// </summary>
    public enum RolUsuario
    {
        None,
        Administrador,
        Moderador,
        Editor,
        Usuario,
        Invitado
    }

    /// <summary>
    /// Dias de la semana
    /// </summary>
    public enum DiaSemana
    {
        None,
        Lunes,
        Martes,
        Miercoles,
        Jueves,
        Viernes,
        Sabado,
        Domingo
    }


    /// <summary>
    /// Enumerado que define el tipo de DTO que ha sufrido un cambio.
    /// </summary>
    public enum TipoEntidad
    {
        None,
        Usuario,
        Contacto,
        Correo,
        Respuesta,
        Adjunto,
        Eliminado,
        Conversacion
    }

    /// <summary>
    /// Tipo de accesos de los usuarios a los campos/funciones definidas
    /// </summary>
    public enum TipoAcceso
    {
        None,
        NoAccesible,
        Consulta,
        Modificaciones,
        Alta,
        Total,
    }

    /// <summary>
    /// Enumerado que lista el nivel de cambio que una entidad ha sufrido.
    /// </summary>
    public enum NivelCambio
    {
        None,
        Alta = 10,
        Modificacion = 21,
        Enviado = 23,
        Baja = 35,
    }

    /// <summary> Resultado de una actualizacion
    /// </summary>
    /// <remarks>
    /// Los niveles de cambio indican el nivel de refresco necesario:
    /// <list type="bullet">
    /// <item>El nivel Range se reserva a cambios que invalidan todo.</item>
    /// <item>El nivel Docum debe usarse cuando se descartan rangos.</item>
    /// <item>El nivel Page refresca el contenido de una sola pagina. No se recargan los rangos limites de acceso a tablas.</item>
    /// <item>El nivel Line se aplica a nivel de una linea o grupo. Por tanto solo se asumen cambios en datos relacionados.</item>
    /// <item>El nivel Data solo afecta al campo usado directamente. Indica operacion completa y el refrresco es automatico.</item>
    /// <item>El nivel Valid indica validacion de una operacion. La operacion esta sin grabar en procesos de usuario.</item>
    /// <item>El nivel Cancel es anulacion completa de la operacion. No se ha realizado nigun cambio en los datos.</item>
    /// </list>
    /// </remarks>
    public enum OpResul
    {
        Cancel,    // No se ha realizado la operacion
        Valid,     // Operacion valida pendiente de salvar
        Data,      // Operacion con cambio simple del dato
        Line,      // Operacion con cambio de una linea 
        Page,      // Operacion con cambio de una pagina
        Docum,     // Operacion con cambio del documento
        Range      // Operacion con cambio de los rangos
    }

    /// <summary>
    /// Enumerado que lista todos los tipos de ventanas que se pueden desplegar, no todas de ellas de busqueda.
    /// </summary>
    public enum WindowType
    {
        None,
    }
}
