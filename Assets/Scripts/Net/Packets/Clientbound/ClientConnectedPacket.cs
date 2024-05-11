using System;
using System.Threading.Tasks;

namespace Net.Packets.Clientbound
{
	public class ClientConnectedPacket : IPacket
	{
		public int Id => throw new NotImplementedException();

		public void Populate(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void Serialize(WizzStream stream)
		{
			throw new NotImplementedException();
		}

		public ValueTask HandleAsync(LocalClient player)
		{
			Login.Instance.OnClientStarted();
			return IPacket.CompletedTask;
		}
	}
}
