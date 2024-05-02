using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeButton : MonoBehaviour
{
    public GameObject slider;

    public void OnHover()
    {
        slider.SetActive(true);
    }

    public void OnExitHover()
    {
        slider.SetActive(false);
    }
}
