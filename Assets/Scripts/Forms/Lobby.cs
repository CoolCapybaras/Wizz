using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

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
        for (int i = 0; i < players.Count; ++i)
        {
            var obj = Instantiate(form.playerPrefab, form.playersLayout);
            obj.GetComponent<RawImage>().texture = players[i].Image;
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = players[i].Name;
        }
    }

    public void InitializeForm()
    {
        form.quizNameText.text = $"Лобби \"{gameManager.currentQuiz.Name}\"";
        form.lobbyCodeText.text = $"Код доступа: #{gameManager.currentLobbyId}";
        form.quizDescriptionText.text = gameManager.currentQuiz.Description;
        OnClientsListChanged();

        form.quizCard.name.text = gameManager.currentQuiz.Name;
        form.quizCard.description.text = gameManager.currentQuiz.Description;
        form.quizCard.image.texture = gameManager.currentQuiz.Image;
        // TODO: form.quizCard.hashtags заполнить
    }
}
