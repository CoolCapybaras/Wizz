using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool muted;
    public Slider slider;

    private float volumeBeforeMute;
    private Image buttonImage;

    public Sprite volumeActiveSprite;
    public Sprite volumeDisabledSprite;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();

        if (PlayerPrefs.HasKey("volume"))
            slider.value = PlayerPrefs.GetFloat("volume");
    }

    public void OnPressed()
    {
        if (slider.value != 0)
        {
            volumeBeforeMute = slider.value;
            slider.value = 0;
            return;
        }

        slider.value = volumeBeforeMute != 0 ? volumeBeforeMute : 1;
    }
    
    public void OnSliderValueChanged()
    {
        SoundManager.Instance.SetSound(slider.value);
        
        PlayerPrefs.SetFloat("volume", slider.value);
        PlayerPrefs.Save();
        if (slider.value != 0)
        {
            buttonImage.sprite = volumeActiveSprite;
            return;
        }

        buttonImage.sprite = volumeDisabledSprite;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        slider.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slider.gameObject.SetActive(false);
    }
}
