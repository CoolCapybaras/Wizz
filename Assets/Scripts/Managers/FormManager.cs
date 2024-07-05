using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FormManager : MonoBehaviour
{
    public enum AnimType
    {
        In,
        Out
    }

    public List<string> QuizForms = new();

    public static FormManager Instance;
    
    public Quiz quiz;

    public RawImage backgroundColor;

    private void Awake()
    {
        WarmupForms();
        ChangeForm("login");
        Instance = this;
    }

    private void Start()
    {
        OverlayManager.Instance.SetActiveBottomButtons(false);
        OverlayManager.Instance.SetActiveTopButtons(false);
    }

    public List<Form> forms;
    public Form activeForm;

    public void ChangeForm(string id, AnimType animType = AnimType.In)
    {
        if (activeForm.Obj == null)
        {
            SetActiveForm(id);
            return;
        }

        switch (animType)
        {
            case AnimType.In:
                GetInSequence(id).Play();
                break;
            case AnimType.Out:
                GetOutSequence(id).Play();
                break;
        }
    }

    private Sequence GetInSequence(string id)
    {
        var inSequence = DOTween.Sequence();
        inSequence
            .InsertCallback(0, () =>
            {
                activeForm.Obj.SetActive(false);
            })
            .InsertCallback(0, () =>
            {
                SetActiveForm(id);
            })
            .Insert(0, GetFormById(id).Obj.transform.DOScale(1, 0.25f).From(0.8f))
            .Insert(0, GetFormById(id).Obj.GetComponent<CanvasGroup>().DOFade(1, 0.25f).From(0));
        return inSequence;
    }

    private Sequence GetOutSequence(string id)
    {
        var outSequence = DOTween.Sequence();
        outSequence
            .InsertCallback(0, () =>
            {
                activeForm.Obj.SetActive(false);
            })
            .InsertCallback(0, () =>
            {
                SetActiveForm(id);
            })
            .Insert(0, GetFormById(id).Obj.transform.DOScale(1, 0.25f).From(1.25f))
            .Insert(0, GetFormById(id).Obj.GetComponent<CanvasGroup>().DOFade(1, 0.25f).From(0));
        return outSequence;
    }

    private void SetActiveForm(string id)
    {
        activeForm = GetFormById(id);
        activeForm.Obj.SetActive(true);
        activeForm.Obj.GetComponent<IForm>().InitializeForm();
    }

    public void WarmupForms()
    {
        foreach (var form in forms)
        {
            form.Obj.SetActive(true);
            form.Obj.SetActive(false);
        }
    }

    public Form GetFormById(string id) => forms.Where(f => f.Id == id).First();

    public void ChangeBackgroundColor()
    {
        for (int i = 0; i < 6; i++)
        {
            if (activeForm.Id == QuizForms[i])
            {
               // backgroundColor.color = quiz.Color;
                break;
            }
            else
            {
                backgroundColor.color = new Color((float)32.0, (float)36.0, (float)66.0, (float)255.0);
            }
        }
        
        
    }
}
