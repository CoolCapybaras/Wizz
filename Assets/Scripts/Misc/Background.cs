using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    private RawImage image;
    private Vector2 offset = new Vector2(0.01f, 0.01f);
    private Vector2 startPosition = new Vector2(0.35f, 0.35f);
    private void Start()
    {
        image = GetComponent<RawImage>();
        image.uvRect = new Rect(startPosition, image.uvRect.size);
    }

    private void Update()
    {
        image.uvRect = new Rect(image.uvRect.position + offset * Time.deltaTime, image.uvRect.size);
    }
}
