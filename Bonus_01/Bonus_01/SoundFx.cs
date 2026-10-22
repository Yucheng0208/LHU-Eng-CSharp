using System.Runtime.InteropServices;

namespace Bonus_01
{
    // 用程式合成的音效，不需要外部音效檔
    internal static class SoundFx
    {
        private const int Rate = 44100;
        private static readonly Random noiseRnd = new(1234);

        // 木魚用 8 個聲道輪流播放，連續快速敲擊時聲音可以重疊，不會被切掉
        private static readonly Voices knock = new(Create(0.30, Knock), 8);
        private static readonly Voices switchOn = new(Create(0.06, t => Click(t, 4200, 2800)), 3);
        private static readonly Voices switchOff = new(Create(0.06, t => Click(t, 2600, 1700)), 3);

        private static readonly Voices celebrate = new(Create(1.8, Chime), 2);

        public static void PlayKnock() => knock.Play();
        public static void PlayCelebrate() => celebrate.Play();

        private static readonly Voices firework = new(Create(1.0, Firework), 4);
        private static readonly Voices lotus = new(Create(4.0, Bowl), 1);

        public static void PlayFirework() => firework.Play();
        public static void PlayLotus() => lotus.Play();

        // 煙火：低沉的「碰」+ 爆裂雜音 + 後段零星的劈啪聲
        private static double Firework(double t)
        {
            double v = 0.9 * Math.Exp(-t * 9) * Math.Sin(2 * Math.PI * (70 - 30 * t) * t) +
                       0.7 * Math.Exp(-t * 7) * Noise();
            if (t > 0.25 && noiseRnd.NextDouble() < 0.004) v += 0.8 * Noise();   // 劈啪
            return v;
        }

        // 蓮花：頌缽般悠長的「嗡～」聲
        private static double Bowl(double t) =>
            Math.Min(1, t * 40) * Math.Exp(-t * 0.9) *
            (Math.Sin(2 * Math.PI * 262 * t) * (1 + 0.15 * Math.Sin(2 * Math.PI * 3 * t)) +
             0.45 * Math.Sin(2 * Math.PI * 262 * 2.71 * t) * Math.Exp(-t * 0.8) +
             0.2 * Math.Sin(2 * Math.PI * 262 * 5.02 * t) * Math.Exp(-t * 2));
        public static void PlaySwitch(bool on) => (on ? switchOn : switchOff).Play();

        private static double Noise() => noiseRnd.NextDouble() * 2 - 1;

        // 木魚：中空木頭的「叩」聲，低頻共鳴 + 高頻泛音 + 敲擊瞬間的雜音
        private static double Knock(double t) =>
            0.80 * Math.Exp(-t * 28) * Math.Sin(2 * Math.PI * 520 * t) +
            0.35 * Math.Exp(-t * 55) * Math.Sin(2 * Math.PI * 1380 * t) +
            0.15 * Math.Exp(-t * 90) * Math.Sin(2 * Math.PI * 2600 * t) +
            0.50 * Math.Exp(-t * 600) * Noise();

        // 功德圓滿：兩聲上揚的鐘聲，尾音拉長
        private static double Chime(double t)
        {
            static double Bell(double t, double f) => t < 0 ? 0 :
                Math.Exp(-t * 2.5) * (Math.Sin(2 * Math.PI * f * t) +
                                      0.5 * Math.Sin(2 * Math.PI * f * 2.76 * t) * Math.Exp(-t * 4) +
                                      0.25 * Math.Sin(2 * Math.PI * f * 5.4 * t) * Math.Exp(-t * 8));
            return Bell(t, 660) + 0.9 * Bell(t - 0.18, 990);
        }

        // 開關：兩段短促的「喀噠」聲，開比關的音調高
        private static double Click(double t, double f1, double f2)
        {
            double v = 0.6 * Math.Exp(-t * 900) * Noise() +
                       0.5 * Math.Exp(-t * 400) * Math.Sin(2 * Math.PI * f1 * t);
            double t2 = t - 0.018;
            if (t2 > 0)
            {
                v += 0.3 * Math.Exp(-t2 * 1200) * Noise() +
                     0.3 * Math.Exp(-t2 * 500) * Math.Sin(2 * Math.PI * f2 * t2);
            }
            return v;
        }

        // 把波形函式取樣成 16-bit 單聲道 PCM
        private static byte[] Create(double seconds, Func<double, double> wave)
        {
            int samples = (int)(Rate * seconds);
            var data = new double[samples];
            double peak = 1e-9;
            for (int i = 0; i < samples; i++)
            {
                data[i] = wave(i / (double)Rate);
                peak = Math.Max(peak, Math.Abs(data[i]));
            }

            var pcm = new byte[samples * 2];
            for (int i = 0; i < samples; i++)
            {
                short s = (short)(data[i] / peak * 0.7 * short.MaxValue);
                pcm[i * 2] = (byte)s;
                pcm[i * 2 + 1] = (byte)(s >> 8);
            }
            return pcm;
        }

        // 同一段聲音開多個 waveOut 聲道輪流使用，Windows 會自動把它們混音
        private sealed class Voices
        {
            private readonly IntPtr[] handles;
            private readonly IntPtr[] headers;
            private int next;

            public Voices(byte[] pcm, int count)
            {
                IntPtr data = Marshal.AllocHGlobal(pcm.Length);   // 程式結束前一直使用，不釋放
                Marshal.Copy(pcm, 0, data, pcm.Length);

                var fmt = new WaveFormat
                {
                    wFormatTag = 1,
                    nChannels = 1,
                    nSamplesPerSec = Rate,
                    nAvgBytesPerSec = Rate * 2,
                    nBlockAlign = 2,
                    wBitsPerSample = 16,
                };

                var hs = new List<IntPtr>();
                var hdrs = new List<IntPtr>();
                for (int i = 0; i < count; i++)
                {
                    if (waveOutOpen(out IntPtr h, WaveMapper, ref fmt, IntPtr.Zero, IntPtr.Zero, 0) != 0) break;

                    var hdr = new WaveHeader { lpData = data, dwBufferLength = (uint)pcm.Length };
                    IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf<WaveHeader>());
                    Marshal.StructureToPtr(hdr, p, false);
                    waveOutPrepareHeader(h, p, (uint)Marshal.SizeOf<WaveHeader>());

                    hs.Add(h);
                    hdrs.Add(p);
                }
                handles = hs.ToArray();
                headers = hdrs.ToArray();
            }

            public void Play()
            {
                if (handles.Length == 0) return;   // 沒有音效裝置就不播放
                int i = next;
                next = (next + 1) % handles.Length;

                // 這個聲道若還在播放上一輪的聲音，先停掉再重播
                waveOutReset(handles[i]);
                waveOutWrite(handles[i], headers[i], (uint)Marshal.SizeOf<WaveHeader>());
            }
        }

        private const uint WaveMapper = 0xFFFFFFFF;

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private struct WaveFormat
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public int nSamplesPerSec;
            public int nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WaveHeader
        {
            public IntPtr lpData;
            public uint dwBufferLength;
            public uint dwBytesRecorded;
            public IntPtr dwUser;
            public uint dwFlags;
            public uint dwLoops;
            public IntPtr lpNext;
            public IntPtr reserved;
        }

        [DllImport("winmm.dll")]
        private static extern int waveOutOpen(out IntPtr hWaveOut, uint uDeviceID, ref WaveFormat lpFormat,
                                              IntPtr dwCallback, IntPtr dwInstance, uint dwFlags);

        [DllImport("winmm.dll")]
        private static extern int waveOutPrepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, uint uSize);

        [DllImport("winmm.dll")]
        private static extern int waveOutWrite(IntPtr hWaveOut, IntPtr lpWaveOutHdr, uint uSize);

        [DllImport("winmm.dll")]
        private static extern int waveOutReset(IntPtr hWaveOut);
    }
}
