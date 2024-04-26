using Net.Packets.Serverbound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizButton : MonoBehaviour
{
    public string quizId;

    public void OnPressed()
    {
        LocalClient.instance.SendPacket(new CreateLobbyPacket() { QuizId = quizId });
    }
}
