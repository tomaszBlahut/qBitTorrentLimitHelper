using System;
using System.Windows.Forms;

namespace qBitTorrentLimitHelper
{
    internal static class QBitTorrentLimitHelper
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new NotifyIconOnlyApplicationContext());
        }
    }
}
