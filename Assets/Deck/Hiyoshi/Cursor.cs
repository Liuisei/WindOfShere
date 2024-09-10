using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    void Update()
    {
        this.GameObject().GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }
}
