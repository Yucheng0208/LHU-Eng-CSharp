using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace Bonus_02
{
    // 機器學習：用 ML.NET 的 SSA (奇異譜分析) 時間序列模型預測未來匯率，再找出最佳買賣日
    public class Forecaster
    {
        public const int Horizon = 5;   // 預測未來幾天

        private class RateInput { public float Value { get; set; } }

        private class RateForecast
        {
            public float[] Forecast { get; set; } = Array.Empty<float>();
            public float[] Lower { get; set; } = Array.Empty<float>();
            public float[] Upper { get; set; } = Array.Empty<float>();
        }

        public class Result
        {
            public DateTime[] Dates = Array.Empty<DateTime>();
            public decimal[] Forecast = Array.Empty<decimal>();
            public decimal[] Lower = Array.Empty<decimal>();
            public decimal[] Upper = Array.Empty<decimal>();
            public decimal BacktestMape;     // 回測：用前段資料預測最後 5 天的平均誤差 (%)
            public int TrainDays;
        }

        public static Result Predict(IReadOnlyList<(DateTime Date, decimal Rate)> series)
        {
            if (series.Count < 30) throw new InvalidOperationException("歷史資料不足 30 天，無法訓練模型。");

            // 正規化：除以平均值，避免像 JPY→USD (0.006) 這種很小的數字讓 float 失去精度
            decimal scale = series.Average(s => s.Rate);
            float[] values = series.Select(s => (float)(s.Rate / scale)).ToArray();

            // 回測：拿掉最後 5 天訓練，再和真實值比較
            var test = Run(values[..^Horizon]);
            decimal mape = 0;
            for (int i = 0; i < Horizon; i++)
            {
                float actual = values[values.Length - Horizon + i];
                mape += (decimal)Math.Abs((test.Forecast[i] - actual) / actual);
            }

            var f = Run(values);
            DateTime last = series[^1].Date;
            return new Result
            {
                Dates = Enumerable.Range(1, Horizon).Select(i => last.AddDays(i)).ToArray(),
                Forecast = f.Forecast.Select(v => (decimal)v * scale).ToArray(),
                Lower = f.Lower.Select(v => (decimal)v * scale).ToArray(),
                Upper = f.Upper.Select(v => (decimal)v * scale).ToArray(),
                BacktestMape = mape / Horizon * 100,
                TrainDays = values.Length,
            };
        }

        private static RateForecast Run(float[] values)
        {
            var ml = new MLContext(seed: 1);
            var data = ml.Data.LoadFromEnumerable(values.Select(v => new RateInput { Value = v }));
            var pipeline = ml.Forecasting.ForecastBySsa(
                outputColumnName: nameof(RateForecast.Forecast),
                inputColumnName: nameof(RateInput.Value),
                windowSize: 7,                      // 以一週為週期找規律
                seriesLength: Math.Min(30, values.Length),
                trainSize: values.Length,
                horizon: Horizon,
                confidenceLevel: 0.90f,
                confidenceLowerBoundColumn: nameof(RateForecast.Lower),
                confidenceUpperBoundColumn: nameof(RateForecast.Upper));
            var model = pipeline.Fit(data);
            var engine = model.CreateTimeSeriesEngine<RateInput, RateForecast>(ml);
            return engine.Predict();
        }

        public class Strategy
        {
            public int BuyIndex;       // 0 = 今天，1~5 = 預測日
            public int SellIndex;
            public decimal ProfitPercent;
            public int BestExchangeIndex;
        }

        // rates[0] 是今天的即時匯率，rates[1..] 是預測值；匯率 = 1 單位 from 可換多少 to
        // 買入 to 貨幣要挑匯率高的日子 (同樣的 from 換到最多 to)，
        // 之後再賣回 from 要挑匯率低的日子 (to 比較值錢)，報酬 = 買入匯率 / 賣出匯率 - 1
        public static Strategy FindStrategy(decimal[] rates)
        {
            var s = new Strategy { BuyIndex = -1, SellIndex = -1, ProfitPercent = 0 };
            for (int i = 0; i < rates.Length; i++)
            {
                for (int j = i + 1; j < rates.Length; j++)
                {
                    decimal p = (rates[i] / rates[j] - 1) * 100;
                    if (p > s.ProfitPercent)
                    {
                        s.ProfitPercent = p;
                        s.BuyIndex = i;
                        s.SellIndex = j;
                    }
                }
            }
            s.BestExchangeIndex = Array.IndexOf(rates, rates.Max());
            return s;
        }
    }
}
