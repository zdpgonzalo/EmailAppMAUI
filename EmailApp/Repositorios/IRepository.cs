namespace MailAppMAUI.Repositorios
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Obtiene una entidad por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la entidad.</param>
        /// <returns>La entidad encontrada o null si no existe.</returns>
        public T? GetById(int id);

        /// <summary>
        /// Obtiene una lista de entidades del tipo especificado.
        /// </summary>
        /// <returns>
        /// Lista de entidades del contexto. 
        /// Si no hay entidades devuelve una lista vacia
        /// </returns>
        public List<T> GetAll();

        /// <summary>
        /// Agrega una nueva entidad al repositorio.
        /// </summary>
        /// <param name="entity">Entidad a agregar.</param>
        public Task<bool> AddAsync(T entity, bool save = true);

        /// <summary>
        /// Actualiza una entidad existente.
        /// </summary>
        /// <param name="entity">Entidad a actualizar.</param>
        public bool Update(T entity, bool updateUI = true);

        /// <summary>
        /// Elimina una entidad por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la entidad a eliminar.</param>
        public Task<bool> DeleteAsync(int? id, bool save = true);

        /// <summary>
        /// Guarda los datos del repositorio
        /// </summary>
        public void Save();

        /// <summary>
        /// Devuelve la cantidad de entidades almacenadas en el repositorio
        /// </summary>
        public int Count();
    }
}
