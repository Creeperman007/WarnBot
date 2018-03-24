using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;

namespace WarnBot
{
    class Commands
    {
        [Command("warn"), Description("Warns a user.")]
        public async Task Warn(CommandContext ctx, [Description("Mention of user.")] DiscordUser usr, [RemainingText, Description("Reason")] string reason)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] >= 1 || ctx.Member.IsOwner) && usr.Id != ctx.Member.Id)
                    {
                        DBConnector.Prepare(usr.Id, ctx.Guild.Id);
                        int count = DBConnector.WarnCount(usr.Id, ctx.Guild.Id) + 1;
                        if (count > 3)
                            count = 1;
                        DBConnector.Warn(usr.Id, ctx.Guild.Id, count);
                        await ctx.RespondAsync(String.Format("Warned: {0}\nReason: {1}\nWarning {2}/3", usr.Mention, reason, count));
                        if (count == 3)
                            await ctx.RespondAsync("User can now be kicked!");
                    }
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("kick"), Description("Kicks a person.")]
        public async Task Kick(CommandContext ctx, [Description("Mention of user.")] DiscordUser usr, [RemainingText, Description("Reason")] string reason)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] >= 1 || ctx.Member.IsOwner) && usr.Id != ctx.Member.Id)
                    {
                        DBConnector.Prepare(usr.Id, ctx.Guild.Id);
                        int info = DBConnector.Info(usr.Id, ctx.Guild.Id)[0];
                        if (info == 3)
                        {
                            await ctx.Guild.GetMemberAsync(usr.Id).Result.RemoveAsync(reason);
                            DBConnector.Kick(usr.Id, ctx.Guild.Id, reason);
                            await ctx.RespondAsync(String.Format("Kicked {0} for {1}", usr.Username, reason));
                        }
                        else
                            await ctx.RespondAsync("User does not have three warnings yet!");
                    }
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("ban"), Description("Bans a person.")]
        public async Task Ban(CommandContext ctx, [Description("Mention of user.")] DiscordUser usr, [RemainingText, Description("Reason")] string reason)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] >= 1 || ctx.Member.IsOwner) && usr.Id != ctx.Member.Id)
                    {
                        DBConnector.Prepare(usr.Id, ctx.Guild.Id);
                        int kick = DBConnector.Info(usr.Id, ctx.Guild.Id)[1];
                        if (kick != 0 && (kick % 5) == 0)
                        {
                            await ctx.Guild.GetMemberAsync(usr.Id).Result.BanAsync(0, reason);
                            DBConnector.Ban(usr.Id, ctx.Guild.Id, reason);
                            await ctx.RespondAsync(String.Format("Banned {0} for {1}", usr.Username, reason));
                        }
                        else
                            await ctx.RespondAsync("User does not have number of kicks divisible by 5!");
                    }
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }
        
        [Command("clear"), Description("Clears warnings count.")]
        public async Task Clear(CommandContext ctx, [Description("Mention of user.")] DiscordUser usr)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] >= 1 || ctx.Member.IsOwner) && usr.Id != ctx.Member.Id)
                    {
                        DBConnector.Clear(usr.Id, ctx.Guild.Id);
                        await ctx.RespondAsync("Cleared record for " + usr.Mention);
                    }
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("info"), Description("Shows warnings and kicks.")]
        public async Task Info(CommandContext ctx, [Description("Mention of user.")] DiscordUser usr)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    DBConnector.Prepare(usr.Id, ctx.Guild.Id);
                    int[] info = DBConnector.Info(usr.Id, ctx.Guild.Id);
                    await ctx.RespondAsync(String.Format("Warnings: {0}\nKicks: {1}", info[0], info[1]));
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("addusr"), Description("Adds user to Admins."), RequirePermissions(Permissions.Administrator), Aliases("adduser")]
        public async Task AddUsr(CommandContext ctx, [Description("Mention of user.")] DiscordUser usr, [Description("K - Kick or KB - Kick and Ban.")] string perms)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    DBConnector.AddUsr(usr.Id, ctx.Guild.Id, perms);
                    await ctx.RespondAsync("Added user to Admins.");
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("rmusr"), Description("Removes user from Admins."), RequirePermissions(Permissions.Administrator), Aliases("removeuser")]
        public async Task RmUsr(CommandContext ctx, [Description("Mention of user.")] DiscordUser usr)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    DBConnector.RmUsr(usr.Id, ctx.Guild.Id);
                    await ctx.RespondAsync("Removed user from Admins.");
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("updateusr"), Description("Updates permissions."), RequirePermissions(Permissions.Administrator), Aliases("updateuser")]
        public async Task UpdateUsr(CommandContext ctx, [Description("Mention of user.")] DiscordUser usr, [Description("K - Kick or KB - Kick and Ban.")] string perms)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    DBConnector.UpdateUsr(usr.Id, ctx.Guild.Id, perms);
                    await ctx.RespondAsync("Updated permissions for user.");
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("about"), Description("About this bot")]
        public async Task About(CommandContext ctx)
        {
            var eb = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#FF0000"),
                Title = "About",
                Description = "Programmed by Creeperman007\nUsing DSharpPlus library\nGitHub repository: <http://github.com/Creeperman007/WarnBot>"
            };
            await ctx.RespondAsync(embed: eb);
        }

        [Command("check"), Hidden]
        public async Task Check(CommandContext ctx, DiscordUser usr)
        {
            try
            {
                if (!usr.IsCurrent && (usr.Id != ctx.Guild.Owner.Id))
                {
                    if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] >= 1 || ctx.Member.IsOwner) && usr.Id != ctx.Member.Id)
                    {
                        DBConnector.Prepare(usr.Id, ctx.Guild.Id);
                        int[] info = DBConnector.Info(usr.Id, ctx.Guild.Id);
                        await ctx.Member.SendMessageAsync(String.Format("Admin check for {0}#{1} from **{2}**\nCurrent warnings: {3}\nTotal warning: {4}\nKicks: {5}", usr.Username, usr.Discriminator, ctx.Guild.Name, info[0], info[2], info[1]));
                    }
                }
                else
                    await ctx.RespondAsync("Whoah, you can't use the command on this person!");
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("example"), Description("Shows example usage of specified command.")]
        public async Task Example(CommandContext ctx, [Description("Command to show example of (without prefix)")] string command)
        {
            switch (command)
            {
                case "ban":
                    await ctx.RespondAsync(String.Format("```/ban @{0}#{1} Spamming```", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "kick":
                    await ctx.RespondAsync(String.Format("```/kick @{0}#{1} Spamming```", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "warn":
                    await ctx.RespondAsync(String.Format("```/warn @{0}#{1} Spamming```", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "clear":
                    await ctx.RespondAsync(String.Format("```/clear @{0}#{1}```", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "info":
                    await ctx.RespondAsync(String.Format("```/info @{0}#{1}```", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "addusr":
                    await ctx.RespondAsync(String.Format("```/addusr @{0}#{1} K``` /\\ for kick only\nOR\n```/addusr @{0}#{1} KB``` /\\ for kick and ban", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "rmusr":
                    await ctx.RespondAsync(String.Format("```/rmusr @{0}#{1}```", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "updateusr":
                    await ctx.RespondAsync(String.Format("```/updateusr @{0}#{1} K``` /\\ for kick only\nOR\n```/updateusr @{0}#{1} KB``` /\\ for kick and ban", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "example":
                    await ctx.RespondAsync(String.Format("```/example kick```", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                case "check":
                    await ctx.RespondAsync(String.Format("```/check @{0}#{1}```", ctx.Member.Username, ctx.Member.Discriminator));
                    break;
                default:
                    await ctx.RespondAsync("This is not existing command, or it does not need any arguments :confused:");
                    break;
            }
        }

        private async void ErrorCatch(CommandContext ctx, Exception e)
        {
            await ctx.Member.SendMessageAsync("Internal error occured!");
            await ctx.Member.SendMessageAsync("Go to <https://github.com/Creeperman007/WarnBot/issues> open new issue and paste text below and how you got this error");
            await ctx.Member.SendMessageAsync("```" + e + "```");
        }
    }
}
