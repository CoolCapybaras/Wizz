using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class QuizEditorQuestionHelper : MonoBehaviour
{
    private bool expanded;
    public Vector2 expandedSize = new Vector2(1600, 500);
    public Vector2 minimizedSize = new Vector2(1600, 100);
    public Transform fakeButton;

    public void OnPressed()
    {
        expanded = !expanded;

        var sequence = DOTween.Sequence();
        if (expanded)
            sequence.Insert(0, gameObject.GetComponent<RectTransform>().DOSizeDelta(expandedSize, 0.25f))
                .Insert(0, fakeButton.DORotateQuaternion(Quaternion.Euler(0, 0, 180), 0.25f));
        else
            sequence.Insert(0, gameObject.GetComponent<RectTransform>().DOSizeDelta(minimizedSize, 0.25f))
                .Insert(0, fakeButton.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.25f));

        sequence.Play();
    }
    
    public int AnswerIndex;

    public void OnQuestionValueChanged()
    {
        QuizEditor.Instance.OnQuestionValueChanged(AnswerIndex);
    }
    
    public void OnTimeValueChanged()
    {
        QuizEditor.Instance.OnTimeValueChanged(AnswerIndex);
    }

    public void OnQuestionImagePressed()
    {
        QuizEditor.Instance.OnQuestionImagePressed(AnswerIndex);
    }

    public void OnDeleteQuestionPressed()
    {
        QuizEditor.Instance.OnDeleteQuestionPressed(AnswerIndex);
    }
}
