using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Net;

namespace invitational {
    class Game {
        public int maxPlayers = 10; 
        public bool completed = false;
        public IUserMessage message;
        public int id;
    }
}