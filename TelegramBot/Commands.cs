﻿using WikiDotNet;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBot
{
    internal abstract class Command
    {
        internal abstract string Name { get; }
        internal abstract string Description { get; }
        internal abstract Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
        internal abstract bool Contains(Message message);

        internal KeyboardButtons buttonCreating = new();
    }
    internal class StartCommand : Command
    {
        internal override string Name => @"/start";
        internal override string Description => @"Приветствие.";
        internal override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }
        internal override async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Привет!!!\n" + "Напиши /help для вывода списка комманд",
                cancellationToken: cancellationToken);
        }
    }
    internal class WikipediaCommand : Command
    {
        internal override string Name => "/300iq";
        internal override string Description => "Поиск на википедии.";
        internal override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }
        internal override async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{Description}\n" + "Напиши что мне искать или нажми 'Назад' чтобы закончить.",
                replyMarkup: KeyboardButtons.KeyboardCreating("Wiki"),
                cancellationToken: cancellationToken);
        }
        internal static async Task UserReacting(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message.Text == "Назад")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Ладно.",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
                MessageReaction.whichcommand = -1;
            }
            else
            {
                if (!message.Text.StartsWith("/"))
                {
                    string searchString = message.Text;
                    WikiSearcher searcher = new();
                    WikiSearchSettings searchSettings = new() { ResultLimit = 1, Language = "ru" };

                    WikiSearchResponse response = searcher.Search(searchString, searchSettings);

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Найдены результаты:",
                        cancellationToken: cancellationToken);

                    foreach (WikiSearchResult result in response.Query.SearchResults)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: $"{result.Preview}",
                            replyMarkup: new InlineKeyboardMarkup(
                                InlineKeyboardButton.WithUrl(
                                    text: "Статья на вики.",
                                    url: $"{result.ConstantUrl}")),
                            cancellationToken: cancellationToken);
                    }
                }
            }
        }
    }
    internal class JokesCommand : Command
    {
        internal override string Name => "/jokes";

        internal override string Description => "Выводит анекдоты.\n" + "Все еще лучше чем российская стэнд-ап комедия.";

        internal override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }

        internal override async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{Description}\n" + "Отправь 🤣 для ультрасмеха(кринжа) или 🤬 чтобы закончить.",
                replyMarkup: KeyboardButtons.KeyboardCreating("JokerMode"),
                cancellationToken: cancellationToken);
        }
        internal static async Task UserReacting(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (!message.Text.StartsWith("/"))
            {
                string takedJoke = "Шутка не нашлась :(";
                try
                {
                    Random rnd = new();
                    string fullfile;
                    using StreamReader reader = new("jokes.txt");

                    fullfile = await reader.ReadToEndAsync();
                    string[] jokes = fullfile.Split("~");

                    var jokeindex = rnd.Next(0, jokes.Length);
                    takedJoke = jokes[jokeindex];

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                switch (message.Text)
                {
                    case "\U0001f923":
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: $"{takedJoke}",
                            cancellationToken: cancellationToken);
                        break;
                    case "\U0001f92c":
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Ладно.",
                            replyMarkup: new ReplyKeyboardRemove(),
                            cancellationToken: cancellationToken);
                        MessageReaction.whichcommand = -1;
                        break;
                }
            }
        }
    }
}
