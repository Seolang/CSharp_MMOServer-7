﻿using System;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.GameRepository;

namespace Server
{
	public class ClientSession : PacketSession
	{
		public Player MyPlayer {  get; set; }
		public int SessionId { get; set; }

		public void Send(IMessage packet)
		{
			// 패킷에서 패킷 클래스 이름을 추출
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			
			// MsgId Enum으로 파싱
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

            base.Send(new ArraySegment<byte>(sendBuffer));
        }

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			// PROTO Test
			MyPlayer = PlayerManager.Instance.Add();

			MyPlayer.Info.Name = $"Player_{MyPlayer.Info.PlayerId}";
			MyPlayer.Info.PosX = 0 ;
			MyPlayer.Info.PosY = 0 ;
			MyPlayer.Session = this;

			RoomManager.Instance.Find(1).EnterGame(MyPlayer);
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			RoomManager.Instance.Find(1).LeaveGame(MyPlayer.Info.PlayerId);
			
			SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
