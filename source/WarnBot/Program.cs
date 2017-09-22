using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WarnBot
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var client = new DiscordSocketClient();

            client.Log += Log;
            client.MessageReceived += MessageReceived;

            string token = File.ReadAllText(@"token.txt"); // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await client.SetGameAsync("/help");
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage msg)
        {
            try
            {
                if (msg.Content.Contains("/") && !msg.Content.Contains("`"))
                {
                    string[] msgSplit = msg.ToString().Split(' ');
                    string cmd = msgSplit[0];
                    string user = "";
                    string context = "";
                    string usr2ulong = "";
                    string[] replace = { "<", ">", "@" };
                    try
                    {
                        user = msgSplit[1];
                        usr2ulong = user;
                        for (int i = 2; i < msgSplit.Length; i++)
                        {
                            context += msgSplit[i] + " ";
                        }

                        foreach (string e in replace)
                        {
                            usr2ulong = usr2ulong.Replace(e, "");
                        }
                    }
                    catch { }
                    var chnl = msg.Channel as SocketGuildChannel;
                    /*Console.WriteLine(chnl.Guild.Id);
                    var echo = chnl.Guild.Owner.Id;
                    Console.WriteLine(msg.MentionedUsers.ToString());
                    var client = new DiscordSocketClient();*/
                    //var role = (msg.Author as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Role");
                    switch (cmd)
                    {
                        case "/warn":
                            try
                            {
                                if (DBConnector.PermCheck(msg.Author.Id, chnl.Guild.Id)[0] >= 1 || msg.Author.Id == chnl.Guild.Owner.Id)
                                {
                                    DBConnector.Prepare(user, chnl.Guild.Id);
                                    int count = DBConnector.WarnCount(user, chnl.Guild.Id) + 1;
                                    if (count > 3)
                                    {
                                        count = 1;
                                    }
                                    DBConnector.Warn(user, chnl.Guild.Id, count);
                                    await msg.Channel.SendMessageAsync("Warned: " + user + "\nReason: " + context + "\nWarning " + count + "/3");
                                    if (count == 3)
                                    {
                                        await msg.Channel.SendMessageAsync("User now can be kicked!");
                                    }
                                }
                                else
                                {
                                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " you don't have permission for this action");
                                }
                            }
                            catch (Exception e)
                            {
                                await msg.Channel.SendMessageAsync("Internal error occured. Cannot warn user!");
                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                await msg.Author.SendMessageAsync("```" + e + "```");
                            }
                            break;
                        case "/kick":
                            try
                            {
                                if (DBConnector.PermCheck(msg.Author.Id, chnl.Guild.Id)[0] >= 1 || msg.Author.Id == chnl.Guild.Owner.Id)
                                {
                                    DBConnector.Prepare(user, chnl.Guild.Id);
                                    int[] info = DBConnector.Info(user, chnl.Guild.Id);
                                    if (info[0] == 3)
                                    {
                                        if (context != "")
                                        {
                                            try
                                            {
                                                await chnl.GetUser(Convert.ToUInt64(usr2ulong)).KickAsync(context);
                                                DBConnector.Kick(user, chnl.Guild.Id, context);
                                                await msg.Channel.SendMessageAsync("Kicked " + user + " for \"" + context + "\"");
                                            }
                                            catch (Exception e)
                                            {

                                                Console.WriteLine(e);
                                                await msg.Channel.SendMessageAsync("Could not kick user :confused:");
                                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                                await msg.Author.SendMessageAsync("```" + e + "```");
                                            }
                                        }
                                        else
                                        {
                                            await msg.Channel.SendMessageAsync("You can't kick without a reason!");
                                        }
                                    }
                                    else
                                    {
                                        await msg.Channel.SendMessageAsync("User does not have 3 warnings yet!");
                                    }
                                }
                                else
                                {
                                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " you don't have permission for this action");
                                }
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(e);
                                await msg.Channel.SendMessageAsync("Internal error occured. Cannot kick user!");
                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                await msg.Author.SendMessageAsync("```" + e + "```");
                            }
                            break;
                        case "/ban":
                            try
                            {
                                if (DBConnector.PermCheck(msg.Author.Id, chnl.Guild.Id)[1] >= 1 || msg.Author.Id == chnl.Guild.Owner.Id)
                                {
                                    if (context != "")
                                    {
                                        int kick = DBConnector.Info(user, chnl.Guild.Id)[1];
                                        if (kick != 0 && (kick % 5) == 0)
                                        {
                                            await chnl.Guild.AddBanAsync(Convert.ToUInt64(usr2ulong), 0, context);
                                            await msg.Channel.SendMessageAsync("Banned " + user + " for \"" + context + "\"");
                                        }
                                        else
                                        {
                                            await msg.Channel.SendMessageAsync("User does not have number of kicks divisible by 5!");
                                        }
                                    }
                                    else
                                    {
                                        await msg.Channel.SendMessageAsync("You can't ban without a reason!");
                                    }
                                }
                                else
                                {
                                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " you don't have permission for this action");
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                await msg.Channel.SendMessageAsync("Internal error occured. Cannot ban user!");
                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                await msg.Author.SendMessageAsync("```" + e + "```");
                            }
                            break;
                        case "/clear":
                            try
                            {
                                if (DBConnector.PermCheck(msg.Author.Id, chnl.Guild.Id)[1] >= 1 || msg.Author.Id == chnl.Guild.Owner.Id)
                                {
                                    DBConnector.Clear(user, chnl.Guild.Id);
                                    await msg.Channel.SendMessageAsync("Cleared record for " + user);
                                }
                                else
                                {
                                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " you don't have permission for this action");
                                }
                            }
                            catch (Exception e)
                            {
                                await msg.Channel.SendMessageAsync("Internal error occured. Cannot clear user!");
                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                await msg.Author.SendMessageAsync("```" + e + "```");
                            }
                            break;
                        case "/info":
                            try
                            {
                                DBConnector.Prepare(user, chnl.Guild.Id);
                                int[] info = DBConnector.Info(user, chnl.Guild.Id);
                                await msg.Channel.SendMessageAsync("Warnings: " + info[0] + "\nKicks: " + info[1]);
                            }
                            catch (Exception e)
                            {
                                await msg.Channel.SendMessageAsync("Internal error occured. Cannot clear user!");
                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                await msg.Author.SendMessageAsync("```" + e + "```");
                            }
                            break;
                        case "/help":
                            await msg.Channel.SendMessageAsync("```Everyone:\n/info <user>......................Shows warnings and kicks\n\nAdmins:\n/ban <user> <reason>..............Bans person\n/clear <user>.....................Clears warning count\n/kick <user> <reason>.............Kicks person\n/warn <user> <reason[optional]>...Give person warning\n\nOwner:\n/addusr <user> <K|KB>.............Adds user to Admins\n/rmusr <user> <K|KB>..............Remove user from Admins\n/updateusr <user> <K|KB> .........Updates permissions for user\n..................................K=Kick, KB=Kick and Ban```");
                            break;
                        case "/addusr":
                            try
                            {
                                if (msg.Author.Id == chnl.Guild.Owner.Id)
                                {
                                    DBConnector.AddUsr(Convert.ToUInt64(usr2ulong), chnl.Guild.Id, context);
                                    await msg.Channel.SendMessageAsync("Added user to Admins.");
                                }
                                else
                                {
                                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " you don't have permission for this action");
                                }
                            }
                            catch (Exception e)
                            {
                                await msg.Channel.SendMessageAsync("Internal error occured. Cannot clear user!");
                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                await msg.Author.SendMessageAsync("```" + e + "```");
                            }
                            break;
                        case "/rmusr":
                            try
                            {
                                if (msg.Author.Id == chnl.Guild.Owner.Id)
                                {
                                    DBConnector.RmUsr(Convert.ToUInt64(usr2ulong), chnl.Guild.Id);
                                    await msg.Channel.SendMessageAsync("Removed user from Admins.");
                                }
                                else
                                {
                                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " you don't have permission for this action");
                                }
                            }
                            catch (Exception e)
                            {
                                await msg.Channel.SendMessageAsync("Internal error occured. Cannot clear user!");
                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                await msg.Author.SendMessageAsync("```" + e + "```");
                            }
                            break;
                        case "/updateusr":
                            try
                            {
                                if (msg.Author.Id == chnl.Guild.Owner.Id)
                                {
                                    DBConnector.UpdateUsr(Convert.ToUInt64(usr2ulong), chnl.Guild.Id, context);
                                    await msg.Channel.SendMessageAsync("Updated permissions for user.");
                                }
                                else
                                {
                                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " you don't have permission for this action");
                                }
                            }
                            catch (Exception e)
                            {
                                await msg.Channel.SendMessageAsync("Internal error occured. Cannot clear user!");
                                await msg.Author.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below");
                                await msg.Author.SendMessageAsync("```" + e + "```");
                            }
                            break;
                        case "/about":
                            var eb = new EmbedBuilder();
                            eb.WithColor(Color.Red);
                            eb.WithTitle("About");
                            eb.WithDescription("Programmed by Creeperman007\nUsing Discord.Net library\nGitHub repository: <http://github.com/Creeperman007/WarnBot>");
                            await msg.Channel.SendMessageAsync("", false, eb);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
    }
}