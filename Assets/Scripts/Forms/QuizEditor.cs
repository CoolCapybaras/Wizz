using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Net.Packets;
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
        public GameObject defaultAnswerPrefab;
        public GameObject trueFalseAnswerPrefab;
        public GameObject inputAnswerPrefab;
        public GameObject matchAnswerPrefab;

        public Transform hashtagsLayout;
        public GameObject hashtagsPrefab;

        public VerticalLayoutGroup mainLayout;

        public Sprite defaultSprite;

        public Color backgroundColor;
    }

    //public Color backgroundColor;
    
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
    public bool needPublish;
    
    private Question AddQuestion(QuizQuestionType type, bool applyToQuiz = true)
    {
        var obj = Instantiate(form.questionPrefab, form.questionsLayout);
        
        obj.GetComponent<QuizEditorQuestionHelper>().expandedSize =
            type == QuizQuestionType.Match ? new(1600, 850) : new(1600, 500);
        
        questions.Add(new Question
        {
            obj = obj,
            count = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>(),
            question = obj.transform.GetChild(2).GetComponent<TMP_InputField>(),
            image = obj.transform.GetChild(6).GetComponent<RawImage>(),
            time = obj.transform.GetChild(7).GetChild(0).GetComponent<TMP_InputField>()
        });
        var i = questions.Count - 1;
        questions[i].answers = InstantiateAnswers(obj.transform.GetChild(4), type, i);
        questions[i].obj.GetComponent<QuizEditorQuestionHelper>().AnswerIndex = i;
        questions[i].count.text = $"{i + 1}.";
        
        if (applyToQuiz)
        {
            quiz.Questions.Add(new QuizQuestion());
            quiz.Questions[i].RightAnswer = new();
            quiz.Questions[i].Answers = new List<string>();
            quiz.Questions[i].RightAnswer.Id = -1;

            quiz.Questions[i].RightAnswer.Ids = 
                type == QuizQuestionType.Match ? new byte[] { 0, 1, 2, 3 } : new byte[4];
            
            quiz.Questions[i].RightAnswer.Type = type;
            quiz.Questions[i].Type = type;
            
            
            for (int m = 0; m < questions[i].answers.Count; m++)
            {
                switch (type)
                {
                    case QuizQuestionType.Default:
                    case QuizQuestionType.Multiple:
                        quiz.Questions[i].Answers.Add(string.Empty);
                        break;
                    case QuizQuestionType.TrueOrFalse:
                        quiz.Questions[i].Answers.Add(m == 0 ? "Правда" : "Ложь");
                        break;
                    case QuizQuestionType.Match:
                        quiz.Questions[i].Answers.AddRange(new []{string.Empty, string.Empty});
                        break;
                }
            }
        }

        UpdateContentSpacing();

        return questions.Last();
    }

    private List<GameObject> InstantiateAnswers(Transform parent, QuizQuestionType type, int answerIndex)
    {
        DestroyLayoutChildren(parent);
        
        var answers = new List<GameObject>();

        var iterLimit = 4;
        switch (type)
        {
            case QuizQuestionType.TrueOrFalse:
                iterLimit = 2;
                break;
            case QuizQuestionType.Input:
                iterLimit = 1;
                break;
        }
        
        for (int j = 0; j < iterLimit; ++j)
        {
            var answerPrefab = form.defaultAnswerPrefab;

            switch (type)
            {
                case QuizQuestionType.TrueOrFalse:
                    answerPrefab = form.trueFalseAnswerPrefab;
                    break;
                case QuizQuestionType.Multiple:
                    answerPrefab = form.defaultAnswerPrefab;
                    break;
                case QuizQuestionType.Input:
                    answerPrefab = form.inputAnswerPrefab;
                    break;
                case QuizQuestionType.Match:
                    answerPrefab = form.matchAnswerPrefab;
                    break;
            }
            
            var answerObj = Instantiate(answerPrefab, parent);

            answerObj.GetComponent<QuizAnswerHelper>().AnswerIndex = answerIndex;
            answers.Add(answerObj);
            
            if (type == QuizQuestionType.TrueOrFalse)
                answerObj.transform.GetChild(1).GetComponent<TMP_InputField>().text = j == 0 ? "Правда" : "Ложь";

            if (type == QuizQuestionType.Match)
                answerObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{j + 1}.";
        }

        return answers;
    }
    
    
    public void OnAddQuestionPressed(int questionType)
    {
        AddQuestion((QuizQuestionType)questionType);
    }

    public void UpdateContentSpacing()
    {
        form.mainLayout.spacing = form.mainLayout.spacing == 30f ? 30.1f : 30f;
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
        if (quiz.Hashtags.Count == 3)
        {
            OverlayManager.Instance.ShowInfo("Максимум 3 хештега", InfoType.Error);
            return;
        }
        
        if (string.IsNullOrEmpty(form.quizHashtags.text) || string.IsNullOrWhiteSpace(form.quizHashtags.text)) 
            return;
        
        quiz.Hashtags.Add($"#{form.quizHashtags.text}");
        var obj = Instantiate(form.hashtagsPrefab, form.hashtagsLayout);
        obj.GetComponent<TextMeshProUGUI>().text = $"#{form.quizHashtags.text}";
        form.quizHashtags.text = string.Empty;
    }

    public void OnQuizHashtagPressed(string text)
    {
        quiz.Hashtags.Remove(text);
    }
    
    public void OnQuizImagePressed()
    {
        Helpers.GetTexture(OnQuizImageChanged);
    }

    public void OnQuizImageChanged(Texture2D image)
    {
        quiz.Image = new ByteImage(image.EncodeToJPG());
        form.quizImage.texture = image;
    }
    
    public void OnQuestionValueChanged(int answerIndex)
    {
        quiz.Questions[answerIndex].Question = questions[answerIndex].question.text;
    }

    public void OnAnswerValueChanged(int index, int answerIndex)
    {
        if (quiz.Questions[answerIndex].Type == QuizQuestionType.Input)
            SetCorrectAnswerOnInputQuestion(index, answerIndex);
        else if (quiz.Questions[answerIndex].Type == QuizQuestionType.Match)
            SetCorrectAnswerOnMatchQuestion(index, answerIndex);
        else
        {
            quiz.Questions[answerIndex].Answers[index] =
                questions[answerIndex].answers[index].transform.GetChild(1).GetComponent<TMP_InputField>().text;
        }
    }
    
    public void OnTogglePressed(int index, int answerIndex)
    {
        switch (quiz.Questions[answerIndex].Type)
        {
            case QuizQuestionType.Default:
            case QuizQuestionType.TrueOrFalse:
                SetCorrectAnswerOnDefaultQuestion(index, answerIndex);
                break;
            case QuizQuestionType.Multiple:
                SetCorrectAnswerOnMultipleQuestion(index, answerIndex);
                break;
        }
    }

    private void SetCorrectAnswerOnDefaultQuestion(int index, int answerIndex)
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
            quiz.Questions[answerIndex].RightAnswer.Id = -1;
        }
        quiz.Questions[answerIndex].RightAnswer.Id = index;
    }

    private void SetCorrectAnswerOnMatchQuestion(int index, int answerIndex)
    {
        quiz.Questions[answerIndex].Answers[index] = questions[answerIndex].answers[index].transform.GetChild(0)
            .GetComponent<TMP_InputField>().text;
        quiz.Questions[answerIndex].Answers[index + 4] = questions[answerIndex].answers[index].transform.GetChild(1)
            .GetComponent<TMP_InputField>().text;
    }

    private void SetCorrectAnswerOnMultipleQuestion(int index, int answerIndex)
    {
        var toggle = questions[answerIndex].answers[index].GetComponent<Toggle>();
        
        quiz.Questions[answerIndex].RightAnswer.Ids[index] = toggle.isOn ? (byte)1 : (byte)0;
    }

    private void SetCorrectAnswerOnInputQuestion(int index, int answerIndex)
    {
        var inputField = questions[answerIndex].answers[index].transform.GetChild(1).GetComponent<TMP_InputField>();

        quiz.Questions[answerIndex].RightAnswer.Input = inputField.text;
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

    private void OnQuestionImageChanged(Texture2D image, int answerIndex)
    {
        questions[answerIndex].image.texture = image;
        quiz.Questions[answerIndex].Image = new ByteImage(image.EncodeToJPG());
    }

    public void OnDeleteQuestionPressed(int answerIndex)
    {
        Destroy(questions[answerIndex].obj);
        questions.RemoveAt(answerIndex);
        quiz.Questions.RemoveAt(answerIndex);
        RefreshInstantiatedQuestions();

        UpdateContentSpacing();
    }

    private void RefreshInstantiatedQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            questions[i].obj.GetComponent<QuizEditorQuestionHelper>().AnswerIndex = i;
            questions[i].count.text = $"{i + 1}.";
        }
    }

    public void OnQuizDeletePressed()
    {
        if (quiz.Id != 0)
            LocalClient.instance.SendPacket(new EditQuizPacket {Type = EditQuizType.Delete, QuizId = quiz.Id});
        
        FormManager.Instance.ChangeForm("mainmenu");
    }

    public void SaveQuiz()
    {
        if(!CheckQuizCorrectness())
            return;
        
        LocalClient.instance.SendPacket(new EditQuizPacket { Quiz = quiz, Type = EditQuizType.Upload });
    }

    public void PublishQuiz()
    {
        needPublish = true;
        SaveQuiz();
    }

    private bool CheckQuizCorrectness()
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

            switch (question.Type)
            {
                case QuizQuestionType.Default:
                case QuizQuestionType.TrueOrFalse:
                    if (question.RightAnswer.Id == -1)
                    {
                        flag = false;
                        OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не указан верный ответ", InfoType.Error);
                    }
                    break;
                case QuizQuestionType.Multiple:
                    var hasAnswer = false;
                    foreach (var answer in question.RightAnswer.Ids)
                    {
                        if (answer == (byte)1)
                            hasAnswer = true;
                    }

                    if (!hasAnswer)
                    {
                        flag = false;
                        OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не указан верный ответ", InfoType.Error);
                    }
                    break;
                case QuizQuestionType.Input:
                    if (string.IsNullOrEmpty(question.RightAnswer.Input))
                    {
                        flag = false;
                        OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не указан верный ответ", InfoType.Error);
                    }
                    break;
            }

            if (question.Type != QuizQuestionType.Input)
            {
                for (int j = 0; j < question.Answers.Count; j++)
                {
                    if (string.IsNullOrEmpty(question.Answers[j]))
                    {
                        flag = false;
                        OverlayManager.Instance.ShowInfo($"Вопрос {i + 1}: не указан {j + 1} вариант ответа",
                            InfoType.Error);
                    }
                }
            }
        }
        return flag;
    }

    public void OnEditQuizPressed(int quizId)
    {
        LocalClient.instance.SendPacket(new EditQuizPacket {Type = EditQuizType.Get, QuizId = quizId});
    }

    public void OnGetQuizResult(EditQuizPacket packet)
    {
        questions = new();
        FormManager.Instance.ChangeForm("quizeditor");
        quiz = packet.Quiz;
        FillUI();
    }

    private void DestroyLayoutChildren(Transform layout)
    {
        for (int i = 0; i < layout.childCount; ++i)
            Destroy(layout.GetChild(i).gameObject);
    }
    
    private void FillUI()
    {
        DestroyLayoutChildren(form.questionsLayout);
        DestroyLayoutChildren(form.hashtagsLayout);
        form.quizImage.texture = quiz.Image == null ? form.defaultSprite.texture : quiz.Image.GetTexture();
        form.quizName.text = quiz.Name;
        form.quizDescription.text = quiz.Description;
        form.quizHashtags.text = string.Empty;

        if (quiz == null)
            return;

        FillQuestionsUI();
        FillHashtagsUI();
        OnBackgroundColorButtonPressed((int)quiz.Color);
    }

    public void OnBackgroundColorButtonPressed(int color)
    {
        quiz.Color = (HexColor)color;
        OverlayManager.Instance.ChangeBackgroundColor(quiz.Color);
    }

    private void FillQuestionsUI()
    {
        foreach (var t in quiz.Questions)
        {
            var question = AddQuestion(t.Type, false);
            question.question.text = t.Question;
            question.time.text = t.Time.ToString();
            question.image.texture = t.Image.GetTexture();
            for (int j = 0; j < question.answers.Count; ++j)
            {
                if (t.Type is not QuizQuestionType.Input and not QuizQuestionType.Match)
                    question.answers[j].transform.GetChild(1).GetComponent<TMP_InputField>().text = t.Answers[j];
                switch (t.Type)
                {
                    case QuizQuestionType.Default:
                    case QuizQuestionType.TrueOrFalse:
                        if(t.RightAnswer.Id == j)
                            question.answers[j].GetComponent<Toggle>().isOn = true;
                        break;
                    case QuizQuestionType.Multiple:
                        if (t.RightAnswer.Ids[j] == 1)
                            question.answers[j].GetComponent<Toggle>().isOn = true;
                        break;
                    case QuizQuestionType.Input:
                        question.answers[j].transform.GetChild(1).GetComponent<TMP_InputField>().text = t.RightAnswer.Input;
                        break;
                    case QuizQuestionType.Match:
                        var inputField1 = question.answers[j].transform.GetChild(0).GetComponent<TMP_InputField>();
                        var inputField1Action = inputField1.onValueChanged;
                        inputField1.onValueChanged = null;
                        
                        var inputField2 = question.answers[j].transform.GetChild(1).GetComponent<TMP_InputField>();
                        var inputField2Action = inputField2.onValueChanged;
                        inputField2.onValueChanged = null;
                        
                        question.answers[j].transform.GetChild(0).GetComponent<TMP_InputField>().text = t.Answers[j];
                        question.answers[j].transform.GetChild(1).GetComponent<TMP_InputField>().text = t.Answers[j + 4];

                        inputField1.onValueChanged = inputField1Action;
                        inputField2.onValueChanged = inputField2Action;
                        break;
                }
            }
        }
    }

    private void FillHashtagsUI()
    {
        quiz.Hashtags = new(); // TODO: убрать
        // TODO
    }

    public void CreateNewQuiz()
    {
        questions = new();
        quiz = new()
        {
            Questions = new(),
            Hashtags = new(),
        };
        
        FillUI();
    }
    
    public void InitializeForm()
    {
    }
}
