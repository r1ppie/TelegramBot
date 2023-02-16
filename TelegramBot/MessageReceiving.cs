using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TelegramBot
{
    internal class MessageReceiving
    {
        private static readonly Dictionary<long, StateMachine> userStateMachine = new();
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            Console.WriteLine(
            $"{message?.From?.FirstName} sent message {message?.Text} " +
            $"to chat {message?.Chat.Id} at {message?.Date}.");

            if (!userStateMachine.ContainsKey(message.Chat.Id))
            {
                userStateMachine.Add(message.Chat.Id, new StateMachine());
                await userStateMachine[message.Chat.Id].SwitchStates(botClient, message, cancellationToken);
            }
            else
            {
                await userStateMachine[message.Chat.Id].SwitchStates(botClient, message, cancellationToken);
            }
        }
        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _ = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.Error.WriteLine(exception);
            return Task.CompletedTask;
        }
    }
}
