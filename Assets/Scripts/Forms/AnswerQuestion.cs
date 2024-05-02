using Net.Packets.Clientbound;
using Net.Packets.Serverbound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerQuestion : MonoBehaviour, IForm
{
    public static AnswerQuestion Instance;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        Instance = this;
    }

    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI questionText;
        public RawImage questionImage;
        public AnswerButton[] answerButtons;
        public Image timerImage;

        public Color32 rightAnswerColor;
        public Color32 wrongAnswerColor;
    }

    public Form form;

    private bool timerStarted;
    private float time;
    private float questionTime;

    private int answeredIndex;

    private void Update()
    {
        if (!timerStarted || time >= questionTime)
            return;

        form.timerImage.fillAmount = time / questionTime;

        time += Time.deltaTime;
    }

    public void InitializeForm()
    {
        var question = gameManager.currentQuestion;
        form.questionText.text = question.Question;
        form.questionImage.texture = question.Image;
        
        for (int i = 0; i < form.answerButtons.Length; i++)
        {
            var button = form.answerButtons[i];
            button.obj.GetComponent<Image>().color = button.defaultColor;
            button.obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = question.Answers[i];
            button.obj.GetComponent<AnswerButtonHelper>().index = i;
            button.obj.GetComponent<Button>().interactable = true;
        }

        time = 0;
        questionTime = question.Time;
        timerStarted = true;
    }

    public void OnPlayerAnswer(int index)
    {
        answeredIndex = index;
        LocalClient.instance.SendPacket(new AnswerGamePacket() { AnswerId = index });
    }

    public void OnRightAnswer(RightAnswerPacket packet)
    {
        if (answeredIndex == packet.AnswerId)
            OverlayManager.Instance.ShowInfo("Правильный ответ!", InfoType.Success);
        else
            OverlayManager.Instance.ShowInfo("Не угадал :-(((((", InfoType.Error);

        timerStarted = false;
        HighlightButton(packet.AnswerId);
    }

    public void HighlightButton(int index)
    {
        for (int i = 0; i < form.answerButtons.Length; ++i)
        {
            var button = form.answerButtons[i];
            button.obj.GetComponent<Button>().interactable = false;

            if (index == i)
                button.obj.GetComponent<Image>().color = form.rightAnswerColor;
            else
                button.obj.GetComponent<Image>().color = form.wrongAnswerColor;
        }
    }
    
    public void OnGameEnded(GameEndedPacket packet)
    {
        gameManager.CurrentScore = packet.Score;
        gameManager.CurrentScore = gameManager.CurrentScore
            .OrderBy(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
        gameManager.isInLobby = false;
        FormManager.Instance.ChangeForm("endgame");
    }

    public void OnRoundEnded(RoundEndedPacket packet)
    {
        gameManager.CurrentScore = packet.Score;
        gameManager.CurrentScore = gameManager.CurrentScore
            .OrderBy(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
        FormManager.Instance.ChangeForm("rating");
    }
}
