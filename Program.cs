using System.Runtime.InteropServices;

namespace CefDetector

{
    internal static class Program
    {
        [DllImport("Everything64.dll")]
        private static extern uint Everything_GetMajorVersion();
        [STAThread]
        static void Main()
        {
            try
            {
                _ = Everything_GetMajorVersion();
            }
            catch (Exception)
            {
                MessageBox.Show("Please install and run Everything first!", "Error:");
                return;
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                var wplayer = new WMPLib.WindowsMediaPlayer
                {
                    URL = "bgm.mp3"
                };
                wplayer.controls.play();
                wplayer.settings.setMode("loop", true);
                wplayer.settings.setMode("shuffle", false);
            } catch { }
            Application.Run(new Form1());
        }
    }
}