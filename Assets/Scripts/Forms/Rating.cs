using Net.Packets.Serverbound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using WizzServer.Models;

public class Rating : MonoBehaviour, IForm
{
    public static Rating Instance;

    private void Awake()
    {
        Instance = this;
        gameManager = GameManager.Instance;
    }
    
    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI FormName;
        public GameObject[] PlayerPrefabs;

        public GameObject NextButton;
    }

    private GameManager gameManager;
    public Form form;


    public void InstantiatePlayers()
    {
        RemovePlayersFromScoreBoard();
        var scores = gameManager.CurrentScore.ToList();
        var count = Math.Min(6, scores.Count);
        for (int i = 0; i < count; i++)
        {
            var player = gameManager.GetClientById(scores[i].Key);
            var obj = form.PlayerPrefabs[i];
            obj.SetActive(true);
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
            obj.transform.GetChild(2).GetComponent<RawImage>().texture = player.Image.GetTexture();
            obj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = player.Name;
            obj.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"{scores[i].Value} баллов";
        }
    }

    public void RemovePlayersFromScoreBoard()
    {
        for(int i = 0; i < form.PlayerPrefabs.Length; i++)
            form.PlayerPrefabs[i].SetActive(false);
    }
    
    public void OnContinuePressed()
    {
        LocalClient.instance.SendPacket(new ContinueGamePacket());
    }

    public void InitializeForm()
    {
        if (!gameManager.isHost)
            form.NextButton.SetActive(false);
        else
            form.NextButton.SetActive(true);
        InstantiatePlayers();
    }
}
