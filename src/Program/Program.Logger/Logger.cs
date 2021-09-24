using System;
using System.IO;
using System.Text;

namespace Program.LoggerLibrary
{
    public class Logger
    {
        private readonly LoggerSettings _settings;
        private readonly StreamWriter _streamWriter;

        public Logger(LoggerSettings settings, Stream inputStream)
        {
            _settings = settings;

            _streamWriter = new StreamWriter(inputStream)
            {
                AutoFlush = true
            };
        }

        public void Log(string message, SeverityLevel? severity = null)
        {
            StringBuilder sb = new StringBuilder();

            if (_settings.DisplayDate)
            {
                var dateNow = _settings.UseUTC ? DateTime.UtcNow : DateTime.Now;
                sb.Append($"[{dateNow}]");
            }
            if (_settings.DisplaySeverity && severity is not null)
            {
                sb.Append($"[{severity}]");
            }

            sb.Append($" {message}");
            _streamWriter.WriteLine(sb);
        }
    }

    public class LoggerSettings
    {
        public static LoggerSettings Default => new()
        {
            DisplayDate = true,
            UseUTC = false,
            DisplaySeverity = true
        };

        public bool DisplayDate { get; init; }
        public bool UseUTC { get; init; }
        public bool DisplaySeverity { get; init; }
    }

    public enum SeverityLevel
    {
        DEBUG,
        INFO,
        WARNING,
        FATAL
    }
}
