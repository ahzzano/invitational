using Discord.Commands;
using System.Threading.Tasks;

public class SayModule: ModuleBase<SocketCommandContext> 
{
    [Command("say")]
    public Task SayAsync([Remainder] string echo) => ReplyAsync(echo);
}