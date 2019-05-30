using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using qBitTorrentWebApiConnector.Models;

namespace qBitTorrentWebApiConnector
{
    public interface IWebApiConnector
    {
        bool CheckIfQbittorrentIsRunning();

        int GetCurrentGlobalLimit();
        void SetCurrentGlobalLimit(int limit);

        IEnumerable<string> GetDownloadingTorrentHashesWithDisabledSequentialDownload();
        IEnumerable<string> GetDownloadingTorrentHashesWithDisabledFirstLastPartPriority();
        void ToggleSequentialDownload(IEnumerable<string> hashes);
        void ToggleFirstLastPiecePriority(IEnumerable<string> hashes);
    }

    public class WebApiConnector : IWebApiConnector
    {
        private readonly HttpClient _httpClient;
        private readonly WebApiConfiguration _configuration;

        public WebApiConnector(WebApiConfiguration configuration = null)
        {
            _configuration = configuration ?? new WebApiConfiguration
            {
                Host = "localhost:8080",
                Login = "admin",
                Password = "admin"
            };

            _httpClient = new HttpClient();
        }

        public bool CheckIfQbittorrentIsRunning()
        {
            var response = _httpClient.GetAsync($"{GetApiUrl()}version/api").Result;

            return response.IsSuccessStatusCode;
        }

        public int GetCurrentGlobalLimit()
        {
            var response = _httpClient.PostAsync($"{GetApiUrl()}command/getGlobalDlLimit",
                new ByteArrayContent(new byte[0])).Result;

            var limitString = response.Content.ReadAsStringAsync().Result;
            var limit = int.Parse(limitString ?? "0");

            return limit;
        }

        public void SetCurrentGlobalLimit(int limit)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{GetApiUrl()}command/setGlobalDlLimit")
            {
                Content = new StringContent($"limit={limit}", Encoding.ASCII, "application/x-www-form-urlencoded")
            };

            _httpClient.SendAsync(request).Wait();
        }

        public IEnumerable<string> GetDownloadingTorrentHashesWithDisabledSequentialDownload()
        {
            var torrents = GetDownloadingTorrents();

            var torrentsWithDisabledSequentialDownload = torrents.Where(x => !x.SequentialDownload);
            var torrentHashes = torrentsWithDisabledSequentialDownload.Select(x => x.Hash);

            return torrentHashes;
        }

        public IEnumerable<string> GetDownloadingTorrentHashesWithDisabledFirstLastPartPriority()
        {
            var torrents = GetDownloadingTorrents();

            var torrentsWithDisabledFirstLastPartPriority = torrents.Where(x => !x.FirstLastPiecePriority);
            var torrentHashes = torrentsWithDisabledFirstLastPartPriority.Select(x => x.Hash);

            return torrentHashes;
        }

        public void ToggleSequentialDownload(IEnumerable<string> hashes)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{GetApiUrl()}command/toggleSequentialDownload")
            {
                Content = new StringContent($"hashes={string.Join("|", hashes)}", Encoding.ASCII, "application/x-www-form-urlencoded")
            };

            _httpClient.SendAsync(request).Wait();
        }

        public void ToggleFirstLastPiecePriority(IEnumerable<string> hashes)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{GetApiUrl()}command/toggleFirstLastPiecePrio ")
            {
                Content = new StringContent($"hashes={string.Join("|", hashes)}", Encoding.ASCII, "application/x-www-form-urlencoded")
            };

            _httpClient.SendAsync(request).Wait();
        }

        private string GetApiUrl()
        {
            return $"http://{_configuration.Host}/";
        }

        private IEnumerable<Torrent> GetDownloadingTorrents()
        {
            var response = _httpClient.GetStringAsync($"{GetApiUrl()}query/torrents?filter=downloading").Result;

            var torrents = JsonConvert.DeserializeObject<List<Torrent>>(response);
            return torrents;
        }
    }
}
