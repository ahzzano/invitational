using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Net;

namespace invitational {
    class Game {
        public int maxPlayers = 10; 
        public bool completed = false;
        public SocketUserMessage message;
        public EmbedBuilder embeddedMessage; 
        public int id;
    }
}