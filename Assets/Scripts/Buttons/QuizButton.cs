using System;
using DG.Tweening;
using Net.Packets.Serverbound;
using System.Collections;
using System.Collections.Generic;
using Net.Packets;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuizButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        LocalClient.instance.SendPacket(new CreateLobbyPacket { QuizId = quizId });
    }

    public void OnPublishPressed()
    {
        LocalClient.instance.SendPacket(new EditQuizPacket {QuizId = quizId, Type = EditQuizType.Publish});
    }

    public void OnEditPressed()
    {
        QuizEditor.Instance.OnEditQuizPressed(quizId);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        content.transform.DOLocalMoveY(-25, 0.25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        content.transform.DOLocalMoveY(startYPos, 0.25f);
    }
}
