namespace MailAppMAUI.General
{
    public static class Paths
    {
        public static string ProjectDirectory { get => AppPath.GetFile(AppPath.GetDirExec(), @"..\..\..\..\..\"); }
        public static string AppLocalDirectory { get => $@"{Environment.SpecialFolder.LocalApplicationData}\001\"; }
        public static string ConfigDirectory { get => $@"{AppLocalDirectory}Config\"; }
        //public static string BaseDatosDirectory { get => $@"{ProjectDirectory}BaseDatos\"; }
        //public static string ContextoDirectory { get => $@"{BaseDatosDirectory}Contexto\"; }
        //public static string DTOsDirectory { get => $@"{BaseDatosDirectory}DTOs\"; }
        public static string DataBaseDirectory { get => $@"{AppLocalDirectory}BaseDatos\"; }
        //public static string MigrationsDirectory { get => $@"{BaseDatosDirectory}Migrations\"; }
        //public static string GeneralDirectory { get => $@"{ProjectDirectory}General\"; }
        public static string GestionDirectory { get => $@"{ProjectDirectory}Gestion\"; }
        //public static string CoreDirectory { get => $@"{GestionDirectory}Core\"; }
        //public static string GesCoreDirectory { get => $@"{GestionDirectory}GesCore\"; }
        public static string InterDirectory { get => $@"{AppLocalDirectory}Inter\"; }
        public static string SpecialDirectory { get => $@"{AppLocalDirectory}Esp\"; }
        //public static string StructsDirectory { get => $@"{InterDirectory}Structs\"; }
        public static string ExportDirectory { get => $@"{AppLocalDirectory}Export\"; }
        //public static string NewDataDirectory { get => $@"{InterDirectory}NewData\"; }
        public static string TransDirectory { get => $@"{InterDirectory}Trans\"; }
        public static string BackupDirectory { get => $@"{InterDirectory}Backup\"; }
        public static string OrdersDirectory { get => $@"{InterDirectory}Orders\"; }
        public static string LogsDirectory { get => $@"{AppLocalDirectory}Logs\"; }
        public static string PlatformsDirectory { get => $@"{ProjectDirectory}Platforms\"; }
        public static string AndroidDirectory { get => $@"{PlatformsDirectory}Android\"; }
        //public static string AndroidResourcesValuesDirectory { get => $@"{AndroidDirectory}Resources\values\"; }
        //public static string IOSDirectory { get => $@"{PlatformsDirectory}iOS\"; }
        //public static string MacCatalystDirectory { get => $@"{PlatformsDirectory}MacCatalyst\"; }
        //public static string TizenDirectory { get => $@"{PlatformsDirectory}Tizen\"; }
        //public static string WindowsDirectory { get => $@"{PlatformsDirectory}Windows\"; }
        public static string ResourcesDirectory { get => $@"{ProjectDirectory}Resources\"; }
        //public static string AppIconDirectory { get => $@"{ResourcesDirectory}AppIcon\"; }
        //public static string FontsDirectory { get => $@"{ResourcesDirectory}Fonts\"; }
        //public static string ImagesDirectory { get => $@"{ResourcesDirectory}Images\"; }
        //public static string RawDirectory { get => $@"{ResourcesDirectory}Raw\"; }
        //public static string SplashDirectory { get => $@"{ResourcesDirectory}Splash\"; }
        //public static string StylesDirectoy { get => $@"{ResourcesDirectory}Styles\"; }
        public static string ScreenImagesDirectory { get => $@"{AppLocalDirectory}ScreenImages\"; }
        public static string FamiliasDirectory { get => $@"{ScreenImagesDirectory}Familias\"; }
        public static string ProductosDirectory { get => $@"{ScreenImagesDirectory}Productos\"; }
        //public static string VistaDirectory { get => $@"{ProjectDirectory}Vista\"; }
        //public static string BehavioursDirectory { get => $@"{VistaDirectory}Behaviours\"; }
        //public static string DataConvertersDirectory { get => $@"{VistaDirectory}DataConverters\"; }
        //public static string UtilitiesDirectory { get => $@"{VistaDirectory}Utilities\"; }
        //public static string ViewModelsDirectory { get => $@"{VistaDirectory}ViewModels\"; }
        //public static string ViewsDirectory { get => $@"{VistaDirectory}Views\"; }
    }
}
