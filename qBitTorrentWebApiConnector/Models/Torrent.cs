using Newtonsoft.Json;

namespace qBitTorrentWebApiConnector.Models
{
    internal class Torrent
    {
        [JsonProperty("added_on")] public long AddedOn { get; set; }
        [JsonProperty("amount_left")] public long AmountLeft { get; set; }
        [JsonProperty("category")] public string Category { get; set; }
        [JsonProperty("completed")] public long Completed { get; set; }
        [JsonProperty("completion_on")] public long CompletedOn { get; set; }
        [JsonProperty("dl_limit")] public long DownloadLimit { get; set; }
        [JsonProperty("dlspeed")] public long DownloadSpeed { get; set; }
        [JsonProperty("downloaded")] public long Downloaded { get; set; }
        [JsonProperty("downloaded_session")] public long DownloadedSession { get; set; }
        [JsonProperty("eta")] public long Eta { get; set; }
        [JsonProperty("f_l_piece_prio")] public bool FirstLastPiecePriority { get; set; }
        [JsonProperty("force_start")] public bool ForceStart { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
        [JsonProperty("last_activity")] public long LastActivity { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("num_complete")] public long NumberComplete { get; set; }
        [JsonProperty("num_incomplete")] public long NumberIncomplete { get; set; }
        [JsonProperty("num_leechs")] public long NumberLeechs { get; set; }
        [JsonProperty("num_seeds")] public long NumberSeeds { get; set; }
        [JsonProperty("priority")] public long Priority { get; set; }
        [JsonProperty("progress")] public long Progress { get; set; }
        [JsonProperty("ratio")] public double Ratio { get; set; }
        [JsonProperty("ratio_limit")] public double RatioLimit { get; set; }
        [JsonProperty("seen_complete")] public long SeenComplete { get; set; }
        [JsonProperty("seq_dl")] public bool SequentialDownload { get; set; }
        [JsonProperty("size")] public long Size { get; set; }
        [JsonProperty("state")] public string State { get; set; }
        [JsonProperty("super_seeding")] public bool SuperSeeding { get; set; }
        [JsonProperty("total_size")] public long TotalSize { get; set; }
        [JsonProperty("tracker")] public string Tracker { get; set; }
        [JsonProperty("up_limit")] public long UploadLimit { get; set; }
        [JsonProperty("uploaded")] public long Uploaded { get; set; }
        [JsonProperty("uploaded_session")] public long UploadedSession { get; set; }
        [JsonProperty("upspeed")] public long UploadSpeed { get; set; }
    }
}
