using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot
{
    internal class MessageReaction
    {
        private static readonly List<Command>? commands = BotCommands.commadsList;
        private static int whichCommand = WhichCommandHelper.WhichCommand(string.Empty);
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            bool messageFinded = false;

            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            Console.WriteLine(
            $"{message?.From?.FirstName} sent message {message?.Text} " +
            $"to chat {message?.Chat.Id} at {message?.Date}.");

            foreach (var command in commands)
            {
                if (message.Text == "/help")
                {
                    messageFinded = true;
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"{command.Name} - {command.Description}",
                        cancellationToken: cancellationToken);
                }
                if (command.Contains(message))
                {
                    await command.Execute(botClient, message, cancellationToken);
                    messageFinded = true;

                    whichCommand = command.Name switch
                    {
                        "/300iq" => WhichCommandHelper.WhichCommand("Wiki"),
                        "/jokes" => WhichCommandHelper.WhichCommand("Jokes"),
                        "/pets" => WhichCommandHelper.WhichCommand("Pets"),
                        _ => WhichCommandHelper.WhichCommand(string.Empty),
                    };
                    break;
                }
            }
            switch (whichCommand)
            {
                case -1 when messageFinded == false:
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Не знаю такой команды.",
                        cancellationToken: cancellationToken);
                    break;
                case (int)CommandsNames.Wikipedia:
                    await WikipediaCommand.UserReacting(botClient, message, cancellationToken);
                    break;
                case (int)CommandsNames.Joke:
                    await JokesCommand.UserReacting(botClient, message, cancellationToken);
                    break;
                case (int)CommandsNames.Pet:
                    await PetsCommand.UserReacting(botClient, message, cancellationToken);
                    break;
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
