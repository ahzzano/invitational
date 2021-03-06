using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;
using System.Linq;
namespace invitational
{
    public class PickMapModule : ModuleBase<SocketCommandContext>
    {
        private bool IfTeamTurn(Game game)
        {
            bool isTeam1 = game.GetTeam1().Contains(Context.User) && game.pickBanTurn == Game.PickBanTurn.Team1;
            bool isTeam2 = game.GetTeam2().Contains(Context.User) && game.pickBanTurn == Game.PickBanTurn.Team2;

            return isTeam1 || isTeam2;
        }

        [Command("banm")]
        [Alias("bm")]
        [Summary("Ban a map")]
        public async Task BanMap(string mapName)
        {      
            if(!Program.instance.IfPlayerInGame(Context.User))
                return;

            Game game = Program.instance.GetGameOfPlayer(Context.User);

            if(!IfTeamTurn(game))
                return;

            if(game.pickBanPhase != Game.PickBanPhase.MapBan)
                return;


            if(game.GetMaps().Contains(mapName))
            {
                game.BanMap(mapName);
            }

            await Task.CompletedTask;
        }

        [Command("pickm")]
        [Alias("pm")]
        [Summary("pick a map")]
        public async Task PickMap(string mapName)
        {
            if(!Program.instance.IfPlayerInGame(Context.User))
                return;

            Game game = Program.instance.GetGameOfPlayer(Context.User);

            if(!IfTeamTurn(game))
                return;

            if(game.pickBanPhase != Game.PickBanPhase.MapPick)
                return;

            if(game.GetMaps().Contains(mapName))
            {
                game.PickMap(mapName);
            }       

            await Task.CompletedTask;
        }
    }
}