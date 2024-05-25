using Net.Packets.Serverbound;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WizzServer.Models;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentLobbyId;
    public Quiz currentQuiz;
    public List<ClientDTO> currentClients = new();
    public Dictionary<int, int> CurrentScore = new();

    public QuizQuestion[] questions;
    public int currentQuestionIndex;

    public List<Quiz> searchedQuizzes;
    
    public bool isInLobby;
    public bool isInStartedGame;
    public bool isHost;
    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 120;
    }

    public void EnsureLeavedLobby()
    {
        if (isInLobby)
            LocalClient.instance.SendPacket(new LeaveLobbyPacket());
    }

    public void EnsureLeavedStartedGame()
    {
        if (!isInStartedGame)
            return;

        SoundManager.Instance.StopMusic();
        SoundManager.Instance.ForceCountdownStop();
        SoundManager.Instance.SetLowPassFilter(false, 0, false);
        SoundManager.Instance.PlayMusic("menu", true);
        isInStartedGame = false;
    }

    public ClientDTO GetClientById(int id) => currentClients.First(x => x.Id == id);
    
}
