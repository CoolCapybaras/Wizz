using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers : MonoBehaviour
{
    public void RemoveAllChilds(Transform transform)
    {
        var count = transform.childCount;
        for (int i = 0; i < count; ++i)
            Destroy(transform.GetChild(i).gameObject);
    }
}
