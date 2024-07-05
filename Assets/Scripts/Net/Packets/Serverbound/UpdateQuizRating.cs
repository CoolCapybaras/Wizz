using System;
using System.Threading.Tasks;

namespace Net.Packets.Serverbound
{
	public class UpdateQuizRating : IPacket
	{
		public int Id => 25;

		public int Score { get; set; }

		public static UpdateQuizRating Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new UpdateQuizRating();
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
			Score = stream.ReadVarInt();
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteVarInt(Score);

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
