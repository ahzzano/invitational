using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;

namespace invitational {
    public class StartGameCommand: ModuleBase<SocketCommandContext>
    {
        [Command("start")]
        [Summary("starts the game")]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task StartGame() 
        {
            Game game = Program.instance.CreateGame();
            
            var message = await Context.Channel.SendMessageAsync("", false, game.GetQueueMessage());
            
            game.message = message;

            Emoji joinEmote = new Emoji("👍");

            await message.AddReactionAsync(joinEmote);
        }

    }
}