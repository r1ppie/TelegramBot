using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
    internal abstract class Command
    {
        internal abstract string Name { get; }
        internal abstract string Description { get; }
        internal virtual bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }
    }
    internal class StartCommand : Command
    {
        internal override string Name => @"/start";
        internal override string Description => @"Приветствие.";
    }
    internal class HelpCommand : Command
    {
        internal override string Name => @"/help";
        internal override string Description => @"Помощь.";
    }
    internal class WikiCommand : Command
    {
        internal override string Name => "/300iq";
        internal override string Description => "Поиск на википедии.";
    }
    internal class JokesCommand : Command
    {
        internal override string Name => "/jokes";
        internal override string Description => "Выводит анекдоты.\n" + "Все еще лучше чем российская стэнд-ап комедия.";
    }
    internal class PetsCommand : Command
    {
        internal override string Name => "/pets";
        internal override string Description => "Рандомные фотки котов или собак.\n" + "Mode: Умиление.";
    }
}
