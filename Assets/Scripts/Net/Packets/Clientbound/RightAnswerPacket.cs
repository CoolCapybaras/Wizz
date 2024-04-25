﻿using System;
using System.Threading.Tasks;

namespace Net.Packets.Clientbound
{
	public class RightAnswerPacket : IPacket
	{
		public int Id => 21;

		public int AnswerId { get; set; }

		public RightAnswerPacket()
		{

		}

		public RightAnswerPacket(int answerId)
		{
			this.AnswerId = answerId;
		}

		public static RightAnswerPacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new RightAnswerPacket();
			packet.Populate(stream);
			return packet;
		}

		public void Populate(byte[] data)
		{
			using var stream = new WizzStream(data);
			Populate(stream);
		}

		public void Populate(WizzStream stream)
		{
			AnswerId = stream.ReadVarInt();
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteVarInt(AnswerId);

			stream.Lock.Wait();
			stream.WriteVarInt(Id.GetVarIntLength() + (int)packetStream.Length);
			stream.WriteVarInt(Id);
			packetStream.Position = 0;
			packetStream.CopyTo(stream);
			stream.Lock.Release();
		}

		public ValueTask HandleAsync(LocalClient client)
		{
			throw new NotImplementedException();
		}
	}
}