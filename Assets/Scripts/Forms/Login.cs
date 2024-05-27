using Net.Packets.Clientbound;
using Net.Packets.Serverbound;
using System.Collections;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour, IForm
{
    public static Login Instance;
    public TMP_InputField inputField;
    private LocalClient localClient;

    private void Start()
    {
        Instance = this;
        localClient = LocalClient.instance;
        SetActiveFields(false);
        SoundManager.Instance.PlayMusic("menu", true);
    }

    public void OnAnonLoginPressed()
    {
        if (!UpdateProfilePacket.NameRegex.IsMatch(inputField.text))
        {
            OverlayManager.Instance.ShowInfo("Имя должно быть от 3 до 24 символов и содержать только буквы или цифры", InfoType.Error);
            return;
        }
        
        localClient.SendPacket(new AuthPacket()
        {
            Type = AuthType.Anonymous,
            Name = inputField.text
        });
    }

    public void OnVkLoginPressed()
    {
        localClient.SendPacket(new AuthPacket()
        {
            Type = AuthType.VK
        });
    }

    public void OnTelegramLoginPressed()
    {
        localClient.SendPacket(new AuthPacket()
        {
            Type = AuthType.Telegram
        });
    }

    public void OnAuthResult(AuthResultPacket packet)
    {
        if (packet.Flags.HasFlag(AuthResultFlags.Ok))
        {
            Debug.Log("Authresult OK");
            OnAuthSuccessful(packet.Name, packet.ClientId, packet.Image);
        }

        if (packet.Flags.HasFlag(AuthResultFlags.HasToken))
        {
            SaveToken(packet.Token);
        }

        if (packet.Flags.HasFlag(AuthResultFlags.HasUrl))
        {
            OpenURL(packet.Url);
        }
    }

    private void OnAuthSuccessful(string name, int id, ByteImage image)
    {
        LocalClient.instance.Name = name;
        LocalClient.instance.Id = id;
        LocalClient.instance.Image = image;
        LocalClient.instance.Authorized = true;
        FormManager.Instance.ChangeForm("mainmenu");

        OverlayManager.Instance.SetActiveBottomButtons(true);
        OverlayManager.Instance.SetActiveTopButtons(true);
    }

    public void OnClientStarted()
    {
        if (PlayerPrefs.HasKey("token"))
            LocalClient.instance.SendPacket(new AuthPacket()
            {
                Type = AuthType.Token,
                Token = PlayerPrefs.GetString("token")
            });
        SetActiveFields(true);
    }

    private void SetActiveFields(bool active)
    {
        inputField.interactable = active;
    }

    private void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    private void SaveToken(string token)
    {
        PlayerPrefs.SetString("token", token);
        PlayerPrefs.Save();
    }

    public void InitializeForm()
    {
        inputField.text = string.Empty;
    }
}
