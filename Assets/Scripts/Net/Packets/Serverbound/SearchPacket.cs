﻿using System;
using System.Threading.Tasks;

namespace Net.Packets.Serverbound
{
	public enum SearchType
	{
		Default,
		Author,
		History
	}

	public class SearchPacket : IPacket
	{
		public int Id => 3;

		public string QuizName { get; set; }
		public SearchType SearchType { get; set; }
		public int Offset { get; set; }
		public int Count { get; set; }

		public static SearchPacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new SearchPacket();
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
			QuizName = stream.ReadString();
			SearchType = (SearchType)stream.ReadVarInt();
			Offset = stream.ReadVarInt();
			Count = stream.ReadVarInt();
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteString(QuizName);
			packetStream.WriteVarInt(SearchType);
			packetStream.WriteVarInt(Offset);
			packetStream.WriteVarInt(Count);

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
