using Net.Packets.Clientbound;
using Net.Packets.Serverbound;
using System.Collections;
using System.Xml.Linq;
using TMPro;
using UnityEditor.Sprites;
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
    }

    public void OnAnonLoginPressed()
    {
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
            OnAuthSuccessful(packet.Name, packet.Id);
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

    private void OnAuthSuccessful(string name, int id)
    {
        LocalClient.instance.Name = name;
        LocalClient.instance.Id = id;
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
