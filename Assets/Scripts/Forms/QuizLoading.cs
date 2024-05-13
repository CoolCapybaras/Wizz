using Net.Packets.Clientbound;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuizLoading : MonoBehaviour, IForm
{
    public static QuizLoading Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI quizNameText;
        public TextMeshProUGUI timerText;
    }

    public Form form;

    private bool timerStarted;
    private float time;
    private float countdownTime = 3f;

    private void Update()
    {
        if (!timerStarted || time <= 0.5f)
            return;

        time -= Time.deltaTime;

        form.timerText.text = $"{Mathf.Ceil(time)}...";
    }

    public void InitializeForm()
    {
        GameManager.Instance.currentQuestionCount = 0;
        form.quizNameText.text = GameManager.Instance.currentQuiz.Name;
        form.timerText.text = "Загрузка...";
        timerStarted = false;
        SoundManager.Instance.SetLowPassFilter(true, 1);
    }

    public void OnTimerStarted(TimerStartedPacket packet)
    {
        timerStarted = true;
        time = countdownTime;
    }
}
