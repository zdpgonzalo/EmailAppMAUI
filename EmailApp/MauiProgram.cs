
using MailAppMAUI.Contexto;
using MailAppMAUI.ContextProvider;
using MailAppMAUI.Repositorios;
using MailAppMAUI.UseCases;
using MailAppMAUI.UseCases.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;
using Microsoft.Maui.LifecycleEvents;
using MailAppMAUI.UseCases.Services.ConcreteServices;
using CommunityToolkit.Maui;
using EmailApp.UseCases.Services;
using Logger = Ifs.Comun.Logger;
using MailAppMAUI.General;
using Microsoft.Maui.Media;


namespace EmailApp
{
    //FORMA PARA HACER UN .EXE 100% WORKING
    //En .csproj meter 	<ItemGroup><PackageReference Include = "Microsoft.WindowsAppSDK" Version="1.5.240311000" /></ItemGroup>
    //cd D:\NET\ifswin\IGest --> EmailApp.exe > log.txt 2>&1 EJECUTAR ESTO PARA VER LOS ERRORES. Esto se crea en la carpeta log dentro de donde esté e ejecutable
    //dotnet publish -c Release -r win10-x64 -o D:\NET\ifswin\IGest --self-contained true /p:WindowsAppSDKSelfContained=true

    //FORMA 2
    //dotnet publish -f net8.0-windows10.0.19041.0 -c Release -r win10-x64 --self-contained false -p:WindowsPackageType=None -o "D:\NET\ifswin\IGest"


    //OTRA FORMA QUE NO SE SI FUNCIONA
    //dotnet publish D:\NET\EmailApp\EmailMAUIHugo\EmailApp\EmailApp\EmailApp.csproj -c Release -f net8.0-windows10.0.19041.0 -o D:\NET\ifswin\IGest PARA SUBIR EL PROYECTO A UNA CARPETA
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //try
            //{
            //    var rutaBase = "C:\\Users\\programacion3\\AppData\\Local";
            //    var logPath = Path.Combine(rutaBase, "StartupManual.txt");
            //    File.AppendAllText(logPath, $"App inició correctamente: {DateTime.Now}\n");
            //}
            //catch (Exception ex)
            //{
            //    var fallback = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log_fallback.txt");
            //    File.WriteAllText(fallback, ex.ToString());
            //}

            var builder = MauiApp.CreateBuilder();

            // Cargar configuración desde appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
            // Clave de licencia de Syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzczODA5OUAzMjM4MmUzMDJlMzBlaDNhU2RhLzBiWXlMRzJZTkMxV2Y1VE4ySFhYY2YweWpjTEdwc0JCUHhNPQ==");

            // Configurar DbContext con MySQL
            builder.Services.AddDbContext<Context>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 36)),
                    opts => opts.EnableRetryOnFailure())
                .LogTo(Console.WriteLine, LogLevel.Information));

            // REGISTROS DE DEPENDENCIAS

            builder.Services.AddScoped<RepositoryManager>();
            builder.Services.AddScoped<ServiceManager>();
            builder.Services.AddScoped<IGenerarRespuestas, GenerarRespuestas>();

            //SCOPED
            //builder.Services.AddSingleton<IGenerarRespuestas, GenerarRespuestas>();
            builder.Services.AddScoped<GesCorreos>();
            builder.Services.AddScoped<GesLogin>();
            builder.Services.AddScoped<GesInter>();

            builder.Services.AddScoped<ReadEmailService>();
            builder.Services.AddScoped<PlanOverService>();
            builder.Services.AddScoped<GenerateResponseService>();
            builder.Services.AddScoped<ReadPoweGestFileService>();
            builder.Services.AddScoped<SendEmailService>();

            builder.Services.AddScoped<IDbContextProvider, ContextProvider>();

            builder.Services.AddSingleton<SampleDataService>();
            builder.Services.AddScoped<SearchService>();
            builder.Services.AddScoped<ContactsActionService>();

            // Soporte para Blazor WebView y Syncfusion
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSyncfusionBlazor();

            Logger.LogLine("Startup", "App started", DateTime.Now.ToString());

            //using (var db = new Context())
            //{
            //    db.ResetDatabase();
            //}

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
{
    events.AddWindows(windows =>
    {
        windows.OnWindowCreated(nativeWindow =>
        {
            // nativeWindow es Microsoft.UI.Xaml.Window
            nativeWindow.Closed += (s, e) =>
            {
                Environment.Exit(0);
            };
        });
    });
});
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
