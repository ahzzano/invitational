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

            foreach(CommandInfo command in commands)
            {
                string embedFieldText = command.Summary ?? "No description available";

                embed.AddField(command.Name, embedFieldText);

            }
            
            await Context.Channel.SendMessageAsync("Hi", false, embed.Build());
        }
    }
}