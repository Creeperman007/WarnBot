using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;

namespace WarnBot
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        static async Task MainAsync(string[] args)
        {
            DiscordClient discord = new DiscordClient(new DiscordConfiguration
            {
                Token = File.ReadAllText("token.txt"),
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Info
            });

            CommandsNextModule commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "/"
            });

            commands.CommandExecuted += Commands_CommandExecuted;

            commands.RegisterCommands<Commands>();

            discord.Ready += Client_Ready;
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "WarnBot", "Client is ready to operate.", DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "WarnBot", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
