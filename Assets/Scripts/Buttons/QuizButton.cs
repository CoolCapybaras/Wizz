using DG.Tweening;
using Net.Packets.Serverbound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizButton : MonoBehaviour
{
    public int quizId;
    private GameObject content;
    private float startYPos;

    private void Awake()
    {
        content = transform.GetChild(1).gameObject;
        startYPos = content.transform.localPosition.y;
    }
    public void OnPressed()
    {
        LocalClient.instance.SendPacket(new CreateLobbyPacket() { QuizId = quizId });
    }

    public void OnHover()
    {
        content.transform.DOLocalMoveY(-25, 0.25f);
    }

    public void OnExitHover()
    {
        content.transform.DOLocalMoveY(startYPos, 0.25f);
    }
}
