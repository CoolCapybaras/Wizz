using System;
using System.Threading.Tasks;
using WizzServer.Models;

namespace Net.Packets.Serverbound
{
	public enum AuthType
	{
		Anonymous,
		Token,
		VK,
		Telegram
	}

	public class AuthPacket : IPacket
	{
		public int Id => 2;
		public AuthType Type { get; set; }
		public string Name { get; set; }
		public string Token { get; set; }

		public AuthPacket()
		{

		}

		public AuthPacket(AuthType type, string data)
		{
			Type = type;
			if (type == AuthType.Anonymous)
				Name = data!;
			else if (type == AuthType.Token)
				Token = data!;
		}

		public static AuthPacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new AuthPacket();
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
			Type = (AuthType)stream.ReadVarInt();
			if (Type == AuthType.Anonymous)
				Name = stream.ReadString();
			else if (Type == AuthType.Token)
				Token = stream.ReadString();
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteVarInt(Type);
			if (Type == AuthType.Anonymous)
				packetStream.WriteString(Name);
			else if (Type == AuthType.Token)
				packetStream.WriteString(Token);

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
