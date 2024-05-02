using Net.Packets.Clientbound;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizQuestionForm : MonoBehaviour, IForm
{
    public static QuizQuestionForm Instance;
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
        public TextMeshProUGUI questionsCount;

        public Image timerImage;
    }

    public Form form;


    private float time;
    private float questionCountdown;

    private void Update()
    {
        if (time >= questionCountdown)
        {
            ShowAnswers();
            return;
        }

        form.timerImage.fillAmount = time / questionCountdown;

        time += Time.deltaTime;
    }

    public void InitializeForm()
    {
        var question = gameManager.currentQuestion;
        form.questionText.text = question.Question;
        form.questionImage.texture = question.Image;
        form.questionsCount.text = $"Вопрос {gameManager.currentQuestionCount} из {gameManager.currentQuiz.QuestionsCount}";

        time = 0;
        questionCountdown = question.Countdown;
    }

    public void OnRoundStarted(RoundStartedPacket packet)
    {
        ++gameManager.currentQuestionCount;
        gameManager.currentQuestion = packet.question;
        FormManager.Instance.ChangeForm("quizquestion");
    }

    public void ShowAnswers()
    {
        FormManager.Instance.ChangeForm("answerquestion");
    }
}
