﻿using System;
using System.Threading.Tasks;

namespace Net.Packets.Serverbound
{
	public class ContinueGamePacket : IPacket
	{
		public int Id => 9;

		public static ContinueGamePacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new ContinueGamePacket();
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

		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();

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