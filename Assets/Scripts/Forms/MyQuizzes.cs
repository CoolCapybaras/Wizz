using Net.Packets.Clientbound;
using Net.Packets.Serverbound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyQuizzes : MonoBehaviour, IForm
{
    [Serializable]
    public enum QuizzesType
    {
        My,
        Server
    }

    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI topText;
        public TMP_InputField searchInputField;
        public Transform quizzesLayout;
        public GameObject quizPrefab;
        public GameObject createQuizPrefab;

        public ScrollRect quizzesScroll;
    }

    public static MyQuizzes Instance;
    public Form form;

    private List<Quiz> quizzes = new();

    private QuizzesType type;

    private int quizOffset;
    private bool quizzesListEnded;
    private void Awake()
    {
        Instance = this;
    }

    public void OnSearchEndEdit(string str)
    {
        quizOffset = 0;
        quizzes.Clear();
        quizzesListEnded = false;
        RemoveQuizzesFromLayout();
        SearchForQuizzes(form.searchInputField.text, 10, quizOffset);
    }

    private void SearchForQuizzes(string quizName, int count, int offset)
    {
        if (SearchResultPacket.RequestQueue.Count != 0) return;
        
        LocalClient.instance.SendPacket(type == QuizzesType.Server
            ? new SearchPacket { QuizName = quizName, Count = count, Offset = offset}
            : new SearchPacket { QuizName = quizName, Count = count, Offset = offset, SearchType = SearchType.Author });
        SearchResultPacket.RequestQueue.Enqueue(1);
    }
    
    private void RemoveQuizzesFromLayout()
    {
        var count = form.quizzesLayout.childCount;
        for (int i = 0; i < count; ++i)
            Destroy(form.quizzesLayout.GetChild(i).gameObject);
        
        if (type == QuizzesType.My)
            Instantiate(form.createQuizPrefab, form.quizzesLayout);
    }

    public void OnScrollValueChanged()
    {
        if (form.quizzesScroll.verticalNormalizedPosition > 0.3f 
            || SearchResultPacket.RequestQueue.Count != 0
            || !form.quizzesScroll.verticalScrollbar.isActiveAndEnabled
            || quizzesListEnded) return;

        quizOffset += 10;
        SearchForQuizzes(form.searchInputField.text, 10, quizOffset);
    }

    private void InstantiateQuizzes()
    {
        StartCoroutine(InstantiateQuizzesCoroutine());
    }

    private IEnumerator InstantiateQuizzesCoroutine()
    {
        for (int i = quizOffset; i < quizzes.Count; ++i)
        {
            var quiz = quizzes[i];
            var obj = Instantiate(form.quizPrefab, form.quizzesLayout);

            obj.GetComponent<QuizButton>().quizId = quiz.Id;

            var transform = obj.transform.GetChild(2);
            obj.transform.GetChild(0).GetComponent<RawImage>().texture = quiz.Image.GetTexture();
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = quiz.Description;
            transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = quiz.Name;
            obj.transform.GetChild(7).GetChild(0).GetComponent<Image>().fillAmount =
                Helpers.ConvertRange(0, 5, 0, 1, quiz.Score);
            obj.transform.GetChild(7).GetChild(1).GetComponent<TextMeshProUGUI>().text = quiz.Score.ToString();
            // TODO: transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = quiz.Hashtags;
            if (quiz.AuthorId != LocalClient.instance.Id)
            {
                obj.transform.GetChild(4).gameObject.SetActive(false);
                obj.transform.GetChild(5).gameObject.SetActive(false);
                obj.transform.GetChild(6).gameObject.SetActive(false);
            }
            else
            {
                if (quiz.ModerationStatus is not (ModerationStatus.ModerationRejected
                    or ModerationStatus.NotModerated))
                {
                    obj.transform.GetChild(6).gameObject.SetActive(false);
                    continue;
                }
                obj.transform.GetChild(6).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                    quiz.ModerationStatus == ModerationStatus.NotModerated ? "Не опубликована" : "Не прошла модерацию";
            }

            yield return null;
        }
    }
    
    public void InitializeForm()
    {
        if (type == QuizzesType.My)
        {
            form.topText.text = "Мои викторины";
        }
        else if (type == QuizzesType.Server)
            form.topText.text = "Поиск викторин";
        
        quizzes.Clear();
        quizzesListEnded = false;
        quizOffset = 0;
        OnSearchEndEdit(string.Empty);
        RemoveQuizzesFromLayout();
    }

    public void SetQuizzesList(int type)
    {
        this.type = (QuizzesType)type;
    }

    public void OnSearchResult(SearchResultPacket packet)
    {
        quizzes.AddRange(packet.Quizzes.ToList());
        if (packet.Quizzes.Length < 10)
            quizzesListEnded = true;
        
        InstantiateQuizzes();
    }
}
