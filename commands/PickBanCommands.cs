using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;
using System.Linq;
namespace invitational
{
    public class PickMapModule : ModuleBase<SocketCommandContext>
    {
        [Command("banm")]
        [Summary("Ban a map")]
        public async Task BanMap(string mapName)
        {       
            if(!Program.instance.IfPlayerInGame(Context.User))
                return;

            Game game = Program.instance.GetGameOfPlayer(Context.User);

            bool isTeam1 = game.GetTeam1().Contains(Context.User) && game.pickBanPhase == Game.PickBanPhase.Team1;
            bool isTeam2 = game.GetTeam2().Contains(Context.User) && game.pickBanPhase == Game.PickBanPhase.Team2;

            if(!(isTeam1 || isTeam2))
                return; 
            

            if(game.GetMaps().Contains(mapName))
            {
                game.BanMap(mapName);
            }

            await Task.CompletedTask;
        }
    }
}