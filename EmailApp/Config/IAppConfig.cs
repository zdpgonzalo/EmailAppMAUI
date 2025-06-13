namespace MailAppMAUI.Config
{
    /// <summary> Interfaz para acceso a configuracion general
    /// </summary>

    internal interface IAppConfig
    {
        T GetConfig<T>(string name);
        bool SetConfig<T>(string name, T value);
    }
}
