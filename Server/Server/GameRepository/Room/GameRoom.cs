using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
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
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public Map Map { get; private set; } = new Map();

        public void Init(int mapId)
        {
            Map.LoadMap(mapId);
        }

        public void Update()
        {
            lock (_lock)
            {
                foreach(Projectile projectile in _projectiles.Values)
                {
                    projectile.Update();
                }
            }
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player _player = gameObject as Player;
                    _players.Add(_player.Id, _player);
                    _player.Room = this;

                    // 본인한테 정보 전송
                    {
                        // 본인에게 방에 접속함을 전달
                        S_EnterGame enterPacket = new S_EnterGame();
                        enterPacket.Player = _player.Info;
                        _player.Session.Send(enterPacket);

                        // 현재 방에 접속한 인원들의 정보를 전달
                        S_Spawn spawnPacket = new S_Spawn();
                        foreach (Player player in _players.Values)
                        {
                            if (player != _player)
                                spawnPacket.Objects.Add(player.Info);
                        }
                        _player.Session.Send(spawnPacket);
                    }
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster _monster = gameObject as Monster;
                    _monsters.Add(_monster.Id, _monster);
                    _monster.Room = this;
                }
                else if (type == GameObjectType.Projectile)
                {
                    Projectile _projectile = gameObject as Projectile;
                    _projectiles.Add(_projectile.Id, _projectile);
                    _projectile.Room = this;
                }

                // 타인한테 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Objects.Add(gameObject.Info);
                    foreach (Player player in _players.Values)
                    {
                        if (player.Id != gameObject.Id) // gameObject가 Player일때 만 적용됨
                            player.Session.Send(spawnPacket);
                    }
                }
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (type == GameObjectType.Player)
                {
                    Player _player = null;
                    if (_players.Remove(objectId, out _player) == false)
                        return;

                    _player.Room = null;

                    // 맵 상에서 플레이어 제거
                    Map.ApplyLeave(_player);

                    // 본인한테 정보 전송
                    {
                        S_LeaveGame leavePacket = new S_LeaveGame();
                        _player.Session.Send(leavePacket);
                    }
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster _monster = null;
                    if (_monsters.Remove(objectId, out _monster) == false)
                        return;

                    _monster.Room = null;

                    Map.ApplyLeave(_monster);
                }
                else if (type == GameObjectType.Projectile)
                {
                    Projectile _projectile = null;
                    if (_projectiles.Remove(objectId, out _projectile) == false)
                        return;

                    _projectile.Room = null;
                }

                // 타인에게 정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.ObjectIds.Add(objectId);
                    foreach (Player player in _players.Values)
                    {
                        if (player.Id != objectId)
                            player.Session.Send(despawnPacket);
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
                    if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                        return;
                }

                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                // 다른 플레이어한테도 알려준다
                S_Move resMovePacket = new S_Move();
                resMovePacket.ObjectId = player.Info.ObjectId;
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
                skill.ObjectId = info.ObjectId;
                skill.Info.SkillId = skillPacket.Info.SkillId;
                Broadcast(skill);

                Data.Skill skillData = null;
                if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
                    return;

                switch (skillData.skillType)
                {
                    case SkillType.SkillAuto:
                        {
                            // TODO : 데미지 판정
                            Vector2Int skillPos = player.GetFrontCellPosition(info.PosInfo.MoveDir);
                            GameObject target = Map.Find(skillPos) as GameObject;
                            if (target != null)
                            {
                                Console.WriteLine("Hit Object!");
                            }
                        }
                        break;
                    case SkillType.SkillProjectile:
                        {
                            Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                            if (arrow == null)
                                return;

                            arrow.Owner = player;
                            arrow.Data = skillData;
                            arrow.Owner = player;
                            arrow.PosInfo.State = CreatureState.Moving;
                            arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                            arrow.PosInfo.PosX = player.PosInfo.PosX;
                            arrow.PosInfo.PosY = player.PosInfo.PosY;
                            EnterGame(arrow);
                        }
                        break;
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
