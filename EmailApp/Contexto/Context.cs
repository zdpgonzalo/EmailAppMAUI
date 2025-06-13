using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Linq;
using MailAppMAUI.Core;
using MailAppMAUI.DTOs;
using MailAppMAUI.Gestion;
using MailAppMAUI.Config;

namespace MailAppMAUI.Contexto
{
    public class Context : DbContext
    {
        /// <summary>
        /// Objeto generico utilizado para sincronizar los posibles hilos en los que se divida la aplicacion. Es utilizado para la carga lazy del contexto.
        /// </summary>
        private static readonly object _syncLock = new();
        public static readonly object _methodLock = new();

        //public Context(DbContextOptions<Context> options): base(options)
        //{
        //}

        //WE CAN DO THIS ON MULTITENANCY
        //public class Context : DbContext
        //{
        //    private readonly string _connectionString;

        //    public Context(string connectionString)
        //    {
        //        _connectionString = connectionString;
        //    }

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        optionsBuilder.UseSqlServer(_connectionString);
        //    }
        //}

        private static Configuration config;
        static Configuration Conf
        {
            get
            {
                return config ??= new();
            }
            set
            {
                config = value;
            }
        }


        public void ResetDatabase()
        {
            try
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Tabla de Usuarios de la base de datos
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// Tabla de contactos almacenados en la base de datos
        /// </summary>
        public DbSet<Contacto> Contactos { get; set; }

        /// <summary>
        /// Tabla de correos almacenados en la base de datos
        /// </summary>
        public DbSet<Correo> Correos { get; set; }

        /// <summary>
        /// Tabla de respuestas generadas de los correos
        /// </summary>
        public DbSet<Respuesta> Respuestas { get; set; }

        /// <summary>
        /// Tabla de los ficheros adjuntos en los correos
        /// </summary>
        public DbSet<Adjunto> Adjuntos { get; set; }

        /// <summary>
        /// Tabla de los ficheros adjuntos en los correos
        /// </summary>
        public DbSet<Eliminado> Eliminados { get; set; }

        /// <summary>
        /// Tabla de los ficheros adjuntos en los correos
        /// </summary>
        public DbSet<Conversacion> Conversacion { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            lock (this)
            {
                if (!optionsBuilder.IsConfigured) // Evita configuraciones duplicadas
                {
                    // Replace with your connection string.

                    //-------------------DESCOMENTAR EN VERSION AVANZADA-------------------------

                    var connectionString = $"server=infoser.net;user=correoia;password=Tr_8745_Sd;database=CorreoBaseDatos";

                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

                    optionsBuilder.UseMySql(connectionString, serverVersion)
                            .LogTo(Console.WriteLine, LogLevel.Information)
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();

                    //var DataBaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EmailHugoBaseDatos");
                    //Directory.CreateDirectory(DataBaseDirectory);

                    //optionsBuilder.UseSqlite($"Data Source={Path.Combine(DataBaseDirectory, "EmailHugoBD.db")}");

                }
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            lock (_methodLock)
            {
                try
                {
                    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                }
                catch (Exception ex)
                {
                    WebLog.LogError(ex);
                    return null;
                }
            }
        }

        public override int SaveChanges()
        {
            lock (_methodLock)
            {
                try
                {
                    return base.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];

                            if (databaseValues != null)
                            {
                                var databaseValue = databaseValues[property];
                            }
                            else
                            {
                                proposedValues[property] = proposedValue;
                            }

                            // TODO: decide which value should be written to database
                            // proposedValues[property] = <value to be saved>;
                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(proposedValues);


                    }
                    return 0;
                }
                catch (DbUpdateException ex)
                {
                    var failedEntries = ex.Entries;
                    foreach (var entry in failedEntries)
                    {
                        var entityName = entry.Metadata.Name;
                        var properties = entry.Properties.Where(p => p.IsModified && !p.IsTemporary);
                        foreach (var property in properties)
                        {
                            var propertyName = property.Metadata.Name;
                            Console.WriteLine($"Failed to update field: {propertyName} in entity: {entityName}");
                        }
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    WebLog.LogError(ex);
                    return 0;
                }
            }
        }

        public void Destroy()
        {
            lock (_methodLock)
            {
                DestroyDatabase();
            }
        }

        public override object Find([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.Interfaces)] Type entityType, params object[] keyValues)
        {
            lock (_methodLock)
            {
                return base.Find(entityType, keyValues);
            }
        }

        /// <summary>
        /// Serializa la lista de Destinatarios de los objetos Correo y Respuesta
        /// en un JSON. De esta manera, guarda el JSON como texto plano.
        /// 
        /// Al guardar, guarda la lista en JSON
        /// Al cargar, guarda el JSON como una lista
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Converter para convertir List<string> a JSON y viceversa.
            var destinatariosConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            );

            // Configurar para la entidad Respuesta
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.Destinatarios)
                .HasConversion(destinatariosConverter)
                .HasColumnType("longtext"); // Ajusta el tipo según tu motor (en MySQL, por ejemplo)

            // Configurar para la entidad Correo
            modelBuilder.Entity<Correo>()
                .Property(c => c.Destinatarios)
                .HasConversion(destinatariosConverter)
                .HasColumnType("longtext");

            modelBuilder.Entity<Adjunto>()
                        .HasOne(a => a.Correo)
                        .WithMany(c => c.Adjuntos)
                        .HasForeignKey(a => a.CorreoId)
                        .OnDelete(DeleteBehavior.Cascade);
        }


        // Obtener la primera entidad
        public InputType GetFirstEntity<InputType>(
            Expression<Func<InputType, bool>>? where = null,
            Expression<Func<InputType, object>>? orderBy = null
        ) where InputType : BaseDTO
        {
            lock (_methodLock)
            {
                IQueryable<InputType> query = Set<InputType>();
                if (where != null) query = query.Where(where);
                if (orderBy != null) query = query.OrderBy(orderBy);
                return query.FirstOrDefault();
            }
        }

        // Obtener la última entidad
        public InputType GetLastEntity<InputType>(
            Expression<Func<InputType, bool>>? where = null,
            Expression<Func<InputType, object>>? orderBy = null
        ) where InputType : BaseDTO
        {
            lock (_methodLock)
            {
                IQueryable<InputType> query = Set<InputType>();
                if (where != null) query = query.Where(where);
                if (orderBy != null) query = query.OrderByDescending(orderBy);
                return query.FirstOrDefault();
            }
        }

        // Obtener propiedad de la primera entidad
        public PropType GetFirstEntityProp<InputType, PropType>(
            Expression<Func<InputType, bool>>? where,
            Expression<Func<InputType, object>>? orderBy,
            Expression<Func<InputType, PropType>> select
        ) where InputType : BaseDTO
        {
            lock (_methodLock)
            {
                IQueryable<InputType> query = Set<InputType>();
                if (where != null) query = query.Where(where);
                if (orderBy != null) query = query.OrderBy(orderBy);
                return query.Select(select).FirstOrDefault();
            }
        }

        // Obtener propiedad de la última entidad
        public PropType GetLastEntityProp<InputType, PropType>(
            Expression<Func<InputType, bool>>? where,
            Expression<Func<InputType, object>>? orderBy,
            Expression<Func<InputType, PropType>> select
        ) where InputType : BaseDTO
        {
            lock (_methodLock)
            {
                IQueryable<InputType> query = Set<InputType>();
                if (where != null) query = query.Where(where);
                if (orderBy != null) query = query.OrderByDescending(orderBy);
                return query.Select(select).FirstOrDefault();
            }
        }

        // Lista ordenada de entidades
        public List<InputType> GetEntityList<InputType>(
            Expression<Func<InputType, bool>>? where = null,
            Expression<Func<InputType, object>>? orderBy = null,
            bool ascending = true
        ) where InputType : BaseDTO
        {
            lock (_methodLock)
            {
                IQueryable<InputType> query = Set<InputType>();
                if (where != null) query = query.Where(where);
                if (orderBy != null)
                    query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
                return query.ToList();
            }
        }

        // Lista ordenada de propiedades
        public List<PropType> GetEntityPropList<InputType, PropType>(
            Expression<Func<InputType, bool>>? where,
            Expression<Func<InputType, object>>? orderBy,
            Expression<Func<InputType, PropType>> select,
            bool ascending
        ) where InputType : BaseDTO
        {
            lock (_methodLock)
            {
                IQueryable<InputType> query = Set<InputType>();
                if (where != null) query = query.Where(where);
                if (orderBy != null)
                    query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
                return query.Select(select).ToList();
            }
        }

        // Verificar existencia de entidades
        public bool Any<InputType>(Expression<Func<InputType, bool>>? where = null) where InputType : BaseDTO
        {
            lock (_methodLock)
            {
                return where == null
                    ? Set<InputType>().Any()
                    : Set<InputType>().Any(where);
            }
        }

        // Agregar entidades
        public void AddEntities<InputType>(params InputType[] entities)
        {
            lock (_methodLock)
            {
                AddRange(entities);
            }
        }

        // Actualizar entidades
        public void UpdateEntities<InputType>(params InputType[] entities)
        {
            lock (_methodLock)
            {
                UpdateRange(entities);
            }
        }

        // Eliminar entidades
        public void DeleteEntities<InputType>(params InputType[] entities)
        {
            lock (_methodLock)
            {
                RemoveRange(entities);
            }
        }

        public void DestroyDatabase()
        {
            Database.EnsureDeleted();
        }
    }
}