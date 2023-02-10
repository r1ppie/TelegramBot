using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    internal class KeyboardButtons
    {
        internal readonly ReplyKeyboardMarkup menu = new(new[]
        {
            Array.Empty<KeyboardButton>()
        })
        {
            ResizeKeyboard = true
        };
        public KeyboardButtons(string setUp)
        {
            switch (setUp)
            {
                case "JokerMode":
                    menu = new(new[]
                    {
                        new KeyboardButton[] {"🤣", "🤬"}
                    })
                    {
                        ResizeKeyboard = true
                    };                   
                    break;
                case "Wiki":
                    menu = new(new[]
                    {
                        new KeyboardButton[] {"Назад"}
                    })
                    {
                        ResizeKeyboard = true
                    };
                    break;
                case "PetsMode":
                    menu = new(new[]
{
                        new KeyboardButton[] { "😻", "🐶", "😿" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                    break;
            }
        }
        public KeyboardButtons() { }
    }
}
