using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;

namespace invitational {
    public class EndGameCommand: ModuleBase<SocketCommandContext>
    {
        [Command("end")]
        [Summary("Starts a game")]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task EndGame(int gameId) 
        {
            Game game = Program.instance.CreateGame();
            
            var message = await Context.Channel.SendMessageAsync("", false, game.GetGameMessage());
            
            game.message = message;

            Emoji joinEmote = new Emoji("👍");

            await message.AddReactionAsync(joinEmote);
        }

    }
}