using Assets.Scripts.Misc;
using DG.Tweening;
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

        public GameObject resultObj;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI scoreText;
        public GameObject continueButton;
        public Transform answersLayout;
        public GameObject answerPrefab;
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
        SetActiveAnswerButtons(true);
        form.resultObj.SetActive(false);
        var question = gameManager.questions[gameManager.currentQuestionIndex - 1];
        form.questionText.text = question.Question;
        form.questionImage.texture = question.Image.GetTexture();
        DestroyLayoutChildren(form.answersLayout);
        for (int i = 0; i < question.Answers.Count; ++i)
        {
            var answer = question.Answers[i];
            form.answerButtons[i].obj = InstantiateAnswerButton(form.answerButtons[i].defaultColor, answer, i);
        }

        time = 0;
        questionTime = question.Time;
        timerStarted = true;
        SoundManager.Instance.SetLowPassFilter(false, 0, false);
        SoundManager.Instance.PlayMusic("ingame");
    }

    private GameObject InstantiateAnswerButton(Color32 color, string text, int index)
    {
        var obj = Instantiate(form.answerPrefab, form.answersLayout);
        obj.GetComponent<Image>().color = color;
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.GetComponent<AnswerButtonHelper>().index = index;
        obj.GetComponent<Button>().interactable = true;
        
        return obj;
    }

    private void DestroyLayoutChildren(Transform layout)
    {
        for (int i = 0; i < layout.childCount; ++i)
            Destroy(layout.GetChild(i).gameObject);
    }
    
    public void OnPlayerAnswer(int index)
    {
        SetActiveAnswerButtons(false);
        answeredIndex = index;
        var answer = new QuizAnswer();
        // TODO: поддерживать разные типы вопросов
        answer.Id = answeredIndex;
        LocalClient.instance.SendPacket(new AnswerGamePacket() { Answer = answer});
    }

    public void OnRightAnswer(RightAnswerPacket packet)
    {
        // TODO: поддерживать разные типы вопросов
        PrepareResultUI(packet.Answer.Id, packet.RoundScore);
        timerStarted = false;
        HighlightButton(packet.Answer.Id);
        SoundManager.Instance.SetLowPassFilter(true, 1);
    }

    public void PrepareResultUI(int answerId, int roundScore)
    {
        form.resultObj.SetActive(true);
        var sequence = DOTween.Sequence();
        sequence.Insert(0, form.resultObj.GetComponent<CanvasGroup>().DOFade(1, 0.25f).From(0))
            .Insert(0, form.resultObj.transform.DOScale(1, 0.25f).From(2))
            .Play();

        if (!gameManager.isHost)
            form.continueButton.SetActive(false);
        else
            form.continueButton.SetActive(true);


        if (answeredIndex == answerId)
        {
            SoundManager.Instance.PlayShortClip("success");
            form.resultText.text = "ќтвет верный!";
            form.scoreText.text = $"+{roundScore} очков\n∆дем продолжени€ игры...";
            form.resultText.color = Colors.yellow;
        }
        else
        {
            SoundManager.Instance.PlayShortClip("fail");
            form.resultText.text = "ќтвет неверный!";
            form.scoreText.text = "∆дем продолжение игры...";
            form.resultText.color = Colors.red;
        }

    }

    public void HighlightButton(int index)
    {
        for (int i = 0; i < form.answersLayout.childCount; ++i)
        {
            var button = form.answerButtons[i];
            button.obj.GetComponent<Button>().interactable = false;

            if (index == i)
                button.obj.GetComponent<Image>().color = form.rightAnswerColor;
            else
                button.obj.GetComponent<Image>().color = form.wrongAnswerColor;
        }
    }

    public void SetActiveAnswerButtons(bool active)
    {
        for (int i = 0; i < form.answersLayout.childCount; ++i)
            form.answersLayout.GetChild(i).GetComponent<Button>().interactable = active;
    }
    
    public void OnContinuePressed()
    {
        LocalClient.instance.SendPacket(new ContinueGamePacket());
    }

    public void OnGameEnded(GameEndedPacket packet)
    {
        gameManager.CurrentScore = packet.Score;
        gameManager.CurrentScore = gameManager.CurrentScore
            .OrderByDescending(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
        gameManager.isInLobby = false;
        FormManager.Instance.ChangeForm("endgame");
        gameManager.isInStartedGame = false;
        OverlayManager.Instance.ChangeBackgroundColor(gameManager.currentQuiz.Color);
    }

    public void OnRoundEnded(RoundEndedPacket packet)
    {
        gameManager.CurrentScore = packet.Score;
        gameManager.CurrentScore = gameManager.CurrentScore
            .OrderByDescending(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
        FormManager.Instance.ChangeForm("rating");
    }
}
