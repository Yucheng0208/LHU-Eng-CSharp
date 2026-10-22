using System.Text.Json;

namespace Bonus_02
{
    // 匯率資料來源：fawazahmed0/currency-api (免費、免金鑰、含新台幣與每日歷史資料)
    // 所有匯率都以 1 USD 為基準，任兩種貨幣的匯率用交叉匯率換算
    public class RateService
    {
        private static readonly HttpClient http = new() { Timeout = TimeSpan.FromSeconds(15) };

        // 日期 → (貨幣代碼 → 1 USD 可換多少該貨幣)
        public SortedDictionary<DateTime, Dictionary<string, decimal>> History { get; } = new();
        public Dictionary<string, decimal> Latest { get; private set; } = new();
        public DateTime LatestDate { get; private set; }
        public DateTime FetchedAt { get; private set; }

        // 即時匯率：主站失敗時改用備援網址
        public async Task LoadLatestAsync()
        {
            string[] urls =
            {
                "https://latest.currency-api.pages.dev/v1/currencies/usd.min.json",
                "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/usd.min.json",
            };
            Exception? last = null;
            foreach (string url in urls)
            {
                try
                {
                    (LatestDate, Latest) = await FetchAsync(url);
                    FetchedAt = DateTime.Now;
                    return;
                }
                catch (Exception ex) { last = ex; }
            }
            throw new InvalidOperationException("無法取得即時匯率，請確認網路連線。", last);
        }

        // 過去 days 天的每日匯率，同時最多 8 個請求
        public async Task LoadHistoryAsync(int days)
        {
            using var gate = new SemaphoreSlim(8);
            var tasks = Enumerable.Range(1, days).Select(async i =>
            {
                DateTime date = DateTime.Today.AddDays(-i);
                await gate.WaitAsync();
                try
                {
                    string d = date.ToString("yyyy-MM-dd");
                    var (_, rates) = await FetchAsync($"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@{d}/v1/currencies/usd.min.json");
                    return (date, rates);
                }
                catch
                {
                    return (date, (Dictionary<string, decimal>?)null);   // 個別日期抓不到就略過
                }
                finally { gate.Release(); }
            });

            foreach (var (date, rates) in await Task.WhenAll(tasks))
            {
                if (rates != null) History[date] = rates;
            }
        }

        private static async Task<(DateTime, Dictionary<string, decimal>)> FetchAsync(string url)
        {
            await using var stream = await http.GetStreamAsync(url);
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;
            DateTime date = DateTime.Parse(root.GetProperty("date").GetString()!);
            var rates = new Dictionary<string, decimal>();
            foreach (var p in root.GetProperty("usd").EnumerateObject())
            {
                if (p.Value.ValueKind == JsonValueKind.Number && p.Value.TryGetDecimal(out decimal v) && v > 0)
                {
                    rates[p.Name.ToUpperInvariant()] = v;
                }
            }
            return (date, rates);
        }

        // 1 單位 from 可換多少 to
        public static decimal Cross(Dictionary<string, decimal> usdRates, string from, string to) =>
            usdRates[to] / usdRates[from];

        public decimal LatestRate(string from, string to) => Cross(Latest, from, to);

        // 歷史匯率序列，最後一筆接上即時匯率
        public List<(DateTime Date, decimal Rate)> Series(string from, string to)
        {
            var list = History
                .Where(h => h.Key < LatestDate && h.Value.ContainsKey(from) && h.Value.ContainsKey(to))
                .Select(h => (h.Key, Cross(h.Value, from, to)))
                .ToList();
            if (Latest.Count > 0)
            {
                list.Add((LatestDate, LatestRate(from, to)));
            }
            return list;
        }
    }
}
