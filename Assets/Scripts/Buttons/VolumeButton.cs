using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    private bool muted;

    public void OnPressed()
    {
        muted = !muted;
        SoundManager.Instance.SetSound(muted);
    }
}
