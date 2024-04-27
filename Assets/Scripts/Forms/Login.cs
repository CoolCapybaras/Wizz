using Net.Packets.Clientbound;
using Net.Packets.Serverbound;
using System.Xml.Linq;
using TMPro;
using UnityEditor.PackageManager;
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
            Type = 0,
            Name = inputField.text
        });
    }

    public void OnLoginSuccess(AuthSuccessPacket packet)
    {
        Debug.Log($"{packet.ClientId} {packet.Name}");
        LocalClient.instance.Name = packet.Name;
        LocalClient.instance.Id = packet.Id;
        LocalClient.instance.Authorized = true;
        FormManager.Instance.ChangeForm("mainmenu");

        OverlayManager.Instance.SetActiveBottomButtons(true);
        OverlayManager.Instance.SetActiveTopButtons(true);
    }

    public void InitializeForm()
    {
        inputField.text = string.Empty;
    }
}
