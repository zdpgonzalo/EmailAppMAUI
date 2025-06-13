using System.Collections.Generic;

namespace MailAppMAUI.Helpers
{
    public static class MimeTypeHelper
    {
        private static readonly Dictionary<string, string> _mappings = new(StringComparer.InvariantCultureIgnoreCase)
        {
            // Imágenes
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".bmp", "image/bmp" },

            // PDF y documentos
            { ".pdf", "application/pdf" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".ppt", "application/vnd.ms-powerpoint" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },

            // Vídeo
            { ".mp4", "video/mp4" },
            { ".webm", "video/webm" },
            { ".ogg", "video/ogg" },

            // Audio
            { ".mp3", "audio/mpeg" },
            { ".wav", "audio/wav" },
            { ".oga", "audio/ogg" },

            // Archivos comprimidos
            { ".zip", "application/zip" },
            { ".rar", "application/x-rar-compressed" },
            { ".7z", "application/x-7z-compressed" },

            // Otros comunes
            { ".txt", "text/plain" },
            { ".csv", "text/csv" }
        };

        public static bool TryGetContentType(string fileName, out string contentType)
        {
            var ext = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(ext) && _mappings.TryGetValue(ext, out contentType))
            {
                return true;
            }

            contentType = "application/octet-stream";
            return false;
        }
    }
}
