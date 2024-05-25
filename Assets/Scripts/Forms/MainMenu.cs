using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenu : MonoBehaviour, IForm
{
    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI topGreetingText;
        public Transform myQuizzesLayout;

        public GameObject quizPrefab;
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

    public void OnNewGamePressed()
    {
        FormManager.Instance.ChangeForm("myquizzes");
    }

    public void InitializeForm()
    {
        var sequence = DOTween.Sequence();
        sequence.Insert(0, form.topGreetingText.transform.DOLocalMoveX(-825f, 1, true).From(-2500))
            .Insert(0, form.topGreetingText.DOFade(1, 1).From(0))
            .Insert(0, form.myQuizzesLayout.GetComponent<CanvasGroup>().DOFade(1, 1).From(0))
            .Play();
        form.topGreetingText.text = $"Пора создавать новое, {localClient.Name}!";
    }

    public void InstantiateQuizzes()
    {
        var quizzes = new List<Quiz>();
        quizzes.
    }
}
