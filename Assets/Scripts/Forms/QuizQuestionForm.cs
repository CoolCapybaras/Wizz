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
    private bool timerStarted;

    private void Update()
    {
        if (timerStarted && time >= questionCountdown)
        {
            timerStarted = false;
            ShowAnswers();
            return;
        }

        form.timerImage.fillAmount = time / questionCountdown;

        time += Time.deltaTime;
    }

    public void InitializeForm()
    {
        var question = gameManager.questions[gameManager.currentQuestionIndex - 1];
        form.questionText.text = question.Question;
        form.questionImage.texture = question.Image.GetTexture();
        form.questionsCount.text = $"Вопрос {gameManager.currentQuestionIndex} из {gameManager.currentQuiz.QuestionCount}";

        time = 0;
        timerStarted = true;
        SoundManager.Instance.StartCountdown((int)questionCountdown);
        SoundManager.Instance.StopMusic();
    }

    public void OnRoundStarted(RoundStartedPacket packet)
    {
        ++gameManager.currentQuestionIndex;
        questionCountdown = packet.Delay;
        FormManager.Instance.ChangeForm("quizquestion");
    }

    public void ShowAnswers()
    {
        FormManager.Instance.ChangeForm("answerquestion");
    }
}
