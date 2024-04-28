using Net.Packets.Clientbound;
using Net.Packets.Serverbound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MyQuizzes : MonoBehaviour, IForm
{
    [Serializable]
    public struct Form
    {
        public TextMeshProUGUI topText;
        public TMP_InputField searchInputField;
        public Transform quizzesLayout;
        public GameObject quizPrefab;
    }

    public static MyQuizzes Instance;
    public Form form;

    public List<Quiz> quizzes;

    private void Awake()
    {
        Instance = this;
    }

    public void OnSearchEndEdit(string str)
    {
        LocalClient.instance.SendPacket(new SearchPacket() { QuizName = form.searchInputField.text });
    }

    public void RemoveQuizzesFromLayout()
    {
        var count = form.quizzesLayout.childCount;
        for (int i = 0; i < count; ++i)
            Destroy(form.quizzesLayout.GetChild(i).gameObject);
    }

    public void InstantiateQuizzes()
    {
        RemoveQuizzesFromLayout();

        foreach(var quiz in quizzes)
        {
            var obj = Instantiate(form.quizPrefab, form.quizzesLayout);

            obj.GetComponent<QuizButton>().quizId = quiz.Id;

            var transform = obj.transform.GetChild(0);
            transform.GetChild(0).GetComponent<RawImage>().texture = quiz.Image;
            transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = quiz.Description;
            transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = quiz.Name;
            // TODO: transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = quiz.Hashtags;
            if (quiz.AuthorId != LocalClient.instance.Id)
            {
                transform.GetChild(6).gameObject.SetActive(false);
                transform.GetChild(7).gameObject.SetActive(false);
            }
        }
    }

    public void InitializeForm()
    {
        RemoveQuizzesFromLayout();
        OnSearchEndEdit(string.Empty);
    }

    public void OnSearchResult(SearchResultPacket packet)
    {
        quizzes = packet.Quizzes.ToList();
        InstantiateQuizzes();
    }
}
