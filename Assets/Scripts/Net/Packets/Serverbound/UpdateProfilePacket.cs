using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Packets.Serverbound
{
	public class UpdateProfilePacket : IPacket
	{
		public static Regex NameRegex { get; } = new("^[A-Za-zА-Яа-я0-9_ ]{3,24}$");

		public int Id => 10;

		public int Type { get; set; }
		public string Name { get; set; }
		public ByteImage Image { get; set; }

		public static UpdateProfilePacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new UpdateProfilePacket();
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
			Type = stream.ReadVarInt();
			Name = stream.ReadString();
			Image = stream.ReadImage();
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteVarInt(Type);
			packetStream.WriteString(Name);
			packetStream.WriteImage(Image);

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
