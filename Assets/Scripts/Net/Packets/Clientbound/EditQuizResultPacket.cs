using System;
using System.Threading.Tasks;

namespace Net.Packets.Clientbound
{
	public class EditQuizResultPacket : IPacket
	{
		public int Id => 26;

		public Quiz Quiz { get; set; }

		public EditQuizResultPacket()
		{

		}

		public EditQuizResultPacket(Quiz quiz)
		{
			Quiz = quiz;
		}

		public static GameStartedPacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new GameStartedPacket();
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
			Quiz = Quiz.Deserialize(stream);
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			Quiz.Serialize(packetStream, false);

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
