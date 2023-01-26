using Genbox.Wikipedia;
using Genbox.Wikipedia.Enums;
using Genbox.Wikipedia.Objects;
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
                text: $"{Description}\nя бесполезный хуила, надеюсь у моего еблана создателя хватит" +
                    " мотивации поднять жопу и сделать хоть что-то полезное.",
                replyMarkup: KeyboardButtons.KeyboardCreating("Main"),
                cancellationToken: cancellationToken);
        }
    }
    internal class WikipediaCommand : Command
    {
        internal override string Name => "300iq";
        internal override string Description => "Поиск на википедии.";
        internal override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }
        internal override async Task Execute(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            //string[] redactedMessage = message.Text.Split(" ");
            //WikipediaClient wikipediaClient = new();
            //WikiSearchRequest request = new(redactedMessage[1])
            //{
            //    WikiLanguage = WikiLanguage.Russian,
            //    Limit = 1
            //};
            //WikiSearchResponse resp = await wikipediaClient.SearchAsync(request);
            //foreach (SearchResult result in resp.QueryResult.SearchResults)
            //{
            //    await botClient.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: $"",
            //        cancellationToken: cancellationToken);
            //}
        }
    }
    internal class JokesCommand : Command
    {
        internal override string Name => "Внимание, АНЕКДОТ";

        internal override string Description => "Все еще лучше чем российская стэнд-ап комедия.";

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
                text: $"{Description}\nОтправь 🤣 для ультрасмеха(кринжа) или 🤬 чтобы закончить.",
                replyMarkup: KeyboardButtons.KeyboardCreating("JokerMode"),
                cancellationToken: cancellationToken);
        }

        internal static async Task UserReactionWaiting(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            string sendthis = null;
            try
            {
                Random rnd = new();
                string fullfile;
                using StreamReader reader = new("Ловушка Джокушкера.txt");

                fullfile = await reader.ReadToEndAsync();
                string[] jokes = fullfile.Split("~");

                var jokeindex = rnd.Next(0, jokes.Length);
                sendthis = jokes[jokeindex];
                
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            switch (message.Text)
            {
                case "\U0001f923":
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"{sendthis}",
                        cancellationToken: cancellationToken);
                    break;
                case "\U0001f92c":
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Ладно.",
                        replyMarkup: KeyboardButtons.KeyboardCreating("Main"),
                        cancellationToken: cancellationToken);
                    MessageReaction.whichcommand = -1;
                    break;
            }
        }
    }
}
