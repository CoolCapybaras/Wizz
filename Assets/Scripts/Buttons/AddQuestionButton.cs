using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddQuestionButton : MonoBehaviour
{
    public GameObject dropdown;
    public VerticalLayoutGroup questionsLayout;
    private bool isDropdownShown;
    public void OnPressed()
    {
        isDropdownShown = !isDropdownShown;
        questionsLayout.padding.top = isDropdownShown ? dropdown.transform.childCount * 50 : 0;
        questionsLayout.SetLayoutVertical();
        dropdown.SetActive(isDropdownShown);
    }
}
