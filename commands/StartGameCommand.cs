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
        public async Task StartGame(int gameId) 
        {
            Program.instance.games[gameId].StartGame();
        }

    }
}