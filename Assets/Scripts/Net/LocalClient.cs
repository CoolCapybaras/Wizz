using Net.Packets;
using Net.Packets.Clientbound;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class LocalClient : MonoBehaviour
{
	public static LocalClient instance;

	public string Hostname = "localhost";
	public int Port = 8887;

	private TcpClient tcpClient;
	private NetworkStream networkStream;
	private WizzStream wizzStream;

	public int Id { get; set; }
	public string Name { get; set; }
	public Texture2D Image { get; set; }
	public bool Authorized { get; set; }

	private ConcurrentQueue<IPacket> packetQueue = new();

	void Awake()
	{
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		Task.Run(StartClient);
    }

    // Update is called once per frame
    void Update()
    {
		while (packetQueue.Count > 0)
		{
			packetQueue.TryDequeue(out IPacket packet);
			packet.HandleAsync(this);
		}
	}

	private async Task StartClient()
	{
		try
		{
			await ProcessClient();
		}
		catch (Exception e)
		{
			Debug.LogException(e);	
		}

		Debug.Log("Shutting down...");
	}

	private async Task ProcessClient()
	{
		tcpClient = new TcpClient();
		await tcpClient.ConnectAsync(Hostname, Port);
		networkStream = tcpClient.GetStream();
		wizzStream = new WizzStream(networkStream);

		Debug.Log("Client started");

		while (true)
		{
			(var id, var data) = await GetNextPacketAsync();

			if (id == 0)
				break;

			switch (id)
			{
				case 1:
					packetQueue.Enqueue(MessagePacket.Deserialize(data));
					break;
				case 11:
					packetQueue.Enqueue(AuthSuccessPacket.Deserialize(data));
					break;
				case 12:
					packetQueue.Enqueue(SearchResultPacket.Deserialize(data));
					break;
				case 13:
					packetQueue.Enqueue(LobbyJoinedPacket.Deserialize(data));
					break;
				case 14:
					packetQueue.Enqueue(ClientJoinedPacket.Deserialize(data));
					break;
				case 15:
					packetQueue.Enqueue(ClientLeavedPacket.Deserialize(data));
					break;
				case 16:
					packetQueue.Enqueue(GameStartedPacket.Deserialize(data));
					break;
				case 17:
					packetQueue.Enqueue(GameEndedPacket.Deserialize(data));
					break;
				case 18:
					packetQueue.Enqueue(TimerStartedPacket.Deserialize(data));
					break;
				case 19:
					packetQueue.Enqueue(RoundStartedPacket.Deserialize(data));
					break;
				case 20:
					packetQueue.Enqueue(RoundEndedPacket.Deserialize(data));
					break;
				case 21:
					packetQueue.Enqueue(RightAnswerPacket.Deserialize(data));
					break;
			}
		}
	}

	private async Task<(int id, byte[] data)> GetNextPacketAsync()
	{
		var length = await wizzStream.ReadVarIntAsync();
		var receivedData = new byte[length];

		_ = await wizzStream.ReadAsync(receivedData.AsMemory(0, length));

		var packetId = 0;
		var packetData = Array.Empty<byte>();

		await using (var packetStream = new WizzStream(receivedData))
		{
			try
			{
				packetId = await packetStream.ReadVarIntAsync();
				var arlen = 0;

				if (length - packetId.GetVarIntLength() > -1)
					arlen = length - packetId.GetVarIntLength();

				packetData = new byte[arlen];
				_ = await packetStream.ReadAsync(packetData.AsMemory(0, packetData.Length));
			}
			catch
			{
				throw;
			}
		}

		return (packetId, packetData);
	}

	public void SendPacket(IPacket packet)
	{
		packet.Serialize(wizzStream);
	}
}
