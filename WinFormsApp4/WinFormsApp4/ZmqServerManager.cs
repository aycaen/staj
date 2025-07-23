using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp4
{
    internal class ZmqServerManager
    {
        public static Process? StartZmqServer(string exeName)
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exeName);
            if (!File.Exists(exePath))
            {
                MessageBox.Show($"Sunucu bulunamadı:\n{exePath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                return Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sunucu başlatılamadı:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
