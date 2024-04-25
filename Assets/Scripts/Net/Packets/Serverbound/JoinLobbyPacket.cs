using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Packets.Serverbound
{
	public class JoinLobbyPacket : IPacket
	{
		private static readonly Dictionary<int, string> easterEggQuizzes = new()
		{
			{ 250252, "easteregg" }
		};

		public int Id => 5;

		public int LobbyId { get; set; }

		public static JoinLobbyPacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new JoinLobbyPacket();
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
			LobbyId = stream.ReadVarInt();
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteVarInt(LobbyId);

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
