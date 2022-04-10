using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;

namespace invitational {
    public class EndGameCommand: ModuleBase<SocketCommandContext>
    {
        [Command("end")]
        [Summary("ends the game")]
        public async Task EndGame(int gameId) 
        {
            Game game = Program.instance.games[gameId];

            game.EndGame();
        }

    }
}