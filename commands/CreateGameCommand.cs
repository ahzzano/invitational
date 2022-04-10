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
            Game game = Program.instance.CreateGame();
            
            var message = await Context.Channel.SendMessageAsync("", false, game.GetGameMessage(), components: game.GetMessageComponent());
            
            game.message = message;
        }

    }
}