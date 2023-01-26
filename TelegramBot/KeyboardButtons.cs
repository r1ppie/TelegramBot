using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    internal class KeyboardButtons
    {
        internal static ReplyKeyboardMarkup? KeyboardCreating(string setUp)
        {
            switch (setUp)
            {
                case "Main":
                    ReplyKeyboardMarkup mainMenu = new(new[]
                    {
                        new KeyboardButton[] { "Внимание, АНЕКДОТ", "300iq", "Прощай!"},
                    })
                    {
                        ResizeKeyboard = true
                    };
                    return mainMenu;
                case "JokerMode":
                    ReplyKeyboardMarkup jokerMenu = new(new[]
                    {
                        new KeyboardButton[] { "🤣", "🤬" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                    return jokerMenu;
            }
            return null;
        }
    }
}
