using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    internal class KeyboardButtons
    {
        internal static ReplyKeyboardMarkup? KeyboardCreating(string setUp)
        {
            switch (setUp)
            {
                case "JokerMode":
                    ReplyKeyboardMarkup jokerMenu = new(new[]
                    {
                        new KeyboardButton[] {"🤣", "🤬"}
                    })
                    {
                        ResizeKeyboard = true
                    };
                    return jokerMenu;
                case "Wiki":
                    ReplyKeyboardMarkup wikiMenu = new(new[]
                    {
                        new KeyboardButton[] {"Назад"}
                    })
                    {
                        ResizeKeyboard = true
                    };
                    return wikiMenu;
                case "PetsMode":
                    ReplyKeyboardMarkup petsMenu = new(new[]
{
                        new KeyboardButton[] { "😻", "🐶", "😿" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                    return petsMenu;
            }
            return null;
        }
    }
}
