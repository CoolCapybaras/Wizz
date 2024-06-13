using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPopupHelper : MonoBehaviour
{
    public void OnYesExitPressed()
    {
        OverlayManager.Instance.EnsureLeaved();
        Application.Quit();
    }

    public void OnExitNoPressed()
    {
        gameObject.SetActive(false);
    }
}
