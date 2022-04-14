using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace invitational 
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("sends a help message")]
        public async Task SendHelp() 
        {
            List<CommandInfo> commands = Program.GetCommandService().Commands as List<CommandInfo>;

            EmbedBuilder embed = new EmbedBuilder();

            embed.AddField("Invitationals Bot", "I am the invitational bot, here are my commands")
                .AddField("Commands", 
                    $"{Settings.instance.commandPrefix}create ~ creates game\n" +
                    $"{Settings.instance.commandPrefix}start <game-id> ~ starts the corresponding game\n" + 
                    $"{Settings.instance.commandPrefix}banm <map-name> ~ bans a map\n" + 
                    $"{Settings.instance.commandPrefix}pickm <map-name> ~ picks a map\n" 
                )
                .WithFooter(Program.instance.GetBotFooter());
            
            await Context.Channel.SendMessageAsync("Hi", false, embed.Build());
        }
    }
}