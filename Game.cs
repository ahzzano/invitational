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
        public int id;

        private int numberOfPlayers = 0;

        public Game(int id) 
        {
            componentBuilder = new ComponentBuilder();
            
            players = new SocketUser[maxPlayers];

            OnGameCreate();
        }

        public void EndGame()
        {
            if(completed == true)
                return;

            completed = true; 
            OnGameEnd();
        }

        public void StartGame()
        {
            if(started == true)
                return;

            started = true;

            OnGameStart();
        }

        private async void OnGameEnd()
        {
            Program.GetClient().ReactionAdded -= OnReactionAdded;
            Program.GetClient().ReactionRemoved -= OnReactionRemoved;
            await message.DeleteAsync();
        
        }

        private async void OnGameStart()
        {
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
                .AddField("Team1", GetQueueString())
                .AddField("Team2", GetQueueString())
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

        
    }
}