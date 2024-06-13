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
				Quiz.Serialize(packetStream, false);
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
			if (Type == EditQuizType.Upload)
			{
				QuizEditor.Instance.quiz.Id = QuizId;
				if (!QuizEditor.Instance.needPublish)
				{
					OverlayManager.Instance.ShowInfo("Викторина успешно сохранена!\n" +
					                                 "Вы в любой момент можете её отправить " +
					                                 "на модерацию из раздела \"Мои викторины\"", InfoType.Success);
					return IPacket.CompletedTask;
				}
				
				LocalClient.instance.SendPacket(new EditQuizPacket{Type = EditQuizType.Publish, QuizId = QuizId});
				OverlayManager.Instance.ShowInfo("Викторина успешно сохранена и отправлена на модерацию", InfoType.Success);
			}

			if (Type == EditQuizType.Get)
			{
				QuizEditor.Instance.OnGetQuizResult(this);
			}
			return IPacket.CompletedTask;
		}
	}
}
