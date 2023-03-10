using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;


namespace TelegramBot
{
    internal class BotSettings
    {
        internal static string? Key { get; set; }
    }
    internal class Startup
    {
        internal static async Task Main()
        {
            using StreamReader reader = new("botToken.txt");
            string token = await reader.ReadToEndAsync();
            BotSettings.Key = token;
            var botClient = new TelegramBotClient(BotSettings.Key);
            var cancellationToken = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: MessageReceiving.HandleUpdateAsync,
                pollingErrorHandler: MessageReceiving.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationToken.Token);

            var me = await botClient.GetMeAsync();

            Console.Title = me.Username;
            Console.WriteLine($"Bot {me.Username} started\n" + "Press any key to stop");
            Console.ReadLine();
            cancellationToken.Cancel();
            Console.WriteLine("Bot stopped");
        }
    }
}
