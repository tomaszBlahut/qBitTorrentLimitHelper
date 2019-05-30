using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using qBitTorrentLimitHelper.Models;
using qBitTorrentLimitHelper.Resources;
using qBitTorrentWebApiConnector;

namespace qBitTorrentLimitHelper
{
    internal class NotifyIconOnlyApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly IConfigurationProvider _configurationProvider = new ConfigurationProvider();

        private IWebApiConnector _webApiConnector;
        private Configuration _configuration;
        private Guid _lastMouseClickGuid;

        private const int BalloonTipTimeout = 1000;

        private bool _isLimitEnabled;
        public bool IsLimitEnabled
        {
            get => _isLimitEnabled;
            set
            {
                _isLimitEnabled = value;
                _notifyIcon.Icon = value ? Icons.Limit : Icons.NoLimit;
            }
        }

        [DllImport("user32.dll")]
        static extern uint GetDoubleClickTime();

        internal NotifyIconOnlyApplicationContext()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Warning,
                Visible = true
            };

            try
            {
                _configuration = _configurationProvider.ReadConfigurationFormFile().Result;
            }
            catch (Exception)
            {
                _configuration = _configurationProvider.GetDefaultConfiguration();
                _notifyIcon.ShowBalloonTip(BalloonTipTimeout, Constants.Error, Constants.UnableToLoadConfigurationFromFile, ToolTipIcon.Error);
            }

            _notifyIcon.MouseDown += ManageMouseClick;

            Initialize();
        }

        private void Initialize()
        {
            _notifyIcon.ContextMenuStrip = CreateContextMenuStrip();

            _webApiConnector = new WebApiConnector(new WebApiConfiguration
            {
                Host = _configuration.Host,
                Login = _configuration.Login,
                Password = _configuration.Password
            });

            if (_webApiConnector.CheckIfQbittorrentIsRunning())
            {
                var limit = _webApiConnector.GetCurrentGlobalLimit();

                IsLimitEnabled = limit != 0;
            }
            else
            {
                _notifyIcon.ShowBalloonTip(BalloonTipTimeout, Constants.Error, Constants.UnableToConnectToQBitTorrent, ToolTipIcon.Error);
            }
        }

        private ContextMenuStrip CreateContextMenuStrip()
        {
            var menuItems = new ToolStripItem[]
            {
                new ToolStripButton(GetMenuButtonText(Constants.Low, _configuration.LowLimitKilobytes), null, SetLowLimit),
                new ToolStripButton(GetMenuButtonText(Constants.Medium, _configuration.MediumLimitKilobytes), null, SetMediumLimit),
                new ToolStripButton(GetMenuButtonText(Constants.High, _configuration.HighLimitKilobytes), null, SetHighLimit),
                new ToolStripSeparator(), 
                new ToolStripButton(Constants.Reload, null, Reload),
                new ToolStripSeparator(),
                new ToolStripButton(Constants.Exit, null, Exit)
            };

            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(menuItems);

            return contextMenuStrip;
        }

        private void ManageMouseClick(object sender, MouseEventArgs eventArgs)
        {
            if (eventArgs.Button != MouseButtons.Left)
            {
                return;
            }

            if (eventArgs.Clicks == 1)
            {
                var mouseClickGuid = Guid.NewGuid();
                _lastMouseClickGuid = mouseClickGuid;

                Task.Run(async () =>
                {
                    await Task.Delay((int)GetDoubleClickTime());

                    if (mouseClickGuid == _lastMouseClickGuid)
                    {
                        SingleClickAction();
                    }
                });

                return;
            }

            _lastMouseClickGuid = Guid.NewGuid();

            DoubleClickAction();
        }

        private void SingleClickAction()
        {
            ToggleLimit();
        }

        private void DoubleClickAction()
        {
            SetAllDownloadingTorrentsToSequenceDownload();
            SetAllDownloadingTorrentsToPrioritizeFirstLastPiece();
        }

        private void ToggleLimit()
        {
            var limit = _webApiConnector.GetCurrentGlobalLimit();

            if (limit == 0)
            {
                SetLimit(_configuration.DefaultLimitBytes);
            }
            else
            {
                _webApiConnector.SetCurrentGlobalLimit(0);

                var currentLimit = _webApiConnector.GetCurrentGlobalLimit();

                if (currentLimit != 0)
                {
                    _notifyIcon.ShowBalloonTip(BalloonTipTimeout, Constants.Error, Constants.FailedToRemoveLimit,
                        ToolTipIcon.Error);
                }
                else
                {
                    IsLimitEnabled = false;
                }
            }
        }

        private void SetAllDownloadingTorrentsToPrioritizeFirstLastPiece()
        {
            var torrentHashes = _webApiConnector.GetDownloadingTorrentHashesWithDisabledSequentialDownload();
            _webApiConnector.ToggleSequentialDownload(torrentHashes);
        }

        private void SetAllDownloadingTorrentsToSequenceDownload()
        {
            var torrentHashes = _webApiConnector.GetDownloadingTorrentHashesWithDisabledFirstLastPartPriority();
            _webApiConnector.ToggleFirstLastPiecePriority(torrentHashes);
        }

        private void SetHighLimit(object sender, EventArgs eventArgs)
        {
            SetLimit(_configuration.HighLimitBytes);
        }

        private void SetMediumLimit(object sender, EventArgs eventArgs)
        {
            SetLimit(_configuration.MediumLimitBytes);
        }

        private void SetLowLimit(object sender, EventArgs eventArgs)
        {
            SetLimit(_configuration.LowLimitBytes);
        }

        private void Reload(object sender, EventArgs eventArgs)
        {
            try
            {
                _configuration = _configurationProvider.ReadConfigurationFormFile().Result;
                Initialize();
            }
            catch (Exception)
            {
                _notifyIcon.ShowBalloonTip(BalloonTipTimeout, Constants.Error, Constants.UnableToLoadConfigurationFromFile, ToolTipIcon.Error);
            }
        }

        private void Exit(object sender, EventArgs eventArgs)
        {
            _notifyIcon.Visible = false;

            Application.Exit();
        }

        private string GetMenuButtonText(string title, int limit)
        {
            return $"{title} ({limit} kb/s)";
        }

        private void SetLimit(int limit)
        {
            _webApiConnector.SetCurrentGlobalLimit(limit);

            var currentLimit = _webApiConnector.GetCurrentGlobalLimit();

            if (currentLimit != _configuration.DefaultLimitBytes)
            {
                _notifyIcon.ShowBalloonTip(BalloonTipTimeout, Constants.Error, Constants.FailedToSetLimit, ToolTipIcon.Error);
            }
            else
            {
                IsLimitEnabled = true;
            }
        }
    }
}
