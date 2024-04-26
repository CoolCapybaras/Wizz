using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizzServer.Models;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentLobbyId;
    public Quiz currentQuiz;
    public List<ClientDTO> currentClients;

    private void Awake()
    {
        Instance = this;
    }
}