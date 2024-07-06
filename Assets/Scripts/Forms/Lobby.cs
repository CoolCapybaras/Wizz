using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Net.Packets.Clientbound;
using System.Linq;
using Net.Packets.Serverbound;

public class Lobby : MonoBehaviour, IForm
{
    public static Lobby Instance;

    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI quizNameText;
        public TextMeshProUGUI lobbyCodeText;
        public TextMeshProUGUI quizDescriptionText;
        public TextMeshProUGUI playersCountText;

        public Transform playersLayout;
        public GameObject playerPrefab;

        public GameObject startGameButton;

        public QuizUI quizCard;
    }

    private GameManager gameManager;
    public Form form;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        Instance = this;
    }

    public void OnClientsListChanged()
    {
        form.playersCountText.text = $"Количество игроков: {gameManager.currentClients.Count}";
        InstantiatePlayers();
    }

    public void RemovePlayersFromLayout()
    {
        var count = form.playersLayout.childCount;
        for (int i = 0; i < count; ++i)
            Destroy(form.playersLayout.GetChild(i).gameObject);
    }

    public void InstantiatePlayers()
    {
        RemovePlayersFromLayout();
        var players = gameManager.currentClients;
        foreach (var player in players)
        {
            var obj = Instantiate(form.playerPrefab, form.playersLayout);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Name;
            obj.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = player.Image.GetTexture();
        }
    }

    public void InitializeForm()
    {
        gameManager.isHost = false;
        form.quizNameText.text = $"Лобби \"{gameManager.currentQuiz.Name}\"";
        form.lobbyCodeText.text = $"Код доступа: #{gameManager.currentLobbyId}";
        form.quizDescriptionText.text = gameManager.currentQuiz.Description;
        OnClientsListChanged();

        form.quizCard.name.text = gameManager.currentQuiz.Name;
        form.quizCard.description.text = gameManager.currentQuiz.Description;
        form.quizCard.image.texture = gameManager.currentQuiz.Image.GetTexture();
        // TODO: form.quizCard.hashtags заполнить

        if (gameManager.currentClients.Count != 1)
        {
            form.startGameButton.SetActive(false);
            return;
        }
        form.startGameButton.SetActive(true);
        gameManager.isHost = true;
    }

    public void OnClientJoined(ClientJoinedPacket packet)
    {
        GameManager.Instance.currentClients.Add(packet.Client);
        OnClientsListChanged();
    }

    public void OnClientLeaved(ClientLeavedPacket packet)
    {
        var currentClients = GameManager.Instance.currentClients;
        currentClients.Remove(currentClients.Where(c => c.Id == packet.ClientId).First());
        OnClientsListChanged();
    }

    public void OnLobbyJoined(LobbyJoinedPacket packet)
    {
        Debug.Log($"LobbyJoined: {packet.LobbyId}");

        var gameManager = GameManager.Instance;
        gameManager.currentLobbyId = packet.LobbyId;
        gameManager.currentQuiz = packet.Quiz;
        gameManager.currentClients = packet.Clients.ToList();
        gameManager.isInLobby = true;
        FormManager.Instance.ChangeForm("lobby");
        OverlayManager.Instance.ChangeBackgroundColor(gameManager.currentQuiz.Color);
    }

    public void OnGameStartPressed()
    {
        LocalClient.instance.SendPacket(new StartGamePacket());
    }

    public void OnGameStarted(GameStartedPacket packet)
    {
        FormManager.Instance.ChangeForm("quizloading");
        gameManager.questions = packet.QuizQuestions;
        gameManager.isInStartedGame = true;
    }
}
