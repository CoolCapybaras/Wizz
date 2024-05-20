using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HashtagButton : MonoBehaviour
{
    public void OnPressed()
    {
        QuizEditor.Instance.OnQuizHashtagPressed(GetComponent<TextMeshProUGUI>().text);
        Destroy(gameObject);
    }
}
