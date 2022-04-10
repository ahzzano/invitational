using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;

namespace invitational {
    public class CreateGameCommand: ModuleBase<SocketCommandContext>
    {
        [Command("start")]
        [Summary("Starts a game")]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task CreateGame(string teamSelection="random") 
        {
            Game game = Program.instance.CreateGame();
            
            var message = await Context.Channel.SendMessageAsync("", false, game.GetGameMessage());
            
            game.message = message;

            Emoji joinEmote = new Emoji("👍");

            await message.AddReactionAsync(joinEmote);
        }

    }
}