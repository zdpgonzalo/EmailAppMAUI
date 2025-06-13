using MailAppMAUI.Contexto;
using MailAppMAUI.Core;
using Microsoft.EntityFrameworkCore;

namespace MailAppMAUI.Repositorios
{
    public class RepositoryManager
    {
        private readonly Context context;

        private ICorreoRepository correoRepository;
        private IContactoRepository contactoRepository;
        private IRepository<Respuesta> respuestaRepository;
        private IRepository<Usuario> usuarioRepository;
        private IRepository<Adjunto> adjuntoRepository;
        private IRepository<Eliminado> eliminadoRepository;
        private IRepository<Conversacion> conversacionRepository;

        private readonly IDbContextFactory<Context> _contextFactory;

        private readonly Context _context;

        public RepositoryManager(Context context)
        {
            _context = context;
        }

        public ICorreoRepository CorreoRepository => correoRepository ??= new CorreoRepository(_context);
        public IContactoRepository ContactoRepository => contactoRepository ??= new ContactoRepository(_context);
        public IRepository<Respuesta> RespuestaRepository => respuestaRepository ??= new RespuestaRepository(_context);
        public IRepository<Usuario> UsuarioRepository => usuarioRepository ??= new UsuarioRepository(_context);
        public IRepository<Adjunto> AdjuntoRepository => adjuntoRepository ??= new AdjuntoRepository(_context);
        public IRepository<Eliminado> EliminadoRepository => eliminadoRepository ??= new EliminadoRepository(_context);
        public IRepository<Conversacion> ConversacionRepository => conversacionRepository ??= new ConversacionRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
        }

        ///// <summary>
        ///// Repositorio de correos
        ///// </summary>
        //public ICorreoRepository CorreoRepository { get =>  correoRepository; }

        //    /// <summary>
        //    /// Repositorio de contactos
        //    /// </summary>
        //    public IContactoRepository ContactoRepository { get =>  contactoRepository; }

        //    /// <summary>
        //    /// Repositorio de respuestas
        //    /// </summary>
        //    public IRepository<Respuesta> RespuestaRespository { get =>  respuestaRepository; }

        //    /// <summary>
        //    /// Repositorio de usuarios
        //    /// </summary>
        //    public IRepository<Usuario> UsuarioRepository { get =>  usuarioRepository; }

        //    /// <summary>
        //    /// Respositorio de archivos adjuntos
        //    /// </summary>
        //    public IRepository<Adjunto> AdjuntoRespository { get => adjuntoRepository; }

        //    /// <summary>
        //    /// Respositorio de eliminados
        //    /// </summary>
        //    public IRepository<Eliminado> EliminadoRepository { get => eliminadoRepository; }
        //    public IRepository<Conversacion> ConversacionRepository { get => conversacionRepository; }
    }
}
