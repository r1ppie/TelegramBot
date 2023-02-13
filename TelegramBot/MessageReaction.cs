using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TelegramBot
{
    internal class MessageReaction
    {
        private static readonly StateMachine stateMachine = new();
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            Console.WriteLine(
            $"{message?.From?.FirstName} sent message {message?.Text} " +
            $"to chat {message?.Chat.Id} at {message?.Date}.");

            await stateMachine.SwitchStates(botClient, message, cancellationToken);
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
