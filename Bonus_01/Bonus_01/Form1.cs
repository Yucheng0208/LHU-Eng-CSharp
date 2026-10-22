using Svg;

namespace Bonus_01
{
    public partial class Form1 : Form
    {
        // 由 img 資料夾中的 SVG 轉成的點陣圖
        private Bitmap imgLightOn = null!;
        private Bitmap imgLightOff = null!;
        private Bitmap imgToggleOn = null!;
        private Bitmap imgToggleOff = null!;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            imgLightOn = LoadSvg("light-on.svg", Color.Gold, 200);
            imgLightOff = LoadSvg("light-off.svg", Color.DimGray, 200);
            imgToggleOn = LoadSvg("toggle-on.svg", Color.SeaGreen, 128);
            imgToggleOff = LoadSvg("toggle-off.svg", Color.DimGray, 128);

            // 所有開關預設為關
            foreach (PictureBox sw in new[] { picSwitch11, picSwitch21, picSwitch22,
                                              picSwitch31, picSwitch32, picSwitch33 })
            {
                SetSwitch(sw, false);
            }
            UpdateLights();
        }

        // 讀取 SVG 檔，以指定顏色填色後轉成 Bitmap
        private static Bitmap LoadSvg(string fileName, Color color, int size)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "img", fileName);
            SvgDocument doc = SvgDocument.Open(path);
            foreach (SvgPath p in doc.Descendants().OfType<SvgPath>())
            {
                p.Fill = new SvgColourServer(color);
            }
            return doc.Draw(size, size);
        }

        private bool IsOn(PictureBox sw) => sw.Tag is true;

        private void SetSwitch(PictureBox sw, bool on)
        {
            sw.Tag = on;
            sw.Image = on ? imgToggleOn : imgToggleOff;
        }

        private void SetLight(PictureBox light, bool on)
        {
            light.Image = on ? imgLightOn : imgLightOff;
        }

        // 點擊任一開關：切換狀態後重新計算燈泡
        private void Switch_Click(object? sender, EventArgs e)
        {
            if (sender is PictureBox sw)
            {
                SetSwitch(sw, !IsOn(sw));
                SoundFx.PlaySwitch(IsOn(sw));
                UpdateLights();
            }
        }

        // 切換到「功德 +1」，關閉該視窗後回到燈泡控制
        private void btnGoMerit_Click(object sender, EventArgs e)
        {
            var merit = new FormMerit();
            merit.FormClosed += (s, args) => Show();
            Hide();
            merit.Show();
        }

        private void UpdateLights()
        {
            // 一對一：S1 控制 L
            SetLight(picLight11, IsOn(picSwitch11));

            // 二對一：樓梯燈，任一開關切換都會改變燈的狀態 (XOR)
            SetLight(picLight21, IsOn(picSwitch21) ^ IsOn(picSwitch22));

            // 三對二：S2 為共用開關，L1 = S1 ⊕ S2，L2 = S2 ⊕ S3
            SetLight(picLight31, IsOn(picSwitch31) ^ IsOn(picSwitch32));
            SetLight(picLight32, IsOn(picSwitch32) ^ IsOn(picSwitch33));
        }
    }
}
