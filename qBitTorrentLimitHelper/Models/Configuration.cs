namespace qBitTorrentLimitHelper.Models
{
    internal class Configuration
    {
        public string Host { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public int LowLimitKilobytes { get; set; }
        public int MediumLimitKilobytes { get; set; }
        public int HighLimitKilobytes { get; set; }
        public int? DefaultLimitKilobytes { get; set; }

        public int LowLimitBytes => LowLimitKilobytes * KilobytesToBytesMultiplier;
        public int MediumLimitBytes => MediumLimitKilobytes * KilobytesToBytesMultiplier;
        public int HighLimitBytes => HighLimitKilobytes * KilobytesToBytesMultiplier;

        public int DefaultLimitBytes => DefaultLimitKilobytes * KilobytesToBytesMultiplier ?? MediumLimitBytes;

        private const int KilobytesToBytesMultiplier = 1024;
    }
}
