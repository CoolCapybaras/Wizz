using Net.Packets.Serverbound;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InfoType
{
    Error,
    Success
}

public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Serializable]
    public struct Form
    {
        public Transform infoLayout;
        public GameObject infoPrefab;
        public Sprite[] infoSprites;

        public GameObject toMainMenuButton;
        public GameObject createNewGameButton;
        public GameObject profileButton;
        public GameObject logoutButton;
    }

    public Form form;

    public void ShowInfo(string text, InfoType type)
    {
        var obj = Instantiate(form.infoPrefab, form.infoLayout);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<Image>().sprite = form.infoSprites[(int)type];
        
        SoundManager.Instance.PlayShortClip(type == InfoType.Success ? "success" : "fail");
    }

    public void SetActiveTopButtons(bool active)
    {
        form.toMainMenuButton.SetActive(active);
        form.createNewGameButton.SetActive(active);
    }

    public void SetActiveBottomButtons(bool active)
    {
        form.profileButton.SetActive(active);
        form.logoutButton.SetActive(active);
    }

    public void OnMainMenuPressed()
    {
        EnsureLeaved();

        FormManager.Instance.ChangeForm("mainmenu", FormManager.AnimType.Out);
    }

    public void OnLogoutPressed()
    {
        EnsureLeaved();

        LocalClient.instance.SendPacket(new LogoutPacket());
        FormManager.Instance.ChangeForm("login", FormManager.AnimType.Out);

        if (PlayerPrefs.HasKey("token"))
        {
            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.Save();
        }
        
        LocalClient.instance.Authorized = false;
        LocalClient.instance.Name = null;
        LocalClient.instance.Image = null;

        SetActiveBottomButtons(false);
        SetActiveTopButtons(false);
    }

    public void OnNewQuizPressed()
    {
        EnsureLeaved();

        MyQuizzes.Instance.SetQuizzesList(0);
        FormManager.Instance.ChangeForm("myquizzes");
    }

    public void EnsureLeaved()
    {
        GameManager.Instance.EnsureLeavedLobby();
        GameManager.Instance.EnsureLeavedStartedGame();
    }

    public void OnProfileEditPressed()
    {
        GameManager.Instance.EnsureLeavedLobby();
        GameManager.Instance.EnsureLeavedStartedGame();

        FormManager.Instance.ChangeForm("profileedit");
    }
}
