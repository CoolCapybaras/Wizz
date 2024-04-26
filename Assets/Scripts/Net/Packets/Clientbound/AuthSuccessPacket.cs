﻿using System.Threading.Tasks;
using UnityEngine;

namespace Net.Packets.Clientbound
{
    public class AuthSuccessPacket : IPacket
	{
		public int Id => 11;

		public int ClientId { get; set; }
		public string Name { get; set; }
		public Texture2D Image { get; set; }

		public AuthSuccessPacket()
		{

		}

		public AuthSuccessPacket(int clientId, string name)
		{
			ClientId = clientId;
			Name = name;
		}

		public static AuthSuccessPacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new AuthSuccessPacket();
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
			ClientId = stream.ReadVarInt();
			Name = stream.ReadString();
			Image = stream.ReadImage();
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteVarInt(ClientId);
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
			Debug.Log($"{ClientId} {Name}");
			LocalClient.instance.Name = Name;
			LocalClient.instance.Id = Id;
			FormManager.Instance.ChangeForm("mainmenu");
			return IPacket.CompletedTask;
		}
	}
}
