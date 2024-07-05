using System;
using System.Threading.Tasks;

namespace Net.Packets
{
	public enum EditQuizType
	{
		Get,
		Upload,
		Delete,
		Publish
	}

	public class EditQuizPacket : IPacket
	{
		public int Id => 11;

		public EditQuizType Type { get; set; }
		public int QuizId { get; set; }
		public Quiz Quiz { get; set; }

		public static EditQuizPacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new EditQuizPacket();
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
			Type = (EditQuizType)stream.ReadVarInt();
			if (Type == EditQuizType.Get)
				Quiz = Quiz.Deserialize(stream);
			else
				QuizId = stream.ReadVarInt();
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteVarInt(Type);
			if (Type == EditQuizType.Upload)
				Quiz.Serialize(packetStream, true);
			else
				packetStream.WriteVarInt(QuizId);

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
