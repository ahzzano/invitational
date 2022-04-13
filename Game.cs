using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Commands;
using System.Collections;
using System.Collections.Generic;
using Discord.Net;
using System.Linq;

namespace invitational {

    class Game {

        public enum GamePhase {
            PickBan,
            InGame
        }

        public enum PickBanTurn
        {
            Team1,
            Team2
        }
        
        public enum PickBanPhase
        {
            MapBan,
            MapPick
        }

        public int maxPlayers = Settings.instance.maxPlayers; 
        public bool completed = false;
        public bool started = false; 
        private bool joinable = true;
        public Emoji joinGameEmote = new Emoji("👍");
        public RestUserMessage gameMessage;
        public RestUserMessage mapsMessage; 
        private ComponentBuilder componentBuilder;
        private SocketUser[] players; 
        private List<string> availableMaps = Settings.instance.maps.ToList();
        private SocketUser[] team1;
        private SocketUser[] team2;
        private List<string> team1Picks = new List<string>();
        private List<string> team2Picks = new List<string>();
        private string decider; 
        private int availableBans; 
        public GamePhase gamePhase;
        public PickBanTurn pickBanTurn;
        public PickBanPhase pickBanPhase;
        private int nextPickPhase;
        public int id;
        private int numberOfPlayers = 0;

        public Game(int id) 
        {
            componentBuilder = new ComponentBuilder();
            
            players = new SocketUser[maxPlayers];
            
            this.id = id;
            
            availableBans = availableMaps.Count - Settings.instance.gameType;
            nextPickPhase = availableBans - 2;

            CreateGame();
        }

        private async void CreateGame()
        {
            await OnGameCreate();
        }

        public void EndGame(int teamWinner)
        {
            if(completed == true)
                return;

            completed = true; 
            OnGameEnd(teamWinner);
        }

        public void StartGame()
        {
            gamePhase = GamePhase.PickBan;
            pickBanTurn = PickBanTurn.Team1;
            joinable = false; 

            if(started == true)
                return;

            OnGameStart();

            started = true;
        }

        private async void OnGameEnd(int teamWinner)
        {
            Program.GetClient().ReactionAdded -= OnReactionAdded;
            Program.GetClient().ReactionRemoved -= OnReactionRemoved;
            
            string winner = "";

            if(teamWinner == 0)
                winner = "Team 1";
            else
                winner = "Team 2";

            EmbedBuilder embed = new EmbedBuilder();

            embed.AddField($"{winner} Wins the Game", "lol")
                .AddField("Members", teamWinner == 0 ? GetTeam1String() : GetTeam2String())
                .WithImageUrl(Settings.instance.winImage)
                .WithCurrentTimestamp();

            await gameMessage.Channel.SendMessageAsync("", false, embed.Build());
        }

        private async void OnGameStart()
        {
            await gameMessage.Channel.SendMessageAsync("Assigning Teams");
            AssignTeam();

            await gameMessage.Channel.SendMessageAsync("Pick/Ban Phase is Starting");

            mapsMessage = (RestUserMessage) await gameMessage.Channel.SendMessageAsync("", false, GetMapPoolMessage());

            await PickBan();

            await gameMessage.Channel.SendMessageAsync("Game Starting, GLHF!");
        }

        private async Task PickBan()
        {
            while(gamePhase == GamePhase.PickBan) {}

            await Task.CompletedTask;
        }

        public async void PickMap(string mapName)
        {
            if(availableMaps.Remove(mapName))
            {      
                if(pickBanTurn == PickBanTurn.Team1)
                {
                    await gameMessage.Channel.SendMessageAsync($"Team 1 Picked {mapName}");
                    team1Picks.Add(mapName);
                    pickBanTurn = PickBanTurn.Team2;
                }
                else {
                    pickBanTurn = PickBanTurn.Team1;
                    team2Picks.Add(mapName);
                    await gameMessage.Channel.SendMessageAsync($"Team 2 Picked {mapName}");
                }

                if(availableMaps.Count <= 1)
                {
                    await gameMessage.Channel.SendMessageAsync($"Last Map Available is {availableMaps[0]}");
                    gamePhase = GamePhase.InGame;
                }

                UpdateMapMessage();

                availableBans--;

                if(availableBans == nextPickPhase && availableMaps.Count > 1)
                {
                    await mapsMessage.Channel.SendMessageAsync("Ban Phase");
                    pickBanPhase = PickBanPhase.MapBan;
                    
                    nextPickPhase -= 2;
                }

                
            }
            else 
            {
                await gameMessage.Channel.SendMessageAsync("Map Name Invalid");
            }
        }

        public async void BanMap(string mapName)
        {
            if(availableMaps.Remove(mapName))
            {
               
                if(pickBanTurn == PickBanTurn.Team1)
                {
                    await gameMessage.Channel.SendMessageAsync($"Team 1 Banned {mapName}");
                    pickBanTurn = PickBanTurn.Team2;
                }
                else {
                    pickBanTurn = PickBanTurn.Team1;
                    await gameMessage.Channel.SendMessageAsync($"Team 2 Banned {mapName}");
                }

                if(availableMaps.Count <= 1)
                {
                    await gameMessage.Channel.SendMessageAsync($"Last Map Available is {availableMaps[0]}");

                    if(Settings.instance.gameType > 1)
                    {
                        decider = mapName;
                    }

                    gamePhase = GamePhase.InGame;

                    return;
                }    

                availableBans--;

                if(Settings.instance.gameType > 1)
                {
                    if(availableBans == nextPickPhase && availableMaps.Count > 1 && availableBans > 0 && nextPickPhase > 0)
                    {
                        await mapsMessage.Channel.SendMessageAsync("Pick Phase");
                        pickBanPhase = PickBanPhase.MapPick;

                        nextPickPhase -= 2;
                    }

                    if(availableBans <= 0 && availableMaps.Count > 1 && nextPickPhase > 0)
                    {
                        pickBanPhase = PickBanPhase.MapPick;
                        await mapsMessage.Channel.SendMessageAsync("Pick Phase");
                    }
                }
            
                UpdateMapMessage();
            }
            else 
            {
                await gameMessage.Channel.SendMessageAsync("Map Name Invalid");
            }
        }

        public SocketUser[] GetTeam1() => team1;
        public SocketUser[] GetTeam2() => team2;
        public SocketUser[] GetPlayers() => players;
        public string[] GetMaps() => availableMaps.ToArray();
        public PickBanTurn GetPickBanPhase() => pickBanTurn;
        public string GetMapPool() => String.Join(", ", availableMaps);

        private async Task OnGameCreate()
        {
            Program.GetClient().ReactionAdded += OnReactionAdded;
            Program.GetClient().ReactionRemoved += OnReactionRemoved;

            await Task.CompletedTask;
        }

        public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> _, Cacheable<IMessageChannel, ulong> __, SocketReaction reaction)
        {
            if(reaction.MessageId != gameMessage.Id || ((SocketUser) reaction.User).IsBot)
            {
                return;
            }

            if(reaction.Emote.Name == joinGameEmote.Name && joinable == true)
            {
                SocketUser user = (SocketUser) reaction.User;

                for(int i=0; i < maxPlayers; i++)
                {
                    if(players[i] == null)
                    {
                        players[i] = user;
                        break;
                    }
                }

                numberOfPlayers++;

                UpdateQueueMessage();
            }

            await Task.CompletedTask;
        }

        public async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> _, Cacheable<IMessageChannel, ulong> __, SocketReaction reaction)
        {
            if(reaction.MessageId != gameMessage.Id || ((SocketUser) reaction.User).IsBot)
            {
                return;
            }

            if(reaction.Emote.Name == joinGameEmote.Name && joinable == true)
            {
                SocketUser user = (SocketUser) reaction.User;

                if(players.Contains(user))
                {
                    for(int i=0; i < maxPlayers; i++)
                    {
                        if(players[i] == user)
                        {
                            players[i] = null;
                            numberOfPlayers--;
                            break;
                        }
                    }
                }

                UpdateQueueMessage();
            }
            await Task.CompletedTask;
        }

        private async void AssignTeam()
        {
            team1 = new SocketUser[(int) players.Length / 2];
            team2 = new SocketUser[players.Length - ((int) players.Length / 2)];

            int team1Index = 0;
            int team2Index = 0;

            for(int i=0; i < (int) (maxPlayers / 2); i += 2)
            {
                Random random = new Random();
                int choice = random.Next(0,2);     

                if(choice == 0) 
                {
                    if(players[i] != null)
                        team1[team1Index] = players[i];
                    
                    if(players[i+1] != null)
                        team2[team2Index] = players[i + 1];
                }
                else 
                {
                    if(players[i] != null)
                        team2[team2Index] = players[i];
                    
                    if(players[i+1] != null)
                        team1[team1Index] = players[i + 1];
                }

                team1Index++;
                team2Index++;
            }

           await gameMessage.RemoveAllReactionsAsync(); 
           UpdateGameMessage();
        }

        public async void UpdateGameMessage()
        {
            await gameMessage.ModifyAsync(delegate(MessageProperties properties) {properties.Embed = GetGameMessage();});
        }

        public async void UpdateQueueMessage()
        {
            await gameMessage.ModifyAsync(delegate(MessageProperties properties) {properties.Embed = GetQueueMessage();});
        }

        public async void UpdateMapMessage()
        {
            await mapsMessage.ModifyAsync(delegate(MessageProperties properties) {properties.Embed = GetMapPoolMessage();});
        }
        public Embed GetQueueMessage() 
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.Green
            };

            embed.AddField($"Game {id}", $"Game #{id} has been initiated, Join or Leave")
                .AddField("Queue", GetQueueString())
                .AddField("Maps", GetMapPool())
                .WithCurrentTimestamp()
                .WithImageUrl(Settings.instance.queueImage);

            return embed.Build();
        }
        
        public Embed GetMapPoolMessage()
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.Red
            };

            if(Settings.instance.gameType > 1)
            {
                embed.AddField("Available Maps", GetMapPool())
                    .AddField("Team 1 Picks", GetTeam1PicksString())
                    .AddField("Team 2 Picks", GetTeam2PicksString())
                    .WithImageUrl(Settings.instance.mapImage)
                    .WithCurrentTimestamp();
            }
            else 
            {
                embed.AddField("Available Maps", GetMapPool())
                    .WithImageUrl(Settings.instance.mapImage)
                    .WithCurrentTimestamp();
            }

            return embed.Build();
        }

        public Embed GetGameMessage()
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.Blue
            };

            embed.AddField($"Game {id}", $"Game #{id} is now starting")
                .AddField("Team1", GetTeam1String())
                .AddField("Team2", GetTeam2String())
                .AddField("Maps", GetMapPool())
                .AddField("How To Map Pick/Ban", "Type !banm <mapName> to ban a map\nType !pickm to pick a map")
                .WithCurrentTimestamp()
                .WithImageUrl(Settings.instance.gameImage);

            return embed.Build();            
        }

        private string GetQueueString()
        {
            string queue = "";

            for(int i=0; i < maxPlayers; i++)
            {
                if(players[i] == null)
                    continue;

                queue += $"{players[i].Username}\n";
            }

            if(queue == "")
            {
                queue = "No one is here";
            }

            return queue;
        }

        private string GetTeam1String()
        {
            string queue = "";

            for(int i=0; i < team1.Length; i++)
            {
                if(team1[i] == null)
                    continue;

                queue += $"{team1[i].Username}\n";
            }

            if(queue == "")
            {
                queue = "No one is here";
            }

            return queue;   
        }

        private string GetTeam1PicksString()
        {
            string maps = "";

            if(team1Picks.Count == 0)
                maps = "None";

            else
                maps = String.Join(", ", team1Picks);
            
            return maps;
        }

        private string GetTeam2PicksString()
        {
            string maps = "";

            if(team2Picks.Count == 0)
                maps = "None";

            else
                maps = String.Join(", ", team2Picks);
            
            return maps;
        }

        private string GetTeam2String()
        {
            string queue = "";

            for(int i=0; i < team2.Length; i++)
            {
                if(team2[i] == null)
                    continue;

                queue += $"{team2[i].Username}\n";
            }

            if(queue == "")
            {
                queue = "No one is here";
            }

            return queue;   
        }
    }
}