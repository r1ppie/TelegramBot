﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Polling;
using System.Net.Security;

namespace TelegramBot
{
    enum WhichCommand 
    {
        Wikipedia,
        Joke,
        Cat
    }
    internal class MessageReaction
    {
        internal static List<Command> commands = BotCommands.GetCommands();
        internal static int whichcommand = -1;
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            bool finded = false;

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
                    finded = true;
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"{command.Name} - {command.Description}",
                        cancellationToken: cancellationToken);
                }
                if (command.Contains(message))
                {
                    await command.Execute(botClient, message, cancellationToken);
                    finded = true;
                    switch (command.Name)
                    {
                        case "/300iq":
                            whichcommand = (int)WhichCommand.Wikipedia;
                            break;
                        case "/jokes":
                            whichcommand = (int)WhichCommand.Joke;
                            break;
                        case "/cats":
                            whichcommand = (int)WhichCommand.Cat;
                            break;
                    }
                    break;
                }
            }
            if (finded == false && whichcommand == -1)
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Не знаю такой команды.",
                    cancellationToken: cancellationToken);
            if (whichcommand == 0)
                await WikipediaCommand.UserReacting(botClient, message, cancellationToken);
            else if (whichcommand == 1)
                await JokesCommand.UserReacting(botClient, message, cancellationToken);
            else if (whichcommand == 2)
                await CatsCommand.UserReacting(botClient, message, cancellationToken);
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
