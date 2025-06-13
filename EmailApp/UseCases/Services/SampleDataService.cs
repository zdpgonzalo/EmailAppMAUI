using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using MailAppMAUI.Config;  // si tu Configuration está en este namespace

namespace EmailApp.UseCases.Services
{
    public class SampleDataService
    {
        private readonly string _attachmentsDirectory;

        public SampleDataService()
        {
            // Base en la carpeta de datos de la app
            _attachmentsDirectory = Path.Combine(FileSystem.AppDataDirectory, "Adjuntos");

            // Asegúrate de que exista
            if (!Directory.Exists(_attachmentsDirectory))
                Directory.CreateDirectory(_attachmentsDirectory);

            // Opcional: también actualizar la configuración global
            if (Configuration.Config != null)
            {
                Configuration.Config.Paths.DirAdjuntos = _attachmentsDirectory;
            }
        }

        public async Task<(bool Success, string FileName, string Message)> SaveFilesAsync(
            IEnumerable<Stream> files,
            IEnumerable<string> fileNames)
        {
            try
            {
                var zipped = files.Zip(fileNames, (stream, name) => new { stream, name });
                string last = "";

                foreach (var item in zipped)
                {
                    var filePath = Path.Combine(_attachmentsDirectory, item.name);
                    last = item.name;

                    if (!File.Exists(filePath))
                    {
                        // Crea el archivo desde el stream
                        using var fs = File.OpenWrite(filePath);
                        await item.stream.CopyToAsync(fs);
                    }
                }

                return (true, last, "Archivos subidos correctamente.");
            }
            catch (Exception ex)
            {
                return (false, "", $"Error al subir archivo: {ex.Message}");
            }
        }

        public bool RemoveFile(string fileName, out string message)
        {
            try
            {
                var filePath = Path.Combine(_attachmentsDirectory, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    message = "Archivo eliminado correctamente";
                    return true;
                }
                message = "Archivo no encontrado";
                return false;
            }
            catch (Exception ex)
            {
                message = $"Error al eliminar archivo: {ex.Message}";
                return false;
            }
        }
    }
}
