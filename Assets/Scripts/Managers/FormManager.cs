using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormManager : MonoBehaviour
{
    public static FormManager Instance;

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
    private Form activeForm;

    public void ChangeForm(string id)
    {
        if (activeForm != null)
            activeForm.Obj.SetActive(false);

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
}
