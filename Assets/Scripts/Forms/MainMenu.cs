using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Net.Packets.Clientbound;
using Net.Packets.Serverbound;

public class MainMenu : MonoBehaviour, IForm
{
    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI topGreetingText;
        public Transform quizzesLayout;

        public GameObject quizPrefab;
        public GameObject createQuizPrefab;
    }

    public static MainMenu Instance;
    public Form form;
    private LocalClient localClient;
    private List<Quiz> quizzes = new();

    private int searchTriesCount;
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
        AnimateForm();
        form.topGreetingText.text = $"Пора создавать новое, {localClient.Name}!";
        quizzes.Clear();
        SearchForQuizzes();
    }

    private void AnimateForm()
    {
        var sequence = DOTween.Sequence();
        sequence.Insert(0, form.topGreetingText.transform.DOLocalMoveX(-825f, 1, true).From(-2500))
            .Insert(0, form.topGreetingText.DOFade(1, 1).From(0))
            .Insert(0, form.quizzesLayout.GetComponent<CanvasGroup>().DOFade(1, 1).From(0))
            .Play();
    }
    
    private void SearchForQuizzes(bool isAuthor = true, int count = 3)
    {
        LocalClient.instance.SendPacket(new SearchPacket {SearchType = SearchType.Author, Count = count});
        SearchResultPacket.RequestQueue.Enqueue(0);
    }

    public void OnSearchResult(SearchResultPacket packet)
    {
        quizzes.AddRange(packet.Quizzes);
        InstantiateQuizzes();
    }
    
    public void RemoveQuizzesFromLayout()
    {
        var count = form.quizzesLayout.childCount;
        for (int i = 0; i < count; ++i)
            Destroy(form.quizzesLayout.GetChild(i).gameObject);
    }
    
    private void InstantiateQuizzes()
    {
        RemoveQuizzesFromLayout();
        if (quizzes.Count == 0)
        {
            Instantiate(form.createQuizPrefab, form.quizzesLayout);
            return;
        }
        StartCoroutine(InstantiateQuizzesCoroutine());
    }

    private IEnumerator InstantiateQuizzesCoroutine()
    {
        foreach(var quiz in quizzes)
        {
            var obj = Instantiate(form.quizPrefab, form.quizzesLayout);
            obj.GetComponent<QuizButton>().quizId = quiz.Id;

            var transform = obj.transform.GetChild(1);
            obj.transform.GetChild(0).GetComponent<RawImage>().texture = quiz.Image.GetTexture();
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = quiz.Description;
            transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = quiz.Name;
            // TODO: transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = quiz.Hashtags;
            if (quiz.AuthorId != LocalClient.instance.Id)
            {
                obj.transform.GetChild(3).gameObject.SetActive(false);
                obj.transform.GetChild(4).gameObject.SetActive(false);
                obj.transform.GetChild(5).gameObject.SetActive(false);
            }
            else
            {
                if (quiz.ModerationStatus is not (ModerationStatus.ModerationRejected
                    or ModerationStatus.NotModerated))
                {
                    obj.transform.GetChild(5).gameObject.SetActive(false);
                    continue;
                }
                obj.transform.GetChild(5).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    quiz.ModerationStatus == ModerationStatus.NotModerated ? "Не опубликована" : "Не прошла модерацию";
            }
            
            yield return null;
        }
    }
}
