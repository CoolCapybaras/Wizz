using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizEditor : MonoBehaviour, IForm
{
    public static QuizEditor Instance;

    public void Awake()
    {
        Instance = this;
        InitializeForm();
    }

    [Serializable]
    public struct Form
    {
        public TMP_InputField quizName;
        public RawImage quizImage;
        public TMP_InputField quizDescription;
        public TMP_InputField quizHashtags;
        public Transform questionsLayout;
        public GameObject questionPrefab;
    }

    public Form form;

    public List<Question> questions = new List<Question>();

    public class Question
    {
        public GameObject obj;
        public TMP_InputField question;
        public List<GameObject> answers;
        public RawImage image;
        public TMP_InputField time;
        public TextMeshProUGUI count;
    }

    public Quiz quiz;
    
    public void AddQuestion()
    {
        var obj = Instantiate(form.questionPrefab, form.questionsLayout);
        
        var answers = new List<GameObject>();
        for (int j = 0; j < obj.transform.GetChild(3).childCount; j++)
        {
            answers.Add(obj.transform.GetChild(3).GetChild(j).gameObject);
        }

        questions.Add(new Question()
        {
            obj = obj,
            count = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>(),
            question = obj.transform.GetChild(1).GetComponent<TMP_InputField>(),
            image = obj.transform.GetChild(5).GetComponent<RawImage>(),
            answers = answers,
            time = obj.transform.GetChild(6).GetChild(0).GetComponent<TMP_InputField>()
        });

        var i = questions.Count - 1;
        questions[i].obj.GetComponent<QuizEditorQuestionHelper>().AnswerIndex = i;

        questions[i].count.text = $"{i + 1}.";
        quiz.Questions.Add(new QuizQuestion());
        quiz.Questions[i].Answers = new List<string>();
        quiz.Questions[i].AnswerIndex = -1;
        for (int m = 0; m < answers.Count; m++)
        {
            quiz.Questions[i].Answers.Add(string.Empty);
        }
    }

    public void OnQuizNameValueChanged()
    {
        quiz.Name = form.quizName.text;
    }

    public void OnQuizDescriptionValueChanged()
    {
        quiz.Description = form.quizDescription.text;
    }

    public void OnQuizHashtagsEndEdit()
    {
        //quiz.
        //TODO
    }

    public void OnQuizImagePressed()
    {
        Helpers.GetTexture((image) =>
        {
            OnQuizImageChanged(image);
        });
    }

    public void OnQuizImageChanged(Texture2D image)
    {
        quiz.Image = new ByteImage(image.GetRawTextureData());
        form.quizImage.texture = image;
    }
    
    public void OnQuestionValueChanged(int answerIndex)
    {
        quiz.Questions[answerIndex].Question = questions[answerIndex].question.text;
    }

    public void OnAnswerValueChanged(int index, int answerIndex)
    {
        quiz.Questions[answerIndex].Answers[index] =
            questions[answerIndex].answers[index].transform.GetChild(1).GetComponent<TMP_InputField>().text;
    }
    
    public void OnTogglePressed(int index, int answerIndex)
    {
        for (int i = 0; i < questions[answerIndex].answers.Count; i++)
        {
            var toggle = questions[answerIndex].answers[i].GetComponent<Toggle>();
            if (i != index)
            {
                var temp = toggle.onValueChanged;
                toggle.onValueChanged = new Toggle.ToggleEvent();
                toggle.GetComponent<Toggle>().isOn = false;
                toggle.onValueChanged = temp;
            }
        }

        if (!questions[answerIndex].answers[index].GetComponent<Toggle>().isOn)
        {
            quiz.Questions[answerIndex].AnswerIndex = -1;
        }
        quiz.Questions[answerIndex].AnswerIndex = index;
    }
    
    public void OnTimeValueChanged(int answerIndex)
    {
        quiz.Questions[answerIndex].Time = int.Parse(questions[answerIndex].time.text);
    }

    public void OnQuestionImagePressed(int answerIndex)
    {
        Helpers.GetTexture((image) =>
        {
            OnQuestionImageChanged(image, answerIndex);
        });
    }

    public void OnQuestionImageChanged(Texture2D image, int answerIndex)
    {
        questions[answerIndex].image.texture = image;
        quiz.Questions[answerIndex].Image = new ByteImage(image.GetRawTextureData());
    }

    public void OnDeleteQuestionPressed(int answerIndex)
    {
        Destroy(questions[answerIndex].obj);
        questions.RemoveAt(answerIndex);
        quiz.Questions.RemoveAt(answerIndex);
        RefreshInstantiatedQuestions();
    }

    public void RefreshInstantiatedQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            questions[i].obj.GetComponent<QuizEditorQuestionHelper>().AnswerIndex = i;
            questions[i].count.text = $"{i + 1}.";
        }
    }

    public void OnQuizDeletePressed()
    {
        
    }

    public void SaveQuiz()
    {
        if(!CheckQuizCorrectness())
            return;
        
    }

    public bool CheckQuizCorrectness()
    {
        var flag = true;
        if (string.IsNullOrEmpty(quiz.Name) || quiz.Name.Length > 48 || quiz.Name.Length < 3)
        {
            flag = false;
            OverlayManager.Instance.ShowInfo("Длина имени викторины должна быть от 3 до 48 символов", InfoType.Error);
        }

        if (quiz.Image == null)
        {
            flag = false;
            OverlayManager.Instance.ShowInfo("Не установлена картинка викторины", InfoType.Error);
        }

        if (string.IsNullOrEmpty(quiz.Description) || quiz.Description.Length > 128 || quiz.Description.Length < 3)
        {
            flag = false;
            OverlayManager.Instance.ShowInfo("Длина описания викторины должна быть от 3 до 128 символов", InfoType.Error);
        }

        if (quiz.Questions.Count == 0)
        {
            flag = false;
            OverlayManager.Instance.ShowInfo("Не создано ни одного вопроса", InfoType.Error);
        }

        for (int i = 0; i < quiz.Questions.Count; i++)
        {
            var question = quiz.Questions[i];
            if (string.IsNullOrEmpty(question.Question))
            {
                flag = false;
                OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не указан вопрос", InfoType.Error);
            }

            if (question.Image == null)
            {
                flag = false;
                OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не установлена картинка", InfoType.Error);
            }

            if (question.Time == 0)
            {
                flag = false;
                OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не указано время ответа", InfoType.Error);
            }

            if (question.AnswerIndex == -1)
            {
                flag = false;
                OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не указан верный ответ", InfoType.Error);
            }

            for (int j = 0; j < question.Answers.Count; j++)
            {
                if (string.IsNullOrEmpty(question.Answers[j]))
                {
                    flag = false;
                    OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не указан {j + 1} вариант ответа", InfoType.Error);
                }
            }
        }
        return flag;
    }
    
    public void InitializeForm()
    {
        quiz = new Quiz();
        quiz.Questions = new();
    }
}
