using DG.Tweening;
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
        var sequence = DOTween.Sequence();
        sequence.Insert(0, GetComponent<CanvasGroup>().DOFade(1, 0.25f).From(0))
            .Play();
        yield return new WaitForSeconds(5f);
        DestroyInfo();
        yield return null;
    }

    private void DestroyInfo()
    {
        var sequence = DOTween.Sequence();
        sequence.Insert(0, GetComponent<CanvasGroup>().DOFade(0, 0.25f).From(1))
            .AppendCallback(() =>
            {
                Destroy(gameObject);
            })
            .Play();
    }
}
