using System;

namespace EmailApp.UseCases.Services
{
    public class SearchService
    {
        // Define un evento que se dispara cuando se ejecuta una búsqueda.
        public static event Action<string>? OnSearch;

        // Método para "disparar" la búsqueda y notificar a los suscriptores.
        public void TriggerSearch(string query)
        {
            OnSearch?.Invoke(query);
        }
    }

}
