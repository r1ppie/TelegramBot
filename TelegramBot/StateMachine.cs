using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using WikiDotNet;

namespace TelegramBot
{
    internal abstract class State
    {
        protected readonly StateMachine stateMachine;
        protected State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
        public virtual Task OnEnterState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public virtual Task StateAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public virtual async Task OnLeaveState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Ладно.",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }
    }
    internal class MainMenuState : State
    {
        public MainMenuState(StateMachine stateMachine) : base (stateMachine) { }
        public override Task OnEnterState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return base.OnEnterState(botClient, message, cancellationToken);
        }
        public override Task StateAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return base.StateAction(botClient, message, cancellationToken);
        }
        public override Task OnLeaveState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
    internal class StartState : State
    {
        public StartState(StateMachine stateMachine) : base(stateMachine) { }
        public override async Task OnEnterState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Привет!!!\n" + "Напиши /help для вывода списка комманд",
                cancellationToken: cancellationToken);
        }
        public override Task StateAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return base.StateAction(botClient, message, cancellationToken);
        }
        public override Task OnLeaveState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
    internal class HelpState : State
    {
        private static readonly List<Command> commands = BotCommands.commadsList;
        public HelpState(StateMachine stateMachine) : base(stateMachine) { }
        public override async Task OnEnterState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            foreach (var command in commands)
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"{command.Name} - {command.Description}",
                    cancellationToken: cancellationToken);
        }
        public override Task StateAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return base.StateAction(botClient, message, cancellationToken);
        }
        public override Task OnLeaveState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
    internal class WikiState : State
    {
        private static readonly WikiCommand wikiCommand = new();
        public WikiState(StateMachine stateMachine) : base(stateMachine) { }
        public override async Task OnEnterState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            KeyboardButtons wikiMenu = new("Wiki");
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{wikiCommand.Description}\n" + "Напиши что мне искать или нажми 'Назад' чтобы закончить.",
                replyMarkup: wikiMenu.menu,
                cancellationToken: cancellationToken);
        }
        public override async Task StateAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
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
        public override Task OnLeaveState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return base.OnLeaveState(botClient, message, cancellationToken);
        }
    }
    internal class JokesState : State
    {
        private static readonly JokesCommand jokesCommand = new();
        public JokesState(StateMachine stateMachine) : base(stateMachine) { }
        public override async Task OnEnterState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            KeyboardButtons jokerMenu = new("JokerMode");
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{jokesCommand.Description}\n" + "Отправь 🤣 для ультрасмеха(кринжа) или 🤬 чтобы закончить.",
                replyMarkup: jokerMenu.menu,
                cancellationToken: cancellationToken);
        }
        public override async Task StateAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            switch (message.Text)
            {
                case "\U0001f923":
                    var jokeTask = GetJokeAsync();
                    string joke = jokeTask.Result;
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: joke,
                        cancellationToken: cancellationToken);
                    break;
                case "\U0001f92c":
                    await OnLeaveState(botClient, message, cancellationToken);
                    break;
                default:
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Нет такой комманды.",
                        cancellationToken: cancellationToken);
                    break;
            }
        }
        public override Task OnLeaveState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return base.OnLeaveState(botClient, message, cancellationToken);
        }
        private static async Task<string> GetJokeAsync()
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
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return takedJoke;
        }
    }
    internal class PetsState : State
    {
        private static readonly PetsCommand petsCommand = new();
        public PetsState(StateMachine stateMachine) : base(stateMachine) { }
        public override async Task OnEnterState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            KeyboardButtons petsMenu = new("PetsMode");
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{petsCommand.Description}\n" + "Отправь 😻 или 🐶 для визуального оргазма, 😿 чтобы закончить.",
                replyMarkup: petsMenu.menu,
                cancellationToken: cancellationToken);
        }
        public override async Task StateAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            switch (message.Text)
            {
                case "😻":
                    var catTask = GetPetPhotoAsync("Cat");
                    string catURL = catTask.Result;
                    await botClient.SendPhotoAsync(
                        chatId: message.Chat.Id,
                        photo: new InputOnlineFile(catURL),
                        cancellationToken: cancellationToken);
                    break;
                case "🐶":
                    var dogTask = GetPetPhotoAsync("Dog");
                    string dogURL = dogTask.Result;
                    await botClient.SendPhotoAsync(
                        chatId: message.Chat.Id,
                        photo: new InputOnlineFile(dogURL),
                        cancellationToken: cancellationToken);
                    break;
                case "😿":
                    await OnLeaveState(botClient, message, cancellationToken);
                    break;
                default:
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Нет такой комманды.",
                        cancellationToken: cancellationToken);
                    break;
            }
        }
        public override Task OnLeaveState(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return base.OnLeaveState(botClient, message, cancellationToken);
        }
        private static async Task<string> GetPetPhotoAsync(string petAttribute)
        {
            string? petURL = "Проблемы с получение питомца, извини.";
            try
            {
                string petJSON;

                var client = new HttpClient();
                HttpResponseMessage response = null;
                switch (petAttribute)
                {
                    case "Cat":
                        response = await client.GetAsync(@"https://api.thecatapi.com/v1/images/search");
                        break;
                    case "Dog":
                        response = await client.GetAsync(@"https://api.thedogapi.com/v1/images/search");
                        break;
                }
                HttpContent content = response.Content;
                using var reader = new StreamReader(await content.ReadAsStreamAsync());
                petJSON = await reader.ReadToEndAsync();

                dynamic? items = JsonConvert.DeserializeObject(petJSON);

                if (items != null)
                    foreach (var item in items)
                    {
                        petURL = Convert.ToString(item.url);
                    }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return petURL;
        }
    }
    internal class StateMachine
    {
        internal State CurrentState { get; set; }
        internal State MainMenuState { get; set; }
        internal State StartState { get; set; }
        internal State HelpState { get; set; }
        internal State WikiState { get; set; }
        internal State JokesState { get; set; }
        internal State PetsState { get; set; }

        public StateMachine() 
        {
            MainMenuState = new MainMenuState(this);
            StartState = new StartState(this);
            HelpState = new HelpState(this);
            WikiState = new WikiState(this);
            JokesState = new JokesState(this);
            PetsState = new PetsState(this);
            CurrentState = MainMenuState;
        }
        internal async Task SwitchStates(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (CurrentState == MainMenuState)
            {
                switch (message.Text)
                {
                    case "/start":
                        await StartState.OnEnterState(botClient, message, cancellationToken);
                        break;
                    case "/help":
                        await HelpState.OnEnterState(botClient, message, cancellationToken);
                        break;
                    case "/300iq":
                        CurrentState = WikiState;
                        await StateEnter(botClient, message, cancellationToken);
                        break;
                    case "/jokes":
                        CurrentState = JokesState;
                        await StateEnter(botClient, message, cancellationToken);
                        break;
                    case "/pets":
                        CurrentState = PetsState;
                        await StateEnter(botClient, message, cancellationToken);
                        break;
                    default:
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Нет такой комманды.",
                            cancellationToken: cancellationToken);
                        break;
                }
            }
            else
            {
                await StateAction(botClient, message, cancellationToken);
            }
        }
        internal async Task StateEnter(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await CurrentState.OnEnterState(botClient, message, cancellationToken);
        }
        internal async Task StateAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message.Text == "\U0001f92c" || message.Text == "Назад" || message.Text == "😿")
            {
                await CurrentState.OnLeaveState(botClient, message, cancellationToken);
                CurrentState = MainMenuState;
            }
            else
                await CurrentState.StateAction(botClient, message, cancellationToken);
        }

    }
}
