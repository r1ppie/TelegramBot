using System;
using System.Collections.Generic;
using System.Globalization;
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
                case "CatsMode":
                    ReplyKeyboardMarkup catsMenu = new(new[]
{
                        new KeyboardButton[] { "😻", "😿" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                    return catsMenu;
            }
            return null;
        }
    }
}
