﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.GameRepository.Object;
using Server.GameRepository.Room;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"C_Move : {clientSession.SessionId} -> ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

		// 멀티스레드에서의 널 안정성을 위해 공유 객체를 로컬 객체로 복사
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.HandleMove(player, movePacket);
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
        C_Skill skillPacket = packet as C_Skill;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"C_Skill : {clientSession.SessionId} -> SKILL_{skillPacket.Info.SkillId}");


        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.HandleSkill(player, skillPacket);
    }
}
