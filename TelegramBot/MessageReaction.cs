﻿using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot
{
    enum Commands
    {
        Wikipedia,
        Joke,
        Pet
    }
    internal class MessageReaction
    {
        internal static List<Command>? commands = BotCommands.commadsList;
        internal static int whichCommand = -1;
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
                    switch (command.Name)
                    {
                        case "/300iq":
                            whichCommand = (int)Commands.Wikipedia;
                            break;
                        case "/jokes":
                            whichCommand = (int)Commands.Joke;
                            break;
                        case "/pets":
                            whichCommand = (int)Commands.Pet;
                            break;
                    }
                    break;
                }
            }
            if (messageFinded == false && whichCommand == -1)
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Не знаю такой команды.",
                    cancellationToken: cancellationToken);
            else if (whichCommand == (int)Commands.Wikipedia)
                await WikipediaCommand.UserReacting(botClient, message, cancellationToken);
            else if (whichCommand == (int)Commands.Joke)
                await JokesCommand.UserReacting(botClient, message, cancellationToken);
            else if (whichCommand == (int)Commands.Pet)
                await PetsCommand.UserReacting(botClient, message, cancellationToken);
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
