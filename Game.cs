using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Commands;
using Discord.Net;
using System.Linq;

namespace invitational {

    class Game {
        public int maxPlayers = 10; 
        public bool completed = false;
        public bool started = false; 
        public RestUserMessage message;
        private ComponentBuilder componentBuilder;
        private SocketUser[] players; 

        private SocketUser[] team1;
        private SocketUser[] team2;

        public int id;


        private int numberOfPlayers = 0;

        public Game(int id) 
        {
            componentBuilder = new ComponentBuilder();
            
            players = new SocketUser[maxPlayers];

            OnGameCreate();
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
            if(started == true)
                return;

            started = true;

            OnGameStart();
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

            embed.AddField("Congratulations to", winner);

            await message.Channel.SendMessageAsync("", false, embed.Build());
        }

        private async void OnGameStart()
        {
            team1 = new SocketUser[(int) players.Length / 2];
            team2 = new SocketUser[players.Length - ((int) players.Length / 2) + 1];

            int team1Index = 0;
            int team2Index = 0;

            for(int i=0; i < numberOfPlayers; i++)
            {
                Random random = new Random();
                int choice = random.Next(0,2);

                if(players[i] == null)
                    continue;
                
                

                if(choice == 0 || team1Index < team1.Length) 
                {
                    team1[team1Index] = players[i];
                    team1Index++;
                }
                else 
                {
                    team2[team2Index] = players[i];
                    team2Index++;
                }
            }

           await message.RemoveAllReactionsAsync(); 
           UpdateGameMessage();
        }

        private async void OnGameCreate()
        {
            Program.GetClient().ReactionAdded += OnReactionAdded;
            Program.GetClient().ReactionRemoved += OnReactionRemoved;
        }

        public async Task OnReactionAdded(Cacheable<IUserMessage, ulong> _, Cacheable<IMessageChannel, ulong> __, SocketReaction reaction)
        {
            if(reaction.MessageId != message.Id || ((SocketUser) reaction.User).IsBot)
            {
                return;
            }

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

        public async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> _, Cacheable<IMessageChannel, ulong> __, SocketReaction reaction)
        {
            if(reaction.MessageId != message.Id || ((SocketUser) reaction.User).IsBot)
            {
                return;
            }

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

        public async void UpdateGameMessage()
        {
            await message.ModifyAsync(delegate(MessageProperties properties) {properties.Embed = GetGameMessage();});
        }

        public async void UpdateQueueMessage()
        {
            await message.ModifyAsync(delegate(MessageProperties properties) {properties.Embed = GetQueueMessage();});
        }
        

        public Embed GetQueueMessage() 
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.Green
            };

            embed.AddField($"Game {id}", $"Game #{id} has been initiated, Join or Leave")
                .AddField("Queue", GetQueueString())
                .WithCurrentTimestamp()
                .WithImageUrl(Settings.instance.queueImage);

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