﻿using Google.Protobuf.Protocol;

namespace Server.GameRepository
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo();
        public GameRoom Room { get; set; }
        public ClientSession Session { get; set; }

    }
}