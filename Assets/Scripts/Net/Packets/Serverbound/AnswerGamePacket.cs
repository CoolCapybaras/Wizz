using System;
using System.Threading.Tasks;

namespace Net.Packets.Serverbound
{
	public class AnswerGamePacket : IPacket
	{
		public int Id => 8;

		public QuizAnswer Answer { get; set; }

		public static AnswerGamePacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new AnswerGamePacket();
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
			Answer = QuizAnswer.Deserialize(stream);
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			Answer.Serialize(packetStream);

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
