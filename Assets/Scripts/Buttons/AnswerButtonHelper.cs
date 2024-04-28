using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButtonHelper : MonoBehaviour
{
    public int index;

    public void OnPressed()
    {
        AnswerQuestion.Instance.OnPlayerAnswer(index);
    }
}
