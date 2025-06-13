
// -------------------------------------------------------------
// Versiones base de Powergest para codigos
//
// PowerGest_300   Version beta de gestion    3.00 (Diciembre 2014)
// PowerGest_301   Version inicial de gestion 3.01 (Abril 2015)
//
// -------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace MailAppMAUI.General
{
    #region RANGOS STANDARD DE IDENTIFICADORES POWERGEST

    // Definciones de rangos de codigos powergest

    public enum InterRange
    {
        // Tipos basicos de mensajes
        Line         =  0x0100000,          // Identificador de campo de lineas

        Name         =  0x2000000,          // Identificador de titulo de campo
        Title        =  0x3000000,          // Identificador de titulo en lineas
        Label        =  0x4000000,          // Identificador de etiqueta del campo
        Image        =  0x6000000,          // Identificador de imagen de un campo

        Action       =  0x8000000,          // Identificador de codigo de operacion
        Info         =  0xA000000,          // Peticion de informacion del sistema

        Event        =  0xC000000,          // Identificador de mensaje de evento
        Query        =  0xD000000,          // Identificador de mensaje con respuesta
        Trace        =  0xE000000,          // Identificador de mensaje de registro
        Error        =  0xF000000,          // Identificador de eventos de error

        // Mascaras del codigo de mensajes
        CodeFull     =  0xFFFFFFF,          // Mascara del identificador completo
        CodeType     =  0xFF00000,          // Mascara del tipo de identificador
        CodeIden     =   0x0FFFFF,          // Mascara del identificador sin tipo
        CodeBase     =   0x0FFFF0,          // Mascara del codigo standard de identificador
        CodeScope    =   0x0F0000,          // Mascara del ambito del identificador
        CodeRange    =   0x0FF000,          // Mascara de rango de identificadores
        CodeIndex    =   0x000FFF,          // Mascara de indice dentro de un rango
        CodeSubRange =   0x00F000,          // Mascara de subrango de identificadores
        CodeOrder    =        0xF,          // Mascara del indice del identificador

        // Definicion del ambito para cualquier mensaje
        Standard     =    0x10000,          // Identificador base de campos standard
        System       =    0x60000,          // Identificador base campos del sistema
        Global       =    0x70000,          // Identificador base campos globales
        Local        =    0x80000,          // Identificador base campos locales
        Autom        =    0x90000,          // Identificador base campos automaticos
        Support      =    0xA0000,          // Identificador base local en clases base
        Group        =    0xB0000,          // Identificador base para definir grupos
        Extern       =    0xC0000,          // Identificador base de clases externas
        Application  =    0xD0000,          // Identificador base de aplicacion principal

        // Definicion de tipos standard combinados
        MsSystem     =    Event + System,   // Identificador base de eventos del sistema
        MsGlobal     =    Event + Global,   // Identificador base de eventos globales
        MsLocal      =    Event + Local,    // Identificador base de eventos locales

        TrGlobal     =    Trace + Global,   // Identificador de evento de registro global
        ErGlobal     =    Error + Global,   // Identificador de evento de error global

        OpSystem     =    Action + System,  // Identificador base de ordenes del sistema
        OpLocal      =    Action + Local,   // Identificador base de ordenes locales

        // Rangos para operaciones standard
        BaseInt      =    Action + System + 0x100,   // Operaciones internas
        BaseMove     =    Action + System + 0x200,   // Operaciones de navegacion
        BaseSel      =    Action + System + 0x300,   // Operaciones de seleccion
        BaseSys      =    Action + System + 0x400,   // Operaciones base del sistema
        BaseVal      =    Action + System + 0x500,   // Retorno de resultados
        BaseGen      =    Action + System + 0x600,   // Operaciones genericas
    }

    public enum InterName
    {
        // Tipos especiales de identificadores
        Line         = InterRange.Line,     // Identificador de campo de lineas
        Name         = InterRange.Name,     // Identificador de titulo de campo
        Title        = InterRange.Title,    // Identificador de titulo en lineas
        Label        = InterRange.Label,    // Identificador de etiqueta del campo
        Image        = InterRange.Image,    // Identificador de imagen de un campo

        // Identificadores standard
        Factura      = InterRange.Standard + 0x010,
        Fecha        = InterRange.Standard + 0x020,
        Cliente      = InterRange.Standard + 0x030,
        Proveedor    = InterRange.Standard + 0x040,
        Agente       = InterRange.Standard + 0x050,
        Almacen      = InterRange.Standard + 0x060,
        Articulo     = InterRange.Standard + 0x070,
        Familia      = InterRange.Standard + 0x080,
        Agencia      = InterRange.Standard + 0x090,
        Comerc       = InterRange.Standard + 0x100,
        Instal       = InterRange.Standard + 0x110,
        Empleado     = InterRange.Standard + 0x120,
                                                  
        Banco        = InterRange.Standard + 0x130,
        Tercer       = InterRange.Standard + 0x140,
        Caja         = InterRange.Standard + 0x150,
        CajaBanco    = InterRange.Standard + 0x160,  
        Tarjeta      = InterRange.Standard + 0x170,
        Tesoreria    = InterRange.Standard + 0x180,
        NumBanco     = InterRange.Standard + 0x190,
        NumCaja      = InterRange.Standard + 0x1A0,
                                                  
        Moneda       = InterRange.Standard + 0x200,
        Codsec       = InterRange.Standard + 0x210,
        Divisa       = InterRange.Standard + 0x220,
                                                  
        Tpago        = InterRange.Standard + 0x230,
        TipCob       = InterRange.Standard + 0x240,
        TipRem       = InterRange.Standard + 0x250,
        TipDoc       = InterRange.Standard + 0x260,
        Recibo       = InterRange.Standard + 0x270,
        Vencim       = InterRange.Standard + 0x280,
        Cpago        = InterRange.Standard + 0x290,
                                                  
        Tipo         = InterRange.Standard + 0x2A0,
        Destino      = InterRange.Standard + 0x300,
        Serie        = InterRange.Standard + 0x310,
        Lote         = InterRange.Standard + 0x320,
        Postal       = InterRange.Standard + 0x330,
        Oferta       = InterRange.Standard + 0x340,
        Tarifa       = InterRange.Standard + 0x350,
        TCompra      = InterRange.Standard + 0x360,

        Ruta         = InterRange.Standard + 0x370,
        Refer        = InterRange.Standard + 0x380,
        Caduc        = InterRange.Standard + 0x390,
                                                  
        Garantia     = InterRange.Standard + 0x400,
        Gremio       = InterRange.Standard + 0x410,
        Docref       = InterRange.Standard + 0x420,
        Doccli       = InterRange.Standard + 0x430,
                                                  
        Tnivel       = InterRange.Standard + 0x440,
        Nivel1       = InterRange.Standard + 0x450,
        Nivel2       = InterRange.Standard + 0x460,
        Clave1       = InterRange.Standard + 0x470,
        Clave2       = InterRange.Standard + 0x480,

        Existen      = InterRange.Standard + 0x490,
        Coper        = InterRange.Standard + 0x4A0,
        Tmovim       = InterRange.Standard + 0x4B0,
        Tdocum       = InterRange.Standard + 0x500,
        Clvtar       = InterRange.Standard + 0x510,
        Unidades     = InterRange.Standard + 0x520,
        Longitud     = InterRange.Standard + 0x530,
                                                  
        Docum        = InterRange.Standard + 0x540,
        Rclave       = InterRange.Standard + 0x550,
                                                  
        DocPed       = InterRange.Standard + 0x560,
                                                  
        Activo       = InterRange.Standard + 0x600,
        Aviso        = InterRange.Standard + 0x610,
        Descrip      = InterRange.Standard + 0x620,
        Unidad       = InterRange.Standard + 0x630,
        Periodo      = InterRange.Standard + 0x640,
        Marca        = InterRange.Standard + 0x650,
        Faviso       = InterRange.Standard + 0x660,
                                                  
        GUser        = InterRange.Standard + 0x710,
        Sucursal     = InterRange.Standard + 0x720,
        Tienda       = InterRange.Standard + 0x730,
        Empresa      = InterRange.Standard + 0x740,
                                                  
        Clave        = InterRange.Standard + 0x800,
        Cuenta       = InterRange.Standard + 0x810,
        Subcta       = InterRange.Standard + 0x820,
        Ventas       = InterRange.Standard + 0x830,
        Compras      = InterRange.Standard + 0x840,

        Costes       = InterRange.Standard + 0x870,
        Presup       = InterRange.Standard + 0x880,
        Punteo       = InterRange.Standard + 0x890,

        FDocum       = InterRange.Standard + 0x4010,   

        Horas        = InterRange.Standard + 0x4020,
        Extras       = InterRange.Standard + 0x4030,

        AlmTipo      = InterRange.Standard + 0x4040,   
        Estado       = InterRange.Standard + 0x4050,   

        Concepto     = InterRange.Standard + 0x4510,
        Precio       = InterRange.Standard + 0x4520,
        Total        = InterRange.Standard + 0x4530,
        Comision     = InterRange.Standard + 0x4540,
        Gastos       = InterRange.Standard + 0x4550,

        CantPed      = InterRange.Standard + 0x4560,
        Cantidad     = InterRange.Standard + 0x4570,
        CantMov      = InterRange.Standard + 0x4580,
        Descuento    = InterRange.Standard + 0x4590,

        Barras       = InterRange.Standard + 0x4600,
        Pedido       = InterRange.Standard + 0x4610,  // Importe pedido de un articulo
        Pagado       = InterRange.Standard + 0x4620,  // Importe pagado de un documento
        Aplazado     = InterRange.Standard + 0x4630,  // Importe aplazado de un documento
        Efectivo     = InterRange.Standard + 0x4640,  // Importe pagado en efectivo
        Contado      = InterRange.Standard + 0x4650,  // Importe pagado al contado
        ImpCambio    = InterRange.Standard + 0x4660,  // Importe de cambio devuelto
        ImpCaja      = InterRange.Standard + 0x4670,  // Importe pagado en efectivo o contado
        IvaTipo      = InterRange.Standard + 0x4680,  // Tipo numerico de Iva
        IvaInfo      = InterRange.Standard + 0x4690,  // Tipo de Iva formateado
        TotalBase    = InterRange.Standard + 0x46A0,  // Tipo de Iva formateado
        
        Peso         = InterRange.Standard + 0x46C0,  // Total peso de un documento
        Volumen      = InterRange.Standard + 0x46D0,  // Total volumen de un documento
        Entero       = InterRange.Standard + 0x46E0,  

        TotalPed     = InterRange.Standard + 0x4710,  // Total cantidad pedida
        TotalCan     = InterRange.Standard + 0x4720,  // Total cantidad servida

        Texto        = InterRange.Standard + 0x4740,  // Texto libre o base de textos
 
        Document     = InterRange.Standard + 0x8000,

        // Identificadores de elementos del sistema
        Filter       = InterRange.System + 0x1,      // Definicion generica de Filtros
        Range        = InterRange.System + 0x2,      // Definicion generica de Rangos
        Sort         = InterRange.System + 0x3,      // Definicion generica de ordenacion
        Find         = InterRange.System + 0x4,      // Deficion generica de busqueda
        Select       = InterRange.System + 0x5,      // Identificador generico de seleccion
        Format       = InterRange.System + 0x6,      // Identificador generico de formato
        Mark         = InterRange.System + 0x7,      // Identificador genrico de marca
        Count        = InterRange.System + 0x8,      // Identificador genrico de cuenta
        Table        = InterRange.System + 0x9,      // Identificador generico de tabla
        Progress     = InterRange.System + 0xA,      // Identificador generico de progreso
        Status       = InterRange.System + 0xB,      // Identificador generico de status
    }                 
                      
    public enum InterAction
    {
        // Ordenes de las operaciones internas
        Loading      = InterRange.BaseInt + 0x1,     // Inicializacion de la clase soporte
        Starting     = InterRange.BaseInt + 0x2,     // Arranque de la clase soporte
        Closing      = InterRange.BaseInt + 0x3,     // Confirmar cierre en la clase soporte
        SetGroup     = InterRange.BaseInt + 0x4,     // Cambia el area por defecto
        Result       = InterRange.BaseInt + 0x5,     // Obtiene resultado final de un proceso
        SetText      = InterRange.BaseInt + 0x6,     // Define reglas de traduccion de mensajes
        GetText      = InterRange.BaseInt + 0x7,     // Obtiene la traduccion de un mensaje
        GetSupport   = InterRange.BaseInt + 0x8,     // Obtiene la traduccion de un mensaje
        ValData      = InterRange.BaseInt + 0x9,     // Validacion de salida de un campo
                                                     
        GetConfig    = InterRange.BaseInt + 0xA,     // Consulta una variable de configuracion
        SetConfig    = InterRange.BaseInt + 0xB,     // Actualiza una variable de configuracion
        EvalExp      = InterRange.BaseInt + 0xC,     // Evalua una expresion generica
        ExecApp      = InterRange.BaseInt + 0xD,     // Ejecuta opcion de alto nivel
                                                     
        // Operaciones del sistema para tablas       
        AddLine      = InterRange.BaseInt + 0x10,    // A�adir una linea al final
        InsLine      = InterRange.BaseInt + 0x11,    // A�adir una linea intermedia
        DelLine      = InterRange.BaseInt + 0x12,    // Borrar la linea actual
        CmpLine      = InterRange.BaseInt + 0x13,    // Validar linea para grabar
        InsEnter     = InterRange.BaseInt + 0x14,    // Entrada en modo insercion
        InsCancel    = InterRange.BaseInt + 0x15,    // Salida de editar columna
        ChangeLine   = InterRange.BaseInt + 0x16,    // Indica cambio de linea
        ChangeCol    = InterRange.BaseInt + 0x17,    // Indica cambio de columna
        GetNext      = InterRange.BaseInt + 0x18,    // Busqueda de siguiente campo
        ColNext      = InterRange.BaseInt + 0x1A,    // Mover a la columa siguiente
        ColPrev      = InterRange.BaseInt + 0x1B,    // Mover a la columa previa
        FirstLine    = InterRange.BaseInt + 0x20,    // Mover a la columa previa
        LastLine     = InterRange.BaseInt + 0x21,    // Mover a la columa previa
        PrevLine     = InterRange.BaseInt + 0x22,    // Mover a la columa previa
        NextLine     = InterRange.BaseInt + 0x23,    // Mover a la columa previa
                                                     
        // Ordenes del sistema de navegacion         
        Next         = InterRange.BaseMove + 0x1,    // Registro principal siguiente
        Prev         = InterRange.BaseMove + 0x2,    // Registro principal previo
        MoveNext     = InterRange.BaseMove + 0x3,    // Registro secundario siguiente
        MovePrev     = InterRange.BaseMove + 0x4,    // Registro secundario previo
        PageNext     = InterRange.BaseMove + 0x5,    // Mover a la pagina siguiente
        PagePrev     = InterRange.BaseMove + 0x6,    // Mover a la pagina previa
        GoTop        = InterRange.BaseMove + 0x7,    // Mover al registro inicial
        GoEnd        = InterRange.BaseMove + 0x8,    // Mover al registro final
                                                     
        // Operaciones del sistema para seleccion
        SelData      = InterRange.BaseSel + 0x1,     // Seleccion de datos multiples
        SelOption    = InterRange.BaseSel + 0x2,     // Seleccion de datos de opcion
        SelDocum     = InterRange.BaseSel + 0x3,     // Seleccion de documento o registro
                                                     
        SelColumn    = InterRange.BaseSel + 0x4,     // Seleccion simple de columna
        SelOrder     = InterRange.BaseSel + 0x5,     // Seleccion de orden de columna
        SelRange     = InterRange.BaseSel + 0x6,     // Seleccionar un rango
                                                     
        // Ordenes de operaciones base de sistema    
        Accept       = InterRange.BaseSys + 0x1,     // Operacion del sistema aceptar
        Cancel       = InterRange.BaseSys + 0x2,     // Operacion del sistema cancelar
        Close        = InterRange.BaseSys + 0x3,     // Operacion del sistema cerrar
        Init         = InterRange.BaseSys + 0x4,     // Operacion del sistema inicializar
        Exit         = InterRange.BaseSys + 0x5,     // Orden de cierre de la aplicacion
        Find         = InterRange.BaseSys + 0x10,    // Operacion standard de busqueda
        Select       = InterRange.BaseSys + 0x11,    // Operacion de busqueda de valor

        Op_SetValue  = InterRange.BaseSys + 0x20,    // Cambio de valorores en un campo

        // Ordenes de envio de resultados
        FindVal      = InterRange.BaseVal + 0x1,     // Operacion resultado de busqueda
                                                       
        // Ordenes de operaciones genericas    
        Append       = InterRange.BaseGen + 0x1,     // A�adir un nuevo registro
        Modify       = InterRange.BaseGen + 0x2,     // A�adir un nuevo registro
        Insert       = InterRange.BaseGen + 0x3,     // Insertar un registro
        Delete       = InterRange.BaseGen + 0x4,     // Borrar un registro
        Apply        = InterRange.BaseGen + 0x5,     // Orden de aplicar cambios
        Valid        = InterRange.BaseGen + 0x6,     // Operacion generica de aceptar
                                                     
        Load         = InterRange.BaseGen + 0x10,    // Cargar datos del registro
        Save         = InterRange.BaseGen + 0x11,    // Salvar datos en el registro

        Import       = InterRange.BaseGen + 0x14,    // Importar datos en el registro
        Export       = InterRange.BaseGen + 0x15,    // Exportar datos del registro

        Create       = InterRange.BaseGen + 0x20,    // Orden de inicio de alta
        Edit         = InterRange.BaseGen + 0x21,    // Orden de inicio de ediccion
        Print        = InterRange.BaseGen + 0x22,    // Orden de impresion
        Read         = InterRange.BaseGen + 0x23,    // Orden generica de lectura
        Update       = InterRange.BaseGen + 0x24,    // Operacion generica de recarga
        Config       = InterRange.BaseGen + 0x25,    // Operacion generica de configuracion
        Back         = InterRange.BaseGen + 0x26,    // Operacion generica de retorno
        Blank        = InterRange.BaseGen + 0x27,    // Operacion generica de anular valor
                                                           
        Copy         = InterRange.BaseGen + 0x30,    // Operacion standard de copia
        Paste        = InterRange.BaseGen + 0x31,    // Operacion standard de copia
        EvalReg      = InterRange.BaseGen + 0x32,    // Evaluacion general sobre un registro

        Filter       = InterRange.BaseGen + 0x40,    // Orden de aplicar filtros
        Format       = InterRange.BaseGen + 0x41,    // Orden de aplicar formato
        Sort         = InterRange.BaseGen + 0x42,    // Aplicar ordenacion
        Mark         = InterRange.BaseGen + 0x43,    // Seleccionar registro
        UnMark       = InterRange.BaseGen + 0x44,    // Desseleccionar registro
        Range        = InterRange.BaseGen + 0x45,    // Operacion generica de rangos
    }
    
    public enum InterInfo
    {
        // Codigos del sistema de informacion de soporte
        Ident        = InterRange.Info + 0x1,        // Consulta la existencia de un Identificador
        Fields       = InterRange.Info + 0x2,        // Consulta de campos o titulos de un control
        Keys         = InterRange.Info + 0x3,        // Consulta de claves de opciones de una lista
        Images       = InterRange.Info + 0x4,        // Consulta de imagenes asociadas a una lista
        Format       = InterRange.Info + 0x6,        // Consulta formato de un campo
        DbField      = InterRange.Info + 0x7,        // Nombre del campo origen en la tabla 
                                              
        KeyTop       = InterRange.Info + 0x10,       // Claves de busqueda inicial de una tabla
        KeyEnd       = InterRange.Info + 0x11,       // Claves de busqueda final de una tabl
        Filter       = InterRange.Info + 0x12,       // Expresion de filtro de una tabla
        Order        = InterRange.Info + 0x13,       // Expresion de ordenacion del fichero
        Table        = InterRange.Info + 0x14,       // Codigo standard de la tabla
        Index        = InterRange.Info + 0x15,       // Numero de fila actual de una tabla
        Start        = InterRange.Info + 0x16,       // Identificador de columna inicial
        Stop         = InterRange.Info + 0x17,       // Identificador de columna final
        Range        = InterRange.Info + 0x1F,       // Busca identificador asociado a una tabla
                                             
        Enable       = InterRange.Info + 0x20,       // Lista de controles a activar
        Disable      = InterRange.Info + 0x21,       // Lista de controles a desactivar
        Hide         = InterRange.Info + 0x22,       // Lista de controles a ocultar
        Show         = InterRange.Info + 0x23,       // Lista de controles a mostrar
                                                     
        Find         = InterRange.Info + 0x30,       // Consulta del campo a buscar
        Valid        = InterRange.Info + 0x31,       // Codigo de validacion y normalizacion
        Search       = InterRange.Info + 0x32,       // Codigo de validacion solo en busqueda
        Seek         = InterRange.Info + 0x33,       // Expresion base de busqueda 

        Edit         = InterRange.Info + 0x40,       // Consulta permiso para editar
        Active       = InterRange.Info + 0x41,       // Consulta del campo activo
                                                  
        Title        = InterRange.Info + 0x50,       // Titulo principal del proceso

        Single       = InterRange.Info  + 0x60,      // Aplicacion de interfaz SDI

        Color        = InterRange.Info + 0x100,      // Codigos especificos de color
    }                                              

    public enum InterEvent
    {
        // Mensajes genericos de informacion
        MenInfo        = InterRange.MsGlobal + 0x1,  // Mensaje de informacion sin espera
        MenTitle       = InterRange.MsGlobal + 0x2,  // Mensaje de titulo sin espera
                                                  
        MenResult      = InterRange.MsGlobal + 0x3,  // Mensaje de informacion con espera
        MenValid       = InterRange.MsGlobal + 0x4,  // Mensaje de aceptacion o rechazo
        MenData        = InterRange.MsGlobal + 0x5,  // Mensaje de entrada simple de datos
        MenInput       = InterRange.MsGlobal + 0x6,  // Mensaje de entrada multiple de campos
        MenRanges      = InterRange.MsGlobal + 0x7,  // Mensaje de entrada standard de rangos
        MenSelect      = InterRange.MsGlobal + 0x8,  // Mensaje seleccion entre varias opciones
                       
        TotInfo        = InterRange.MsGlobal + 0x10, // Mensaje de totales sin espera
        TotResult      = InterRange.MsGlobal + 0x11, // Mensaje de totales con espera
        TotValid       = InterRange.MsGlobal + 0x12, // Mensaje de totales con validacion
                                                   
        UserAccess     = InterRange.MsGlobal + 0x1A, // Mensaje de nivel de acceso de usuario
        UserRead       = InterRange.MsGlobal + 0x1B, // Mensaje de acceso solo consulta
        UserDenied     = InterRange.MsGlobal + 0x1C, // Mensaje de acceso denegado al usuario
        UserDocum      = InterRange.MsGlobal + 0x1D, // Mensaje de acceso denegado a documento
                                                   
        UserLock       = InterRange.MsGlobal + 0x1E, // Mensaje de documento bloqueado

        AppDemo        = InterRange.MsGlobal + 0x1F, // Mensaje de aplicacion en modo demo
                                                   
        // Mensajes de registro y de error         
        LogInfo        = InterRange.TrGlobal + 0x20, // Mensaje de registro sin espera
        ErrInfo        = InterRange.ErGlobal + 0x21, // Mensaje de error sin espera
                                                   
        LogResult      = InterRange.TrGlobal + 0x23, // Mensaje de aviso con espera
        ErrResult      = InterRange.ErGlobal + 0x24, // Mensaje de error con espera
                                                   
        // Indicaciones de estado de proceso       
        DocInit        = InterRange.MsGlobal + 0x31, // Informacion de inicio de un proceso
        DocNext        = InterRange.MsGlobal + 0x32, // Informacion de documento en proceso
        DocEnd         = InterRange.MsGlobal + 0x34, // Informacion de proceso finalizado
           
        Ms_DocInfo     = InterRange.MsGlobal + 0x35, // Mensaje de informacion de estado
        Ms_DocTitle    = InterRange.MsGlobal + 0x36, // Mensaje de titulo de estado
        
        DocResult      = InterRange.MsGlobal + 0x37, // Informacion de resultado general
        DocValid       = InterRange.MsGlobal + 0x38, // Informacion de resultado valido
        DocRange       = InterRange.MsGlobal + 0x39, // Informacion de resultado nulo por chequeo
        DocCheck       = InterRange.MsGlobal + 0x3A, // Informacion de resultado nulo por filtros
        DocError       = InterRange.MsGlobal + 0x3B, // Informacion de errores de un proceso
                                                   
        // Mensajes de gestion de ventanas          
        AppFind        = InterRange.MsGlobal + 0x41, // Ventana general de busqueda de codigos
        AppValid       = InterRange.MsGlobal + 0x42, // Validacion y busqueda si se requere
        AppSelect      = InterRange.MsGlobal + 0x43, // Ventana de seleccion y retorno de valor
        AppExit        = InterRange.MsGlobal + 0x44, // Ventana de salida de la aplicacion
        AppClose       = InterRange.MsGlobal + 0x45, // Cierre de la ventana actual
        AppExec        = InterRange.MsGlobal + 0x46, // Inicia un proceso de la aplicacion
        AppReload      = InterRange.MsGlobal + 0x47, // Recarga completa de la empresa
        AppUser        = InterRange.MsGlobal + 0x48, // Ventana de salida de la aplicacion
        AppRegen       = InterRange.MsGlobal + 0x49, // Ventana de regeneracion forzada
                                                            
        // Mensajes de dialogos standard           
        SelectFont     = InterRange.MsGlobal + 0x51, // Dialogo de seleccionar Font
        SelectColor    = InterRange.MsGlobal + 0x52, // Dialogo de seleccionar Color
        SelectPrint    = InterRange.MsGlobal + 0x53, // Dialogo de seleccionar Impresora
        ConfigPrint    = InterRange.MsGlobal + 0x54, // Dialogo de configurar Impresora
        SelectComputer = InterRange.MsGlobal + 0x55, // Dialogo de configurar Impresora
        SelectDir      = InterRange.MsGlobal + 0x56, // Dialogo de seleccionar carpeta
                                                    
        OpenFile       = InterRange.MsGlobal + 0x60, // Dialogo de abrir fichero
        OpenImage      = InterRange.MsGlobal + 0x61, // Dialogo de abrir ficheros de imagen
        OpenData       = InterRange.MsGlobal + 0x62, // Dialogo de abrir ficheros de datos
        OpenBackup     = InterRange.MsGlobal + 0x63, // Dialogo de abrir ficheros de copia zip
        OpenText       = InterRange.MsGlobal + 0x64, // Dialogo de abrir ficheros de texto
                                                    
        SaveFile       = InterRange.MsGlobal + 0x70, // Dialogo de salvar fichero
        SaveImage      = InterRange.MsGlobal + 0x71, // Dialogo de salvar imagen
        SaveData       = InterRange.MsGlobal + 0x72, // Dialogo de salvar datos
        SaveBackup     = InterRange.MsGlobal + 0x73, // Dialogo de salvar fichero de copia
        SaveText       = InterRange.MsGlobal + 0x74, // Dialogo de salvar ficheros de texto
                                                    
        // Mensajes de operacion sobre controles   
        ColNext        = InterRange.MsGlobal + 0x81, // Mover a la columna siguiente
        ColPrev        = InterRange.MsGlobal + 0x82, // Mover a la columna anterior

        Enable         = InterRange.MsGlobal + 0x84, // Evento para activacion de control
        Disable        = InterRange.MsGlobal + 0x85, // Evento para desactivacion de control
        Activate       = InterRange.MsGlobal + 0x86, // Evento para dar foco a un control
        SetText        = InterRange.MsGlobal + 0x87, // Evento para poner texto del control
        ValEdit        = InterRange.MsGlobal + 0x88, // Evento para editar valor o etiqueta
                                             
        // Mensajes de operaciones del sistema      
        Action         = InterRange.MsGlobal + 0x91, // Evento generico de enviar operacion
        Update         = InterRange.MsGlobal + 0x92, // Evento generico de actualizacion
        Reload         = InterRange.MsGlobal + 0x93, // Evento generico de recarga de datos
    } 

    public enum InterModules
    {
         Docum         = 0x1000,    // Rango base de facturacion
         Pagos         = 0x2000,    // Rango base de gestion de cobros
         Conta         = 0x3000,    // Rango base de contabilidad
         Cuentas       = 0x4000,    // Rango base de cuentas
         Tercer        = 0x5000,    // Rango base de intertiendas
         Trasp         = 0x6000,    // Rango base de intertiendas
         Alma          = 0x7000,    // Rango base de almacen
         Comun         = 0xF000,    // Rango base de opeciones comunes
    }
                                              
    public enum InterChange                          
    {                                                
        // Nivel de cambios de una actualizacion     
        Cancel      = 0,     // Modificacion cancelada
        Valid       = 1,     // Modificacion valida sin realizar
        Data        = 2,     // Modificacion a nivel campo
        Line        = 3,     // Modificacion a nivel linea
        Page        = 4,     // Modificacion a nivel pagina/tabla
        Docum       = 5,     // Modificacion a nivel general/documento
        Range       = 6,     // Modificacion general y cambio de rangos
    }

    public enum InterTable
    {
       None      = 0,
       Clien     = 3,
       Proveedor = 4,
       Agente    = 5,
       Cuenta    = 7,
       Terceros  = 8,
       Almacen   = 17,
       Artic     = 18,
       Familia   = 24,
       Docum     = 36,
       Lineas    = 37,
       Vencim    = 40,
       Caja      = 41,
       Tiendas   = 88,
       Imagen    = 121,
    }

    public enum InterDocum
    {
        None,
        MovimVenta             = 1000,
        VentaInform            = 1100,
        VentaOferta            = 1200,
        VentaOfertaDetalle     = 1210,
        VentaOrden             = 1300,
        VentaOrdenOferta       = 1302,
        VentaOrdenPedido       = 1304,
        VentaOrdenServida      = 1305,
        VentaOrdenEspecial     = 1309,
        VentaOrdenSalida       = 1310,
        VentaOrdenEntrada      = 1320,
        VentaServidaSalida     = 1315,
        VentaServidaEntrada    = 1325,
        VentaPedido            = 1400,
        VentaPedidoOferta      = 1401,
        VentaPedidoEspecial    = 1409,
        VentaPedidoDetalle     = 1410,
        VentaAlbaran           = 1500,
        VentaAlbaranDirecto    = 1501,
        VentaAlbaranOferta     = 1502,
        VentaAlbaranOrden      = 1503,
        VentaAlbaranPedido     = 1504,
        VentaAlbaranFacturado  = 1505,
        VentaAlbaranDiferido   = 1506,
        VentaFacturadoPedido   = 1507,
        VentaDiferidoPedido    = 1508,
        VentaAlbaranEspecial   = 1509,
        VentaAlbaranSalida     = 1510,
        VentaAlbaranEntrada    = 1520,
        VentaFactura           = 1600,
        VentaFacturaDirecta    = 1601,
        VentaFacturaOferta     = 1602,
        VentaFacturaOrden      = 1603,
        VentaFacturaPedido     = 1604,
        VentaFacturaComision   = 1607,
        VentaFacturaEspecial   = 1609,
        VentaFacturaAlbaran    = 1605,
        VentaFacturaDiferida   = 1606,
        VentaFacturaSalida     = 1610,
        VentaFacturaEntrada    = 1620,
        VentaAbono             = 1700,
        VentaFacturaAbono      = 1702,
        VentaRecibo            = 1800,
                             
        MovimCompra            = 2000,
        CompraInform           = 2100,
        CompraOferta           = 2200,
        CompraPedido           = 2400,
        CompraAlbaran          = 2500,
        CompraAlbaranDirecto   = 2501,
        CompraAlbaranPedido    = 2504,
        CompraAlbaranFacturado = 2505,
        CompraAlbaranDiferido  = 2506,
        CompraFacturadoPedido  = 2507,
        CompraDiferidoPedido   = 2508,
        CompraFactura          = 2600,
        CompraFacturaDirecta   = 2601,
        CompraFacturaPedido    = 2604,
        CompraFacturaAlbaran   = 2605,
        CompraFacturaDiferida  = 2606,
        CompraAbono            = 2700,
        CompraFacturaAbono     = 2700,
        CompraRecibo           = 2800,
        MovimAlmacen           = 3000,
        MovimAlmacenSalida     = 3010,
        MovimAlmacenEntrada    = 3020,
        MovimAlmacenPendiente  = 3005,
                             
        MovimMultiple          = 3100,
        MovimMultipleSalida    = 3110,
        MovimMultipleEntrada   = 3120,
        MovimProduccion        = 3200,
        MovimProduccionSalida  = 3210,
        MovimProduccionEntrada = 3220,
        MovimReserva           = 3500,
        MovimReservaSalida     = 3510,
        MovimReservaEntrada    = 3520,
        MovimReservaPedido     = 3505,
        MovimInventario        = 3800,
    }

    public enum InterPago
    {
        Contado  = 1,
        Aplazado = 2,
    }

    public enum InterRegimen
    {
        Normal    = 1,
        Recargo   = 2,
        Inversion = 3,
        Exento    = 4,
        Export    = 5,
        ExpCom    = 6,
        ImpCom    = 7,
        Import    = 8,
        Aduana    = 9
    }

    #endregion
}













