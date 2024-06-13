using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSliderHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button volumeButton;

    public void OnPointerEnter(PointerEventData eventData)
    {
        volumeButton.interactable = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        volumeButton.interactable = true;
    }
}
