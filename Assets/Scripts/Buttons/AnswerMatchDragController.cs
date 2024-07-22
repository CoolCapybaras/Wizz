using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnswerMatchDragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public int answerIndex;
    
    private GameObject mainContent;
    private Vector3 currentPosition;
    private RectTransform rectTransform;
    private int totalChild;

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform = GetComponent<RectTransform>();
        currentPosition = transform.localPosition;
        mainContent = transform.parent.gameObject;
        totalChild = mainContent.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition =
            new Vector3(transform.localPosition.x, eventData.position.y - 337, 0);
        for (int i = 0; i < totalChild; i++)
        {
            if (i != transform.GetSiblingIndex())
            {
                Transform otherTransform = mainContent.transform.GetChild(i);
                int distance = (int) Vector3.Distance(transform.localPosition,
                    otherTransform.localPosition);
                if (distance <= 10)
                {
                    Vector3 otherTransformOldPosition = otherTransform.localPosition;
                    otherTransform.localPosition = new Vector3(otherTransform.localPosition.x, currentPosition.y,
                        0);
                    transform.localPosition = new Vector3(transform.localPosition.x, otherTransformOldPosition.y,
                       0);
                    transform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                    currentPosition = transform.localPosition;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localPosition = currentPosition;
    }
}
