using PumpClient.PumpServiceReference;
using System;

namespace PumpClient
{
    internal class CallbackHandler : IPumpServiceCallback
    {
        public void UpdateStatistics(StatisticsService statisticsService)
        {
            Console.Clear();

            Console.WriteLine("Обновим статистику");
            Console.WriteLine($"Всего тактов: {statisticsService.AllTacts}");
            Console.WriteLine($"Успешных тактов: {statisticsService.SuccessTacts}");
            Console.WriteLine($"Ошибочных тактов: {statisticsService.ErrorTacts}");
        }
    }
}
