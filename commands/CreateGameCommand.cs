using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;

namespace invitational {
    public class CreateGameCommand: ModuleBase<SocketCommandContext>
    {
        [Command("create")]
        [Summary("creates a game")]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task CreateGame() 
        {
            Game game = Program.instance.CreateGame();

            var message = await Context.Channel.SendMessageAsync("", false, game.GetQueueMessage());
            
            game.gameMessage = message;

            await message.AddReactionAsync(game.joinGameEmote);
        }

    }
}