using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormManager : MonoBehaviour
{
    public static FormManager Instance;

    private void Awake()
    {
        activeForm = GetFormById("login");
        Instance = this;
    }

    public List<Form> forms;
    private Form activeForm;

    public void ChangeForm(string id)
    {
        activeForm.Obj.SetActive(false);
        activeForm = GetFormById(id);
        activeForm.Obj.SetActive(true);
        activeForm.Obj.GetComponent<IForm>().InitializeForm();
    }

    public Form GetFormById(string id) => forms.Where(f => f.Id == id).First();
}
