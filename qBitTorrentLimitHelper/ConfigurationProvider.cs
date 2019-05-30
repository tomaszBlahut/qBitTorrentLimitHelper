using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using qBitTorrentLimitHelper.Models;
using qBitTorrentLimitHelper.Resources;

namespace qBitTorrentLimitHelper
{
    internal interface IConfigurationProvider
    {
        Task<Configuration> ReadConfigurationFormFile();
        Configuration GetDefaultConfiguration();
    }

    internal class ConfigurationProvider : IConfigurationProvider
    {
        public async Task<Configuration> ReadConfigurationFormFile()
        {
            using (var reader = new StreamReader(new FileStream(Constants.ConfigurationFileName, FileMode.Open)))
            {
                var jsonContent = await reader.ReadToEndAsync();
                var configuration = JsonConvert.DeserializeObject<Configuration>(jsonContent);

                return configuration;
            }
        }

        public Configuration GetDefaultConfiguration()
        {
            return new Configuration
            {
                Host = "localhost:8080",
                Login = "admin",
                Password = "admin",

                LowLimitKilobytes = 800,
                MediumLimitKilobytes = 500,
                HighLimitKilobytes = 200,
                DefaultLimitKilobytes = 600
            };
        }
    }
}