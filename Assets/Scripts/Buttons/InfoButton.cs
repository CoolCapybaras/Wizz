using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DissapearCoroutine());
    }

    public void OnPressed()
    {
        DestroyInfo();
    }

    private IEnumerator DissapearCoroutine()
    {
        yield return new WaitForSeconds(5f);
        DestroyInfo();
        yield return null;
    }

    private void DestroyInfo()
    {
        Destroy(gameObject);
    }
}
