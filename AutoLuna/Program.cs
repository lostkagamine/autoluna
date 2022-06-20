using System;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using dotenv.net;

namespace AutoLuna
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DotEnv.Load();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
            });
            
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "]" }
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}