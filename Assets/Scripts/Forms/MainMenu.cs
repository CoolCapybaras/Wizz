using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour, IForm
{
    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI topGreetingText;
        public Transform myQuizzesLayout;
    }

    public static MainMenu Instance;
    public Form form;
    private LocalClient localClient;

    private void Awake()
    {
        localClient = LocalClient.instance;
        Instance = this;
    }

    public void OnJoinLobbyPressed()
    {
        FormManager.Instance.ChangeForm("joinlobby");
    }

    public void InitializeForm()
    {
        form.topGreetingText.text = $"Пора создавать новое, {localClient.Name}!";
    }
}
