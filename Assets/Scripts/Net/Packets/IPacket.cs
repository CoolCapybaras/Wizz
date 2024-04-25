using System.Threading.Tasks;

namespace Net.Packets
{
	public interface IPacket
	{
		protected static ValueTask CompletedTask => default;

		public int Id { get; }

		public void Populate(byte[] data);
		public void Serialize(WizzStream stream);
		public ValueTask HandleAsync(LocalClient player);
	}
}
