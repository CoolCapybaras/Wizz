using Net.Packets.Serverbound;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour, IForm
{
    public TMP_InputField inputField;

    private LocalClient localClient;

    // Start is called before the first frame update
    void Start()
    {
        localClient = LocalClient.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    int counter = 0;

    public void OnClick()
    {
        if (counter == 0)
        {
            localClient.SendPacket(new AuthPacket()
            {
                Type = 0,
                Name = inputField.text
            });
        }
        else if (counter == 1)
        {
            localClient.SendPacket(new CreateLobbyPacket()
            {
                QuizId = "philosophy"
            });
        }
        else if (counter == 2)
        {
            localClient.SendPacket(new StartGamePacket());
        }

        counter++;
    }

    public void InitializeForm()
    {
    }
}
