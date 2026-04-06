using Core.Game;

namespace Features.BotBalancer.AI
{
    public static class BotMathExtensions
    {
        /// <summary>
        /// Расчет ROI для лота в магазине (с учетом множителя Quality)
        /// </summary>
        public static float GetRoi(this StoreLot lot, FishConfig baseConfig)
        {
            var actualIncome = baseConfig.IncomeCoins * lot.Quality;
            var actualLifetime = baseConfig.LifetimeSeconds * lot.Quality;
            var actualPrice = lot.Price == 0 ? 1 : lot.Price;

            var expectedTotalIncome = actualIncome * (actualLifetime / baseConfig.IncomeCooldownSeconds);

            return expectedTotalIncome / actualPrice;
        }

        /// <summary>
        /// Расчет ROI для базового конфига или живой рыбы (без Quality)
        /// </summary>
        public static float GetRoi(this FishConfig config)
        {
            var expectedTotalIncome = config.IncomeCoins * (config.LifetimeSeconds / config.IncomeCooldownSeconds);
            var actualPrice = config.Price == 0 ? 1 : config.Price;

            return expectedTotalIncome / actualPrice;
        }
    }
}