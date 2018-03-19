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
        [Command("warn")]
        public async Task Warn(CommandContext ctx, DiscordUser usr, [RemainingText] string reason)
        {
            try
            {
                if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] > 1 || ctx.Member.Id == ctx.Guild.Owner.Id) && usr.Id != ctx.Member.Id)
                {
                    DBConnector.Prepare(usr.Id, ctx.Guild.Id);
                    int count = DBConnector.WarnCount(usr.Id, ctx.Guild.Id) + 1;
                    if (count > 3)
                        count = 1;
                    DBConnector.Warn(usr.Id, ctx.Guild.Id, count);
                    await ctx.RespondAsync(String.Format("Warned: {0}\nReason: {1}\nWarning {2}/3", usr.Mention, reason, count));
                    if (count == 3)
                        await ctx.RespondAsync("User can now be kicked");
                }
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("kick")]
        public async Task Kick(CommandContext ctx, DiscordUser usr, [RemainingText] string reason)
        {
            try
            {
                if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] > 1 || ctx.Member.Id == ctx.Guild.Owner.Id) && usr.Id != ctx.Member.Id)
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
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("ban")]
        public async Task Ban(CommandContext ctx, DiscordUser usr, [RemainingText] string reason)
        {
            try
            {
                if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] > 1 || ctx.Member.Id == ctx.Guild.Owner.Id) && usr.Id != ctx.Member.Id)
                {
                    DBConnector.Prepare(usr.Id, ctx.Guild.Id);
                    int kick = DBConnector.Info(usr.Id, ctx.Guild.Id)[1];
                    if (kick != 3 && (kick % 5) == 0)
                    {
                        await ctx.Guild.GetMemberAsync(usr.Id).Result.BanAsync(0, reason);
                        DBConnector.Ban(usr.Id, ctx.Guild.Id, reason);
                        await ctx.RespondAsync(String.Format("Banned {0} for {1}", usr.Username, reason));
                    }
                    else
                        await ctx.RespondAsync("User does not have number of kicks divisible by 5!");
                }
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
            }
        }

        [Command("clear")]
        public async Task Clear(CommandContext ctx, DiscordUser usr)
        {
            try
            {
                if ((DBConnector.PermCheck(ctx.Member.Id, ctx.Guild.Id)[0] > 1 || ctx.Member.Id == ctx.Guild.Owner.Id) && usr.Id != ctx.Member.Id)
                {
                    DBConnector.Clear(usr.Id, ctx.Guild.Id);
                    await ctx.RespondAsync("Cleared record for " + usr.Mention);
                }
            }
            catch (Exception e)
            {
                ErrorCatch(ctx, e);
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
