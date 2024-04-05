using Google.Protobuf.Protocol;
using System.Collections.Generic;

namespace Server.GameRepository
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        List<Player> _players = new List<Player>();

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null) 
                return;

            lock (_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    // 본인에게 방에 접속함을 전달
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    // 현재 방에 접속한 인원들의 정보를 전달
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player player in _players)
                    {
                        if (player != newPlayer)
                            spawnPacket.Players.Add(player.Info);
                    }
                    newPlayer.Session.Send(spawnPacket);
                }

                // 타인한테 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach(Player player in _players)
                    {
                        if (player != newPlayer)
                            player.Session.Send(spawnPacket);
                    }
                }
            }
        }

        public void LeaveGame(int playerId)
        {
            lock ( _lock)
            {
                Player player = _players.Find(player => player.Info.PlayerId == playerId);
                if (player == null)
                    return;

                _players.Remove(player);
                player.Room = null;

                // 본인한테 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }

                // 타인에게 정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.PlayerIds.Add(player.Info.PlayerId);
                    foreach(Player p in _players)
                    {
                        if (p != player)
                            p.Session.Send(despawnPacket);
                    }
                }
            }   
        }
    }
}
