using Newtonsoft.Json;
using System.Diagnostics;
using NetMQ;
using NetMQ.Sockets;


namespace WinFormsApp4
{
    public partial class Form1 : Form
    {

        private Process? zmqProc;
        private const string ZMQ_EXE = "zeromq.exe";
        private string binPath = @"C:\Users\LENOVO\OneDrive - Yildiz Technical University\Desktop\converted.bin";
        private List<string> outputPaths = new List<string>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureUI();
            zmqProc = ZmqServerManager.StartZmqServer(ZMQ_EXE);
            if (zmqProc == null)
            {
                Close();
                return;
            }
            System.Threading.Thread.Sleep(600);

            if (!File.Exists(binPath))
            {
                MessageBox.Show($"Bin dosyasý bulunamadý:\n{binPath}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            YukleSureSecenekleri(binPath);
            labelSure.Visible = true;
            comboBoxSure.Visible = true;
            btnConvert.Visible = true;
        }

        private void ConfigureUI()
        {
            labelSure.Visible = false;
            comboBoxSure.Visible = false;
            btnConvert.Visible = false;
            lblStatus.Visible = false;
            labelOutput.Visible = false;
            comboBoxOutput.Visible = false;

            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 10);
            this.StartPosition = FormStartPosition.CenterScreen;

            btnConvert.BackColor = Color.SteelBlue;
            btnConvert.ForeColor = Color.White;
            btnConvert.FlatStyle = FlatStyle.Flat;
            btnConvert.FlatAppearance.BorderSize = 0;

            comboBoxSure.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxOutput.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void YukleSureSecenekleri(string path)
        {
            const int sampleRate = 44100;
            const int bitsPerSample = 8;
            const int numChannels = 1;

            long byteSize = new FileInfo(path).Length;
            int bytesPerSecond = sampleRate * numChannels * (bitsPerSample / 8);
            int totalSeconds = (int)(byteSize / bytesPerSecond);
            int totalHours = totalSeconds / 3600;

            comboBoxSure.Items.Clear();
            if (totalHours < 2)
            {
                comboBoxSure.Items.Add("Tamamý");
            }
            else
            {
                for (int h = 2; h < totalHours; h += 2)
                    comboBoxSure.Items.Add($"Ýlk {h} saat");
                comboBoxSure.Items.Add("Tamamý");
            }
            comboBoxSure.SelectedIndex = 0;
        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            string secim = comboBoxSure.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(secim))
            {
                MessageBox.Show("Lütfen bir bölüm seçin.", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int durationSec = 0; // Tamamý
            if (secim.StartsWith("Ýlk"))
            {
                int saat = int.Parse(secim.Split(' ')[1]);
                durationSec = saat * 3600;
            }

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string baseName = Path.GetFileNameWithoutExtension(binPath);
            string outputDir = Path.Combine(desktop, baseName + "_Kayitlar");
            Directory.CreateDirectory(outputDir);

            string wavPath = Path.Combine(outputDir, secim.Replace(" ", "_") + ".wav");

            var request = new ZmqRequest
            {
                binPath = binPath,
                outputFolder = outputDir,
                numChannels = 1,
                sampleRate = 44100,
                bitsPerSample = 8,
                durationSec = durationSec,
            };
            string json = JsonConvert.SerializeObject(request);

            btnConvert.Enabled = false;
            lblStatus.Visible = true;
            lblStatus.Text = "Dönüþtürülüyor...";

            await Task.Run(() =>
            {
                using var client = new RequestSocket();
                client.Connect("tcp://localhost:5555");
                client.SendFrame(json);
                string reply = client.ReceiveFrameString();
                dynamic resp = JsonConvert.DeserializeObject(reply);

                Invoke((Action)(() =>
                {
                    outputPaths.Clear();
                    comboBoxOutput.Items.Clear();

                    if ((string)resp.status == "success")
                    {
                        foreach (string p in resp.outputs.ToObject<List<string>>())
                        {
                            outputPaths.Add(p);
                            comboBoxOutput.Items.Add(Path.GetFileName(p));
                        }
                        comboBoxOutput.SelectedIndex = 0;
                        labelOutput.Visible = true;
                        comboBoxOutput.Visible = true;
                        lblStatus.Text = "Dönüþtürme tamamlandý.";
                    }
                    else
                    {
                        lblStatus.Text = "Dönüþtürme baþarýsýz.";
                        MessageBox.Show($"Sunucu hatasý: {(string)resp.status}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }));
            });

            btnConvert.Enabled = true;
        }

        private void comboBoxOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = comboBoxOutput.SelectedIndex;
            if (idx < 0 || idx >= outputPaths.Count) return;

            string filePath = outputPaths[idx];
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Dosya bulunamadý.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string folder = Path.GetDirectoryName(filePath);
            var psi = new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                if (zmqProc is { HasExited: false })
                    zmqProc.Kill();
            }
            catch { }

            base.OnFormClosed(e);
        }

    }
}
