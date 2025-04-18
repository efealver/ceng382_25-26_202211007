using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;

namespace razorpages.Helpers
{
    public sealed class Utils
    {
        // Singleton instance
        private static readonly Lazy<Utils> _instance = new Lazy<Utils>(() => new Utils());

        // Private constructor to prevent external instantiation
        private Utils() { }

        // Public accessor for the singleton instance
        public static Utils Instance => _instance.Value;

        /// <summary>
        /// Converts any object or list of objects to a formatted JSON string.
        /// </summary>
        public string ExportToJson<T>(T data, bool indented = true)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = indented
            };

            return JsonSerializer.Serialize(data, options);
        }

        /// <summary>
        /// Converts the data to a byte array ready for download/export.
        /// </summary>
        public byte[] ExportToJsonBytes<T>(T data, bool indented = true)
        {
            string json = ExportToJson(data, indented);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
