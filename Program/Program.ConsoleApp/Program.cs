using Microsoft.Extensions.Configuration;
using Program.LoggerLibrary;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        static async Task Main(string[] args)
        {
            var serverSettings = _configuration.GetSection("server1Settings");

            _logger.Log("Program ready to send CSV data to SERVER_1", SeverityLevel.INFO);
            _logger.Log("Press any key to continue...", SeverityLevel.INFO);

            Console.ReadKey(true);

            _logger.Log("Sending data to SERVER_1...", SeverityLevel.INFO);

            var csvText = File.ReadAllText("Data/WeatherData.csv");

            // Get server settings from configuration
            var serverAddress = serverSettings["address"];
            var serverPort = serverSettings.GetValue<int>("port");
            var serverEndpoint = serverSettings["endpoint"];

            using (var client = new HttpClient())
            {
                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsync("http://" + serverAddress + ":" + serverPort + "/" + serverEndpoint, 
                        new StringContent(csvText, Encoding.UTF8));
                }
                catch (Exception)
                {
                    _logger.Log($"The program could not connect to the server", SeverityLevel.FATAL);
                    return;
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Log($"The program received the unsuccessful status code ({response.StatusCode}) back!", SeverityLevel.FATAL);
                    return;
                }

                 _logger.Log("\n\n" + await response.Content.ReadAsStringAsync(), SeverityLevel.INFO);
            
            }

            Console.ReadKey();
        }

        private static IConfiguration GetSettingsConfiguration()
        {
            var appSettingsPath = Path.Combine(Directory.GetParent(
                Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName,
                "appsettings.json");

            return new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath)
                .Build();
        }
    }
}
