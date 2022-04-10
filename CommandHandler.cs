using System;
using System.Text.Json;
using Discord;
using Discord.Net; 
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;

using System.Threading.Tasks;

namespace invitational {
    public class CommandHandler {
        
        public CommandHandler() 
        {
            if(!Program.IsIstantiated())
                throw new ProgramNotInstantiated("");
        }

        public async Task RegisterCommandsAsync()
        {
            await Program.GetCommandService().AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
            Program.GetClient().MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg) 
        {
            var client = Program.GetClient();
            var commands = Program.GetCommandService();

            SocketUserMessage message = arg as SocketUserMessage;

            if(message == null)
                return;
            
            int argPos = 0;

            if(!message.HasCharPrefix(Settings.instance.commandPrefix, ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.Author.IsBot)
                return;
            
            var context = new SocketCommandContext(client, message);

            await commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }

    }
}