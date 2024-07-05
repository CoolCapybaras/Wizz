using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNewQuizButton : MonoBehaviour
{
    public void OnPressed()
    {
        QuizEditor.Instance.CreateNewQuiz();
        FormManager.Instance.ChangeForm("quizeditor");
    }
}
