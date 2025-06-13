using Logger = Ifs.Comun.Logger;

namespace EmailApp
{
    public partial class App : Application
    {
        public App()
        {
            try
            {

                //var rutaBase = "C:\\Users\\programacion3\\AppData\\Local";
                //var logPath = Path.Combine(rutaBase, "StartupManual.txt");
                //File.AppendAllText(logPath, $"App inició correctamente desde APPxaml: {DateTime.Now}\n");

                InitializeComponent();

                MainPage = new MainPage();
            }
            catch (Exception ex)
            {
                var fallback = Path.Combine("C:\\Users\\programacion3\\AppData\\Local", "log_fallback.txt");
                File.WriteAllText(fallback, ex.ToString());

                Logger.LogError(ex);
                throw;
            }
        }
    }
}
