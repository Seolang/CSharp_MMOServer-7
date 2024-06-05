using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.GameRepository.Object;
using System;
using System.Collections.Generic;

namespace Server.GameRepository.Room
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        Map _map = new Map();

        public void Init(int mapId)
        {
            _map.LoadMap(mapId);
        }

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (_lock)
            {
                _players.Add(newPlayer.Info.ObjectId, newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    // 본인에게 방에 접속함을 전달
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    // 현재 방에 접속한 인원들의 정보를 전달
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player player in _players.Values)
                    {
                        if (player != newPlayer)
                            spawnPacket.Objects.Add(player.Info);
                    }
                    newPlayer.Session.Send(spawnPacket);
                }

                // 타인한테 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Objects.Add(newPlayer.Info);
                    foreach (Player player in _players.Values)
                    {
                        if (player != newPlayer)
                            player.Session.Send(spawnPacket);
                    }
                }
            }
        }

        public void LeaveGame(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.Remove(playerId, out player) == false)
                    return;

                player.Room = null;

                // 본인한테 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }

                // 타인에게 정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.PlayerIds.Add(player.Info.ObjectId);
                    foreach (Player p in _players.Values)
                    {
                        if (p != player)
                            p.Session.Send(despawnPacket);
                    }
                }
            }
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                // TODO: 정상 패킷인지 검증

                // 서버에서 좌표 이동
                PositionInfo movePosInfo = movePacket.PosInfo;
                ObjectInfo info = player.Info;

                // 다른 좌표로 이동 할 경우, 갈 수 있는지 체크
                if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
                {
                    if (_map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                        return;
                }

                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                _map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                // 다른 플레이어한테도 알려준다
                S_Move resMovePacket = new S_Move();
                resMovePacket.PlayerId = player.Info.ObjectId;
                resMovePacket.PosInfo = movePacket.PosInfo;

                Broadcast(resMovePacket);
            }
        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                ObjectInfo info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                // TODO : 스킬 사용 가능 여부 체크
                info.PosInfo.State = CreatureState.Skill;
                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.PlayerId = info.ObjectId;
                skill.Info.SkillId = skillPacket.Info.SkillId;
                Broadcast(skill);

                if (skillPacket.Info.SkillId == 1)
                {
                    // TODO : 데미지 판정
                    Vector2Int skillPos = player.GetFrontCellPosition(info.PosInfo.MoveDir);
                    Player target = _map.Find(skillPos);
                    if (target != null)
                    {
                        Console.WriteLine("Hit Player!");
                    }
                }
                else if (skillPacket.Info.SkillId == 2)
                {
                    // TODO : Arrow
                }
            }
        }

        public void Broadcast(IMessage packet)
        {
            lock (_lock)
            {
                foreach (Player p in _players.Values)
                {
                    p.Session.Send(packet);
                }
            }
        }


    }
}
