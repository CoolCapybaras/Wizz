using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizAnswerHelper : MonoBehaviour
{
    public int AnswerIndex;
    public void OnAnswerValueChanged()
    {
        QuizEditor.Instance.OnAnswerValueChanged(transform.GetSiblingIndex(), AnswerIndex);
    }
    
    public void OnTogglePressed()
    {
        QuizEditor.Instance.OnTogglePressed(transform.GetSiblingIndex(), AnswerIndex);
    }

}
