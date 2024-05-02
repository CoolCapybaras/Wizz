using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Net.Packets.Serverbound;
public class JoinLobby : MonoBehaviour, IForm
{
    public static JoinLobby Instance;

    public TMP_InputField inputField;
    private LocalClient localClient;
    private void Awake()
    {
        localClient = LocalClient.instance;
        Instance = this;
    }

    public void OnJoinPressed()
    {
        localClient.SendPacket(new JoinLobbyPacket() { LobbyId = int.Parse(inputField.text) });
    }

    public void InitializeForm()
    {
        inputField.text = string.Empty;
    }
}
