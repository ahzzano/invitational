using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace invitational {
    public class CreateGame: ModuleBase<SocketCommandContext> 
    {
        [Command("start")]
        [Summary("Starts a game")]
        public async Task CreateGameCommand() 
        {
            Game game = new Game();
            Program.instance.AddGame(game);

            EmbedBuilder embed = new EmbedBuilder();

            embed.AddField("Field Title", "")
                .AddField("Team 1", "Player1\nPlayer2\nPlayer3\nPlayer4\nPlayer5")
                .AddField("Team 2", "Player1\nPlayer2\nPlayer3\nPlayer4\nPlayer5")
                .WithCurrentTimestamp();

            //IUserMessage message = ReplyAsync(embed: embed.Build()) as IUserMessage;
            await Context.Channel.SendMessageAsync(embed: embed.Build());
            //game.message = message;

            //return Task.CompletedTask;
        }
    }
}