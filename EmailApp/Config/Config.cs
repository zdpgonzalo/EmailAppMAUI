
using MailAppMAUI.General;
using MailAppMAUI.Core;

namespace MailAppMAUI.Config
{
    public class Configuration
    {
        #region Propiedades de configuracion predefinidas

        // Propiedades de configuracion predefinidas
        public WebConfig Web;  // Configuracion de acceso a los API Web

        public UserConfig User;  // Configuracion de las credenciales de usuario

        public DbConfig Db;   // Configuracion de acceso a base de datos

        public MainConfig App; // Configuracion de la aplicacion principal

        public PathConfig Paths; // Configuracion de acceso a ficheros

        #endregion

        #region Clases de configuracion predefinidas

        /// <summary> Definicion de acceso api Web
        /// </summary>

        public class WebConfig
        {
            public UserKeys Wp;     // Acceso a WordPress
            public UserKeys Wc;     // Acceso a WooCommerce
            public MainConfig App;    // Configuracion general
            public DbConfig Db;     // Configuracion datos
        }

        /// <summary> Definicion de acceso a bases de datos
        /// </summary>

        public class DbConfig
        {
            public string host;
            public string DataBase;
            public string user;
            public string pass;
            public int port;
        }

        /// <summary> Definiciones generales de la aplicaion
        /// </summary>

        public class MainConfig
        {
            public GeneralConfig General;   // Configuracion general
            public PathConfig Path;      // Configuracion carpetas
            public Docum Docum;     //Configuracion de facturas
            public Empresa Empresa;   //Configuracion de empresa
            public Iva Iva;       //Configuracion de empresa
            public Tesor Tesor;     //Configuracion de empresa
            public Agente Agente;    //Configuracion de agente
            public Almacen Almacen;   //Configuracion de almacen
            public TPago TPago;     //Configuracion de tipo de pago
            public Tarifa Tarifa;    //Configuracion de tarifas
            public Familia Familia;   //Configuracion de familias
            public Producto Producto;  //Configuracion de productos
            public Cliente Clien;   //Configuracion de clientes
        }

        public class UserConfig
        {
            public string Email;
            public string Password;
            public string Name;
            public string AccessToken;

            public string ImapConexion;
            public string SmtpConexion;

            public int SmtpPort;
            public int ImapPort;

            public int UserId;

        }

        public class GeneralConfig
        {
            public bool ModoTest;

            /// <summary>
            /// Longitud de la cuenta del banco
            /// </summary>
            public int LonCuenta;
        }

        /// <summary> Definicion de carpetas de la aplicacion
        /// </summary>

        public class PathConfig
        {
            public string DirApp;
            public string DirExport;
            //D:\NET\MailAppMAUI\EmailAppHugo\MailAppMAUI\wwwroot\Adjuntos
            //public string DirAdjuntos = Path.Combine("D:\\NET\\MailAppMAUI\\EmailAppHugo\\MailAppMAUI\\wwwroot\\", "Adjuntos"); /*CAMBIAR ESTO DE MOMENTO LOCAL*/
            public string DirWroot = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
            public string DirEmbebidos = AppBase.GetFullPath("CorreosPlantillas/images");//Path.Combine(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot"), "Adjuntos");
            public string DirAdjuntos = AppBase.GetFullPath("CorreosPlantillas/images");//Path.Combine(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot"), "Adjuntos");
            public string DirDatos;
            public string DirDatabase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BaseDatos") + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Configuracion de Tipo de pago
        /// </summary>
        public class TPago
        {
            /// <summary>
            /// Tipo de pago
            /// </summary>
            public string TipoPago;


            public int Longitud;
        }

        /// <summary>
        /// Configuracion de agente
        /// </summary>
        public class Agente
        {
            public int Longitud;
        }

        /// <summary>
        /// Configuracion de Tesor
        /// </summary>
        public class Tesor
        {
            public int Longitud;
        }

        /// <summary>
        /// Configuracion de almacenes
        /// </summary>
        public class Almacen
        {
            public int Longitud;
        }

        /// <summary>
        /// Configuracion de Tarifas
        /// </summary>
        public class Tarifa
        {
            public int Longitud;
        }

        /// <summary>
        /// Configuracion de Familias
        /// </summary>
        public class Familia
        {
            public int Longitud;
        }

        /// <summary>
        /// Configuracion de Productos
        /// </summary>
        public class Producto
        {
            public int Longitud;
        }

        /// <summary>
        /// Configuracion de cliente
        /// </summary>
        public class Cliente
        {
            public int Longitud;
        }

        /// <summary>
        /// Configuracion de Facturas
        /// </summary>
        public class Docum
        {
            /// <summary>
            /// Longitud de la serie
            /// </summary>
            public int LonSerie;

            /// <summary>
            /// Longitud 
            /// </summary>
            public int Longitud;

            /// <summary>
            /// Extension de la factura
            /// </summary>
            public string FaceExt;

            /// <summary>
            /// Codigo IBAN de la cuenta del banco
            /// </summary>
            public string Banco;

            /// <summary>
            /// URL del certificado
            /// </summary>
            public string FacCertFile;

            /// <summary>
            /// Contraseña del certificasdo
            /// </summary>
            public string FacCertPass;

            public bool FacCertExt;

            public bool FacCertSelect;

            public bool FacSignExtern;
        }

        public class Empresa
        {
            /// <summary>
            /// Cif de la empresa
            /// </summary>
            public string CIF;

            /// <summary>
            /// Codigo de la empresa
            /// </summary>
            public string Codigo;

            /// <summary>
            /// Nombre de la empresa
            /// </summary>
            public string Nombre;

            /// <summary>
            /// Direccion de la empresa
            /// </summary>
            public string Direccion;

            /// <summary>
            /// Provincia de la empresa
            /// </summary>
            public string Provincia;

            /// <summary>
            /// Poblacion de empresa
            /// </summary>
            public string Poblacion;

            /// <summary>
            /// Codigo postal de la empresa
            /// </summary>
            public string Postal;
        }

        public class Iva
        {
            public double IvaActual;
            public bool DescInc;
        }

        #endregion

        #region Clases de configuracion comun

        /// <summary> Configuracion de datos y credenciales de acceso
        /// </summary>

        public class UserKeys
        {
            public string User;
            public string Password;
            public string BaseUrl;
        }

        #endregion

        // Datos y tablas de configuracion 
        #region Incializacion de clases de congiguracion

        public Configuration()
        {
            ConfigValues = new Dictionary<string, object>();

            Initialize();

            Config = this;
        }

        internal string FileConfig;

        Dictionary<string, object> ConfigValues;

        public void Initialize()
        {
            Web = new WebConfig();
            Web.Wp = new UserKeys();
            Web.Wc = new UserKeys();

            User = new UserConfig();

            Paths = new PathConfig();

            App = new MainConfig();
            App.General = new GeneralConfig();
            App.Path = new PathConfig();

            Db = new DbConfig();
        }

        public static Configuration Config { get; private set; }

        #endregion
    }

    #region Enumerados para cargar la configuracion

    internal enum GroupKeys
    {
        None = 0,
        EmailConfig
    }

    internal enum ConfigKeys
    {
        None = 0,

        UserEmail,
        UserPassword,
        UserName,
        UserToken,
        UserImapConf,
        UserSmtpConf,
        UserSmtpPort,
        UserImapPort,
        UserUserId
    }

    #endregion
}
