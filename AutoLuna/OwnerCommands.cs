using System;
using System.Diagnostics;
using System.Reflection;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

#pragma warning disable CS8618

namespace AutoLuna
{
    public class OwnerCommands : BaseCommandModule
    {
#if DEBUG
        public class ScriptGlobals
        {
            public CommandContext ctx;
        }

        [Command("eval")]
        public async Task EvalCommand(CommandContext ctx, [RemainingText] string code)
        {
            if (ctx.Member!.Id != 190544080164487168L)
            {
                await ctx.RespondAsync("absolutely not");
                return;
            }

            var outMsg = await ctx.Channel.SendMessageAsync("`Running script...`");
            var sw = new Stopwatch();

            var globals = new ScriptGlobals();
            globals.ctx = ctx;

            var references = new List<Assembly>();
            // Ourselves
            references.Add(Assembly.GetExecutingAssembly());
            references.Add(Assembly.GetAssembly(typeof(DSharpPlus.DiscordClient))!);

            code = code.Replace("```cs", "").Replace("```", "");

            sw.Start();

            try
            {
                var res =
                    await CSharpScript.EvaluateAsync(code,
                        ScriptOptions.Default.WithImports(
                            "System",
                            "System.Math"
                        ).AddReferences(references),
                        globals);
                sw.Stop();
                var outStr = $"Completed in {sw.ElapsedMilliseconds}ms:\n```\n{res ?? "<null>"}```";
                if (outStr.Length >= 2000)
                {
                    var pasteContents = $"{res ?? "<null>"}";
                    var haste = new Hastebin("https://p.kagamine.tech");
                    var pres = await haste.CreatePaste(pasteContents);
                    await outMsg.ModifyAsync(
                        $"Completed in {sw.ElapsedMilliseconds}ms, but message too long.\n" +
                        $"Result here: {pres.URL} ({pasteContents.Length} characters)");
                } else
                {
                    await outMsg.ModifyAsync(outStr);
                }
            }
            catch (Exception ex)
            {
                var outStr = $"caught exception `{ex.GetType().FullName}`:\n" +
                    $"```\n{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}```";
                if (outStr.Length >= 2000)
                {
                    var pasteContents = $"{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}";
                    var haste = new Hastebin("https://p.kagamine.tech");
                    var pres = await haste.CreatePaste(pasteContents);
                    await outMsg.ModifyAsync(
                        $"Caught `{ex.GetType().FullName}` in {sw.ElapsedMilliseconds}ms, but message too long.\n" +
                        $"Result here: {pres.URL} ({pasteContents.Length} characters)");
                }
                else
                {
                    await outMsg.ModifyAsync(outStr);
                }
            }
        }
    }
#endif
}

#pragma warning restore