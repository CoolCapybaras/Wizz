using Net.Packets.Serverbound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizzServer.Models;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentLobbyId;
    public Quiz currentQuiz;
    public List<ClientDTO> currentClients = new();

    public QuizQuestion currentQuestion;
    public int currentQuestionCount;

    public bool isInLobby;
    private void Awake()
    {
        Instance = this;
    }

    public void EnsureLeavedLobby()
    {
        if (isInLobby)
            LocalClient.instance.SendPacket(new LeaveLobbyPacket());
    }
}