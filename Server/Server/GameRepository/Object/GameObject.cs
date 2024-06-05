﻿using Google.Protobuf.Protocol;
using Server.GameRepository.Room;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.GameRepository.Object
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public GameRoom Room { get; set; }
        public ObjectInfo Info { get; set; }
        public PositionInfo PosInfo { get; private set; }
        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        public GameObject()
        {
            Info = new ObjectInfo();
            PosInfo = new PositionInfo();
            Info.PosInfo = PosInfo;
        }

        public Vector2Int CellPos
        {
            get
            {
                return new Vector2Int(PosInfo.PosX, PosInfo.PosY);
            }

            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }

        public Vector2Int GetFrontCellPosition()
        {
            return GetFrontCellPosition(PosInfo.MoveDir);
        }

        public Vector2Int GetFrontCellPosition(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;
            }

            return cellPos;
        }

    }
}