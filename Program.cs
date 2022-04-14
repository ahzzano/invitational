using System;
using System.Text.Json;
using System.Collections.Generic;
using Discord;
using Discord.Net; 
using Discord.WebSocket;
using System.Linq;
using Discord.Commands;

using System.Threading.Tasks;

namespace invitational
{
    class SettingsFile {
        public string token {get; set;}
        public int maxPlayers {get; set;}
        public int maxTeams {get; set;}
        public char commandPrefix {get; set;}

        public string queueImage {get; set;}
        public string gameImage {get; set;}
        public string winImage {get; set;}
        public string[] maps {get; set;}
        public int gameMode {get; set;}
        public string mapImage {get; set;}
    }

    class Program
    {

        private DiscordSocketClient _client;
        private CommandService _commands; 
        private CommandHandler _commandHandler; 

        public static Program instance; 
        public List<Game> games = new List<Game>(); 

        public static Task Main(string[] args) {
            
            Program instance = new Program();

            if(Program.instance == null) {
                Program.instance = instance;
            }
            
            return instance.RunBotAsync();
        }

        private void LoadSettings() 
        {
            if(!System.IO.File.Exists("settings.json"))
            {
                string defaultSettings = System.IO.File.ReadAllText("./env/defaultSettings.json");
                System.IO.File.WriteAllText("settings.json", defaultSettings);
                Console.WriteLine("write the Discord token in the settings.json then re-run the program");

                Environment.Exit(0);
            }
        
            string settingsString = System.IO.File.ReadAllText("settings.json");
            SettingsFile settingsValues = JsonSerializer.Deserialize<SettingsFile>(settingsString);

            Settings.Load(settingsValues);   
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

            _commandHandler = new CommandHandler();
            
            await _commandHandler.RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task CLI()
        {
            while(true)
            {
                Console.Write(">>");
                string inputs = Console.ReadLine();
            }
        }

        private Task Log(LogMessage message) 
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public Game CreateGame(SocketGuild guild)
        {
            int gameID = games.Count;

            Game game = new Game(gameID, guild);
            games.Add(game);
            return game;
        }

        public void EndGame(int id, int teamWinner)
        {
            Game game = games[id];
            game.EndGame(teamWinner);
            games.RemoveAt(id);
        }


        public bool IfPlayerInGame(SocketUser player)
        {
            foreach(Game game in games)
            {
                if(game.GetPlayers().Contains(player))
                {
                    return true;
                }
            }

            return false; 
        }

        public Game GetGameOfPlayer(SocketUser player)
        {
            foreach(Game game in games)
            {
                if(game.GetPlayers().Contains(player))
                {
                    return game;
                }
            }

            return null;            
        }

        public EmbedFooterBuilder GetBotFooter()
        {
            EmbedFooterBuilder footer = new EmbedFooterBuilder();

            footer.WithText("[source](https://github.com/ahzzano/invitational.git)");

            return footer;
        }
    }

}
