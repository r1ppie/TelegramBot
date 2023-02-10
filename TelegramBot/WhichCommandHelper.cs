namespace TelegramBot
{
    internal enum CommandsNames
    {
        Wikipedia,
        Joke,
        Pet
    }
    internal class WhichCommandHelper
    {
        internal static int WhichCommand(string commandName)
        {
            return commandName switch
            {
                "Wiki" => (int)CommandsNames.Wikipedia,
                "Jokes" => (int)CommandsNames.Joke,
                "Pets" => (int)CommandsNames.Pet,
                _ => -1,
            };
        }
    }
}
