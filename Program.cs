using System;
using System.Text.Json;
using System.Collections.Generic;
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
        public char commandPrefix {get; set;}
    }

    class Program
    {

        private DiscordSocketClient _client;
        private CommandService _commands; 

        public static Program instance; 
        public List<Game> games = new List<Game>(); 

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

        public void AddGame(Game game) 
        {
            games.Add(game);
        }

        public static DiscordSocketClient GetClient() => instance._client;
        public static CommandService GetCommandService() => instance._commands;
        public static bool IsIstantiated() {
            if(instance == null) 
                return false;

            else 
                return true;   
        }

        public async Task RunBotAsync()
        {
            LoadSettings();

            _client = new DiscordSocketClient();
            _client.Log += Log;

            _commands = new CommandService();

            string token = Settings.GetDiscordToken();

            CommandHandler commandHandler = new CommandHandler();

            await commandHandler.RegisterCommandsAsync();

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
