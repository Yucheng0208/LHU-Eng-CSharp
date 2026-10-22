using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace Bonus_01
{
    // 自繪木魚：畫面全部用 GDI+ 畫出來，不用 Label，也不用外部圖片
    public class WoodenFishView : Control
    {
        private const int HitAnim = 150;       // 敲擊動畫長度 (ms)
        private const int FloatLife = 900;     // 「功德 +1」飄字存在時間 (ms)
        private const int Milestone = 100;     // 每敲幾下慶祝一次
        private const int CelebrateLife = 2500;  // 「功德圓滿」橫幅顯示時間 (ms)
        private const int ConfettiLife = 1600;   // 彩帶存在時間 (ms)

        private class FloatText
        {
            public PointF Pos;
            public long Start;
        }

        private class Confetti
        {
            public PointF Pos;
            public PointF Vel;   // 每秒移動的像素
            public Color Color;
            public float Size;
        }

        private const int FireworkMilestone = 500;   // 每敲幾下放煙火
        private const int RocketRise = 500;          // 煙火升空時間 (ms)
        private const int BurstLife = 1400;          // 爆開後的火花存在時間 (ms)

        private class Firework
        {
            public long Launch;         // 升空時間點
            public PointF From;         // 從畫面底部發射
            public PointF Burst;        // 爆開位置
            public Color Color;
            public PointF[] Sparks = Array.Empty<PointF>();   // 每顆火花的初速 (像素/秒)
            public bool Exploded;
        }

        private const int LotusMilestone = 1000;     // 每敲幾下蓮花綻放
        private const int LotusLife = 5000;          // 蓮花顯示時間 (ms)
        private const int LotusBloom = 900;          // 花瓣張開時間 (ms)

        private class Petal
        {
            public float X;          // 起始水平位置 (0~1，相對寬度)
            public float Delay;      // 延遲出現 (ms)
            public float Speed;      // 落下速度 (像素/秒)
            public float Phase;      // 左右飄動相位
        }

        private readonly List<Petal> petals = new();
        private long lotusStart = -100000;

        private readonly List<Firework> fireworks = new();
        private long fireworksEnd = -100000;
        private readonly List<Confetti> confetti = new();
        private long celebrateStart = -100000;
        private BigInteger celebrateMerit;

        private readonly System.Windows.Forms.Timer timer = new() { Interval = 15 };
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private readonly List<FloatText> floats = new();
        private readonly Random rnd = new();
        private long lastHit = -100000;

        // 用 BigInteger 計數，敲再多次也不會溢位變成負數
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BigInteger Merit { get; private set; }

        public WoodenFishView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.Hand;
            timer.Tick += Timer_Tick;
        }

        public void Hit()
        {
            long now = clock.ElapsedMilliseconds;
            Merit += 1;
            lastHit = now;

            // 飄字出現在木魚左上方，避開右邊的槌子
            GetFishLayout(out float cx, out float cy, out float r);
            int jitter = (int)(r * 0.15f);
            floats.Add(new FloatText
            {
                Start = now,
                Pos = new PointF(cx - r * 0.45f + rnd.Next(-jitter, jitter + 1), cy - r * 1.05f),
            });

            SoundFx.PlayKnock();
            if (Merit % Milestone == 0) Celebrate(now, cx, cy, r);
            if (Merit % FireworkMilestone == 0) LaunchFireworks(now);
            if (Merit % LotusMilestone == 0) BloomLotus(now);
            timer.Start();
            Invalidate();
        }

        // 每 100 下：顯示「恭喜功德圓滿」橫幅、噴出彩帶並播放鐘聲
        private void Celebrate(long now, float cx, float cy, float r)
        {
            celebrateStart = now;
            celebrateMerit = Merit;

            Color[] colors = { Color.Gold, Color.Orange, Color.OrangeRed, Color.White, Color.FromArgb(255, 236, 179) };
            float dpi = DeviceDpi / 96f;
            confetti.Clear();
            for (int i = 0; i < 60; i++)
            {
                double angle = (-150 + rnd.NextDouble() * 120) * Math.PI / 180;   // 往上方扇形噴出
                float speed = (300 + rnd.Next(400)) * dpi;
                confetti.Add(new Confetti
                {
                    Pos = new PointF(cx, cy - r * 0.6f),
                    Vel = new PointF(speed * (float)Math.Cos(angle), speed * (float)Math.Sin(angle)),
                    Color = colors[rnd.Next(colors.Length)],
                    Size = (4 + rnd.Next(5)) * dpi,
                });
            }

            SoundFx.PlayCelebrate();
        }

        // 每 500 下：連續發射 7 發煙火
        private void LaunchFireworks(long now)
        {
            Color[] colors = { Color.Gold, Color.DeepPink, Color.DeepSkyBlue, Color.LimeGreen, Color.OrangeRed, Color.MediumPurple, Color.White };
            float dpi = DeviceDpi / 96f;
            fireworks.Clear();
            for (int i = 0; i < 7; i++)
            {
                var burst = new PointF(Width * (0.12f + (float)rnd.NextDouble() * 0.76f),
                                       Height * (0.15f + (float)rnd.NextDouble() * 0.3f));
                var sparks = new PointF[48];
                for (int k = 0; k < sparks.Length; k++)
                {
                    double angle = k * 2 * Math.PI / sparks.Length + rnd.NextDouble() * 0.1;
                    float speed = (120 + rnd.Next(80)) * dpi;
                    sparks[k] = new PointF(speed * (float)Math.Cos(angle), speed * (float)Math.Sin(angle));
                }
                fireworks.Add(new Firework
                {
                    Launch = now + i * 280,
                    From = new PointF(burst.X + rnd.Next(-40, 41) * dpi, Height),
                    Burst = burst,
                    Color = colors[i % colors.Length],
                    Sparks = sparks,
                });
            }
            fireworksEnd = fireworks[^1].Launch + RocketRise + BurstLife;
        }

        // 每 1000 下：木魚底下綻放蓮花，佛光環繞，天上飄落花瓣
        private void BloomLotus(long now)
        {
            lotusStart = now;
            petals.Clear();
            for (int i = 0; i < 36; i++)
            {
                petals.Add(new Petal
                {
                    X = (float)rnd.NextDouble(),
                    Delay = rnd.Next(2500),
                    Speed = (60 + rnd.Next(60)) * DeviceDpi / 96f,
                    Phase = (float)(rnd.NextDouble() * Math.PI * 2),
                });
            }
            SoundFx.PlayLotus();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) Hit();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            long now = clock.ElapsedMilliseconds;
            floats.RemoveAll(f => now - f.Start > FloatLife);

            // 煙火爆開的瞬間播放爆炸聲
            foreach (var fw in fireworks)
            {
                if (!fw.Exploded && now - fw.Launch >= RocketRise)
                {
                    fw.Exploded = true;
                    SoundFx.PlayFirework();
                }
            }

            if (floats.Count == 0 && now - lastHit > 500 && now - celebrateStart > CelebrateLife &&
                now > fireworksEnd && now - lotusStart > LotusLife)
            {
                timer.Stop();
            }
            Invalidate();
        }

        private void GetFishLayout(out float cx, out float cy, out float r)
        {
            cx = Width / 2f;
            cy = Height * 0.58f;
            r = Math.Max(20, Math.Min(Width, Height) * 0.22f);
        }

        // 0~1，剛敲下去為 1，隨時間衰減到 0
        private float Decay(long now, int duration) =>
            Math.Clamp(1f - (now - lastHit) / (float)duration, 0f, 1f);

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            long now = clock.ElapsedMilliseconds;
            GetFishLayout(out float cx, out float cy, out float r);
            float hit = Decay(now, HitAnim);

            // 背景
            using (var bg = new LinearGradientBrush(ClientRectangle.IsEmpty ? new Rectangle(0, 0, 1, 1) : ClientRectangle,
                       Color.FromArgb(30, 27, 46), Color.FromArgb(62, 46, 79), 90f))
            {
                g.FillRectangle(bg, ClientRectangle);
            }

            DrawFireworks(g, now);
            float lotus = DrawLotusBack(g, now, cx, cy, r);

            // 敲擊時的金色光暈
            float glow = Decay(now, 500);
            if (glow > 0)
            {
                using var gp = new GraphicsPath();
                gp.AddEllipse(cx - r * 2.2f, cy - r * 2.2f, r * 4.4f, r * 4.4f);
                using var pb = new PathGradientBrush(gp)
                {
                    CenterColor = Color.FromArgb((int)(120 * glow), 255, 200, 90),
                    SurroundColors = new[] { Color.FromArgb(0, 255, 200, 90) },
                };
                g.FillPath(pb, gp);
            }

            // 擴散波紋
            long since = now - lastHit;
            if (since < 400)
            {
                float t = since / 400f;
                float rr = r * (1.1f + t * 0.8f);
                using var ring = new Pen(Color.FromArgb((int)(160 * (1 - t)), 255, 220, 150), 3);
                g.DrawEllipse(ring, cx - rr, cy - rr * 0.85f, rr * 2, rr * 1.7f);
            }

            // 坐墊
            using (var cushion = new SolidBrush(Color.FromArgb(139, 30, 30)))
            {
                g.FillEllipse(cushion, cx - r * 1.35f, cy + r * 0.62f, r * 2.7f, r * 0.55f);
            }

            // 木魚本體 (敲擊時縮一下)
            float s = 1f - 0.07f * hit;
            float bw = r * 2.2f * s, bh = r * 1.8f * s;
            var body = new RectangleF(cx - bw / 2, cy - bh / 2 + (1 - s) * r, bw, bh);
            using (var bp = new GraphicsPath())
            {
                bp.AddEllipse(body);
                using var pb = new PathGradientBrush(bp)
                {
                    CenterPoint = new PointF(body.X + bw * 0.38f, body.Y + bh * 0.35f),
                    CenterColor = Color.FromArgb(222, 145, 80),
                    SurroundColors = new[] { Color.FromArgb(110, 52, 18) },
                };
                g.FillPath(pb, bp);
                using var outline = new Pen(Color.FromArgb(74, 35, 11), 3);
                g.DrawPath(outline, bp);
            }

            // 魚嘴 (下方的弧形開口)
            using (var mouth = new Pen(Color.FromArgb(43, 18, 6), r * 0.13f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                var mr = new RectangleF(body.X + bw * 0.18f, body.Y + bh * 0.25f, bw * 0.64f, bh * 0.5f);
                g.DrawArc(mouth, mr, 25, 130);
            }

            // 高光
            using (var hl = new SolidBrush(Color.FromArgb(70, 255, 255, 255)))
            {
                g.FillEllipse(hl, body.X + bw * 0.2f, body.Y + bh * 0.1f, bw * 0.3f, bh * 0.18f);
            }

            if (lotus > 0) DrawLotusFront(g, now, cx, cy, r, lotus);
            DrawMallet(g, cx, cy, r, hit);
            DrawFallingPetals(g, now, r);
            float titleBottom = DrawHud(g);
            DrawFloats(g, now, titleBottom);            // 文字最後畫，永遠在最上層不會被遮住
            DrawCelebration(g, now, cy - r * 0.15f);
        }

        private void DrawFireworks(Graphics g, long now)
        {
            if (now > fireworksEnd) return;
            float dpi = DeviceDpi / 96f;
            float gravity = 90 * dpi;

            foreach (var fw in fireworks)
            {
                long t = now - fw.Launch;
                if (t < 0) continue;

                if (t < RocketRise)
                {
                    // 升空：一顆亮點拖著尾巴
                    float k = t / (float)RocketRise;
                    k = 1 - (1 - k) * (1 - k);
                    float x = fw.From.X + (fw.Burst.X - fw.From.X) * k;
                    float y = fw.From.Y + (fw.Burst.Y - fw.From.Y) * k;
                    using var trail = new Pen(Color.FromArgb(120, fw.Color), 2 * dpi);
                    g.DrawLine(trail, x, y, x - (fw.Burst.X - fw.From.X) * 0.06f, y + 30 * dpi);
                    using var head = new SolidBrush(Color.FromArgb(255, 255, 240, 200));
                    g.FillEllipse(head, x - 3 * dpi, y - 3 * dpi, 6 * dpi, 6 * dpi);
                    continue;
                }

                // 爆開：火花往外擴散、受重力下墜並淡出
                long tb = t - RocketRise;
                if (tb > BurstLife) continue;
                float sec = tb / 1000f;
                float alpha = 1 - tb / (float)BurstLife;
                float spread = sec * (1 - sec * 0.35f);   // 越飛越慢
                using var pen = new Pen(Color.FromArgb((int)(220 * alpha), fw.Color), 2 * dpi) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                using var dot = new SolidBrush(Color.FromArgb((int)(255 * alpha), Color.White));
                foreach (var v in fw.Sparks)
                {
                    float x = fw.Burst.X + v.X * spread;
                    float y = fw.Burst.Y + v.Y * spread + 0.5f * gravity * sec * sec;
                    float x0 = fw.Burst.X + v.X * spread * 0.8f;
                    float y0 = fw.Burst.Y + v.Y * spread * 0.8f + 0.5f * gravity * sec * sec;
                    g.DrawLine(pen, x0, y0, x, y);
                    g.FillEllipse(dot, x - 1.5f * dpi, y - 1.5f * dpi, 3 * dpi, 3 * dpi);
                }
            }
        }

        // 蓮花整體透明度 (0~1)：最後 800ms 淡出
        private float LotusAlpha(long since) =>
            since > LotusLife ? 0 : since > LotusLife - 800 ? (LotusLife - since) / 800f : 1f;

        // 一片花瓣：從 basePt 往 angle 方向 (0 度 = 正上方) 長出去
        private static void FillPetal(Graphics g, PointF basePt, float angle, float len, float width, float alpha)
        {
            if (len < 1) return;
            using var path = new GraphicsPath();
            path.AddBezier(0, 0, width, -len * 0.35f, width * 0.6f, -len * 0.8f, 0, -len);
            path.AddBezier(0, -len, -width * 0.6f, -len * 0.8f, -width, -len * 0.35f, 0, 0);
            using (var m = new Matrix())
            {
                m.Translate(basePt.X, basePt.Y);
                m.Rotate(angle);
                path.Transform(m);
            }

            var bounds = path.GetBounds();
            bounds.Inflate(1, 1);
            using var brush = new LinearGradientBrush(bounds,
                Color.FromArgb((int)(255 * alpha), 230, 110, 155),
                Color.FromArgb((int)(255 * alpha), 255, 228, 238), angle - 90);
            using var pen = new Pen(Color.FromArgb((int)(200 * alpha), 190, 70, 120), 1.5f);
            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        // 蓮花後層：佛光 + 往上張開的花瓣 (畫在木魚後面)；回傳綻放程度
        private float DrawLotusBack(Graphics g, long now, float cx, float cy, float r)
        {
            long since = now - lotusStart;
            float alpha = LotusAlpha(since);
            if (alpha <= 0) return 0;

            float bloom = Math.Clamp(since / (float)LotusBloom, 0f, 1f);
            bloom = 1 - (1 - bloom) * (1 - bloom) * (1 - bloom);

            // 旋轉的佛光
            var state = g.Save();
            g.TranslateTransform(cx, cy);
            g.RotateTransform(since * 0.02f);
            using (var ray = new SolidBrush(Color.FromArgb((int)(45 * alpha * bloom), 255, 215, 120)))
            {
                float len = r * 3f;
                for (int i = 0; i < 16; i++)
                {
                    g.FillPolygon(ray, new[] { new PointF(0, 0), new PointF(-r * 0.18f, -len), new PointF(r * 0.18f, -len) });
                    g.RotateTransform(360f / 16);
                }
            }
            g.Restore(state);

            var basePt = new PointF(cx, cy + r * 0.85f);
            float[] angles = { -72, -48, -24, 0, 24, 48, 72 };
            foreach (float a in angles)
            {
                FillPetal(g, basePt, a * bloom, r * 1.9f * (0.4f + 0.6f * bloom), r * 0.4f, alpha);
            }
            return bloom;
        }

        // 蓮花前層：兩側往外攤開的花瓣，托住木魚
        private void DrawLotusFront(Graphics g, long now, float cx, float cy, float r, float bloom)
        {
            float alpha = LotusAlpha(now - lotusStart);
            var basePt = new PointF(cx, cy + r * 0.85f);
            float[] angles = { -95, -68, 68, 95 };
            foreach (float a in angles)
            {
                FillPetal(g, basePt, a * bloom, r * 1.35f * (0.4f + 0.6f * bloom), r * 0.34f, alpha);
            }
        }

        // 從天上飄落的花瓣
        private void DrawFallingPetals(Graphics g, long now, float r)
        {
            long since = now - lotusStart;
            float alpha = LotusAlpha(since);
            if (alpha <= 0) return;

            float sway = 20 * DeviceDpi / 96f;
            foreach (var p in petals)
            {
                float t = (since - p.Delay) / 1000f;
                if (t < 0) continue;
                float x = p.X * Width + (float)Math.Sin(p.Phase + t * 2) * sway;
                float y = -r * 0.2f + p.Speed * t;
                float angle = (p.Phase + t * 1.5f) * 57.3f;
                FillPetal(g, new PointF(x, y), angle, r * 0.22f, r * 0.08f, alpha * 0.9f);
            }
        }

        private void DrawMallet(Graphics g, float cx, float cy, float r, float hit)
        {
            var pivot = new PointF(cx + r * 1.7f, cy - r * 1.5f);
            var contact = new PointF(cx + r * 0.55f, cy - r * 0.78f);
            double hitAngle = Math.Atan2(contact.Y - pivot.Y, contact.X - pivot.X);
            double restAngle = hitAngle + 0.75;   // 平常槌子抬起
            double a = restAngle + (hitAngle - restAngle) * hit;

            float headR = r * 0.17f;
            float len = (float)Math.Sqrt(Math.Pow(contact.X - pivot.X, 2) + Math.Pow(contact.Y - pivot.Y, 2)) - headR;
            var head = new PointF(pivot.X + len * (float)Math.Cos(a), pivot.Y + len * (float)Math.Sin(a));

            using (var stick = new Pen(Color.FromArgb(232, 210, 166), r * 0.09f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                g.DrawLine(stick, pivot, head);
            }
            using (var hb = new SolidBrush(Color.FromArgb(160, 82, 45)))
            using (var hp = new Pen(Color.FromArgb(90, 40, 15), 2))
            {
                g.FillEllipse(hb, head.X - headR, head.Y - headR, headR * 2, headR * 2);
                g.DrawEllipse(hp, head.X - headR, head.Y - headR, headR * 2, headR * 2);
            }
        }

        // 依字型實際高度排版，高 DPI 縮放時文字才不會被切掉；回傳統計數字的底部位置
        private float DrawHud(Graphics g)
        {
            float margin = 16 * DeviceDpi / 96f;
            using var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            float titleBottom;
            using (var f = new Font("Microsoft JhengHei UI", 24, FontStyle.Bold))
            using (var b = new SolidBrush(Color.FromArgb(255, 223, 128)))
            {
                float h = f.GetHeight(g) * 1.2f;
                g.DrawString($"功德：{Merit}", f, b, new RectangleF(0, margin, Width, h), center);
                titleBottom = margin + h;
            }

            using (var f = new Font("Microsoft JhengHei UI", 10))
            using (var b = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
            {
                float h = f.GetHeight(g) * 1.4f;
                g.DrawString("點擊木魚或按空白鍵", f, b, new RectangleF(0, Height - margin / 2 - h, Width, h), center);
            }
            return titleBottom;
        }

        private void DrawFloats(Graphics g, long now, float ceiling)
        {
            using var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var f = new Font("Microsoft JhengHei UI", 18, FontStyle.Bold);
            float lineH = f.GetHeight(g);
            float minY = ceiling + lineH * 0.6f;   // 飄字中心不能高於此，避免碰到統計數字
            float limit = float.MaxValue;

            // 由新到舊畫，較舊的飄字至少比新的高一行，連敲時文字才不會互相蓋住
            for (int i = floats.Count - 1; i >= 0; i--)
            {
                var ft = floats[i];
                float t = (now - ft.Start) / (float)FloatLife;
                if (t > 1) continue;
                float y = Math.Min(ft.Pos.Y - 70 * t, limit);
                limit = y - lineH;
                if (y < minY) break;   // 再舊的只會更高，全部不畫

                // 接近上限時淡出
                float edge = Math.Clamp((y - minY) / lineH, 0f, 1f);
                int alpha = (int)(255 * (1 - t * t) * edge);
                using var shadow = new SolidBrush(Color.FromArgb(alpha / 2, 0, 0, 0));
                using var b = new SolidBrush(Color.FromArgb(alpha, 255, 236, 179));
                g.DrawString("功德 +1", f, shadow, ft.Pos.X + 2, y + 2, center);
                g.DrawString("功德 +1", f, b, ft.Pos.X, y, center);
            }
        }

        private void DrawCelebration(Graphics g, long now, float centerY)
        {
            long since = now - celebrateStart;
            if (since > CelebrateLife) return;

            // 彩帶：拋物線落下並淡出
            if (since < ConfettiLife)
            {
                float t = since / 1000f;
                float gravity = 1200 * DeviceDpi / 96f;
                int alpha = (int)(255 * (1 - since / (float)ConfettiLife));
                foreach (var c in confetti)
                {
                    float x = c.Pos.X + c.Vel.X * t;
                    float y = c.Pos.Y + c.Vel.Y * t + 0.5f * gravity * t * t;
                    using var b = new SolidBrush(Color.FromArgb(alpha, c.Color));
                    g.FillEllipse(b, x - c.Size / 2, y - c.Size / 2, c.Size, c.Size * 0.6f);
                }
            }

            // 橫幅：彈出 → 停留 → 淡出
            float appear = Math.Clamp(since / 250f, 0f, 1f);
            float fade = since > CelebrateLife - 500 ? (CelebrateLife - since) / 500f : 1f;
            float a = appear * fade;
            float pop = 0.7f + 0.3f * appear + 0.15f * (float)Math.Sin(appear * Math.PI);

            string text = $"恭喜功德圓滿 {celebrateMerit} 下！";
            float size = 24 * pop;
            using (var probe = new Font("Microsoft JhengHei UI", size, FontStyle.Bold))
            {
                float w = g.MeasureString(text, probe).Width;
                if (w > Width * 0.9f) size *= Width * 0.9f / w;   // 太寬就縮小，避免左右被切掉
            }

            using var f = new Font("Microsoft JhengHei UI", size, FontStyle.Bold);
            float bandH = f.GetHeight(g) * 1.8f;
            var band = new RectangleF(0, centerY - bandH / 2, Width, bandH);

            using (var bg = new SolidBrush(Color.FromArgb((int)(170 * a), 20, 10, 30)))
            using (var line = new Pen(Color.FromArgb((int)(255 * a), 255, 200, 60), 2 * DeviceDpi / 96f))
            {
                g.FillRectangle(bg, band);
                g.DrawLine(line, 0, band.Top, Width, band.Top);
                g.DrawLine(line, 0, band.Bottom, Width, band.Bottom);
            }

            using var center = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var shadow = new SolidBrush(Color.FromArgb((int)(180 * a), 0, 0, 0));
            using var gold = new SolidBrush(Color.FromArgb((int)(255 * a), 255, 215, 80));
            var shadowRect = band;
            shadowRect.Offset(2, 2);
            g.DrawString(text, f, shadow, shadowRect, center);
            g.DrawString(text, f, gold, band, center);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) timer.Dispose();
            base.Dispose(disposing);
        }
    }
}
