using Net.Packets.Clientbound;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

        public GameObject questionTypeObj;
        public Image questionTypeImage;
        public TextMeshProUGUI questionTypeText;
        
        public Sprite defaultQuestionSprite;
        public Sprite trueOrFalseQuestionSprite;
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
        AnimateQuestionType();
        time = 0;
        timerStarted = true;
        SoundManager.Instance.StartCountdown((int)questionCountdown);
        SoundManager.Instance.StopMusic();
    }

    public void AnimateQuestionType()
    {
        form.questionTypeObj.SetActive(true);
        var question = gameManager.questions[gameManager.currentQuestionIndex - 1];
        
        switch (question.Type)
        {
            case QuizQuestionType.Default:
                form.questionTypeText.text = "Обычный вопрос";
                form.questionTypeImage.sprite = form.defaultQuestionSprite;
                break;
            case QuizQuestionType.TrueOrFalse:
                form.questionTypeText.text = "Правда или ложь";
                form.questionTypeImage.sprite = form.trueOrFalseQuestionSprite;
                break;
            case QuizQuestionType.Multiple:
                form.questionTypeText.text = "Множественный выбор";
                // TODO: sprite
                break;
            case QuizQuestionType.Input:
                form.questionTypeText.text = "Ввод ответа";
                // TODO: sprite
                break;
        }
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
