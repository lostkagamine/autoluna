using System;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Linq;
using DSharpPlus.Entities;

namespace AutoLuna
{
    public class MassCommands : BaseCommandModule
    {
        async Task<List<DiscordMember>> GetAllToKick(DiscordGuild g, long after, ulong currentId)
        {
            var time = DateTime.UnixEpoch.AddSeconds(after);
            var mems = await g.GetAllMembersAsync();
            var afterJoin = mems.Where(x => x.JoinedAt > time).Where(x => x.Id != currentId);
            return afterJoin.ToList();
        }

        [Command("spy")]
        public async Task SpyCommand(CommandContext ctx, long after = 1655729820L)
        {
            if (!ctx.Member!.Roles.Any(x => x.Id == 974318115427057694UL || x.Id == 602443969145602048UL))
            {
                await ctx.RespondAsync("no");
                return;
            }

            Console.WriteLine("doing it now");
            var afterJoin = await GetAllToKick(ctx.Guild!, after, ctx.Client.CurrentUser.Id);
            var outp = $"=== Compiled list of {afterJoin.Count()} members: ===\n";
            foreach (var m in afterJoin)
            {
                outp += $"{m.Username}#{m.Discriminator} [{m.Id}] - joined {m.JoinedAt}\n";
            }
            var haste = new Hastebin("https://p.kagamine.tech");
            var paste = await haste.CreatePaste(outp);
            await ctx.RespondAsync($"okay: {paste.URL}");
        }

        [Command("lever")]
        public async Task LeverCommand(CommandContext ctx, long after = 1655729820L)
        {
            if (!ctx.Member!.Roles.Any(x => x.Id == 974318115427057694UL || x.Id == 602443969145602048UL))
            {
                await ctx.RespondAsync("no");
                return;
            }

            Console.WriteLine("lever's been pulled");
            var afterJoin = await GetAllToKick(ctx.Guild!, after, ctx.Client.CurrentUser.Id);
            var outp = $"=== Kicked members: {afterJoin.Count()} ===\n";
            foreach (var m in afterJoin)
            {
                try
                {
                    await m.RemoveAsync($"Anti-raid prevention measure initiated by {ctx.Member.Username}#{ctx.Member.Discriminator}");
                    outp += $"ok: {m.Username}#{m.Discriminator} [{m.Id}] - joined {m.JoinedAt}\n";
                } catch(Exception e)
                {
                    outp += $"NG: {m.Username}#{m.Discriminator} [{m.Id}] - joined {m.JoinedAt}\n";
                    Console.WriteLine($"failed to kick {m.Username}#{m.Discriminator}");
                    Console.WriteLine(e.Message);
                }
            }
            var haste = new Hastebin("https://p.kagamine.tech");
            var paste = await haste.CreatePaste(outp);
            await ctx.RespondAsync($"{afterJoin.Count()} members kicked: {paste.URL}");
        }
    }
}