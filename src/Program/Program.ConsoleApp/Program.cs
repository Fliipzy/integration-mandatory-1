using Microsoft.Extensions.Configuration;
using Program.LoggerLibrary;
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Program.ConsoleApp
{
    class Program
    {
        private static readonly IConfiguration _configuration;
        private static readonly Logger _logger;

        static Program()
        {
            Console.Title = "Program";

            _logger = new Logger(LoggerSettings.Default, Console.OpenStandardOutput());
            _logger.Log("Trying to load config file...", SeverityLevel.INFO);
           
            // Try to build configuration from appsettings.json
            try
            {
                _configuration = GetSettingsConfiguration();
                _logger.Log("Config file loaded successfully", SeverityLevel.INFO);
            }
            catch (Exception)
            {
                _logger.Log("Could not build settings configuration. Closing program!", SeverityLevel.FATAL);
                Environment.Exit(1);
            }
        }

        static void Main(string[] args)
        {
            var serverSettings = _configuration.GetSection("server1Settings");

            _logger.Log("Program ready to send CSV data to SERVER_1", SeverityLevel.INFO);
            _logger.Log("Press any key to continue...", SeverityLevel.INFO);

            Console.ReadKey(true);

            _logger.Log("Sending data to SERVER_1...", SeverityLevel.INFO);

            // Get server settings from configuration
            var serverAddress = serverSettings["address"];
            var serverPort = serverSettings.GetValue<int>("port");
            var serverEndpoint = serverSettings["endpoint"];

            // Setup and send HTTP request to server
            var request = HttpWebRequest.CreateHttp($"{serverAddress}:{serverPort}/{serverEndpoint}");
            request.Method = "POST";
            request.ContentType = "text/csv";
            

            
        }

        private static IConfiguration GetSettingsConfiguration()
        {
            //Yikes, should probably find a more gracious way to do this...
            var appSettingsPath = Path.Combine(Directory.GetParent(
                Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName,
                "appsettings.json");

            return new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath)
                .Build();
        }
    }
}
