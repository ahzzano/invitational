using System;
using System.Text.Json;
using Discord;
using Discord.Net; 
using Discord.WebSocket;
using Discord.Commands;

using System.Threading.Tasks;

namespace invitational
{
    class SettingsFile {
        public string token {get; set;}
        public int maxPlayers {get; set;}
        public int maxTeams {get; set;}
    }

    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands; 

        public static Program instance; 

        public static Task Main(string[] args) {
            

            Program instance = new Program();

            if(Program.instance == null) {
                Program.instance = instance;
            }
            
            return instance.RunBotAsync();
        }

        private void LoadSettings() {

            // Load the settings.json file
            string settingsString = System.IO.File.ReadAllText("settings.json");
            SettingsFile settingsValues = JsonSerializer.Deserialize<SettingsFile>(settingsString);

            // Instantiate the Settings

            Settings.Load(settingsValues);   
        }

        public static DiscordSocketClient GetClient() 
        {
            return instance._client;
        }

        public async Task RunBotAsync()
        {
            LoadSettings();

            _client = new DiscordSocketClient();
            _client.Log += Log;

            string token = Settings.GetDiscordToken();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage message) 
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}
