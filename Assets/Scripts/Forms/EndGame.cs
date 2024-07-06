using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Net.Packets.Clientbound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour, IForm
{
    public static EndGame Instance;

    private void Awake()
    {
        Instance = this;
        gameManager = GameManager.Instance;
    }

    [Serializable]
    public struct Form
    {
        public GameObject[] pedestals;
        public Transform scoresLayout;
        public GameObject scorePrefab;

        public ParticleSystem[] particles;
    }
    
    

    private GameManager gameManager;
    public Form form;
    
    public void InstantiateScores()
    {
        RemoveScoresFromScroll();
        var scores = gameManager.CurrentScore.ToList();
        for (int i = 3; i < gameManager.CurrentScore.Count; i++)
        {
            var client = gameManager.GetClientById(scores[i].Key);
            var obj = Instantiate(form.scorePrefab, form.scoresLayout);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = client.Name;
            obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{scores[i].Value} баллов";
        }
    }

    public void OnMainMenuPressed()
    {
        OverlayManager.Instance.EnsureLeaved();
        FormManager.Instance.ChangeForm("mainmenu", FormManager.AnimType.Out);
    }

    public void RemoveScoresFromScroll()
    {
        for (int i = 0; i < form.scoresLayout.childCount; i++)
        {
            Destroy(form.scoresLayout.GetChild(i).gameObject);
        }
    }
    
    public void InitializeForm()
    {
        foreach (var pedestal in form.pedestals)
        {
            pedestal.SetActive(false);
        }

        var scores = gameManager.CurrentScore.ToList();
        for (int i = 0; i < Math.Min(3, gameManager.CurrentScore.Count); i++)
        {
            var pedestal = form.pedestals[i];
            var client = gameManager.GetClientById(scores[i].Key);
            pedestal.SetActive(true);
            pedestal.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = client.Name;
            pedestal.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{scores[i].Value} баллов";
            pedestal.transform.GetChild(0).GetChild(1).GetComponent<RawImage>().texture = client.Image.GetTexture();

            var transform = pedestal.GetComponent<RectTransform>();
            var sequence = DOTween.Sequence();
            sequence.Insert(0.5f * i, pedestal.GetComponent<RectTransform>()
                    .DOSizeDelta(transform.sizeDelta, 1)
                    .From(new Vector2(transform.sizeDelta.x, 0)))
                .Play();
        }

        if(scores.Count > 3)
            InstantiateScores();
        else
            RemoveScoresFromScroll();

        foreach(var particle in form.particles)
            particle.Play();
        
        SoundManager.Instance.PlayShortClip("endgame");
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.SetLowPassFilter(false, 1);
        SoundManager.Instance.PlayMusic("menu", true);
    }
}
