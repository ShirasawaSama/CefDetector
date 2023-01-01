using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CefDetector
{
    public partial class Form1 : Form
    {
        private const int EVERYTHING_REQUEST_FILE_NAME = 0x00000001;
        private const int EVERYTHING_REQUEST_PATH = 0x00000002;

        [DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
        private static extern UInt32 Everything_SetSearchW(string lpSearchString);

        [DllImport("Everything64.dll")]
        public static extern bool Everything_QueryW(bool bWait);
        [DllImport("Everything64.dll")]
        private static extern UInt32 Everything_GetNumResults();
        [DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
        private static extern void Everything_GetResultFullPathName(UInt32 nIndex, StringBuilder lpString, UInt32 nMaxCount);

        [DllImport("Everything64.dll")]
        private static extern void Everything_SetRequestFlags(UInt32 dwRequestFlags);

        private static readonly byte[] LIBCEF = Encoding.ASCII.GetBytes("cef_string_utf8_to_utf16"),
            ELECTRON = Encoding.ASCII.GetBytes("third_party/electron_node"),
            ELECTRON2 = Encoding.ASCII.GetBytes("register_atom_browser_web_contents"),
            CEF_SHARP = Encoding.ASCII.GetBytes("CefSharp.Internals"),
            NWJS = Encoding.ASCII.GetBytes("url-nwjs");

        private readonly HashSet<string> processes = new();
        private int cnt = 0;
        private long totalSize = 0;
        private readonly string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        private readonly HashSet<string> cache = new();

        private static bool Contains(byte[] file, byte[] search)
        {
            return Parallel.For(0, file.Length - search.Length, (i, loopState) =>
            {
                if (file[i] == search[0])
                {
                    byte[] localCache = new byte[search.Length];
                    Array.Copy(file, i, localCache, 0, search.Length);
                    if (Enumerable.SequenceEqual(localCache, search))
                        loopState.Stop();
                }
            }).IsCompleted == false;
        }

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalcLabelPos();
        }

        private void CalcLabelPos()
        {
            int x = (int)(0.5 * (this.Width - label.Width));
            int y = label.Location.Y;
            label.Location = new Point(x, y);
        }

        private static Icon? GetIcon(string filePath)
        {
            try { return Icon.ExtractAssociatedIcon(filePath); } catch { return null; }
        }

        private void AddIcon(string fileName, string? type)
        {
            if (cache.Contains(fileName)) return;
            cache.Add(fileName);
            var name = (type ?? "Unknown: ") + Path.GetFileName(fileName);
            Debug.WriteLine(name);
            var button = new System.Windows.Forms.Button
            {
                Size = new Size(100, 100),
                TabIndex = 0,
                BackgroundImageLayout = ImageLayout.Stretch,
                Dock = DockStyle.Top,
            };
            if (type == null) button.Text = "Unknown";
            else button.BackgroundImage = GetIcon(fileName)?.ToBitmap();
            var label2 = new System.Windows.Forms.Label
            {
                Text = name,
                Dock = DockStyle.Bottom,
                Font = new Font(label.Font.FontFamily, 6),
                AutoEllipsis = true,
            };
            if (type != null && processes.Contains(fileName)) label2.ForeColor = System.Drawing.Color.Green;
            var panel = new Panel()
            {
                Controls = { button, label2 },
                BackColor = Color.Transparent,
                Size = new Size(100, 128)
            };
            button.MouseEnter += (object? sender, EventArgs e) => toolTip1.Show(fileName, (System.Windows.Forms.Button)sender!);
            button.MouseLeave += (object? sender, EventArgs e) => toolTip1.Hide((System.Windows.Forms.Button)sender!);
            button.Click += (object? sender, EventArgs e) => Process.Start("Explorer", "/select," + fileName);

            cnt++;
            FolderSize(Directory.GetParent(fileName)!);

            int order = 0;
            double len = totalSize;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024.0;
            }
            Invoke(new MethodInvoker(delegate
            {
                label.Text = $"这台电脑上总共有 {cnt} 个 Chromium 内核的应用 ({String.Format("{0:0.##}{1}", len, sizes[order])})";
                CalcLabelPos();
                apps.Controls.Add(panel);
            }));
        }

        private void FolderSize(DirectoryInfo folder, int deep = 0)
        {
            if (deep > 10) return;
            try
            {
                FileInfo[] allFiles = folder.GetFiles();
                foreach (FileInfo file in allFiles) try
                    {
                        totalSize += file.Length;
                    }
                    catch (Exception)
                    {
                    }
                foreach (DirectoryInfo dir in folder.GetDirectories()) FolderSize(dir, deep + 1);
            }
            catch (Exception)
            {
                return;
            }
        }

        private void SearchResult(string defaultType = "Unknown: ")
        {
            var buf = new StringBuilder(260);
            for (uint i = 0; i < Everything_GetNumResults(); i++)
            {
                buf.Clear();
                Everything_GetResultFullPathName(i, buf, 260);

                var path = Path.GetDirectoryName(buf.ToString())!;
                if (cache.Contains(path) || File.GetAttributes(buf.ToString()).HasFlag(FileAttributes.Directory) ||
                    path.Contains("$RECYCLE.BIN") || path.Contains("OneDrive")) continue;
                cache.Add(path);
                bool flag = false;
                string? firstExe = null;
                var search = (string path) =>
                {
                    try
                    {
                        var f = Path.Join(path, "msedge.exe");
                        if (File.Exists(f))
                        {
                            flag = true;
                            AddIcon(f, "Edge: ");
                            return;
                        }
                        if (File.Exists(f = Path.Join(path, "chrome_pwa_launcher.exe")) && File.Exists(f = Path.Join(path, "../chrome.exe")))
                        {
                            flag = true;
                            AddIcon(f, "Chrome: ");
                            return;
                        }
                        foreach (var it in Directory.GetFiles(path, "*.exe"))
                        {
                            string type;
                            var data = File.ReadAllBytes(it);
                            var fileName = Path.GetFileName(it);
                            if (Contains(data, ELECTRON) || Contains(data, ELECTRON2)) type = "Electron: ";
                            else if (Contains(data, CEF_SHARP)) type = "CefSharp: ";
                            else if (Contains(data, NWJS)) type = "NWJS: ";
                            else if (Contains(data, LIBCEF)) type = "CEF: ";
                            else if (firstExe == null && !fileName.Contains("uninst", StringComparison.OrdinalIgnoreCase) &&
                                !fileName.Contains("setup", StringComparison.OrdinalIgnoreCase) &&
                                !fileName.Contains("report", StringComparison.OrdinalIgnoreCase))
                            {
                                firstExe = it;
                                continue;
                            }
                            else continue;
                            flag = true;
                            AddIcon(it, type);
                        }
                    }
                    catch { }
                };
                search(path);
                if (!flag)
                {
                    if (firstExe == null)
                    {
                        search(Path.GetDirectoryName(path)!);
                        if (firstExe == null) AddIcon(path, null);
                        else AddIcon(firstExe, defaultType);
                    }
                    else AddIcon(firstExe, defaultType);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CalcLabelPos();
            new Thread(() =>
            {
                try
                {

                    foreach (var it in Process.GetProcesses())
                    {
                        try
                        {
                            if (it.MainModule?.FileName != null) processes.Add(it.MainModule.FileName);
                        }
                        catch { }
                    }
                    Everything_SetSearchW("_percent.pak");
                    Everything_SetRequestFlags(EVERYTHING_REQUEST_PATH | EVERYTHING_REQUEST_FILE_NAME);
                    Everything_QueryW(true);

                    Invoke(new MethodInvoker(delegate
                    {
                        label.Text = "这台电脑上总共有 0 个 Chromium 内核的应用";
                        CalcLabelPos();
                    }));

                    SearchResult();

                    Everything_SetSearchW("libcef");
                    Everything_QueryW(true);
                    SearchResult("CEF: ");
                } finally
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        label.ForeColor = Color.Green;
                        if (cnt == 0) label.Text = "这台电脑上没有 Chromium 内核的应用 (也可能是你没装 Everything)";
                    }));
                }
            }).Start();
        }
    }
}