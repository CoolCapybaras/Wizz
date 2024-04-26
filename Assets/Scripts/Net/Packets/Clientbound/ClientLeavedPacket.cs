﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace Net.Packets.Clientbound
{
	public class ClientLeavedPacket : IPacket
	{
		public int Id => 15;

		public int ClientId { get; set; }

		public ClientLeavedPacket()
		{

		}

		public ClientLeavedPacket(int clientId)
		{
			this.ClientId = clientId;
		}

		public static ClientLeavedPacket Deserialize(byte[] data)
		{
			using var stream = new WizzStream(data);
			var packet = new ClientLeavedPacket();
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
		}

		public void Serialize(WizzStream stream)
		{
			using var packetStream = new WizzStream();
			packetStream.WriteVarInt(ClientId);

			stream.Lock.Wait();
			stream.WriteVarInt(Id.GetVarIntLength() + (int)packetStream.Length);
			stream.WriteVarInt(Id);
			packetStream.Position = 0;
			packetStream.CopyTo(stream);
			stream.Lock.Release();
		}

		public ValueTask HandleAsync(LocalClient client)
		{
			var currentClients = GameManager.Instance.currentClients;
            currentClients.Remove(currentClients.Where(c => c.Id == ClientId).First());
			Lobby.Instance.OnClientsListChanged();
            return IPacket.CompletedTask;
        }
	}
}
