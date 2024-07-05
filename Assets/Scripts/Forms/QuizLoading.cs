using Net.Packets.Clientbound;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        public TextMeshProUGUI loadingText;
        public TextMeshProUGUI timerText;
        public GameObject countdownObj;
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
        form.quizNameText.gameObject.SetActive(true);
        form.loadingText.gameObject.SetActive(true);
        
        GameManager.Instance.currentQuestionIndex = 0;
        form.quizNameText.text = GameManager.Instance.currentQuiz.Name;
        form.timerText.gameObject.SetActive(false);
        form.countdownObj.gameObject.SetActive(false);
        timerStarted = false;
        SoundManager.Instance.SetLowPassFilter(true, 1);
    }

    public void OnTimerStarted(TimerStartedPacket packet)
    {
        timerStarted = true;
        time = countdownTime;
        SoundManager.Instance.StartCountdown(3);
        AnimateCountdown();
    }

    public void AnimateCountdown()
    {
        form.quizNameText.gameObject.SetActive(false);
        form.loadingText.gameObject.SetActive(false);
        
        form.timerText.gameObject.SetActive(true);
        form.countdownObj.gameObject.SetActive(true);
        var countdownImage = form.countdownObj.GetComponent<Image>();
        var countdownTransform = form.countdownObj.transform;
        var sequence = DOTween.Sequence();
        
        sequence.Insert(0, form.timerText.GetComponent<CanvasGroup>().DOFade(1, 0.5f).From(0))
            .Insert(0, form.timerText.transform.DOScale(1, 0.5f).From(0))
            .Insert(0, form.countdownObj.GetComponent<CanvasGroup>().DOFade(1, 0.5f).From(0))
            .Insert(0, countdownTransform.DOScale(1f, 0.5f).From(0.5f))
            .Insert(0, countdownTransform.DOLocalRotate(new Vector3(0, 0, 45), 0.5f).From(Vector3.zero))
            .Insert(1, countdownTransform.DOLocalRotate(new Vector3(0, 0, 90), 0.5f))
            .Insert(1, DOTween.To(
                () => countdownImage.pixelsPerUnitMultiplier, 
                x => countdownImage.pixelsPerUnitMultiplier = x, 
                7.5f, 0.5f).From(15f))
            .Insert(1, countdownTransform.DOScale(1.5f, 0.5f))
            .Insert(1, form.timerText.transform.DOScale(1.25f, 0.5f))
            .Insert(2, DOTween.To(
                () => countdownImage.pixelsPerUnitMultiplier, 
                x => countdownImage.pixelsPerUnitMultiplier = x, 
                2f, 1f))
            .Insert(2, countdownTransform.DOLocalRotate(new Vector3(0, 0, 175), 0.9f))
            .Insert(2, countdownTransform.DOScale(2f, 0.9f))
            .Insert(2, form.timerText.transform.DOScale(1.5f, 0.9f))
            .Play();
    }
}
