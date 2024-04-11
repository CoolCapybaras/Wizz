using System.Collections;
using System.Collections.Generic;
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
        gameObject.GetComponent<RectTransform>().sizeDelta = expanded ? expandedSize : minimizedSize;
        fakeButton.localRotation = expanded ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0);
    }
}
