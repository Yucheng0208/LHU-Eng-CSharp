using System.Globalization;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace Bonus_02
{
    public partial class Form1 : Form
    {
        private const int HistoryDays = 90;

        private class Currency
        {
            public string Code = "";
            public string Name = "";
            public override string ToString() => $"{Code} {Name}";
        }

        private static readonly Currency[] currencies =
        {
            new() { Code = "TWD", Name = "新台幣" },
            new() { Code = "USD", Name = "美元" },
            new() { Code = "JPY", Name = "日圓" },
            new() { Code = "EUR", Name = "歐元" },
            new() { Code = "CNY", Name = "人民幣" },
            new() { Code = "HKD", Name = "港幣" },
            new() { Code = "KRW", Name = "韓元" },
            new() { Code = "GBP", Name = "英鎊" },
            new() { Code = "AUD", Name = "澳幣" },
            new() { Code = "SGD", Name = "新加坡幣" },
            new() { Code = "THB", Name = "泰銖" },
            new() { Code = "CAD", Name = "加幣" },
        };

        private static readonly Color UpColor = Color.FromArgb(214, 39, 40);     // 台灣習慣：漲紅
        private static readonly Color DownColor = Color.FromArgb(44, 160, 44);   // 跌綠
        private static readonly Color ActualColor = Color.FromArgb(31, 119, 180);
        private static readonly Color ForecastColor = Color.FromArgb(255, 127, 14);

        private readonly RateService rates = new();
        private bool loaded;
        private bool suppressUpdate;

        public Form1()
        {
            InitializeComponent();
        }

        private string From => ((Currency)cboFrom.SelectedItem!).Code;
        private string To => ((Currency)cboTo.SelectedItem!).Code;

        private async void Form1_Load(object sender, EventArgs e)
        {
            suppressUpdate = true;
            cboFrom.Items.AddRange(currencies);
            cboTo.Items.AddRange(currencies);
            cboFrom.SelectedIndex = 1;   // USD
            cboTo.SelectedIndex = 0;     // TWD
            suppressUpdate = false;

            SetupCharts();
            SetupGrid();
            SetInputsEnabled(false);
            lblStatus.Text = $"正在下載即時匯率與近 {HistoryDays} 天歷史匯率…";
            txtResult.Text = "載入中…";

            try
            {
                await Task.WhenAll(rates.LoadLatestAsync(), rates.LoadHistoryAsync(HistoryDays));
                loaded = true;
                UpdateAll();
                refreshTimer.Start();
            }
            catch (Exception ex)
            {
                txtResult.Text = "";
                lblStatus.Text = "匯率載入失敗：" + ex.Message;
                MessageBox.Show(ex.Message, "無法取得匯率", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetInputsEnabled(true);
            }
        }

        // 每 10 分鐘自動更新即時匯率
        private async void refreshTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                await rates.LoadLatestAsync();
                UpdateAll();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "自動更新失敗，沿用上次的匯率：" + ex.Message;
            }
        }

        private void SetInputsEnabled(bool on)
        {
            txtAmount.Enabled = cboFrom.Enabled = cboTo.Enabled = btnSwap.Enabled = on;
        }

        private void txtAmount_TextChanged(object sender, EventArgs e) => UpdateConversion();

        private void Currency_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!suppressUpdate) UpdateAll();
        }

        // 單一對轉按鈕：交換「從」與「換成」的貨幣
        private void btnSwap_Click(object sender, EventArgs e)
        {
            suppressUpdate = true;
            (cboFrom.SelectedIndex, cboTo.SelectedIndex) = (cboTo.SelectedIndex, cboFrom.SelectedIndex);
            suppressUpdate = false;
            UpdateAll();
        }

        private void UpdateAll()
        {
            if (!loaded) return;
            UpdateConversion();

            if (From == To)
            {
                ClearAnalysis("「從」和「換成」是同一種貨幣，請選擇不同的貨幣。");
                return;
            }

            var series = rates.Series(From, To);
            Forecaster.Result? forecast = null;
            string? mlError = null;
            try
            {
                forecast = Forecaster.Predict(series);
            }
            catch (Exception ex)
            {
                mlError = ex.Message;
            }

            UpdateLineChart(series, forecast);
            UpdateBarChart(series);
            UpdateGrid(series, forecast);
            UpdateForecastTab(series, forecast, mlError);

            lblStatus.Text = $"資料來源：fawazahmed0/currency-api　｜　歷史資料 {series.Count} 天　｜　機器學習：ML.NET SSA 時間序列預測";
        }

        private void UpdateConversion()
        {
            if (!loaded) return;

            decimal rate = rates.LatestRate(From, To);
            lblRate.Text = $"1 {From} = {FormatRate(rate)} {To}　｜　1 {To} = {FormatRate(1 / rate)} {From}";
            lblUpdated.Text = $"即時匯率日期 {rates.LatestDate:yyyy/MM/dd}，更新於 {rates.FetchedAt:HH:mm:ss}（每 10 分鐘自動更新）";

            if (!decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal amount) || amount < 0)
            {
                txtResult.Text = "請輸入有效的金額";
                return;
            }
            decimal result = amount * rate;
            txtResult.Text = $"{result.ToString(result != 0 && result < 1 ? "N4" : "N2")} {To}";
        }

        // 依大小決定小數位數，例如 32.4858 / 0.006327
        private static string FormatRate(decimal v) =>
            v >= 100 ? v.ToString("N2") : v >= 1 ? v.ToString("N4") : v.ToString("0.######");

        private static string AxisFormat(IEnumerable<decimal> values)
        {
            decimal max = values.DefaultIfEmpty(1).Max();
            return max >= 100 ? "N1" : max >= 1 ? "N3" : "0.#####";
        }

        private static string Weekday(DateTime d) => "日一二三四五六"[(int)d.DayOfWeek].ToString();

        // ===== 圖表 =====

        private static ChartArea NewArea(string dateFormat)
        {
            var area = new ChartArea("main");
            area.AxisX.LabelStyle.Format = dateFormat;
            area.AxisX.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.IsStartedFromZero = false;
            area.AxisX.LabelStyle.Font = area.AxisY.LabelStyle.Font = new Font("Microsoft JhengHei UI", 9F);
            return area;
        }

        private static void PrepareChart(Chart chart)
        {
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.Legends.Clear();
            chart.Titles.Clear();
            chart.BackColor = Color.White;
            chart.Legends.Add(new Legend { Docking = Docking.Top, Alignment = StringAlignment.Center, Font = new Font("Microsoft JhengHei UI", 9F) });
        }

        private void SetupCharts()
        {
            foreach (var c in new[] { chartLine, chartBar, chartForecast })
            {
                PrepareChart(c);
            }
        }

        private static Title NewTitle(string text) => new(text) { Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold) };

        // 折線圖：近 90 天匯率 + ML 預測未來 5 天 (含 90% 信心區間)
        private void UpdateLineChart(List<(DateTime Date, decimal Rate)> series, Forecaster.Result? fc)
        {
            PrepareChart(chartLine);
            var area = NewArea("MM/dd");
            area.AxisY.LabelStyle.Format = AxisFormat(series.Select(s => s.Rate));
            chartLine.ChartAreas.Add(area);
            chartLine.Titles.Add(NewTitle($"{From} → {To}　近 {series.Count} 天匯率與未來 {Forecaster.Horizon} 天預測"));

            var actual = new Series("歷史匯率")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.Date,
                Color = ActualColor,
                BorderWidth = 2,
                ToolTip = "#VALX{yyyy/MM/dd}\n1 " + From + " = #VALY " + To,
            };
            foreach (var (date, rate) in series) actual.Points.AddXY(date, rate);
            chartLine.Series.Add(actual);

            if (fc == null) return;

            var band = new Series("90% 信心區間")
            {
                ChartType = SeriesChartType.Range,
                YValuesPerPoint = 2,
                XValueType = ChartValueType.Date,
                Color = Color.FromArgb(50, ForecastColor),
            };
            var predicted = new Series("ML 預測")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.Date,
                Color = ForecastColor,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                ToolTip = "#VALX{yyyy/MM/dd} 預測\n1 " + From + " = #VALY " + To,
            };
            var (lastDate, lastRate) = series[^1];
            predicted.Points.AddXY(lastDate, lastRate);   // 從最後一筆實際值接上預測線
            band.Points.AddXY(lastDate, lastRate, lastRate);
            for (int i = 0; i < fc.Dates.Length; i++)
            {
                predicted.Points.AddXY(fc.Dates[i], fc.Forecast[i]);
                band.Points.AddXY(fc.Dates[i], fc.Lower[i], fc.Upper[i]);
            }
            chartLine.Series.Add(band);
            chartLine.Series.Add(predicted);
        }

        // 長條圖：近 30 天每日漲跌幅
        private void UpdateBarChart(List<(DateTime Date, decimal Rate)> series)
        {
            PrepareChart(chartBar);
            chartBar.Legends.Clear();
            var area = NewArea("MM/dd");
            area.AxisY.IsStartedFromZero = true;
            area.AxisY.LabelStyle.Format = "0.00'%'";
            chartBar.ChartAreas.Add(area);
            chartBar.Titles.Add(NewTitle($"{From} → {To}　近 30 天每日漲跌幅（紅漲綠跌）"));

            var bars = new Series("漲跌幅")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.Date,
                ToolTip = "#VALX{yyyy/MM/dd}\n#VALY{0.000}%",
            };
            bars["PointWidth"] = "0.7";
            int start = Math.Max(1, series.Count - 30);
            for (int i = start; i < series.Count; i++)
            {
                decimal pct = (series[i].Rate / series[i - 1].Rate - 1) * 100;
                int idx = bars.Points.AddXY(series[i].Date, pct);
                bars.Points[idx].Color = pct >= 0 ? UpColor : DownColor;
            }
            chartBar.Series.Add(bars);
        }

        // ===== 表格 =====

        private void SetupGrid()
        {
            gridRates.Columns.Clear();
            gridRates.Columns.Add("Date", "日期");
            gridRates.Columns.Add("Rate", "匯率");
            gridRates.Columns.Add("Change", "漲跌");
            gridRates.Columns.Add("Percent", "漲跌幅");
            gridRates.Columns.Add("Kind", "類型");
            foreach (DataGridViewColumn c in gridRates.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            gridRates.Columns["Date"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridRates.Columns["Kind"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridRates.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void UpdateGrid(List<(DateTime Date, decimal Rate)> series, Forecaster.Result? fc)
        {
            gridRates.Columns["Rate"]!.HeaderText = $"匯率（1 {From} = ? {To}）";
            gridRates.Rows.Clear();

            // 新的在上：先放預測，再放實際資料
            var rows = new List<(DateTime Date, decimal Rate, bool Forecast)>();
            rows.AddRange(series.Select(s => (s.Date, s.Rate, false)));
            if (fc != null) rows.AddRange(fc.Dates.Select((d, i) => (d, fc.Forecast[i], true)));

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var (date, rate, isForecast) = rows[i];
                string change = "", pct = "";
                Color color = Color.Black;
                if (i > 0)
                {
                    decimal diff = rate - rows[i - 1].Rate;
                    decimal p = diff / rows[i - 1].Rate * 100;
                    change = (diff >= 0 ? "▲ " : "▼ ") + FormatRate(Math.Abs(diff));
                    pct = $"{p:+0.000;-0.000}%";
                    color = diff >= 0 ? UpColor : DownColor;
                }
                int r = gridRates.Rows.Add($"{date:yyyy/MM/dd} ({Weekday(date)})", FormatRate(rate), change, pct, isForecast ? "ML 預測" : "實際");
                var row = gridRates.Rows[r];
                row.Cells["Change"].Style.ForeColor = color;
                row.Cells["Percent"].Style.ForeColor = color;
                if (isForecast) row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 225);
            }
        }

        // ===== 機器學習：未來 5 天預測與最佳買賣策略 =====

        private void UpdateForecastTab(List<(DateTime Date, decimal Rate)> series, Forecaster.Result? fc, string? error)
        {
            PrepareChart(chartForecast);
            if (fc == null)
            {
                txtStrategy.Text = "機器學習預測失敗：" + error;
                return;
            }

            // 今天 (實際) + 未來 5 天 (預測)
            var (today, todayRate) = series[^1];
            DateTime[] dates = new[] { today }.Concat(fc.Dates).ToArray();
            decimal[] values = new[] { todayRate }.Concat(fc.Forecast).ToArray();
            var strategy = Forecaster.FindStrategy(values);

            // 圖：近 14 天實際 + 5 天預測，標出買點與賣點
            var area = NewArea("MM/dd");
            area.AxisY.LabelStyle.Format = AxisFormat(values);
            chartForecast.ChartAreas.Add(area);
            chartForecast.Titles.Add(NewTitle($"未來 {Forecaster.Horizon} 天預測與買賣點"));

            var recent = new Series("近 14 天實際") { ChartType = SeriesChartType.Line, XValueType = ChartValueType.Date, Color = ActualColor, BorderWidth = 2 };
            foreach (var (d, r) in series.Skip(Math.Max(0, series.Count - 14))) recent.Points.AddXY(d, r);

            var band = new Series("90% 信心區間") { ChartType = SeriesChartType.Range, YValuesPerPoint = 2, XValueType = ChartValueType.Date, Color = Color.FromArgb(50, ForecastColor) };
            var line = new Series("ML 預測")
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.Date,
                Color = ForecastColor,
                BorderWidth = 2,
                BorderDashStyle = ChartDashStyle.Dash,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7,
                ToolTip = "#VALX{MM/dd}\n#VALY",
            };
            band.Points.AddXY(today, todayRate, todayRate);
            line.Points.AddXY(today, todayRate);
            for (int i = 0; i < fc.Dates.Length; i++)
            {
                band.Points.AddXY(fc.Dates[i], fc.Lower[i], fc.Upper[i]);
                line.Points.AddXY(fc.Dates[i], fc.Forecast[i]);
            }

            var marks = new Series("買賣點") { ChartType = SeriesChartType.Point, XValueType = ChartValueType.Date, MarkerSize = 14, IsVisibleInLegend = false };
            if (strategy.BuyIndex >= 0)
            {
                AddMark(marks, dates[strategy.BuyIndex], values[strategy.BuyIndex], $"買入 {To}", UpColor, MarkerStyle.Triangle);
                AddMark(marks, dates[strategy.SellIndex], values[strategy.SellIndex], $"賣回 {From}", DownColor, MarkerStyle.Diamond);
            }
            else
            {
                AddMark(marks, dates[strategy.BestExchangeIndex], values[strategy.BestExchangeIndex], "最佳換匯", UpColor, MarkerStyle.Triangle);
            }

            chartForecast.Series.Add(recent);
            chartForecast.Series.Add(band);
            chartForecast.Series.Add(line);
            chartForecast.Series.Add(marks);

            txtStrategy.Text = BuildReport(dates, values, fc, strategy);
        }

        private static void AddMark(Series s, DateTime x, decimal y, string label, Color color, MarkerStyle style)
        {
            int i = s.Points.AddXY(x, y);
            var p = s.Points[i];
            p.Color = color;
            p.MarkerStyle = style;
            p.Label = label;
            p.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
            p.LabelForeColor = color;
        }

        private string BuildReport(DateTime[] dates, decimal[] values, Forecaster.Result fc, Forecaster.Strategy s)
        {
            decimal amount = decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal a) && a > 0 ? a : 100;
            string Day(int i) => i == 0 ? $"今天 {dates[0]:MM/dd}" : $"{dates[i]:MM/dd} ({Weekday(dates[i])})";

            var sb = new StringBuilder();
            sb.AppendLine("【機器學習模型】");
            sb.AppendLine("ML.NET 時間序列 SSA（奇異譜分析，Singular Spectrum Analysis）");
            sb.AppendLine($"訓練資料：近 {fc.TrainDays} 天 {From}→{To} 每日匯率");
            sb.AppendLine($"回測誤差 MAPE：{fc.BacktestMape:N3}%（用較早的資料預測最近 5 天，與實際值比較）");
            sb.AppendLine();

            sb.AppendLine($"【未來 {Forecaster.Horizon} 天預測】1 {From} = ? {To}");
            sb.AppendLine($"{Day(0)}　{FormatRate(values[0])}（實際）");
            for (int i = 0; i < fc.Dates.Length; i++)
            {
                decimal pct = (fc.Forecast[i] / values[0] - 1) * 100;
                sb.AppendLine($"{Day(i + 1)}　{FormatRate(fc.Forecast[i])}　較今天 {pct:+0.000;-0.000}%　(90% 區間 {FormatRate(fc.Lower[i])} ~ {FormatRate(fc.Upper[i])})");
            }
            sb.AppendLine();

            sb.AppendLine($"【最佳買賣策略】（手上持有 {From}）");
            int best = s.BestExchangeIndex;
            decimal bestGain = (values[best] / values[0] - 1) * 100;
            sb.AppendLine(best == 0
                ? $"▶ 若要把 {From} 換成 {To}：建議今天就換，預測未來幾天匯率都比今天低。"
                : $"▶ 若要把 {From} 換成 {To}：建議在 {Day(best)} 換，預測 1 {From} = {FormatRate(values[best])} {To}，比今天多換 {bestGain:N3}%。");

            if (s.BuyIndex >= 0)
            {
                decimal got = amount * values[s.BuyIndex];
                decimal back = got / values[s.SellIndex];
                sb.AppendLine($"▶ 低買高賣：{Day(s.BuyIndex)} 買入 {To}（匯率最高），{Day(s.SellIndex)} 賣回 {From}（匯率最低）");
                sb.AppendLine($"　 以 {amount:N2} {From} 計算：買入得 {got:N2} {To} → 賣回得 {back:N2} {From}，預期報酬 {s.ProfitPercent:N3}%");
                if (s.ProfitPercent < fc.BacktestMape)
                {
                    sb.AppendLine("　 ⚠ 預期報酬小於模型誤差，訊號偏弱，不建議為了價差頻繁換匯。");
                }
            }
            else
            {
                sb.AppendLine($"▶ 低買高賣：預測期間 {To} 持續走弱，沒有先買後賣的獲利機會，建議不要進行來回操作。");
            }

            sb.AppendLine();
            sb.AppendLine("※ 預測僅供參考，實際交易還需考量銀行買賣價差與手續費。");
            return sb.ToString();
        }

        private void ClearAnalysis(string message)
        {
            foreach (var c in new[] { chartLine, chartBar, chartForecast })
            {
                PrepareChart(c);
                c.Titles.Add(NewTitle(message));
            }
            gridRates.Rows.Clear();
            txtStrategy.Text = message;
        }
    }
}
