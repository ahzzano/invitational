using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Commands;
using Discord.Net;

namespace invitational {
    class Game {
        public int maxPlayers = 10; 
        public bool completed = false;
        public RestUserMessage message;
        private ComponentBuilder componentBuilder;
        private SocketUser[] players; 
        public int id;

        private int numberOfPlayers = 0;

        public Game(int id) 
        {
            componentBuilder = new ComponentBuilder();

            componentBuilder.WithButton("Join", $"join-button-{id}")
                .WithButton("Leave", $"leave-button-{id}");
            
            Program.GetClient().ButtonExecuted += ButtonHandler;

            players = new SocketUser[maxPlayers];
        }

        public void OnGameEnd()
        {
            Program.GetClient().ButtonExecuted -= ButtonHandler;
        }

        public void OnGameStart()
        {

        }

        public Embed GetGameMessage() 
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.Green
            };

            embed.AddField($"Game {id}", "Game 1, Join or Leave")
                .AddField("Queue", "Player1\nPlayer2\nPlayer3\nPlayer4\nPlayer5\nPlayer1\nPlayer2\nPlayer3\nPlayer4\nPlayer5")
                .WithCurrentTimestamp()
                .WithImageUrl("https://c.tenor.com/CYJS0cjte98AAAAC/sengoku-nadeko-hitagi-end.gif");

            return embed.Build();
        }

        public MessageComponent GetMessageComponent() => componentBuilder.Build();
        

        public async Task ButtonHandler(SocketMessageComponent component)
        {       
            if($"join-button-{id}" == component.Data.CustomId)
            {
                EmbedBuilder embed = new EmbedBuilder();

                string queue = "";

                players[numberOfPlayers] = component.User;

                for(int i=0; i < numberOfPlayers; i++)
                {
                    queue += $"{players[i].Username}\n";
                }

                Console.WriteLine(queue);

                embed.AddField($"Game {id}", "Game 1, Join or Leave")
                    .AddField("Queue", "Player54\nPlayer2\nPlayer3\nPlayer4\nPlayer5\nPlayer1\nPlayer2\nPlayer3\nPlayer4\nPlayer5")
                    .WithCurrentTimestamp()
                    .WithImageUrl("https://c.tenor.com/CYJS0cjte98AAAAC/sengoku-nadeko-hitagi-end.gif");

                await component.Channel.SendMessageAsync("Lmao");

                await message.ModifyAsync(delegate(MessageProperties message) {
                    message.Embed = embed.Build();
                });

                numberOfPlayers++;
            }

            if($"leave-button-{id}" == component.Data.CustomId)
            {
                numberOfPlayers--;

                if(numberOfPlayers < 0)
                    numberOfPlayers = 0;
            }

        }
    }
}