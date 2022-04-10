using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;

namespace invitational {
    public class CreateGameCommand: ModuleBase<SocketCommandContext>
    {
        [Command("start")]
        [Summary("Starts a game")]
        public async Task CreateGame() 
        {
            Game game = new Game();
            Program.instance.AddGame(game);

            EmbedBuilder embed = new EmbedBuilder() 
            {
                Color = Color.Green
            };

            embed.AddField("Game 1", "Here is Game 1")
                .AddField("Team 1", "Player1\nPlayer2\nPlayer3\nPlayer4\nPlayer5")
                .AddField("Team 2", "Player1\nPlayer2\nPlayer3\nPlayer4\nPlayer5")
                .WithCurrentTimestamp()
                .WithImageUrl("https://c.tenor.com/CYJS0cjte98AAAAC/sengoku-nadeko-hitagi-end.gif");

            game.embeddedMessage = embed; 

            await Context.Channel.SendMessageAsync("", false, embed.Build());
            

        }

    }
}