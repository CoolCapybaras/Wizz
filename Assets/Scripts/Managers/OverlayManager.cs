using Net.Packets.Serverbound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        gameManager = GameManager.Instance;
    }

    [Serializable]
    public struct Form
    {
        public Transform infoLayout;
        public GameObject infoPrefab;
        public Sprite[] infoSprites;

        public RawImage backgroundColor;

        public GameObject toMainMenuButton;
        public GameObject createNewGameButton;
        public GameObject profileButton;
        public GameObject logoutButton;
        public GameObject exitObj;
        public GameObject unExitObj;
        public GameObject histObj;
    }

    public Form form;
    private GameManager gameManager;
    
    public void ShowInfo(string text, InfoType type)
    {
        var obj = Instantiate(form.infoPrefab, form.infoLayout);
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(0).GetComponent<Image>().sprite = form.infoSprites[(int)type];
        
        SoundManager.Instance.PlayShortClip(type == InfoType.Success ? "success" : "fail");
    }

    public void SetActiveTopButtons(bool active)
    {
        form.toMainMenuButton.SetActive(active);
        form.createNewGameButton.SetActive(active);
        form.histObj.SetActive(active);
    }

    public void SetActiveBottomButtons(bool active)
    {
        form.profileButton.SetActive(active);
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

        MyQuizzes.Instance.SetQuizzesList(1);
        FormManager.Instance.ChangeForm("myquizzes");
    }

    public void EnsureLeaved()
    {
        GameManager.Instance.EnsureLeavedLobby();
        GameManager.Instance.EnsureLeavedStartedGame();
        ResetBackgroundColor();
    }

    public void ChangeFullscreen()
    {
        var mode = Screen.fullScreenMode == FullScreenMode.FullScreenWindow
            ? FullScreenMode.Windowed
            : FullScreenMode.FullScreenWindow;
        
        Screen.SetResolution(mode == FullScreenMode.FullScreenWindow
                ? Display.main.systemWidth
                : Display.main.systemWidth / 2,
            mode == FullScreenMode.FullScreenWindow 
                ? Display.main.systemHeight 
                : Display.main.systemHeight / 2, mode);
    }

    public void OnProfileEditPressed()
    {
        EnsureLeaved();

        FormManager.Instance.ChangeForm("profileedit");
    }

    public void OnExitPressed()
    {
        form.exitObj.SetActive(true);
        var sequence = DOTween.Sequence();
        sequence.Insert(0, form.unExitObj.GetComponent<CanvasGroup>().DOFade(1, 0.25f).From(0))
            .Insert(0, form.unExitObj.transform.DOScale(1, 0.25f).From(0))
            .Play();
    }
    
    public void ChangeBackgroundColor(HexColor color)
    {
        if ((int)color == 0)
            return;
        
        form.backgroundColor.color = Colors.GetColorFromInt((int)color);
    }

    public void ResetBackgroundColor()
    {
        form.backgroundColor.color = Colors.GetColorFromInt((int)HexColor.Default);
    }

    public void OnQuizHistoryPressed()
    {
        EnsureLeaved();
        FormManager.Instance.ChangeForm("myquizzes");
    }
}
